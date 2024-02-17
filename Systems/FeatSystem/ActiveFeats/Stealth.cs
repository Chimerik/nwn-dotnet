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

      if (caster.Classes.Any(c => c.Class.ClassType == ClassType.Rogue && c.Level > 1))
      {
        if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value > 0)
        {
          caster.GetObjectVariable<LocalVariableInt>("_STEALTH_AUTHORIZED").Value = 1;
          caster.SetActionMode(ActionMode.Stealth, true);
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;
          onUseFeat.PreventFeatUse = true;
          return;
        }
        else
          caster.LoginPlayer?.SendServerMessage("Aucune action bonus disponible", ColorConstants.Orange);
      }
    }
    private static void Stealth(NwCreature caster)
    {
      caster.SetActionMode(ActionMode.Stealth, true);
    }
  }
}
