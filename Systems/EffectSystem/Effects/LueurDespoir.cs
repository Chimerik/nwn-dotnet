using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LueurDespoirEffectTag = "_LUEUR_DESPOIR_EFFECT";
    public static Effect LueurDespoir
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.LueurDespoir);
        eff.Tag = LueurDespoirEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.LueurDespoir);
        return eff;
      }
    }
  }
}
