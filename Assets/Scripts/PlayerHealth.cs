using System;
using System.Collections;
using EZCameraShake;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public Rigidbody rb;

    [Header("Hp")]
    public float maxHp;
    private float currentHp;

    [Header("Regeneration")]
    public float regenTime;
    public float regenspeed;
    private float regenTimer;

    [Header("Knockback")]
    public float knockbackTolerance;
    
    [Header("Effects")]
    public CameraShakerPreset damagePreset;
    public AudioLowPassFilter lowPassFilter;
    public Volume healthEffect;
    private Color vignetteColor;
    public Vignette healthVignette;
    public GUI gui;

    private void Start()
    {
        currentHp = maxHp;
        if(healthEffect.profile.TryGet(out Vignette vignette)) healthVignette = vignette;
    }

    private void Update()
    {
        //Health Regeneration
        regenTimer += Time.deltaTime;

        // started healing
        if (regenTimer > regenTime)
        {
            currentHp = Mathf.Min(currentHp + regenspeed * Time.deltaTime, maxHp);

            vignetteColor = Color.green;
        }

        // stopped health
        else
        {
            vignetteColor = Color.red;
        }

        //Update Ui
        gui.UpdateHealthFill(currentHp / maxHp);
        
        //Update Heal UrpVolume
        healthEffect.weight = Mathf.Lerp(healthEffect.weight, 1 - Mathf.InverseLerp(0, maxHp, currentHp), 5 * Time.deltaTime);

        healthVignette.color.value = Color.Lerp(healthVignette.color.value, vignetteColor, 1 * Time.deltaTime);


        float healthNormalized = currentHp / maxHp;
        lowPassFilter.cutoffFrequency = Mathf.Lerp(1000f, 22000f, healthNormalized);
    }

    public void Damage(float damage, Collider collider, float knockback)
    {
        //Damage & Cancel Regen
        currentHp -= damage;
        regenTimer = 0;

        //Effects
        CameraShaker.Instance.ShakeOnce(damagePreset.magnitude, damagePreset.roughness, damagePreset.fadeInTime, damagePreset.fadeOutTime);
        AudioManager.instance.PlaySound("Player_Damaged");

        //Knockback
        Vector3 knockbackDirection = (transform.position - collider.transform.position).normalized;
        knockbackDirection.y = 0;
        rb.AddForce(knockbackDirection * knockback, ForceMode.Acceleration);

        //Death
        if (currentHp <= 0)
        {
            PlayerManager.instance.TriggerPlayerDeath();
        }
    }
}
