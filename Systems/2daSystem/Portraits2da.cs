using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class PortraitEntry : ITwoDimArrayEntry
  {
    public string resRef { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      resRef = entry.GetString("BaseResRef");
    }
  }

  [ServiceBinding(typeof(Portraits2da))]
  public class Portraits2da
  {
    public static readonly TwoDimArray<PortraitEntry> portraitsTable = new("portraits.2da");
    public Portraits2da()
    {

    }
  }
}
