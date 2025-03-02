using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void DispelConcentrationEffects(NwCreature caster)
    {
      EffectUtils.RemoveTaggedEffect(caster, EffectSystem.ConcentrationEffectTag);
    }
  }
}
