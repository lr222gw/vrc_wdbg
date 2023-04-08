
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
        if(gameObject.GetComponent<VRC_Pickup>().currentPlayer == null && isHeld)
        {
            Debug.Log("## VRC_Pickup.currentPlayer == null, But is it being held?");
            SendCustomEventDelayedFrames(nameof(removeCollider),
            2,
            VRC.Udon.Common.Enums.EventTiming.LateUpdate);
            return;
        }

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
        if(gameObject.GetComponent<VRC_Pickup>().currentPlayer != null && !isHeld) // && isHeld
        {
            Debug.Log("## VRC_Pickup.currentPlayer == a player, But Shouldnt!");
            SendCustomEventDelayedFrames(nameof(addCollider),
            2,
            VRC.Udon.Common.Enums.EventTiming.LateUpdate);
            return;
        }
        
        if(gameObject.GetComponent<VRC_Pickup>().currentPlayer != Networking.LocalPlayer)
        {
            Debug.Log("## addCollider!");
            changeOnDrop = true; 
            gameObject.GetComponent<Rigidbody>().detectCollisions = true;
        }

    }

    public void d_s_removeCollider()
    {
        
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(s_removeCollider)
        );   
    }
    public void s_removeCollider()
    {
        Debug.Log("## s_Picked up");
        this.justPicked  = true;
        this.pickupReady = true;
        isHeld = true;
        removeCollider();
    }
    public void s_addCollider()
    {
        Debug.Log("## s_Dropped ");
        this.justDropped = true; 
        this.dropReady   = true;
        isHeld = false;
        addCollider();
    }

    // public void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("## ERERAERAERASDF DSAF!");
    // }

    public bool pickupReady = false; 
    public bool dropReady = false; 

    public bool isHeld = false; 
    
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

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        base.OnPlayerJoined(player);
        if(player.playerId == Networking.LocalPlayer.playerId)
        {
            Debug.Log("## Player joined! : " + player.playerId);
        }
    }
}
