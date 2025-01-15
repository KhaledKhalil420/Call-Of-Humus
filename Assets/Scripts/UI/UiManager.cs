using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject currentSelectedObject;
    public Image image;

    public static UiManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        /*
        if(currentSelectedObject != null)
        {
            image.color = Color.Lerp(image.color, new(image.color.r, image.color.g, image.color.b, 100), 10 * Time.deltaTime);
            image.transform.position = Vector3.Lerp(image.transform.position, currentSelectedObject.transform.position, 25 * Time.deltaTime);

            Vector3 size = new(image.rectTransform.sizeDelta .x, currentSelectedObject.GetComponent<RectTransform>().sizeDelta.y);
            image.rectTransform.sizeDelta  = Vector3.Lerp(image.rectTransform.sizeDelta , size, 15 * Time.deltaTime);
        }

        else
        {
            image.color = Color.Lerp(image.color, new(image.color.r, image.color.g, image.color.b, 0), 25 * Time.deltaTime);
        }
        */
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
