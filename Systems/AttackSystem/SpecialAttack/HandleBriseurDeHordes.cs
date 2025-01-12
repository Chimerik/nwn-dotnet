using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleBriseurDeHordes(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound, string attackerName, CNWSItem weapon, bool isRangedAttack)
    {
      if (weapon is null || !attacker.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.BriseurDeHordesEffectTag))
        return;

      CNWSCreature target = null;

      if (isRangedAttack)
      {
        foreach (var targetId in currentTarget.GetArea().m_aGameObjects)
        {
          if (targetId == currentTarget.m_idSelf)
            continue;

          CNWSCreature areaCreature = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(targetId);

          if (areaCreature is null || areaCreature.m_idSelf == 0x7F000000 // OBJECT_INVALID
            || Vector3.DistanceSquared(areaCreature.m_vPosition.ToManagedVector(), currentTarget.m_vPosition.ToManagedVector()) > 9)
            continue;

          target = areaCreature;
          break;
        }
      }
      else
      {
        target = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.GetNearestEnemy(3, currentTarget.m_idSelf, 1, 1));
        
        if (target is null || target.m_idSelf == 0x7F000000) // OBJECT_INVALID
          return;
      }

      if (target is null)
        return;

      EffectUtils.RemoveTaggedNativeEffect(attacker, EffectSystem.BriseurDeHordesEffectTag);
      attacker.m_ScriptVars.SetInt(CreatureUtils.BriseurDeHordesVariableExo, 1);

      combatRound.AddWhirlwindAttack(target.m_idSelf, 1);

      string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      DelayMessage($"{attackerName.ColorString(ColorConstants.Cyan)} brise la horde de {targetName.ColorString(ColorConstants.Cyan)}", attacker);
    }
  }
}
