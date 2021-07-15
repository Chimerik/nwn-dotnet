using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  static class HalfHealth
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      if (oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_HALF_HEALTH").HasNothing)
        oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_HALF_HEALTH").Value = oTarget.LevelInfo[0].HitDie;

      oTarget.LevelInfo[0].HitDie = (byte)(oTarget.LevelInfo[0].HitDie / 2);

      if (oTarget.HP > oTarget.MaxHP)
        oTarget.HP = oTarget.MaxHP;
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      if (oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_HALF_HEALTH").HasNothing)
        return;

      oTarget.LevelInfo[0].HitDie = (byte)oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_HALF_HEALTH").Value;
      oTarget.GetObjectVariable<LocalVariableInt>("CUSTOM_EFFECT_HALF_HEALTH").Delete();
    }
  }
}
