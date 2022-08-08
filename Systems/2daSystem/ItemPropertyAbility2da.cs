using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class ItemPropertyAbilityEntry : ITwoDimArrayEntry
  {
    public string name { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      name = entry.GetStrRef("Name").ToString();
    }
  }

  [ServiceBinding(typeof(ItemPropertyAbility2da))]
  public class ItemPropertyAbility2da
  {
    public static readonly TwoDimArray<ItemPropertyAbilityEntry> ipAbilityTable = NwGameTables.GetTable<ItemPropertyAbilityEntry>("iprp_abilities.2da");
    public ItemPropertyAbility2da()
    {

    }
  }
}
