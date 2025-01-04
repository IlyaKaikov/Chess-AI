namespace ChessAI.UI
{
    using ChessAI.Core;
    using ChessAI.Pieces;
    using ChessAI.Audio;
    using System.Collections.Generic;
    using UnityEngine;

    public class InputManager : MonoBehaviour
    {
        private GameManager gameManager;
        private MovementManager movementManager;
        private TileManager tileManager;

        private GameObject draggedPiece = null;
        private SpriteRenderer draggedSpriteRenderer = null;
        private Vector2Int startDragPosition;

        private readonly int originalSortingOrder = 3; // Default sorting order

        void Start()
        {
            gameManager = GameManager.Instance;
            movementManager = gameManager.movementManager;
            tileManager = gameManager.tileManager;

            if (gameManager == null || movementManager == null || tileManager == null)
            {
                Debug.LogError("Missing essential managers in InputManager.");
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (gameManager.board.IsGameFinished() || (gameManager.currentGameMode == GameMode.HumanVsAI && gameManager.isWhiteTurn != gameManager.isWhitePerspective))
                {
                    tileManager.ClearMarkedTilesAndArrows();
                    return;
                }
                OnMouseDown();
            }
            else if (Input.GetMouseButton(0) && draggedPiece != null)
            {
                OnMouseDrag();
            }
            else if (Input.GetMouseButtonUp(0) && draggedPiece != null)
            {
                OnMouseUp();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                OnRightMouseDown();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                OnRightMouseUp();
            }
        }

        private Vector2Int GetMouseBoardPosition()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y));
        }

        private void OnMouseDown()
        {
            tileManager.ClearMarkedTilesAndArrows();
            Vector2Int boardPosition = GetMouseBoardPosition();

            if (!gameManager.IsPositionValid(boardPosition)) return;

            int piece = gameManager.board.GetPieceAt(boardPosition);
            if (!gameManager.isWhitePerspective)
            {
                piece = gameManager.board.GetPieceAt(new(7 - boardPosition.x, 7 - boardPosition.y));
            }

            if (piece != Piece.None)
            {
                if ((Piece.IsColor(piece, Piece.White) && !gameManager.isWhiteTurn) ||
                    (Piece.IsColor(piece, Piece.Black) && gameManager.isWhiteTurn))
                {
                    // Debug.Log("Not your turn.");
                    return;
                }

                // Start dragging
                draggedPiece = gameManager.pieceManager.GetPieceAt(boardPosition);
                if (draggedPiece != null)
                {
                    startDragPosition = boardPosition;

                    // Change the sorting order of the dragged piece
                    draggedSpriteRenderer = draggedPiece.GetComponent<SpriteRenderer>();
                    if (draggedSpriteRenderer != null)
                    {
                        draggedSpriteRenderer.sortingOrder = 4; // Bring to the front
                    }

                    Vector2Int startDragPositionAdjusted = startDragPosition; //Adjusted for black's perspective
                    if (!gameManager.isWhitePerspective)
                    {
                        startDragPositionAdjusted.x = 7 - startDragPositionAdjusted.x;
                        startDragPositionAdjusted.y = 7 - startDragPositionAdjusted.y;
                    }

                    List<Vector2Int> validMoves = MoveValidator.GetValidMovesForPiece(startDragPositionAdjusted, gameManager.board,
                                                                                      gameManager.board.GetPieceAt(startDragPositionAdjusted));
                    if (!gameManager.isWhitePerspective)
                    {
                        for (int i = 0; i < validMoves.Count; i++)
                        {
                            validMoves[i] = new(7 - validMoves[i].x, 7 - validMoves[i].y);
                        }
                    }
                    tileManager.HighlightMoveOptions(validMoves);
                }
            }
        }

        private void OnMouseDrag()
        {
            if (draggedPiece == null) return;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggedPiece.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0); // Follow the mouse
        }

        private void OnMouseUp()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int boardPosition = new(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y));
            tileManager.ClearMoveOptionsHighlights();

            draggedSpriteRenderer = draggedPiece.GetComponent<SpriteRenderer>();
            if (draggedSpriteRenderer != null)
            {
                draggedSpriteRenderer.sortingOrder = originalSortingOrder; // Restore original sorting order
            }

            if (!gameManager.IsPositionValid(boardPosition))
            {
                ResetDraggedPiece(); // Reset if released outside the board
                return;
            }

            Vector2Int startDragPositionAdjusted = startDragPosition; //Adjusted for black's perspective
            if (!gameManager.isWhitePerspective)
            {
                startDragPositionAdjusted.x = 7 - startDragPositionAdjusted.x;
                startDragPositionAdjusted.y = 7 - startDragPositionAdjusted.y;
            }

            List<Vector2Int> validMoves = MoveValidator.GetValidMovesForPiece(startDragPositionAdjusted, gameManager.board, gameManager.board.GetPieceAt(startDragPositionAdjusted));
            Vector2Int boardPostionAdjusted = boardPosition;
            if (!gameManager.isWhitePerspective)
            {
                boardPostionAdjusted.x = 7 - boardPostionAdjusted.x;
                boardPostionAdjusted.y = 7 - boardPostionAdjusted.y;
            }
            bool isMoveValid = false;
            foreach (Vector2Int move in validMoves)
            {
                if (move == boardPostionAdjusted)
                {
                    isMoveValid = true;
                    break;
                }
            }

            if (isMoveValid)
            {
                int moveSound = movementManager.MovePiece(startDragPosition, boardPosition);
                tileManager.HighlightLastMove(startDragPosition, boardPosition);
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

                    draggedPiece = null; // Stop dragging
                    return;
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
            }
            else
            {
                // Illegal move
                if (startDragPosition != boardPosition)
                {
                    // Debug.Log("Illegal move");
                    AudioManager.Instance.PlaySound(AudioManager.Instance.illegal);
                }
                ResetDraggedPiece();
            }

            draggedPiece = null; // Stop dragging
        }

        public void ResetDraggedPiece()
        {
            if (draggedPiece != null)
            {
                draggedPiece.transform.position = new Vector3(startDragPosition.x, startDragPosition.y, 0);

                draggedPiece = null;
            }
        }

        private void OnRightMouseDown()
        {
            Vector2Int boardPosition = GetMouseBoardPosition();
            if (!gameManager.IsPositionValid(boardPosition)) return;

            startDragPosition = boardPosition;
        }

        private void OnRightMouseUp()
        {
            Vector2Int boardPosition = GetMouseBoardPosition();
            if (!gameManager.IsPositionValid(boardPosition)) return;

            if (startDragPosition == boardPosition)
            {
                tileManager.ToggleMarkedTiles(boardPosition);
            }
            else
            {
                tileManager.ToggleArrow(startDragPosition, boardPosition);
            }
        }
    }
}
