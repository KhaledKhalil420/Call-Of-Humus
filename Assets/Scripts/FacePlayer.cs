using System.Collections;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z,
        None // No rotation adjustment
    }

    public Axis lockAxis = Axis.Y; // Default axis to lock is Y
    private Transform player;

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        player = PlayerManager.instance.localPlayer.transform;
    }

    private void Update()
    {
        if (player == null) return;

        // Calculate direction to the player
        Vector3 directionToPlayer = player.position - transform.position;

        // Adjust the direction based on the locked axis
        switch (lockAxis)
        {
            case Axis.X:
                directionToPlayer.x = 0f;
                break;
            case Axis.Y:
                directionToPlayer.y = 0f;
                break;
            case Axis.Z:
                directionToPlayer.z = 0f;
                break;
            case Axis.None:
                // Do nothing, retain full direction vector
                break;
        }

        // Rotate only if the direction is meaningful
        if (directionToPlayer.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }
    }
}
