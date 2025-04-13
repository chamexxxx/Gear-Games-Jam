using System;
using Common;
using Player;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("References")]
    public Transform frontAxle; // Объект, включающий оба передних колеса
    public Transform rearAxle;  // Объект, включающий оба задних колеса

    [Header("Settings")]
    public float moveForce = 150f;
    public float turnSpeed = 50f;
    public float maxTurnAngle = 10f; // Максимальный угол поворота оси
    public float wheelRotationSpeed = 360f; // Скорость вращения колёс

    private Rigidbody rb;
    private PlayerInput _playerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        var playerSwitch = FindAnyObjectByType<GameManager>();
        if (playerSwitch is null)
        {
            Debug.LogError("No SwitchManager registered");
        }
        else
        {
            _playerInput = playerSwitch.GetPlayerInput();
        }
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = _playerInput.movementInput;

        // Направление движения
        float forwardAmount = moveInput.y;
        float turnAmount = moveInput.x;

        // Движение
        Vector3 force = -transform.right * forwardAmount * moveForce * Time.fixedDeltaTime;
        rb.AddForce(force);

        // Поворот
        float turn = turnAmount * turnSpeed * Time.fixedDeltaTime;
        if (Mathf.Abs(forwardAmount) > 0.1f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // Визуальный поворот передней оси (для эффекта)
        // if (frontAxle != null)
        // {
        //     float steerAngle = maxTurnAngle * turnAmount;
        //     frontAxle.localRotation = Quaternion.Euler(0f, steerAngle, 0f);
        // }

        // Вращение осей (визуальный эффект)
        // RotateAxles(forwardAmount);
    }

    // private void RotateAxles(float forwardAmount)
    // {
    //     float rotation = forwardAmount * wheelRotationSpeed * Time.fixedDeltaTime;
    //
    //     if (rearAxle != null)
    //     {
    //         rearAxle.Rotate(rearAxle.right, rotation, Space.World); // вращаем по локальной оси X в глобальном пространстве
    //     }
    //
    //     if (frontAxle != null)
    //     {
    //         frontAxle.Rotate(frontAxle.right, rotation, Space.World);
    //     }
    // }

}