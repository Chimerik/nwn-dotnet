using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void TirIncurve(NwCreature caster)
    {
      if (ItemUtils.HasBowEquipped(caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType))
      {
        if(caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value > 0) 
        {
          caster.LoginPlayer?.EnterTargetMode(SelectTirIncurveTarget, Config.attackCreatureTargetMode);
          caster.LoginPlayer?.SendServerMessage("Tir Incurvé - Veuillez choisir une cible", ColorConstants.Orange);
        }
        else
          caster.LoginPlayer?.SendServerMessage("Vous n'avez plus d'action bonus disponible", ColorConstants.Red);
      }
      else
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'un arc", ColorConstants.Red);
    }
    private static void SelectTirIncurveTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is not NwCreature creature || creature is null || !creature.IsValid)
        return;

      if(creature.DistanceSquared(selection.Player.ControlledCreature) > 325)
      {
        selection.Player.SendServerMessage($"{StringUtils.ToWhitecolor(creature.Name)} n'est pas à portée", ColorConstants.Red);
        return;
      }

      selection.Player.LoginCreature.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.TirIncurveVariable).Value = creature;
      selection.Player.LoginCreature.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;
      CreatureUtils.HandleBonusActionCooldown(selection.Player.LoginCreature);

      StringUtils.DisplayStringToAllPlayersNearTarget(selection.Player.ControlledCreature, "Tir incurvé", StringUtils.gold, true);
    }
  }
}
