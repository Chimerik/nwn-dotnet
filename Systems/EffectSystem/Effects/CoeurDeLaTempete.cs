using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CoeurDeLaTempeteEffectTag = "_COEUR_DE_LA_TEMPETE_EFFECT";
    public static Effect CoeurDeLaTempete
    {
      get
      {
        Effect eff = Effect.LinkEffects(ResistanceElec, ResistanceTonnerre);
        eff.Tag = CoeurDeLaTempeteEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
