using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AntidetectionEffectTag = "_ANTIDETECTION_EFFECT";
    public static Effect Antidetection(bool fromSpell)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.Antidetection);
      eff.Tag = AntidetectionEffectTag;

      if(fromSpell)
      {
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.Antidetection);
      }
      else
        eff.SubType = EffectSubType.Unyielding;

      return eff;
    }
  }
}
