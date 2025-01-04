namespace ChessAI.Tests
{
    using ChessAI.Core;
    using ChessAI.Pieces;
    using System.Collections.Generic;
    using UnityEngine;

    public static class Perft
    {
        public static long PerftTest(Board board, int depth, bool isWhiteTurn)
        {
            if (depth == 0)
            {
                return 1;
            }

            long nodes = 0;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int position = new(x, y);
                    int piece = board.GetPieceAt(position);

                    if ((isWhiteTurn && Piece.IsColor(piece, Piece.White)) ||
                        (!isWhiteTurn && Piece.IsColor(piece, Piece.Black)))
                    {
                        List<Vector2Int> validMoves = MoveValidator.GetValidMovesForPiece(position, board, piece);
                        foreach (Vector2Int move in validMoves)
                        {
                            if (Piece.PieceType(piece) == Piece.Pawn &&
                                (move.y == 7 || move.y == 0)) // Check for promotion
                            {
                                foreach (int promotionType in new[] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight })
                                {
                                    board.MovePiece(position, move, promotionType);
                                    nodes += PerftTest(board, depth - 1, !isWhiteTurn);
                                    board.UndoMove();
                                }
                            }
                            else
                            {
                                board.MovePiece(position, move);
                                nodes += PerftTest(board, depth - 1, !isWhiteTurn);
                                board.UndoMove();
                            }
                        }
                    }
                }
            }

            return nodes;
        }

        public static Dictionary<string, long> PerftDivide(Board board, int depth, bool isWhiteTurn)
        {
            if (depth == 0)
            {
                return new();
            }

            Dictionary<string, long> results = new();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int position = new(x, y);
                    int piece = board.GetPieceAt(position);

                    if ((isWhiteTurn && Piece.IsColor(piece, Piece.White)) ||
                        (!isWhiteTurn && Piece.IsColor(piece, Piece.Black)))
                    {
                        List<Vector2Int> validMoves = MoveValidator.GetValidMovesForPiece(position, board, piece);
                        foreach (Vector2Int move in validMoves)
                        {

                            if (Piece.PieceType(piece) == Piece.Pawn &&
                                (move.y == 7 || move.y == 0)) // Check for promotion
                            {
                                foreach (int promotionType in new[] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight })
                                {
                                    board.MovePiece(position, move, promotionType);

                                    string moveNotation = $"{(char)(position.x + 'a')}{position.y + 1}{(char)(move.x + 'a')}{move.y + 1}={PromotionTypeToChar(promotionType)}";
                                    long nodes = PerftTest(board, depth - 1, !isWhiteTurn);
                                    board.UndoMove();

                                    if (results.ContainsKey(moveNotation))
                                    {
                                        results[moveNotation] += nodes;
                                    }
                                    else
                                    {
                                        results[moveNotation] = nodes;
                                    }
                                }
                            }
                            else
                            {
                                board.MovePiece(position, move);

                                string moveNotation = $"{(char)(position.x + 'a')}{position.y + 1}{(char)(move.x + 'a')}{move.y + 1}";
                                long nodes = PerftTest(board, depth - 1, !isWhiteTurn);
                                board.UndoMove();

                                if (results.ContainsKey(moveNotation))
                                {
                                    results[moveNotation] += nodes;
                                }
                                else
                                {
                                    results[moveNotation] = nodes;
                                }
                            }
                        }
                    }
                }
            }

            return results;
        }

        public static char PromotionTypeToChar(int promotionType)
        {
            return promotionType switch
            {
                Piece.Queen => 'Q',
                Piece.Rook => 'R',
                Piece.Bishop => 'B',
                Piece.Knight => 'N',
                _ => ' ',
            };
        }
    }
}
