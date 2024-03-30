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
        byte? warMasterLevel = onRest.Player.LoginCreature.GetClassInfo(ClassType.Fighter);

        if (warMasterLevel.HasValue)
          RestoreManoeuvres(onRest.Player.LoginCreature, warMasterLevel.Value);
      }*/
    }
  }
}
