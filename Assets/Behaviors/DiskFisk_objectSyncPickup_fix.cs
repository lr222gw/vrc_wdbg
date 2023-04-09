//#define DISKFISK_DEBUG
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
    public Collider objectsCollider = null;
    private DiskFisk_doOnTrigger_fix triggerScript = null;
    private bool dropped = false;     
    private bool ignore = false;

    void Start()
    {
    }

    public void addCollider()
    {
        
        if(gameObject.GetComponent<VRC_Pickup>().currentPlayer != Networking.LocalPlayer)
        {
#if DISKFISK_DEBUG
            Debug.Log("## addCollider!");
#endif
            gameObject.GetComponent<Rigidbody>().detectCollisions = true;
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
            triggerScript.doOnTriggerEnter(gameObject.GetComponent<Collider>());
        }else{
#if DISKFISK_DEBUG
            Debug.Log("## s_triggerEnter (TriggerScript was null)");
#endif
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
            triggerScript.doOnTriggerExit(gameObject.GetComponent<Collider>());
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
            // System.Collections.Queue a;
#if DISKFISK_DEBUG
            Debug.Log("## Player joined! : " + player.playerId);
#endif
        }  
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
            
            Component vrcPickup = objSyncFix.gameObject.GetComponent<VRC_Pickup>(); 
            Component vrcObjSync = objSyncFix.gameObject.GetComponent<VRC.SDK3.Components.VRCObjectSync>();
            

            if(vrcPickup == null)
            {
                objSyncFix.gameObject.AddComponent<VRC.SDK3.Components.VRCPickup>();
            }
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