using UnityEngine;

public class ResultBear : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;

    private void Start()
    {
        // GameManagerからサイズを適用
        if (GameManager.Instance != null)
        {
            float scale = GameManager.Instance.FinalBearScale;
            transform.localScale = new Vector3(scale, scale, scale);
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
