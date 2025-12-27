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

    [Header("Settings")]
    [SerializeField] private float _displayDelay = 2.0f;

    private void Start()
    {
        // 最初は非表示
        if (_contentRoot != null) _contentRoot.SetActive(false);

        StartCoroutine(ShowResultRoutine());
    }

    private IEnumerator ShowResultRoutine()
    {
        yield return new WaitForSeconds(_displayDelay);

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

        // スコア設定
        if (_sushiCountText != null && GameManager.Instance != null)
        {
            _sushiCountText.text = $"{GameManager.Instance.TotalSushiEaten}";
        }

        // 表示
        if (_contentRoot != null) _contentRoot.SetActive(true);
    }
}
