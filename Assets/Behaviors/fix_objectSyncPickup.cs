
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class fix_objectSyncPickup : UdonSharpBehaviour
{
    void Start()
    {
        // justEntered         = false;
        // wasInside           = false;
        // justDropped         = false;
        // justPicked          = false;
        // changeOnDrop        = false;
    }


    // // public bool justEntered;
    // // public bool justDropped;
    // // public bool justPicked;
    // // public bool wasInside;


    // public bool changeOnDrop;

    // [UdonSynced]
    // public bool isInside = false;

    // public void removeCollider()
    // {
    //     if(gameObject.GetComponent<VRC_Pickup>().currentPlayer == null && isHeld)
    //     {
    //         Debug.Log("## VRC_Pickup.currentPlayer == null, But is it being held?");
    //         SendCustomEventDelayedFrames(nameof(removeCollider),
    //         2,
    //         VRC.Udon.Common.Enums.EventTiming.LateUpdate);
    //         return;
    //     }

    //     if( gameObject.GetComponent<VRC_Pickup>().currentPlayer != Networking.LocalPlayer &&
    //         gameObject.GetComponent<VRC_Pickup>().currentPlayer != null)
    //     {
    //         Debug.Log("## removeCollider!");
            
    //         gameObject.GetComponent<Rigidbody>().detectCollisions = false;
    //         // this.wasInside = true;
    //     }
    // }
    public void addCollider()
    {
        // if(gameObject.GetComponent<VRC_Pickup>().currentPlayer != null) // && isHeld
        // {
        //     Debug.Log("## VRC_Pickup.currentPlayer == a player, But Shouldnt!");
        //     SendCustomEventDelayedFrames(nameof(addCollider),
        //     2,
        //     VRC.Udon.Common.Enums.EventTiming.LateUpdate);
        //     return;
        // }
        
        if(gameObject.GetComponent<VRC_Pickup>().currentPlayer != Networking.LocalPlayer)
        {
            Debug.Log("## addCollider!");
            gameObject.GetComponent<Rigidbody>().detectCollisions = true;
            wasDropped = true;
        }

    }

    // public void d_s_removeCollider()
    // {
        
    //     SendCustomNetworkEvent(
    //         VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
    //         nameof(s_removeCollider)
    //     );   
    // }
    // public void s_removeCollider()
    // {
    //     Debug.Log("## s_Picked up");
    //     // this.justPicked  = true;
    //     // this.pickupReady = true;
    //     isHeld = true;
    //     removeCollider();
    // }
    // public void s_addCollider()
    // {
    //     Debug.Log("## s_Dropped ");
    //     // this.justDropped = true; 
    //     // this.dropReady   = true;
    //     // isHeld = false;
    //     addCollider();
    // }

    // public bool transitionIntoEnter = false;

    

    // public void s_tempActivateCollider()
    // {
    //     gameObject.GetComponent<Rigidbody>().detectCollisions = true;
    //     transitionIntoEnter = true;
    //     this.isInside = true; 

        
    //     // SendCustomEventDelayedFrames(nameof(addCollider),
    //     //     2,
    //     //     VRC.Udon.Common.Enums.EventTiming.LateUpdate
    //     // );
    // }

    // public void ss_leavingTrigger()
    // {
    //     SendCustomNetworkEvent(
    //         VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
    //         nameof(s_leavingTrigger)
    //     );
    // }

    // public void s_leavingTrigger()
    // {
    //     gameObject.GetComponent<Rigidbody>().detectCollisions = true;
    //     this.isInside = false; 
        
    //     // SendCustomEventDelayedFrames(nameof(addCollider),
    //     //     2,
    //     //     VRC.Udon.Common.Enums.EventTiming.LateUpdate
    //     // );
    // }

    // public void ss_tempActivateCollider()
    // {
    //     SendCustomNetworkEvent(
    //         VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
    //         nameof(s_tempActivateCollider)
    //     );
    // }

    // // public bool pickupReady = false; 
    // // public bool dropReady = false; 

    // [UdonSynced]
    // public bool isHeld = false; 

    public onTrigger_script myScript = null;
    
    public void ss_triggerEnter()
    {
        Debug.Log("## ss_triggerEnter");
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(s_triggerEnter)
        );
    }

    public void s_triggerEnter()
    {
        Debug.Log("## s_triggerEnter");
        myScript.doOnTriggerEnter(gameObject.GetComponent<Collider>());
    }

    public void ss_triggerExit()
    {
        Debug.Log("## ss_triggerExit");
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(s_triggerExit)
        );
    }
    public void s_triggerExit()
    {
        Debug.Log("## s_triggerExit");
        myScript.doOnTriggerExit(gameObject.GetComponent<Collider>());
    }

    // public void s_addCollider()
    // {
    //     addCollider();
    // }


    public void setWasDroppedFalse_a()
    {
        this.wasDropped = false;
    }

    public void setWasDroppedFalse()
    {
        setWasDroppedFalse_a();
        // if(!gameObject.GetComponent<Collider>().attachedRigidbody.IsSleeping())
        // {
        //     SendCustomEventDelayedFrames(
        //     nameof(SendCustomEventDelayedFrames),
        //     1,
        //     VRC.Udon.Common.Enums.EventTiming.LateUpdate);
        // }else {
        //     setWasDroppedFalse_a();
        // }
    }
    public void s_setWasDroppedFalse()
    {
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(setWasDroppedFalse)
        );
    }
    
    public override void OnPickup()
    {
        base.OnPickup();

    }

    public bool wasDropped = false; 
    public override void OnDrop()
    {
        base.OnDrop();
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(addCollider)
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
