using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MoquerieVicieuseEffectTag = "_MOQUERIE_VICIEUSE_EFFECT";
    public static Effect MoquerieVicieuse
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.AttackDecrease), Effect.VisualEffect(VfxType.DurMindAffectingNegative));
        eff.Tag = MoquerieVicieuseEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.MoquerieVicieuse);
        return eff;
      } 
    }
  }
}

