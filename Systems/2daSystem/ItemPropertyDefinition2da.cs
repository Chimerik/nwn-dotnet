using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class ItemPropertyDefinitionEntry : ITwoDimArrayEntry
  {
    public string name { get; private set; }
    public string subTypeResRef { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      name = entry.GetStrRef("Name").ToString();
      subTypeResRef = entry.GetString("SubTypeResRef");
    }
  }

  [ServiceBinding(typeof(ItemPropertyDefinition2da))]
  public class ItemPropertyDefinition2da
  {
    public static readonly TwoDimArray<ItemPropertyDefinitionEntry> ipDefinitionTable = new("itempropdef.2da");
    public ItemPropertyDefinition2da()
    {
      
    }

    public static string GetSubTypeName(ItemPropertyType ipType, int subType)
    {
      /*PlayerSystem.Log.Info(ipType);
      PlayerSystem.Log.Info(subType);
      PlayerSystem.Log.Info(entries[ipType].subTypeResRef);
      PlayerSystem.Log.Info(NWScript.Get2DAString(entries[ipType].subTypeResRef, "Name", subType));*/
      return NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString(ipDefinitionTable[(int)ipType].subTypeResRef, "Name", subType)));
    }
  }
}
