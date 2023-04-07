
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class fix_objectSyncPickup : UdonSharpBehaviour
{
    void Start()
    {
        justEntered         = false;
        wasInside           = false;
        justDropped         = false;
        justPicked          = false;
        changeOnDrop        = false;
    }


    public bool justEntered;
    
    
    // [UdonSynced,FieldChangeCallback(nameof(justDropped))]
    // private bool _justDropped;
    public bool justDropped;
    // {
    //     set {
    //         _justDropped = value;
    //     }
    //     get => _justDropped;
    // }
    public bool justPicked;
    public bool wasInside;

    public bool changeOnDrop;

    public void removeCollider()
    {
        
        if( gameObject.GetComponent<VRC_Pickup>().currentPlayer != Networking.LocalPlayer &&
            gameObject.GetComponent<VRC_Pickup>().currentPlayer != null)
        {
            Debug.Log("## removeCollider!");
            
            gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            // this.wasInside = true;
        }
    }
    public void addCollider()
    {
        
        if(gameObject.GetComponent<VRC_Pickup>().currentPlayer != Networking.LocalPlayer)
        {
            Debug.Log("## addCollider!");
            changeOnDrop = true; 
            gameObject.GetComponent<Rigidbody>().detectCollisions = true;
        }

    }

    public void s_removeCollider()
    {
        Debug.Log("Picked up");
        this.justPicked  = true;
        this.pickupReady = true;
        removeCollider();
    }
    public void s_addCollider()
    {
        Debug.Log("Dropped ");
        this.justDropped = true; 
        this.dropReady   = true;
        addCollider();
    }

    // public void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("## ERERAERAERASDF DSAF!");
    // }

    public bool pickupReady = false; 
    public bool dropReady = false; 
    
    public override void OnPickup()
    {
        base.OnPickup();
        pickupReady = false;
        // Debug.Log("Picked up");
        // this.justPicked  = true;
        // gameObject.GetComponent<Rigidbody>().detectCollisions = false;

        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(s_removeCollider)
        );       

    }

    public override void OnDrop()
    {
        base.OnDrop();
        dropReady = false;
        // Debug.Log("Dropped ");
        // this.justDropped = true; 
        // gameObject.GetComponent<Rigidbody>().detectCollisions = true;

        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(s_addCollider)
        );  
    }
}
