using UnityEngine;

public class Wheel : MonoBehaviour
{
    public WheelCollider wheelCollider;
    public Transform visualWheel;

    public bool isSteering;   // можно ли рулить этим колесом
    public bool isDriving;    // можно ли применять крутящий момент

    public float maxSteerAngle = 30f;
    public float motorTorque = 150f;

    private void Update()
    {
        UpdateVisual();
    }

    public void ApplyInputs(float steerInput, float motorInput)
    {
        if (isSteering)
            wheelCollider.steerAngle = steerInput * maxSteerAngle;

        if (isDriving)
            wheelCollider.motorTorque = motorInput * motorTorque;
    }

    private void UpdateVisual()
    {
        if (visualWheel == null || wheelCollider == null) return;

        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        visualWheel.position = pos;
        visualWheel.rotation = rot;
    }
}