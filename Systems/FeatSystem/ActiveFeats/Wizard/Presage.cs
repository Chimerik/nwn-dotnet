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

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.PresageVariable).Value = int.Parse(presage.Name.ToString().Replace("Présage : ", ""));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Présage".ColorString(ColorConstants.White)} sur {target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Orange, true);
    }
  }
}
