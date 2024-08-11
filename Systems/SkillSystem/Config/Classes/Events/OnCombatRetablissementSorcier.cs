
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static void OnCombatEnsoRecoverSource(OnCombatStatusChange onStatus)
    {
      if (onStatus.CombatStatus == CombatStatus.EnterCombat && GetSorcerySource(onStatus.Player.LoginCreature) < 1)
      {
        byte? level = onStatus.Player.LoginCreature.GetClassInfo(ClassType.Sorcerer)?.Level;

        if (!level.HasValue)
          return;

        RestoreSorcerySource(onStatus.Player.LoginCreature, (byte)(level.Value / 5));
      }
    }
  }
}
