using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VisionDiaboliqueEffectTag = "_VISION_DIABOLIQUE_EFFECT";
    public static Effect VisionDiabolique
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.SpellImmunity(Spell.Darkness), Effect.Ultravision());
        eff.ShowIcon = false;
        eff.Tag = VisionDiaboliqueEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
