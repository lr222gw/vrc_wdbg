
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Data;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Game : UdonSharpBehaviour
{    
    public const int NR_OF_ROSES_PER_PLAYER = 4;
    public const int NR_OF_SKULLS_PER_PLAYER = 1;
    public const int MAX_NR_OF_PLAYERS = 6;
    private Card[] roses;
    private Card[] skulls;
    [SerializeField]
    private PlayerPool playerPool;
    [SerializeField]
    private CardPool cardPool;
    private PlayerData[] players;

    [UdonSynced]
    private int nrOfPlayers;
    [UdonSynced]
    private int nrOfRoses;
    [UdonSynced]
    private int nrOfSkulls;

    [SerializeField]
    private PlayerManager playerMan;
    

    public override void OnDeserialization()
    {

        
        Debug.Log("## OnDeserialization[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());
        
    }

    private void UpdateSyncedData()
    {
        
    }

    public override void OnPreSerialization()
    {
        Debug.Log("## OnPreSerialization[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());
        
    }
    

    void Start()
    {
        this.players= new PlayerData[MAX_NR_OF_PLAYERS];        
        this.roses  = new Card[Game.NR_OF_ROSES_PER_PLAYER  * MAX_NR_OF_PLAYERS];
        this.skulls = new Card[Game.NR_OF_SKULLS_PER_PLAYER * MAX_NR_OF_PLAYERS];
        this.nrOfRoses   = 0;
        this.nrOfSkulls  = 0;
        this.nrOfPlayers = 0;
    }
    
    public void print_p1_cards(){ this.playerMan.logPlayersCards(0);}
    public void print_p2_cards(){ this.playerMan.logPlayersCards(1);}
    
    public void displayPlayerData()
    {
        player1_index.text           = playerMan.getPlayerIndex(1).ToString();
        player1_id.text              = playerMan.getPlayerID(0).ToString();
        player1_nrRoseCards.text     = playerMan.getNrOfRoseCards(0).ToString();
        player1_nrSkullCards.text    = playerMan.getNrOfSkullCards(0).ToString();
        player2_index.text           = playerMan.getPlayerIndex(2).ToString();
        player2_id.text              = playerMan.getPlayerID(1).ToString();
        player2_nrRoseCards.text     = playerMan.getNrOfRoseCards(1).ToString();
        player2_nrSkullCards.text    = playerMan.getNrOfSkullCards(1).ToString();
        // player1_index.text           = players[0].playerIndex.ToString();
        // player1_id.text              = players[0].playerID.ToString();
        // player1_nrRoseCards.text     = players[0].nrOfroseCards.ToString();
        // player1_nrSkullCards.text    = players[0].nrOfskullCards.ToString();
        // player2_index.text           = players[1].playerIndex.ToString();
        // player2_id.text              = players[1].playerID.ToString();
        // player2_nrRoseCards.text     = players[1].nrOfroseCards.ToString();
        // player2_nrSkullCards.text    = players[1].nrOfskullCards.ToString();
    }
    public void LogPlayerID()
    {
        Debug.Log("## Player ID = "+Networking.LocalPlayer.playerId.ToString());
    }
    public void StartOnClick()
    {
        Debug.Log("## StartOnClick ["+Networking.LocalPlayer.playerId.ToString()+"]");
        
        VRCPlayerApi[] allPlayers = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]; 
        VRCPlayerApi.GetPlayers(allPlayers);
        
        foreach (VRCPlayerApi player in allPlayers) 
        {
            this.players[this.nrOfPlayers] = playerPool.getFreePlayer(player);
            this.players[this.nrOfPlayers].initCardIndexesArray();
            playerMan.addPlayer(player.playerId,this.nrOfPlayers);

            for(int i = 0; i < NR_OF_ROSES_PER_PLAYER; i++)
            {
                this.roses[nrOfRoses]  = cardPool.getFreeRose();
                this.players[this.nrOfPlayers].addRose(this.roses[nrOfRoses]);
                playerMan.addRose(this.nrOfPlayers, this.roses[nrOfRoses]);
                nrOfRoses++;
            }
            for(int i = 0; i < NR_OF_SKULLS_PER_PLAYER; i++)
            {
                this.skulls[nrOfSkulls]  = cardPool.getFreeSkull();
                this.players[this.nrOfPlayers].addSkull(this.skulls[nrOfSkulls]);
                playerMan.addSkull(this.nrOfPlayers, this.skulls[nrOfSkulls]);
                nrOfSkulls++;
            }
            this.players[this.nrOfPlayers].playerIndex = this.nrOfPlayers;
            this.players[this.nrOfPlayers].playerID = player.playerId;

            // this.players[this.nrOfPlayers].UpdateSyncedData();
            // this.players[this.nrOfPlayers].RequestSerialization();
            this.nrOfPlayers++;            
            
        }

        RequestSerialization();
    }

    // public const int NR_OF_ROSES_PER_PLAYER = 4;
    // public const int NR_OF_SKULLS_PER_PLAYER = 1;
    // public const int MAX_NR_OF_PLAYERS = 6;
    // private Card[] roses;
    // private Card[] skulls;
    // [SerializeField]
    // private PlayerPool playerPool;
    // [SerializeField]
    // private CardPool cardPool;
    // private PlayerData[] players;
    // [UdonSynced]
    // private string players_jsonSync;
    // private DataList players_strSync;

    // [UdonSynced]
    // private int nrOfPlayers;
    // [UdonSynced]
    // private int nrOfRoses;
    // [UdonSynced]
    // private int nrOfSkulls;
    
    // public void updatePlayersFromStr()
    // {
    //      Debug.Log("## updatePlayersFromStr[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());    
    //     if (VRCJson.TryDeserializeFromJson(this.players_jsonSync, out DataToken result))
    //     {
    //         Debug.Log("## Sucess at Deserialize players_jsonSync \nupdatePlayersFromStr[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());    
    //         this.players_strSync = result.DataList;
    //     }
    // }
    // // public PlayerData playersStr_to_Players(int index)
    // // {
    // //      Debug.Log("## playersStr_to_Players[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());    
    // //     if (VRCJson.TryDeserializeFromJson(this.players_strSync[index].String, out DataToken result))
    // //     {
    // //         Debug.Log("## Sucess at Deserialize this.players_strSync["+index.ToString()+"].String \nplayersStr_to_Players[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());    
    // //         this.players_strSync = result.DataList;
    // //     }
    // // }

    // public override void OnDeserialization()
    // {

        
    //     Debug.Log("## OnDeserialization[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());
    //     updatePlayersFromStr();
    //     base.OnDeserialization(); 
    //     for (int i = 0; i < players_strSync.Count; i++)
    //     {
    //         // players[i] = playersStr_to_Players(i);
    //         PlayerData player = players[i];
    //         if (player == null)
    //         {
    //             Debug.Log("## player was null! \nOnDeserialization[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());    
    //         }
    //         else
    //         {
    //             player.UpdateSyncedData();
    //             Debug.Log("## player was not NulL! \nOnDeserialization[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());
    //         }                        
    //     }
    // }

    // private void UpdateSyncedData()
    // {
        
    // }

    // public override void OnPreSerialization()
    // {
    //     Debug.Log("## OnPreSerialization[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());
    //     foreach (PlayerData player in players)
    //     {
    //         if (player == null)
    //         {
    //             Debug.Log("## player was null! \nOnPreSerialization[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());    
    //         }
    //         else
    //         {
    //             player.loadSyncedData();
    //             Debug.Log("## player was not NulL! \nOnPreSerialization[GAME]: Player ID = "+Networking.LocalPlayer.playerId.ToString());
    //         }                        
    //     }
    // }
    

    // void Start()
    // {
    //     this.players= new PlayerData[MAX_NR_OF_PLAYERS];        
    //     this.roses  = new Card[Game.NR_OF_ROSES_PER_PLAYER  * MAX_NR_OF_PLAYERS];
    //     this.skulls = new Card[Game.NR_OF_SKULLS_PER_PLAYER * MAX_NR_OF_PLAYERS];
    //     this.nrOfRoses   = 0;
    //     this.nrOfSkulls  = 0;
    //     this.nrOfPlayers = 0;
    // }
    
    // public void displayPlayerData()
    // {
    //     player1_index.text           = players[0].playerIndex.ToString();
    //     player1_id.text              = players[0].playerID.ToString();
    //     player1_nrRoseCards.text     = players[0].nrOfroseCards.ToString();
    //     player1_nrSkullCards.text    = players[0].nrOfskullCards.ToString();
    //     player2_index.text           = players[1].playerIndex.ToString();
    //     player2_id.text              = players[1].playerID.ToString();
    //     player2_nrRoseCards.text     = players[1].nrOfroseCards.ToString();
    //     player2_nrSkullCards.text    = players[1].nrOfskullCards.ToString();
    // }
    // public void LogPlayerID()
    // {
    //     Debug.Log("## Player ID = "+Networking.LocalPlayer.playerId.ToString());
    // }
    // public void StartOnClick()
    // {
    //     Debug.Log("## StartOnClick ["+Networking.LocalPlayer.playerId.ToString()+"]");
        
    //     VRCPlayerApi[] allPlayers = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]; 
    //     VRCPlayerApi.GetPlayers(allPlayers);
        
    //     foreach (VRCPlayerApi player in allPlayers) 
    //     {
    //         this.players[this.nrOfPlayers] = playerPool.getFreePlayer(player);
    //         this.players[this.nrOfPlayers].initCardIndexesArray();

    //         for(int i = 0; i < NR_OF_ROSES_PER_PLAYER; i++)
    //         {
    //             this.roses[nrOfRoses]  = cardPool.getFreeRose();
    //             this.players[this.nrOfPlayers].addRose(this.roses[nrOfRoses]);
    //             nrOfRoses++;
    //         }
    //         for(int i = 0; i < NR_OF_SKULLS_PER_PLAYER; i++)
    //         {
    //             this.skulls[nrOfSkulls]  = cardPool.getFreeSkull();
    //             this.players[this.nrOfPlayers].addSkull(this.skulls[nrOfSkulls]);
    //             nrOfSkulls++;
    //         }
    //         this.players[this.nrOfPlayers].playerIndex = this.nrOfPlayers;
    //         this.players[this.nrOfPlayers].playerID = player.playerId;

    //         this.players_strSync.Add(this.players[this.nrOfPlayers].getSyncStr());

    //         // this.players[this.nrOfPlayers].UpdateSyncedData();
    //         // this.players[this.nrOfPlayers].RequestSerialization();
    //         this.nrOfPlayers++;            
            
    //     }
    //     if (VRCJson.TrySerializeToJson(this.players_strSync, JsonExportType.Minify, out DataToken result))
    //     {
    //         this.players_jsonSync = result.String;
    //     }
    //     RequestSerialization();
    // }

    public Text player1_index;
    public Text player1_id;
    public Text player1_nrRoseCards;
    public Text player1_nrSkullCards;
    public Text player2_index;
    public Text player2_id;
    public Text player2_nrRoseCards;
    public Text player2_nrSkullCards;
}
