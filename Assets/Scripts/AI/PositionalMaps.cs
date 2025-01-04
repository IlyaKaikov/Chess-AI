namespace ChessAI.AI
{
    public static class PositionalMaps
    {
        // Helper function to mirror maps for black
        private static int[] MirrorForBlack(int[] whiteMap)
        {
            int[] blackMap = new int[64];
            for (int i = 0; i < 64; i++)
            {
                blackMap[63 - i] = whiteMap[i];
            }
            return blackMap;
        }

        // -------------------- Pawn Maps --------------------
        public static readonly int[] WhitePawnEarlyGameMap = new int[]
        {
             0,   0,   0,   0,   0,   0,   0,   0,
            50,  50,  50,  50,  50,  50,  50,  50,
            10,  10,  20,  30,  30,  20,  10,  10,
             5,   5,  10,  25,  25,  10,   5,   5,
             0,   0,   0,  20,  20,   0,   0,   0,
             5,  -5, -10,   0,   0, -10,  -5,   5,
             5,  10,  10, -20, -20,  10,  10,   5,
             0,   0,   0,   0,   0,   0,   0,   0
        };
        public static readonly int[] BlackPawnEarlyGameMap = MirrorForBlack(WhitePawnEarlyGameMap);

        public static readonly int[] WhitePawnEndGameMap = new int[]
        {
             0,   0,   0,   0,   0,   0,   0,   0,
            70,  70,  70,  70,  70,  70,  70,  70,
            50,  50,  50,  50,  50,  50,  50,  50,
            30,  30,  30,  30,  30,  30,  30,  30,
            20,  20,  20,  20,  20,  20,  20,  20,
            10,  10,  10,  10,  10,  10,  10,  10,
            10,  10,  10,  10,  10,  10,  10,  10,
             0,   0,   0,   0,   0,   0,   0,   0
        };
        public static readonly int[] BlackPawnEndGameMap = MirrorForBlack(WhitePawnEndGameMap);

        // -------------------- Knight Maps --------------------
        public static readonly int[] WhiteKnightEarlyGameMap = new int[]
        {
            -50, -40, -30, -30, -30, -30, -40, -50,
            -40, -20,   0,   5,   5,   0, -20, -40,
            -30,   0,  10,  15,  15,  10,   0, -30,
            -30,   5,  15,  20,  20,  15,   5, -30,
            -30,   5,  15,  20,  20,  15,   5, -30,
            -30,   0,  10,  15,  15,  10,   0, -30,
            -40, -20,   0,   5,   5,   0, -20, -40,
            -50, -40, -30, -30, -30, -30, -40, -50
        };
        public static readonly int[] BlackKnightEarlyGameMap = MirrorForBlack(WhiteKnightEarlyGameMap);

        public static readonly int[] WhiteKnightEndGameMap = new int[]
        {
            -50, -40, -30, -30, -30, -30, -40, -50,
            -40, -20,   0,   5,   5,   0, -20, -40,
            -30,   0,  10,  15,  15,  10,   0, -30,
            -30,   5,  15,  20,  20,  15,   5, -30,
            -30,   5,  15,  20,  20,  15,   5, -30,
            -30,   0,  10,  15,  15,  10,   0, -30,
            -40, -20,   0,   5,   5,   0, -20, -40,
            -50, -40, -30, -30, -30, -30, -40, -50
        };
        public static readonly int[] BlackKnightEndGameMap = MirrorForBlack(WhiteKnightEndGameMap);

        // -------------------- Bishop Maps --------------------
        public static readonly int[] WhiteBishopEarlyGameMap = new int[]
        {
            -20, -10, -10, -10, -10, -10, -10, -20,
            -10,   0,   0,   0,   0,   0,   0, -10,
            -10,   5,  10,  10,  10,  10,   5, -10,
            -10,   0,  10,  10,  10,  10,   0, -10,
            -10,   5,   5,  10,  10,   5,   5, -10,
            -10,   0,   5,  10,  10,   5,   0, -10,
            -10,   0,   0,   0,   0,   0,   0, -10,
            -20, -10, -10, -10, -10, -10, -10, -20
        };
        public static readonly int[] BlackBishopEarlyGameMap = MirrorForBlack(WhiteBishopEarlyGameMap);

        public static readonly int[] WhiteBishopEndGameMap = new int[]
        {
            -10, -10, -10, -10, -10, -10, -10, -10,
            -10,   0,   0,   0,   0,   0,   0, -10,
            -10,   0,  10,  10,  10,  10,   0, -10,
            -10,   0,  10,  20,  20,  10,   0, -10,
            -10,   0,  10,  20,  20,  10,   0, -10,
            -10,   0,  10,  10,  10,  10,   0, -10,
            -10,   0,   0,   0,   0,   0,   0, -10,
            -10, -10, -10, -10, -10, -10, -10, -10
        };
        public static readonly int[] BlackBishopEndGameMap = MirrorForBlack(WhiteBishopEndGameMap);

        // -------------------- Rook Maps --------------------
        public static readonly int[] WhiteRookEarlyGameMap = new int[]
        {
             0,   0,   0,   0,   0,   0,   0,   0,
            -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,
            -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,
            -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,
            -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,
            -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,
             5,   5,  10,  10,  10,  10,   5,   5,
             0,   0,   0,   5,   5,   0,   0,   0
        };
        public static readonly int[] BlackRookEarlyGameMap = MirrorForBlack(WhiteRookEarlyGameMap);

        public static readonly int[] WhiteRookEndGameMap = new int[]
        {
            10,  15,  15,  15,  15,  15,  15,  10,
            10,  15,  15,  15,  15,  15,  15,  10,
             5,  10,  10,  10,  10,  10,  10,   5,
             5,  10,  10,  10,  10,  10,  10,   5,
             5,  10,  10,  10,  10,  10,  10,   5,
             5,  10,  10,  10,  10,  10,  10,   5,
            10,  15,  15,  15,  15,  15,  15,  10,
             0,   0,   0,   5,   5,   0,   0,   0
        };
        public static readonly int[] BlackRookEndGameMap = MirrorForBlack(WhiteRookEndGameMap);

        // -------------------- Queen Maps --------------------
        public static readonly int[] WhiteQueenEarlyGameMap = new int[]
        {
            -20, -10, -10,  -5,  -5, -10, -10, -20,
            -10,   0,   5,   0,   0,   0,   0, -10,
            -10,   5,   5,   5,   5,   5,   0, -10,
              0,   0,   5,   5,   5,   5,   0,  -5,
             -5,   0,   5,   5,   5,   5,   0,  -5,
            -10,   0,   5,   5,   5,   5,   0, -10,
            -10,   0,   0,   0,   0,   0,   0, -10,
            -20, -10, -10,  -5,  -5, -10, -10, -20
        };
        public static readonly int[] BlackQueenEarlyGameMap = MirrorForBlack(WhiteQueenEarlyGameMap);

        public static readonly int[] WhiteQueenEndGameMap = new int[]
        {
             0,  15,  30,  30,  30,  30,  15,   0,
            10,  20,  40,  40,  40,  40,  20,  10,
            10,  20,  40,  50,  50,  40,  20,  10,
            10,  20,  40,  50,  50,  40,  20,  10,
            10,  20,  40,  50,  50,  40,  20,  10,
            10,  15,  20,  25,  25,  20,  15,  10,
            10,  15,  15,  20,  20,  15,  15,  10,
             0,   0,   0,  10,  10,   0,   0,   0
        };
        public static readonly int[] BlackQueenEndGameMap = MirrorForBlack(WhiteQueenEndGameMap);

        // -------------------- King Maps --------------------
        public static readonly int[] WhiteKingEarlyGameMap = new int[]
        {
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -20, -30, -30, -40, -40, -30, -30, -20,
            -10, -20, -20, -20, -20, -20, -20, -10,
             20,  20,   0,   0,   0,   0,  20,  20,
             20,  30,  10,   0,   0,  10,  30,  20
        };
        public static readonly int[] BlackKingEarlyGameMap = MirrorForBlack(WhiteKingEarlyGameMap);

        public static readonly int[] WhiteKingEndGameMap = new int[]
        {
            -20, -10, -10, -10, -10, -10, -10, -20,
            -10,   5,   5,   5,   5,   5,   5, -10,
            -10,   5,  15,  15,  15,  15,   5, -10,
            -10,   5,  15,  20,  20,  15,   5, -10,
            -10,   5,  15,  20,  20,  15,   5, -10,
            -10,   5,  15,  15,  15,  15,   5, -10,
            -10,   5,   5,   5,   5,   5,   5, -10,
            -20, -10, -10, -10, -10, -10, -10, -20,
        };
        public static readonly int[] BlackKingEndGameMap = MirrorForBlack(WhiteKingEndGameMap);
    }
}
