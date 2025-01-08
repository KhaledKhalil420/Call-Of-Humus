using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public bool isRadioOn = false;

    private void Awake()
    {
        instance = this;
    }
}
