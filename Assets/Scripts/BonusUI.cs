using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class BonusUI : MonoBehaviour
{
    [SerializeField] private GameObject _bonusButtonPrefab;
    [SerializeField] private Transform _buttonParent;

    private Action _onComplete;

    /// <summary>
    /// ボーナス選択UIを表示します。
    /// </summary>
    /// <param name="options">表示するボーナスの選択肢</param>
    /// <param name="onComplete">選択が完了した時のコールバック</param>
    public void Show(List<BonusOption> options, Action onComplete)
    {
        _onComplete = onComplete;
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        // 既存のボタンを削除
        foreach (Transform child in _buttonParent)
        {
            Destroy(child.gameObject);
        }

        // 選択肢ボタンを生成
        foreach (var option in options)
        {
            GameObject btnObj = Instantiate(_bonusButtonPrefab, _buttonParent);
            
            // ボタンのテキストを設定（TextMeshProUGUIがボタンの子にある想定）
            var text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = option.description;

            // ボタンのクリックイベントを設定
            var button = btnObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnBonusSelected(option));
            }
        }
    }

    private void OnBonusSelected(BonusOption option)
    {
        // ボーナスを適用
        if (BonusManager.Instance != null)
        {
            BonusManager.Instance.ApplyBonus(option);
        }

        // UIを閉じてコールバックを実行
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        _onComplete?.Invoke();
    }
}
