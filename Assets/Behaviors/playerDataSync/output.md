Your understanding seems to be largely correct. However, be aware that as of Udon version 0.6.0, Udon Networking only supports UdonSynced on Basic Types, not UdonBehavior Array Types nor UdonBehaviours. With UdonSharp, you are also currently unable to sync UdonSynced on object (UdonBehaviour-derived types), arrays or lists due to lack of support.

It seems like you are trying to sync an array of `PlayerData` objects. Unfortunately, the current system may seem to be able to support your structure with each object having its own `_json` but upon trying to sync such objects as arrays or list, it will fail.

What you are showing is a clear and structured implementation, but the fundamental problem is the lack of support for syncing non-basic type. A workaround for this would be to split all the necessary data into basic types that can be synced like so:

```csharp
[UdonSynced] private string syncedPlayerId;
[UdonSynced] private int syncedPlayerIndex;
[UdonSynced] private int syncedRoseCards;
[UdonSynced] private int syncedSkullCards;

public void StartOnClick()
{
    // rest of the code just like before...

    foreach (VRCPlayerApi player in allPlayers)
    {
      this.players[this.nrOfPlayers] = playerPool.getFreePlayer(player);
      this.players[this.nrOfPlayers].initCardIndexesArray();

      // rest of the code just like before...
      this.players[this.nrOfPlayers].RequestSerialization();
      this.nrOfPlayers++;

      syncedPlayerId += player.playerId + ";";
      syncedPlayerIndex += this.players[this.nrOfPlayers].playerIndex + ";";
      syncedRoseCards += this.players[this.nrOfPlayers].nrOfroseCards + ";";
      syncedSkullCards += this.players[this.nrOfPlayers].nrOfskullCards + ";";

    }

    RequestSerialization();
}

public override void OnDeserialization()
{
  string[] playerIdArray = syncedPlayerId.Split(';');
  string[] playerIndexArray = syncedPlayerIndex.Split(';');
  string[] nrOfroseCardsArray = syncedRoseCards.Split(';');
  string[] nrOfskullCardsArray = syncedSkullCards.Split(';');

  for(int i = 0; i < playerIdArray.Length - 1; i++) // Subtract one due to the last ";" yielding an empty string
  {
    this.players[i].playerId = int.Parse(playerIdArray[i]);
    this.players[i].playerIndex = int.Parse(playerIndexArray[i]);
    this.players[i].nrOfroseCards = int.Parse(nrOfroseCardsArray[i]);
    this.players[i].nrOfskullCards = int.Parse(nrOfskullCardsArray[i]);
  }
}
```

This method of string splitting can have limitations with regards to maximum length of a string, and type-severity. For instance, if the id, index or amount of roses and skulls ever contain the ";" character, it would break this system. Then you would have to implement another separator system instead.

Keep in mind that synchronization is not immediate. When you call `RequestSerialization`, data is queued to be synced, not sent immediately. You could use `SendCustomNetworkEvent` to ensure when the clients should update their data using the new synced values.  You would still need to code logic to maintain the right order of operations when working with multiplayer synchronization. Also, the code provided is just a workaround given the current UdonSharp limitations. When UdonSharp starts supporting complex types like dictionaries, arrays and lists for syncing, you can come back to your original approach.