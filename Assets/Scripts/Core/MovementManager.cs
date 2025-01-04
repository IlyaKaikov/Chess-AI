namespace ChessAI.Core
{
    using UnityEngine;
    using ChessAI.Pieces;
    using ChessAI.UI;
    using ChessAI.Audio;

    public class MovementManager : MonoBehaviour
    {
        private GameManager gameManager;

        private void Start()
        {
            gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                Debug.LogError("GameManager instance not found!");
            }
        }

        public int MovePiece(Vector2Int from, Vector2Int to, int promotion = 0)
        {
            GameObject pieceObject = gameManager.pieceManager.GetPieceAt(from);
            if (pieceObject != null)
            {
                if (!gameManager.isWhitePerspective) // handle logic from black's perspective
                {
                    from.x = 7 - from.x;
                    from.y = 7 - from.y;

                    to.x = 7 - to.x;
                    to.y = 7 - to.y;
                }
                int piece = gameManager.board.GetPieceAt(from);

                // handles pawn promotion for (human) player
                // promotion is 0 as the (human) player is yet to make a decision
                // promotion is non-zero for the AI algorithm, and is handled differently (no UI pop-up)
                if (promotion == 0 && Piece.PieceType(piece) == Piece.Pawn &&
                   (to.y == 7 && Piece.IsColor(piece, Piece.White) || to.y == 0 && Piece.IsColor(piece, Piece.Black)))
                {
                    return TriggerPawnPromotion(pieceObject, from ,to, gameManager.isWhitePerspective);
                }

                gameManager.board.MovePiece(from, to, promotion);

                if (!gameManager.isWhitePerspective) // handle logic from black's perspective
                {
                    from.x = 7 - from.x;
                    from.y = 7 - from.y;

                    to.x = 7 - to.x;
                    to.y = 7 - to.y;
                }
                int move = gameManager.pieceManager.MovePiece(pieceObject, from, to, gameManager.isWhitePerspective, promotion);
                gameManager.isWhiteTurn = !gameManager.isWhiteTurn;
                PlayerClockManager.Instance.SwitchClockTurn();
                return move;
            }
            else
            {
                Debug.LogError($"No GameObject found at {from} to move!");
                return 0;
            }
        }

        private int TriggerPawnPromotion(GameObject pieceObject, Vector2Int from, Vector2Int to, bool isWhitePerspective)
        {
            if (!gameManager.isWhitePerspective)
            {
                gameManager.pieceManager.RemovePiece(new(7 - to.x, 7 - to.y));
            }
            else
            {
                gameManager.pieceManager.RemovePiece(to);
            }

            UIManager.Instance.pawnPromotionUI.ShowPromotionOptions(gameManager.isWhiteTurn, promotedPiece =>
            {
                gameManager.board.MovePiece(from, to, promotedPiece);
                if (!gameManager.isWhitePerspective) // handle logic from black's perspective
                {
                    from.x = 7 - from.x;
                    from.y = 7 - from.y;

                    to.x = 7 - to.x;
                    to.y = 7 - to.y;
                }
                gameManager.pieceManager.MovePiece(pieceObject, from, to, isWhitePerspective, promotedPiece);
                AudioManager.Instance.PlaySound(AudioManager.Instance.promotionSound);
                gameManager.isWhiteTurn = !gameManager.isWhiteTurn;
                PlayerClockManager.Instance.SwitchClockTurn();
                if (MoveValidator.NoLegalMovesLeft(gameManager.board, gameManager.isWhiteTurn))
                {
                    PlayerClockManager.Instance.StopClock();
                    gameManager.board.EndGame();
                    AudioManager.Instance.PlaySound(AudioManager.Instance.gameEndSound);

                    if (MoveValidator.IsKingInCheck(gameManager.board, gameManager.isWhiteTurn))
                    {
                        GameMenu.Instance.Checkmate(!gameManager.isWhiteTurn);
                        Debug.Log("Checkmate!");
                    }
                    else
                    {
                        GameMenu.Instance.Stalemate();
                        Debug.Log("Stalemate!");
                    }
                    return;
                }
            });
            return 4; //audio flag
        }
    }
}
