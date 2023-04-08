
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class onTrigger_script : UdonSharpBehaviour
{
    // [SerializeField]
    // private BoxCollider box;
    private Collider[] temp_colliders;
    private VRC_Pickup[] temp_colliderPickup;
    private int temp_nrOfColliders;
    public int temp_max_nrOfColliders = 100;
    private Collider[] colliders;
    private VRC_Pickup[] collider_pickup;
    [UdonSynced]
    private int[] colliders_busy;

    [UdonSynced]
    private int nrOfColliders;
    [UdonSynced]
    private int max_nrOfColliders;
    private int L_max_nrOfColliders = 10;


    // [UdonSynced, FieldChangeCallback(nameof(test))]
    [UdonSynced, FieldChangeCallback(nameof(test))]
    private int[] _test;

    private int[] test {
        set{
            Debug.Log("## WasChanged");
            _test = value;
        }
        get => _test;
    }
    void Start()
    {
        temp_nrOfColliders       = 0;
        nrOfColliders            = 0; 
        temp_max_nrOfColliders   = 100;
        max_nrOfColliders        = 10;
        temp_colliders  = new Collider[temp_max_nrOfColliders];
        temp_colliderPickup  = new VRC_Pickup[temp_max_nrOfColliders];
        colliders       = new Collider[max_nrOfColliders];
        colliders_busy  = new int[max_nrOfColliders];
        collider_pickup = new VRC_Pickup[max_nrOfColliders];
        for(int i = 0; i < colliders_busy.Length; i++){
            colliders_busy[i]= -1;
        }
    }

    private void G_increaseArraySize() //Global
    {
        int prevMax = max_nrOfColliders;
        max_nrOfColliders += 10;
        int[] temp_busy = new int[max_nrOfColliders];
        for(int i = 0; i < nrOfColliders; i++)
        {
            temp_busy[i] = colliders_busy[i];
        }

        this.colliders_busy = temp_busy;

        for(int i = prevMax; i < colliders_busy.Length; i++){
            colliders_busy[i]= -1;
        }
        RequestSerialization();
    }
    private void L_increaseArraySize() // Local
    {
        Collider[] tempColl = new Collider[max_nrOfColliders];
        VRC_Pickup[] tempPickup = new VRC_Pickup[max_nrOfColliders];
        for(int i = 0; i < nrOfColliders; i++)
        {
            tempColl[i]   = this.colliders[i];
            tempPickup[i] = this.collider_pickup[i];
        }
        this.colliders = tempColl;
        this.collider_pickup = tempPickup;
    }
    private int findFirstFreeIndex()
    {
        for(int i= 0; i < max_nrOfColliders; i++)
        {
            if(colliders_busy[i] == -1)
            {
                return i;
            }
        }
        return -1;
    }
    private int findMatchingColliderIndex(Collider collider)
    {
        for(int i= 0; i < max_nrOfColliders; i++)
        {
            if(colliders_busy[i] == collider.GetInstanceID())
            {
                return i;
            }
        }
        return -1;
    }
    private void G_addCollider(Collider collider)
    {
        if(nrOfColliders == max_nrOfColliders)
        {
            
            G_increaseArraySize();
            // L_increaseArraySize();
        }
        int freeIndex = findFirstFreeIndex();
        if(freeIndex != -1)
        {
            colliders_busy[freeIndex] = collider.GetInstanceID();
            nrOfColliders++;
            RequestSerialization();
        }
        else
        {
            Debug.LogWarning("## Found no free index");
        }
    }
    private void L_addCollider(Collider collider)
    {
        if(L_max_nrOfColliders != max_nrOfColliders)
        {
            L_increaseArraySize();
        }
        int matchingIndex = findMatchingColliderIndex(collider);

        if(matchingIndex!=-1)
        {
            this.colliders[matchingIndex] = collider;
            VRC_Pickup temp = collider.GetComponent<VRC_Pickup>();
            if(temp != null)
            {
               this.collider_pickup[matchingIndex] = temp; 
            }
        }else
        {
            Debug.LogWarning("## Found no matching collider index");
        }
        
    }

    private int findCollider(Collider collider)
    {
        for(int i= 0; i < max_nrOfColliders; i++)
        {
            if(colliders[i] != null)
            {
                if(colliders[i].GetInstanceID() == collider.GetInstanceID())
                {
                    return i;
                }
            }
        }
        return -1;
    }
    private int findTempCollider(Collider collider)
    {
        for(int i= 0; i < temp_max_nrOfColliders; i++)
        {
            if(temp_colliders[i] != null)
            {
                if(temp_colliders[i].GetInstanceID() == collider.GetInstanceID())
                {
                    return i;
                }
            }
        }
        return -1;
    }
    private void addTempCollider(Collider collider)
    {
        for(int i= 0; i < temp_max_nrOfColliders; i++)
        {
            if(temp_colliders[i] == null)
            {
                temp_colliders[i] = collider;
                temp_nrOfColliders++;
            }
        }
        
    }
    private int findColliderBusyIndex(int index)
    {
        for(int i= 0; i < max_nrOfColliders; i++)
        {
            
            if(colliders_busy[i] == index)
            {
                return i;
            }
            
        }
        return -1;
    }
    private void G_removeCollider(Collider collider)
    {
        int indexToRemove = findMatchingColliderIndex(collider);
        if(indexToRemove != -1)
        {
            colliders_busy[indexToRemove] = -1;
            //int getIndexOfBusy = findColliderBusyIndex(indexToRemove);
            nrOfColliders--;
            RequestSerialization();
        }
    }

    private void L_removeCollider(Collider collider)
    {
        int indexToRemove = findMatchingColliderIndex(collider);
        //int getIndexOfBusy = findColliderBusyIndex(indexToRemove);
        if(indexToRemove != -1)
        {
            colliders[indexToRemove] = null;
            RequestSerialization();
        }
        else
        {
            Debug.LogWarning("## Found no collider to be removed");
        }
        
    }


    public void s_addColliderFromTemp()
    {
        
        for(int i= 0; i < temp_max_nrOfColliders; i++)
        {
            
            if(temp_colliders[i] != null)
            {
                L_addCollider(temp_colliders[i]);
                temp_colliders[i] = null;
                temp_colliderPickup[i] = null;
                temp_nrOfColliders--;
            }
            if(temp_nrOfColliders == 0)
            {
                break;
            }
        }
    }
    public void s_removeCollider()
    {
        Debug.Log("## removeCollider!");
        for(int i= 0; i < max_nrOfColliders; i++)
        {
            
            if(colliders[i] != null && colliders_busy[i] == -1)
            {
                colliders[i].attachedRigidbody.detectCollisions = true;
                Debug.Log("## removed["+i.ToString()+"]: " + colliders[i].GetInstanceID());
                colliders[i] = null;
                collider_pickup[i] = null;
            }
        }
    }
    
    public void printContents()
    {
        Debug.Log("## nrOfColliders: " + this.nrOfColliders.ToString());
        Debug.Log("## max_nrOfColliders: " + this.max_nrOfColliders.ToString());
        Debug.Log("## temp_nrOfColliders: " + this.temp_nrOfColliders.ToString());
        string colliderString ="## Colliders Index: \n";
        for(int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i] != null)
            {
                Collider coll  = colliders[i];
                colliderString += "colliders["+i.ToString()+"]: " +coll.GetInstanceID().ToString() + "\n";
            }
        }
        string tempColliderString ="## TempColliders Index: \n";
        for(int i = 0; i < temp_colliders.Length; i++)
        {
            if(temp_colliders[i] != null)
            {
                Collider coll  = temp_colliders[i];
                tempColliderString += "temp_colliders["+i.ToString()+"]: " +coll.GetInstanceID().ToString() + "\n";
            }
        }
        
        Debug.Log(colliderString);
        Debug.Log(tempColliderString);
    }

    [UdonSynced]
    private int t_c = 1;
    public void test_init()
    {
        test = new int[10];
        test[1] = t_c++;
        RequestSerialization();
    }

    public void test_change()
    {
        test[0] = test[0]++;
        test[1] = 12;
        RequestSerialization();
    }


    public void OnTriggerEnter(Collider other)
    {
        VRC_Pickup other_pickup = other.GetComponent<VRC_Pickup>();
        if(other.GetComponent<fix_objectSyncPickup>().changeOnDrop)
        {
            other.GetComponent<fix_objectSyncPickup>().changeOnDrop = false; 
            return; 
        }

        if(other_pickup != null)
        {
            
            if(other_pickup.currentPlayer == Networking.LocalPlayer)
            {
                Debug.Log("## YES WE ARE THE SAME; nrObj:"+this.nrOfColliders.ToString());
                G_addCollider(other);
                L_addCollider(other);
                SendCustomNetworkEvent(
                    VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                    nameof(s_addColliderFromTemp)
                );
            }
            else if(other_pickup.currentPlayer != null)
            {
                Debug.Log("## NO WE ARE NOT THE SAME; nrObj:"+this.nrOfColliders.ToString());
                addTempCollider(other);
                other.attachedRigidbody.detectCollisions = false;
                other.GetComponent<fix_objectSyncPickup>().justEntered = true; 
            }
            else
            {
                addTempCollider(other);
            }
            
        }
        Debug.Log(
            "## ENTER Triggered     : " + other.GetInstanceID().ToString()  + "" + 
            "## ENTER Player ID     : " + Networking.LocalPlayer.playerId.ToString() + "" + 
            "## ENTER Is Cube Held? : " + other.GetComponent<VRC_Pickup>().IsHeld
        );
  
    }
    
    public void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<fix_objectSyncPickup>().justEntered)
        { other.GetComponent<fix_objectSyncPickup>().justEntered = false; return; } 

        VRC_Pickup other_pickup = other.GetComponent<VRC_Pickup>();
        if(other_pickup != null)
        {
            
            if(other_pickup.currentPlayer == Networking.LocalPlayer)
            {
                G_removeCollider(other);
                SendCustomNetworkEvent(
                    VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                    nameof(s_removeCollider)
                );
            
            }
            else if(other_pickup.currentPlayer != null){
                Debug.Log("## Was AnotherPlayer");
                other.attachedRigidbody.detectCollisions = true;
                
            }else {
                Debug.Log("## Use Collisions");
            }
            
        }

        Debug.Log(
            "## EXIT Triggered     : " + other.GetInstanceID().ToString()  + "" + 
            "## EXIT Player ID     : " + Networking.LocalPlayer.playerId.ToString() + "" + 
            "## EXIT Is Cube Held? : " + other.GetComponent<VRC_Pickup>().IsHeld
            );
    }
}
