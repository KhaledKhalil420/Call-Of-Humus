
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class Shop : MonoBehaviour, IInteractable
{
    public ItemDisplay imageDisplay;
    public Transform parent;
    public Transform group;
    public Volume volume;
    public SkinnedMeshRenderer skinnedRen;
    public Animator animator;

    public List<PurchasableItem> purchasableItems = new();
    private bool canSell = false;
    private bool isUsing = false;
    
    
    private void Awake()
    {
        purchasableItems = Resources.LoadAll<PurchasableItem>("Shop").ToList();
    }

    private void Start()
    {
        for (int i = 0; i < purchasableItems.Count; i++)
        {
            DisplayItem(i);
        }

        parent.GetComponent<CanvasGroup>().alpha = 1;
        volume.weight = 1;
        parent.gameObject.SetActive(false);
        
        GameManager.instance.OnWaveTriggered += UpdateSellingStatus;
    }

    public void UpdateSellingStatus(object sender, bool args)
    {
        canSell = !args;
        animator.SetBool("IsOpen", canSell);

        if(!canSell)
        {
            PlayerManager.instance.UnlockPlayer();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1;
        }
    }

    private void Update()
    {
        if(isUsing)
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerManager.instance.UnlockPlayer();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PauseMenu.instance.parent.gameObject.SetActive(false);
        }
    }

    private void DisplayItem(int num)
    {
        ItemDisplay display = Instantiate(imageDisplay.gameObject, group).GetComponent<ItemDisplay>();

        display.UpdateDisplay(purchasableItems[num].weapon.sprite, purchasableItems[num].price.ToString(), purchasableItems[num].description.ToString());
        display.purchasableItem = purchasableItems[num];
        display.playerManager = PlayerManager.instance;
    }

    public void Interact(GameObject sender)
    {
        if(canSell)
        Trigger();
    }


    public void Trigger()
    {
        parent.gameObject.SetActive(!parent.gameObject.activeSelf);

        if (parent.gameObject.activeSelf)
        {
            PlayerManager.instance.LockPlayer();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0.00000000001f;

            isUsing = true;
        }
        else
        {
            PlayerManager.instance.UnlockPlayer();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1;

            isUsing = false;
        }
    }
}

