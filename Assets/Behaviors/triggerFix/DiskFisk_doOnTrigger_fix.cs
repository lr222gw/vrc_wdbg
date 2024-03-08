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

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DiskFisk_doOnTrigger_fix : UdonSharpBehaviour
{
    virtual public void doOnTriggerEnter(Collider other)
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
        var f = other.GetComponent<DiskFisk_objectSyncPickup_fix>();
        if(f == null){return;}

#if DISKFISK_DEBUG
        Debug.Log("## OnTriggerEnter");
#endif
  

        if(f.pickup != null)
        {            
            f.setTriggerScript(this);
            if(f.isIgnored()){return;}
            if(f.pickup.currentPlayer == Networking.LocalPlayer)
            {
                f.ss_triggerEnter();
            }
            else 
            {
                if(f.pickup.IsHeld && !f.wasDropped())
                {
#if DISKFISK_DEBUG
                    Debug.Log("## OnTriggerEnter Was Held");
#endif                    
                    other.attachedRigidbody.detectCollisions = false;
                }
                if(!f.pickup.IsHeld && f.pickup.currentPlayer == null && 
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

    virtual public void doOnTriggerExit(Collider other)
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
        var f = other.GetComponent<DiskFisk_objectSyncPickup_fix>();
        if(f == null){return;}

#if DISKFISK_DEBUG
        Debug.Log("## OnTriggerExit");
#endif        
        
        if(f.pickup != null)
        {          
            if(f.isIgnored()){return;}
            if(f.wasDropped()){
#if DISKFISK_DEBUG
                Debug.Log("## OnTriggerExit wasDropped");
#endif                
                f.s_setWasDroppedFalse();
                f.ignoreMePlz();
            }

            if(f.pickup.currentPlayer == Networking.LocalPlayer)
            {
                f.ss_triggerExit();
            }
            else
            {
                if(!f.pickup.IsHeld && f.pickup.currentPlayer == null && 
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

    // Editor scripts must be wrapped in a UNITY_EDITOR check to prevent issues while uploading worlds. The !COMPILER_UDONSHARP check prevents UdonSharp from throwing errors about unsupported code here.
#if !COMPILER_UDONSHARP && UNITY_EDITOR 
    [CustomEditor(typeof(DiskFisk_doOnTrigger_fix))]
    public class DiskFisk_doOnTrigger_fix_Editor : Editor
    {
        private void OnEnable()
        {
            
        }

        
        

        public override void OnInspectorGUI() 
        {
            // // Draws the default convert to UdonBehaviour button, program asset field, sync settings, etc.
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            serializedObject.Update();

            // Draw the default inspector
            DrawDefaultInspector();


            DiskFisk_doOnTrigger_fix triggerSyncFix = (DiskFisk_doOnTrigger_fix)target;



            string errorMsg = "## DiskFisk_doOnTrigger_fix is not meant to be used directly as a component!\n## Instead add a UdonBehaviorScript and inherit from DiskFisk_doOnTrigger_fix! \n## Then to use the triggerEnter/Exit functionality just\n## override the \"doOnTriggerEnter/Exit functions!\".\n## Finally add a Collider of choice and enable \"is Trigger\"!";
            EditorGUILayout.HelpBox(errorMsg, MessageType.Error);




        }
    }
#endif