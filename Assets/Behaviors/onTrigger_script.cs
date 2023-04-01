
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class onTrigger_script : UdonSharpBehaviour
{
    void Start()
    {
        // box.attachedRigidbody.detectCollisions = false;
    }
    [SerializeField]
    private BoxCollider box;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            "## ENTER Triggered     : " + other.GetInstanceID().ToString()  + "\n" + 
            "## ENTER Player ID     : " + Networking.LocalPlayer.playerId.ToString() + "\n" + 
            "## ENTER Is Cube Held? : " + other.GetComponent<VRC_Pickup>().IsHeld
        );
  
    }
    public void OnTriggerExit(Collider other)
    {
        
        Debug.Log(
            "## EXIT Triggered     : " + other.GetInstanceID().ToString()  + "\n" + 
            "## EXIT Player ID     : " + Networking.LocalPlayer.playerId.ToString() + "\n" + 
            "## EXIT Is Cube Held? : " + other.GetComponent<VRC_Pickup>().IsHeld
            );
    }
}
