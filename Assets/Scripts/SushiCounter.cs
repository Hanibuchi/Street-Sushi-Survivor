using UnityEngine;

public class SushiCounter : MonoBehaviour
{
    public static SushiCounter Instance { get; private set; }

    private int _totalPoints = 0;
    public int TotalPoints => _totalPoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int points)
    {
        _totalPoints += points;
        Debug.Log($"Sushi Eaten! Points: {points}, Total: {_totalPoints}");
    }
}
