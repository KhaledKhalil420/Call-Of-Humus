using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public TMP_Text interactText;

    private void Awake()
    {
        instance = this;
        UpdateWeaponGraphically();
    }

    private void LateUpdate()
    {
        Inputs();
        UpdateUI();
        Interact();
    }

    void UpdateUI()
    {
        if (heldWeapon is Weapon currentGun)
        {
            if(currentGun.runtimeData != null)
            ammoText.text = currentGun.runtimeData.currentAmmo + "/" + currentGun.maxAmmo.ToString();
        }
    }

    void Inputs()
    {
        if (!canShoot) return;

        if (heldWeapon == null) return;

        // Shooting
        if (Input.GetMouseButton(0))
        {
            heldWeapon.TriggerWeapon(PlayerLook.mainCamera.transform, animator, speedIncrease, this);
        }

        if (Input.GetMouseButtonDown(0))
        {
            heldWeapon.TriggerWeapon(PlayerLook.mainCamera.transform, animator, speedIncrease, this);
        }

        if (Input.GetMouseButtonUp(0))
        {
            heldWeapon.TriggerRelease(PlayerLook.mainCamera.transform, animator, speedIncrease, this);
        }

        // Reloading
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(heldWeapon is Weapon gun)
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

    IEnumerator ReloadWeaponCoroutine(Weapon gun)
    {
        canShoot = false;
        Transform holder = currentWeaponModel.transform.parent;
        float reloadTime = gun.reloadTime / speedIncrease;
        Vector3 originalPosition = holder.localPosition;
        Vector3 loweredPosition = originalPosition + Vector3.down * 3f;
        AudioManager.instance.PlaySound("Gun_Reload");

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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 8f, LayerMask.GetMask("Interactable")))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactText.enabled = true;

                if(Input.GetKeyDown(KeyCode.E))
                interactable.Interact(gameObject);
            }

            return;
        }

        interactText.enabled = false;
    }

    #endregion

    #region Pew Pew Data

    internal GameObject currentWeaponModel;
    public void UpdateWeaponGraphically()
    {
        if (currentWeaponModel != null) Destroy(currentWeaponModel);

        if (heldWeapon == null) return;
        currentWeaponModel = Instantiate(heldWeapon.model, pivot);
        currentWeaponModel.name = currentWeaponModel.name.Replace("(Clone)", "");
        currentWeaponModel.layer = LayerMask.NameToLayer("Item");

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
