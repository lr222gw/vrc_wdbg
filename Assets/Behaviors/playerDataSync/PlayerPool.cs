
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

public class PlayerPool : UdonSharpBehaviour
{
    [SerializeField]
    private VRCObjectPool playerPool; 


    void Start()
    {       
       if(!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Debug.Log("## Player("+Networking.LocalPlayer.playerId+"): Was not Owner!");
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }else {Debug.Log("## Player("+Networking.LocalPlayer.playerId+"): Was Owner!");}

        // Init RosePool
        foreach(GameObject player in playerPool.Pool)
        {
            PlayerData playerDat = player.GetComponent<PlayerData>();
        }

        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        RequestSerialization();
    }
    
    public PlayerData getActivePlayerData(int playerID)
    {        
        int i = 0;
        Debug.Log("### getActivePlayerData" );
        Debug.Log("### playerPool.Pool.Length:" +playerPool.Pool.Length.ToString());
        foreach(var a in this.playerPool.Pool)
        {
            
            Debug.Log("### iteration: " + i.ToString());
            Debug.Log("### a.GetInstanceID: " + a.GetInstanceID().ToString());
            Debug.Log("###  Looking for playerID: " + playerID.ToString());

            if(a.activeSelf)
            {
                Debug.Log("### a.activeSelf:  TRUE");
                PlayerData playerdat = a.GetComponent<PlayerData>();
                if(playerdat.playerID == playerID)
                {
                    Debug.Log("### playerdat.playerID == playerID:  TRUE");
                    return playerdat;
                }
            }
            i++;
        }
        return null;        
    }


    public PlayerData getFreePlayer(VRCPlayerApi recievingPlayer)
    {        
        // if(!Networking.IsOwner(this.playerPool.gameObject))
        if(!Networking.IsOwner(this.playerPool.gameObject))
        {
            // Networking.SetOwner(Networking.LocalPlayer, this.playerPool.gameObject);
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }

        GameObject temp = playerPool.TryToSpawn(); 
        if(!Networking.IsOwner(temp))
        {
            Networking.SetOwner(Networking.LocalPlayer, temp);
        }

        if(temp != null)
        {            
            Debug.Log("## Player["+ Networking.LocalPlayer.displayName + 
                "("+Networking.LocalPlayer.playerId+")"+"]: Got a PlayerSpace");

            if(!Networking.IsOwner(recievingPlayer, temp))
            {
                Networking.SetOwner(recievingPlayer, temp);
            }

            return temp.GetComponent<PlayerData>(); 
        }

        return null; 
    }

    public void releasePlayer(PlayerData playerData)
    {
        // if(!Networking.IsOwner(this.playerPool.gameObject))
        if(!Networking.IsOwner(this.gameObject))
        {
            // Networking.SetOwner(Networking.LocalPlayer, this.playerPool.gameObject);
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }

        playerPool.Return(playerData.gameObject);
        
    }
}
