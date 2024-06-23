using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PuissanceInquisitriceEffectTag = "_PUISSANCE_INQUISITRICE_EFFECT";
    public static Effect GetPuissanceInquisitriceEffect(NwCreature target, int charismaModifier)
    {
      charismaModifier = charismaModifier > 1 ? charismaModifier : 1;
      target.OnCreatureAttack -= PaladinUtils.OnAttackPuissanceInquisitrice;
      target.OnCreatureAttack += PaladinUtils.OnAttackPuissanceInquisitrice;
      Effect eff = Effect.DamageIncrease(charismaModifier, DamageType.Divine);
      eff.Tag = PuissanceInquisitriceEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
