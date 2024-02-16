using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Feinte(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1)
      {
        caster.ControllingPlayer?.SendServerMessage("Vous ne disposez plus d'action bonus", ColorConstants.Red);
        return;
      }

      if (caster.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.NumDamageDice > 0)
      {
        int warMasterLevel = caster.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter)).Level;
        int superiorityDice = warMasterLevel > 9 ? warMasterLevel > 17 ? 10 : 12 : 8;

        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterFeinte;
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Value = superiorityDice;
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

        StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Feinte", StringUtils.gold, true);

        FeatUtils.DecrementManoeuvre(caster);
      }
      else
        caster?.LoginPlayer.SendServerMessage("Veuillez vous équiper d'une arme", ColorConstants.Red);
    }
  }
}
