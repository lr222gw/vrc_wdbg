
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class onTrigger_script : UdonSharpBehaviour
{
    public void doOnTriggerEnter(Collider other)
    {

        Debug.Log(
            "## ENTER Triggered     : " + other.GetInstanceID().ToString()  + "" + 
            "## ENTER Player ID     : " + Networking.LocalPlayer.playerId.ToString() + "" + 
            "## ENTER Is Cube Held? : " + other.GetComponent<VRC_Pickup>().IsHeld
        );

    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("## OnTriggerEnter");
        VRC_Pickup other_pickup = other.GetComponent<VRC_Pickup>();
        

        if(other_pickup != null)
        {
            var f = other.GetComponent<fix_objectSyncPickup>();
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
                    Debug.Log("## OnTriggerEnter Was Held");
                    other.attachedRigidbody.detectCollisions = false;
                }
                if(!other_pickup.IsHeld && other_pickup.currentPlayer == null && 
                    Networking.IsOwner(Networking.LocalPlayer, other.gameObject))
                {
                    Debug.Log("## OnTriggerEnter Thrown into zone");
                    f.ss_triggerEnter();
                }
            }
        }
    }

    public void doOnTriggerExit(Collider other)
    {

        Debug.Log(
            "## EXIT Triggered     : " + other.GetInstanceID().ToString()  + "" + 
            "## EXIT Player ID     : " + Networking.LocalPlayer.playerId.ToString() + "" + 
            "## EXIT Is Cube Held? : " + other.GetComponent<VRC_Pickup>().IsHeld
            );

    }

    
    public void OnTriggerExit(Collider other)
    {
        Debug.Log("## OnTriggerExit");
        VRC_Pickup other_pickup = other.GetComponent<VRC_Pickup>();
        if(other_pickup != null)
        {
          
            var f = other.GetComponent<fix_objectSyncPickup>();
            if(f.isIgnored()){return;}
            if(f.wasDropped()){
                Debug.Log("## OnTriggerExit wasDropped");
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
                    Debug.Log("## OnTriggerEnter Thrown out of zone");
                    f.ss_triggerExit();
                }
                return;
            }
        }
    }
}

