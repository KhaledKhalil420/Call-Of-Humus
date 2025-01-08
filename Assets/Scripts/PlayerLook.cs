using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float Sensitivity = 50f;
    public Vector2 rotations;  
    public Vector3 offset;
    public Transform Player;
    private CapsuleCollider playerCollider;

    internal bool disableLook;

    public static Camera mainCamera;

    private void Start()
    {   
        transform.parent = null;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rotations.y = transform.localEulerAngles.y;
        playerCollider = Player.GetComponent<CapsuleCollider>();

        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        Follow();

        if(!disableLook)
        Inputs();
        Look();
    }

    void Inputs()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Sensitivity * 0.1f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Sensitivity * 0.1f;

        rotations.x -= mouseY;
        rotations.y += mouseX;

        rotations.x = Mathf.Clamp(rotations.x, -90f, 90f);
    }

    void Look()
    {
        if(Player == null) return;
        
        Player.eulerAngles = new(Player.eulerAngles.x, rotations.y, Player.eulerAngles.z);
        transform.eulerAngles = new(rotations.x, Player.eulerAngles.y, transform.eulerAngles.z);
    }


    void Follow()
    {
        if (playerCollider == null) return;

        transform.position = playerCollider.bounds.center + new Vector3(0, playerCollider.bounds.extents.y, 0) + offset;
    }

    public Vector3 ForwardDirection()
    {
        return transform.forward;
    }
}
