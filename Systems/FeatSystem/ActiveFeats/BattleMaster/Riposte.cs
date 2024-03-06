using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Riposte(NwCreature caster)
    {
      FeatUtils.ClearPreviousManoeuvre(caster);

      if (caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.NumDamageDice > 0)
      {
        if (!caster.GetItemInSlot(InventorySlot.RightHand).BaseItem.IsRangedWeapon)
        {
          int warMasterLevel = caster.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter)).Level;
          int superiorityDice = warMasterLevel > 17 ? 12 : warMasterLevel > 9 ? 10 : 8;

          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterRiposte;
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Value = superiorityDice;

          FeatUtils.DecrementManoeuvre(caster);
          StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Riposte", StringUtils.gold, true);
        }
        else
          caster?.LoginPlayer.SendServerMessage("Veuillez vous équiper d'une arme de mêlée", ColorConstants.Red);
      }
      else
        caster?.LoginPlayer.SendServerMessage("Veuillez vous équiper d'une arme", ColorConstants.Red);
    }
  }
}
