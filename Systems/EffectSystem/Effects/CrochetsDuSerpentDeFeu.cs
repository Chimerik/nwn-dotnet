using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CrochetsDuSerpentDeFeuEffectTag = "_CROCHETS_DU_SERPENT_DE_FEU_EFFECT";
    public const string CrochetsDuSerpentDeFeuBisEffectTag = "_CROCHETS_DU_SERPENT_DE_FEU_BIS_EFFECT";
    public static Effect CrochetsDuSerpentDeFeu(NwCreature creature)
    {
      creature.OnCreatureAttack -= MonkUtils.OnAttackCrochetsDuSerpentDeFeu;
      creature.OnCreatureAttack += MonkUtils.OnAttackCrochetsDuSerpentDeFeu;

      Effect eff = Effect.DamageIncrease((int)DamageBonus.Plus1d10, DamageType.Fire);
      eff.Tag = CrochetsDuSerpentDeFeuEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    public static Effect CrochetsDuSerpentDeFeuBis(NwCreature creature)
    {
      creature.OnCreatureAttack -= MonkUtils.OnAttackCrochetsDuSerpentDeFeu;

      Effect eff = Effect.DamageIncrease((int)DamageBonus.Plus1d4, DamageType.Fire);
      eff.Tag = CrochetsDuSerpentDeFeuBisEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
