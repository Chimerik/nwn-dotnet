using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnRest(ModuleEvents.OnPlayerRest onRest)
    {
      /*(onRest.RestEventType == RestEventType.Finished)
      {
        byte? warMasterLevel = onRest.Player.LoginCreature.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter))?.Level;

        if (warMasterLevel.HasValue)
          RestoreManoeuvres(onRest.Player.LoginCreature, warMasterLevel.Value);
      }*/
    }
  }
}
