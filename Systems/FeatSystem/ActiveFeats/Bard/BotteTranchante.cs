using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    public const string BotteTranchanteVariable = "_BOTTE_TRANCHANTE";
    public static readonly Native.API.CExoString BotteTranchanteExoVariable = BotteTranchanteVariable.ToExoString();
 
    private static void BotteTranchante(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.GetObjectVariable<LocalVariableInt>(BotteTranchanteVariable).Value = 1;
      caster.GetObjectVariable<LocalVariableInt>(BotteDamageVariable).Value = EffectSystem.GetInspirationBardiqueBonus(caster.GetClassInfo(ClassType.Bard).Level);

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BardInspiration);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DefenseVaillante);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DegatsVaillants);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BotteDefensive);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BotteTranchante);
    }
  }
}
