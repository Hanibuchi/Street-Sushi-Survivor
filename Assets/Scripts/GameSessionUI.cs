using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameSessionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _sushiCountText;
    [SerializeField] private TextMeshProUGUI _targetSushiCountText;
    [SerializeField] private Slider _sushiSlider;
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private TextMeshProUGUI _timeOfDayText;

    private float _lastLogTime = 0f;

    private void Start()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.OnTimeChanged += UpdateTimeUI;
            GameSessionManager.Instance.OnSushiCountChanged += UpdateSushiUI;
            GameSessionManager.Instance.OnDayChanged += UpdateDayUI;
            GameSessionManager.Instance.OnRoundChanged += UpdateRoundUI;
            GameSessionManager.Instance.OnTimeOfDayChanged += UpdateTimeOfDayUI;

            // 初期表示の更新
            UpdateTimeUI(GameSessionManager.Instance.RemainingTime);
            UpdateSushiUI(GameSessionManager.Instance.SushiEatenInRound, GameSessionManager.Instance.TargetSushi);
            UpdateDayUI(GameSessionManager.Instance.CurrentDay);
            UpdateRoundUI(GameSessionManager.Instance.CurrentRound);
            UpdateTimeOfDayUI(GameSessionManager.Instance.CurrentTimeOfDay);
        }
    }

    private void OnDestroy()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.OnTimeChanged -= UpdateTimeUI;
            GameSessionManager.Instance.OnSushiCountChanged -= UpdateSushiUI;
            GameSessionManager.Instance.OnDayChanged -= UpdateDayUI;
            GameSessionManager.Instance.OnRoundChanged -= UpdateRoundUI;
            GameSessionManager.Instance.OnTimeOfDayChanged -= UpdateTimeOfDayUI;
        }
    }

    private void UpdateTimeUI(float remainingTime)
    {
        if (_timeText != null)
            _timeText.text = $"{remainingTime:F1}";

        // 1秒ごとにログ出力
        if (Time.time - _lastLogTime >= 1f)
        {
            // Debug.Log($"Remaining Time: {remainingTime:F1}s");
            _lastLogTime = Time.time;
        }
    }

    private void UpdateSushiUI(int current, int target)
    {
        if (_targetSushiCountText != null)
            _targetSushiCountText.text = $"{target}";
        if (_sushiCountText != null)
            _sushiCountText.text = $"{current}";

        if (_sushiSlider != null)
        {
            _sushiSlider.maxValue = target;
            _sushiSlider.value = current;
        }
    }

    private void UpdateDayUI(int day)
    {
        if (_dayText != null)
            _dayText.text = $"{day}日目";
    }

    private void UpdateRoundUI(int round)
    {
        if (_roundText != null)
            _roundText.text = $"Round: {round}";
    }

    private void UpdateTimeOfDayUI(TimeOfDay timeOfDay)
    {
        if (_timeOfDayText == null) return;

        string timeStr = timeOfDay switch
        {
            TimeOfDay.Morning => "朝",
            TimeOfDay.Afternoon => "昼",
            TimeOfDay.Evening => "夜",
            _ => timeOfDay.ToString()
        };

        _timeOfDayText.text = timeStr;
    }
}
