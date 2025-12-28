using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    public float Speed => speed;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float dashSpeedRatio = 2f;
    public float DashSpeed => speed * dashSpeedRatio;
    [SerializeField] private float dashCooldown = 10f;
    [SerializeField] private float wasabiStunDuration = 2f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float fallThreshold = -10f;

    [Header("Growth Settings")]
    [SerializeField] private float baseScale = 1f;
    [SerializeField] private float growthPerPoint = 0.01f;
    public float CurrentScale => transform.root.localScale.x;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack1";
    [SerializeField] private string jumpTriggerName = "Jump";
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackDelay = 0.3f;
    [SerializeField] private float attackDashDuration = 5f;

    [Header("Attack Effects")]
    [SerializeField] private GameObject shockwavePrefab;
    [SerializeField] private Transform shockwaveSpawnPoint;
    [SerializeField] private float shockwaveSizeMultiplier = 1f;
    public float ShockwaveSizeMultiplier => shockwaveSizeMultiplier;

    [Header("Sensor Settings")]
    [SerializeField] private SushiSensor sushiSensor;

    private float _baseSpeed;
    private float _baseDashCooldown;
    private float _baseShockwaveSizeMultiplier;
    private float _baseDashSpeedRatio;
    private float _baseDashDuration;
    private float _baseSushiSensorScale;

    public void SetSushiSensorScale(float scale)
    {
        if (sushiSensor != null)
        {
            sushiSensor.SetSensorScale(scale);
        }
    }

    public void SetSushiSensorScaleMultiplier(float multiplier)
    {
        if (sushiSensor != null)
        {
            sushiSensor.SetSensorScale(_baseSushiSensorScale * multiplier);
        }
    }

    public void SetShockwaveSizeMultiplier(float multiplier)
    {
        shockwaveSizeMultiplier = _baseShockwaveSizeMultiplier * multiplier;
    }

    public void SetSpeed(float multiplier)
    {
        speed = _baseSpeed * multiplier;
    }

    public void SetDashSpeedMultiplier(float multiplier)
    {
        dashSpeedRatio = _baseDashSpeedRatio * multiplier;
    }

    public void SetDashDurationMultiplier(float multiplier)
    {
        attackDashDuration = _baseDashDuration * multiplier;
    }

    public void SetRootScale(float scale)
    {
        transform.root.localScale = new Vector3(scale, scale, scale);
    }

    public void SetWasabiStunDuration(float duration)
    {
        wasabiStunDuration = duration;
    }

    public void SetDashCooldown(float multiplier)
    {
        dashCooldown = _baseDashCooldown * multiplier;
    }

    private void OnTriggerEnter(Collider other)
    {
        // セッションが開始されていない、または死亡している場合は何もしない
        if (GameSessionManager.Instance != null && (!GameSessionManager.Instance.IsSessionActive || GameSessionManager.Instance.IsGameOver)) return;
        if (isDead) return;

        // 寿司との接触判定
        Sushi sushi = other.GetComponentInChildren<Sushi>();
        if (sushi != null)
        {
            if (sushi.IsEaten) return;

            // ワサビレイヤーかどうかの判定
            bool isWasabi = other.gameObject.layer == LayerMask.NameToLayer("Wasabi");

            if (isWasabi && !sushi.IsProcessed)
            {
                PlaySE(wasabiEatClip);
                sushi.Eat();
                OnWasabiHit();
                return;
            }

            int points = sushi.Points;

            sushi.Eat();
            PlaySE(sushiEatClip);
            // Debug.Log($"Sushi Eaten: {points} points");

            // ゲームセッション管理クラスに通知
            if (GameSessionManager.Instance != null)
            {
                GameSessionManager.Instance.OnSushiEaten(points);
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
    private InputAction dashAction;
    private InputAction jumpAction;
    private Vector3 velocity;
    [SerializeField] float gravity = -9.81f;

    private bool isAttacking = false;
    private bool isDashing = false;
    private bool isStunned = false;
    private bool isDead = false;
    private float lastDashStartTime = -999f;
    private float lastDashEndTime = -999f;
    private float lastAttackEndTime = -999f;
    private Coroutine attackCoroutine;
    private Coroutine dashCoroutine;
    private Coroutine stunCoroutine;

    public bool IsDashing => isDashing;
    public float DashDuration => attackDashDuration;
    public float DashCooldown => dashCooldown;
    public float LastDashStartTime => lastDashStartTime;
    public float LastDashEndTime => lastDashEndTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _baseSpeed = speed;
            _baseDashCooldown = dashCooldown;
            _baseShockwaveSizeMultiplier = shockwaveSizeMultiplier;
            _baseDashSpeedRatio = dashSpeedRatio;
            _baseDashDuration = attackDashDuration;
            if (sushiSensor != null)
            {
                _baseSushiSensorScale = sushiSensor.transform.localScale.x;
            }
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
        dashAction = InputSystem.actions.FindAction("Sprint");
        jumpAction = InputSystem.actions.FindAction("Jump");

        moveAction?.Enable();
        attackAction?.Enable();
        dashAction?.Enable();
        jumpAction?.Enable();

        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.OnTotalPointsChanged += UpdateScale;
            GameSessionManager.Instance.OnGameOver += OnGameOver;
            UpdateScale(GameSessionManager.Instance.TotalPoints);
        }
    }

    private void OnDestroy()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.OnTotalPointsChanged -= UpdateScale;
            GameSessionManager.Instance.OnGameOver -= OnGameOver;
        }
    }

    private void OnGameOver(GameSessionManager.GameOverType type)
    {
        if (type != GameSessionManager.GameOverType.Secret)
        {
            isDead = true;
            // 入力を無効化
            moveAction?.Disable();
            attackAction?.Disable();
            dashAction?.Disable();
            jumpAction?.Disable();
            // 死亡アニメーション（シークレットエンド以外の場合）
            if (animator != null)
            {
                animator.SetBool("Death", true);
                SetIdle(false);
                SetStunned(false);
                SetRunning(false);
                SetWalking(false);
            }
        }

        // 寿司センサーを無効化
        if (sushiSensor != null)
        {
            sushiSensor.gameObject.SetActive(false);
        }
    }

    private void UpdateScale(int totalPoints)
    {
        // 寿司の量に対して n^0.5 (平方根) のオーダーでサイズを増加させる
        float newScale = baseScale + (Mathf.Sqrt(totalPoints) * growthPerPoint);
        SetRootScale(newScale);
    }

    bool triggerSecret = false;
    private void Update()
    {
        if (controller == null || isDead) return;

        // 地面接地判定と重力の初期化
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = Vector3.zero;

        if (!isStunned)
        {
            HandleAttackInput();
            HandleDashInput();
            HandleJumpInput();

            float currentSpeed = isDashing ? DashSpeed : speed;

            // 操作可能
            if (moveAction != null)
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

        // 落下判定
        if (!triggerSecret && transform.position.y < fallThreshold && !isDead)
        {
            triggerSecret = true;
            if (GameSessionManager.Instance != null)
            {
                GameSessionManager.Instance.TriggerSecretEnd();
            }
        }

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
            if (Time.time >= lastAttackEndTime + attackCooldown)
            {
                attackCoroutine = StartCoroutine(AttackRoutine());
            }
        }
    }

    private void HandleDashInput()
    {
        if (dashAction != null && dashAction.WasPressedThisFrame() && !isDashing)
        {
            // クールダウンのチェック（ダッシュ終了時間から計測）
            if (Time.time >= lastDashEndTime + dashCooldown)
            {
                dashCoroutine = StartCoroutine(DashRoutine());
            }
            else
            {
                Debug.Log("Dash is on cooldown!");
            }
        }
    }

    private void HandleJumpInput()
    {
        if (jumpAction != null && jumpAction.WasPressedThisFrame() && controller.isGrounded)
        {
            // サイズに応じてジャンプ力を調整
            float currentScale = transform.root.localScale.x;
            float scaledJumpHeight = jumpHeight * currentScale;

            // ジャンプ速度の計算: v = sqrt(h * -2 * g)
            velocity.y = Mathf.Sqrt(scaledJumpHeight * -2f * gravity);

            TriggerJump();
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        TriggerAttack();
        PlaySE(attackClip);

        // 攻撃の振りかぶり待ち
        yield return new WaitForSeconds(attackDelay);
        SpawnShockwave();

        isAttacking = false;
        lastAttackEndTime = Time.time;
        attackCoroutine = null;
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        lastDashStartTime = Time.time;

        yield return new WaitForSeconds(attackDashDuration);

        isDashing = false;
        lastDashEndTime = Time.time;
        dashCoroutine = null;
    }

    /// <summary>
    /// 障害物にぶつかった時などに外部から呼び出すことで、ダッシュ状態を強制リセットします。
    /// </summary>
    public void InterruptAction()
    {
        if (isDashing)
        {
            if (dashCoroutine != null) StopCoroutine(dashCoroutine);
            isDashing = false;
            lastDashEndTime = Time.time;
            dashCoroutine = null;
        }
    }

    private void ResetAttackState()
    {
        InterruptAction();
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

    public void TriggerJump()
    {
        if (animator != null)
        {
            animator.SetTrigger(jumpTriggerName);
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
