using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImmuniteTranchantEffectTag = "_IMMUNITE_TRANCHANT_EFFECT";
    public static Effect ImmuniteTranchant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.SlashingImmunity);
        eff.Tag = ImmuniteTranchantEffectTag;
        return eff;
      }
    }
  }
}
