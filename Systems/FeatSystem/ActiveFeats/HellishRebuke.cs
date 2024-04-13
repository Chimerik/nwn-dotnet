using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void HellishRebuke(NwCreature caster, NwGameObject targetObject)
    {
      if(targetObject is not NwCreature target || caster == target)
      {
        if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.HellishRebukeEffectTag))
        {
          caster.OnDamaged -= CreatureUtils.OnDamageHellishRebuke;
          EffectUtils.RemoveTaggedEffect(caster, EffectSystem.HellishRebukeEffectTag);
        }
        else
          caster.LoginPlayer?.SendServerMessage("Veuillez choisir une cible valide", ColorConstants.Red);

        return;
      }

      NWScript.AssignCommand(target, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.hellishRebukeEffect));
      caster.OnDamaged -=  CreatureUtils.OnDamageHellishRebuke;
      caster.OnDamaged += CreatureUtils.OnDamageHellishRebuke;
    }
  }
}
