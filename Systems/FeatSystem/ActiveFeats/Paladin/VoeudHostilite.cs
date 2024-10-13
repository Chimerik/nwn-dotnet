using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void VoeudHostilite(NwCreature caster, NwGameObject targetObject)
    {
      if(caster == targetObject || targetObject is not NwCreature target || !caster.IsReactionTypeHostile(target))
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez choisir une cible valide", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.VoeuDHostilite, NwTimeSpan.FromRounds(10)));
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReduceAbilityScore));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} déclare son hostilité à {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);

      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
