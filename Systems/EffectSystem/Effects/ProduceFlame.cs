using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect produceFlameEffect
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurLightRed5);
        eff.Tag = ProduceFlameEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.ProduceFlame);
        return eff;
      }
    }
    public const string ProduceFlameEffectTag = "_PRODUCE_FLAME_EFFECT";
  }
}
