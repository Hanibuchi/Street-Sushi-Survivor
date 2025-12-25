using UnityEngine;
using TMPro;

public class DayTransitionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _previousDayText;
    [SerializeField] private TextMeshProUGUI _nextDayText;
    [SerializeField] private Animator _animator;

    /// <summary>
    /// 表示する日数を設定します。
    /// </summary>
    public void Setup(int day)
    {
        if (_previousDayText != null) _previousDayText.text = $"{day - 1}日目";
        if (_nextDayText != null) _nextDayText.text = $"{day}日目";

        if (_animator == null)
        {
            TryGetComponent<Animator>(out _animator);
        }

        if (_animator != null)
        {
            _animator.SetTrigger("Show");
        }
    }

    /// <summary>
    /// アニメーションイベントから呼び出される、自身を非表示にするメソッド。
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
