using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Purchasables")]
public class PurchasableItem : ScriptableObject
{
    public Weapon weapon;
    public int price;
}
