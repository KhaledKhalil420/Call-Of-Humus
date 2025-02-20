using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    private void LateUpdate()
    {
        transform.position = player.position + new Vector3(0, 15, 0);
        transform.eulerAngles = new Vector3(90, player.eulerAngles.y + 90, 90);
    }
}
