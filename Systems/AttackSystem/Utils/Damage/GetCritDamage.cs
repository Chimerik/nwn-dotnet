using System;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCritDamage(CNWSCreature attacker, CNWSItem attackWeapon, CNWSCombatAttackData attackData, int sneakAttack, bool isDuelFightingStyle)
    {
      LogUtils.LogMessage($"Ajout des dégâts du coup critique", LogUtils.LogType.Combat);

      return attackWeapon is not null 
        ? GetWeaponCritDamage(attacker, attackWeapon, attackData, sneakAttack, isDuelFightingStyle) 
        : GetUnarmedCritDamage(attacker);
    }
    public static int GetWeaponCritDamage(CNWSCreature attacker, CNWSItem attackWeapon, CNWSCombatAttackData attackData, int sneakAttack, bool isDuelFightingStyle)
    {
      if (attackData.m_nAttackType == 6 && attacker.m_ScriptVars.GetInt(CreatureUtils.HastMasterSpecialAttackExo).ToBool())
      {
        attacker.m_ScriptVars.DestroyInt(CreatureUtils.HastMasterSpecialAttackExo);
        return NwRandom.Roll(Utils.random, 4, 1);
      }

      NwBaseItem baseWeapon = NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem);
      int damage = RollWeaponDamage(attacker, baseWeapon, attackData, true);
      damage += isDuelFightingStyle ? 2 : 0;

      if (sneakAttack > 0)
        damage += GetSneakAttackCritDamage(attacker);

      return damage;
    }
    public static int GetSneakAttackCritDamage(CNWSCreature attacker) // Hé oui, dans DD5, les dégâts des sournoises crit !
    {
      int sneakLevel = (int)Math.Ceiling((double)attacker.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Rogue) / 2);
      int sneakRoll = NwRandom.Roll(Utils.random, 6, sneakLevel);
      int damage = sneakRoll;

      BroadcastNativeServerMessage($"Sournoise Critique : {sneakLevel}d{6} = {sneakRoll}", attacker);
      LogUtils.LogMessage($"Critique - Sournoise - {sneakLevel}d{6} => {sneakRoll} - Total : {damage}", LogUtils.LogType.Combat);

      return damage;
    }
    public static int GetUnarmedCritDamage(CNWSCreature attacker)
    {
      int unarmedDieToRoll = CreatureUtils.GetUnarmedDamage(attacker.m_pStats);
      int damage = NwRandom.Roll(Utils.random, unarmedDieToRoll);

      LogUtils.LogMessage($"Critique - Mains nues - 1d{unarmedDieToRoll} => {damage}", LogUtils.LogType.Combat);

      return damage;
    }
  }
}
