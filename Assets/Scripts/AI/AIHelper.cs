using ChessAI.Core;
using ChessAI.Pieces;
using System.Collections.Generic;
using UnityEngine;

namespace ChessAI.AI
{
    public static class AIHelper
    {
        private static List<(Vector2Int from, Vector2Int to, int promotion)> GetAllPossibleMoves(Board board, bool isWhiteTurn)
        {
            List<(Vector2Int from, Vector2Int to, int promotion)> allMoves = new();
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int position = new(x, y);
                    int piece = board.GetPieceAt(position);

                    if (piece != Piece.None && Piece.IsColor(piece, isWhiteTurn ? Piece.White : Piece.Black))
                    {
                        List<Vector2Int> validMoves = MoveValidator.GetValidMovesForPiece(position, board, piece);
                        foreach (var move in validMoves)
                        {
                            if (Piece.PieceType(piece) == Piece.Pawn && (move.y == 0 || move.y == 7))
                            {
                                foreach (int promotionType in new[] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight })
                                {
                                    allMoves.Add((position, move, promotionType));
                                }
                            }
                            else
                            {
                                allMoves.Add((position, move, 0));
                            }
                        }
                    }
                }
            }
            return allMoves;
        }

        public static (Vector2Int from, Vector2Int to, int promotion)? GetRandomMove(Board board, bool isWhiteTurn)
        {
            List<(Vector2Int from, Vector2Int to, int promotion)> allMoves = GetAllPossibleMoves(board, isWhiteTurn);

            if (allMoves.Count == 0)
            {
                return null;
            }
            return allMoves[Random.Range(0, allMoves.Count)];
        }

        public static (Vector2Int from, Vector2Int to, int promotion)? GetBestMove(Board board, bool isWhiteTurn, int maxDepth)
        {
            List<(Vector2Int from, Vector2Int to, int promotion)> allMoves = GetAllPossibleMoves(board, isWhiteTurn);

            if (allMoves.Count == 0)
            {
                return null;
            }

            // reordering moves so high value moves are checked first. This improves the chances to prune branches.
            allMoves = ReorderMoves.SortMoves(board, allMoves, isWhiteTurn);

            (Vector2Int from, Vector2Int to, int promotion)? bestMove = null;
            int bestScore = 0;

            for (int currentDepth = 1; currentDepth <= maxDepth; currentDepth++)
            {
                List<(Vector2Int from, Vector2Int to, int promotion, int score)> moveEvaluations = new();
                if (isWhiteTurn)
                {
                    bestScore = int.MinValue;

                    foreach (var (from, to, promotion) in allMoves)
                    {
                        board.MovePiece(from, to, promotion);
                        int score = Minimax(board, currentDepth - 1, int.MinValue + 1, int.MaxValue - 1, !isWhiteTurn);
                        board.UndoMove();

                        moveEvaluations.Add((from, to, promotion, score));

                        if (score >= bestScore)
                        {
                            bestScore = score;
                            bestMove = (from, to, promotion);
                        }
                    }
                }
                else
                {
                    bestScore = int.MaxValue;

                    foreach (var (from, to, promotion) in allMoves)
                    {
                        board.MovePiece(from, to, promotion);
                        int score = Minimax(board, currentDepth - 1, int.MinValue + 1, int.MaxValue - 1, !isWhiteTurn);
                        board.UndoMove();

                        moveEvaluations.Add((from, to, promotion, score));

                        if (score <= bestScore)
                        {
                            bestScore = score;
                            bestMove = (from, to, promotion);
                        }
                    }
                }
                moveEvaluations.Sort((a, b) => isWhiteTurn ? b.score.CompareTo(a.score) : a.score.CompareTo(b.score));
                allMoves = new List<(Vector2Int from, Vector2Int to, int promotion)>();
                foreach (var (from, to, promotion, score) in moveEvaluations)
                {
                    allMoves.Add((from, to, promotion));
                }
            }
            return bestMove;
        }

        private static int Minimax(Board board, int depth, int alpha, int beta, bool isWhiteTurn)
        {
            if (MoveValidator.NoLegalMovesLeft(board, isWhiteTurn))
            {
                if (MoveValidator.IsKingInCheck(board, isWhiteTurn))
                    if (isWhiteTurn)
                    {
                        return int.MinValue + 1;
                    }
                    else
                    {
                        return int.MaxValue - 1;
                    }
                return 0;
            }
            // enemy king checkmated
            if (MoveValidator.IsKingInCheck(board, !isWhiteTurn))
                if (isWhiteTurn)
                {
                    return int.MaxValue - 1;
                }
                else
                {
                    return int.MinValue + 1;
                }

            if (depth == 0)
                return HeuristicEvaluator.Evaluate(board);

            List<(Vector2Int from, Vector2Int to, int promotion)> allMoves = GetAllPossibleMoves(board, isWhiteTurn);

            // reordering moves so high value moves are checked first. This improves the chances to prune branches.
            allMoves = ReorderMoves.SortMoves(board, allMoves, isWhiteTurn);

            if (isWhiteTurn)
            {
                int curMax = int.MinValue + 1;
                foreach (var (from, to, promotion) in allMoves)
                {
                    board.MovePiece(from, to, promotion);
                    int evaluation = Minimax(board, depth - 1, alpha, beta, !isWhiteTurn);
                    board.UndoMove();

                    curMax = Mathf.Max(curMax, evaluation);
                    alpha = Mathf.Max(alpha, curMax);
                    if (curMax >= beta)
                        return int.MaxValue - 1;
                }
                return curMax;
            }
            else
            {
                int curMin = int.MaxValue - 1;
                foreach (var (from, to, promotion) in allMoves)
                {
                    board.MovePiece(from, to, promotion);
                    int evaluation = Minimax(board, depth - 1, alpha, beta, !isWhiteTurn);
                    board.UndoMove();

                    curMin = Mathf.Min(curMin, evaluation);
                    beta = Mathf.Min(beta, curMin);
                    if (curMin <= alpha)
                        return int.MinValue + 1;
                }
                return curMin;
            }
        }

    }
}
