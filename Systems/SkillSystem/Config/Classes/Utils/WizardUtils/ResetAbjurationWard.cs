using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class WizardUtils
  {
    public static void ResetAbjurationWard(NwCreature creature)
    {
      if(creature.KnowsFeat((Feat)CustomSkill.AbjurationWard))
      {
        NwCreature target = creature.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").HasValue
        ? creature.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").Value : creature;

        target.OnDamaged -= OnDamageAbjurationWard;
        target.OnDamaged += OnDamageAbjurationWard;

        EffectUtils.RemoveTaggedEffect(target, creature, EffectSystem.AbjurationWardEffectTag);
        NWScript.AssignCommand(creature, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(creature.GetClassInfo(ClassType.Wizard).Level)));
      }
    }
  }
}
