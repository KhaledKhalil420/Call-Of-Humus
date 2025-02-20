using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Rigidbody))]
public class ExplosiveProjectile : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionDamage = 50f;
    public float knockbackForce = 10f;
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    public AudioMixerGroup sfxMixer;

    [Header("Movement Settings")]
    public float force = 20f;
    
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Launch();
    }

    private void Launch()
    {
        rb.AddForce((2 * force * transform.forward) + (transform.up * force / 4));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        if (explosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.TryGetComponent(out IDamagable damagable))
            {
                if(!hit.CompareTag("Player"))
                damagable.Damage(explosionDamage, GetComponent<Collider>(), knockbackForce);

                else
                {
                    PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                    if(!playerHealth.isImmuneToExplosions)
                    damagable.Damage(hit.GetComponent<PlayerHealth>().currentHp / 2, GetComponent<Collider>(), knockbackForce);

                    else
                    damagable.Damage(0, GetComponent<Collider>(), knockbackForce);
                }
            }
        }
        
        PlayAudio();
        Destroy(gameObject);
    }

    public void PlayAudio()
    {
        GameObject tempAudio = new ("TempAudio");
        tempAudio.transform.position = transform.position;

        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = explosionSound;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 15f;
        audioSource.maxDistance = 50f;
        audioSource.outputAudioMixerGroup = sfxMixer;

        audioSource.Play();
        Destroy(tempAudio, explosionSound.length);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
