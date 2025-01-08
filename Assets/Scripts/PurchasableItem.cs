using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Purchasables")]
public class PurchasableItem : ScriptableObject
{
    public Sprite sprite;
    public Weapon weapon;
    public int price;
}
