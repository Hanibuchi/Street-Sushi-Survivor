using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionUI : MonoBehaviour
{
    public static SceneTransitionUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _fadeToBlackTrigger = "FadeOut";
    [SerializeField] private string _fadeToClearTrigger = "FadeIn";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 画面を真っ黒にするアニメーションを開始します。
    /// </summary>
    public void FadeToBlack()
    {
        if (_animator != null)
        {
            _animator.SetTrigger(_fadeToBlackTrigger);
        }
    }

    /// <summary>
    /// 画面を元に戻す（透明にする）アニメーションを開始します。
    /// </summary>
    public void FadeToClear()
    {
        if (_animator != null)
        {
            _animator.SetTrigger(_fadeToClearTrigger);
        }
    }
}
