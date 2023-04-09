
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class fix_objectSyncPickup : UdonSharpBehaviour
{
    void Start()
    {
    }

    public void addCollider()
    {
        
        if(gameObject.GetComponent<VRC_Pickup>().currentPlayer != Networking.LocalPlayer)
        {
            Debug.Log("## addCollider!");
            gameObject.GetComponent<Rigidbody>().detectCollisions = true;
        }
    }
    public void setWasDroppedTrue()
    {
        wasDropped = true;
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(addCollider)
        );
    }

    public onTrigger_script myScript = null;
    
    public bool ignore = false;

    public void _ignoreNoMorePlz()
    {
        ignore = false; 
    }
    public void _ignoreMePlz()
    {
        gameObject.GetComponent<Rigidbody>().detectCollisions = true;
        ignore = true; 
        SendCustomEventDelayedSeconds(
            nameof(_ignoreNoMorePlz),
            0.5f,
            VRC.Udon.Common.Enums.EventTiming.LateUpdate
        );
    }
    public void ignoreMePlz()
    {
        SendCustomEventDelayedSeconds(
            nameof(_ignoreMePlz),
            0.5f,
            VRC.Udon.Common.Enums.EventTiming.LateUpdate
        );
    }

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
        if(myScript != null)
        {
            Debug.Log("## s_triggerEnter");
            myScript.doOnTriggerEnter(gameObject.GetComponent<Collider>());
        }else{Debug.Log("## s_triggerEnter (TriggerScript was null)");}
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
        if(myScript != null)
        {
            Debug.Log("## s_triggerExit");
            myScript.doOnTriggerExit(gameObject.GetComponent<Collider>());
        }else{Debug.Log("## s_triggerExit (TriggerScript was null)");}
    }

    public void setWasDroppedFalse()
    {
        this.wasDropped = false;
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
            nameof(setWasDroppedTrue)
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
