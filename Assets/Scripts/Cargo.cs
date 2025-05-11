using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
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
    
    public bool adjusted = false;
    private BuoyancyObject m_buoyancyObject;

    private CargoManager cargoManager;

    private float origMass;

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_buoyancyObject = GetComponent<BuoyancyObject>();
        m_rb.mass = this.mass;
        origMass = this.mass;
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
            cargoManager.LostCargo(this.gameObject);

            //I think I should destroy here but I'm not sure
            // Destroy gameObject because it's no longer used
            Destroy(this.gameObject);
        }
    }

    public void ApplyPickUpDetail(){
        if(adjusted) return;

        GetComponent<Rigidbody>().mass = 1;

        Color pickupColor = origColor + tint;

        this.GetComponent<Renderer>().material.color = pickupColor;
        
        adjusted = true;
    }
    public void ResetColor(){
        if(adjusted){
            GetComponent<Rigidbody>().mass = origMass;
            this.GetComponent<Renderer>().material.color = origColor;

            adjusted = false;
        }


    }

}
