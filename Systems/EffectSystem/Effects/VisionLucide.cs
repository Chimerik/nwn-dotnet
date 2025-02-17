using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VisionLucideEffectTag = "_VISION_LUCIDE_EFFECT";
    public static Effect VisionLucide
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Ultravision(), Effect.SeeInvisible(), Effect.Icon(EffectIcon.TrueSeeing));
        eff.Tag = VisionLucideEffectTag;
        eff.Spell = NwSpell.FromSpellType(Spell.TrueSeeing);
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

