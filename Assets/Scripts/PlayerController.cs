using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 720f;
    
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack1";

    [Header("References")]
    [SerializeField] private CharacterController controller;

    private InputAction moveAction;
    private Vector3 velocity;
    [SerializeField] float gravity = -9.81f;

    private void Start()
    {
        // "Move" のリファレンスを探す
        if (InputSystem.actions == null)
        {
            Debug.LogError("InputSystem.actions is null. Please set 'Default Input Actions' in Project Settings > Input System Package.");
            return;
        }

        moveAction = InputSystem.actions.FindAction("Move");
        moveAction?.Enable();

        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (moveAction == null || controller == null) return;

        // 地面接地判定と重力の初期化
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 移動入力の取得
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        // CharacterController を使用した移動
        controller.Move(move * speed * Time.deltaTime);

        // アニメーションの更新
        UpdateAnimation(move);

        // なめらかな回転処理
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 重力の適用
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimation(Vector3 move)
    {
        bool isMoving = move.magnitude > 0.1f;
        
        // 移動している時は走る、止まっている時はIdle
        SetRunning(isMoving);
        SetIdle(!isMoving);
        
        // 歩きが必要な場合は条件に応じて SetWalking を呼ぶ
    }

    public void SetIdle(bool isIdle)
    {
        if (animator != null)
        {
            animator.SetBool("Idle", isIdle);
        }
    }

    public void SetRunning(bool isRunning)
    {
        if (animator != null)
        {
            animator.SetBool("Run Forward", isRunning);
        }
    }

    public void SetWalking(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("WalkForward", isWalking);
        }
    }

    public void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger(attackTriggerName);
        }
    }
}
