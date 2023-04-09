#define DISKFISK_DEBUG
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

#if !COMPILER_UDONSHARP && UNITY_EDITOR // These using statements must be wrapped in this check to prevent issues on builds
using UnityEditor;
using UdonSharpEditor;
using UnityEditor.Compilation;
#endif

public class DiskFisk_objectSyncPickup_fix : UdonSharpBehaviour
{    
    public VRC_Pickup pickup = null;
    public Rigidbody rigid_body = null;
    public Collider objectsCollider = null;
    [HideInInspector]
    public DiskFisk_doOnTrigger_fix triggerScript = null;
    private bool dropped = false;     
    private bool ignore = false;

    void Start()
    {

    }

    public void addCollider()
    {
        
        if(pickup.currentPlayer != Networking.LocalPlayer)
        {
#if DISKFISK_DEBUG
            Debug.Log("## addCollider!");
#endif
            rigid_body.detectCollisions = true;
        }
    }
    public void setWasDroppedTrue()
    {
        dropped = true; 
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(addCollider)
        );
    }

    public bool wasDropped()
    {
        return this.dropped;
    }

    public void _ignoreNoMorePlz()
    {
        ignore = false; 
    }
    public void _ignoreMePlz()
    {
        rigid_body.detectCollisions = true;
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
    public bool isIgnored()
    {
        return this.ignore;
    }

    public void setTriggerScript(DiskFisk_doOnTrigger_fix triggerScript)
    {
        this.triggerScript = triggerScript;
    }

    public void ss_triggerEnter()
    {
#if DISKFISK_DEBUG
        Debug.Log("## ss_triggerEnter");
#endif        
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(s_triggerEnter)
        );
    }    

    public void s_triggerEnter()
    {
        if(triggerScript != null)
        {
#if DISKFISK_DEBUG
            Debug.Log("## s_triggerEnter");
#endif
            triggerScript.doOnTriggerEnter(objectsCollider);
        }else{
#if DISKFISK_DEBUG
            Debug.Log("## s_triggerEnter (TriggerScript was null)");
#endif
            SendCustomEventDelayedFrames(
                nameof(s_triggerEnter),
                10,
                VRC.Udon.Common.Enums.EventTiming.LateUpdate
            );
        }
    }

    public void ss_triggerExit()
    {
#if DISKFISK_DEBUG
        Debug.Log("## ss_triggerExit");
#endif        
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(s_triggerExit)
        );
    }
    public void s_triggerExit()
    {
        if(triggerScript != null)
        {
#if DISKFISK_DEBUG
            Debug.Log("## s_triggerExit");
#endif            
            triggerScript.doOnTriggerExit(objectsCollider);
        }else{
#if DISKFISK_DEBUG
            Debug.Log("## s_triggerExit (TriggerScript was null)");
#endif
        }
    }

    public void setWasDroppedFalse()
    {
        this.dropped = false;
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
#if DISKFISK_DEBUG
            Debug.Log("## Player joined! : " + player.playerId);
#endif
        }
        
        // If player joins at roughly the same time, it causes sync problems
        // To avoid this I force sync by turning it off and then on again after a while
        // To ensure that no other problems the newly joined player wont be able to  pick
        // up stuff while this occurs...
        if(!Networking.IsMaster)
        {
            reActivateForNewPlayers();
        }

    }
    public void reActivateForNewPlayers()
    {
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(forceNewPlayerToRedoObjectSync_disable)
        );
    }

    public void forceNewPlayerToRedoObjectSync_disable()
    {
        this.gameObject.SetActive(false);
        gameObject.GetComponent<VRC.SDK3.Components.VRCObjectSync>().enabled = false; 
        SendCustomNetworkEvent(
            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
            nameof(forceNewPlayerToRedoObjectSync_enable)
        );
        
    }
    public void forceNewPlayerToRedoObjectSync_enable()
    {
        this.gameObject.SetActive(true);
        gameObject.GetComponent<VRC.SDK3.Components.VRCObjectSync>().FlagDiscontinuity();
        gameObject.GetComponent<VRC.SDK3.Components.VRCObjectSync>().enabled = true;        
    }

    public override void OnDeserialization()
    {
        base.OnDeserialization();

    }
  
}

    // Editor scripts must be wrapped in a UNITY_EDITOR check to prevent issues while uploading worlds. The !COMPILER_UDONSHARP check prevents UdonSharp from throwing errors about unsupported code here.
#if !COMPILER_UDONSHARP && UNITY_EDITOR 
    [CustomEditor(typeof(DiskFisk_objectSyncPickup_fix))]
    public class DiskFisk_objectSyncPickup_fix_Editor : Editor
    {
        private void OnEnable()
        {
            DiskFisk_objectSyncPickup_fix objSyncFix = (DiskFisk_objectSyncPickup_fix)target;
            
            objSyncFix.pickup     = objSyncFix.gameObject.GetComponent<VRC_Pickup>(); 
            objSyncFix.rigid_body = objSyncFix.gameObject.GetComponent<Rigidbody>(); 

            if(objSyncFix.pickup == null)
            {
                objSyncFix.pickup     = objSyncFix.gameObject.AddComponent<VRC.SDK3.Components.VRCPickup>();
                objSyncFix.rigid_body = objSyncFix.GetComponent<Rigidbody>();
            }

            Component vrcObjSync = objSyncFix.gameObject.GetComponent<VRC.SDK3.Components.VRCObjectSync>();
            if(vrcObjSync == null)
            {
                objSyncFix.gameObject.AddComponent<VRC.SDK3.Components.VRCObjectSync>();
            }
            
        }

        
        

        public override void OnInspectorGUI() 
        {
            // // Draws the default convert to UdonBehaviour button, program asset field, sync settings, etc.
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            serializedObject.Update();

            // Draw the default inspector
            DrawDefaultInspector();


            DiskFisk_objectSyncPickup_fix objSyncFix = (DiskFisk_objectSyncPickup_fix)target;

            
            SerializedProperty requiredObjectProperty = serializedObject.FindProperty(nameof(objSyncFix.objectsCollider));

            if (requiredObjectProperty.objectReferenceValue == null)
            {
                string errorMsg = "## DiskFisk_objectSyncPickup_fix requires a collider!";
                EditorGUILayout.HelpBox(errorMsg, MessageType.Error);

            }            


        }
    }
#endif