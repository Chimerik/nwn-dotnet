using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PoisonAffinityEffectTag = "_POISON_AFFINITY_EFFECT";
    public static Effect PoisonAffinity
    {
      get
      {
        Effect eff = ResistancePoison;
        eff.Tag = PoisonAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
