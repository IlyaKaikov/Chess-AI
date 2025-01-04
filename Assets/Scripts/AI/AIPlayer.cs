using UnityEngine;
using System.Collections;
using ChessAI.Core;
using ChessAI.UI;
using ChessAI.Audio;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChessAI.AI
{
    public class AIPlayer
    {
        private readonly bool isWhite;
        private readonly GameManager gameManager;
        private readonly MovementManager movementManager;
        private readonly TileManager tileManager;

        public AIPlayer(bool isWhite)
        {
            this.isWhite = isWhite;
            this.gameManager = GameManager.Instance;
            this.movementManager = gameManager.movementManager;
            this.tileManager = gameManager.tileManager;
        }

        public async Task TakeTurn(Board board)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            int depth = 3;
            var move = await Task.Run(() => AIHelper.GetBestMove(board, isWhite, depth));

            stopwatch.Stop();
            UnityEngine.Debug.Log($"AI calculated move in {stopwatch.ElapsedMilliseconds} ms at depth {depth}");
            if (move.HasValue && !board.IsGameFinished())
            {
                Vector2Int from = move.Value.from;
                Vector2Int to = move.Value.to;
                if (!gameManager.isWhitePerspective) // handle logic from black's perspective
                {
                    from.x = 7 - from.x;
                    from.y = 7 - from.y;

                    to.x = 7 - to.x;
                    to.y = 7 - to.y;
                }

                int moveSound = movementManager.MovePiece(from, to, move.Value.promotion);
                tileManager.HighlightLastMove(from, to);
                if (MoveValidator.NoLegalMovesLeft(gameManager.board, gameManager.isWhiteTurn))
                {
                    PlayerClockManager.Instance.StopClock();
                    gameManager.board.EndGame();
                    AudioManager.Instance.PlaySound(AudioManager.Instance.gameEndSound);

                    if (MoveValidator.IsKingInCheck(gameManager.board, gameManager.isWhiteTurn))
                    {
                        GameMenu.Instance.Checkmate(!gameManager.isWhiteTurn);
                        UnityEngine.Debug.Log("Checkmate!");
                    }
                    else
                    {
                        GameMenu.Instance.Stalemate();
                        UnityEngine.Debug.Log("Stalemate!");
                    }
                }
                else if (MoveValidator.IsKingInCheck(gameManager.board, gameManager.isWhiteTurn))
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.checkSound);
                }
                else if (moveSound == 1)
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.movePieceSound);
                }
                else if (moveSound == 2)
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.capturePieceSound);
                }
                // these sounds will be played on-top of check Sound, indicating they caused a check on the enemy's king
                if (moveSound == 3)
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.castleSound);
                }
                if (moveSound == 4)
                {
                    AudioManager.Instance.PlaySound(AudioManager.Instance.promotionSound);
                }
            }
        }
    }
}
