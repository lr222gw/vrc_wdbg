
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerManager : UdonSharpBehaviour
{
    public int nrOfPlayers;
    public int[] playerIDs;
    public int[] playerIndicies;

    private Card[][] roseCards;
    public int[] nrOfroseCards;
    private Card[][] skullCards;
    public int[] nrOfskullCards;

    [SerializeField]
    private GameObject CardPrefab;

    [UdonSynced]
    private string _json = "";
    private DataDictionary _dictionary = new DataDictionary()
    {
        {"nrOfPlayers", -1},
        {"playerIDs", new DataList()},
        {"playerIndicies", new DataList()},
        {"nrOfskullCards", new DataList()},
        {"nrOfroseCards", new DataList()},
        {"roseCards", new DataList(){new DataList()}},
        {"skullCards", new DataList(){new DataList()}}
        
    };

    public void addPlayer(int playerID, int playerIndex)
    {
        if(nrOfPlayers < Game.MAX_NR_OF_PLAYERS)
        {
            playerIDs[nrOfPlayers] = playerID;
            playerIndicies[nrOfPlayers] = playerIndex;
            nrOfroseCards[nrOfPlayers] = 0;
            nrOfskullCards[nrOfPlayers] = 0;
            nrOfPlayers++;
        }        

    }

    public void logPlayersCards(int playerIndex)
    {
        Debug.Log("## Player["+playerIndex.ToString()+"] has the following cards: ");
        foreach(Card c in this.roseCards[playerIndex])
        {
            Debug.Log("## \t "+c.getDebugID().ToString());
        }
    }
    
    void Start()
    {
        this.playerIDs       = new int[Game.MAX_NR_OF_PLAYERS];        
        this.playerIndicies  = new int[Game.MAX_NR_OF_PLAYERS];        
        this.roseCards       = new Card[Game.MAX_NR_OF_PLAYERS][];// Game.NR_OF_ROSES_PER_PLAYER  ];
        this.skullCards      = new Card[Game.MAX_NR_OF_PLAYERS][];// Game.NR_OF_SKULLS_PER_PLAYER ];
        this.nrOfroseCards   = new int[Game.MAX_NR_OF_PLAYERS];
        this.nrOfskullCards  = new int[Game.MAX_NR_OF_PLAYERS];
        this.nrOfPlayers     = 0;

        for(int i = 0; i < this.roseCards.Length;i++)
        {
            this.roseCards[i]   = new Card[Game.NR_OF_ROSES_PER_PLAYER];
            for(int j = 0; j < this.roseCards[i].Length;j++)
            {
                // Instantiate a new instance of the Card prefab
                GameObject myCardObject = GameObject.Instantiate(CardPrefab);        
                this.roseCards[i][j] = myCardObject.GetComponent<Card>();
            }
        }
        for(int i = 0; i < this.skullCards.Length;i++){
            this.skullCards[i] = new Card[Game.NR_OF_SKULLS_PER_PLAYER];
            for(int j = 0; j < this.skullCards[i].Length;j++)
            {
                // Instantiate a new instance of the Card prefab
                GameObject myCardObject = GameObject.Instantiate(CardPrefab);        
                this.skullCards[i][j] = myCardObject.GetComponent<Card>();
            }
        }        

        for(int i = 0; i < this.playerIDs.Length;i++){
            this.playerIDs[i] = -1;
            _dictionary["playerIDs"].DataList.Add(-1);
        }
        for(int i = 0; i < this.playerIndicies.Length;i++){
            this.playerIndicies[i] = -1; 
            _dictionary["playerIndicies"].DataList.Add(-1);
        }
        for(int i = 0; i < this.nrOfroseCards.Length;i++){
            this.nrOfroseCards[i] = -1;
            _dictionary["nrOfroseCards"].DataList.Add(-1);
        }
        for(int i = 0; i < this.nrOfskullCards.Length;i++){
            this.nrOfskullCards[i] = -1;
            _dictionary["nrOfskullCards"].DataList.Add(-1);
        }
        
    }

    public void addRose(int playerIndex, Card card)
    {
        this.roseCards[playerIndex][nrOfroseCards[playerIndex]] = card;
        nrOfroseCards[playerIndex]++;
    }
    public void addSkull(int playerIndex, Card card)
    {
        this.skullCards[playerIndex][nrOfskullCards[playerIndex]] = card;
        nrOfskullCards[playerIndex]++;
    }
    public int getPlayerIndex(int playerID){
        for(int i = 0; i < nrOfPlayers; i++)
        {
            if(this.playerIDs[i] == playerID){return i;}
        }
        return -1;
    }
    public int getPlayerID(int playerIndex){
        return this.playerIDs[playerIndex];        
    }
    public int getNrOfRoseCards(int playerIndex){
        return this.nrOfroseCards[playerIndex];        
    }
    public int getNrOfSkullCards(int playerIndex){
        return this.nrOfskullCards[playerIndex];        
    }

    private void helper_int(string collectionName,int[] collection)
    {
        for(int i = 0; i < collection.Length;i++)
        {
            _dictionary[collectionName].DataList.SetValue(i, collection[i]);            
        }
    }
    private void helper_cardArr(string collectionName,Card[][] collection)
    {
        for(int i = 0; i < this.nrOfPlayers;i++)
        {
            for(int j = 0; j < collection[i].Length; j++){
                _dictionary[collectionName].DataList[i].DataDictionary.SetValue(j,collection[i][j].savemedown());
                //.SetValue(i, collection[i]);
                
            }
                        
        }
    }
    public void UpdateSyncedData()
    {

        // Adding a key-value pair to the DataDictionary
        _dictionary.SetValue((string)nameof(nrOfPlayers), nrOfPlayers);

        helper_int((string)nameof(playerIDs), playerIDs);
        helper_int((string)nameof(playerIndicies), playerIndicies);
        helper_int((string)nameof(nrOfroseCards), nrOfroseCards);
        helper_int((string)nameof(nrOfskullCards), nrOfskullCards); 

        
        helper_cardArr((string)nameof(roseCards), roseCards);
        helper_cardArr((string)nameof(skullCards), skullCards);
        
        
        
        // Serializing the DataDictionary to JSON
        if (VRCJson.TrySerializeToJson(_dictionary, JsonExportType.Minify, out DataToken result))
        {
            _json = result.String;
            // RequestSerialization();
        }
        else
        {
            Debug.LogError(result.ToString());
        }
    }

    public override void OnPreSerialization()
    {
        Debug.Log("## OnPreSerialization[PlayerManager] PlayerID["+Networking.LocalPlayer.playerId.ToString()+"]");
        UpdateSyncedData();
    }

    public override void OnDeserialization()
    {
        // base.OnDeserialization(); 

        loadSyncedData();

        // RequestSerialization();
    }

    public void loadSyncedData()
    {
        Debug.Log("## OnDeserialization[PlayerManager] PlayerID[" + Networking.LocalPlayer.playerId.ToString() + "]");

        if (VRCJson.TryDeserializeFromJson(_json, out DataToken result))
        {
            Debug.Log("## OnDeserialization PlayerID[" + Networking.LocalPlayer.playerId.ToString() + "]: Deserialize Json");
            _dictionary = result.DataDictionary;

            deserializeToken_int(nameof(nrOfPlayers), out this.nrOfPlayers);

            deserializeToken_intArr(nameof(playerIDs), out this.playerIDs);
            deserializeToken_intArr(nameof(playerIndicies), out this.playerIndicies);
            deserializeToken_intArr(nameof(nrOfroseCards), out this.nrOfroseCards);
            deserializeToken_intArr(nameof(nrOfskullCards), out this.nrOfskullCards);
            deserializeToken_cardArrArr(nameof(roseCards), out this.roseCards );
            deserializeToken_cardArrArr(nameof(skullCards), out this.skullCards );

        }
        else
        {
            Debug.LogError(result.ToString());
        }
    }

    private void deserializeToken_cardArrArr(string fieldName, out Card[][] memberRef)
    {
        if (_dictionary.TryGetValue(fieldName, out DataToken  dat)){            
            memberRef = new Card[dat.DataList.Count][];
            for( int i = 0; i < dat.DataList.Count; i++)
            {                
                memberRef[i] = new Card[dat.DataList.Count];
                for(int j = 0; j < dat.DataList[i].DataList.Count;j++)
                {
                    // memberRef[i][j] = dat.DataList[i].DataList[j].String;
                    DataDictionary cardDic = dat.DataList[i].DataList[j].DataDictionary;            
                    memberRef[i][j].fillmeup(cardDic);
                    
                }
            }
            
            Debug.Log($"Success! {dat}");
        }else{
            memberRef = new Card[dat.DataList.Count][];
            // memberRef = new int[dat.DataList.Count];
            // for( int i = 0; i < dat.DataList.Count; i++)
            // {                
            //     memberRef[i] = -1;
            // }
            Debug.LogError("Failed to retrieve score from DataDictionary");}
    }

    private void deserializeToken_intArr(string fieldName, out int[] memberRef)
    {
        if (_dictionary.TryGetValue(fieldName, out DataToken  dat)){            
            memberRef = new int[dat.DataList.Count];
            for( int i = 0; i < dat.DataList.Count; i++)
            {                
                memberRef[i] = (int)dat.DataList[i].Double;
            }
            
            Debug.Log($"Success! {dat}");
        }else{
            memberRef = new int[dat.DataList.Count];
            for( int i = 0; i < dat.DataList.Count; i++)
            {                
                memberRef[i] = -1;
            }
            Debug.LogError("Failed to retrieve score from DataDictionary");}
    }


    private void deserializeToken_int(string fieldName, out int memberRef)
    {
        if (_dictionary.TryGetValue(fieldName, out DataToken  dat)){            
            memberRef = (int)dat.Double;
            Debug.Log($"Success! {dat}");
        }else{
            memberRef = -1;
            Debug.LogError("Failed to retrieve score from DataDictionary");}
    }
    private void deserializeToken_bool(string fieldName, out bool memberRef)
    {
        if (_dictionary.TryGetValue(fieldName, out DataToken  dat)){            
            memberRef = (bool)dat.Boolean;
            Debug.Log($"Success! {dat}");
        }else{
            memberRef = false;
            Debug.LogError("Failed to retrieve score from DataDictionary");}
    }
}
