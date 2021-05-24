using NWN.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  static class HalfHealth
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      if (oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_HALF_HEALTH").HasNothing)
        oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_HALF_HEALTH").Value = CreaturePlugin.GetMaxHitPointsByLevel(oTarget, 1);

      CreaturePlugin.SetMaxHitPointsByLevel(oTarget, 1, CreaturePlugin.GetMaxHitPointsByLevel(oTarget, 1) / 2);

      if (oTarget.HP > oTarget.MaxHP)
        oTarget.HP = oTarget.MaxHP;
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      if (oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_HALF_HEALTH").HasNothing)
        return;

      CreaturePlugin.SetMaxHitPointsByLevel(oTarget, 1, oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_HALF_HEALTH").Value);
      oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_HALF_HEALTH").Delete();
    }
  }
}
