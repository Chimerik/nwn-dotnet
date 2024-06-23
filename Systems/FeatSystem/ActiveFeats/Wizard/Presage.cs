using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Presage(NwCreature caster, NwGameObject targetObject, NwFeat presage)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (targetObject is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage($"Veuillez choisir une cible valide", ColorConstants.Red);
        return;
      }

      var variableId = presage.Id switch
      {
        CustomSkill.DivinationPresage2 => CreatureUtils.Presage2Variable,
        CustomSkill.DivinationPresageSuperieur => CreatureUtils.Presage3Variable,
        _ => CreatureUtils.Presage1Variable,
      };

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.PresageVariable).Value = caster.GetObjectVariable<PersistentVariableInt>(variableId).Value;
      caster.GetObjectVariable<PersistentVariableInt>(variableId).Delete();
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Présage".ColorString(ColorConstants.White)} sur {target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Orange, true, true);

      caster.DecrementRemainingFeatUses(presage);
    }
  }
}
