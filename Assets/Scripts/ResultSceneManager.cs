using UnityEngine;
using System.Collections;

public class ResultSceneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ResultUI _resultUI;

    [Header("Settings")]
    [SerializeField] private float _uiDisplayDelay = 2.0f;

    [Header("Rank Display")]
    [SerializeField] private GameObject[] _rankObjects; // 3つのオブジェクト
    [SerializeField] private int[] _thresholds; // 3つの閾値

    [SerializeField] private AudioClip _bGM;

    private void Awake()
    {
        UpdateRankObjects();
    }

    private void UpdateRankObjects()
    {
        if (GameManager.Instance == null || _rankObjects == null || _thresholds == null || _thresholds.Length < 3) return;

        int score = GameManager.Instance.TotalSushiEaten;

        // すべて非表示にする
        foreach (var obj in _rankObjects)
        {
            if (obj != null) obj.SetActive(false);
        }

        // 閾値判定
        if (score >= _thresholds[0])
        {
            if (_rankObjects[0] != null) _rankObjects[0].SetActive(true);
        }
        if (score >= _thresholds[1])
        {
            if (_rankObjects[1] != null) _rankObjects[1].SetActive(true);
        }
        if (score >= _thresholds[2])
        {
            if (_rankObjects[2] != null) _rankObjects[2].SetActive(true);
        }
    }

    private void Start()
    {
        PlayBGM();
        StartCoroutine(ResultSequence());
    }

    private IEnumerator ResultSequence()
    {
        // 指定された時間待機
        yield return new WaitForSeconds(_uiDisplayDelay);

        // UIを表示（SE/BGM再生も含む）
        if (_resultUI != null)
        {
            _resultUI.Show();
        }
    }

    /// <summary>
    /// BGMを再生します
    /// </summary>
    public void PlayBGM()
    {
        if (SoundManager.Instance != null && _bGM != null)
        {
            SoundManager.Instance.PlayBGM(_bGM, true);
        }
    }
}
