using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string InterventionDivineEffectTag = "_INTERVENTION_DIVINE_EFFECT";
    public static Effect InterventionDivine
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.SpellLevelAbsorption);
        eff.Tag = InterventionDivineEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

