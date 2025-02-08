using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EnduranceImplacableEffectTag = "_HALFORC_ENDURANCE_EFFECT";
    public const string EnduranceImplacableVariable = "_HALFORC_ENDURANCE";

    public static Effect enduranceImplacable(NwCreature creature)
    {
      creature.OnDamaged -= CreatureUtils.HandleImplacableEndurance;
      creature.OnDamaged += CreatureUtils.HandleImplacableEndurance;

      Effect eff = Effect.Icon(CustomEffectIcon.EnduranceImplacable);
      eff.Tag = EnduranceImplacableEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
