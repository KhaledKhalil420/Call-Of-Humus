using UnityEngine;

public class Radio : MonoBehaviour, IInteractable
{
    public bool turnedOn = false;
    public AudioSource source;

    public void Interact(GameObject sender)
    {
        turnedOn = !turnedOn;
        EnemyManager.instance.isRadioOn = turnedOn;
        GetComponent<Animator>().SetBool("on", turnedOn);

        if(turnedOn)
        {
            source.Play();
        }

        else
        {
            source.Stop();
        }
    }
}
