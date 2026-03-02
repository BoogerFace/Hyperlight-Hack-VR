using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShooting : MonoBehaviour
{
    [Header("General")]
    public Transform shootPoint; //Where Raycast starts from
    public Transform gunPoint; //Where the visual trails from
    public LayerMask layerMask; //What the raycast hits

    [Header("Shooting")]
    public Vector3 spread = new Vector3(0.06f, 0.06f, 0.06f); //How much the raycast can deviate/bulletspread
    public TrailRenderer bulletTrail; //The trail renderer prefab
    private Enemy_References enemyReferences;


    private void Awake()
    {
        enemyReferences = GetComponent<Enemy_References>();
    }

    public void Shoot()
    {
        Vector3 direction = GetDirection();
        if (Physics.Raycast(shootPoint.position, direction, out RaycastHit hit, float.MaxValue, layerMask))
        {
            Debug.DrawLine(shootPoint.position, shootPoint.position + direction * 10f, Color.red, 1f);
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

