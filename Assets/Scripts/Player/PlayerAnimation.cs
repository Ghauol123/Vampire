using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private PlayerMoving _playerMoving;
    private SpriteRenderer _spriteRender;
    private float previousHorizontalDirection;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerMoving = GetComponent<PlayerMoving>();
        _spriteRender = GetComponent<SpriteRenderer>();
        previousHorizontalDirection = 1f; // Mặc định nhìn sang phải
    }

    void Update()
    {
        UpdateMovementAnimation();
        UpdateFacingDirection();
    }

    private void UpdateMovementAnimation()
    {
        bool isMoving = _playerMoving.moveDir.x != 0 || _playerMoving.moveDir.y != 0;
        _animator.SetBool("IsRun", isMoving);
    }

    private void UpdateFacingDirection()
    {
        // Nếu đang di chuyển ngang, cập nhật hướng nhìn
        if (_playerMoving.moveDir.x != 0)
        {
            _spriteRender.flipX = _playerMoving.moveDir.x < 0;
            previousHorizontalDirection = _playerMoving.moveDir.x;
        }
        // Nếu chỉ di chuyển dọc, giữ nguyên hướng nhìn cuối cùng
        else if (_playerMoving.moveDir.y != 0)
        {
            _spriteRender.flipX = previousHorizontalDirection < 0;
        }
        // Khi đứng yên, giữ nguyên hướng nhìn cuối cùng
        else
        {
            _spriteRender.flipX = previousHorizontalDirection < 0;
        }
    }
}
