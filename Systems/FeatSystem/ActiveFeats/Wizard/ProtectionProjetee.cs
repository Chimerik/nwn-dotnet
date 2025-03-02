using System.Linq;
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

      if (targetObject is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage($"Veuillez choisir une cible valide", ColorConstants.Red);
        return;
      }

      if (target.ActiveEffects.Any(e => e.Tag == EffectSystem.AbjurationWardEffectTag))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name} est déjà sous l'effet d'une protection arcanique", ColorConstants.Orange);
        return;
      }

      NwCreature currentWardBearer = caster.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").HasValue
       ? caster.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").Value : caster;

      var ward = currentWardBearer.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.AbjurationWardEffectTag && e.Creator == caster);
      int intensity = ward is not null ? ward.CasterLevel : caster.GetClassInfo(ClassType.Wizard).Level;

      EffectUtils.RemoveTaggedEffect(caster, caster, EffectSystem.AbjurationWardEffectTag);

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(intensity)));

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGlobeUse));
      target.OnDeath -= EffectSystem.OnDeathAbjurationWard;
      target.OnDeath += EffectSystem.OnDeathAbjurationWard;

      if (target.IsLoginPlayerCharacter)
      {
        target.LoginPlayer.OnClientDisconnect -= EffectSystem.OnLeaveAbjurationWard;
        target.LoginPlayer.OnClientDisconnect += EffectSystem.OnLeaveAbjurationWard;
      }

      caster.GetObjectVariable<LocalVariableObject<NwCreature>>("_ABJURATION_WARD_TARGET").Value = target;

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Protection Arcanique".ColorString(ColorConstants.White)} sur {target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Orange, true, true);
    }
  }
}
