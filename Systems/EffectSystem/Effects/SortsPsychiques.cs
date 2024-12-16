using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SortsPsychiquesEffectTag = "_SORTS_PSYCHIQUES_EFFECT";
    public static Effect SortsPsychiques
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.SortsPsychiques);
        eff.Tag = SortsPsychiquesEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}

