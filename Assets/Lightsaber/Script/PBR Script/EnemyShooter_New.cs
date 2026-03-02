using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter_New : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("General")]
    public Transform shootPoint; //Where Raycast starts from
    public Transform gunPoint; //Where the visual trails from
    public LayerMask layerMask; //What the raycast hits

    [Header("Shooting")]
    public Vector3 spread = new Vector3(0.06f, 0.06f, 0.06f); //How much the raycast can deviate/bulletspread
    public TrailRenderer bulletTrail; //The trail renderer prefab
    private Enemy_References enemyReferences;
    public GameObject projectile; // Projectile prefab
    public float projectileSpeed = 20f; // Speed of projectile
    public float fireRate = 1.0f;       // Delay between shots (seconds)

    private float lastFireTime = 0f;    // Tracks cooldown

    public AudioSource audioSource;
    public AudioClip shootSound;

    private void Awake()
    {
        enemyReferences = GetComponent<Enemy_References>();
        
    }

    public void Shoot()
    {

        if (Time.time < lastFireTime + fireRate) 
            return; //still on cooldown
        lastFireTime = Time.time;

        Vector3 direction = GetDirection();
        if (Physics.Raycast(shootPoint.position, direction, out RaycastHit hit, float.MaxValue, layerMask))
        {
            Debug.DrawLine(shootPoint.position, shootPoint.position + direction * 10f, Color.red, 1f);
        }

        if (projectile != null)
        {
            audioSource.PlayOneShot(shootSound);
            GameObject proj = Instantiate(projectile, gunPoint.position, Quaternion.LookRotation(direction));

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
            //if (proj.TryGetComponent<Rigidbody>(out Rigidbody rb))
            //{
            //    rb.velocity = direction * projectileSpeed;
            //}
        }

        TrailRenderer trail = Instantiate(bulletTrail, gunPoint.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, hit));
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;
        direction += new Vector3(
            Random.Range(-spread.x, spread.x),
            Random.Range(-spread.y, spread.y),
            Random.Range(-spread.z, spread.z)
        );
        direction.Normalize();
        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit.point;
        Destroy(trail.gameObject, trail.time);
    }

        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
