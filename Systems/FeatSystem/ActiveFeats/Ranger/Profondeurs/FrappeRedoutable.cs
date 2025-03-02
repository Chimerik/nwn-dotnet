using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappeRedoutable(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.FrappeRedoutableEffectTag))
      {
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FrappeRedoutable(caster.GetFeatRemainingUses((Feat)CustomSkill.ProfondeursFrappeRedoutable)));
        caster.SetFeatRemainingUses((Feat)CustomSkill.ProfondeursFrappeRedoutable, 0);
      }
    }
  }
}
