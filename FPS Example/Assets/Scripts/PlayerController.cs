using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Controller Settings")]
    [SerializeField] private float _walkSpeed = 8f;
    [SerializeField] private float _runSpeed = 12f;
    [SerializeField] private float _gravityModifier = 0.95f;
    [SerializeField] private float _jumpPower = 0.25f;
    [SerializeField] private InputAction _newMomentInput;
    [Header("Mouse Control Options")] 
    [SerializeField] private float _mouseSensitivity = 2.5f;
    [SerializeField] private float _maxViewAngle = 60f;

    private CharacterController _characterController;

    private float _currentSpeed = 8f;
    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _heightMovement;
    private bool _jump;

    private Transform mainCamera;
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        if (Camera.main.GetComponent<CameraController>() == null)
        {
            Camera.main.gameObject.AddComponent<CameraController>();
        }
        mainCamera = GameObject.FindWithTag("CameraPoint").transform;
    }

    private void OnEnable()
    {
        _newMomentInput.Enable();
    }

    private void OnDisable()
    {
        _newMomentInput.Disable();
    }
    
    private void Update()
    {
        KeyboardInput();
    }

    private void KeyboardInput()
    {
        _horizontalInput = _newMomentInput.ReadValue<Vector2>().x;
        _verticalInput =  _newMomentInput.ReadValue<Vector2>().y;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && _characterController.isGrounded)
        {
            _jump = true;
        }
       
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            _currentSpeed = _runSpeed;
        }
        else
        {
            _currentSpeed = _walkSpeed;
        }
    }

    private void FixedUpdate() // waits physical operations to run
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        if (_jump)
        {
            _heightMovement.y = _jumpPower;
            _jump = false;
        }

        _heightMovement.y -= _gravityModifier * Time.deltaTime;
        
        Vector3 localVerticalVector = transform.forward * _verticalInput;
        Vector3 localHorizontalVector = transform.right * _horizontalInput;
        Vector3 _movementVector = localHorizontalVector + localVerticalVector;
        _movementVector.Normalize();
        _movementVector *= _currentSpeed * Time.deltaTime;
        _characterController.Move(_movementVector + _heightMovement);

        if (_characterController.isGrounded)
        {
            _heightMovement.y = 0f;
        }
    }

    private void Rotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + MouseInput().x,
            transform.eulerAngles.z);
        if (mainCamera != null)
        {
            if (mainCamera.eulerAngles.x > _maxViewAngle && mainCamera.eulerAngles.x < 180f)
            {
                mainCamera.rotation =
                    Quaternion.Euler(_maxViewAngle, mainCamera.eulerAngles.y, mainCamera.eulerAngles.z);
            }
            else if (mainCamera.eulerAngles.x > 180f && mainCamera.eulerAngles.x < 360f - _maxViewAngle)
            {
                mainCamera.rotation = Quaternion.Euler(360f - _maxViewAngle, mainCamera.eulerAngles.y,
                    mainCamera.eulerAngles.z);
            }
            else
            {
                mainCamera.rotation = Quaternion.Euler(mainCamera.rotation.eulerAngles +
                                                       new Vector3(-MouseInput().y, 0, 0));
            }
        }
    }

    private Vector2 MouseInput()
    {
        return new Vector2(Mouse.current.delta.x.ReadValue(), Mouse.current.delta.y.ReadValue()) * _mouseSensitivity;
    }
}
