using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Cargo : MonoBehaviour
{
    [Range(1, 25)]
    [SerializeField] private float mass = 1f;

    [SerializeField] private float waterCurrentForce = 100.0f;
    [SerializeField] private Vector3 waterCurrentDirection;

    [SerializeField] private int valueOfCargo = 5;

    [SerializeField] private float killDistance = 40f;

    private Color tint = new Color(0.15f, 0.15f, 0.15f);
    private Color origColor;

    private Rigidbody m_rb;
    private BuoyancyObject m_buoyancyObject;

    private CargoManager cargoManager;

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_buoyancyObject = GetComponent<BuoyancyObject>();
        m_rb.mass = this.mass;
        cargoManager = FindAnyObjectByType<CargoManager>();

        // don't work on player
        if(!CompareTag("Player")){
            origColor = GetComponent<Renderer>().material.color;
        }else{
            origColor = new Color(0, 0, 0);
        }
        
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
            cargoManager.LostCrate();
            if(CompareTag("Player")){
                cargoManager.LostPlayer();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void ApplyPickupColor(){
        origColor = this.GetComponent<Renderer>().material.color;

        Color pickupColor = this.GetComponent<Renderer>().material.color;

        pickupColor += tint;
        this.GetComponent<Renderer>().material.color = pickupColor;
    }
    public void ResetColor(){
        this.GetComponent<Renderer>().material.color = origColor;
    }

}
