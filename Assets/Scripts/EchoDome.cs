using UnityEngine;

public class EchoDome : MonoBehaviour
{
    [Header("Echo Dome Settings")]
    public float maxRadius = 12f;
    public float expandSpeed = 15f;
    public float lifeTime = 1.2f;

    private float currentRadius = 0f;
    private float timer = 0f;

    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        // Expand the sphere
        currentRadius += expandSpeed * Time.deltaTime;
        float r = Mathf.Min(currentRadius, maxRadius);

        transform.localScale = Vector3.one * r;

        // Lifetime control
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
