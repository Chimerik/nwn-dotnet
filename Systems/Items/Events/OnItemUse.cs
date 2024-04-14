using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static async void OnItemUse(OnItemUse onUse)
    {
      NwCreature oPC = onUse.UsedBy;
      NwItem oItem = onUse.Item;
      NwGameObject oTarget = onUse.TargetObject;

      if (oPC is null || oItem is null)
        return;

      switch (oItem.Tag)
      {
        case "private_contract": HandlePrivateContract(onUse); break;
        case "sequence_register": HandleCastSequence(onUse); break;
        case "bank_contract": HandleBankContract(onUse); break;
        //case "potion_core_influx": Potion.HandleCoreInflux(oPC, oItem); break;
        case "potion_cure_mini": Potion.CureMini(oPC.ControllingPlayer); break;
        case "potion_cure_frog": Potion.CureFrog(oPC.ControllingPlayer); break;
        case "potion_alchimique": Potion.AlchemyEffect(onUse); break;
        case "PierredeTransmutation": TransmutationStone(onUse); break;
      }

      if (oTarget is not null)
      {
        switch (oTarget.Tag)
        {
          // Utilisation en mode extraction
          case "mineable_materia": HandleOnTargetMineableMateria(onUse); break;
          case "forge":
          case "scierie":
          case "tannerie": HandleOnTargetCraftWorkshop(onUse); break;
        }
      }
      else // Utilisation en mode scanner ?
        HandleMateriaScanning(onUse);

      await NwTask.Delay(TimeSpan.FromSeconds(0.2));
      feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.UseItemCantUse, oPC.ControllingPlayer);
      feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.UseItemNotEquipped, oPC.ControllingPlayer);
    }
  }
}
