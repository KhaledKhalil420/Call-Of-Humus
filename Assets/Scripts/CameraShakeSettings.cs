[System.Serializable]
public class CameraShakeSettings
{
    public float magnitude, roughness, fadeIn, fadeOut;
}

[System.Serializable]
public class GunSettings
{
    public int damage;
    public float range;
    public int bulletsPerShot;
    public float spray;
    public float shootCooldown;
    
    public int magazineSize;
}
