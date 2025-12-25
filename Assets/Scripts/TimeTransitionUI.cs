using UnityEngine;
using TMPro;

public class TimeTransitionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _previousTimeText;
    [SerializeField] private TextMeshProUGUI _nextTimeText;
    [SerializeField] private Animator _animator;

    /// <summary>
    /// 表示するテキストを設定します。
    /// </summary>
    public void Setup(string previousTime, string nextTime)
    {
        if (_previousTimeText != null) _previousTimeText.text = previousTime;
        if (_nextTimeText != null) _nextTimeText.text = nextTime;

        if (TryGetComponent<Animator>(out _animator))
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
        // 必要に応じて破棄する場合は Destroy(gameObject);
    }
}
