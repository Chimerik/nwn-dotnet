using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsEnchanteurRedirection(CNWSCreature attacker, CNWSCreature target, CNWSCombatRound combatRound, string attackerName)
    {
      if (!target.m_pStats.HasFeat(CustomSkill.EnchantementCharmeInstinctif).ToBool()
        || EffectSystem.IsCharmeImmune(target, attacker))
        return false;

      var reaction = target.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.ReactionEffectTag);

      if(reaction is null || target.m_ScriptVars.GetString(CreatureUtils.CharmeInstinctifVariableExo).ToString().Split("_").Any(i => i == attacker.m_idSelf.ToString()))
        return false;

      var newTarget = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(target.GetNearestEnemy(4, attacker.m_idSelf, 1, 1));

      if (newTarget is null || newTarget.m_idSelf == 0x7F000000)
        return false;

      string newTargetName = $"{newTarget.GetFirstName().GetSimple(0)} {newTarget.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      BroadcastNativeServerMessage($"{targetName.ColorString(ColorConstants.Cyan)} redirige l'attaque de {attackerName.ColorString(ColorConstants.Cyan)} sur {newTargetName.ColorString(ColorConstants.Cyan)}", target);

      combatRound.AddWhirlwindAttack(newTarget.m_idSelf, 1);
      target.RemoveEffect(reaction);
      target.m_ScriptVars.SetString(CreatureUtils.CharmeInstinctifVariableExo, (target.m_ScriptVars.GetString(CreatureUtils.CharmeInstinctifVariableExo).ToString() + $"{attacker.m_idSelf}_").ToExoString());

      return true;
    }
  }
}
