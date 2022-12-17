using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI = UIController;
using Random = System.Random;

public class GameController : MonoBehaviour
{
    const string LevelKey = "LevelIndex";

    const string BestScoreKey = "BestScore";

    const string CachedScoreKey = "CachedScore";

    public static Random Random { get; private set; }

    public static State GameState { get; private set; } = State.Playing;

    public static Transform LevelMap;

    public static ControlsBehaviour LevelControls;

    public static CameraBehaviour GameCamera;


    public static int CachedScore
    {
        get => PlayerPrefs.GetInt(CachedScoreKey, 0);
        private set
        {
            PlayerPrefs.SetInt(CachedScoreKey, value);
            PlayerPrefs.Save();
        }
    }

    public static int Score
    {
        get;
        private set;
    }

    public static int Best
    {
        get => PlayerPrefs.GetInt(BestScoreKey, 0);
        private set
        {
            PlayerPrefs.SetInt(BestScoreKey, value);
            PlayerPrefs.Save();
        }
    }

    public static int Level
    {
        get => PlayerPrefs.GetInt(LevelKey, 0);
        private set
        {
            PlayerPrefs.SetInt(LevelKey, value);
            PlayerPrefs.Save();
        }
    }

    public static PlatformBehaviour LastTouchedPlatform { get; private set; }


    public readonly static PlayerDieEvent OnPlayerDie = new();

    public readonly static PlayerEnterPlatformEvent OnEnterPlatform = new();

    public readonly static PlayerContactSectorEvent OnContactSector = new();

    public readonly static PlayerReachFinishEvent OnReachFinish = new();

    public static void OnPlayerDied(PlayerBehaviour player)
    {
        GameState = State.Loss;
        OnPlayerDie.Invoke(player);
    }

    public static void OnFinishReached(PlayerBehaviour player)
    {
        GameState = State.Won;
        OnReachFinish.Invoke(player);
    }

    public static void OnPlatformEntered(PlatformBehaviour platformBehaviour, PlayerBehaviour player) => OnEnterPlatform.Invoke(platformBehaviour, player);

    public static void OnSectorTouched(SectorBehaviour sector, PlayerBehaviour player) => OnContactSector.Invoke(sector, player);

    
    [SerializeField] protected Transform Map;

    [SerializeField] protected ControlsBehaviour Controls;

    [SerializeField] protected LevelGen Generator;

    [SerializeField] protected CameraBehaviour Camera;

    [SerializeField, Min(0.1f)] private float _gameSpeed = 1.5f;

    public float GameSpeed
    {
        get => _gameSpeed;
        protected set => _gameSpeed = Mathf.Max(0.1f, value);
    }


    private void Awake()
    {
        if (Map == null || Controls == null || Generator == null)
            Debug.LogWarning("Level generator, game map and controls should be initialized!");

        Random = new(Level);

        OnPlayerDie.RemoveAllListeners();
        OnEnterPlatform.RemoveAllListeners();
        OnContactSector.RemoveAllListeners();
        OnReachFinish.RemoveAllListeners();

        LevelMap = Map;

        LevelControls = Controls;

        GameCamera = Camera;

        Score = CachedScore;

        Generator.Init();

        Time.timeScale = GameSpeed;
    }

    private void Start()
    {
        OnEnterPlatform.AddListener(e =>
        {
            if (e.Player.TargetY == 0) e.Player.TargetY = Generator.FinishPlatform.gameObject.transform.position.y;
            Score += e.Platform.Score;
            Best = Mathf.Max(Best, Score);
            UI.SyncStats();
        });

        OnContactSector.AddListener(e =>
        {
            PlatformBehaviour platform = e.Player.CurrentPlatform;
            if (LastTouchedPlatform == null) LastTouchedPlatform = platform;

            if (e.Sector.State is not SectorBehaviour.SectorState.Finish and not SectorBehaviour.SectorState.Bad)   // Я человек простой: ВС предложила, я сделал :D
                platform.Damage(LastTouchedPlatform.transform.position.y - platform.transform.position.y);

            LastTouchedPlatform = platform;
        });

        UI.UISetProgress(0);
        UI.UISetScreen(UI.Screen.Ingame);
        GameState = State.Playing;
    }

    public static void ReloadLevel()
    {
        Score = CachedScore;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameState = State.Playing;
        UI.UISetScreen(UI.Screen.Ingame);
    }

    public static void NextLevel()
    {
        Level++;
        CachedScore = Score;
        ReloadLevel();
    }

    public enum State
    {
        Playing,
        Won,
        Loss
    }
}
