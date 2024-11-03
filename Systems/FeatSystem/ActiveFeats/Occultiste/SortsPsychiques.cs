using System;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SortsPsychiques(NwCreature caster)
    {
      if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.SortsPsychiquesEffectTag))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.SortsPsychiquesEffectTag);
        caster.LoginPlayer?.SendServerMessage("Sorts Psychiques désactivés", StringUtils.brightPurple);
      }
      else
      {
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.SortsPsychiques));
        caster.LoginPlayer?.SendServerMessage("Sorts Psychiques activés", StringUtils.brightPurple);
      }
    }
  }
}
