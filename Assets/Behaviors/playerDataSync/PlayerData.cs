
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerData : UdonSharpBehaviour
{
    public int playerID;
    public int playerIndex;

    private Card[] roseCards;
    public int nrOfroseCards;
    private Card[] skullCards;
    public int nrOfskullCards;

    [UdonSynced]
    private string _json = "";
    private DataDictionary _dictionary;
    private DataList playerList; 
    
    public void addRose(Card card)
    {
        this.roseCards[this.nrOfroseCards] = card;
        this.nrOfroseCards++;
        RequestSerialization();
    }

    public void addSkull(Card card)
    {
        this.skullCards[this.nrOfskullCards] = card;
        this.nrOfskullCards++;
        RequestSerialization();
    }

    public Card getRandomCardId()
    {
        Debug.Log("### getRandomCardId! ");
        Debug.Log("### playerID " + this.playerID.ToString());
        int nrOfCards = this.nrOfroseCards + this.nrOfskullCards;
        Card[] tempAllPlayerCards    = new Card[nrOfCards];        
        for(int i = 0; i < this.nrOfroseCards; i++)
        {                    
            tempAllPlayerCards[i] = this.roseCards[i];
        }
        for(int i = 0; i < this.nrOfskullCards; i++)
        {                    
            tempAllPlayerCards[this.nrOfroseCards+i] = this.skullCards[i];
        }
        int rndCard = Random.Range(0,nrOfCards);
        Debug.Log("### rnd was:" + rndCard.ToString());
        Debug.Log("### tempAllPlayerCards[rndCard].getDebugID() was:" + tempAllPlayerCards[rndCard].getDebugID().ToString());
        
        return tempAllPlayerCards[rndCard];
    }

    public void initCardIndexesArray()
    {
        this.nrOfroseCards =  0;
        this.roseCards = new Card[Game.NR_OF_ROSES_PER_PLAYER];
        this.nrOfskullCards =  0;
        this.skullCards = new Card[Game.NR_OF_SKULLS_PER_PLAYER];
        RequestSerialization();
    }
    void Start()
    {        
         if (_dictionary == null)
        {
            _dictionary = new DataDictionary();
            UpdateSyncedData();
        }
    }

    public string getSyncStr()
    {
        UpdateSyncedData();
        return this._json;
    }
    public void initiateFromSyncStr(string jsonStr)
    {
        this._json = jsonStr;
        loadSyncedData();

    }

    public void UpdateSyncedData()
    {;

        // Adding a key-value pair to the DataDictionary
        _dictionary.SetValue((string)nameof(playerID), playerID);
        _dictionary.SetValue((string)nameof(playerIndex), playerIndex);
        _dictionary.SetValue((string)nameof(nrOfroseCards), nrOfroseCards);
        _dictionary.SetValue((string)nameof(nrOfskullCards), nrOfskullCards);
        
        
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
        Debug.Log("## OnPreSerialization[PlayerData] PlayerID["+Networking.LocalPlayer.playerId.ToString()+"]");
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
        Debug.Log("## OnDeserialization[PlayerData] PlayerID[" + Networking.LocalPlayer.playerId.ToString() + "]");

        if (VRCJson.TryDeserializeFromJson(_json, out DataToken result))
        {
            Debug.Log("## OnDeserialization PlayerID[" + Networking.LocalPlayer.playerId.ToString() + "]: Deserialize Json");
            _dictionary = result.DataDictionary;

            deserializeToken_int(nameof(playerID), out this.playerID);
            deserializeToken_int(nameof(playerIndex), out this.playerIndex);
            deserializeToken_int(nameof(nrOfroseCards), out this.nrOfroseCards);
            deserializeToken_int(nameof(nrOfskullCards), out this.nrOfskullCards);

        }
        else
        {
            Debug.LogError(result.ToString());
        }
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

    public void test ()
    {
        
        Debug.Log("## this is a test"+ playerID.ToString());
    }
}
