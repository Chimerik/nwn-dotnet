using System.Collections.Generic;
using System.Numerics;
using Anvil.API;
using NWN.Core;
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

      if(attacker.m_pStats.m_pBaseCreature.m_ScriptVars.GetInt(CreatureUtils.FrappeGuideeVariableExo).ToBool())
      {
        attackBonus += 10;
        attacker.m_pStats.m_pBaseCreature.m_ScriptVars.DestroyInt(CreatureUtils.FrappeGuideeVariableExo);
        LogUtils.LogMessage("Frappe Guidée : +10 BA", LogUtils.LogType.Combat);
      }

      var initiaLocation = attacker.m_pStats.m_pBaseCreature.m_ScriptVars.GetLocation(EffectSystem.chargerVariableExo);

      if (initiaLocation.m_oArea != NWScript.OBJECT_INVALID && Vector3.DistanceSquared(initiaLocation.m_vPosition.ToManagedVector(), attacker.m_vPosition.ToManagedVector()) > 9)
      {
        attackBonus += 5;

        foreach (var eff in attacker.m_appliedEffects)
          if (eff.m_sCustomTag.CompareNoCase(EffectSystem.chargeurEffectExoTag).ToBool())
            attacker.RemoveEffect(eff);

        BroadcastNativeServerMessage("Charge (+5 BA)".ColorString(ColorConstants.Orange), attacker);
        LogUtils.LogMessage("Chargeur : +5 BA", LogUtils.LogType.Combat);
      }

      attacker.m_pStats.m_pBaseCreature.m_ScriptVars.DestroyLocation(EffectSystem.chargerVariableExo);

      if (weapon is not null)
      {
        if (IsCogneurLourd(attacker, weapon))
        {
          attackBonus -= 5;
          LogUtils.LogMessage("Cogneur Lourd : -5 BA", LogUtils.LogType.Combat);
        }
        else if (IsTireurDelite(attacker, attackData, weapon))
        {
          attackBonus -= 5;
          LogUtils.LogMessage("Tireur d'élite : -5 BA", LogUtils.LogType.Combat);
        }
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
      }
      
      return attackBonus;
    }
  }
}
