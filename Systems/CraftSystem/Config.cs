using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class CollectSystem
  {
    private readonly static string LOOT_CONTAINER_ON_CLOSE_SCRIPT = "ls_load_onclose";
    private readonly static string ON_LOOT_SCRIPT = "ls_onloot";
    private readonly static string CHEST_AREA_TAG = "la_zone_des_loots";
    private readonly static string SQL_TABLE = "loot_containers";
    private readonly static string IS_LOOTED_VARNAME = "LS__IS_LOOTED";

    public enum Ore
    {
      Veldspar = 1,
      Scordite = 2,
      Pyroxeres = 3,
    }
  }
}
