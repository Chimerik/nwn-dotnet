using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BelluaireDisengage(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

        if (companion.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1)
        {
          _ = companion.ActionCastSpellAt((Spell)CustomSpell.Disengage, companion);
          return;
        }

        if (companion.KnowsFeat((Feat)CustomSkill.BelluaireDefenseDeLaBete))
        {
          companion.ApplyEffect(EffectDuration.Temporary, EffectSystem.disengageEffect, NwTimeSpan.FromRounds(1));
          companion.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

          StringUtils.DisplayStringToAllPlayersNearTarget(companion, $"{companion.Name.ColorString(ColorConstants.Cyan)} se désengage", ColorConstants.Orange, true);
        }
        else
          _ = companion.ActionCastSpellAt((Spell)CustomSpell.Disengage, companion);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
    }
  }
}
