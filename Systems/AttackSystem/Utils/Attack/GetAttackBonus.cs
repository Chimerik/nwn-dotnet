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

      List<string> appliedEffects = new();

      foreach(var eff in attacker.m_appliedEffects)
      {
        if(eff.m_sCustomTag.CompareNoCase(EffectSystem.WildMagicBienfaitExoTag).ToBool() && !appliedEffects.Contains(EffectSystem.WildMagicBienfaitEffectTag))
        {
          int boonBonus = NwRandom.Roll(Utils.random, 4);
          attackBonus += boonBonus;
          appliedEffects.Add(EffectSystem.WildMagicBienfaitEffectTag);
          LogUtils.LogMessage($"Magie Sauvage : +{boonBonus} BA", LogUtils.LogType.Combat);
        }
        else if (eff.m_sCustomTag.CompareNoCase(EffectSystem.FleauEffectExoTag).ToBool() && !appliedEffects.Contains(EffectSystem.FleauEffectTag))
        {
          int fleauMalus = NwRandom.Roll(Utils.random, 4);
          attackBonus -= fleauMalus;
          appliedEffects.Add(EffectSystem.FleauEffectTag);
          LogUtils.LogMessage($"Fléau : -{fleauMalus} BA", LogUtils.LogType.Combat);
        }
        else if (eff.m_sCustomTag.CompareNoCase(EffectSystem.BenedictionEffectExoTag).ToBool() && !appliedEffects.Contains(EffectSystem.BenedictionEffectTag))
        {
          int beneBonus = NwRandom.Roll(Utils.random, 4);
          attackBonus += beneBonus;
          appliedEffects.Add(EffectSystem.FleauEffectTag);
          LogUtils.LogMessage($"Bénédiction : +{beneBonus} BA", LogUtils.LogType.Combat);
        }
        else if(eff.m_sCustomTag.CompareNoCase(EffectSystem.faveurDuMalinEffectExoTag).ToBool() && eff.GetInteger(5) == CustomSpell.FaveurDuMalinAttaque)
        {
          int faveurBonus = NwRandom.Roll(Utils.random, 10);
          attackBonus += faveurBonus;
          attacker.RemoveEffect(eff);
          LogUtils.LogMessage($"Faveur du malin attaque : +{faveurBonus}", LogUtils.LogType.Combat);
        }
      }

      foreach (var eff in target.m_appliedEffects)
      {
        if (eff.m_sCustomTag.CompareNoCase(EffectSystem.FrappeDechiranteEffectExoTag).ToBool() && eff.m_oidCreator != attacker.m_idSelf)
        {
          attackBonus += 5;
          target.RemoveEffect(eff);
          LogUtils.LogMessage("Frappe Déchirante : +5", LogUtils.LogType.Combat);
        }
        else if(eff.m_sCustomTag.CompareNoCase(EffectSystem.ProtectionContreLesLamesEffectExoTag).ToBool())
        {
          int bladeWardMalus = NwRandom.Roll(Utils.random, 4);
          attackBonus -= bladeWardMalus;
          LogUtils.LogMessage($"Protection contre les lames : -{bladeWardMalus}", LogUtils.LogType.Combat);
        }
      }
      
      return attackBonus;
    }
  }
}
