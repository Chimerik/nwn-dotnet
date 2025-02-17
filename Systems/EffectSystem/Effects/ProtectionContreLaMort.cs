using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ProtectionContreLaMortEffectTag = "_PROTECTION_CONTRE_LA_MORT_EFFECT";
    public static readonly Native.API.CExoString ProtectionContreLaMortExoTag = ProtectionContreLaMortEffectTag.ToExoString();
    public static Effect ProtectionContreLaMort
    {
      get
      {
        Effect eff = Effect.Immunity(ImmunityType.Death);
        eff.Tag = ProtectionContreLaMortEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.ProtectionContreLaMort);
        return eff;
      }
    }
  }
}
