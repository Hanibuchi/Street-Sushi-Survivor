using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float dashSpeed = 25f;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack1";
    [SerializeField] private float attackDashDelay = 0.3f;
    [SerializeField] private float attackDashDuration = 5f;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask obstacleLayers;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip obstacleHitClip;

    [Header("References")]
    [SerializeField] private CharacterController controller;

    private InputAction moveAction;
    private InputAction attackAction;
    private Vector3 velocity;
    [SerializeField] float gravity = -9.81f;

    private bool isAttacking = false;
    private bool isDashing = false;
    private Coroutine attackCoroutine;

    private void Start()
    {
        // "Move" と "Attack" のリファレンスを探す
        if (InputSystem.actions == null)
        {
            Debug.LogError("InputSystem.actions is null. Please set 'Default Input Actions' in Project Settings > Input System Package.");
            return;
        }

        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");

        moveAction?.Enable();
        attackAction?.Enable();

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
        if (controller == null) return;

        // 地面接地判定と重力の初期化
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        HandleAttackInput();

        Vector3 move = Vector3.zero;
        float currentSpeed = isDashing ? dashSpeed : speed;

        // 攻撃の振りかぶり中（isAttacking かつ !isDashing）以外は操作可能
        if (moveAction != null && (!isAttacking || isDashing))
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            move = new Vector3(moveInput.x, 0f, moveInput.y) * currentSpeed;
        }

        // CharacterController を使用した移動
        controller.Move(move * Time.deltaTime);

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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // ダッシュ中に指定したレイヤーのオブジェクトにぶつかったらリセット
        if (isDashing && (obstacleLayers.value & (1 << hit.gameObject.layer)) != 0)
        {
            PlaySE(obstacleHitClip);
            InterruptAction();
        }
    }

    private void HandleAttackInput()
    {
        if (attackAction != null && attackAction.WasPressedThisFrame() && !isAttacking)
        {
            attackCoroutine = StartCoroutine(AttackSequence());
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        TriggerAttack();
        PlaySE(attackClip);

        // 攻撃の振りかぶり待ち
        yield return new WaitForSeconds(attackDashDelay);

        // ダッシュ開始
        isDashing = true;

        yield return new WaitForSeconds(attackDashDuration);

        ResetAttackState();
    }

    /// <summary>
    /// 障害物にぶつかった時などに外部から呼び出すことで、攻撃・ダッシュ状態を強制リセットします。
    /// </summary>
    public void InterruptAction()
    {
        if (isAttacking || isDashing)
        {
            ResetAttackState();
        }
    }

    private void ResetAttackState()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        isAttacking = false;
        isDashing = false;
        // 次のUpdateで通常のUpdateAnimationが呼ばれ、状態が復元されます
    }

    private void UpdateAnimation(Vector3 move)
    {
        bool isMoving = move.magnitude > 0.1f;

        if (isMoving)
        {
            SetIdle(false);
            if (isDashing)
            {
                SetRunning(true);
                SetWalking(false);
            }
            else
            {
                SetRunning(false);
                SetWalking(true);
            }
        }
        else
        {
            SetIdle(true);
            SetRunning(false);
            SetWalking(false);
        }
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

    private void PlaySE(AudioClip clip)
    {
        if (SoundManager.Instance != null && clip != null)
        {
            SoundManager.Instance.PlaySE(clip);
        }
    }
}
