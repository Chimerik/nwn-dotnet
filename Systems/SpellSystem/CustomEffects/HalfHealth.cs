using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class HalfHealth
  {
    public HalfHealth(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
    {
      if (oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_HALF_HEALTH").HasNothing)
        oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_HALF_HEALTH").Value = CreaturePlugin.GetMaxHitPointsByLevel(oTarget, 1);

      CreaturePlugin.SetMaxHitPointsByLevel(oTarget, 1, CreaturePlugin.GetMaxHitPointsByLevel(oTarget, 1) / 2);

      if (oTarget.HP > oTarget.MaxHP)
        oTarget.HP = oTarget.MaxHP;
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      if (oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_HALF_HEALTH").HasNothing)
        return;

      CreaturePlugin.SetMaxHitPointsByLevel(oTarget, 1, oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_HALF_HEALTH").Value);
      oTarget.GetLocalVariable<int>("CUSTOM_EFFECT_HALF_HEALTH").Delete();
    }
    private void NoMagicMalus(OnSpellCast onSpellCast)
    {
      onSpellCast.PreventSpellCast = true;
      ((NwPlayer)onSpellCast.Caster).SendServerMessage("L'interdiction d'usage de sorts est en vigueur.", Color.RED);
    }
  }
}
