namespace ChessAI.AI
{
    using ChessAI.Core;
    using ChessAI.Pieces;
    using UnityEngine;

    public static class HeuristicEvaluator
    {
        private static readonly int[] PieceValues = {
            0,   // None
            0,   // King
            100, // Pawn
            300, // Knight
            0,   // Not in use
            320, // Bishop
            500, // Rook
            900  // Queen
        };

        private const int InitialMaterial = 7800;
        private const int LowerMaterialThreshold = 1600;
        //private const int MaxKingProximityScore = 150;

        public static int Evaluate(Board board)
        {
            float gamePhase = CalculateGamePhase(board);
            int materialScore = CalculateMaterial(board);
            int positionalScore = CalculatePositionalBonuses(board, gamePhase);
            int kingSafetyScore = EvaluateKingProximity(board, gamePhase);

            return materialScore + positionalScore;// + kingSafetyScore;
        }

        private static float CalculateGamePhase(Board board)
        {
            int remainingMaterial = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    int piece = board.GetPieceAt(new Vector2Int(x, y));
                    if (piece == Piece.None) continue;

                    remainingMaterial += PieceValues[Piece.PieceType(piece)];
                }
            }
            if (remainingMaterial <= LowerMaterialThreshold)
            {
                return 0f;
            }
            return Mathf.Clamp01((float)(remainingMaterial - LowerMaterialThreshold) / (InitialMaterial - LowerMaterialThreshold));
        }

        private static int CalculateMaterial(Board board)
        {
            int score = 0;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int position = new(x, y);
                    int piece = board.GetPieceAt(position);
                    if (piece == Piece.None) continue;

                    int value = PieceValues[Piece.PieceType(piece)];
                    if (Piece.IsColor(piece, Piece.White))
                    {
                        score += value;
                    }
                    else
                    {
                        score -= value;
                    }
                }
            }

            return score;
        }

        private static int CalculatePositionalBonuses(Board board, float gamePhase)
        {
            int score = 0;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int position = new(x, y);
                    int piece = board.GetPieceAt(position);
                    if (piece == Piece.None) continue;

                    int pieceType = Piece.PieceType(piece);
                    bool isWhitePiece = Piece.IsColor(piece, Piece.White);
                    int[] earlyGameMap = isWhitePiece ? GetWhiteEarlyGameMap(pieceType) : GetBlackEarlyGameMap(pieceType);
                    int[] endGameMap = isWhitePiece ? GetWhiteEndGameMap(pieceType) : GetBlackEndGameMap(pieceType);
                    if (earlyGameMap == null || endGameMap == null) continue;

                    int index = x + (7 - y) * 8;
                    int bonus = Interpolate(earlyGameMap[index], endGameMap[index], gamePhase);
                    score += isWhitePiece ? bonus : -bonus;
                }
            }
            return score;
        }

        private static int[] GetWhiteEarlyGameMap(int pieceType)
        {
            return pieceType switch
            {
                Piece.Pawn => PositionalMaps.WhitePawnEarlyGameMap,
                Piece.Rook => PositionalMaps.WhiteRookEarlyGameMap,
                Piece.Knight => PositionalMaps.WhiteKnightEarlyGameMap,
                Piece.Bishop => PositionalMaps.WhiteBishopEarlyGameMap,
                Piece.Queen => PositionalMaps.WhiteQueenEarlyGameMap,
                Piece.King => PositionalMaps.WhiteKingEarlyGameMap,
                _ => null
            };
        }

        private static int[] GetBlackEarlyGameMap(int pieceType)
        {
            return pieceType switch
            {
                Piece.Pawn => PositionalMaps.BlackPawnEarlyGameMap,
                Piece.Rook => PositionalMaps.BlackRookEarlyGameMap,
                Piece.Knight => PositionalMaps.BlackKnightEarlyGameMap,
                Piece.Bishop => PositionalMaps.BlackBishopEarlyGameMap,
                Piece.Queen => PositionalMaps.BlackQueenEarlyGameMap,
                Piece.King => PositionalMaps.BlackKingEarlyGameMap,
                _ => null
            };
        }

        private static int[] GetWhiteEndGameMap(int pieceType)
        {
            return pieceType switch
            {
                Piece.Pawn => PositionalMaps.WhitePawnEndGameMap,
                Piece.Rook => PositionalMaps.WhiteRookEndGameMap,
                Piece.Knight => PositionalMaps.WhiteKnightEndGameMap,
                Piece.Bishop => PositionalMaps.WhiteBishopEndGameMap,
                Piece.Queen => PositionalMaps.WhiteQueenEndGameMap,
                Piece.King => PositionalMaps.WhiteKingEndGameMap,
                _ => null
            };
        }

        private static int[] GetBlackEndGameMap(int pieceType)
        {
            return pieceType switch
            {
                Piece.Pawn => PositionalMaps.BlackPawnEndGameMap,
                Piece.Rook => PositionalMaps.BlackRookEndGameMap,
                Piece.Knight => PositionalMaps.BlackKnightEndGameMap,
                Piece.Bishop => PositionalMaps.BlackBishopEndGameMap,
                Piece.Queen => PositionalMaps.BlackQueenEndGameMap,
                Piece.King => PositionalMaps.BlackKingEndGameMap,
                _ => null
            };
        }

        private static int EvaluateKingProximity(Board board, float gamePhase)
        {
            if (gamePhase >= 0.5f) return 0;
            Vector2Int whiteKingPosition = FindKingPosition(board, true);
            Vector2Int blackKingPosition = FindKingPosition(board, false);

            if (whiteKingPosition == new Vector2Int(-1, -1) || blackKingPosition == new Vector2Int(-1, -1))
                return -1;
            int evaluation = 0;

            int enemyDistToCenterFile = Mathf.Max(3 - blackKingPosition.x, blackKingPosition.x - 4);
            int enemyDistToCenterRank = Mathf.Max(3 - blackKingPosition.y, blackKingPosition.y - 4);
            int enemyDistToCenter = enemyDistToCenterFile + enemyDistToCenterRank;

            evaluation += enemyDistToCenter;

            // Manhattan distance
            int distance = Mathf.Abs(whiteKingPosition.x - blackKingPosition.x) + Mathf.Abs(whiteKingPosition.y - blackKingPosition.y);
            int maxDistance = 14;
            int proximityBonus = maxDistance - distance;
            evaluation += 2 * proximityBonus;

            return -Mathf.RoundToInt(evaluation * 10 * (1 - gamePhase));
        }

        private static Vector2Int FindKingPosition(Board board, bool isWhite)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2Int position = new(x, y);
                    int piece = board.GetPieceAt(position);

                    if (isWhite && piece == (Piece.King | Piece.White))
                    {
                        return position;
                    }
                    else if (!isWhite && piece == (Piece.King | Piece.Black))
                    {
                        return position;
                    }
                }
            }
            return new(-1, -1);
        }

        private static int Interpolate(int earlyValue, int endValue, float phaseProgress)
        {
            return Mathf.RoundToInt((phaseProgress * earlyValue) + ((1 - phaseProgress) * endValue));
        }
    }
}
