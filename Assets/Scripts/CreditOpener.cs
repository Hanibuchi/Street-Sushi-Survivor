using UnityEngine;

/// <summary>
/// クレジットUIのプレハブを生成するためのシンプルなクラス。
/// タイトル画面のボタンなどにアタッチして使用します。
/// </summary>
public class CreditOpener : MonoBehaviour
{
    [Header("Prefab Reference")]
    [SerializeField] private GameObject _creditPrefab;

    /// <summary>
    /// クレジットUIを生成します。
    /// すでに開いている場合は何もしません。
    /// </summary>
    public void OpenCredit()
    {
        if (_creditPrefab == null)
        {
            Debug.LogWarning("Credit Prefab is not assigned in CreditOpener.");
            return;
        }

        // CreditUI側のシングルトンチェックにより、
        // すでに存在する場合は生成直後に破棄されますが、
        // ここでもチェックしておくことで無駄な生成を防ぎます。
        if (CreditUI.Instance == null)
        {
            Instantiate(_creditPrefab);
        }
        else
        {
            Debug.Log("Credit UI is already open.");
        }
    }
}
