using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DesertionEffectTag = "_DESERTION_EFFECT";
    public static Effect Desertion
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurEtherealVisage), ResistanceNecrotique, ResistancePsychique,
          ResistanceAcide, ResistanceFeu, ResistanceTonnerre, ResistanceFroid, ResistanceElec, ResistanceContondant,
          ResistanceRadiant, ResistancePercant, ResistancePercant);
        
        eff.Tag = DesertionEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
