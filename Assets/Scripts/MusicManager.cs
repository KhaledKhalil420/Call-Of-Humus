using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource calm, action;
    public float transitionSpeed;

    //1 for calm -1 for action!
    public float v = 1;

    private void Start()
    {
        GameManager.instance.OnWaveTriggered += UpdateMusic;
    }

    public void UpdateMusic(object sender, bool state)
    {
        v *= -1;
    }

    private void Update()
    {
        calm.volume = Mathf.Lerp(calm.volume, -v, transitionSpeed * Time.deltaTime);
        action.volume = Mathf.Lerp(action.volume, v, transitionSpeed * Time.deltaTime);
    }
}
