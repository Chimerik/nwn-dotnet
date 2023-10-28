using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void DispelConcentrationEffects(NwCreature caster)
    {
      foreach (var eff in caster.ActiveEffects)
      {
        if (eff.Tag == EffectSystem.ConcentrationEffectTag)
          caster.RemoveEffect(eff);
      }
    }
  }
}
