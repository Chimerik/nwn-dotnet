using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MaitreTactiqueTag = "_MAITRE_TACTIQUE_EFFECT";
    public static readonly Native.API.CExoString MaitreTactiqueExoTag = MaitreTactiqueTag.ToExoString();
    public static Effect MaitreTactique
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.AttackIncrease));
        eff.Tag = MaitreTactiqueTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
