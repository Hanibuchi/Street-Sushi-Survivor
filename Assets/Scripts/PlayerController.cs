using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashCooldown = 10f;
    [SerializeField] private float wasabiStunDuration = 2f;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack1";
    [SerializeField] private float attackDashDelay = 0.3f;
    [SerializeField] private float attackDashDuration = 5f;

    [Header("Attack Effects")]
    [SerializeField] private GameObject shockwavePrefab;
    [SerializeField] private Transform shockwaveSpawnPoint;
    [SerializeField] private float shockwaveSizeMultiplier = 1f;
    public void SetShockwaveSizeMultiplier(float multiplier)
    {
        shockwaveSizeMultiplier = multiplier;
    }

    public void SetRootScale(float scale)
    {
        transform.root.localScale = new Vector3(scale, scale, scale);
    }

    public void SetWasabiStunDuration(float duration)
    {
        wasabiStunDuration = duration;
    }

    public void SetDashCooldown(float cooldown)
    {
        dashCooldown = cooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 寿司との接触判定
        Sushi sushi = other.GetComponentInChildren<Sushi>();
        if (sushi != null)
        {
            if (sushi.IsEaten) return;

            // ワサビレイヤーかどうかの判定
            bool isWasabi = other.gameObject.layer == LayerMask.NameToLayer("Wasabi");

            if (isWasabi)
            {
                PlaySE(wasabiEatClip);
                sushi.Eat();
                OnWasabiHit();
                return;
            }

            int points = sushi.Points;

            sushi.Eat();
            PlaySE(sushiEatClip);

            // 寿司集計用クラスに通知
            if (SushiCounter.Instance != null)
            {
                SushiCounter.Instance.AddPoints(points);
            }
        }
    }

    private void OnWasabiHit()
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }
        stunCoroutine = StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        isStunned = true;
        InterruptAction(); // 攻撃やダッシュを中断

        yield return new WaitForSeconds(wasabiStunDuration);

        isStunned = false;
        stunCoroutine = null;
    }

    [Header("Collision Settings")]
    [SerializeField] private LayerMask obstacleLayers;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip obstacleHitClip;
    [SerializeField] private AudioClip sushiEatClip;
    [SerializeField] private AudioClip wasabiEatClip;

    [Header("References")]
    [SerializeField] private CharacterController controller;

    private InputAction moveAction;
    private InputAction attackAction;
    private Vector3 velocity;
    [SerializeField] float gravity = -9.81f;

    private bool isAttacking = false;
    private bool isDashing = false;
    private bool isStunned = false;
    private float lastDashTime = -999f;
    private Coroutine attackCoroutine;
    private Coroutine stunCoroutine;

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

        Vector3 move = Vector3.zero;

        if (!isStunned)
        {
            HandleAttackInput();

            float currentSpeed = isDashing ? dashSpeed : speed;

            // 攻撃の振りかぶり中（isAttacking かつ !isDashing）以外は操作可能
            if (moveAction != null && (!isAttacking || isDashing))
            {
                Vector2 moveInput = moveAction.ReadValue<Vector2>();
                move = new Vector3(moveInput.x, 0f, moveInput.y) * currentSpeed;
            }

            // なめらかな回転処理
            if (move != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // CharacterController を使用した移動
        controller.Move(move * Time.deltaTime);

        // アニメーションの更新
        UpdateAnimation(move);

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
            // クールダウンのチェック
            if (Time.time >= lastDashTime + dashCooldown)
            {
                attackCoroutine = StartCoroutine(AttackSequence());
            }
            else
            {
                Debug.Log("Dash is on cooldown!");
            }
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        lastDashTime = Time.time; // ダッシュ（攻撃シーケンス）開始時間を記録
        TriggerAttack();
        PlaySE(attackClip);

        // 攻撃の振りかぶり待ち
        yield return new WaitForSeconds(attackDashDelay);
        SpawnShockwave();

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
        if (isStunned)
        {
            SetStunned(true);
            SetIdle(false);
            SetRunning(false);
            SetWalking(false);
            return;
        }

        SetStunned(false);
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

    public void SetStunned(bool isStunned)
    {
        if (animator != null)
        {
            animator.SetBool("Stunned Loop", isStunned);
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

    private void SpawnShockwave()
    {
        if (shockwavePrefab != null)
        {
            // スポーン地点が設定されていればその位置と向き、なければ自身の位置と向きを使用
            Vector3 spawnPos = shockwaveSpawnPoint != null ? shockwaveSpawnPoint.position : transform.position;
            Quaternion spawnRot = shockwaveSpawnPoint != null ? shockwaveSpawnPoint.rotation : transform.rotation;

            GameObject shockwave = Instantiate(shockwavePrefab, spawnPos, spawnRot);

            // ルートのサイズに倍率をかけてスケールを設定
            Vector3 baseScale = transform.root.localScale;
            shockwave.transform.localScale = baseScale * shockwaveSizeMultiplier;
        }
    }
}
