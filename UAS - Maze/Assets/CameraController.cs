using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public float distance = 3.0f;
    public Vector3 cameraOffset = new Vector3(0, 2.0f, 0);

    [Header("Rotation Settings")]
    public float rotationSpeed = 100.0f;
    public Vector2 pitchLimits = new Vector2(-20, 80);
    public float smoothSpeed = 0.1f;

    [Header("Collision Settings")]
    public LayerMask collisionLayers;
    public float collisionOffset = 0.1f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Vector3 currentPosition;

    void Start()
    {
        currentPosition = transform.position;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + cameraOffset - (rotation * Vector3.forward * distance);

        RaycastHit hit;
        if (Physics.Raycast(target.position + cameraOffset, desiredPosition - (target.position + cameraOffset), out hit, distance, collisionLayers))
        {
            desiredPosition = hit.point - (desiredPosition - (target.position + cameraOffset)).normalized * collisionOffset;
        }

        currentPosition = Vector3.Lerp(currentPosition, desiredPosition, smoothSpeed);
        transform.position = currentPosition;

        transform.LookAt(target.position + cameraOffset);
    }
}
