using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _seSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float _bgmVolume = 0.6f;
    [Range(0f, 1f)]
    [SerializeField] private float _seVolume = 0.8f;

    private const string BGM_VOLUME_KEY = "BGM_Volume";
    private const string SE_VOLUME_KEY = "SE_Volume";

    /// <summary>
    /// BGMの音量（0.0 ～ 1.0）
    /// </summary>
    public float BGMVolume
    {
        get => _bgmVolume;
        set
        {
            _bgmVolume = Mathf.Clamp01(value);
            if (_bgmSource != null)
            {
                _bgmSource.volume = _bgmVolume;
            }
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, _bgmVolume);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// SEの音量（0.0 ～ 1.0）
    /// </summary>
    public float SEVolume
    {
        get => _seVolume;
        set
        {
            _seVolume = Mathf.Clamp01(value);
            // PlayOneShotを使用するため、ソース自体の音量は1.0に保ち、再生時に指定するのが一般的ですが、
            // ここではソースの音量も同期させておきます。
            if (_seSource != null)
            {
                _seSource.volume = _seVolume;
            }
            PlayerPrefs.SetFloat(SE_VOLUME_KEY, _seVolume);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSettings()
    {
        _bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, _bgmVolume);
        _seVolume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, _seVolume);

        if (_bgmSource != null) _bgmSource.volume = _bgmVolume;
        if (_seSource != null) _seSource.volume = _seVolume;
    }

    /// <summary>
    /// BGMを再生します。
    /// </summary>
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (_bgmSource == null || clip == null) return;

        _bgmSource.clip = clip;
        _bgmSource.loop = loop;
        _bgmSource.volume = _bgmVolume;
        _bgmSource.Play();
    }

    /// <summary>
    /// BGMを停止します。
    /// </summary>
    public void StopBGM()
    {
        if (_bgmSource != null)
        {
            _bgmSource.Stop();
        }
    }

    /// <summary>
    /// SEを再生します。
    /// </summary>
    /// <param name="clip">再生するオーディオクリップ</param>
    /// <param name="volumeScale">音量の倍率（デフォルトは1.0）</param>
    public void PlaySE(AudioClip clip, float volumeScale = 1f)
    {
        if (_seSource == null || clip == null) return;
        _seSource.PlayOneShot(clip, _seVolume * volumeScale);
    }
}
