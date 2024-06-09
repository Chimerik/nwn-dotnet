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
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGlobeUse));
        EffectUtils.RemoveTaggedEffect(target, creature, EffectSystem.AbjurationWardEffectTag);

        creature.OnDamaged -= OnDamageAbjurationWard;
        creature.OnDamaged += OnDamageAbjurationWard;
        creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGlobeUse));
        
        NWScript.AssignCommand(creature, () => creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(creature.GetClassInfo(ClassType.Wizard).Level)));
      }
    }
  }
}
