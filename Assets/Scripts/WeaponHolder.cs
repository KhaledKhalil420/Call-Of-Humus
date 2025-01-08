using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public void TriggerThroughAnimation()
    {
        PlayerInventory.instance.heldWeapon.TriggerAnimation(PlayerLook.mainCamera.transform, PlayerInventory.instance.animator, PlayerInventory.speedIncrease);
    }

    public void PlaySound(string soundName)
    {
        AudioManager.instance.PlaySound(soundName, 1, 1.2f);
    }
}
