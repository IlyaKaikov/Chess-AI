using ChessAI.Audio;
using ChessAI.Core;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChessAI.UI
{
    public class PlayerClockManager : MonoBehaviour
    {
        public static PlayerClockManager Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI playerClockText;
        [SerializeField] private TextMeshProUGUI opponentClockText;
        [SerializeField] private Button cancelButton;

        [SerializeField] private int startingMinutes = 1; // Starting time in minutes for each player

        private float playerTimeRemaining;
        private float opponentTimeRemaining;
        private bool isPlayerTurn = true; // Tracks whose turn it is
        private bool gameRunning = false;

        private GameManager gameManager;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (!gameRunning) return;

            if (isPlayerTurn)
            {
                playerTimeRemaining -= Time.deltaTime;
                if (playerTimeRemaining <= 0)
                {
                    GameMenu.Instance.TimeWin(!gameManager.isWhitePerspective);
                    EndGame();
                    return;
                }
                UpdateClock(playerClockText, playerTimeRemaining);
            }
            else
            {
                opponentTimeRemaining -= Time.deltaTime;
                if (opponentTimeRemaining <= 0)
                {
                    GameMenu.Instance.TimeWin(gameManager.isWhitePerspective);
                    EndGame();
                    return;
                }
                UpdateClock(opponentClockText, opponentTimeRemaining);
            }
        }

        public void StartClock()
        {
            gameManager = GameManager.Instance;

            playerTimeRemaining = startingMinutes * 60;
            opponentTimeRemaining = startingMinutes * 60;

            cancelButton.onClick.AddListener(StopClock);

            isPlayerTurn = (gameManager.isWhitePerspective == gameManager.isWhiteTurn);

            UpdateClock(playerClockText, playerTimeRemaining);
            UpdateClock(opponentClockText, opponentTimeRemaining);
            gameRunning = true;
        }

        public void SwitchClockTurn()
        {
            isPlayerTurn = !isPlayerTurn;
        }

        private void UpdateClock(TextMeshProUGUI clockText, float timeRemaining)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            int tenths = Mathf.FloorToInt((timeRemaining % 1) * 10);

            clockText.text = $"{minutes:00}:{seconds:00}.{tenths}";
        }

        private void EndGame()
        {
            gameManager = GameManager.Instance;

            gameRunning = false;
            UIManager.Instance.pawnPromotionUI.HidePromotionUI();
            gameManager.inputManager.ResetDraggedPiece();
            gameManager.tileManager.ClearMoveOptionsHighlights();
            gameManager.board.EndGame();

            AudioManager.Instance.PlaySound(AudioManager.Instance.gameEndSound);
        }

        public void StopClock()
        {
            gameRunning = false;
        }

        public void SetStartingMinutes(int minutes)
        {
            startingMinutes = minutes;
        }
    }
}
