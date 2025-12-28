using UnityEngine;

/// <summary>
/// スマホ用操作UI（ジョイスティックやボタン）を管理するクラス。
/// 実行プラットフォームに応じて表示・非表示を切り替えます。
/// </summary>
public class MobileControlsUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject controlsContainer;
    [SerializeField] private bool hideOnNonMobilePlatforms = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (controlsContainer == null)
        {
            controlsContainer = gameObject;
        }

        SetupVisibility();
    }

    public static MobileControlsUI Instance { get; private set; }

    public void SetVisible(bool visible)
    {
        if (controlsContainer != null)
        {
            controlsContainer.SetActive(visible);
        }
    }

    public bool IsVisible => controlsContainer != null && controlsContainer.activeSelf;

    private void SetupVisibility()
    {
        // PlayerPrefs から設定を読み込む（デフォルトはプラットフォーム依存）
        bool defaultVisibility = Application.isMobilePlatform;

        bool isVisible = PlayerPrefs.GetInt("ShowMobileControls", defaultVisibility ? 1 : 0) == 1;
        SetVisible(isVisible);
    }
}
