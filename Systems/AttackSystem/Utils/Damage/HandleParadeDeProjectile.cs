using Anvil.API;
using NWN.Native.API;
using ClassType = NWN.Native.API.ClassType;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleParadeDeProjectile(CNWSCreature attacker, CNWSCreature target, bool isRangedAttack)
    {
      int monkLevel = GetClassLevel(target, ClassType.Monk);

      if (isRangedAttack 
        && monkLevel > 2
        && !target.m_ScriptVars.GetInt(CreatureUtils.ParadeDeProjectileCooldownVariableExo).ToBool())
      {
        target.m_ScriptVars.SetInt(CreatureUtils.ParadeDeProjectileCooldownVariableExo, 1);

        int roll = NwRandom.Roll(Utils.random, 10);
        int dexMod = target.m_pStats.GetDEXMod(1);
        int dexBonus = dexMod > 122 ? dexMod - 255 : dexMod;
        int damageReduction = roll + dexBonus + monkLevel;

        LogUtils.LogMessage($"Parade de projectile - Réduction de dégâts : {roll} (1d10) + {dexBonus} + {monkLevel} = {damageReduction}", LogUtils.LogType.Combat);
        BroadcastNativeServerMessage($"Parade de projectile - Réduction de dégâts : {roll} (1d10) + {dexBonus} + {monkLevel} = {damageReduction}", target, true);
        return damageReduction;
      }

      return 0;
    }
  }
}
