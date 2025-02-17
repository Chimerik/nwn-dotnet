using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EclairTracantEffectTag = "_ECLAIR_TRACANT_EFFECT";
    public static readonly Native.API.CExoString eclairTracantEffectExoTag = EclairTracantEffectTag.ToExoString();
    public static Effect EclairTracant
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACDecrease);
        eff.Tag = EclairTracantEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.EclairTracant);
        return eff;
      }
    }
  }
}

