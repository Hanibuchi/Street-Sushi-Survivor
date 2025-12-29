using UnityEngine;

public class OneMoreUI : MonoBehaviour
{
    /// <summary>
    /// アニメーションイベントから呼び出される、自身を破棄するメソッド。
    /// </summary>
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
