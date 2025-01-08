using System;
using System.Collections;
using EZCameraShake;
using UnityEngine;
using UnityEngine.Rendering;
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
    public Volume damagedEffect;
    public AnimationCurve damageEffectCurve;
    public float effectDuration = 1f;
    public GUI gui;

    private void Start()
    {
        currentHp = maxHp;
    }

    private void Update()
    {
        regenTimer += Time.deltaTime;

        if (regenTimer > regenTime)
        {
            currentHp = Mathf.Min(currentHp + regenspeed * Time.deltaTime, maxHp);
        }

        gui.UpdateHealthFill(currentHp / maxHp);
    }

    public void Damage(float damage, Collider collider, float knockback)
    {
        //Damage & Cancel Regen
        currentHp -= damage;
        regenTimer = 0;

        //Effects
        CameraShaker.Instance.ShakeOnce(5, 5, 0.05f, 1);
        AudioManager.instance.PlaySound("Player_Damaged");
        StartCoroutine(DamagedEffect());

        //Knockback
        Vector3 knockbackDirection = (transform.position - collider.transform.position).normalized;
        knockbackDirection.y = 0;
        rb.AddForce(knockbackDirection * knockback, ForceMode.Acceleration);

        //Death
        if (currentHp <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator DamagedEffect()
    {
        float elapsedTime = 0f;

        while (elapsedTime < effectDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / effectDuration);
            damagedEffect.weight = damageEffectCurve.Evaluate(normalizedTime);
            yield return null;
        }

        damagedEffect.weight = 0f;
    }
}
