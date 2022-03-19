using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Controller Settings")]
    [SerializeField] private float _walkSpeed = 8f;
    [SerializeField] private float _runSpeed = 12f;

    private CharacterController _characterController;

    private float _currentSpeed = 8f;
    private float _horizontalInput;
    private float _verticalInput;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        KeyboardInput();
    }

    private void KeyboardInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
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
        Vector3 localVerticalVector = transform.forward * _verticalInput;
        Vector3 localHorizontalVector = transform.right * _horizontalInput;
        Vector3 _movementVector = localHorizontalVector + localVerticalVector ;
        _movementVector.Normalize();
        _movementVector *= _currentSpeed * Time.deltaTime;
        _characterController.Move(_movementVector);
    }

}
