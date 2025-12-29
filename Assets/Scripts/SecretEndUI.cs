using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// シークレットエンドの演出を管理するクラス。
/// </summary>
public class SecretEndUI : MonoBehaviour
{
    public static SecretEndUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject _contentRoot;
    [SerializeField] private TextMeshProUGUI _messageText;

    [Header("Visuals")]
    [SerializeField] private Material _secretSkybox;

    [Header("Audio")]
    [SerializeField] private AudioClip _secretBGM;

    [Header("Sequence Settings")]
    [SerializeField] private float _initialDelay = 5.0f;
    [SerializeField] private float _fadeDuration = 1.0f;
    [SerializeField] private float _displayDuration = 2.0f;
    [SerializeField] private float _endDelay = 10.0f;
    [SerializeField] private List<string> _messages = new List<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (_contentRoot != null) _contentRoot.SetActive(false);
        if (_messageText != null)
        {
            _messageText.text = "";
            Color c = _messageText.color;
            c.a = 0f;
            _messageText.color = c;
        }
    }

    /// <summary>
    /// シークレットエンドの演出を開始します。
    /// </summary>
    public void StartSequence()
    {
        StartCoroutine(SecretEndRoutine());
    }

    private IEnumerator SecretEndRoutine()
    {
        // 1. スカイボックスの変更
        if (_secretSkybox != null)
        {
            RenderSettings.skybox = _secretSkybox;
            DynamicGI.UpdateEnvironment(); // 環境光の更新
        }

        // 2. BGMの変更
        if (SoundManager.Instance != null && _secretBGM != null)
        {
            SoundManager.Instance.PlayBGM(_secretBGM);
        }

        // 3. 画面をクリアにする
        if (SceneTransitionUI.Instance != null)
        {
            SceneTransitionUI.Instance.FadeToClear();
        }

        if (_contentRoot != null) _contentRoot.SetActive(true);
        yield return new WaitForSeconds(_initialDelay);

        // 4. メッセージを順番に出す
        foreach (string msg in _messages)
        {
            if (_messageText != null)
            {
                _messageText.text = msg;
                
                // フェードイン
                yield return StartCoroutine(FadeText(0f, 1f));
                
                // 表示維持
                yield return new WaitForSeconds(_displayDuration);
                
                // フェードアウト
                yield return StartCoroutine(FadeText(1f, 0f));
            }
        }

        // 演出終了後、数秒待ってからシーン再読み込み
        yield return new WaitForSeconds(_endDelay);

        // 画面を真っ黒にする
        if (SceneTransitionUI.Instance != null)
        {
            SceneTransitionUI.Instance.FadeToBlack();
            yield return new WaitForSeconds(1.0f);
        }

        // BGMを停止
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBGM();
        }

        // MainGameシーンを再読み込み
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = _messageText.color;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / _fadeDuration);
            _messageText.color = color;
            yield return null;
        }

        color.a = endAlpha;
        _messageText.color = color;
    }
}
