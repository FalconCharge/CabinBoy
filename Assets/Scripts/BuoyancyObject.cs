using System;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyObject : MonoBehaviour
{
    [SerializeField] private Transform[] floaters;
    [SerializeField] private float underWaterDrag = 3f;
    [SerializeField] private float underWaterAngularDrag = 1f;
    [SerializeField] private float airDrag = 0f;
    [SerializeField] private float airAngularDrag = 0f;

    [SerializeField] private float waterHeightOffset = 1f;

    [SerializeField] private float floatingPower = 15f;

    private Rigidbody rb;
    private OceanManager oceanManager;

    [SerializeField] private float particleDown = 2.0f;
    private int floatersUnderWater;

    private bool underWater;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        oceanManager = FindFirstObjectByType<OceanManager>();
    }
    void FixedUpdate()
    {
        floatersUnderWater = 0;
        
        for(int i = 0; i < floaters.Length; i++){
            float difference = floaters[i].position.y - oceanManager.WaterHeightAtPosition(floaters[i].position) + waterHeightOffset;
            

            if(difference < 0){

                if (gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    rb.AddForce(Vector3.up * floatingPower * Math.Abs(difference), ForceMode.Force);
                    var c = GetComponent<PlayerManager>();
                    if(c!= null)
                        c.isInteracting = true;

                }
                else
                {
                    rb.AddForceAtPosition(Vector3.up * floatingPower * Math.Abs(difference), floaters[i].position, ForceMode.Force);
                }
                floatersUnderWater++;

                if(!underWater){
                    underWater = true;
                    
                    SwitchState(true);
                }
            }
        }

        if(underWater && floatersUnderWater == 0){
            underWater = false;
            SwitchState(false);
        }
    }

    void SwitchState(bool isUnderWater){
        if(isUnderWater){
            rb.linearDamping = underWaterDrag;
            rb.angularDamping = underWaterAngularDrag;
        }else{
            rb.linearDamping = airDrag;
            rb.angularDamping = airAngularDrag;
        }
    }

    public bool IsUnderWater(){
        return underWater;
    }
}
