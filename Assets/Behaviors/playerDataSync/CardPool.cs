
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using System.Threading;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CardPool : UdonSharpBehaviour
{
    [SerializeField]
    public VRCObjectPool rosePool; 

    [SerializeField]    
    private VRCObjectPool skullPool; 


    int counter;
 
    void Start()
    {
        if(!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Debug.Log("## Player("+Networking.LocalPlayer.playerId+"): Was not Owner!");
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }else {Debug.Log("## Player("+Networking.LocalPlayer.playerId+"): Was Owner!");}

        counter = 0;
        // Init RosePool
        foreach(GameObject rose in rosePool.Pool)
        {
            Card roseCard = rose.GetComponent<Card>();
            roseCard.setSkull(false);
            
            // roseCard.debugID = counter;
            roseCard.setDebugID(counter);
            roseCard.setDebugText(counter.ToString());
            counter++;
            roseCard.setText("ROSE");
        }

        // Init RosePool
        foreach(GameObject skull in skullPool.Pool)
        {
            Card skullCard = skull.GetComponent<Card>();
            
            skullCard.setSkull(true);
            
            // skullCard.debugID = counter;
            skullCard.setDebugID(counter);
            skullCard.setDebugText(counter.ToString());
            counter++;
            skullCard.setText("SKULL");
        }
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        RequestSerialization();
    }


    public Card getFreeRose()
    {        
        if(!Networking.IsOwner(this.rosePool.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.rosePool.gameObject);
        }

        GameObject temp = rosePool.TryToSpawn();
        
        Debug.Log("## Player["+Networking.LocalPlayer.playerId+"] : " + rosePool.GetInstanceID().ToString());
        
        if(temp != null)
        {   
            Debug.Log("## Player["+ Networking.LocalPlayer.displayName + 
                "("+Networking.LocalPlayer.playerId+")"+"]: Got a Rose");

            if(!Networking.IsOwner(Networking.LocalPlayer, temp))
            {
                Networking.SetOwner(Networking.LocalPlayer, temp);
            }

            RequestSerialization();
            var card = temp.GetComponent<Card>();
            card.reset();
            return card; 

        }

        return null; 
    }

    public void releaseRose(Card roseCard)
    {
        if(!Networking.IsOwner(this.rosePool.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.rosePool.gameObject);
        }
        rosePool.Return(roseCard.gameObject);
        
    }
    public Card getFreeSkull()
    {
        if(!Networking.IsOwner(this.skullPool.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.skullPool.gameObject);
        }
        
        GameObject temp = skullPool.TryToSpawn(); 

        if(temp != null)
        {            
            Debug.Log("## Player["+ Networking.LocalPlayer.displayName + 
                "("+Networking.LocalPlayer.playerId+")"+"]: Got a Skull");

            if(!Networking.IsOwner(Networking.LocalPlayer, temp))
            {
                Networking.SetOwner(Networking.LocalPlayer, temp);
            }

            RequestSerialization();
            var card = temp.GetComponent<Card>();
            card.reset();
            return card; 
        }

        return null; 
    }

    public void releaseSkull(Card skullCard)
    {
        if(!Networking.IsOwner(this.skullPool.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.skullPool.gameObject);
        }
        skullPool.Return(skullCard.gameObject);
        
    }
}
