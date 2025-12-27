using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Title : MonoBehaviour
{
    [Header("Start Settings")]
    [SerializeField] private Button _startButton;
    [SerializeField] private AudioClip _startSE;
    [SerializeField] private float _startDelay = 1.5f;
    [SerializeField] private GameObject _titleCamera;

    private bool _isStarted = false;

    private void Start()
    {
        if (_startButton != null)
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }

    private void OnStartButtonClicked()
    {
        if (_isStarted) return;
        _isStarted = true;

        // ボタンを無効化
        if (_startButton != null) _startButton.interactable = false;

        // カメラを非アクティブにする
        if (_titleCamera != null)
        {
            _titleCamera.SetActive(false);
        }

        // 効果音を鳴らす
        if (SoundManager.Instance != null && _startSE != null)
        {
            SoundManager.Instance.PlaySE(_startSE);
        }

        // 一定時間後にゲーム開始を通知
        StartCoroutine(StartGameAfterDelay());
    }

    private IEnumerator StartGameAfterDelay()
    {
        yield return new WaitForSeconds(_startDelay);

        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.StartNewSession();
        }

        // 自身を破棄
        Destroy(gameObject);
    }
}
