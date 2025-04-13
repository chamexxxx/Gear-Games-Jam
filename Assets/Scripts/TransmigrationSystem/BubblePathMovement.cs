using System.Collections.Generic;
using Common;
using UnityEngine;

namespace TransmigrationSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class BubblePathMovement : MovableObject
    {
        [Header("Path Settings")]
        public List<Transform> waypoints = new List<Transform>(); // Точки пути как Transform
        public float arrivalThreshold = 0.5f; // Дистанция для смены точки
        public bool loopPath = true; // Зацикливать путь

        [Header("Bubble Physics")]
        public float floatForce = 2f; // Сила всплытия
        public float moveForce = 3f; // Сила движения к точке
        public float maxSpeed = 2f; // Макс. скорость
        public float swayFrequency = 0.5f; // Частота покачивания
        public float swayAmplitude = 0.3f; // Амплитуда покачивания
        public float randomForceFactor = 0.1f; // Случайные воздействия

        private Rigidbody rb;
        private int currentWaypointIndex = 0;
        private Vector3 CurrentWaypointPosition => 
            waypoints[currentWaypointIndex] != null ? 
            waypoints[currentWaypointIndex].position : 
            transform.position;
        private float noiseOffsetX;
        private float noiseOffsetZ;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.linearDamping = 0.5f; // Сопротивление воздуха
            rb.angularDamping = 1f; // Сопротивление вращению
            
            // Инициализация случайных смещений для шума Перлина
            noiseOffsetX = Random.Range(0f, 100f);
            noiseOffsetZ = Random.Range(0f, 100f);
            
            ValidateWaypoints();
        }

        void ValidateWaypoints()
        {
            // Удаляем null-элементы из списка
            waypoints.RemoveAll(item => item == null);
            
            if (waypoints.Count == 0)
            {
                Debug.LogWarning("No waypoints assigned! Using current position as single waypoint.");
                // Создаем временный объект для текущей позиции
                var tempGO = new GameObject("Temp_Waypoint");
                tempGO.transform.position = transform.position;
                waypoints.Add(tempGO.transform);
            }
        }

        void FixedUpdate()
        {
            if (waypoints.Count == 0 || waypoints[currentWaypointIndex] == null)
            {
                ValidateWaypoints();
                return;
            }

            // 1. Добавляем силу всплытия
            rb.AddForce(Vector3.up * floatForce);

            // 2. Вычисляем направление к текущей точке
            Vector3 direction = (CurrentWaypointPosition - transform.position).normalized;
            
            // 3. Добавляем основную силу движения
            rb.AddForce(direction * moveForce);

            // 4. Ограничиваем максимальную скорость
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }

            // 5. Добавляем плавное покачивание
            Vector3 sway = new Vector3(
                Mathf.PerlinNoise(Time.time * swayFrequency, noiseOffsetX) - 0.5f,
                0,
                Mathf.PerlinNoise(noiseOffsetZ, Time.time * swayFrequency) - 0.5f
            ) * swayAmplitude;
            
            rb.AddForce(sway);

            // 6. Случайные воздействия
            if (randomForceFactor > 0)
            {
                rb.AddForce(new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-0.2f, 0.2f),
                    Random.Range(-1f, 1f)
                ) * randomForceFactor);
            }

            // 7. Проверяем достижение точки
            if (Vector3.Distance(transform.position, CurrentWaypointPosition) < arrivalThreshold)
            {
                GetNextWaypoint();
            }
        }

        void GetNextWaypoint()
        {
            currentWaypointIndex++;
            
            if (currentWaypointIndex >= waypoints.Count)
            {
                if (loopPath)
                {
                    currentWaypointIndex = 0;
                }
                else
                {
                    currentWaypointIndex = Mathf.Max(0, waypoints.Count - 1);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (waypoints.Count > 0)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < waypoints.Count; i++)
                {
                    if (waypoints[i] == null) continue;
                    
                    Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                    if (i > 0 && waypoints[i-1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i-1].position, waypoints[i].position);
                    }
                }
                if (loopPath && waypoints.Count > 1 && 
                    waypoints[0] != null && waypoints[waypoints.Count-1] != null)
                {
                    Gizmos.DrawLine(waypoints[waypoints.Count-1].position, waypoints[0].position);
                }
            }
        }

        void OnDestroy()
        {
            // Удаляем временно созданные waypoints
            foreach (var wp in waypoints)
            {
                if (wp != null && wp.name == "Temp_Waypoint")
                {
                    Destroy(wp.gameObject);
                }
            }
        }
    }
}