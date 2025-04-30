using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabHandler : MonoBehaviour
{
    
    [SerializeField] Animator anim;
    
    public bool leftArm = false;

    FixedJoint fixedJoint;
    Rigidbody rb;

    //player class
    PlayerManager player;

    void Awake()
    {
        //get player class
        player = transform.root.GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();

        //cahnging solver iterations
        rb.solverIterations = 100;
    }

    void Update()
    {
        attemptLetGo();
    }

    public void attemptLetGo()
    {
        if(leftArm)
        {
            if(!player.inputManager.left_Input)
            {
                if(fixedJoint != null)
                {
                    if(fixedJoint.connectedBody != null)
                    {
                        float f = 0.1f;

                        fixedJoint.connectedBody.AddForce((player.transform.forward + Vector3.up * 0.25f) * f,ForceMode.Impulse);
                        Destroy(fixedJoint);
                    }
                }
            }
        }
        else
        {
            if(!player.inputManager.right_Input)
            {
                if(fixedJoint != null)
                {
                    if(fixedJoint.connectedBody != null)
                    {
                        float f = 0.1f;

                        fixedJoint.connectedBody.AddForce((player.transform.forward + Vector3.up * 0.25f) * f,ForceMode.Impulse);
                        Destroy(fixedJoint);
                    }
                }
            }
        }
    }

    bool TryCarryObject(Collision col)
    {
        //if has authority

        //if player is not active ragdoll

        //if not already carrying object
        if(fixedJoint != null) return false;

        //dont grab ourselves
        if(col.transform.root == player.transform) return false;

        if(!col.collider.TryGetComponent(out Rigidbody otherObjRb)) return false;

        Debug.Log("SHOULD GRAB");

        fixedJoint = transform.gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = otherObjRb;

        fixedJoint.autoConfigureConnectedAnchor = false;
        fixedJoint.connectedAnchor = col.transform.InverseTransformPoint(col.GetContact(0).point);

        return true;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLLISION");
        
        if(player.inputManager.left_Input || player.inputManager.right_Input)
        {
            TryCarryObject(collision);  
        }
    }

}
