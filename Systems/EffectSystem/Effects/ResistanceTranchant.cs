using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceTranchantEffectTag = "_RESISTANCE_TRANCHANT_EFFECT";
    public static Effect ResistanceTranchant
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.SlashingResistance);
        eff.Tag = ResistanceTranchantEffectTag;
        return eff;
      }
    }
  }
}
