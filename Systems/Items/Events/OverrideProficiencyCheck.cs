using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OverrideProficiencyCheck(OnCreatureCheckProficiencies onCheck)
    {
      if (onCheck.Item != null && onCheck.Item.BaseItem != null && onCheck.Item.BaseItem.EquipmentSlots != EquipmentSlots.None)
        onCheck.ResultOverride = CheckProficiencyOverride.HasProficiency;
    }
  }
}
