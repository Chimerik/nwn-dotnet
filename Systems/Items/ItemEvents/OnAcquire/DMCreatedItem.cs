using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnAcquireDMCreatedItem(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      NwItem oItem = onAcquireItem.Item;

      if (onAcquireItem.AcquiredBy is null || onAcquireItem.AcquiredBy is not NwCreature oPC || oPC.ControllingPlayer is null 
        || oItem is null || oPC.LoginPlayer.IsDM || oItem.GetObjectVariable<LocalVariableString>("DM_ITEM_CREATED_BY").HasNothing)
        return;

      LogUtils.LogMessage($"{oPC.Name} vient d'acquérir {oItem.Name} créé par {oItem.GetObjectVariable<LocalVariableString>("DM_ITEM_CREATED_BY").Value}", LogUtils.LogType.DMAction);
    }
  }
}
