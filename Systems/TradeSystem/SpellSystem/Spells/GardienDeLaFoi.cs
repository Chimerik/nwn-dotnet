using System.Collections.Generic;
using System.Numerics;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void GardienDeLaFoi(NwCreature caster, Location targetLocation, int featId)
    {
      NwCreature guardian = NwCreature.Create("gardiendelafoi", targetLocation);
      guardian.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster3));
      guardian.AiLevel = AiLevel.VeryLow;
      CreaturePlugin.AddAssociate(caster, guardian, (int)AssociateType.Summoned);

      NWScript.AssignCommand(guardian, () => guardian.ApplyEffect(EffectDuration.Temporary, EffectSystem.GardienDeLaFoiAura, NwTimeSpan.FromRounds(4800)));

      guardian.GetObjectVariable<LocalVariableObject<NwCreature>>("_GUARDIAN_CASTER").Value = caster;
      guardian.OnEffectRemove += OnGuardianEffectRemove;
    }

    private static void OnGuardianEffectRemove(OnEffectRemove onRemove)
    {
      if(onRemove.Effect.Tag == EffectSystem.GardienDeLaFoiAuraEffectTag)
      {
        if(onRemove.Object is NwCreature guardian)
        {
          guardian.PlotFlag = false;
          guardian.Destroy();
        }
      }
    }
  }
}
