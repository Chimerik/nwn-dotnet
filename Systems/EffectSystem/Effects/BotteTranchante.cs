using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect BotteTranchante(NwCreature caster, bool botteDeMaitre = false)
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

      caster.OnCreatureAttack -= BardUtils.OnAttackBotteTranchante;
      caster.OnCreatureAttack += BardUtils.OnAttackBotteTranchante;

      Effect eff = Effect.Icon(EffectIcon.DamageIncrease);
      eff.Tag = BotteSecreteEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = bonus;

      return eff;
    }
  }
}
