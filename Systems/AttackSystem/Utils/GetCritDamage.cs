using System;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCritDamage(CNWSCreature attacker, CNWSItem attackWeapon, int bSneakAttack)
    {
      LogUtils.LogMessage($"Ajout des dégâts du coup critique", LogUtils.LogType.Combat);

      return attackWeapon is not null 
        ? GetWeaponCritDamage(attacker, attackWeapon, bSneakAttack) 
        : GetUnarmedCritDamage(attacker);
    }
    public static int GetWeaponCritDamage(CNWSCreature attacker, CNWSItem attackWeapon, int bSneakAttack)
    {
      NwBaseItem baseWeapon = NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem);
      int damage = NwRandom.Roll(Utils.random, baseWeapon.DieToRoll, baseWeapon.NumDamageDice);

      LogUtils.LogMessage($"{baseWeapon.Name.ToString()} - {baseWeapon.NumDamageDice}d{baseWeapon.DieToRoll} => {damage}", LogUtils.LogType.Combat);

      if (bSneakAttack > 0)
        damage += GetSneakAttackCritDamage(attacker);

      return damage;
    }
    public static int GetSneakAttackCritDamage(CNWSCreature attacker) // Hé oui, dans DD5, on reroll les dégâts des sournoises
    {
      int sneakLevel = (int)Math.Ceiling((double)attacker.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Rogue) / 2);
      int sneakRoll = NwRandom.Roll(Utils.random, 6, sneakLevel);
      int damage = sneakRoll;

      LogUtils.LogMessage($"Critique - Sournoise - {sneakLevel}d{6} => {sneakRoll} - Total : {damage}", LogUtils.LogType.Combat);

      return damage;
    }
    public static int GetUnarmedCritDamage(CNWSCreature attacker)
    {
      int unarmedDieToRoll = CreatureUtils.GetUnarmedDamage(attacker.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Monk));
      int damage = NwRandom.Roll(Utils.random, unarmedDieToRoll);

      LogUtils.LogMessage($"Critique - Mains nues - 1d{unarmedDieToRoll} => {damage}", LogUtils.LogType.Combat);

      return damage;
    }
  }
}
