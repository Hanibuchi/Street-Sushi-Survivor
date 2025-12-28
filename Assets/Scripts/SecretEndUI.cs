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
    [SerializeField] private float _textInterval = 3.0f;
    [SerializeField] float _endDelay = 10.0f;
    [SerializeField] private List<string> _messages = new List<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (_contentRoot != null) _contentRoot.SetActive(false);
        if (_messageText != null) _messageText.text = "";
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
                // フェードインなどの演出をここに入れても良い
            }
            yield return new WaitForSeconds(_textInterval);
        }

        // 演出終了後、数秒待ってからリザルト画面へ（またはタイトルへ）
        yield return new WaitForSeconds(_endDelay);

        // リザルト画面へ遷移
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.LoadResultScene();
        }
    }
}
