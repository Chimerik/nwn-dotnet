using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistancePoisonEffectTag = "_RESISTANCE_POISON_EFFECT";
    public static Effect ResistancePoison
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PoisonResistance);
        eff.Tag = ResistancePoisonEffectTag;
        return eff;
      }
    }
  }
}
