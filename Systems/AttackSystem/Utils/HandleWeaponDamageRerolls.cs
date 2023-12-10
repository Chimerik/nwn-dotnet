using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleWeaponDamageRerolls(CNWSCreature creature, NwBaseItem weapon,  int numDamageDice)
    {
      int damage = 0;

      for (int i = 0; i < numDamageDice; i++)
      {
        int roll = NwRandom.Roll(Utils.random, weapon.DieToRoll, 1);

        if (creature.m_pStats.HasFeat(CustomSkill.FighterCombatStyleTwoHanded).ToBool()
          && IsGreatWeaponStyle(weapon, creature))
          roll = roll < 3 ? NwRandom.Roll(Utils.random, weapon.DieToRoll) : roll;

        if (creature.m_pStats.HasFeat(CustomSkill.Empaleur).ToBool()
        && weapon.WeaponType.Any(d => d == Anvil.API.DamageType.Piercing))
          roll = roll < 3 ? NwRandom.Roll(Utils.random, weapon.DieToRoll) : roll;

        damage += roll;
      }

      return damage;
    }
  }
}
