using UnityEngine;
using TMPro;
using System.Collections;

public class ResultUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _contentRoot;
    [SerializeField] private TextMeshProUGUI _sushiCountText;

    [Header("Audio")]
    [SerializeField] private AudioClip _resultBGM;
    [SerializeField] private AudioClip _resultSE;

    public void Setup()
    {
        // 最初は非表示
        if (_contentRoot != null) _contentRoot.SetActive(false);

        // スコア設定
        if (_sushiCountText != null && GameManager.Instance != null)
        {
            _sushiCountText.text = $"{GameManager.Instance.TotalSushiEaten}";
        }
    }

    public void Show()
    {
        // SE再生
        if (SoundManager.Instance != null && _resultSE != null)
        {
            SoundManager.Instance.PlaySE(_resultSE);
        }

        // BGM再生
        if (SoundManager.Instance != null && _resultBGM != null)
        {
            SoundManager.Instance.PlayBGM(_resultBGM, true);
        }

        // 表示
        if (_contentRoot != null) _contentRoot.SetActive(true);
    }
}
