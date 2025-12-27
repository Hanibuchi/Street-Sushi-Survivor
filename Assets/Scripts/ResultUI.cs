using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ResultUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _contentRoot;
    [SerializeField] private TextMeshProUGUI _sushiCountText;
    [SerializeField] private Button _titleButton;
    [SerializeField] private Button _tweetButton;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _countDuration = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip _resultSE;
    [SerializeField] private AudioClip _countTickSE;
    [SerializeField] private float _tickInterval = 0.05f;

    [Header("Tweet Settings")]
    [SerializeField] private string _tweetTextFormat = "Street Sushi Survivorで {0} 個の寿司を食べました！ #StreetSushiSurvivor";

    private void Awake()
    {
        if (_contentRoot != null) _contentRoot.SetActive(false);

        if (_titleButton != null) _titleButton.onClick.AddListener(OnTitleButtonClicked);
        if (_tweetButton != null) _tweetButton.onClick.AddListener(OnTweetButtonClicked);
    }

    public void Show()
    {
        // 表示
        if (_contentRoot != null) _contentRoot.SetActive(true);

        // アニメーション開始
        if (_animator != null)
        {
            _animator.SetTrigger("Show");
        }
    }


    /// <summary>
    /// スコアのカウントアップ演出を開始します（Animatorのイベント等から呼び出し可能）
    /// </summary>
    public void StartCountScore()
    {
        if (GameManager.Instance != null)
        {
            StartCoroutine(CountScoreRoutine(GameManager.Instance.TotalSushiEaten));
        }
    }

    private IEnumerator CountScoreRoutine(int targetScore)
    {
        float elapsed = 0f;
        float lastTickTime = 0f;

        while (elapsed < _countDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / _countDuration;
            int currentDisplayScore = Mathf.FloorToInt(targetScore * progress);

            if (_sushiCountText != null)
            {
                _sushiCountText.text = $"{currentDisplayScore}個";
            }

            // 一定時間ごとにカウント音を鳴らす
            if (elapsed - lastTickTime >= _tickInterval)
            {
                if (SoundManager.Instance != null && _countTickSE != null)
                {
                    SoundManager.Instance.PlaySE(_countTickSE);
                }
                lastTickTime = elapsed;
            }

            yield return null;
        }

        // SE再生
        if (SoundManager.Instance != null && _resultSE != null)
        {
            SoundManager.Instance.PlaySE(_resultSE);
        }
        if (_sushiCountText != null)
        {
            _sushiCountText.text = $"{targetScore}個";
        }
    }

    private void OnTitleButtonClicked()
    {
        SceneManager.LoadScene("MainGame");
    }

    private void OnTweetButtonClicked()
    {
        if (GameManager.Instance == null) return;

        string message = string.Format(_tweetTextFormat, GameManager.Instance.TotalSushiEaten);
        string url = "https://twitter.com/intent/tweet?text=" + UnityEngine.Networking.UnityWebRequest.EscapeURL(message);
        Application.OpenURL(url);
    }
}
