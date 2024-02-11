using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleWeaponDamageRerolls(CNWSCreature creature, NwBaseItem weapon,  int numDamageDice, int dieToRoll)
    {
      int damage = 0;

      for (int i = 0; i < numDamageDice; i++)
      {
        int roll = NwRandom.Roll(Utils.random, dieToRoll, 1);

        if (creature.m_pStats.HasFeat(CustomSkill.FighterCombatStyleTwoHanded).ToBool()
          && IsGreatWeaponStyle(weapon, creature)
          && roll < 3)
        {
          int reroll = NwRandom.Roll(Utils.random, dieToRoll);
          LogUtils.LogMessage($"rolled {roll} - Great Weapon Style rerolled {reroll}", LogUtils.LogType.Combat);
          roll = reroll;
        }
        else if (creature.m_pStats.HasFeat(CustomSkill.Empaleur).ToBool()
        && weapon.WeaponType.Any(d => d == Anvil.API.DamageType.Piercing)
        && roll < 3)
        {
          int reroll = NwRandom.Roll(Utils.random, dieToRoll);
          LogUtils.LogMessage($"rolled {roll} - Empaleur rerolled {reroll}", LogUtils.LogType.Combat);
          roll = reroll;
        }

        damage += roll;
      }

      return damage;
    }
  }
}
