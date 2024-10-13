using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void PuissanceInquisitrice(NwCreature caster, NwGameObject targetObject)
    {
      if(caster == targetObject || targetObject is not NwCreature target || !caster.IsReactionTypeHostile(target))
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez choisir une cible valide", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetPuissanceInquisitriceEffect(target, caster.GetAbilityModifier(Ability.Charisma)), NwTimeSpan.FromRounds(2)));
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} : Puissance Inquisitrice", StringUtils.gold, true, true);

      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
