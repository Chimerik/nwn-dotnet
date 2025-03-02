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

        target.OnDeath -= EffectSystem.OnDeathAbjurationWard;

        if (target.IsLoginPlayerCharacter)
          target.LoginPlayer.OnClientDisconnect -= EffectSystem.OnLeaveAbjurationWard;
        
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDispel));
        EffectUtils.RemoveTaggedEffect(target, creature, EffectSystem.AbjurationWardEffectTag);

        creature.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").Delete();
        creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGlobeUse));
        
        NWScript.AssignCommand(creature, () => creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(creature.GetClassInfo(ClassType.Wizard).Level)));
      }
    }
  }
}
