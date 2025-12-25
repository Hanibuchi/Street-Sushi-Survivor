using UnityEngine;

public class TimeOfDayManager : MonoBehaviour
{
    public static TimeOfDayManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Animator _lightAnimator;

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
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.OnTimeOfDayChanged += SetTimeOfDay;
            // 初期状態の設定
            SetTimeOfDay(GameSessionManager.Instance.CurrentTimeOfDay);
        }
    }

    private void OnDestroy()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.OnTimeOfDayChanged -= SetTimeOfDay;
        }
    }

    /// <summary>
    /// 時間帯に応じたアニメーションを再生します。
    /// </summary>
    /// <param name="timeOfDay">設定する時間帯</param>
    public void SetTimeOfDay(TimeOfDay timeOfDay)
    {
        if (_lightAnimator == null) return;

        switch (timeOfDay)
        {
            case TimeOfDay.Morning:
                _lightAnimator.SetTrigger("Morning");
                break;
            case TimeOfDay.Afternoon:
                _lightAnimator.SetTrigger("Afternoon");
                break;
            case TimeOfDay.Evening:
                _lightAnimator.SetTrigger("Evening");
                break;
        }
    }
}
