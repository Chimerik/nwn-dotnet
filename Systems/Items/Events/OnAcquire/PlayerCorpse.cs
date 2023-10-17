using Anvil.API.Events;
using Anvil.API;
using System.Linq;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnAcquirePlayerCorpse(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwGameObject oAcquiredFrom = onAcquireItem.AcquiredFrom;
      NwItem oItem = onAcquireItem.Item;

      if (onAcquireItem.AcquiredBy is null || onAcquireItem.AcquiredBy is not NwCreature oPC || oPC.ControllingPlayer is null || oItem is null)
        return;

      if (oItem.Tag == "item_pccorpse" && oAcquiredFrom?.Tag == "pccorpse_bodybag")
      {
        PlayerSystem.DeletePlayerCorpseFromDatabase(oItem.GetObjectVariable<LocalVariableInt>("_PC_ID").Value);

        NwObject.FindObjectsWithTag<NwCreature>("pccorpse").Where(c => c.GetObjectVariable<LocalVariableInt>("_PC_ID").Value == oItem.GetObjectVariable<LocalVariableInt>("_PC_ID")).FirstOrDefault()?.Destroy();
        oAcquiredFrom.Destroy();
      }
    }
    public static void OnUnacquirePlayerCorpse(ModuleEvents.OnUnacquireItem onUnacquireItem)
    {
      NwItem oItem = onUnacquireItem.Item;

      if (onUnacquireItem.LostBy.ControllingPlayer == null || oItem == null || oItem.Tag != "item_pccorpse" || oItem.Possessor is not null) // signifie que l'item a été drop au sol et pas donné à un autre PJ ou mis dans un placeable
        return;

      NwCreature oCorpse = NwCreature.Deserialize(oItem.GetObjectVariable<LocalVariableString>("_SERIALIZED_CORPSE").Value.ToByteArray());
      oCorpse.Location = oItem.Location;
      Utils.DestroyInventory(oCorpse);
      oCorpse.AcquireItem(oItem);
      oCorpse.VisibilityOverride = VisibilityMode.Hidden;
      PlayerSystem.SetupPCCorpse(oCorpse);

      PlayerSystem.SavePlayerCorpseToDatabase(oItem.GetObjectVariable<LocalVariableInt>("_PC_ID").Value, oCorpse);
    }
  }
}
