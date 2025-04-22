using Unity.Mathematics;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] private GameObject[] products;
    [SerializeField] private float timer = 1f;

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
            Instantiate(products[0], transform.position, quaternion.identity);
            countDownTimer.Reset();
        }
    }
}
