using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MaitreBouclier(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature targetCreature || caster == targetCreature)
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      NwItem shield = caster.GetItemInSlot(InventorySlot.LeftHand);

      if(shield is null)
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'un bouclier", ColorConstants.Red);
        return;
      }

      var isShieldEquipped = shield.BaseItem.ItemType switch
      {
        BaseItemType.SmallShield or BaseItemType.LargeShield or BaseItemType.TowerShield => true,
        _ => false,
      };

      if(!isShieldEquipped) 
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez vous équiper d'un bouclier", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (targetCreature.Size > caster.Size + 1)
      {
        caster.LoginPlayer?.SendServerMessage("La cible est trop grande pour que vous la renversiez !", ColorConstants.Red);
        return;
      }

      if(targetCreature.DistanceSquared(caster) > 9)
      {
        caster.LoginPlayer?.SendServerMessage("La cible est hors de portée", ColorConstants.Red);
        return;
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Maître bouclier", StringUtils.gold, true);
      EffectSystem.ApplyKnockdown(caster, targetCreature);
    }
  }
}
