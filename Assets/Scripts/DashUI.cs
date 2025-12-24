using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider _dashSlider;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Color _dashingColor = Color.yellow;
    [SerializeField] private Color _cooldownColor = Color.red;
    [SerializeField] private Color _readyColor = Color.green;

    private void Update()
    {
        if (PlayerController.Instance == null || _dashSlider == null) return;

        PlayerController player = PlayerController.Instance;
        float currentTime = Time.time;

        if (player.IsDashing)
        {
            // ダッシュ中：持続時間の残り割合を表示 (1 -> 0)
            float elapsed = currentTime - player.LastDashStartTime;
            float progress = 1f - (elapsed / player.DashDuration);
            _dashSlider.value = Mathf.Clamp01(progress);
            
            if (_fillImage != null) _fillImage.color = _dashingColor;
        }
        else
        {
            // クールダウン中：回復割合を表示 (0 -> 1)
            float cooldownEndTime = player.LastDashEndTime + player.DashCooldown;
            if (currentTime < cooldownEndTime)
            {
                float elapsed = currentTime - player.LastDashEndTime;
                float progress = elapsed / player.DashCooldown;
                _dashSlider.value = Mathf.Clamp01(progress);
                
                if (_fillImage != null) _fillImage.color = _cooldownColor;
            }
            else
            {
                // 準備完了
                _dashSlider.value = 1f;
                if (_fillImage != null) _fillImage.color = _readyColor;
            }
        }
    }
}
