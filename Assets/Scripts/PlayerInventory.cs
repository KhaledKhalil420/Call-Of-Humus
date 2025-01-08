using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        instance = this;
        UpdateWeaponGraphically();
    }

    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(0);
        Inputs();
    }

    void Inputs()
    {
        if(!canShoot) return;
        if(Input.GetKeyDown(KeyCode.E)) Interact();

        if(heldWeapon ==  null) return;
        if(Input.GetMouseButton(0))
        {
            heldWeapon.TriggerWeapon(PlayerLook.mainCamera.transform, animator, speedIncrease);
        }

        if(Input.GetMouseButtonDown(0))
        {
            heldWeapon.TriggerWeapon(PlayerLook.mainCamera.transform, animator, speedIncrease);
        }

        if(Input.GetMouseButtonUp(0))
        {
            heldWeapon.TriggerRelease(PlayerLook.mainCamera.transform, animator, speedIncrease);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            ThrowWeapon(heldWeapon);
        }
        
        for (int i = 0; i < storedWeapons.Count + 1; i++)
        {
            if(Input.GetKeyDown(i.ToString()))
            {
                AudioManager.instance.PlaySound("Switch_Weapon", 1, 1.2f);
                heldWeapon = storedWeapons[i - 1];
                UpdateWeaponGraphically();
                UpdateWeaponLogically();
            }
        }
    }


    #region Weapon Interactions

    void Interact()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, LayerMask.NameToLayer("Interactable")))
        {
            if(hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(gameObject);
            }
        }
    }

    public void TakeWeapon(Weapon weapon)
    {
        if(storedWeapons.Count > 2) 
        {
            ThrowWeapon(weapon);
            return;
        }

        storedWeapons.Add(weapon);
        heldWeapon = weapon;
        UpdateWeaponGraphically();
        UpdateWeaponLogically();
    }

    void ThrowWeapon(Weapon weapon)
    {
        if(weapon != null)
        {
            Rigidbody rb = Instantiate(weapon.model, pivot.position, pivot.rotation).AddComponent<Rigidbody>();
            MeshCollider collider = rb.gameObject.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.sharedMesh = weapon.model.GetComponent<MeshFilter>().sharedMesh;
            rb.gameObject.layer = 8;
            rb.AddForce(pivot.forward * 85, ForceMode.Impulse);
            rb.gameObject.AddComponent<PickableWeapon>().weaponData = weapon;

            weapon = null;
            UpdateWeaponGraphically();
        }
    }

    #endregion

    #region Pew Pew Data
     
    private GameObject lastModel;
    public void UpdateWeaponGraphically()
    {
        // Update model
        if(lastModel != null) Destroy(lastModel);

        if(heldWeapon == null) return;
        lastModel = Instantiate(heldWeapon.model, heldWeapon.model.transform.position, heldWeapon.model.transform.rotation, pivot.transform);
        lastModel.name = lastModel.name.Replace("(Clone)", "");
        lastModel.layer = LayerMask.NameToLayer("Item");
        
        // Update animator
        // Update Animations
        animator.runtimeAnimatorController = heldWeapon.animator;
        StartCoroutine(UpdateAnimator());

        animator.runtimeAnimatorController = heldWeapon.animator;
        animator.Rebind();
        animator.Update(0);     
        animator.Play("Idle", 0, 0);
    }

    private IEnumerator UpdateAnimator()
    {
        lastModel.transform.position = new Vector3(0, 15, 0);
        yield return new WaitForEndOfFrame();

        animator.Update(0);
        animator.Rebind();
    }

    void UpdateWeaponLogically()
    {

    }

    #endregion
}