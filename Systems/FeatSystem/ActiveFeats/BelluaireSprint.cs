using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BelluaireSprint(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

        if (companion.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1)
        {
          _ = companion.ActionCastSpellAt((Spell)CustomSpell.Sprint, companion);
          return;
        }

        if (companion.KnowsFeat((Feat)CustomSkill.BelluaireDefenseDeLaBete))
        {
          EffectUtils.RemoveTaggedEffect(caster, EffectSystem.SprintEffectTag);

          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintEffect, NwTimeSpan.FromRounds(1));

          companion.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

          StringUtils.DisplayStringToAllPlayersNearTarget(companion, $"{companion.Name.ColorString(ColorConstants.Cyan)} sprinte", ColorConstants.Orange, true);
        }
        else
          _ = companion.ActionCastSpellAt((Spell)CustomSpell.Sprint, companion);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
    }
  }
}
