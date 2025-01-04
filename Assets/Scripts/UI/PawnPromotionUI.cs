namespace ChessAI.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using ChessAI.Pieces;

    public class PawnPromotionUI : MonoBehaviour
    {
        public GameObject promotionPanel;
        public Button queenButton;
        public Button rookButton;
        public Button bishopButton;
        public Button knightButton;

        private Action<int> onPromotionSelected;

        private void Start()
        {
            promotionPanel.SetActive(false);

            queenButton.onClick.AddListener(() => SelectPromotion(Piece.Queen));
            rookButton.onClick.AddListener(() => SelectPromotion(Piece.Rook));
            bishopButton.onClick.AddListener(() => SelectPromotion(Piece.Bishop));
            knightButton.onClick.AddListener(() => SelectPromotion(Piece.Knight));
        }

        public void ShowPromotionOptions(bool isWhite, Action<int> callback)
        {
            onPromotionSelected = callback;
            promotionPanel.SetActive(true);

            var spriteManager = PieceSpriteManager.Instance;

            queenButton.GetComponent<Image>().sprite = isWhite ? spriteManager.whiteQueen : spriteManager.blackQueen;
            rookButton.GetComponent<Image>().sprite = isWhite ? spriteManager.whiteRook : spriteManager.blackRook;
            bishopButton.GetComponent<Image>().sprite = isWhite ? spriteManager.whiteBishop : spriteManager.blackBishop;
            knightButton.GetComponent<Image>().sprite = isWhite ? spriteManager.whiteKnight : spriteManager.blackKnight;
        }

        public void HidePromotionUI()
        {
            promotionPanel.SetActive(false);
            onPromotionSelected = null; // Cancel the callback.
        }

        private void SelectPromotion(int pieceType)
        {
            promotionPanel.SetActive(false);
            onPromotionSelected?.Invoke(pieceType);
        }
    }
}
