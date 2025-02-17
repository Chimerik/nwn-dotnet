using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ProtectionContreLeMalEtLeBienEffectTag = "_PROTECTION_CONTRE_LE_MAL_ET_LE_BIEN_EFFECT";
    public static Effect ProtectionContreLeMalEtLeBien
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurProtectionGoodMinor);
        eff.Tag = ProtectionContreLeMalEtLeBienEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.ProtectionContreLeMalEtLeBien);
        return eff;
      }
    }
  }
}
