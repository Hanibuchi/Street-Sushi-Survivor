using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class BonusUI : MonoBehaviour
{
    [System.Serializable]
    public struct BonusUIElements
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI dynamicDescriptionText;
        public Button selectButton;
    }

    [SerializeField] private BonusUIElements[] _bonusOptions = new BonusUIElements[3];
    [SerializeField] private Animator _animator;

    private Action _onComplete;
    private List<BonusData> _currentOptions;

    /// <summary>
    /// ボーナス選択UIを表示します。
    /// </summary>
    /// <param name="onComplete">選択が完了した時のコールバック</param>
    public void Show(Action onComplete)
    {
        _onComplete = onComplete;

        if (BonusManager.Instance == null)
        {
            _onComplete?.Invoke();
            return;
        }

        _currentOptions = BonusManager.Instance.GetRandomBonuses(3);
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        if (_animator == null) TryGetComponent<Animator>(out _animator);
        if (_animator != null)
        {
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            _animator.SetTrigger("Show");
        }

        for (int i = 0; i < _bonusOptions.Length; i++)
        {
            if (i < _currentOptions.Count)
            {
                var option = _currentOptions[i];
                var ui = _bonusOptions[i];

                if (ui.nameText != null) ui.nameText.text = option.bonusName;
                if (ui.dynamicDescriptionText != null) ui.dynamicDescriptionText.text = BonusManager.Instance.GetDynamicDescription(option.type);

                if (ui.selectButton != null)
                {
                    ui.selectButton.onClick.RemoveAllListeners();
                    int index = i; // クロージャ用
                    ui.selectButton.onClick.AddListener(() => OnBonusSelected(index));
                    ui.selectButton.gameObject.SetActive(true);
                }
            }
            else
            {
                // 選択肢が足りない場合は非表示
                _bonusOptions[i].selectButton?.gameObject.SetActive(false);
            }
        }
    }

    private void OnBonusSelected(int index)
    {
        if (_currentOptions != null && index < _currentOptions.Count)
        {
            // ボーナスを適用
            if (BonusManager.Instance != null)
            {
                BonusManager.Instance.ApplyBonus(_currentOptions[index]);
            }
        }

        if (_animator != null)
        {
            _animator.SetTrigger("Hide");
        }
        else
        {
            OnHideAnimationComplete();
        }
    }

    public void OnHideAnimationComplete()
    {
        // UIを閉じてコールバックを実行
        gameObject.SetActive(false);
        _onComplete?.Invoke();
    }
}
