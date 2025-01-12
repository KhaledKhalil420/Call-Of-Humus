using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public List<GameObject> players;
    public GameObject localPlayer;
    internal float localPlayerMoney = 0;

    public GUI gui;

    public Gun[] guns;

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

    private void Awake()
    {
        instance = this;

        players = GameObject.FindGameObjectsWithTag("Player").ToList();   

        Debug.Log(GetClosestPlayer(transform.position).name);

        guns = Resources.LoadAll<Gun>("");
        Array.ForEach(guns, gun => gun.InitializeRuntimeData());
    }
}
