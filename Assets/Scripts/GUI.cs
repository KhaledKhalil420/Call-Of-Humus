using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{    
    public TMP_Text waveText;
    public TMP_Text scoreText;

    public Image heartFill;

    public TMP_Text waveTextAnim;
    public Animator anim;

    public void TriggerWaveText(string wave)
    {
        waveTextAnim.text = "Wave: " + wave;
        anim.SetTrigger("Play");
    }

    public void UpdateWaveText(string waveIncrease)
    {
        waveText.text = "Wave: " + waveIncrease;
    }

    public void UpdateScoreText(float scoreIncrese)
    {
        scoreText.text =  scoreIncrese + ": Points";
    }

    public void UpdateHealthFill(float healthAmount)
    {
        heartFill.fillAmount = Mathf.Lerp(heartFill.fillAmount, healthAmount, 15 * Time.deltaTime);
    }
}
