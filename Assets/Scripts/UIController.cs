using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject IngameScreen;

    [SerializeField] private Image ProgressBGSprite;

    [SerializeField] private Image ProgressSprite;

    [SerializeField, Range(-1, 1)] float ProgressOffsetMax = 0;

    [SerializeField] private Image LevelBGSprite;

    [SerializeField] private Text LevelLabel;

    [SerializeField] private TextMeshProUGUI ScoreLabel;

    [SerializeField] private TextMeshProUGUI BestLabel;

    [SerializeField] private GameObject WinScreen;

    [SerializeField] private TextMeshProUGUI ScoreWinLabel;

    [SerializeField] private TextMeshProUGUI BestWinLabel;

    [SerializeField] private GameObject LoseScreen;

    [SerializeField] private TextMeshProUGUI ScoreLoseLabel;

    [SerializeField] private TextMeshProUGUI BestLoseLabel;

    [SerializeField] private Button NextLevelButton;

    [SerializeField] private Button RestartButton;

    [SerializeField] private Button ReviveButton;


    const string BEST_TEXT = "BEST: {0}";

    const string BEST_TEXT_FORMATTED = "BEST:\n<size=200%>{0}</size>";

    const string SCORE_TEXT_FORMATTED = "SCORE:\n<size=200%>{0}</size>";


    public static UIController Instance { get; private set; }

    private static int _level;
    public static int Level
    {
        get => int.TryParse(Instance.LevelLabel.text, out _level) ? _level : -1;
        set
        {
            _level = value;
            Instance.LevelLabel.text = (_level + 1).ToString();
        }
    }

    private static int _score;
    public static int Score
    {
        get => int.TryParse(Instance.ScoreLabel.text, out _score) ? _score : -1;
        set
        {
            _score = value;
            Instance.ScoreLabel.text = _score.ToString();
            Instance.ScoreWinLabel.text = string.Format(SCORE_TEXT_FORMATTED, _score);
            Instance.ScoreLoseLabel.text = string.Format(SCORE_TEXT_FORMATTED, _score);
        }
    }

    private static int _best;
    public static int Best
    {
        get => int.TryParse(Regex.Match(Instance.ScoreLabel.text, @"\d+").Value, out _best) ? _best : -1;
        set
        {
            _best = value;
            Instance.BestLabel.text = string.Format(BEST_TEXT, _best);
            Instance.BestWinLabel.text = string.Format(BEST_TEXT_FORMATTED, _best);
            Instance.BestLoseLabel.text = string.Format(BEST_TEXT_FORMATTED, _best);
        }
    }

    public static void UISetProgress(float progress) => Instance.SetProgress(progress * (1 + Instance.ProgressOffsetMax));

    public static void UISetScreen(Screen screen) => Instance.SetScreen(screen);


    private void Awake() => Instance = this;

    private void Start()
    {
        GameController.OnPlayerDie.AddListener(e => SetScreen(Screen.GameOver));
        GameController.OnReachFinish.AddListener(e => SetScreen(Screen.Win));

        NextLevelButton.onClick.AddListener(GameController.NextLevel);
        RestartButton.onClick.AddListener(GameController.ReloadLevel);
    }

    public void SetProgress(float progress) => ProgressSprite.fillAmount = Mathf.Clamp(progress, 0, 1);

    public void SetScreen(Screen screen)
    {
        SyncStats();
        switch (screen)
        {
            case Screen.Ingame:
                IngameScreen.SetActive(true);
                WinScreen.SetActive(false);
                LoseScreen.SetActive(false);
                break;
            case Screen.GameOver:
                WinScreen.SetActive(false);
                LoseScreen.SetActive(true);
                break;
            case Screen.Win:
                WinScreen.SetActive(true);
                LoseScreen.SetActive(false);
                break;
            default:
                break;
        }
    }

    public static void SyncStats()
    {
        Level = GameController.Level;
        Score = GameController.Score;
        Best = GameController.Best;
    }

    public enum Screen
    {
        Ingame,
        GameOver,
        Win
    }
}
