using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandlePourfendeurDeColosse(CNWSCreature attacker, CNWSCreature target, CNWSItem weapon)
    {
      if (!attacker.m_pStats.HasFeat(CustomSkill.ChasseurPourfendeurDeColosses).ToBool() || weapon is null
        || attacker.m_ScriptVars.GetInt(CreatureUtils.PourfendeurDeColosseVariableExo).ToBool()
        || target.GetMaxHitPoints(1) <= target.GetCurrentHitPoints(0))
        return 0;

      int bonusDamage = NwRandom.Roll(Utils.random, 8);

      LogUtils.LogMessage($"Pourfendeur de Colosses : +{bonusDamage}", LogUtils.LogType.Combat);

      return bonusDamage;
    }
  }
}
