using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AntidetectionEffectTag = "_ANTIDETECTION_EFFECT";
    public static Effect Antidetection
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.Antidetection);
        eff.Tag = AntidetectionEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.Antidetection);
        return eff;
      }
    }
  }
}
