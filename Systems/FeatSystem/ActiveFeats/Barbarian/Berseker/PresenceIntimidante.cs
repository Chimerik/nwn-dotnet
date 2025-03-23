using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BersekerPresenceIntimidante(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(caster.Gender == Gender.Female ? VfxType.FnfHowlWarCryFemale : VfxType.FnfHowlWarCry));

      int spellDC = SpellUtils.GetCasterSpellDC(caster, Ability.Strength);

      foreach(var target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false))
      {
        if(caster.IsReactionTypeHostile(target) 
          && CreatureUtils.GetSavingThrow(caster, target, Ability.Wisdom, spellDC, effectType:SpellConfig.SpellEffectType.Fear) == SavingThrowResult.Failure)
        {
          EffectSystem.ApplyEffroi(target, caster, NwTimeSpan.FromRounds(10), spellDC, true);
        }
      }

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BersekerPresenceIntimidante);

      if (caster.GetFeatRemainingUses(Feat.BarbarianRage) > 0)
        caster.SetFeatRemainingUses((Feat)CustomSkill.BersekerRestorePresenceIntimidante, 1);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Présence Intimidante", StringUtils.gold, true, true);
    }
  }
}
