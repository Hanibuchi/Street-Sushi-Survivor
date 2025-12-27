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
        if (controlsContainer == null)
        {
            controlsContainer = gameObject;
        }

        SetupVisibility();
    }

    private void SetupVisibility()
    {
        if (!hideOnNonMobilePlatforms) return;

        // スマホ（Android, iOS）以外、かつエディタでない場合は非表示にする
        bool isMobile = Application.isMobilePlatform;
        
#if UNITY_EDITOR
        // エディタ上では動作確認のために表示したままにする（必要に応じて変更可能）
        controlsContainer.SetActive(true);
#else
        controlsContainer.SetActive(isMobile);
#endif
    }
}
