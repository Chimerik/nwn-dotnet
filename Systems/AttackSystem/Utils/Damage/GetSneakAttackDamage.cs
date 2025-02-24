using System;
using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetSneakAttackDamage(CNWSCreature attacker, CNWSCreature target, CNWSItem attackWeapon, CNWSCombatAttackData attackData, CNWSCombatRound round)
    {
      if (target is null) // L'attaque sournoise ne peut toucher que des créatures (les objets sont exclus)
        return 0;

      int sneakLevel = (int)Math.Ceiling((double)RogueUtils.GetRogueLevel(attacker) / 2);

      // Limitation à une sournoise par round
      if (sneakLevel < 1 || attacker.m_ScriptVars.GetInt(CreatureUtils.SneakAttackCooldownVariableExo).ToBool()) 
        return 0;

      // Nécessite une arme à distance ou une arme de finesse
      if(!attackData.m_bRangedAttack.ToBool() && !attackWeapon.m_ScriptVars.GetInt(ItemConfig.isFinesseWeaponCExoVariable).ToBool())
        return 0;

      int advantage = attacker.m_ScriptVars.GetInt($"_ADVANTAGE_ATTACK_{round.m_nCurrentAttack}".ToExoString());

      // Pas de sournoise si désavantage
      if (advantage < 0)
        return 0;

      // Si pas d'avantage, sournoise possible si un autre ennemi de la cible se trouve à portée de mêlée
      if (advantage == 0)
      {
        var distraction = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(target.GetNearestEnemy(3, attacker.m_idSelf, 1, 1));

        if (distraction is null || distraction.m_idSelf == 0x7F000000) // OBJECT_INVALID
          return 0;
      }

      foreach(var eff in attacker.m_appliedEffects.Where(e => e.m_sCustomTag.CompareNoCase(EffectSystem.FrappeRuseeEffectExoTag).ToBool()))
      {
        sneakLevel -= eff.m_nSpellId switch
        {
          CustomSpell.FrappePerfideHebeter => 2,
          CustomSpell.FrappePerfideObscurcir => 3,
          CustomSpell.FrappePerfideAssommer => 6,
          _ => 1,
        };  

        attacker.m_ScriptVars.SetString(EffectSystem.FrappeRuseeVariableExo,
          attacker.m_ScriptVars.GetString(EffectSystem.FrappeRuseeVariableExo).GetLength() < 1 
          ? $"{eff.m_nSpellId}".ToExoString()
          : $"{attacker.m_ScriptVars.GetString(EffectSystem.FrappeRuseeVariableExo)}_{eff.m_nSpellId}".ToExoString());

        if (sneakLevel < 1)
          break;
      }

      int sneakRoll = NwRandom.Roll(Utils.random, 6, sneakLevel);
      int assassinateBonus = IsAssassinate(attacker) ? attacker.m_pStats.GetNumLevelsOfClass(CustomClass.Rogue) : 0;
      attacker.m_ScriptVars.SetInt(CreatureUtils.SneakAttackCooldownVariableExo, 1);
      attacker.m_ScriptVars.DestroyInt($"_ADVANTAGE_ATTACK_{round.m_nCurrentAttack}".ToExoString());

      BroadcastNativeServerMessage($"Sournoise {sneakLevel}d{6}{(assassinateBonus > 0 ? " +" + assassinateBonus : "") } => {sneakRoll + assassinateBonus}", attacker);
      LogUtils.LogMessage($"Sournoise - {sneakLevel}d{6}{(assassinateBonus > 0 ? " +" + assassinateBonus : "")} => {sneakRoll + assassinateBonus} - Total : {sneakRoll + assassinateBonus}", LogUtils.LogType.Combat);

      return sneakRoll + assassinateBonus;
    }
  }
}
