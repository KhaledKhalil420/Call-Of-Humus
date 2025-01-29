using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public Image image;
    public TMP_Text text;
    internal bool canBuy = true;
    internal PurchasableItem purchasableItem;

    internal PlayerManager playerManager;

    public void UpdateDisplay(Sprite sprite, string stringText)
    {
        image.sprite = sprite;
        text.text = stringText;
    }

    public void Purchase()
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
}
