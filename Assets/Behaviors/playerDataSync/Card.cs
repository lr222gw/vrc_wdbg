
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using VRC.SDK3.Data;

public class Card : UdonSharpBehaviour
{
    [SerializeField]
    [UdonSynced]
    private bool skull = false;

    [SerializeField]
    [UdonSynced]
    private int debugID = -1;

    // Add Color
    [SerializeField]
    private Text cardText;
    private string cardText_str;
    [SerializeField]
    private Text cardTextDebug;
    private string cardTextDebug_str;


    private bool pickedUp = false;


    public DataDictionary savemedown()
    {
        return new DataDictionary(){
            {"skull", this.skull},
            {"pickedUp", this.pickedUp},
            {"debugID", this.debugID},
            {"cardTextDebug_str", this.cardTextDebug.text},
            {"cardText_str", this.cardText.text},
        };

    }

    public void fillmeup(DataDictionary cardDat)
    {
        deserializeToken_bool(cardDat,(string)nameof(this.skull), out this.skull);
        deserializeToken_bool(cardDat,(string)nameof(this.pickedUp), out this.pickedUp);
        deserializeToken_int(cardDat,(string)nameof(this.debugID), out this.debugID);
        deserializeToken_string(cardDat,(string)nameof(this.cardTextDebug_str), out this.cardTextDebug_str);
        deserializeToken_string(cardDat,(string)nameof(this.cardText_str), out this.cardText_str);
        cardText.text = this.cardText_str;
        cardTextDebug.text = this.cardTextDebug_str;
    }
    private void deserializeToken_string(DataDictionary dict, string fieldName, out string memberRef)
    {
        if (dict.TryGetValue(fieldName, out DataToken  dat)){            
            memberRef = (string)dat.String;
            Debug.Log($"Success! {dat}");
        }else{
            memberRef = "NULL";
            Debug.LogError("Failed to retrieve score from DataDictionary");}
    }
    private void deserializeToken_int(DataDictionary dict, string fieldName, out int memberRef)
    {
        if (dict.TryGetValue(fieldName, out DataToken  dat)){            
            memberRef = (int)dat.Double;
            Debug.Log($"Success! {dat}");
        }else{
            memberRef = -1;
            Debug.LogError("Failed to retrieve score from DataDictionary");}
    }
    private void deserializeToken_bool(DataDictionary dict, string fieldName, out bool memberRef)
    {
        if (dict.TryGetValue(fieldName, out DataToken  dat)){            
            memberRef = (bool)dat.Boolean;
            Debug.Log($"Success! {dat}");
        }else{
            memberRef = false;
            Debug.LogError("Failed to retrieve score from DataDictionary");}
    }

    void Start()
    {
        // OBS; Be careful defining anything here! 
        // The Pool tries to define some values, however if you define them here
        // you will overwrite whatever the Pool has written! 

        // Better to define them in their declaration as seen above... (?)
    }


    public void reset()
    {
        cardText.gameObject.SetActive(false);                
        pickedUp = false; 
    }


    public void setDebugID(int id)
    {
        debugID = id;
    }
    public int getDebugID()
    {
        return debugID;
    }
    public void setSkull(bool isSkull)
    {
        skull = isSkull;
    }
    public bool isSkull()
    {
        return skull;
    }
    public void setText(string text)
    {
        cardText.text = text;
    }
    public void setDebugText(string text)
    {
        cardTextDebug.text = text;
    }
}
