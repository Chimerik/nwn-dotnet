using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FourberieMagique(NwCreature caster)
    {
      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Non utilisable en combat", ColorConstants.Red);
        return;
      }

      CreatureClassInfo occultisteClass = caster.GetClassInfo((ClassType)CustomClass.Occultiste);

      if (occultisteClass is null)
        return;

      int occultisteLevel = occultisteClass.Level;
      var spellGainTable = NwClass.FromClassType((ClassType)CustomClass.Occultiste).SpellGainTable;
      double maxSpellSlots = spellGainTable[occultisteLevel - 1][1];
      byte remainingspellSlots = occultisteClass.GetRemainingSpellSlots(1);

      if (remainingspellSlots >= maxSpellSlots)
      {
        caster.LoginPlayer?.SendServerMessage("Aucun emplacement à restaurer", ColorConstants.Red);
        return;
      }

      byte restoredSpellSlots = (byte)(occultisteLevel > 19 ? maxSpellSlots : Math.Round(maxSpellSlots / 2, MidpointRounding.AwayFromZero));
      restoredSpellSlots = (byte)(remainingspellSlots + restoredSpellSlots > maxSpellSlots ? maxSpellSlots : remainingspellSlots + restoredSpellSlots);

      for (byte i = 1; i < 10; i++) 
        occultisteClass.SetRemainingSpellSlots(i, restoredSpellSlots);
      
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.OccultisteFourberieMagique);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpMagicalVision));

      if(caster.KnowsFeat((Feat)CustomSkill.FouleeRafraichissante))
      {
        byte remainingUses = caster.GetFeatRemainingUses((Feat)CustomSkill.FouleeRafraichissante);
        byte maxUses = (byte)(CreatureUtils.GetAbilityModifierMin1(caster, Ability.Charisma) + restoredSpellSlots);

        if (remainingUses + restoredSpellSlots > maxUses)
          restoredSpellSlots = maxUses;

        caster.IncrementRemainingFeatUses((Feat)CustomSkill.FouleeRafraichissante, restoredSpellSlots);
        caster.IncrementRemainingFeatUses((Feat)CustomSkill.FouleeProvocatrice, restoredSpellSlots);
        caster.IncrementRemainingFeatUses((Feat)CustomSkill.FouleeEvanescente, restoredSpellSlots);
        caster.IncrementRemainingFeatUses((Feat)CustomSkill.FouleeRedoutable, restoredSpellSlots);
      }

      OccultisteUtils.HandleResilienceCeleste(caster);

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Fourberie Magique", StringUtils.brightPurple, true, true);
    }
  }
}
