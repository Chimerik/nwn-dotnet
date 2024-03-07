using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MaitreTacticqueTag = "_EFFECT_DODGE";
    public static readonly Native.API.CExoString MaitreTactiqueExoTag = MaitreTacticqueTag.ToExoString();
    public static Effect maitreTactique
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.AttackIncrease), Effect.VisualEffect(VfxType.DurCessatePositive));
        eff.Tag = MaitreTacticqueTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
