using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ColdAffinityEffectTag = "_COLD_AFFINITY_EFFECT";
    public static Effect ColdAffinity
    {
      get
      {
        Effect eff = Effect.LinkEffects(ResistanceFroid);
        eff.Tag = ColdAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
