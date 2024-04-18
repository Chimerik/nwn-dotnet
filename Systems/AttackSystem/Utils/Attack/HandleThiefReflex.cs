using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleThiefReflex(CNWSCreature attacker, CNWSObject currentTarget, CNWSCombatRound combatRound, string attackerName, string targetName)
    {
      if (attacker.m_pStats.HasFeat(CustomSkill.ThiefReflex).ToBool())
      {
        foreach (var eff in attacker.m_appliedEffects)
          if (eff.m_sCustomTag.CompareNoCase(EffectSystem.ThiefReflexExoTag).ToBool())
          {
            combatRound.AddCleaveAttack(currentTarget.m_idSelf);
            DelayMessage($"{attackerName.ColorString(ColorConstants.Cyan)} déchaîne ses réflexes de voleur sur {targetName.ColorString(ColorConstants.Cyan)}", attacker);
            
            attacker.RemoveEffect(eff);
            return;
          }
      }
    }
  }
}
