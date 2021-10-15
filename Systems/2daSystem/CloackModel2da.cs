using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class CloakModelTable : ITwoDimArray
  {
    public List<NuiComboEntry> cloakModelEntries = new List<NuiComboEntry>();

    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      int model = int.TryParse(twoDimEntry("MODEL"), out model) ? model : -1;

      if (model < 0)
        return;

      cloakModelEntries.Add(new NuiComboEntry(twoDimEntry("LABEL"), rowIndex));
    }
  }

  [ServiceBinding(typeof(CloakModel2da))]
  public class CloakModel2da
  {
    public static CloakModelTable cloakModelTable;
    public CloakModel2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      cloakModelTable = twoDimArrayFactory.Get2DA<CloakModelTable>("cloakmodel");
    }
  }
}
