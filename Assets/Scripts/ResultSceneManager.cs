using UnityEngine;
using System.Collections;

public class ResultSceneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ResultUI _resultUI;

    [Header("Settings")]
    [SerializeField] private float _uiDisplayDelay = 2.0f;

    [SerializeField] private AudioClip _bGM;

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
