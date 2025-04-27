using Unity.VisualScripting;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    [Range(1, 25)]
    [SerializeField] private float mass = 1f;

    [SerializeField] private float waterCurrentForce = 100.0f;
    [SerializeField] private Vector3 waterCurrentDirection;

    [SerializeField] private int valueOfCargo = 5;

    [SerializeField] private float killDistance = 40f;

    private Rigidbody m_rb;
    private BuoyancyObject m_buoyancyObject;

    private PlayerScore playerScore;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_buoyancyObject = GetComponent<BuoyancyObject>();
        m_rb.mass = this.mass;
        playerScore = FindAnyObjectByType<PlayerScore>();
        
    }

    void FixedUpdate()
    {
        //Applies force to the cargo
        if(m_buoyancyObject.IsUnderWater()){
            m_rb.AddForce(waterCurrentDirection.normalized * waterCurrentForce * m_rb.mass/2f, ForceMode.Acceleration);
        }
        
    }

    public float GetValue(){
        return valueOfCargo;
    }

    void Update()
    {
        if(transform.position.z > killDistance){
            playerScore.ReducePoints(valueOfCargo);
            Destroy(this.gameObject);
        }
    }

}
