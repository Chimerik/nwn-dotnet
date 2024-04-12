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
        if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.HellishRebukeSourceEffectTag))
        {
          if (caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.HellishRebukeSourceEffectTag).Creator is NwCreature previousTarget && previousTarget.IsValid)
          {
            previousTarget.OnCreatureDamage -= CreatureUtils.OnDamageHellishRebuke;
            EffectUtils.RemoveTaggedEffect(previousTarget, EffectSystem.HellishRebukeTargetTag, caster);
          }

          EffectUtils.RemoveTaggedEffect(caster, EffectSystem.HellishRebukeSourceEffectTag);
        }
        else
          caster.LoginPlayer?.SendServerMessage("Veuillez choisir une cible valide", ColorConstants.Red);

        return;
      }

      NWScript.AssignCommand(target, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.hellishRebukeSourceEffect));
      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.hellishRebukeTargetEffect));
      target.OnCreatureDamage -=  CreatureUtils.OnDamageHellishRebuke;
      target.OnCreatureDamage += CreatureUtils.OnDamageHellishRebuke;
    }
  }
}
