#define DISKFISK_DEBUG 
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DiskFisk_doOnTrigger_fix : UdonSharpBehaviour
{
    public void doOnTriggerEnter(Collider other)
    {

#if DISKFISK_DEBUG
        Debug.Log(
            "## ENTER Triggered     : " + other.GetInstanceID().ToString()  + "" + 
            "## ENTER Player ID     : " + Networking.LocalPlayer.playerId.ToString() + "" + 
            "## ENTER Is Cube Held? : " + other.GetComponent<VRC_Pickup>().IsHeld
        );
#endif

    }

    public void OnTriggerEnter(Collider other)
    {
#if DISKFISK_DEBUG
        Debug.Log("## OnTriggerEnter");
#endif

        VRC_Pickup other_pickup = other.GetComponent<VRC_Pickup>();
        

        if(other_pickup != null)
        {
            var f = other.GetComponent<DiskFisk_objectSyncPickup_fix>();
            if(f.isIgnored()){return;}
            f.setTriggerScript(this);
            if(other_pickup.currentPlayer == Networking.LocalPlayer)
            {
                f.ss_triggerEnter();
            }
            else 
            {
                if(other_pickup.IsHeld && !f.wasDropped())
                {
#if DISKFISK_DEBUG
                    Debug.Log("## OnTriggerEnter Was Held");
#endif                    
                    other.attachedRigidbody.detectCollisions = false;
                }
                if(!other_pickup.IsHeld && other_pickup.currentPlayer == null && 
                    Networking.IsOwner(Networking.LocalPlayer, other.gameObject))
                {
#if DISKFISK_DEBUG
                    Debug.Log("## OnTriggerEnter Thrown into zone");
#endif                    
                    f.ss_triggerEnter();
                }
            }
        }
    }

    public void doOnTriggerExit(Collider other)
    {
#if DISKFISK_DEBUG
        Debug.Log(
            "## EXIT Triggered     : " + other.GetInstanceID().ToString()  + "" + 
            "## EXIT Player ID     : " + Networking.LocalPlayer.playerId.ToString() + "" + 
            "## EXIT Is Cube Held? : " + other.GetComponent<VRC_Pickup>().IsHeld
            );
#endif

    }

    public void OnTriggerExit(Collider other)
    {
#if DISKFISK_DEBUG
        Debug.Log("## OnTriggerExit");
#endif        
        VRC_Pickup other_pickup = other.GetComponent<VRC_Pickup>();
        if(other_pickup != null)
        {
          
            var f = other.GetComponent<DiskFisk_objectSyncPickup_fix>();
            if(f.isIgnored()){return;}
            if(f.wasDropped()){
#if DISKFISK_DEBUG
                Debug.Log("## OnTriggerExit wasDropped");
#endif                
                f.s_setWasDroppedFalse();
                f.ignoreMePlz();
            }

            if(other_pickup.currentPlayer == Networking.LocalPlayer)
            {
                f.ss_triggerExit();
            }
            else
            {
                if(!other_pickup.IsHeld && other_pickup.currentPlayer == null && 
                    Networking.IsOwner(Networking.LocalPlayer, other.gameObject))
                {
#if DISKFISK_DEBUG
                    Debug.Log("## OnTriggerEnter Thrown out of zone");
#endif                    
                    f.ss_triggerExit();
                }
                return;
            }
        }
    }
}

