namespace ChessAI.Tests
{
    using ChessAI.Core;
    using UnityEngine;

    public class PerftTester : MonoBehaviour
    {
        [SerializeField]
        private string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        [SerializeField]
        private int depth = 3;

        void Start()
        {
            Board board = new();
            board.LoadFromFEN(fen);

            bool isWhiteTurn = GameManager.Instance.isWhiteTurn;

            for (int i = 1; i <= depth; i++)
            {
                long totalNodes = Perft.PerftTest(board, i, isWhiteTurn);
                Debug.Log($"Perft depth {i}: {totalNodes} nodes");
            }

            /*var divideResults = Perft.PerftDivide(board, depth, isWhiteTurn);
            foreach (var entry in divideResults)
            {
                Debug.Log($"{entry.Key}: {entry.Value} nodes");
            }*/
        }
    }
}
