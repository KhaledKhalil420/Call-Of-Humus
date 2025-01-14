using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused = false;

    [Header("UI Elements")] // UI Elements
    public Transform parent;

    [Header("References")] // References
    public PlayerLook playerLook;
    public PlayerInventory playerInventory;

    private void Update()
    {
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
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerLook.disableLook = true;
        playerInventory.canShoot = false;
    }

    //Resume
    public void ResumeGame()
    {
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerLook.disableLook = false;
        playerInventory.canShoot = true;
    }
}
