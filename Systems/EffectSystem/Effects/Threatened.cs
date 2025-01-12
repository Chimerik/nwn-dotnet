using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    // TODO : en cas d'application de l'effet knockdown, penser à retirer l'effet et à le remettre une fois que le knockdown est dissipé
    public static Effect threatAoE
    {
      get
      {
        Effect eff = Effect.AreaOfEffect(PersistentVfxType.PerGlyphOfWarding, scriptHandleFactory.CreateUniqueHandler(OnEnterThreatRange), null, scriptHandleFactory.CreateUniqueHandler(OnExitThreatRange));
        eff.Tag = ThreatenedAoETag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect threatenedEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(NwGameTables.EffectIconTable.GetRow(144)));
        eff.Tag = ThreatenedEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public const string ThreatenedAoETag = "_THREAT_RANGE";
    public const string ThreatenedEffectTag = "_THREATENED_EFFECT";
  }
}
