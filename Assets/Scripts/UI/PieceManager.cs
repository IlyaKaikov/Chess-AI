namespace ChessAI.UI
{
    using UnityEngine;
    using Pieces;
    using System.Collections.Generic;
    using System.Collections;
    using ChessAI.Core;

    public class PieceManager : MonoBehaviour
    {
        //return values for different moves
        private const float moveDuration = 0.04f; // Duration of the move animation

        private const int playedMove = 1;
        private const int playedCapture = 2;
        private const int playedCastle = 3;
        private const int playedPromotion = 4;

        public GameObject piecePrefab;

        private readonly Dictionary<Vector2Int, GameObject> pieceMap = new();

        public GameObject SpawnPiece(int pieceType, Vector2Int position)
        {
            var pieceObject = Instantiate(piecePrefab);
            pieceObject.transform.position = new Vector3(position.x, position.y, 0);

            var pieceView = pieceObject.GetComponent<ChessPieceView>();
            pieceView.SetPieceSprite(pieceType);

            pieceMap[position] = pieceObject;
            return pieceObject;
        }

        public int MovePiece(GameObject pieceObject, Vector2Int from, Vector2Int to, bool isWhitePerspective, int promotion = 0) // return value is audio flag
        {
            var pieceView = pieceObject.GetComponent<ChessPieceView>();

            // AI animation for piece movement
            GameManager gameManager = GameManager.Instance;
            if (promotion == 0 && gameManager.currentGameMode == GameMode.HumanVsAI && gameManager.isWhiteTurn != isWhitePerspective)
            {
                StartCoroutine(SmoothMove(pieceObject, from, to));
            }

            if (pieceView.spriteRenderer.sprite == pieceView.whiteKing || pieceView.spriteRenderer.sprite == pieceView.blackKing)
            {
                if (HandleCastle(pieceObject, from, to, isWhitePerspective))
                {
                    return playedCastle; // audio flag
                }
            }

            if (pieceView.spriteRenderer.sprite == pieceView.whitePawn || pieceView.spriteRenderer.sprite == pieceView.blackPawn)
            {
                if (promotion != 0)
                {
                    bool isWhiteTurn = pieceView.spriteRenderer.sprite == pieceView.whitePawn;
                    HandlePawnPromotion(from, to, isWhiteTurn, promotion);
                    return playedPromotion; // audio flag
                }
                if (HandleEnPassant(pieceObject, from, to, isWhitePerspective))
                {
                    return playedCapture; // audio flag
                }
            }

            bool isCaptured = RemovePiece(to);

            pieceObject.transform.position = new Vector3(to.x, to.y, 0);
            pieceMap.Remove(from);
            pieceMap[to] = pieceObject;

            if (isCaptured)
            {
                return playedCapture; // audio flag
            }
            else
            {
                return playedMove; // audio flag
            }
        }

        // AI animation for piece movement
        private IEnumerator SmoothMove(GameObject pieceObject, Vector2Int from, Vector2Int to)
        {
            Vector3 startPosition = new(from.x, from.y, 0);
            Vector3 endPosition = new(to.x, to.y, 0);
            float elapsedTime = 0;

            while (elapsedTime < moveDuration)
            {
                pieceObject.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            pieceObject.transform.position = endPosition;
        }

        public GameObject GetPieceAt(Vector2Int position)
        {
            pieceMap.TryGetValue(position, out GameObject pieceObject);
            return pieceObject;
        }

        public bool RemovePiece(Vector2Int position)
        {
            if (pieceMap.TryGetValue(position, out GameObject pieceObject))
            {
                Destroy(pieceObject);
                pieceMap.Remove(position);
                return true;
            }
            return false;
        }

        private bool HandleCastle(GameObject pieceObject, Vector2Int from, Vector2Int to, bool isWhitePerspective)
        {
            bool isCastle = false;

            if (isWhitePerspective)
            {
                //white castle short
                if (from.x == 4 && from.y == 0 && to.x == 6 && to.y == 0)
                {
                    isCastle = true;
                    pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                    pieceMap.Remove(from);
                    pieceMap[to] = pieceObject;

                    GameObject rookObject = pieceMap[new(7, 0)];
                    rookObject.transform.position = new Vector3(5, 0, 0);
                    pieceMap.Remove(new(7, 0));
                    pieceMap[new(5, 0)] = rookObject;
                }
                //white castle long
                else if (from.x == 4 && from.y == 0 && to.x == 2 && to.y == 0)
                {
                    isCastle = true;
                    pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                    pieceMap.Remove(from);
                    pieceMap[to] = pieceObject;

                    GameObject rookObject = pieceMap[new(0, 0)];
                    rookObject.transform.position = new Vector3(3, 0, 0);
                    pieceMap.Remove(new(0, 0));
                    pieceMap[new(3, 0)] = rookObject;
                }
                //black castle short
                else if (from.x == 4 && from.y == 7 && to.x == 6 && to.y == 7)
                {
                    isCastle = true;
                    pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                    pieceMap.Remove(from);
                    pieceMap[to] = pieceObject;

                    GameObject rookObject = pieceMap[new(7, 7)];
                    rookObject.transform.position = new Vector3(5, 7, 0);
                    pieceMap.Remove(new(7, 7));
                    pieceMap[new(5, 7)] = rookObject;
                }
                //black castle long
                else if (from.x == 4 && from.y == 7 && to.x == 2 && to.y == 7)
                {
                    isCastle = true;
                    pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                    pieceMap.Remove(from);
                    pieceMap[to] = pieceObject;

                    GameObject rookObject = pieceMap[new(0, 7)];
                    rookObject.transform.position = new Vector3(3, 7, 0);
                    pieceMap.Remove(new(0, 7));
                    pieceMap[new(3, 7)] = rookObject;
                }
                return isCastle;
            }

            //black castle short
            if (from.x == 3 && from.y == 0 && to.x == 1 && to.y == 0)
            {
                isCastle = true;
                pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                pieceMap.Remove(from);
                pieceMap[to] = pieceObject;

                GameObject rookObject = pieceMap[new(0, 0)];
                rookObject.transform.position = new Vector3(2, 0, 0);
                pieceMap.Remove(new(0, 0));
                pieceMap[new(2, 0)] = rookObject;
            }
            //black castle long
            else if (from.x == 3 && from.y == 0 && to.x == 5 && to.y == 0)
            {
                isCastle = true;
                pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                pieceMap.Remove(from);
                pieceMap[to] = pieceObject;

                GameObject rookObject = pieceMap[new(7, 0)];
                rookObject.transform.position = new Vector3(4, 0, 0);
                pieceMap.Remove(new(7, 0));
                pieceMap[new(4, 0)] = rookObject;
            }
            //white castle short
            else if (from.x == 3 && from.y == 7 && to.x == 1 && to.y == 7)
            {
                isCastle = true;
                pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                pieceMap.Remove(from);
                pieceMap[to] = pieceObject;

                GameObject rookObject = pieceMap[new(0, 7)];
                rookObject.transform.position = new Vector3(2, 7, 0);
                pieceMap.Remove(new(0, 7));
                pieceMap[new(2, 7)] = rookObject;
            }
            //white castle long
            else if (from.x == 3 && from.y == 7 && to.x == 5 && to.y == 7)
            {
                isCastle = true;
                pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                pieceMap.Remove(from);
                pieceMap[to] = pieceObject;

                GameObject rookObject = pieceMap[new(7, 7)];
                rookObject.transform.position = new Vector3(4, 7, 0);
                pieceMap.Remove(new(7, 7));
                pieceMap[new(4, 7)] = rookObject;
            }
            return isCastle;
        }

        private bool HandleEnPassant(GameObject pieceObject, Vector2Int from, Vector2Int to, bool isWhitePerspective)
        {
            bool isEnPassant = false;
            var pieceView = pieceObject.GetComponent<ChessPieceView>();
            if (!pieceMap.ContainsKey(new(to.x, from.y)) || pieceMap.ContainsKey(to))
            {
                return isEnPassant;
            }
            var otherView = pieceMap[new(to.x, from.y)].GetComponent<ChessPieceView>();

            if (isWhitePerspective)
            {
                // white pawn capturing
                if (from.y == 4 && to.y == 5 && ((from.x - to.x) == 1 || (from.x - to.x) == -1) &&
                    pieceView.spriteRenderer.sprite == pieceView.whitePawn &&
                    otherView.spriteRenderer.sprite == pieceView.blackPawn)
                {
                    isEnPassant = true;
                    pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                    pieceMap.Remove(from);
                    pieceMap[to] = pieceObject;

                    RemovePiece(new(to.x, from.y));
                }
                // black pawn capturing
                else if (from.y == 3 && to.y == 2 && ((from.x - to.x) == 1 || (from.x - to.x) == -1) &&
                    pieceView.spriteRenderer.sprite == pieceView.blackPawn &&
                    otherView.spriteRenderer.sprite == pieceView.whitePawn)
                {
                    isEnPassant = true;
                    pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                    pieceMap.Remove(from);
                    pieceMap[to] = pieceObject;

                    RemovePiece(new(to.x, from.y));
                }
            }
            else
            {
                // black pawn capturing
                if (from.y == 4 && to.y == 5 && ((from.x - to.x) == 1 || (from.x - to.x) == -1) &&
                    pieceView.spriteRenderer.sprite == pieceView.blackPawn &&
                    otherView.spriteRenderer.sprite == pieceView.whitePawn)
                {
                    isEnPassant = true;
                    pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                    pieceMap.Remove(from);
                    pieceMap[to] = pieceObject;

                    RemovePiece(new(to.x, from.y));
                }
                // white pawn capturing
                else if (from.y == 3 && to.y == 2 && ((from.x - to.x) == 1 || (from.x - to.x) == -1) &&
                    pieceView.spriteRenderer.sprite == pieceView.whitePawn &&
                    otherView.spriteRenderer.sprite == pieceView.blackPawn)
                {
                    isEnPassant = true;
                    pieceObject.transform.position = new Vector3(to.x, to.y, 0);
                    pieceMap.Remove(from);
                    pieceMap[to] = pieceObject;

                    RemovePiece(new(to.x, from.y));
                }
            }
            return isEnPassant;
        }
        private void HandlePawnPromotion(Vector2Int from, Vector2Int to, bool isWhiteTurn, int promotion)
        {
            RemovePiece(from);
            RemovePiece(to);

            if (isWhiteTurn)
            {
                pieceMap[to] = promotion switch
                {
                    Piece.Knight => SpawnPiece((Piece.Knight | Piece.White), to),
                    Piece.Bishop => SpawnPiece((Piece.Bishop | Piece.White), to),
                    Piece.Rook => SpawnPiece((Piece.Rook | Piece.White), to),
                    Piece.Queen => SpawnPiece((Piece.Queen | Piece.White), to),
                    _ => SpawnPiece((Piece.Pawn | Piece.White), to),
                };
            }
            else
            {
                pieceMap[to] = promotion switch
                {
                    Piece.Knight => SpawnPiece((Piece.Knight | Piece.Black), to),
                    Piece.Bishop => SpawnPiece((Piece.Bishop | Piece.Black), to),
                    Piece.Rook => SpawnPiece((Piece.Rook | Piece.Black), to),
                    Piece.Queen => SpawnPiece((Piece.Queen | Piece.Black), to),
                    _ => SpawnPiece((Piece.Pawn | Piece.Black), to),
                };
            }
            // AI animation for pawn promotion
            GameManager gameManager = GameManager.Instance;
            if (gameManager.currentGameMode == GameMode.HumanVsAI && gameManager.isWhiteTurn != gameManager.isWhitePerspective)
            {
                StartCoroutine(SmoothMove(pieceMap[to], from, to));
            }
        }
    }
}
