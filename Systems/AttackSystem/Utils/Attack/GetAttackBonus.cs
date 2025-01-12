using System.Collections.Generic;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetAttackBonus(CNWSCreature attacker, CNWSCreature target, CNWSCombatAttackData attackData, CNWSItem weapon, Anvil.API.Ability attackAbility)
    {
      int attackBonus = attacker.m_pStats.GetAttackModifierVersus(target);
      attackBonus -= attackData.m_bRangedAttack.ToBool() 
        ? attackBonus -= GetAbilityModifier(attacker, Anvil.API.Ability.Dexterity) : attackBonus -= GetAbilityModifier(attacker, Anvil.API.Ability.Strength);

      LogUtils.LogMessage($"modifier versus target : {attackBonus}", LogUtils.LogType.Combat);

      int abilityModifier = GetAbilityModifier(attacker, attackAbility);
      LogUtils.LogMessage($"Adding {attackAbility} modifier : {abilityModifier}", LogUtils.LogType.Combat);
      attackBonus += abilityModifier;

      if (attackData.m_bRangedAttack.ToBool() && attacker.m_pStats.HasFeat(CustomSkill.FighterCombatStyleArchery).ToBool())
      {
        attackBonus += 2;
        LogUtils.LogMessage("Style de combat archerie : +2 BA", LogUtils.LogType.Combat);
      }

      List<string> noStack = new();

      foreach(var eff in attacker.m_appliedEffects)
      {
        string tag = eff.m_sCustomTag.ToString();

        if (noStack.Contains(tag))
          continue;

        switch(tag)
        {
          case EffectSystem.WildMagicBienfaitEffectTag: attackBonus += GetWildMagicBienfaitBonus(noStack); break;
          case EffectSystem.FleauEffectTag: attackBonus -= GetFleauMalus(noStack); break;
          case EffectSystem.BenedictionEffectTag: attackBonus += GetBenedictionBonus(noStack); break;
          case EffectSystem.FaveurDuMalinEffectTag: attackBonus += GetFaveurDuMalinBonus(attacker, eff, noStack); break;
        }
      }

      foreach (var eff in target.m_appliedEffects)
      {
        string tag = eff.m_sCustomTag.ToString();

        if (noStack.Contains(tag))
          continue;

        switch (tag)
        {
          case EffectSystem.FrappeDechiranteEffectTag: attackBonus += GetFrappeDechiranteBonus(attacker, eff, noStack); break;
          case EffectSystem.ProtectionContreLesLamesEffectTag: attackBonus -= GetProtectionContreLesLamesMalus(noStack); break;

        }
      }
      
      return attackBonus;
    }
  }
}
