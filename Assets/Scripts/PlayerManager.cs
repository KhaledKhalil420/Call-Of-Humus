using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public static bool isPlayerDead;

    public List<GameObject> players;
    public GameObject localPlayer;
    internal float localPlayerMoney = 0;
    private float allTimeCollected;

    public GUI gui;

    public Weapon[] guns;
    public Animator hitMarkerAniamtor;
    public Transform deathScreen;
    public TMP_Text wavesSurvived;
    public TMP_Text pointsCollected;
    public Animator fadeAnimator;

    public void LockPlayer()
    {
        localPlayer.GetComponent<PlayerInventory>().canShoot = false;
        localPlayer.GetComponent<PlayerMovement>().look.disableLook = true;
        localPlayer.GetComponent<PlayerMovement>().disableMovement = true;
    }

    public void UnlockPlayer()
    {
        localPlayer.GetComponent<PlayerInventory>().canShoot = true;
        localPlayer.GetComponent<PlayerMovement>().look.disableLook = false;
        localPlayer.GetComponent<PlayerMovement>().disableMovement = false;
    }

    public void ChangeMoney(float money)
    {
        localPlayerMoney += money;
        gui.UpdateScoreText(localPlayerMoney);

        if(money > 0)
        allTimeCollected += money;
    }
    
    public Transform GetClosestPlayer(Vector3 locateFrom)
    {
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(locateFrom, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer.transform;
    }

    #region Player Death

    public void TriggerPlayerDeath()
    {
        Time.timeScale = 0.00000001f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LockPlayer();


        deathScreen.gameObject.SetActive(true);

        wavesSurvived.text = "Waves Survived:" + GameManager.instance.currentWaveIndex.ToString();
        pointsCollected.text = "Points Collected: " + allTimeCollected.ToString(); 

        isPlayerDead = true;
    }

    public void Restart()
    {
        StartCoroutine(ResetartScene());
    }

    public IEnumerator ResetartScene()
    {
        fadeAnimator.Play("Fade");
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        isPlayerDead = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

    private void Awake()
    {
        instance = this;

        players = GameObject.FindGameObjectsWithTag("Player").ToList();   

        Debug.Log(GetClosestPlayer(transform.position).name);

        guns = Resources.LoadAll<Weapon>("");
        Array.ForEach(guns, gun => gun.InitializeRuntimeData());
    }

    public void TriggerHitMarker()
    {
        hitMarkerAniamtor.SetTrigger("Trigger");
    }
}
