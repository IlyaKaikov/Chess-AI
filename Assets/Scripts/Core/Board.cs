namespace ChessAI.Core
{
    using ChessAI.Pieces;
    using ChessAI.Tests;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class Board
    {
        private const int BoardSize = 8;

        private const int whiteCastleShortMask = 0b0001;
        private const int whiteCastleLongMask = 0b0010;
        private const int blackCastleShortMask = 0b0100;
        private const int blackCastleLongMask = 0b1000;

        private readonly int[] grid;

        private int castlingRights = 15;
        private bool gameFinished = true;
        private int enPassantRow = -1;
        private readonly Stack<MoveState> moveHistory = new();

        public struct MoveState
        {
            public Vector2Int from;
            public Vector2Int to;
            public int pieceMoved;
            public int pieceCaptured;
            public int castlingRights;
            public int enPassantRow;
        }

        public Board()
        {
            grid = new int[BoardSize * BoardSize];
        }
        public Board(Board other)
        {
            this.grid = (int[])other.grid.Clone();
            this.castlingRights = other.castlingRights;
            this.gameFinished = other.gameFinished;
            this.enPassantRow = other.enPassantRow;
            this.moveHistory = new(other.moveHistory.Reverse());
        }

        public void StartGame()
        {
            gameFinished = false;
        }

        public void EndGame()
        {
            gameFinished = true;
        }

        public bool IsGameFinished()
        {
            return gameFinished;
        }

        public void SetEnPassantRow(int x)
        {
            enPassantRow = x;
        }

        public int GetEnPassantRow()
        {
            return enPassantRow;
        }

        public int GetIndex(int x, int y)
        {
            Debug.Assert(x >= 0 && x < BoardSize && y >= 0 && y < BoardSize, "Index out of bounds!");
            return x + (y * BoardSize);
        }

        public int GetIndex(Vector2Int position)
        {
            return GetIndex(position.x, position.y);
        }

        public void LoadFromFEN(string fen)
        {
            string[] parts = fen.Split(' ');
            if (parts.Length < 4)
            {
                throw new ArgumentException("Invalid FEN string");
            }

            string piecePlacement = parts[0];
            string activeColor = parts[1];
            string castlingAvailability = parts[2];
            string enPassant = parts[3];

            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = Piece.None;
            }

            int rank = 7;
            int file = 0;
            foreach (char c in piecePlacement)
            {
                if (c == '/')
                {
                    rank--;
                    file = 0;
                }
                else if (char.IsDigit(c))
                {
                    file += c - '0';
                }
                else
                {
                    int piece = Piece.None;
                    bool isWhite = char.IsUpper(c);
                    char lower = char.ToLower(c);

                    if (lower == 'p') piece = Piece.Pawn;
                    else if (lower == 'r') piece = Piece.Rook;
                    else if (lower == 'n') piece = Piece.Knight;
                    else if (lower == 'b') piece = Piece.Bishop;
                    else if (lower == 'q') piece = Piece.Queen;
                    else if (lower == 'k') piece = Piece.King;

                    piece |= isWhite ? Piece.White : Piece.Black;

                    PlacePiece(piece, new Vector2Int(file, rank));
                    file++;
                }
            }

            GameManager.Instance.isWhiteTurn = activeColor == "w";

            castlingRights = 0;
            if (castlingAvailability.Contains("K")) castlingRights |= whiteCastleShortMask;
            if (castlingAvailability.Contains("Q")) castlingRights |= whiteCastleLongMask;
            if (castlingAvailability.Contains("k")) castlingRights |= blackCastleShortMask;
            if (castlingAvailability.Contains("q")) castlingRights |= blackCastleLongMask;

            if (enPassant != "-")
            {
                int epFile = enPassant[0] - 'a';
                //int epRank = int.Parse(enPassant[1].ToString()) - 1;
                enPassantRow = epFile;
            }
            else
            {
                enPassantRow = -1;
            }
        }

        public bool IsInBounds(Vector2Int position)
        {
            return position.x >= 0 && position.x < BoardSize && position.y >= 0 && position.y < BoardSize;
        }

        public int GetPieceAt(Vector2Int position)
        {
            return grid[GetIndex(position)];
        }

        public void PlacePiece(int piece, Vector2Int position)
        {
            if (IsInBounds(position))
                grid[GetIndex(position)] = piece;
        }

        public void RemovePiece(Vector2Int position)
        {
            if (IsInBounds(position))
                grid[GetIndex(position)] = Piece.None;
        }

        public void MovePiece(Vector2Int from, Vector2Int to, int promotion = 0)
        {
            bool handled = false;
            if (!(IsInBounds(from) && IsInBounds(to)))
            {
                Debug.Log("Out of bounds!");
                return;
            }

            int pieceMoved = grid[GetIndex(from)];
            int pieceCaptured = grid[GetIndex(to)];
            moveHistory.Push(new MoveState
            {
                from = from,
                to = to,
                pieceMoved = pieceMoved,
                pieceCaptured = pieceCaptured,
                castlingRights = castlingRights,
                enPassantRow = enPassantRow
            });

            FlagUpdater.UpdateCastlingFlags(this, from, to, pieceMoved, Piece.IsColor(pieceMoved, Piece.White));

            // handle special case - pawn promotion
            if (promotion != 0)
            {
                HandlePawnPromotion(from, to, promotion);
                handled = true;
            }

            // handle special case - castle
            if (!handled && Piece.PieceType(grid[GetIndex(from)]) == Piece.King)
            {
                if (HandleCastle(from, to)) handled = true;
            }

            //handle special case - en passant
            if (!handled && Piece.PieceType(grid[GetIndex(from)]) == Piece.Pawn && Piece.PieceType(grid[GetIndex(to)]) == Piece.None)
            {
                if (HandleEnPassant(from, to)) handled = true;
            }
            if (!handled)
            {
                grid[GetIndex(to)] = grid[GetIndex(from)];
                grid[GetIndex(from)] = Piece.None;
                FlagUpdater.UpdateEnPassantFlags(this, pieceMoved, from, to);
                return;
            }

            FlagUpdater.UpdateEnPassantFlags(this, pieceMoved, from, to);
        }

        private bool HandleCastle(Vector2Int from, Vector2Int to)
        {
            bool isCastle = false;

            //white castle short
            if (from.x == 4 && from.y == 0 && to.x == 6 && to.y == 0)
            {
                isCastle = true;
                grid[GetIndex(to)] = grid[GetIndex(from)];
                grid[GetIndex(from)] = Piece.None;
                grid[GetIndex(5, 0)] = Piece.Rook | Piece.White;
                grid[GetIndex(7, 0)] = Piece.None;
            }
            //white castle long
            else if (from.x == 4 && from.y == 0 && to.x == 2 && to.y == 0)
            {
                isCastle = true;
                grid[GetIndex(to)] = grid[GetIndex(from)];
                grid[GetIndex(from)] = Piece.None;
                grid[GetIndex(3, 0)] = Piece.Rook | Piece.White;
                grid[GetIndex(0, 0)] = Piece.None;
            }
            //black castle short
            else if (from.x == 4 && from.y == 7 && to.x == 6 && to.y == 7)
            {
                isCastle = true;
                grid[GetIndex(to)] = grid[GetIndex(from)];
                grid[GetIndex(from)] = Piece.None;
                grid[GetIndex(5, 7)] = Piece.Rook | Piece.Black;
                grid[GetIndex(7, 7)] = Piece.None;
            }
            //black castle long
            else if (from.x == 4 && from.y == 7 && to.x == 2 && to.y == 7)
            {
                isCastle = true;
                grid[GetIndex(to)] = grid[GetIndex(from)];
                grid[GetIndex(from)] = Piece.None;
                grid[GetIndex(3, 7)] = Piece.Rook | Piece.Black;
                grid[GetIndex(0, 7)] = Piece.None;
            }
            return isCastle;
        }

        private bool HandleEnPassant(Vector2Int from, Vector2Int to)
        {
            bool isEnPassant = false;
            // white pawn capturing
            if (from.y == 4 && to.y == 5 && ((from.x - to.x) == 1 || (from.x - to.x) == -1) &&
                GetPieceAt(new(to.x, from.y)) == (Piece.Pawn | Piece.Black))
            {
                isEnPassant = true;
                grid[GetIndex(to)] = grid[GetIndex(from)];
                grid[GetIndex(from)] = Piece.None;
                grid[GetIndex(new(to.x, from.y))] = Piece.None;
            }
            // black pawn capturing
            else if (from.y == 3 && to.y == 2 && ((from.x - to.x) == 1 || (from.x - to.x) == -1) &&
                    GetPieceAt(new(to.x, from.y)) == (Piece.Pawn | Piece.White))
            {
                isEnPassant = true;
                grid[GetIndex(to)] = grid[GetIndex(from)];
                grid[GetIndex(from)] = Piece.None;
                grid[GetIndex(new(to.x, from.y))] = Piece.None;
            }
            return isEnPassant;
        }

        private void HandlePawnPromotion(Vector2Int from, Vector2Int to, int promotion)
        {
            int color = Piece.Color(grid[GetIndex(from)]);
            grid[GetIndex(from)] = Piece.None;
            grid[GetIndex(to)] = promotion switch
            {
                Piece.Knight => (Piece.Knight | color),
                Piece.Bishop => (Piece.Bishop | color),
                Piece.Rook => (Piece.Rook | color),
                Piece.Queen => (Piece.Queen | color),
                _ => (Piece.Pawn | color),
            };
        }

        public bool IsEmpty(Vector2Int position)
        {
            return IsInBounds(position) && grid[GetIndex(position)] == Piece.None;
        }

        public bool IsEnemyPiece(Vector2Int position, int color)
        {
            if (!IsInBounds(position)) return false;
            int piece = grid[GetIndex(position)];
            return piece != Piece.None && !Piece.IsColor(piece, color);
        }

        //castling-related methods
        public bool WhiteAllowedToCastleShort()
        {
            return (castlingRights & whiteCastleShortMask) != 0;
        }

        public bool WhiteAllowedToCastleLong()
        {
            return (castlingRights & whiteCastleLongMask) != 0;
        }
        public bool BlackAllowedToCastleShort()
        {
            return (castlingRights & blackCastleShortMask) != 0;
        }

        public bool BlackAllowedToCastleLong()
        {
            return (castlingRights & blackCastleLongMask) != 0;
        }

        public void WhiteDisallowToCastleShort()
        {
            castlingRights &= ~whiteCastleShortMask;
        }

        public void WhiteDisallowToCastleLong()
        {
            castlingRights &= ~whiteCastleLongMask;
        }
        public void BlackDisallowToCastleShort()
        {
            castlingRights &= ~blackCastleShortMask;
        }

        public void BlackDisallowToCastleLong()
        {
            castlingRights &= ~blackCastleLongMask;
        }

        public void UndoMove()
        {
            if (moveHistory.Count == 0) return;
            MoveState lastMove = moveHistory.Pop();
            grid[GetIndex(lastMove.from)] = lastMove.pieceMoved;
            grid[GetIndex(lastMove.to)] = lastMove.pieceCaptured;
            castlingRights = lastMove.castlingRights;
            enPassantRow = lastMove.enPassantRow;

            if (Piece.PieceType(lastMove.pieceMoved) == Piece.King && UndoCastle(lastMove.from, lastMove.to)) return;
            if (Piece.PieceType(lastMove.pieceMoved) == Piece.Pawn && UndoEnPassant(lastMove.from, lastMove.to, lastMove.pieceCaptured)) return;
        }

        private bool UndoCastle(Vector2Int from, Vector2Int to)
        {
            bool isCastle = false;

            //white castle short
            if (from.x == 4 && from.y == 0 && to.x == 6 && to.y == 0)
            {
                isCastle = true;
                grid[GetIndex(5, 0)] = Piece.None;
                grid[GetIndex(7, 0)] = Piece.Rook | Piece.White;
            }
            //white castle long
            else if (from.x == 4 && from.y == 0 && to.x == 2 && to.y == 0)
            {
                isCastle = true;
                grid[GetIndex(3, 0)] = Piece.None;
                grid[GetIndex(0, 0)] = Piece.Rook | Piece.White;
            }
            //black castle short
            else if (from.x == 4 && from.y == 7 && to.x == 6 && to.y == 7)
            {
                isCastle = true;
                grid[GetIndex(5, 7)] = Piece.None;
                grid[GetIndex(7, 7)] = Piece.Rook | Piece.Black;
            }
            //black castle long
            else if (from.x == 4 && from.y == 7 && to.x == 2 && to.y == 7)
            {
                isCastle = true;
                grid[GetIndex(3, 7)] = Piece.None;
                grid[GetIndex(0, 7)] = Piece.Rook | Piece.Black;
            }
            return isCastle;
        }

        private bool UndoEnPassant(Vector2Int from, Vector2Int to, int pieceCaptured)
        {
            bool isEnPassant = false;
            // white pawn capturing
            if (from.y == 4 && to.y == 5 && ((from.x - to.x) == 1 || (from.x - to.x) == -1) &&
                pieceCaptured == Piece.None)
            {
                isEnPassant = true;
                grid[GetIndex(new(to.x, from.y))] = Piece.Pawn | Piece.Black;
            }
            // black pawn capturing
            else if (from.y == 3 && to.y == 2 && ((from.x - to.x) == 1 || (from.x - to.x) == -1) &&
                    pieceCaptured == Piece.None)
            {
                isEnPassant = true;
                grid[GetIndex(new(to.x, from.y))] = Piece.Pawn | Piece.White;
            }
            return isEnPassant;
        }
    }
}
