using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void InitializeNumAttackPerRound(NwCreature creature)
    {
      NwItem mainWeapon = creature.GetItemInSlot(InventorySlot.RightHand);
      NwItem secondaryWeapon = creature.GetItemInSlot(InventorySlot.LeftHand);

      creature.BaseAttackCount = 1;

      if (creature.KnowsFeat((Feat)CustomSkill.AttaqueSupplementaire))
        creature.BaseAttackCount += 1;
      else if (creature.KnowsFeat((Feat)CustomSkill.LameAssoiffee))
      {
        if ((mainWeapon is not null && mainWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(PacteDeLaLameVariable).Value == creature)
        || (secondaryWeapon is not null && secondaryWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(PacteDeLaLameVariable).Value == creature))
        {
          creature.BaseAttackCount += 1;

          creature.OnItemUnequip -= OccultisteUtils.OnUnequipPacteDeLaLame;
          creature.OnItemUnequip += OccultisteUtils.OnUnequipPacteDeLaLame;
        }
        else
        {
          creature.OnItemEquip -= OccultisteUtils.OnEquipPacteDeLaLame;
          creature.OnItemEquip += OccultisteUtils.OnEquipPacteDeLaLame;
        }
      }

      if (creature.KnowsFeat((Feat)CustomSkill.AttaqueSupplementaire2))
        creature.BaseAttackCount += 1;
      else if (creature.KnowsFeat((Feat)CustomSkill.LameDevorante))
        if ((mainWeapon is not null && mainWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(PacteDeLaLameVariable).Value == creature)
        || (secondaryWeapon is not null && secondaryWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(PacteDeLaLameVariable).Value == creature))
          creature.BaseAttackCount += 1;

      if (creature.KnowsFeat((Feat)CustomSkill.AttaqueSupplementaire3))
        creature.BaseAttackCount += 1;
    }
  }
}
