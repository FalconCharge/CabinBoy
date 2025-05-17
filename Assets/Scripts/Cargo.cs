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

    private Color tint = new Color(0.25f, 0.25f, 0.25f);
    private Color origColor;
    private float origDrag; 

    float baseLinearDrag;
    float baseAngularDrag;

    private Rigidbody m_rb;

    public int isGrabbed = 0;
    
    public bool adjusted = false;
    private BuoyancyObject m_buoyancyObject;

    private CargoManager cargoManager;

    private float origMass;

    
    [SerializeField] private ParticleSystem splashPrefab;
    [SerializeField] private ParticleSystem splashingPrefab;
    private bool hasSplashed = false;
    [SerializeField] private float particleDown = 2.0f;

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();

        baseLinearDrag = m_rb.linearDamping;
        baseAngularDrag = m_rb.angularDamping;

        m_buoyancyObject = GetComponent<BuoyancyObject>();
        m_rb.mass = this.mass;
        origMass = this.mass;   
        origDrag = m_rb.angularDamping; //ORig drag
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
        if (m_buoyancyObject.IsUnderWater())
        {
            m_rb.AddForce(waterCurrentDirection.normalized * waterCurrentForce * m_rb.mass / 2f, ForceMode.Acceleration);

            if (!hasSplashed)
            {
                AudioManager.Instance.playSplash(0.5f);
                //splash
                Instantiate(splashPrefab, transform.position, Quaternion.identity, this.transform);
                Instantiate(splashingPrefab, transform.position + (Vector3.down * particleDown), Quaternion.identity, this.transform);
                hasSplashed = true;
            }
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

    public void ApplyPickUpDetail(float linearDrag, float angularDrag){
        isGrabbed += 1;

        m_rb.mass = 1;

        Color pickupColor = origColor + tint;

        this.GetComponent<Renderer>().material.color = pickupColor;

        m_rb.linearDamping = linearDrag;
        m_rb.angularDamping = angularDrag;
        //Adjust the rigibody to make changes to the drag same as seen in color and mass

    }
    public void ResetColor(){
        isGrabbed -= 1;
        if(isGrabbed == 0){
            m_rb.mass = origMass;
            this.GetComponent<Renderer>().material.color = origColor;
            m_rb.linearDamping = baseLinearDrag;
            m_rb.angularDamping = baseAngularDrag;
            // Adjust the drag back to it's normal values
        }


    
    }

}
