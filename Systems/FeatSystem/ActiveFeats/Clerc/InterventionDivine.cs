using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void InterventionDivine(NwCreature caster)
    {
      var clerc = caster.GetClassInfo((ClassType)CustomClass.Clerc);

      if (clerc is null || clerc.Level < 1)
        return;

      bool success = true;

      if (clerc.Level < 20)
      {
        int roll = NwRandom.Roll(Utils.random, 100);

        if(clerc.Level > roll)
          success = false;

        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Intervention Divine - {roll} vs {clerc.Level} - {(success ? "Réussi".ColorString(StringUtils.brightGreen) : "Echec".ColorString(ColorConstants.Red))}", StringUtils.gold, true, true);
      }
      else
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Intervention Divine", StringUtils.gold, true, true);
      
      if (success)
        caster.GetObjectVariable<PersistentVariableString>("_DIVINE_INTERVENTION_COOLDOWN").Value = DateTime.Now.ToString(); 

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercInterventionDivine);
    }
  }
}
