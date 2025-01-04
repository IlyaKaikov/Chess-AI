namespace ChessAI.Core
{
    using System.Collections.Generic;
    using UnityEngine;
    using ChessAI.Pieces;

    public class MoveValidator
    {
        private static List<Vector2Int> GetValidMovesForSlidingPiece(Vector2Int position, Board board, int piece, Vector2Int[] directions)
        {
            List<Vector2Int> validMoves = new();
            foreach (var direction in directions)
            {
                Vector2Int current = position;
                while (true)
                {
                    current += direction;
                    if (!board.IsInBounds(current)) break;
                    int targetPiece = board.GetPieceAt(current);
                    if (Piece.IsColor(targetPiece, Piece.Color(piece))) break;

                    bool isWhiteTurn = (Piece.Color(piece) == Piece.White);
                    if (Piece.PieceType(targetPiece) == Piece.None)
                    {
                        if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                        validMoves.Add(current);
                    }
                    else
                    {
                        if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) break;
                        validMoves.Add(current);
                        break;
                    }
                }
            }
            return validMoves;
        }

        private static List<Vector2Int> GetValidMovesForKnight(Vector2Int position, Board board, int piece, Vector2Int[] directions)
        {
            List<Vector2Int> validMoves = new();
            foreach (var direction in directions)
            {
                Vector2Int current = position;
                current += direction;
                if (!board.IsInBounds(current)) continue;
                int targetPiece = board.GetPieceAt(current);

                if (Piece.IsColor(targetPiece, Piece.Color(piece))) continue;

                bool isWhiteTurn = (Piece.Color(piece) == Piece.White);
                if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                validMoves.Add(current);
            }
            return validMoves;
        }

        private static List<Vector2Int> GetValidMovesForKing(Vector2Int position, Board board, int piece, Vector2Int[] directions)
        {
            List<Vector2Int> validMoves = new();
            bool isWhiteTurn = (Piece.Color(piece) == Piece.White);

            foreach (var direction in directions)
            {
                Vector2Int current = position;
                current += direction;
                if (!board.IsInBounds(current)) continue;
                int targetPiece = board.GetPieceAt(current);

                if (targetPiece == Piece.None)
                {
                    if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                    validMoves.Add(current);
                }
                else if (!Piece.IsColor(targetPiece, Piece.Color(piece)))
                {
                    if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                    validMoves.Add(current);
                }
            }

            ulong squaresUnderAttack = SquaresUnderAttack(board, isWhiteTurn);

            // white castle short
            if (isWhiteTurn && board.WhiteAllowedToCastleShort())
            {
                int bitIndex1 = 4;
                Vector2Int square2 = new(5, 0);
                int bitIndex2 = 5;
                Vector2Int square3 = new(6, 0);
                int bitIndex3 = 6;

                if ((squaresUnderAttack & (1UL << bitIndex1)) == 0 &&
                    Piece.PieceType(board.GetPieceAt(square2)) == Piece.None && (squaresUnderAttack & (1UL << bitIndex2)) == 0 &&
                    Piece.PieceType(board.GetPieceAt(square3)) == Piece.None && (squaresUnderAttack & (1UL << bitIndex3)) == 0)
                {
                    validMoves.Add(square3);
                }
            }

            // white castle long
            if (isWhiteTurn && board.WhiteAllowedToCastleLong())
            {
                int bitIndex1 = 4;
                Vector2Int square2 = new(3, 0);
                int bitIndex2 = 3;
                Vector2Int square3 = new(2, 0);
                int bitIndex3 = 2;
                Vector2Int square4 = new(1, 0);

                if ((squaresUnderAttack & (1UL << bitIndex1)) == 0 &&
                    Piece.PieceType(board.GetPieceAt(square2)) == Piece.None && (squaresUnderAttack & (1UL << bitIndex2)) == 0  &&
                    Piece.PieceType(board.GetPieceAt(square3)) == Piece.None && (squaresUnderAttack & (1UL << bitIndex3)) == 0 &&
                    Piece.PieceType(board.GetPieceAt(square4)) == Piece.None)
                {
                    validMoves.Add(square3);
                }
            }

            // black castle short
            if ((!isWhiteTurn) && board.BlackAllowedToCastleShort())
            {
                int bitIndex1 = 4 + 7 * 8;
                Vector2Int square2 = new(5, 7);
                int bitIndex2 = 5 + 7 * 8;
                Vector2Int square3 = new(6, 7);
                int bitIndex3 = 6 + 7 * 8;

                if ((squaresUnderAttack & (1UL << bitIndex1)) == 0 &&
                    Piece.PieceType(board.GetPieceAt(square2)) == Piece.None && (squaresUnderAttack & (1UL << bitIndex2)) == 0 &&
                    Piece.PieceType(board.GetPieceAt(square3)) == Piece.None && (squaresUnderAttack & (1UL << bitIndex3)) == 0)
                {
                    validMoves.Add(square3);
                }
            }

            // black castle long
            if ((!isWhiteTurn) && board.BlackAllowedToCastleLong())
            {
                int bitIndex1 = 4 + 7 * 8;
                Vector2Int square2 = new(3, 7);
                int bitIndex2 = 3 + 7 * 8;
                Vector2Int square3 = new(2, 7);
                int bitIndex3 = 2 + 7 * 8;
                Vector2Int square4 = new(1, 7);

                if ((squaresUnderAttack & (1UL << bitIndex1)) == 0 &&
                    Piece.PieceType(board.GetPieceAt(square2)) == Piece.None && (squaresUnderAttack & (1UL << bitIndex2)) == 0 &&
                    Piece.PieceType(board.GetPieceAt(square3)) == Piece.None && (squaresUnderAttack & (1UL << bitIndex3)) == 0 &&
                    Piece.PieceType(board.GetPieceAt(square4)) == Piece.None)
                {
                    validMoves.Add(square3);
                }
            }
            return validMoves;
        }

        private static List<Vector2Int> GetValidMovesForPawn(Vector2Int position, Board board, int piece, Vector2Int[] directions)
        {
            List<Vector2Int> validMoves = new();

            bool isWhiteTurn = (Piece.Color(piece) == Piece.White);
            foreach (var direction in directions)
            {
                Vector2Int current = position;
                current += direction;
                if (!board.IsInBounds(current)) continue;
                int targetPiece = board.GetPieceAt(current);

                if (isWhiteTurn)
                {
                    // double step
                    if (position.y == 1 && current.y == 3 &&
                        Piece.PieceType(board.GetPieceAt(new(position.x, 2))) == Piece.None &&
                        Piece.PieceType(targetPiece) == Piece.None)
                    {
                        if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                        validMoves.Add(current);
                    }
                    // single step
                    else if (current.y == (position.y + 1) && position.x == current.x &&
                            Piece.PieceType(targetPiece) == Piece.None)
                    {
                        if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                        validMoves.Add(current);
                    }
                    // piece capture
                    else if (current.y == (position.y + 1) &&
                            ((position.x - current.x) == 1 || (position.x - current.x) == -1) &&
                            Piece.PieceType(targetPiece) != Piece.None &&
                            Piece.Color(targetPiece) == Piece.Black)
                    {
                        if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                        validMoves.Add(current);
                    }
                    // en passant
                    else if (position.y == 4 && current.y == (position.y + 1) &&
                            ((position.x - current.x) == 1 || (position.x - current.x) == -1) &&
                            Piece.PieceType(targetPiece) == Piece.None &&
                            (board.GetEnPassantRow() == current.x))
                    {
                        if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                        validMoves.Add(current);
                    }
                }
                else
                {
                    // double step
                    if (position.y == 6 && current.y == 4 &&
                        Piece.PieceType(board.GetPieceAt(new(position.x, 5))) == Piece.None &&
                        Piece.PieceType(targetPiece) == Piece.None)
                    {
                        if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                        validMoves.Add(current);
                    }
                    // single step
                    else if (current.y == (position.y - 1) && position.x == current.x &&
                            Piece.PieceType(targetPiece) == Piece.None)
                    {
                        if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                        validMoves.Add(current);
                    }
                    // piece capture
                    else if (current.y == (position.y - 1) &&
                            ((position.x - current.x) == 1 || (position.x - current.x) == -1) &&
                            Piece.PieceType(targetPiece) != Piece.None &&
                            Piece.Color(targetPiece) == Piece.White)
                    {
                        if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                        validMoves.Add(current);
                    }
                    // en passant
                    else if (position.y == 3 && current.y == (position.y - 1) &&
                            ((position.x - current.x) == 1 || (position.x - current.x) == -1) &&
                            Piece.PieceType(targetPiece) == Piece.None &&
                            (board.GetEnPassantRow() == current.x))
                    {
                        if (!NextMoveIsNotCheck(board, isWhiteTurn, position, current)) continue;
                        validMoves.Add(current);
                    }
                }
            }
            return validMoves;
        }

        public static List<Vector2Int> GetValidMovesForPiece(Vector2Int position, Board board, int piece)
        {
            int pieceType = Piece.PieceType(piece);

            List<Vector2Int> validMoves = pieceType switch
            {
                Piece.Bishop => GetValidMovesForSlidingPiece(position, board, piece, new Vector2Int[]
                { new(1, 1), new(-1, 1), new(1, -1), new(-1, -1) }),

                Piece.Rook => GetValidMovesForSlidingPiece(position, board, piece, new Vector2Int[]
                { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) }),

                Piece.Queen => GetValidMovesForSlidingPiece(position, board, piece, new Vector2Int[]
                { new(1, 1), new(-1, 1), new(1, -1), new(-1, -1), new(1, 0), new(-1, 0), new(0, 1), new(0, -1) }),

                Piece.Knight => GetValidMovesForKnight(position, board, piece, new Vector2Int[]
                { new(1, 2), new(-1, 2), new(1, -2), new(-1, -2), new(2, 1), new(2, -1), new(-2, 1), new(-2, -1) }),

                Piece.King => GetValidMovesForKing(position, board, piece, new Vector2Int[]
                { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) }),

                Piece.Pawn => GetValidMovesForPawn(position, board, piece, new Vector2Int[]
                { new(0, 1), new(0, 2), new(1, 1), new(-1, 1), new(0, -1), new(0, -2), new(1, -1), new(-1, -1) }),

                _ => new(),
            };
            return validMoves;
        }

        public static List<Vector2Int> GetAttackingMovesForPiece(Vector2Int position, Board board, int piece)
        {
            int pieceType = Piece.PieceType(piece);

            List<Vector2Int> validMoves = pieceType switch
            {
                Piece.Bishop => GetAttackingMovesForSlidingPiece(position, board, new Vector2Int[]
                { new(1, 1), new(-1, 1), new(1, -1), new(-1, -1) }),

                Piece.Rook => GetAttackingMovesForSlidingPiece(position, board, new Vector2Int[]
                { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) }),

                Piece.Queen => GetAttackingMovesForSlidingPiece(position, board, new Vector2Int[]
                { new(1, 1), new(-1, 1), new(1, -1), new(-1, -1), new(1, 0), new(-1, 0), new(0, 1), new(0, -1) }),

                Piece.Knight => GetAttackingMovesForKnight(position, board, new Vector2Int[]
                { new(1, 2), new(-1, 2), new(1, -2), new(-1, -2), new(2, 1), new(2, -1), new(-2, 1), new(-2, -1) }),

                Piece.King => GetAttackingMovesForKing(position, board, new Vector2Int[]
                { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) }),

                Piece.Pawn => GetAttackingMovesForPawn(position, board, piece, new Vector2Int[]
                { new(1, 1), new(-1, 1), new(1, -1), new(-1, -1) }),

                _ => new(),
            };
            return validMoves;
        }

        private static List<Vector2Int> GetAttackingMovesForSlidingPiece(Vector2Int position, Board board, Vector2Int[] directions)
        {
            List<Vector2Int> attackingMoves = new();
            int slidingPieceColor = Piece.Color(board.GetPieceAt(position));
            foreach (var direction in directions)
            {
                Vector2Int current = position;
                while (true)
                {
                    current += direction;
                    if (!board.IsInBounds(current)) break;
                    int targetPiece = board.GetPieceAt(current);

                    if (Piece.PieceType(targetPiece) == Piece.None || (Piece.PieceType(targetPiece) == Piece.King && !Piece.IsColor(targetPiece , slidingPieceColor)))
                    {
                        attackingMoves.Add(current);
                    }
                    else
                    {
                        attackingMoves.Add(current);
                        break;
                    }
                }
            }
            return attackingMoves;
        }

        private static List<Vector2Int> GetAttackingMovesForKnight(Vector2Int position, Board board, Vector2Int[] directions)
        {
            List<Vector2Int> attackingMoves = new();
            foreach (var direction in directions)
            {
                Vector2Int current = position;
                current += direction;
                if (!board.IsInBounds(current)) continue;

                attackingMoves.Add(current);
            }
            return attackingMoves;
        }

        private static List<Vector2Int> GetAttackingMovesForKing(Vector2Int position, Board board, Vector2Int[] directions)
        {
            List<Vector2Int> attackingMoves = new();
            foreach (var direction in directions)
            {
                Vector2Int current = position;
                current += direction;
                if (!board.IsInBounds(current)) continue;

                attackingMoves.Add(current);
            }
            return attackingMoves;
        }
        private static List<Vector2Int> GetAttackingMovesForPawn(Vector2Int position, Board board, int piece, Vector2Int[] directions)
        {
            List<Vector2Int> attackingMoves = new();
            foreach (var direction in directions)
            {
                Vector2Int current = position;
                current += direction;
                if (!board.IsInBounds(current)) continue;
                if ((Piece.Color(piece) == Piece.White && position.y > current.y) || (Piece.Color(piece) == Piece.Black && position.y < current.y)) continue;

                attackingMoves.Add(current);
            }
            return attackingMoves;
        }
        public static ulong SquaresUnderAttack(Board board, bool isWhiteTurn)
        {
            ulong squaresUnderAttack = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int position = new(x, y);
                    int piece = board.GetPieceAt(position);
                    if (Piece.PieceType(piece) == Piece.None) continue;
                    if ((isWhiteTurn && Piece.Color(piece) == Piece.White) || ((!isWhiteTurn) && Piece.Color(piece) == Piece.Black)) continue;

                    List<Vector2Int> attackingMoves = GetAttackingMovesForPiece(position, board, piece);
                    foreach (var move in attackingMoves)
                    {
                        int bitIndex = move.x + move.y * 8;
                        squaresUnderAttack |= 1UL << bitIndex;
                    }
                }
            }
            return squaresUnderAttack;
        }

        public static bool IsKingInCheck(Board board, bool isWhiteTurn)
        {
            ulong squaresUnderAttack = SquaresUnderAttack(board, isWhiteTurn);
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if ((isWhiteTurn && board.GetPieceAt(new(x, y)) == (Piece.King | Piece.White)) || ((!isWhiteTurn) && board.GetPieceAt(new(x, y)) == (Piece.King | Piece.Black)))
                    {
                        int bitIndex = x + y * 8;
                        return (squaresUnderAttack & (1UL << bitIndex)) != 0;
                    }
                }
            }
            return false;
        }

        private static bool NextMoveIsNotCheck(Board board, bool isWhiteTurn, Vector2Int from, Vector2Int to)
        {
            board.MovePiece(from, to);
            bool escapedCheck = !IsKingInCheck(board, isWhiteTurn);
            board.UndoMove();
            return escapedCheck;
        }

        public static bool NoLegalMovesLeft(Board board, bool isWhiteTurn)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int position = new(x, y);
                    int piece = board.GetPieceAt(position);
                    if ((isWhiteTurn && Piece.Color(piece) == Piece.White) || ((!isWhiteTurn) && Piece.Color(piece) == Piece.Black))
                    {
                        List<Vector2Int> validMoves = GetValidMovesForPiece(position, board, piece);
                        if (validMoves.Count > 0) return false;
                    }
                }
            }
            return true;
        }

        public static List<Vector2Int> GetAllValidMoves(Board board, bool isWhiteTurn)
        {
            List<Vector2Int> validMoves = new();
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int position = new(x, y);
                    int piece = board.GetPieceAt(position);
                    if (Piece.PieceType(piece) == Piece.None) continue;
                    if ((isWhiteTurn && Piece.Color(piece) != Piece.White) || ((!isWhiteTurn) && Piece.Color(piece) != Piece.Black)) continue;

                    List<Vector2Int> attackingMoves = GetValidMovesForPiece(position, board, piece);
                    foreach (var move in attackingMoves)
                    {
                        validMoves.Add(move);
                    }
                }
            }
            return validMoves;
        }
    }
}
