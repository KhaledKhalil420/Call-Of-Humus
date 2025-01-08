using System.Collections;
using UnityEngine;

public class MeleeEnemy : ParentEnemyAI
{
    [Header("Attack Settings")]
    public float attackRange;
    public float attackKnockback = 500;
    public float attackDamage;
    public float attackDelay = 1f;
    private bool canAttack = true;
    private Vector3 facePos;

    [Header("Effects & Sounds")]
    public GameObject death;
    public AudioSource damagedAudio;

    private void Start()
    {
        StartCoroutine(AttackRoutine());
    }

    public override void OnDeath()
    {
        // Use object pooling for death effect
        Instantiate(death, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            settings.animator.SetBool("Twerk", EnemyManager.instance.isRadioOn);

            if (canAttack)
            {
                AttackCheck();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void AttackCheck()
    {
        if (!canAttack) return;

        facePos = transform.position + transform.forward * attackRange;
        Collider[] scannedColliders = Physics.OverlapSphere(facePos, attackRange, settings.playerLayer);

        if (scannedColliders.Length > 0)
        {
            Attack(scannedColliders);
        }
    }

    private void Attack(Collider[] colliders)
    {
        foreach (Collider player in colliders)
        {
            if (player.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damage(attackDamage, settings.bodyCollider, attackKnockback);
            }
        }

        canAttack = false;
        Invoke(nameof(PrepareAttack), attackDelay);
    }

    private void PrepareAttack()
    {
        canAttack = true;
    }

    public override void OnDamage()
    {
        damagedAudio.pitch = Random.Range(0.75f, 1.1f);
        damagedAudio.Play();
        settings.animator.SetTrigger("Damage");
    }
}
