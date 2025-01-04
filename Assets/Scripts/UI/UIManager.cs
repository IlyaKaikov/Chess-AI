namespace ChessAI.UI
{
    using UnityEngine;

    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        public PawnPromotionUI pawnPromotionUI;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // Ensure pawnPromotionUI is assigned
            if (pawnPromotionUI == null)
            {
                pawnPromotionUI = GetComponent<PawnPromotionUI>();
                if (pawnPromotionUI == null)
                {
                    Debug.LogError("PawnPromotionUI is missing from UIManager!");
                }
            }
        }
    }
}
