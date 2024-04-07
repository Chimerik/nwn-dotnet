using System.Linq;
using System.Security.Cryptography;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ProtectionProjetee(NwCreature caster, NwGameObject targetObject)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (targetObject is not NwCreature target || caster.IsReactionTypeHostile(target))
      {
        caster.LoginPlayer?.SendServerMessage($"Veuillez choisir une cible valide", ColorConstants.Red);
        return;
      }

      if (target.ActiveEffects.Any(e => e.Tag == EffectSystem.AbjurationWardEffectTag))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name} est déjà sous l'effet d'une protection arcanique", ColorConstants.Orange);
        return;
      }

      var ward = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.AbjurationWardEffectTag && e.Creator == caster);

      if(ward is null)
      {
        caster.LoginPlayer?.SendServerMessage("Vous ne disposez d'aucune protection arcanique active", ColorConstants.Red);
        return;
      }

      caster.OnDamaged -= WizardUtils.OnDamageAbjurationWard;

      EffectUtils.RemoveTaggedEffect(caster, EffectSystem.AbjurationWardEffectTag, caster);

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, 
        EffectSystem.GetAbjurationWardEffect(ward.CasterLevel)));
      
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGlobeUse));
      target.OnDamaged -= WizardUtils.OnDamageAbjurationWard;
      target.OnDamaged += WizardUtils.OnDamageAbjurationWard;
      caster.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").Value = target;

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Protection Arcanique".ColorString(ColorConstants.White)} sur {target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Orange, true);
    }
  }
}
