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

    [Header("Animators")]
    [SerializeField] private Animator _dayAnimator;
    [SerializeField] private Animator _timeOfDayAnimator;
    [SerializeField] private Animator _targetSushiAnimator;
    [SerializeField] private Animator _sushiCountAnimator;
    [SerializeField] private Animator _timeAnimator;

    [Header("OneMore UI")]
    [SerializeField] private GameObject _oneMoreUIPrefab;
    [SerializeField] private RectTransform _uiSpawnParent;

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

        // 10秒以下の時、1秒ごとにアニメーションを再生
        if (remainingTime <= 10f && remainingTime >= 0)
        {
            // 小数点以下を切り捨てた値が変わったタイミング（＝1秒経過）で実行
            if (Mathf.FloorToInt(remainingTime) != Mathf.FloorToInt(remainingTime + Time.deltaTime))
            {
                if (_timeAnimator != null) _timeAnimator.SetTrigger("Update");
            }
        }

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
        {
            string text = $"{target}";
            if (_targetSushiAnimator != null && _targetSushiCountText.text != text) _targetSushiAnimator.SetTrigger("Update");
            _targetSushiCountText.text = text;
        }
        if (_sushiCountText != null)
        {
            string text = $"{current}";
            if (_sushiCountAnimator != null && _sushiCountText.text != text) _sushiCountAnimator.SetTrigger("Update");
            _sushiCountText.text = text;
        }

        if (_sushiSlider != null)
        {
            _sushiSlider.maxValue = target;
            _sushiSlider.value = current;
        }

        if (current == target - 1)
        {
            if (_oneMoreUIPrefab != null && _uiSpawnParent != null)
            {
                Instantiate(_oneMoreUIPrefab, _uiSpawnParent);
            }
        }
    }

    private void UpdateDayUI(int day)
    {
        if (_dayText != null)
        {
            _dayText.text = $"{day}日目";
            if (_dayAnimator != null) _dayAnimator.SetTrigger("Update");
        }
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
        if (_timeOfDayAnimator != null) _timeOfDayAnimator.SetTrigger("Update");
    }
}
