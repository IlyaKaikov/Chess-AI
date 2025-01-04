using ChessAI.Audio;
using ChessAI.Core;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ChessAI.UI
{
    public class GameMenu : MonoBehaviour
    {
        public static GameMenu Instance { get; private set; }

        [Header("Panels")]
        [SerializeField] private GameObject startMenuPanel;
        [SerializeField] private GameObject clocksPanel;
        [SerializeField] private GameObject whiteCheckmatePanel;
        [SerializeField] private GameObject whiteTimeWinPanel;
        [SerializeField] private GameObject blackCheckmatePanel;
        [SerializeField] private GameObject blackTimeWinPanel;
        [SerializeField] private GameObject stalematePanel;

        [Header("Input Fields")]
        [SerializeField] private TMP_InputField fenInputField;

        [Header("Buttons")]
        [SerializeField] private Button playHumanVsHumanButton;
        [SerializeField] private Button playHumanVsAIButton;
        [SerializeField] private Button playAsWhiteButton;
        [SerializeField] private Button playAsBlackButton;

        [SerializeField] private Button cancelButton;
        [SerializeField] private Button backToMenuButton;
        [SerializeField] private Button applyFenButton;

        [SerializeField] private Button time1Button;
        [SerializeField] private Button time3Button;
        [SerializeField] private Button time5Button;
        [SerializeField] private Button time10Button;

        private GameManager gameManager;
        private int clockMinutes;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            startMenuPanel.SetActive(true);
            clocksPanel.SetActive(false);
            whiteCheckmatePanel.SetActive(false);
            whiteTimeWinPanel.SetActive(false);
            blackCheckmatePanel.SetActive(false);
            blackTimeWinPanel.SetActive(false);
            stalematePanel.SetActive(false);
            backToMenuButton.gameObject.SetActive(false);

            playHumanVsHumanButton.onClick.AddListener(() => StartGame(GameMode.HumanVsHuman));
            playHumanVsAIButton.onClick.AddListener(() => StartGame(GameMode.HumanVsAI));
            playAsWhiteButton.onClick.AddListener(() => SetPerspective(true));
            playAsBlackButton.onClick.AddListener(() => SetPerspective(false));

            cancelButton.onClick.AddListener(() => BackToMenu());
            backToMenuButton.onClick.AddListener(() => BackToMenu());
            applyFenButton.onClick.AddListener(ApplyFenString);

            time1Button.onClick.AddListener(() => SetStartingMinutes(1));
            time3Button.onClick.AddListener(() => SetStartingMinutes(3));
            time5Button.onClick.AddListener(() => SetStartingMinutes(5));
            time10Button.onClick.AddListener(() => SetStartingMinutes(10));

            fenInputField.text = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            SetStartingMinutes(1);
        }

        private void StartGame(GameMode gameMode)
        {
            gameManager.currentGameMode = gameMode;
            startMenuPanel.SetActive(false);
            clocksPanel.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            PlayerClockManager.Instance.SetStartingMinutes(clockMinutes);
            gameManager.StartGame();
        }

        private void ApplyFenString()
        {
            string fenString = fenInputField.text.Trim();
            if (string.IsNullOrEmpty(fenString))
            {
                Debug.LogError("FEN string is empty!");
                return;
            }
            try
            {
                gameManager.fenString = fenString;
                gameManager.RestartGame();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Invalid FEN string: {ex.Message}");
            }
        }

        private void SetStartingMinutes(int minutes)
        {
            clockMinutes = minutes;
            //Debug.Log($"Game time set to {minutes} minutes.");
        }

        private void SetPerspective(bool isWhitePerspective)
        {
            if (isWhitePerspective == gameManager.isWhitePerspective) return;

            gameManager.isWhitePerspective = isWhitePerspective;
            gameManager.SetPerspective();
        }

        public void BackToMenu()
        {
            startMenuPanel.SetActive(true);
            clocksPanel.SetActive(false);
            whiteCheckmatePanel.SetActive(false);
            whiteTimeWinPanel.SetActive(false);
            blackCheckmatePanel.SetActive(false);
            blackTimeWinPanel.SetActive(false);
            stalematePanel.SetActive(false);
            backToMenuButton.gameObject.SetActive(false);
            UIManager.Instance.pawnPromotionUI.HidePromotionUI();

            gameManager.RestartGame();
        }

        public void Checkmate(bool isWhite)
        {
            cancelButton.gameObject.SetActive(false);
            backToMenuButton.gameObject.SetActive(true);
            if (isWhite)
            {
                whiteCheckmatePanel.SetActive(true);
            }
            else
            {
                blackCheckmatePanel.SetActive(true);
            }
        }

        public void Stalemate()
        {
            cancelButton.gameObject.SetActive(false);
            backToMenuButton.gameObject.SetActive(true);
            stalematePanel.SetActive(true);
        }

        public void TimeWin(bool isWhite)
        {
            cancelButton.gameObject.SetActive(false);
            backToMenuButton.gameObject.SetActive(true);
            if (isWhite)
            {
                whiteTimeWinPanel.SetActive(true);
            }
            else
            {
                blackTimeWinPanel.SetActive(true);
            }
        }
    }
}
