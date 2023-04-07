
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
            Debug.Log("WasChanged");
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
        colliders_busy[freeIndex] = collider.GetInstanceID();
        nrOfColliders++;
        RequestSerialization();
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
        colliders_busy[indexToRemove] = -1;
        //int getIndexOfBusy = findColliderBusyIndex(indexToRemove);
        nrOfColliders--;
        RequestSerialization();
    }

    private void L_removeCollider(Collider collider)
    {
        int indexToRemove = findMatchingColliderIndex(collider);
        //int getIndexOfBusy = findColliderBusyIndex(indexToRemove);
        colliders[indexToRemove] = null;
        RequestSerialization();
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

    VRCPlayerApi[] players;
    [UdonSynced]
    private int nrOfPlayers = 0;
    [UdonSynced]
    private int nrOfPlayersInside = 0;
    [UdonSynced]
    private int nrOfUniquePlayersVisited = 0;
    [UdonSynced]
    private int nrOfUniquePlayersVisited_CAP = 0;
    [UdonSynced]
    // private int[] playerIDs;
    private int[] playerIDs_status;
    [UdonSynced]
    private int[] playerIDs_HeldStatus;

    private int[] playerID;

    private bool isNewUser(VRCPlayerApi player)
    {
        if(player.playerId < nrOfUniquePlayersVisited_CAP)
        {
            if(playerIDs_status[player.playerId] == -1)
            {
                return true; 
            }else{
                return false;
            }

        }
        else{
            return true;
        }
    }

    private const int PLAYER_IS_UNDEF   =-1;
    private const int PLAYER_IS_OFFLINE = 0;
    private const int PLAYER_IS_OUTSIDE  = 2;
    private const int PLAYER_IS_INSIDE  = 3;
    private const int HELD_STATUS_TRUE   = 1;
    private const int HELD_STATUS_FALSE  = -1;
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        base.OnPlayerJoined(player);

        if(isNewUser(player))
        {
            Debug.Log("was New player");
            if(nrOfUniquePlayersVisited_CAP >= nrOfUniquePlayersVisited)
            {
                nrOfUniquePlayersVisited_CAP += 10;
                int[] tempArr = new int[nrOfUniquePlayersVisited_CAP];
                int[] tempArr_h = new int[nrOfUniquePlayersVisited_CAP];
                for(int i = 0; i < nrOfUniquePlayersVisited; i++)
                {
                    tempArr[i] = playerIDs_status[i];
                    tempArr_h[i] = playerIDs_HeldStatus[i];
                }
                for(int i = nrOfUniquePlayersVisited; i < nrOfUniquePlayersVisited_CAP; i++)
                {
                    tempArr[i]   = PLAYER_IS_UNDEF;
                    tempArr_h[i] = HELD_STATUS_FALSE; 
                }
                this.playerIDs_status = tempArr;
                this.playerIDs_HeldStatus = tempArr_h;
            }            
            nrOfUniquePlayersVisited++;
        }
        this.nrOfPlayers++;
        this.playerIDs_status[player.playerId]      = PLAYER_IS_OUTSIDE;
        this.playerIDs_HeldStatus[player.playerId]  = HELD_STATUS_FALSE;
        RequestSerialization();
    }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        base.OnPlayerLeft(player);
        this.nrOfPlayers--;
        this.playerIDs_status[player.playerId] = PLAYER_IS_OFFLINE;
        this.playerIDs_HeldStatus[player.playerId]  = HELD_STATUS_FALSE;

        RequestSerialization();
    }
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        base.OnPlayerTriggerEnter(player);
        if(player == Networking.LocalPlayer)
        {
            Debug.LogWarning("Player Enter:" + player.playerId.ToString());
            this.playerIDs_status[player.playerId] = PLAYER_IS_INSIDE;
            nrOfPlayersInside++;
            RequestSerialization();
        }
    }
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        base.OnPlayerTriggerExit(player);
        if(player == Networking.LocalPlayer)
        {
            Debug.LogWarning("Player Exit:" + player.playerId.ToString());
            this.playerIDs_status[player.playerId] = PLAYER_IS_OUTSIDE; //?
            nrOfPlayersInside--;
            RequestSerialization();
        }
    }
    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        
    }


    public void Update()
    {        
        if(Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            // if(nrOfPlayers != VRCPlayerApi.GetPlayerCount())
            // {
            //     this.players      = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            //     // this.playerIDs    = new int[VRCPlayerApi.GetPlayerCount()];
            //     // // this.playerIDs_idx  = new int[VRCPlayerApi.GetPlayerCount()];
            //     // VRCPlayerApi.GetPlayers(this.players);
            //     // for(int i = 0; i < VRCPlayerApi.GetPlayerCount();i++)
            //     // {
            //     //     this.playerIDs[i]    = this.players[i].playerId;
            //     //     // this.playerIDs_idx[] =
            //     // }
            // }
            
            // for(int i = 0; i < players.Length; i++)
            // {
                
                
            //     if(players[i].GetPickupInHand(VRC_Pickup.PickupHand.None))
            //     {

            //     }
            // }

            // for(int i = 0; i < this.colliders.Length; i++ )
            // {
            //     if(this.colliders[i] != null)
            //     {
            //         for(int j = 0; j < players.Length; j++)
            //         {
                        
            //         }
            //     }
            // }
        }
        else
        {
            Debug.Log("OnUpdate");
            if(this.playerIDs_status[Networking.LocalPlayer.playerId] == PLAYER_IS_INSIDE)
            {
                VRC_Pickup l = Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Left);
                VRC_Pickup r = Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Right);
                if(l.IsHeld ||r.IsHeld)
                {
                    if(this.playerIDs_HeldStatus[Networking.LocalPlayer.playerId] == HELD_STATUS_FALSE)
                    {
                        this.playerIDs_HeldStatus[Networking.LocalPlayer.playerId]  = HELD_STATUS_TRUE;
                        SendCustomNetworkEvent(
                            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                            nameof(s_updateHeldStatus)
                        );
                        RequestSerialization();
                    }
                    
                // }
                // else if(r.IsHeld)
                // {
                    // this.playerIDs_HeldStatus[Networking.LocalPlayer.playerId]  = HELD_STATUS_TRUE;
                }else 
                {
                    if(this.playerIDs_HeldStatus[Networking.LocalPlayer.playerId] == HELD_STATUS_TRUE)
                    {
                        this.playerIDs_HeldStatus[Networking.LocalPlayer.playerId]  = HELD_STATUS_FALSE;
                        SendCustomNetworkEvent(
                            VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                            nameof(s_updateHeldStatus)
                        );
                        RequestSerialization();
                    }
                    
                }
            }
        }
    }
    public void s_updateHeldStatus()
    {
        for(int i = 0; i < this.nrOfUniquePlayersVisited; i++)
        {
            if(this.playerIDs_status[i] != PLAYER_IS_UNDEF)
            {
                if(this.playerIDs_HeldStatus[i] != HELD_STATUS_FALSE)
                {
                    for(int j= 0; j < max_nrOfColliders; j++)
                    {
                        
                        if(colliders[j] != null && colliders_busy[j] == -1)
                        {
                            colliders[j].attachedRigidbody.detectCollisions = true;
                            Debug.Log("## removed["+j.ToString()+"]: " + colliders[j].GetInstanceID());
                            colliders[j] = null;
                            collider_pickup[j] = null;
                        }
                    }
                }
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        VRC_Pickup other_pickup = other.GetComponent<VRC_Pickup>();
        

        if(other_pickup != null)
        {
            
            if(other_pickup.currentPlayer == Networking.LocalPlayer)
            {
                Debug.Log("## YES WE ARE THE SAME; nrObj:"+this.nrOfColliders.ToString());
                this.playerIDs_status[Networking.LocalPlayer.playerId] = PLAYER_IS_INSIDE;
                G_addCollider(other);
                L_addCollider(other);
                SendCustomNetworkEvent(
                    VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                    nameof(s_addColliderFromTemp)
                );
            }else if(other_pickup.currentPlayer != null){
                Debug.Log("## NO WE ARE NOT THE SAME; nrObj:"+this.nrOfColliders.ToString());
                // L_addCollider(other);
                addTempCollider(other);
                // other.attachedRigidbody.detectCollisions = false;
                return; 
                // box.attachedRigidbody.detectCollisions = false;
            }
            
        }
        Debug.Log(
            "## ENTER Triggered     : " + other.GetInstanceID().ToString()  + "\n" + 
            "## ENTER Player ID     : " + Networking.LocalPlayer.playerId.ToString() + "\n" + 
            "## ENTER Is Cube Held? : " + other.GetComponent<VRC_Pickup>().IsHeld
        );
  
    }
    public void OnTriggerExit(Collider other)
    {
        VRC_Pickup other_pickup = other.GetComponent<VRC_Pickup>();
        if(other_pickup != null)
        {
            
            if(other_pickup.currentPlayer == Networking.LocalPlayer)
            {
                this.playerIDs_status[Networking.LocalPlayer.playerId] = PLAYER_IS_OUTSIDE;
                G_removeCollider(other);
                SendCustomNetworkEvent(
                    VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                    nameof(s_removeCollider)
                );
            }else if(other_pickup.currentPlayer != null){
                Debug.Log("## Was AnotherPlayer");
                return;
            }else {
                Debug.Log("## Use Collisions");
                other.attachedRigidbody.detectCollisions = true;
            }
            
        }

        Debug.Log(
            "## EXIT Triggered     : " + other.GetInstanceID().ToString()  + "\n" + 
            "## EXIT Player ID     : " + Networking.LocalPlayer.playerId.ToString() + "\n" + 
            "## EXIT Is Cube Held? : " + other.GetComponent<VRC_Pickup>().IsHeld
            );
    }
}
