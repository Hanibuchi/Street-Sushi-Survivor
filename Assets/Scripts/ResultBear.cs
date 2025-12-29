using UnityEngine;

public class ResultBear : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _secretObject;

    private void Start()
    {
        // GameManagerからサイズを適用
        if (GameManager.Instance != null)
        {
            float scale = GameManager.Instance.FinalBearScale;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        // シークレットエンドが開放されている場合、オブジェクトをアクティブにする
        if (_secretObject != null)
        {
            bool isAchieved = PlayerPrefs.GetInt("SecretEndAchieved", 0) == 1;
            _secretObject.SetActive(isAchieved);
        }

        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        if (_animator != null)
        {
            _animator.SetBool("Idle", false);
            _animator.SetBool("Eat", true);
        }
    }
}
