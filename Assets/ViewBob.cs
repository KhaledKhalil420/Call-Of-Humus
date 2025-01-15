using UnityEngine;

public class ViewBob : MonoBehaviour
{
    public BobSettings Bob;

    private void Update()
    {
        ViewBobbing();
    }


    private void ViewBobbing()
    {
        Bob.timer += Time.deltaTime;

        float x = Mathf.Sin(Bob.timer * Bob.currentSpeed) * Bob.currentStrength;
        float y = Mathf.Sin(Bob.timer * Bob.currentSpeed / 3) * Bob.currentStrength;

        Bob.lerpedX = Mathf.Lerp(Bob.lerpedX, x, 3 * Time.deltaTime);

        if(Bob.lerpOnY)
        Bob.lerpedY = Mathf.Lerp(Bob.lerpedY, y, 3 * Time.deltaTime);
        
        transform.localEulerAngles = new Vector3(Bob.lerpedX, Bob.lerpedY, transform.localEulerAngles.z);
    }
}
