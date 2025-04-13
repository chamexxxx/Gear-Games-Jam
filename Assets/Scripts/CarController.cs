using System;
using Common;
using Player;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Settings")]
    public float moveForce = 4;
    public float turnTorque = 45.5f;
    public float minSpeedForTurning = 0f;  // Минимальная скорость, при которой начинается поворот
    public float maxAngularVelocity = 2f;    // Ограничение на вращение (в рад/сек)
    public float maxSpeed = 1f;              // Максимальная линейная скорость

    private Rigidbody rb;
    private PlayerInput _playerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity; // ограничим вращение
    }

    private void Start()
    {
        var gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager is null)
        {
            Debug.LogError("No GameManager found");
        }
        else
        {
            _playerInput = gameManager.GetPlayerInput();
        }
    }

    private void FixedUpdate()
    {
        if (_playerInput == null) return;

        Vector2 moveInput = _playerInput.movementInput;
        float forwardInput = moveInput.y;
        float turnInput = moveInput.x;

        // Расчёт текущей скорости и направления движения
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = flatVelocity.magnitude;
        float movementDirection = Vector3.Dot(flatVelocity, -transform.right); // >0 — движется вперёд, <0 — назад

        // Движение — только если не превышена максимальная скорость
        if (speed < maxSpeed)
        {
            Vector3 force = -transform.right * forwardInput * moveForce;
            rb.AddForce(force);
        }

        // Поворот — только если машина движется хотя бы чуть-чуть
        if (speed > minSpeedForTurning && Mathf.Abs(turnInput) > 0.01f)
        {
            float direction = movementDirection >= 0f ? 1f : -1f;

            // Масштабируем поворот по скорости
            float speedFactor = Mathf.Clamp01(speed / 10f);
            float torqueAmount = turnInput * turnTorque * direction * speedFactor;

            rb.AddTorque(Vector3.up * torqueAmount, ForceMode.VelocityChange);
        }
    }
}