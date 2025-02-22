using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingButton : MonoBehaviour
{
    public CanvasGroup canvas;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "Ending")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadEndScene()
    {
        SceneManager.LoadScene("Ending");
    }
}
