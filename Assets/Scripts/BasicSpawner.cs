using UnityEngine;

public class BasicSpawner : MonoBehaviour
{
    [Header("What to spawn")]
    [Tooltip("Drag your prefab variants here")]
    [SerializeField] private GameObject[] products;

    [Header("Spawn timing")]
    [Tooltip("Seconds between each spawn")]
    [SerializeField] private float timer = 5f;

    [Header("Spawn volume")]
    [Tooltip("Minimum corner of the spawn box (relative to this object)")]
    [SerializeField] private Vector3 minOffset = new Vector3(-5f, 0f, -5f);
    [Tooltip("Maximum corner of the spawn box (relative to this object)")]
    [SerializeField] private Vector3 maxOffset = new Vector3( 5f, 2f,  5f);

    private CountDownTimer countDownTimer;

    void Start()
    {
        countDownTimer = new CountDownTimer(timer);
    }

    void Update()
    {
        if (countDownTimer.IsReady())
        {
            SpawnRandomProduct();
            countDownTimer.Reset();
        }
    }

    private void SpawnRandomProduct()
    {
        if (products == null || products.Length == 0)
        {
            Debug.LogWarning("[BasicSpawner] No products assigned!");
            return;
        }

        GameObject prefab = products[Random.Range(0, products.Length)];

        Vector3 randomOffset = new Vector3(
            Random.Range(minOffset.x, maxOffset.x),
            Random.Range(minOffset.y, maxOffset.y),
            Random.Range(minOffset.z, maxOffset.z)
        );

        Vector3 spawnPos = transform.position + randomOffset;

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
