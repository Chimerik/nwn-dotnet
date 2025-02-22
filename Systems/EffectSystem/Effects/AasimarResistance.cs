using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AasimarResistanceEffectTag = "_AASIMAR_RESISTANCE_EFFECT";
    public static Effect AasimarResistance
    {
      get
      {
        Effect link = Effect.LinkEffects(ResistanceRadiant, ResistanceNecrotique);

        link.Tag = AasimarResistanceEffectTag;
        link.SubType = EffectSubType.Unyielding;

        return link;
      }
    }
  }
}
