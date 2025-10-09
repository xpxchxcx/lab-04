using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownBatController : MonoBehaviour, IControllable
{
    public static event Action OnBatDeath;

    [Header("Sonar")]
    public GameObject sonarPulsePrefab;
    public Transform sonarSpawnPoint;
    public float sonarCooldown = 0.5f;
    private float lastSonarTime = -Mathf.Infinity;

    [Header("Movement")]
    public float maxMoveSpeed = 8f;
    public float acceleration = 15f;
    public float deceleration = 10f;
    public float rotationSpeed = 720f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Camera Behavior")]
    [Range(0f, 2f)]
    public float cameraLerpAggression = 1f;
    public Vector2 targetCharacterPosition = new(0.5f, 0.4f);
    public float targetLookAheadRatio = 0.6f;
    public float lookDirectionDistance = 2f;

    private Rigidbody2D _rb;
    private PlayerActions _playerActions;
    private Vector2 _moveInput;
    private Vector2 _lookDirection = Vector2.up;

    private bool _isDashing = false;
    private float _dashTimeLeft;
    private float _lastDashTime = -Mathf.Infinity;
    private Vector2 _dashDirection;
    private bool _isInControl = true;

    private bool _cameraSettingsChanged = false;

    private Animator _batAnim;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _batAnim = GetComponent<Animator>();
        _playerActions = new PlayerActions();

        _playerActions.BatMovement.Move.performed += OnMove;
        _playerActions.BatMovement.Move.canceled += OnMove;
        _playerActions.BatMovement.Sonar.performed += OnSonar;

    }

    private void OnSonar(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) TrySonarPulse();
    }

    private void TrySonarPulse()
    {
        if (Time.time - lastSonarTime < sonarCooldown) return;

        AudioManager.I.PlaySonar();
        Vector3 spawnPos = sonarSpawnPoint ? sonarSpawnPoint.position : transform.position;
        Instantiate(sonarPulsePrefab, spawnPos, Quaternion.identity);

        lastSonarTime = Time.time;
    }

    private void OnEnable() => _playerActions.BatMovement.Enable();
    private void OnDisable() => _playerActions.BatMovement.Disable();

    private void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();

        _batAnim.SetFloat("moveH", _moveInput.x);
        _batAnim.SetFloat("moveV", _moveInput.y);

        if (_moveInput.sqrMagnitude > 0.01f)
        {
            _lookDirection = _moveInput.normalized;
        }
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) TryDash();
    }

    private void TryDash()
    {
        if (_isDashing || Time.time - _lastDashTime < dashCooldown) return;

        _isDashing = true;
        _dashTimeLeft = dashDuration;
        _dashDirection = _lookDirection.normalized;
        _lastDashTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (_isDashing)
        {
            _rb.linearVelocity = _dashDirection * dashSpeed;
            _dashTimeLeft -= Time.fixedDeltaTime;
            if (_dashTimeLeft <= 0f) _isDashing = false;
            return;
        }

        Vector2 targetVelocity = _moveInput * maxMoveSpeed;
        Vector2 velocityDiff = targetVelocity - _rb.linearVelocity;

        float accel = (_moveInput.magnitude > 0.1f) ? acceleration : deceleration;
        Vector2 force = velocityDiff * accel;
        _rb.AddForce(force);

        // Optional smooth rotation toward look direction
        if (_lookDirection.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRot = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    // === IControllable interface ===
    public bool IsInControl() => _isInControl;

    public bool OnTakeControl()
    {
        _isInControl = true;
        OnEnable();
        return true;
    }

    public void OnControlRemoved()
    {
        _isInControl = false;
        OnDisable();
    }

    private void Start()
    {
        _moveInput = Vector2.zero;
    }

    public Vector3 WorldPosition() => _rb ? _rb.position : Vector3.zero;

    public Vector2 TargetCharacterPosition() => targetCharacterPosition;

    public Vector2 TargetLookAhead()
    {
        if (_rb == null) return Vector2.zero;
        Vector2 velocity = _rb.linearVelocity;
        Vector2 facingBias = _lookDirection.normalized * lookDirectionDistance;

        // Optional: suppress bias if moving very slowly
        if (velocity.magnitude < 0.5f)
            facingBias *= 0.5f;

        return velocity * targetLookAheadRatio + facingBias;
    }

    public float Aggression() => cameraLerpAggression;

    public bool CameraSettingsChanged()
    {
        if (_cameraSettingsChanged)
        {
            _cameraSettingsChanged = false;
            return true;
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("teoenming"))
        {
            OnBatDeath.Invoke();
        }
    }
}
