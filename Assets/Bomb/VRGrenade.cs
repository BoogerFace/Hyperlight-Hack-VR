using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class VRGrenade : MonoBehaviour
{
    [Header("Grenade Settings")]
    public float fuseTime = 3f;             // Seconds before explosion
    public float explosionRadius = 5f;      // Radius of damage
    public float explosionForce = 700f;     // Force applied to nearby rigidbodies
    public int damage = 100;                // Damage to enemies
    public GameObject explosionEffectPrefab;
    public float respawnDelay = 3f;         // Seconds to wait before respawn

    [Header("Respawn Settings")]
    public Transform spawnPoint;            // Drag a spawn point in the Inspector

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private bool hasBeenThrown = false;
    private Vector3 originalScale;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;

        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (hasBeenThrown) return;
        hasBeenThrown = true;

        // Make grenade physical after release
        rb.isKinematic = false;

        // Start fuse
        StartCoroutine(FuseCoroutine());
    }

    private IEnumerator FuseCoroutine()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    private void Explode()
    {
        // Spawn explosion effect
        if (explosionEffectPrefab != null)
        {
            // Auto-destroy VFX after its duration (or 3 seconds fallback)
            GameObject fx = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            Destroy(fx, ps ? ps.main.duration : 3f);
        }

        // Damage enemies in radius
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            // Regular enemy
            if (hit.TryGetComponent(out Enemy_Health enemyHealth))
                enemyHealth.TakeDamage(damage);

            // Boss enemy
            if (hit.TryGetComponent(out EnemyBossAI enemyBossAI))
                enemyBossAI.TakeDamage(damage);

            // Melee enemy
            if (hit.TryGetComponent(out EnemyMeleeAI enemyMeleeAI))
                enemyMeleeAI.TakeDamage(damage);

            // Apply explosion force to rigidbodies
            Rigidbody rbHit = hit.attachedRigidbody;
            if (rbHit != null)
                rbHit.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // Shrink before hiding and respawn
        StartCoroutine(ScaleDownAndRespawn());
    }

    private IEnumerator ScaleDownAndRespawn()
    {
        float duration = 0.2f;
        float t = 0;
        Vector3 startScale = transform.localScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / duration);
            yield return null;
        }

        // Disable physics & grabbing while hidden
        rb.isKinematic = true;
        grabInteractable.enabled = false;
        GetComponent<Collider>().enabled = false;

        // Wait before respawn
        yield return new WaitForSeconds(respawnDelay);

        // Move back to spawn point or original position
        if (spawnPoint != null)
            transform.position = spawnPoint.position;

        // Reset scale & state
        transform.localScale = originalScale;
        hasBeenThrown = false;

        // Re-enable components
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // stays kinematic until grabbed again
        grabInteractable.enabled = true;
        GetComponent<Collider>().enabled = true;
    }
}
