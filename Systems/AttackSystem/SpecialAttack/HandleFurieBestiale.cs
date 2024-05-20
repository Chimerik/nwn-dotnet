using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleFurieBestiale(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound, string attackerName)
    {
      if(attacker.m_ScriptVars.GetInt(CreatureUtils.FurieBestialeVariableExo).ToBool())
      {
        foreach (var targetId in currentTarget.GetArea().m_aGameObjects)
        {
          var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(targetId);

          if (target is null || target.m_idSelf == 0x7F000000 // OBJECT_INVALID
            || Vector3.DistanceSquared(target.m_vPosition.ToManagedVector(), attacker.m_vPosition.ToManagedVector()) > 9)
            continue;

          combatRound.AddWhirlwindAttack(target.m_idSelf, 1);
        }

        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} : Furie Bestiale", attacker);
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.FurieBestialeVariableExo);
      }
    }
  }
}
