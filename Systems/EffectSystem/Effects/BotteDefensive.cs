using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BotteSecreteEffectTag = "_BOTTE_SECRETE_EFFECT";
    public const string BotteDefensiveEffectTag = "_BOTTE_DEFENSIVE_EFFECT";
    public static Effect BotteSecrete(NwCreature caster, bool botteDeMaitre = false)
    {
      int bonus;

      if (botteDeMaitre)
        bonus = Utils.Roll(6);
      else
      {
        bonus = GetInspirationBardiqueBonus(caster.GetClassInfo(ClassType.Bard).Level);

        caster.DecrementRemainingFeatUses((Feat)CustomSkill.BardInspiration);
        caster.DecrementRemainingFeatUses((Feat)CustomSkill.DefenseVaillante);
        caster.DecrementRemainingFeatUses((Feat)CustomSkill.DegatsVaillants);
        caster.DecrementRemainingFeatUses((Feat)CustomSkill.BotteDefensive);
        caster.DecrementRemainingFeatUses((Feat)CustomSkill.BotteTranchante);
      }

      caster.OnCreatureAttack -= BardUtils.OnAttackBotteDefensive;
      caster.OnCreatureAttack += BardUtils.OnAttackBotteDefensive;

      Effect eff = Effect.Icon(EffectIcon.DamageIncrease);
      eff.Tag = BotteSecreteEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = bonus;

      return eff;
    }
    public static Effect GetBotteDefensiveEffect(int bonus)
    {
      Effect eff = Effect.ACIncrease(bonus);
      eff.Tag = BotteDefensiveEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
