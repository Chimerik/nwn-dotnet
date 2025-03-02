using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class WizardUtils
  {
    public static void AbjurationSuperieure(NwCreature creature)
    {
      if(creature.KnowsFeat((Feat)CustomSkill.AbjurationImproved))
      {
        int casterLevel = creature.GetClassInfo(ClassType.Wizard).Level;
        int maxIntensity = 2 * casterLevel;
        NwCreature target = creature.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").HasValue 
          ? creature.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").Value : creature;

        int intensity = 0;

        foreach (var eff in target.ActiveEffects)
        {
          if (eff.Tag == EffectSystem.AbjurationWardEffectTag && eff.Creator == creature)
          {
            intensity = eff.CasterLevel;
            creature.RemoveEffect(eff);
          }
        }

        NWScript.AssignCommand(creature, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(casterLevel + intensity > maxIntensity ? maxIntensity : casterLevel + intensity)));

        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGlobeUse));
      }
    }
  }
}
