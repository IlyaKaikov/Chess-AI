namespace ChessAI.UI
{
    using UnityEngine;

    public class PieceSpriteManager : MonoBehaviour
    {
        public static PieceSpriteManager Instance { get; private set; }

        public Sprite whiteQueen;
        public Sprite blackQueen;
        public Sprite whiteRook;
        public Sprite blackRook;
        public Sprite whiteBishop;
        public Sprite blackBishop;
        public Sprite whiteKnight;
        public Sprite blackKnight;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
