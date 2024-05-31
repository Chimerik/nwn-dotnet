using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    public const string BotteDamageVariable = "_BOTTE_DAMAGE";
    public const string BotteDefensiveVariable = "_BOTTE_DEFENSIVE";
    public static readonly Native.API.CExoString BotteDamageExoVariable = BotteDamageVariable.ToExoString();
    private static void BotteDefensive(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      int bonus = EffectSystem.GetInspirationBardiqueBonus(caster.GetClassInfo(ClassType.Bard).Level);
      caster.GetObjectVariable<LocalVariableInt>(BotteDamageVariable).Value = bonus;
      caster.GetObjectVariable<LocalVariableInt>(BotteDefensiveVariable).Value = bonus;

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BardInspiration);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DefenseVaillante);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DegatsVaillants);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BotteDefensive);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BotteTranchante);

      caster.OnCreatureAttack -= BardUtils.OnAttackBotteDefensive;
      caster.OnCreatureAttack += BardUtils.OnAttackBotteDefensive;
    }
  }
}
