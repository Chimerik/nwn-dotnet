using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Stealth(NwCreature caster, OnUseFeat onUseFeat)
    {
      if(caster.GetActionMode(ActionMode.Stealth))
      {
        caster.SetActionMode(ActionMode.Stealth, false);
        onUseFeat.PreventFeatUse = true;
        return;
      }

      if (caster.Classes.Any(c => c.Class.ClassType == ClassType.Rogue && c.Level > 1)
        || caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.MonkTenebres))
        || caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.RangerDisparition)))
      {
        if (!CreatureUtils.HandleBonusActionUse(caster))
          return;

        caster.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").Value = 1;
        caster.SetActionMode(ActionMode.Stealth, true);
        onUseFeat.PreventFeatUse = true;
        return;
      }
    }
    private static void Stealth(NwCreature caster)
    {
      caster.SetActionMode(ActionMode.Stealth, true);
    }
  }
}
