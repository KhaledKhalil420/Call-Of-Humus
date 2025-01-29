using UnityEngine;


public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public bool isPaused = false;

    [Header("UI Elements")] // UI Elements
    public Transform parent;

    [Header("References")] // References
    public PlayerLook playerLook;
    public PlayerInventory playerInventory;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(!PlayerManager.isPlayerDead)
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Trigger();
        }
    }

    public void Trigger()
    {
        isPaused = !isPaused;
        if(isPaused)
            PauseGame();

        else
            ResumeGame();

        parent.gameObject.SetActive(isPaused);
    }

    
    //Load UI Values
    public void PauseGame()
    {
        Time.timeScale = 0.00000001f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerManager.instance.LockPlayer();
        playerLook.enabled = false;
    }

    //Resume
    public void ResumeGame()
    {
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.instance.UnlockPlayer();
        playerLook.enabled = true;
    }
}
