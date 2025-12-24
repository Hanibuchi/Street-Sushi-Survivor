using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class VolumeSettingsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _seSlider;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip _volumeTestClip;
    [SerializeField] private float _testSEInterval = 0.5f;

    private float _lastTestSETime;
    private InputAction _menuAction;
    private float _previousTimeScale = 1f;

    private void Awake()
    {
        // InputSystemの"Menu"アクション（Escキーなど）を設定
        _menuAction = new InputAction("Menu", binding: "<Keyboard>/escape");
        _menuAction.performed += _ => TogglePanel();
    }

    private void OnEnable()
    {
        _menuAction?.Enable();
    }

    private void OnDisable()
    {
        _menuAction?.Disable();
    }

    private void Start()
    {
        // 初期値の設定
        if (SoundManager.Instance != null)
        {
            _bgmSlider.value = SoundManager.Instance.BGMVolume;
            _seSlider.value = SoundManager.Instance.SEVolume;
        }

        // イベントの登録
        _bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        _seSlider.onValueChanged.AddListener(OnSEVolumeChanged);
        
        _openButton.onClick.AddListener(OpenPanel);
        _closeButton.onClick.AddListener(ClosePanel);

        // 最初はパネルを閉じておく
        _settingsPanel.SetActive(false);
    }

    private void OnBGMVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.BGMVolume = value;
        }
    }

    private void OnSEVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SEVolume = value;

            // スライダーを動かしている間、一定間隔でテスト音を鳴らす
            if (Time.time > _lastTestSETime + _testSEInterval && _volumeTestClip != null)
            {
                SoundManager.Instance.PlaySE(_volumeTestClip);
                _lastTestSETime = Time.time;
            }
        }
    }

    public void TogglePanel()
    {
        if (_settingsPanel.activeSelf)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    public void OpenPanel()
    {
        _settingsPanel.SetActive(true);
        
        // 一時停止処理
        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        
        // パネルを開いた時に最新の値を反映
        if (SoundManager.Instance != null)
        {
            _bgmSlider.value = SoundManager.Instance.BGMVolume;
            _seSlider.value = SoundManager.Instance.SEVolume;
        }
    }

    public void ClosePanel()
    {
        _settingsPanel.SetActive(false);
        
        // 再開処理
        Time.timeScale = _previousTimeScale;
    }
}
