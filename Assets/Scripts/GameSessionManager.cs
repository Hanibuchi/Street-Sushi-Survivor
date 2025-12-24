using UnityEngine;
using System.Collections;
using System;

public enum TimeOfDay
{
    Morning,
    Afternoon,
    Evening
}

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    [Header("Session Settings")]
    [SerializeField] private float _initialRoundTime = 30f;
    [SerializeField] private float[] _timeIncreasePerDayArray = new float[] { 10f, 15f, 20f };
    [SerializeField] private int[] _targetSushiPerDayArray = new int[] { 5, 8, 12 };

    [Header("UI References")]
    [SerializeField] private GameObject _oneMoreUIPrefab;
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameObject _bonusSelectionUI;

    private int _totalPoints = 0;
    private int _currentDay = 1;
    private TimeOfDay _currentTimeOfDay = TimeOfDay.Morning;
    private int _currentRound = 1;
    private float _remainingTime;
    private int _targetSushi;
    private int _sushiEatenInRound;
    private bool _isGameOver = false;
    private bool _isPaused = false;

    public int TotalPoints => _totalPoints;
    public int CurrentDay => _currentDay;
    public TimeOfDay CurrentTimeOfDay => _currentTimeOfDay;
    public int CurrentRound => _currentRound;
    public float RemainingTime => _remainingTime;
    public int TargetSushi => _targetSushi;
    public int SushiEatenInRound => _sushiEatenInRound;

    public event Action OnRoundStart;
    public event Action OnRoundComplete;
    public event Action OnGameOver;
    public event Action<float> OnTimeChanged;
    public event Action<int, int> OnSushiCountChanged;
    public event Action<int> OnDayChanged;
    public event Action<int> OnRoundChanged;
    public event Action<TimeOfDay> OnTimeOfDayChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartNewSession();
    }

    public void StartNewSession()
    {
        _totalPoints = 0;
        _currentDay = 1;
        _currentTimeOfDay = TimeOfDay.Morning;
        _currentRound = 1;
        UpdateTargetSushi();
        _remainingTime = _initialRoundTime;
        _isGameOver = false;
        _sushiEatenInRound = 0;

        OnDayChanged?.Invoke(_currentDay);
        OnTimeOfDayChanged?.Invoke(_currentTimeOfDay);
        OnRoundChanged?.Invoke(_currentRound);
        OnSushiCountChanged?.Invoke(_sushiEatenInRound, _targetSushi);

        StartRound();
    }

    private void UpdateTargetSushi()
    {
        if (_targetSushiPerDayArray != null && _targetSushiPerDayArray.Length > 0)
        {
            int index = Mathf.Clamp(_currentDay - 1, 0, _targetSushiPerDayArray.Length - 1);
            _targetSushi = _targetSushiPerDayArray[index];
        }
    }

    private void StartRound()
    {
        _sushiEatenInRound = 0;
        _isPaused = false;
        OnRoundStart?.Invoke();
    }

    private void Update()
    {
        if (_isGameOver || _isPaused) return;

        _remainingTime -= Time.deltaTime;
        OnTimeChanged?.Invoke(_remainingTime);

        if (_remainingTime <= 0)
        {
            GameOver();
        }
    }

    public void OnSushiEaten(int points)
    {
        if (_isGameOver) return;

        _totalPoints += points;
        _sushiEatenInRound += points;
        OnSushiCountChanged?.Invoke(_sushiEatenInRound, _targetSushi);
        Debug.Log($"Sushi Eaten: {points} points, Total: {_totalPoints}, Round: {_sushiEatenInRound}/{_targetSushi}");

        if (_sushiEatenInRound == _targetSushi - 1)
        {
            ShowOneMoreUI();
        }

        if (_sushiEatenInRound >= _targetSushi)
        {
            CompleteRound();
        }
    }

    public void AddTime(float seconds)
    {
        _remainingTime += seconds;
    }

    private void ShowOneMoreUI()
    {
        if (_oneMoreUIPrefab != null)
        {
            // Instantiate or enable "One More!" UI
            GameObject ui = Instantiate(_oneMoreUIPrefab);
            Destroy(ui, 2f);
        }
        Debug.Log("もうひとつ！ (One More!)");
    }

    private void CompleteRound()
    {
        _isPaused = true;
        OnRoundComplete?.Invoke();

        // Progress Time of Day
        if (_currentTimeOfDay == TimeOfDay.Morning) _currentTimeOfDay = TimeOfDay.Afternoon;
        else if (_currentTimeOfDay == TimeOfDay.Afternoon) _currentTimeOfDay = TimeOfDay.Evening;
        else
        {
            _currentTimeOfDay = TimeOfDay.Morning;
            _currentDay++;
            OnDayChanged?.Invoke(_currentDay);
            ApplyDayEndBonus();
        }
        OnTimeOfDayChanged?.Invoke(_currentTimeOfDay);

        _currentRound++;
        OnRoundChanged?.Invoke(_currentRound);
        
        // ターゲット寿司数を更新
        UpdateTargetSushi();

        // 日ごとの追加時間を配列から取得（配列外の場合は最後の要素を使用）
        float timeIncrease = 0f;
        if (_timeIncreasePerDayArray != null && _timeIncreasePerDayArray.Length > 0)
        {
            int index = Mathf.Clamp(_currentDay - 1, 0, _timeIncreasePerDayArray.Length - 1);
            timeIncrease = _timeIncreasePerDayArray[index];
        }
        
        _remainingTime += timeIncrease;
        OnSushiCountChanged?.Invoke(_sushiEatenInRound, _targetSushi);

        ShowBonusSelection();
    }

    private void ApplyDayEndBonus()
    {
        Debug.Log($"Day {_currentDay - 1} Complete! Bonus Applied.");
        // Add points or other permanent bonuses here
        _totalPoints += 100 * (_currentDay - 1);
    }

    private void ShowBonusSelection()
    {
        if (_bonusSelectionUI != null)
        {
            _bonusSelectionUI.SetActive(true);
            Time.timeScale = 0f; // Pause for selection
        }
        else
        {
            // If no UI, just start next round
            StartRound();
        }
    }

    public void OnBonusSelected()
    {
        _bonusSelectionUI.SetActive(false);
        Time.timeScale = 1f;
        StartRound();
    }

    private void GameOver()
    {
        _isGameOver = true;
        Time.timeScale = 0f;
        if (_gameOverUI != null) _gameOverUI.SetActive(true);
        OnGameOver?.Invoke();
        Debug.Log("Game Over!");
    }
}
