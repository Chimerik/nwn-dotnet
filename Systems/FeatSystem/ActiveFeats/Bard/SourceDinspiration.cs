using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SourceDinspiration(NwCreature caster)
    {
      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Non utilisable en combat", ColorConstants.Red);
        return;
      }

      CreatureClassInfo bardeClass = caster.GetClassInfo(ClassType.Bard);

      if (bardeClass is null)
        return;

      int currentInspi = caster.GetFeatRemainingUses((Feat)CustomSkill.BardInspiration);

      if(currentInspi < caster.GetAbilityModifier(Ability.Charisma))
      {
        int bardeLevel = bardeClass.Level;

        for (byte i = 1; i < 9; i++)
        {
          byte remainingspellSlots = bardeClass.GetRemainingSpellSlots(i);

          if (remainingspellSlots > 0)
          {
            bardeClass.SetRemainingSpellSlots(i, (byte)(remainingspellSlots - 1));
            caster.SetFeatRemainingUses((Feat)CustomSkill.BardInspiration, (byte)(currentInspi + 1));

            caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadSonic));
            StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Source d'Inspiration", StringUtils.gold, true, true);
            return;
          }
        }

        caster.LoginPlayer?.SendServerMessage("Aucune emplacement de sort disponible", ColorConstants.Red);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Aucune inspiration à restaurer", ColorConstants.Red);
    }
  }
}
