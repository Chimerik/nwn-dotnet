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

        roll = HandleRenforcement(creature, weapon, roll, dieToRoll);
        roll = HandleStabilisation(creature, weapon, roll, dieToRoll);

        if (creature.m_pStats.HasFeat(CustomSkill.FighterCombatStyleTwoHanded).ToBool()
          && IsGreatWeaponStyle(weapon, creature)
          && roll < 3)
        {
          int reroll = NwRandom.Roll(Utils.random, dieToRoll);
          LogUtils.LogMessage($"rolled {roll} - Great Weapon Style replace to 3", LogUtils.LogType.Combat);
          roll = 3;
        }
        else if (creature.m_pStats.HasFeat(CustomSkill.Empaleur).ToBool()
          && !creature.m_ScriptVars.GetInt(CreatureUtils.EmpaleurCooldownVariableExo).ToBool()
          && weapon.WeaponType.Any(d => d == Anvil.API.DamageType.Piercing)
          && roll < 3)
        {
          int reroll = NwRandom.Roll(Utils.random, dieToRoll);
          LogUtils.LogMessage($"rolled {roll} - Empaleur rerolled {reroll}", LogUtils.LogType.Combat);
          BroadcastNativeServerMessage("Empaleur".ColorString(StringUtils.gold), creature);
          roll = reroll;

          creature.m_ScriptVars.SetInt(CreatureUtils.EmpaleurCooldownVariableExo, 1);
        }

        damage += roll;
      }

      return damage;
    }
  }
}
