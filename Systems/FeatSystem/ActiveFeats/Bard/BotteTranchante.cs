using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    public const string BotteTranchanteVariable = "_BOTTE_TRANCHANTE";
    public static readonly Native.API.CExoString BotteTranchanteExoVariable = BotteTranchanteVariable.ToExoString();
 
    private static void BotteTranchante(NwCreature caster, int featId)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.GetObjectVariable<LocalVariableInt>(BotteTranchanteVariable).Value = 1;
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.BotteSecrete(caster, featId == CustomSkill.BotteTranchanteDeMaitre));
    }
  }
}
