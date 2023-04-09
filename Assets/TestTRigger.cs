
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Image;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class TestTRigger : DiskFisk_doOnTrigger_fix
{
    public override void doOnTriggerEnter(Collider other)
    {
        Debug.Log("I Like Me some Pizza");
    }

    public override void doOnTriggerExit(Collider other)
    {
        Debug.Log("Uh oh! Pizza stinky!");
    }

    void Start()
    {
        
    }
}
