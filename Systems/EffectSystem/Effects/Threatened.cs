using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect threatAoE;
    public static Effect threatenedEffect;

    public static void InitThreatenedEffect()
    {
      // TODO : en cas d'application de l'effet knockdown, penser à retirer l'effet et à le remettre une fois que le knockdown est dissipé
      threatAoE = Effect.AreaOfEffect(PersistentVfxType.PerGlyphOfWarding, scriptHandleFactory.CreateUniqueHandler(OnEnterThreatRange), null, scriptHandleFactory.CreateUniqueHandler(OnExitThreatRange));
      threatAoE.Tag = "_THREAT_RANGE";

      threatenedEffect = Effect.LinkEffects(Effect.RunAction(null, null, null), Effect.Icon(NwGameTables.EffectIconTable.GetRow(144)));
      threatenedEffect.Tag = StringUtils.threatenedEffectTag;
      threatenedEffect.SubType = EffectSubType.Supernatural;
    }
  }
}
