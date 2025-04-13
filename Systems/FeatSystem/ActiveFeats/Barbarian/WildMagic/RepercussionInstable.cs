using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void RepercussionInstable(NwCreature caster)
    {
      var eff = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.RepercussionInstableEffectTag);
      
      if (eff is not null)
      {
        caster.OnDamaged -= EffectSystem.OnDamagedRepercussionInstable;
        caster.RemoveEffect(eff);
        caster.LoginPlayer?.SendServerMessage($"{StringUtils.ToWhitecolor("Répercussion Instable")} désactivé", ColorConstants.Orange);
        return;
      }

      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.RepercussionInstable(caster));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));
    }
  }
}
