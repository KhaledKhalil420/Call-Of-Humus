using System;
using UnityEngine;

[Serializable]
public struct TiltSettings
{
    public float tiltValue;
    public float tiltSmoothness;

    [HideInInspector] public float angle;
}

[Serializable]
public struct BobSettings
{
    [HideInInspector] public float timer;
    internal float currentSpeed, currentStrength, lerpedX, lerpedY;

    public float speed, strength;

    public bool lerpOnY;
}

public class ViewEffects : MonoBehaviour
{
    public TiltSettings Tilt;
    public BobSettings Bob;

    public PlayerMovement movement;
    internal bool disable = false;

    private void Update()
    {
        if(disable || movement == null) return;
        
        ViewTilt();
        ViewBob();
    }


    private void ViewTilt()
    {
        Tilt.angle = Mathf.Lerp(Tilt.angle, -movement.rawMovingDirection.x * Tilt.tiltValue, Tilt.tiltSmoothness * Time.deltaTime);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Tilt.angle);
    }

    private void ViewBob()
    {
        if(movement.rawMovingDirection.magnitude > 0)
        {
            Bob.currentSpeed = movement.currentVelocity * Bob.speed;
            Bob.currentStrength = movement.currentVelocity * Bob.strength;
        }
        else
        {
            Bob.currentSpeed = 0;
            Bob.currentSpeed = 0;
        }

        Bob.timer += Time.deltaTime;

        float x = Mathf.Sin(Bob.timer * Bob.currentSpeed) * Bob.currentStrength;
        float y = Mathf.Sin(Bob.timer * Bob.currentSpeed / 3) * Bob.currentStrength;

        Bob.lerpedX = Mathf.Lerp(Bob.lerpedX, x, 3 * Time.deltaTime);

        if(Bob.lerpOnY)
        Bob.lerpedY = Mathf.Lerp(Bob.lerpedY, y, 3 * Time.deltaTime);
        
        transform.localEulerAngles = new Vector3(Bob.lerpedX, Bob.lerpedY, transform.localEulerAngles.z);
    }
}
