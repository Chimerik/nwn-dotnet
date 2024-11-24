using System;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCritDamage(CNWSCreature attacker, CNWSItem attackWeapon, CNWSCombatAttackData attackData, int sneakAttack, bool isDuelFightingStyle, CNWSCreature target, Anvil.API.Ability damageAbility)
    {
      LogUtils.LogMessage("Ajout des dégâts du coup critique", LogUtils.LogType.Combat);

      return attackWeapon is null || attacker.m_ScriptVars.GetInt(CreatureUtils.MonkUnarmedDamageVariableExo).ToBool()
        ? GetUnarmedCritDamage(attacker)  
        : GetWeaponCritDamage(attacker, attackWeapon, attackData, sneakAttack, isDuelFightingStyle, target, damageAbility);
    }
    public static int GetWeaponCritDamage(CNWSCreature attacker, CNWSItem attackWeapon, CNWSCombatAttackData attackData, int sneakAttack, bool isDuelFightingStyle, CNWSCreature target, Anvil.API.Ability damageAbility)
    {
      if (attackData.m_nAttackType == 6 && attacker.m_ScriptVars.GetInt(CreatureUtils.HastMasterSpecialAttackExo).ToBool())
      {
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.HastMasterSpecialAttackExo);
        return NwRandom.Roll(Utils.random, 4, 1);
      }

      NwBaseItem baseWeapon = NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem);
      int damage = RollWeaponDamage(attacker, baseWeapon, attackData, target, attackWeapon, damageAbility, true);
      damage += isDuelFightingStyle ? 2 : 0;

      if (sneakAttack > 0)
        damage += GetSneakAttackCritDamage(attacker);

      return damage;
    }
    public static int GetSneakAttackCritDamage(CNWSCreature attacker) // Hé oui, dans DD5, les dégâts des sournoises crit !
    {
      int sneakLevel = (int)Math.Ceiling((double)RogueUtils.GetRogueLevel(attacker) / 2);
      int sneakRoll = NwRandom.Roll(Utils.random, 6, sneakLevel);
      int damage = sneakRoll;
      int assassinateBonus = IsAssassinate(attacker) ? attacker.m_pStats.GetNumLevelsOfClass(CustomClass.Rogue) : 0;

      BroadcastNativeServerMessage($"Sournoise Critique : {sneakLevel}d{6}{(assassinateBonus > 0 ? " +" + assassinateBonus : "")} = {sneakRoll + assassinateBonus}", attacker);
      LogUtils.LogMessage($"Critique - Sournoise - {sneakLevel}d{6}{(assassinateBonus > 0 ? " +" + assassinateBonus : "")} => {sneakRoll + assassinateBonus} - Total : {damage + assassinateBonus}", LogUtils.LogType.Combat);

      return damage + assassinateBonus;
    }
    public static int GetUnarmedCritDamage(CNWSCreature attacker)
    {
      int unarmedDieToRoll = CreatureUtils.GetUnarmedDamage(attacker);
      int damage = NwRandom.Roll(Utils.random, unarmedDieToRoll);

      LogUtils.LogMessage($"Critique - Mains nues - 1d{unarmedDieToRoll} => {damage}", LogUtils.LogType.Combat);

      return damage;
    }
  }
}
