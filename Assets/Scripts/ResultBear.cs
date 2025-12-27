using UnityEngine;

public class ResultBear : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;

    private void Start()
    {
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
