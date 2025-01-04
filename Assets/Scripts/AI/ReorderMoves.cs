using ChessAI.Core;
using ChessAI.Pieces;
using System.Collections.Generic;
using UnityEngine;

namespace ChessAI.AI
{
    public static class ReorderMoves
    {
        public static List<(Vector2Int from, Vector2Int to, int promotion)> SortMoves(Board board, List<(Vector2Int from, Vector2Int to, int promotion)> moves, bool isWhiteTurn)
        {
            List<(Vector2Int from, Vector2Int to, int promotion, int score)> scoredMoves = new();

            foreach (var move in moves)
            {
                int score = EvaluateMove(board, move, isWhiteTurn);
                scoredMoves.Add((move.from, move.to, move.promotion, score));
            }

            // sorts in descending order, prioritizing high value moves.
            scoredMoves.Sort((a, b) => b.score.CompareTo(a.score));

            List<(Vector2Int from, Vector2Int to, int promotion)> sortedMoves = new();
            foreach (var (from, to, promotion, score) in scoredMoves)
            {
                sortedMoves.Add((from, to, promotion));
            }
            return sortedMoves;
        }

        private static int EvaluateMove(Board board, (Vector2Int from, Vector2Int to, int promotion) move, bool isWhiteTurn)
        {
            int score = 0;
            int pieceMoved = board.GetPieceAt(move.from);
            int pieceCaptured = board.GetPieceAt(move.to);

            // piece capture
            if (pieceCaptured != Piece.None)
            {
                score += GetPieceValue(pieceCaptured);
            }

            // en passant
            if (Piece.PieceType(pieceMoved) == Piece.Pawn && Piece.PieceType(pieceCaptured) == Piece.None &&
                ((move.from.x - move.to.x) == 1 || (move.from.x - move.to.x) == -1))
            {
                score += 100;
            }

            // Pawn Promotion
            if (Piece.PieceType(pieceMoved) == Piece.Pawn && (move.to.y == 0 || move.to.y == 7))
            {
                score += 800; // High value for promotion (regardless of promotion type, as it can be situational)
            }

            // castle
            if (Piece.PieceType(pieceMoved) == Piece.King &&
                (move.from.x == 4 || move.to.x == 6) && (move.from.x == 4 || move.to.x == 2))
            {
                score += 150;
            }

            // piece in danger
            board.MovePiece(move.from, move.to, move.promotion);
            if (IsPieceInDanger(board, move.to, isWhiteTurn))
            {
                score -= GetPieceValue(pieceMoved);
            }
            board.UndoMove();

            return score;
        }

        private static int GetPieceValue(int piece)
        {
            return Piece.PieceType(piece) switch
            {
                Piece.Pawn => 100,
                Piece.Knight => 300,
                Piece.Bishop => 320,
                Piece.Rook => 500,
                Piece.Queen => 900,
                Piece.King => 10000, // High value to reflect king's importance
                _ => 0
            };
        }

        private static bool IsPieceInDanger(Board board, Vector2Int position, bool isWhiteTurn)
        {
            ulong squaresUnderAttack = MoveValidator.SquaresUnderAttack(board, isWhiteTurn);
            int bitIndex = position.x + position.y * 8;

            return (squaresUnderAttack & (1UL << bitIndex)) != 0;
        }
    }
}