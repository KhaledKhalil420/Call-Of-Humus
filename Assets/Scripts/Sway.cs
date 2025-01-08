using UnityEngine;

public class Sway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;
    private Quaternion initializedRotation;

    private void Start()
    {
        initializedRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // get mouse input
        float mouseX = Input.GetAxis("Mouse X") * multiplier * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * multiplier * Time.deltaTime;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
        
        Quaternion targetRotation = rotationX * rotationY * initializedRotation;

        // rotate 
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, smooth * Time.smoothDeltaTime);
    }
}
