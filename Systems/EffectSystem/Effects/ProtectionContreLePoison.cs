using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ProtectionContreLePoisonEffectTag = "_PROTECTION_CONTRE_LE_POISON_EFFECT";
    public static Effect ProtectionContreLePoison
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.ProtectionContreLePoison), ResistancePoison);
        eff.Tag = ProtectionContreLePoisonEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

