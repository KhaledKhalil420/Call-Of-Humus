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

    public List<PurchasableItem> purchasableItems = new();

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
    }

    private void DisplayItem(int num)
    {
        ItemDisplay display = Instantiate(imageDisplay.gameObject, group).GetComponent<ItemDisplay>();

        display.UpdateDisplay(purchasableItems[num].weapon.sprite, purchasableItems[num].price.ToString());
        display.purchasableItem = purchasableItems[num];
        display.playerManager = PlayerManager.instance;
    }

    public void Interact(GameObject sender)
    {
        Exit();
    }


    public void Exit()
    {
        parent.gameObject.SetActive(!parent.gameObject.activeSelf);

        if (parent.gameObject.activeSelf)
        {
            PlayerManager.instance.LockPlayer();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            PlayerManager.instance.UnlockPlayer();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

