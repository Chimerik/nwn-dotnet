using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public async void TeleportToSavedLocation()
      {
        LogUtils.LogMessage($"{oid.PlayerName} location.Area connection : {location.Area?.Name}", LogUtils.LogType.PlayerConnections);

        if (location.Area is null)
        {
          var query = SqLiteUtils.SelectQuery("playerCharacters",
          new List<string>() { { "location" } },
          new List<string[]>() { { new string[] { "rowid", characterId.ToString() } } });
          
          foreach (var result in query)
            location = SqLiteUtils.DeserializeLocation(result[0]);

          LogUtils.LogMessage($"{oid.PlayerName} location.Area DB : {location.Area?.Name}", LogUtils.LogType.PlayerConnections);
          LogUtils.LogMessage($"{oid.PlayerName} in characted creation : {oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION").HasValue}", LogUtils.LogType.PlayerConnections);

          if (location.Area is null && oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION").HasValue)
            location = CreateIntroScene(oid, areaSystem);

          LogUtils.LogMessage($"{oid.PlayerName} final location : {location.Area?.Name}", LogUtils.LogType.PlayerConnections);

          await NwTask.WaitUntil(() => oid.LoginCreature.Location.Area is not null);
          oid.LoginCreature.Location = location;
        }
      }
    }
  }
}
