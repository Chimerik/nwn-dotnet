using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private void DmToolsBindings()
        {
          dmMode.SetBindWatch(player.oid, nuiToken.Token, false);
          instantCraft.SetBindWatch(player.oid, nuiToken.Token, false);
          instantCraft.SetBindValue(player.oid, nuiToken.Token, false);
          instantCraft.SetBindWatch(player.oid, nuiToken.Token, true);

          if (player.oid.IsDM)
          {
            dmModeLabel.SetBindValue(player.oid, nuiToken.Token, "Sortir du mode DM");
            dmMode.SetBindValue(player.oid, nuiToken.Token, true);
          }
          else
          {
            dmModeLabel.SetBindValue(player.oid, nuiToken.Token, "Mode DM");
            dmMode.SetBindValue(player.oid, nuiToken.Token, false);
          }

          dmMode.SetBindWatch(player.oid, nuiToken.Token, true);
        }
      }
    }
  }
}
