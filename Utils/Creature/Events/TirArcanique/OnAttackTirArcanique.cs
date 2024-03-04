using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnAttackTirArcanique(OnCreatureAttack onAttack)
    {
      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (!ItemUtils.HasBowEquipped((BaseItemType)onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType))
            return;

          int tirId = onAttack.Attacker.GetObjectVariable<LocalVariableInt>(TirArcaniqueVariable).Value;

          switch (tirId)
          {
            case CustomSkill.ArcaneArcherTirAffaiblissant: HandleTirAffaiblissant(onAttack); break;
            case CustomSkill.ArcaneArcherTirAgrippant: HandleTirAgrippant(onAttack); break;
            case CustomSkill.ArcaneArcherTirBannissement: HandleTirBannissement(onAttack); break;
            case CustomSkill.ArcaneArcherTirEnvoutant: HandleTirEnvoutant(onAttack); break;
            case CustomSkill.ArcaneArcherTirExplosif: HandleTirExplosif(onAttack); break;
            case CustomSkill.ArcaneArcherTirOmbres: HandleTirDesOmbres(onAttack); break;
          }

          break;
      }
    }
  }
}
