using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] private GameObject[] products;
    [SerializeField] private float timer = 5f;

    [SerializeField] private float minOffset = -5f;
    [SerializeField] private float maxOffset = 5f;

    private CountDownTimer countDownTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countDownTimer = new CountDownTimer(timer);
    }

    // Update is called once per frame
    void Update()
    {
        if(countDownTimer.IsReady()){
            SpawnRandomProduct();
            countDownTimer.Reset();
        }
    }

    private void SpawnRandomProduct()
    {
        if (products == null || products.Length == 0)
        {
            Debug.LogWarning("[Spawner] No products assigned!");
            return;
        }

        GameObject prefab = products[Random.Range(0, products.Length)];

        float xOff = Random.Range(minOffset, maxOffset);
        float zOff = Random.Range(minOffset, maxOffset);
        Vector3 spawnPos = transform.position + new Vector3(xOff, 0f, zOff);

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
