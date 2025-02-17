using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LueurEtoileeEffectTag = "_LUEUR_ETOILEE_EFFECT";
    public static Effect LueurEtoilee
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurLightOrange10);
        eff.Tag = LueurEtoileeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.LueurEtoilee);
        return eff;
      }
    }
  }
}
