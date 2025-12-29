using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// クレジットを表示するためのUIクラス。
/// シングルトンとして動作し、重複して生成されないように管理します。
/// </summary>
public class CreditUI : MonoBehaviour
{
    public static CreditUI Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _creditText;
    [SerializeField] private Button _closeButton;

    [Header("Content")]
    [TextArea(10, 20)]
    [SerializeField] private string _creditContent;

    private void Awake()
    {
        // シングルトンのチェック
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 閉じるボタンの設定
        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(Close);
        }

        // テキストの反映
        UpdateText();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// インスペクタで設定されたテキストをUIに反映します。
    /// </summary>
    public void UpdateText()
    {
        if (_creditText != null)
        {
            _creditText.text = _creditContent;
        }
    }

    /// <summary>
    /// クレジット画面を閉じ、自身を破棄します。
    /// </summary>
    public void Close()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 外部からテキストをセットして表示を更新します。
    /// </summary>
    /// <param name="content">表示するクレジット内容</param>
    public void SetCreditContent(string content)
    {
        _creditContent = content;
        UpdateText();
    }
}
