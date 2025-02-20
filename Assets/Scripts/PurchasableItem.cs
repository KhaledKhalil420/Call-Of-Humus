using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Purchasables")]
public class PurchasableItem : ScriptableObject
{
    public Weapon weapon;
    public int price;

    public string description;

    public enum PurchasableItemType
    {
        Hp, Weapon, ExplosionsImmunity
    }

    public PurchasableItemType type = PurchasableItemType.Weapon;
}
