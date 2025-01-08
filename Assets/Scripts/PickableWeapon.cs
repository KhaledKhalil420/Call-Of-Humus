using System;
using UnityEngine;

public class PickableWeapon : MonoBehaviour, IInteractable
{
    public Weapon weaponData;

    private void Start()
    {
        gameObject.layer = 0;

        Transform[] children = GetComponentsInChildren<Transform>();
        Array.ForEach(children, child => child.gameObject.layer = 0);
    }

    public void Interact(GameObject sender)
    {
        PlayerInventory inventory = sender.GetComponent<PlayerInventory>();

        if(inventory.heldWeapon == null) inventory.TakeWeapon(weaponData);
        Destroy(gameObject);
    }
}
