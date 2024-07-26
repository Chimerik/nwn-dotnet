using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleVolee(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound, bool isRangedAttack, string attackerName)
    {
      if(attacker.m_ScriptVars.GetInt(CreatureUtils.HunterVoleeVariableExo).ToBool())
      {
        CNWSObject center = isRangedAttack ? currentTarget : attacker;

        foreach (var targetId in currentTarget.GetArea().m_aGameObjects)
        {
          var target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(targetId);

          if (target is null || target.m_idSelf == 0x7F000000 // OBJECT_INVALID
            || target.m_idSelf == attacker.m_idSelf || target.m_idSelf == currentTarget.m_idSelf
            || Vector3.DistanceSquared(target.m_vPosition.ToManagedVector(), center.m_vPosition.ToManagedVector()) > 16)
            continue;

          combatRound.AddWhirlwindAttack(target.m_idSelf, 1);
        }

        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} : Volée", attacker);
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.HunterVoleeVariableExo);
      }
    }
  }
}
