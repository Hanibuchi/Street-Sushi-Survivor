using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private CharacterController controller;

    private InputAction moveAction;
    private Vector3 velocity;
    [SerializeField] float gravity = -9.81f;

    private void Start()
    {
        // "Move" と "Jump" のリファレンスを探す
        // Input System 1.7以降のグローバルアクション参照を使用
        if (InputSystem.actions == null)
        {
            Debug.LogError("InputSystem.actions is null. Please set 'Default Input Actions' in Project Settings > Input System Package.");
            return;
        }

        moveAction = InputSystem.actions.FindAction("Move");

        // アクションを有効化する
        moveAction?.Enable();

        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
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
}
