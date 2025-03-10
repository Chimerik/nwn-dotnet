using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void LueurDeGuérison(NwCreature caster, NwGameObject oTarget)
    {
      if (oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      byte remainingUses = caster.GetFeatRemainingUses((Feat)CustomSkill.LueurDeGuérison);
      int chaMod = caster.GetAbilityModifier(Ability.Charisma);
      int dicesUsed = chaMod > 1 ? chaMod > remainingUses ? remainingUses : chaMod : 1;

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.LueurDeGuérison, dicesUsed);
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Heal(SpellUtils.HandleHealerFeat(caster, 6, dicesUsed))));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Lueur de Guérison sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
    }
  }
}
