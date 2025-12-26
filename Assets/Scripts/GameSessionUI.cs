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

    [Header("Floating Text UI")]
    [SerializeField] private GameObject _floatingTextPrefab;
    [SerializeField] private RectTransform _sushiCountFloatingParent;
    [SerializeField] private RectTransform _targetSushiFloatingParent;
    [SerializeField] private RectTransform _timeFloatingParent;

    [Header("Time Transition UI")]
    [SerializeField] private TimeTransitionUI _timeTransitionUI;

    [Header("Day Transition UI")]
    [SerializeField] private DayTransitionUI _dayTransitionUI;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _timeChangeSE;
    [SerializeField] private AudioClip _dayChangeSE;

    private float _lastLogTime = 0f;
    private TimeOfDay _lastTimeOfDay;
    private int _lastSushiCount;
    private int _lastTargetSushi;
    private float _lastRemainingTime;

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
            _lastRemainingTime = GameSessionManager.Instance.RemainingTime;
            _lastSushiCount = GameSessionManager.Instance.SushiEatenInRound;
            _lastTargetSushi = GameSessionManager.Instance.TargetSushi;

            UpdateTimeUI(_lastRemainingTime);
            UpdateSushiUI(_lastSushiCount, _lastTargetSushi);
            UpdateDayUI(GameSessionManager.Instance.CurrentDay);
            UpdateRoundUI(GameSessionManager.Instance.CurrentRound);

            _lastTimeOfDay = GameSessionManager.Instance.CurrentTimeOfDay;
            UpdateTimeOfDayUI(_lastTimeOfDay);
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
        // 残り時間が増加した場合に浮遊テキストを表示
        if (remainingTime > _lastRemainingTime + 0.1f)
        {
            float diff = remainingTime - _lastRemainingTime;
            SpawnFloatingText(_timeFloatingParent, $"+{diff:F0}");
        }
        _lastRemainingTime = remainingTime;

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
        // 目標寿司数の更新
        if (target != _lastTargetSushi)
        {
            int diff = target - _lastTargetSushi;
            if (diff > 0)
            {
                string sign = diff > 0 ? "+" : "";
                SpawnFloatingText(_targetSushiFloatingParent, $"{sign}{diff}");
            }
            _lastTargetSushi = target;
        }

        // 食べた寿司数の更新
        if (current != _lastSushiCount)
        {
            int diff = current - _lastSushiCount;
            if (diff > 0)
            {
                string sign = diff > 0 ? "+" : "";
                SpawnFloatingText(_sushiCountFloatingParent, $"{sign}{diff}");
            }
            _lastSushiCount = current;
        }

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

        // 日が変わった時に演出用UIを表示
        if (day > 1 && _dayTransitionUI != null)
        {
            _dayTransitionUI.gameObject.SetActive(true);
            _dayTransitionUI.Setup(day);

            // 日替わりSEを再生
            if (SoundManager.Instance != null && _dayChangeSE != null)
            {
                SoundManager.Instance.PlaySE(_dayChangeSE);
            }
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

        string prevStr = GetTimeOfDayJapanese(_lastTimeOfDay);
        string nextStr = GetTimeOfDayJapanese(timeOfDay);

        _timeOfDayText.text = nextStr;
        if (_timeOfDayAnimator != null) _timeOfDayAnimator.SetTrigger("Update");

        // 時間帯が変わった時に演出用UIを表示
        // 日が変わった時は日替わりUIを優先するため、ここでは表示しない
        if (timeOfDay != _lastTimeOfDay && _timeTransitionUI != null && timeOfDay != TimeOfDay.Morning)
        {
            _timeTransitionUI.gameObject.SetActive(true);
            _timeTransitionUI.Setup(prevStr, nextStr);

            // 時間帯変更SEを再生
            if (SoundManager.Instance != null && _timeChangeSE != null)
            {
                SoundManager.Instance.PlaySE(_timeChangeSE);
            }
        }

        _lastTimeOfDay = timeOfDay;
    }

    private string GetTimeOfDayJapanese(TimeOfDay timeOfDay)
    {
        return timeOfDay switch
        {
            TimeOfDay.Morning => "朝",
            TimeOfDay.Afternoon => "昼",
            TimeOfDay.Evening => "夕",
            _ => timeOfDay.ToString()
        };
    }

    private void SpawnFloatingText(RectTransform parent, string text)
    {
        if (_floatingTextPrefab == null || parent == null) return;

        GameObject go = Instantiate(_floatingTextPrefab, parent);
        
        // RectTransformの設定
        RectTransform rect = go.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.pivot = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.anchoredPosition = Vector2.zero;
        }

        var tmp = go.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = text;
        }

        // 2秒後に破棄
        Destroy(go, 2f);
    }
}
