using UnityEngine;
using System.Collections;

public class TargetMover : MonoBehaviour
{
    public float moveDistance = 3f;
    public float speed = 2f;
    public float respawnDelay = 3f;

    public AudioClip destroySound;
    public AudioSource audioSource;

    private Vector3 startPos;
    private bool isDestroyed = false;

    void Start()
    {
        startPos = transform.position;
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (isDestroyed) return;

        float t = Time.time;
        float xOffset = Mathf.Sin(t * speed) * moveDistance;
        transform.position = startPos + new Vector3(xOffset, 0f, 0f);
    }

    /// <summary>Called only when shot.</summary>
    public void TakeHit()
    {
        if (!isDestroyed)
            StartCoroutine(DestroyAndRespawn());
    }

    private IEnumerator DestroyAndRespawn()
    {
        isDestroyed = true;

        if (destroySound && audioSource)
            audioSource.PlayOneShot(destroySound);

        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(respawnDelay);

        transform.position = startPos;
        transform.localScale = originalScale;
        isDestroyed = false;
    }

    
}
