namespace ChessAI.Core
{
    using System.Collections;
    using ChessAI.Pieces;
    using ChessAI.AI;
    using ChessAI.UI;
    using ChessAI.Audio;
    using UnityEngine;

    public enum GameMode
    {
        HumanVsHuman,
        HumanVsAI
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public bool isWhitePerspective = true; // renders the chessboard from the human's perspective (as opposed to AI's perspective)
        public bool isWhiteTurn = true; // tracks whose round it is
        public GameMode currentGameMode = GameMode.HumanVsHuman;

        private const int boardSize = 8;
        private AIPlayer aiPlayer;
        private bool isAITakingTurn = false; // Prevents overlapping moves

        [HideInInspector] public TileManager tileManager;
        [HideInInspector] public PieceManager pieceManager;
        [HideInInspector] public InputManager inputManager;
        [HideInInspector] public MovementManager movementManager;
        [HideInInspector] public PlayerClockManager clockManager;
        [HideInInspector] public Board board;

        public string fenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"; // starting position
        public GameObject tilePrefab;
        public GameObject arrowPrefab;
        public GameObject piecePrefab;
        public Color brightColor = Color.white;
        public Color darkColor = Color.black;

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

        void Start()
        {
            InitializeGame();
        }
        private async void Update()
        {
            if (currentGameMode == GameMode.HumanVsAI && (isWhiteTurn != isWhitePerspective) && !isAITakingTurn && !board.IsGameFinished())
            {
                isAITakingTurn = true; // Prevents multiple AI moves
                await aiPlayer.TakeTurn(board);
                isAITakingTurn = false;
            }
        }

        public void InitializeGame()
        {
            board = new Board();
            board.LoadFromFEN(fenString);

            tileManager = gameObject.AddComponent<TileManager>();
            pieceManager = gameObject.AddComponent<PieceManager>();
            inputManager = gameObject.AddComponent<InputManager>();
            movementManager = gameObject.AddComponent<MovementManager>();

            tileManager.tilePrefab = tilePrefab;
            tileManager.arrowPrefab = arrowPrefab;
            pieceManager.piecePrefab = piecePrefab;


            tileManager.GenerateChessboard(boardSize, brightColor, darkColor);
            PlaceStartingPieces();

            // AudioManager.Instance.PlaySound(AudioManager.Instance.gameStartSound);
        }

        public void RestartGame()
        {
            RemoveStartingPieces();
            board.EndGame();
            tileManager.ClearLastMoveHighlights();

            board = new Board();
            board.LoadFromFEN(fenString);

            PlaceStartingPieces();
        }

        public void StartGame()
        {
            board.StartGame();
            PlayerClockManager.Instance.StartClock();
            if (currentGameMode == GameMode.HumanVsAI)
            {
                aiPlayer = new AIPlayer(isWhite: !isWhitePerspective);
            }
            AudioManager.Instance.PlaySound(AudioManager.Instance.gameStartSound);
        }

        public void SetPerspective()
        {
            RemoveStartingPieces(); // delete old perspective
            PlaceStartingPieces(); // place new perspective
        }

        private void PlaceStartingPieces()
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Vector2Int position = new(x, y);


                    int piece = board.GetPieceAt(position);
                    if (!isWhitePerspective)
                    {
                        piece = board.GetPieceAt(new(7 - position.x, 7 - position.y));
                    }

                    if (piece == Piece.None) continue;

                    pieceManager.SpawnPiece(piece, position);
                }
            }
        }

        private void RemoveStartingPieces()
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Vector2Int position = new(x, y);


                    int piece = board.GetPieceAt(position);
                    if (!isWhitePerspective)
                    {
                        piece = board.GetPieceAt(new(7 - position.x, 7 - position.y));
                    }

                    if (piece == Piece.None) continue;

                    pieceManager.RemovePiece(position);
                }
            }
        }

        public bool IsPositionValid(Vector2Int position)
        {
            return (position.x >= 0 && position.x < boardSize && position.y >=0 && position.y < boardSize);
        }
    }
}
