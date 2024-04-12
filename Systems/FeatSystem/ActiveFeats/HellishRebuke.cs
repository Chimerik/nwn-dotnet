using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void HellishRebuke(NwCreature caster, NwGameObject targetObject)
    {
      if(targetObject is not NwCreature target || caster == target)
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez choisir une cible valide", ColorConstants.Red);
        return;
      }

      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.HellishRebukeEffectTag))
      {
        foreach (var effect in caster.ActiveEffects)
          if (effect.Tag == EffectSystem.HellishRebukeEffectTag)
          {
            caster.RemoveEffect(effect);
            caster.OnDamaged -= CreatureUtils.OnDamageHellishRebuke;
          }
      }
      else
      {
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.hellishRebukeEffect);
        caster.OnDamaged -= CreatureUtils.OnDamageHellishRebuke;
        caster.OnDamaged += CreatureUtils.OnDamageHellishRebuke;
        caster.GetObjectVariable<LocalVariableObject<NwCreature>>("_HELLISH_REBUKE_TARGET").Value = target;
      }
    }
  }
}
