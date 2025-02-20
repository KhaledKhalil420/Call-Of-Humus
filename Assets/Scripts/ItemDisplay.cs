using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public Image image;
    public TMP_Text text, descriptionText;
    internal bool canBuy = true;
    internal PurchasableItem purchasableItem;

    internal PlayerManager playerManager;

    public void UpdateDisplay(Sprite sprite, string stringText, string descriptionTetx)
    {
        image.sprite = sprite;
        text.text = stringText;
        descriptionText.text = descriptionTetx;
    }

    public void Trigger()
    {
        switch (purchasableItem.type)
        {
            case PurchasableItem.PurchasableItemType.Hp:
            PurchaseHealth();
            break;

            case PurchasableItem.PurchasableItemType.Weapon:
            PurchaseWeapon();
            break;

            case PurchasableItem.PurchasableItemType.ExplosionsImmunity:
            PurchaseImmunityToExplosions();
            break;
        }
    }

    public void PurchaseWeapon()
    {
        if (canBuy && playerManager.localPlayerMoney >= purchasableItem.price)
        {
            playerManager.ChangeMoney(-purchasableItem.price);
            playerManager.localPlayer.GetComponent<PlayerInventory>().storedWeapons.Add(purchasableItem.weapon);
            playerManager.localPlayer.GetComponent<PlayerInventory>().heldWeapon = purchasableItem.weapon;
            playerManager.localPlayer.GetComponent<PlayerInventory>().UpdateWeaponGraphically();

            AudioManager.instance.PlaySound("Buy", 1, 1.1f);

            GetComponent<Button>().interactable = false;
            canBuy = false;
        }
    }

    public void PurchaseHealth()
    {
        if (canBuy && playerManager.localPlayerMoney >= purchasableItem.price)
        {
            playerManager.ChangeMoney(-purchasableItem.price);
            
            AudioManager.instance.PlaySound("Buy", 1, 1.1f);
            PlayerInventory .instance.GetComponent<PlayerHealth>().maxHp *= 1.5f;
        }        
    }

    public void PurchaseImmunityToExplosions()
    {
        if (canBuy && playerManager.localPlayerMoney >= purchasableItem.price)
        {
            playerManager.ChangeMoney(-purchasableItem.price);
            
            AudioManager.instance.PlaySound("Buy", 1, 1.1f);
            PlayerInventory.instance.GetComponent<PlayerHealth>().isImmuneToExplosions = true;

            GetComponent<Button>().interactable = false;
            canBuy = false;
        }        
    }
}
