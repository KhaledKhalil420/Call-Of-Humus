using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public Weapon heldWeapon;
    public List<Weapon> storedWeapons;

    public Transform pivot;
    public Animator animator;
    public static float speedIncrease = 1;

    public bool canShoot = true;
    public TMP_Text ammoText;

    private void Awake()
    {
        instance = this;
        UpdateWeaponGraphically();
    }

    private void LateUpdate()
    {
        UpdateUI();
        Inputs();
    }

    void UpdateUI()
    {
        if (heldWeapon is Gun currentGun)
        {
            ammoText.text = currentGun.runtimeData.currentAmmo + "/" + currentGun.maxAmmo.ToString();
        }
    }

    void Inputs()
    {
        if (!canShoot) return;

        // Weapon interaction
        if (Input.GetKeyDown(KeyCode.E)) Interact();

        if (heldWeapon == null) return;

        // Shooting
        if (Input.GetMouseButton(0))
        {
            heldWeapon.TriggerWeapon(PlayerLook.mainCamera.transform, animator, speedIncrease);
        }

        if (Input.GetMouseButtonDown(0))
        {
            heldWeapon.TriggerWeapon(PlayerLook.mainCamera.transform, animator, speedIncrease);
        }

        if (Input.GetMouseButtonUp(0))
        {
            heldWeapon.TriggerRelease(PlayerLook.mainCamera.transform, animator, speedIncrease);
        }

        // Reloading
        if (Input.GetKeyDown(KeyCode.R) && heldWeapon is Gun gun)
        {
            StartCoroutine(ReloadWeaponCoroutine(gun));
        }

        // Switching weapons
        for (int i = 1; i <= storedWeapons.Count; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                AudioManager.instance.PlaySound("Switch_Weapon", 1, 1.2f);
                SwitchWeapon(i - 1);
            }
        }
    }

    void SwitchWeapon(int index)
    {
        if (index < 0 || index >= storedWeapons.Count) return;

        heldWeapon = storedWeapons[index];
        UpdateWeaponGraphically();
    }

    IEnumerator ReloadWeaponCoroutine(Gun gun)
    {
        canShoot = false;
        Transform holder = lastModel.transform.parent;
        float reloadTime = gun.reloadTime / speedIncrease;
        Vector3 originalPosition = holder.localPosition;
        Vector3 loweredPosition = originalPosition + Vector3.down * 3f;

        Debug.Log("Reload");

        // Move weapon down
        float elapsed = 0;
        while (elapsed < reloadTime / 2)
        {
            elapsed += Time.deltaTime;
            holder.localPosition = Vector3.Lerp(originalPosition, loweredPosition, elapsed / (reloadTime / 2));
            yield return null;
        }

        // Move weapon back up
        elapsed = 0;
        while (elapsed < reloadTime / 2)
        {
            elapsed += Time.deltaTime;
            holder.localPosition = Vector3.Lerp(loweredPosition, originalPosition, elapsed / (reloadTime / 2));
            yield return null;
        }

        gun.FinishReloading();
        canShoot = true;
    }

    #region Weapon Interactions

    void Interact()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 2f, LayerMask.GetMask("Interactable")))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(gameObject);
            }
        }
    }

    #endregion

    #region Pew Pew Data

    private GameObject lastModel;
    public void UpdateWeaponGraphically()
    {
        if (lastModel != null) Destroy(lastModel);

        if (heldWeapon == null) return;
        lastModel = Instantiate(heldWeapon.model, pivot);
        lastModel.name = lastModel.name.Replace("(Clone)", "");
        lastModel.layer = LayerMask.NameToLayer("Item");

        animator.runtimeAnimatorController = heldWeapon.animator;
        StartCoroutine(UpdateAnimator());
    }

    private IEnumerator UpdateAnimator()
    {
        yield return new WaitForEndOfFrame();
        animator.Rebind();
        animator.Update(0);
    }

    #endregion
}
