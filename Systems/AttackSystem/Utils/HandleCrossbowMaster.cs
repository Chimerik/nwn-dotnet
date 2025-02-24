using System;
using System.Linq;
using Anvil.API;
using NWN.Native.API;
using Feat = Anvil.API.Feat;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleCrossbowMaster(CNWSCreature attacker, CNWSObject target, CNWSCombatRound combatRound, CNWSItem weapon, string attackerName)
    {
      if (weapon is not null && !attacker.m_ScriptVars.GetInt(CreatureUtils.BonusAttackCooldownVariableExo).ToBool()
        && weapon.m_nBaseItem == (uint)BaseItemType.Shuriken && attacker.m_pStats.HasFeat((ushort)Feat.RapidReload).ToBool())
      {
        var bonusAction = attacker.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.BonusActionEffectTag);

        if (bonusAction is null)
          return;

        //string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
        //($"{attackerName.ColorString(ColorConstants.Cyan)} maître arbalétrier contre {targetName}", attacker);

        RemoveBonusAttackCooldown(attacker);
        attacker.RemoveEffect(bonusAction);
        combatRound.AddCleaveAttack(target.m_idSelf);
        attacker.m_ScriptVars.SetInt(CreatureUtils.BonusAttackCooldownVariableExo, 1);

        LogUtils.LogMessage($"Attaque supplémentaire - Maître arbalétrier", LogUtils.LogType.Combat);
      }
    }
    private static async void RemoveBonusAttackCooldown(CNWSCreature attacker)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(2));
      attacker.m_ScriptVars.DestroyInt(CreatureUtils.BonusAttackCooldownVariableExo);
    }
  }
}
