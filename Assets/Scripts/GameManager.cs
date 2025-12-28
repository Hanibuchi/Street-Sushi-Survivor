using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Results")]
    [SerializeField] float _finalBearScale;
    public float FinalBearScale { get => _finalBearScale; private set => _finalBearScale = value; }
    [SerializeField] int _totalPoints;
    public int TotalSushiEaten { get => _totalPoints; private set => _totalPoints = value; }
    [SerializeField] float _secretEndPlayTime;
    public float SecretEndPlayTime { get => _secretEndPlayTime; private set => _secretEndPlayTime = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ゲームの結果を保存します。
    /// </summary>
    /// <param name="scale">熊の最終的な大きさ</param>
    /// <param name="sushiCount">食べた寿司の総量</param>
    public void SaveResults(float scale, int sushiCount)
    {
        FinalBearScale = scale;
        TotalSushiEaten = sushiCount;
        Debug.Log($"Results Saved: Scale={FinalBearScale}, Sushi={TotalSushiEaten}");
    }

    /// <summary>
    /// シークレットエンド到達時のプレイ時間を保存します。
    /// </summary>
    /// <param name="time">経過時間</param>
    public void SaveSecretEndResult(float time)
    {
        SecretEndPlayTime = time;
        Debug.Log($"Secret End Reached: Time={SecretEndPlayTime}");
    }
}
