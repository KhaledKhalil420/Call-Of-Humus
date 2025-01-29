using UnityEngine;

public class Sway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth = 5f;
    [SerializeField] private float multiplier = 1f;

    private Vector2 mouseInput;
    private Vector3 initializedEulerAngles;

    private void Start()
    {
        // Cache the initial localEulerAngles
        initializedEulerAngles = transform.localEulerAngles;
    }

    private void Update()
    {
        // Cache mouse input in Update
        mouseInput.x = Input.GetAxis("Mouse X") * multiplier;
        mouseInput.y = Input.GetAxis("Mouse Y") * multiplier;
    }

    private void LateUpdate()
    {
        // Skip calculations if there's no input
        if (mouseInput == Vector2.zero) return;

        // Calculate target euler angles
        Vector3 targetEulerAngles = new Vector3(
            initializedEulerAngles.x - mouseInput.y,
            initializedEulerAngles.y + mouseInput.x,
            initializedEulerAngles.z
        );

        // Smoothly interpolate rotation
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, targetEulerAngles, smooth * Time.deltaTime);
    }
}