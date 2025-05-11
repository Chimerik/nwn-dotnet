using System;
using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private void PortraitBindings()
        {
         /* List<string>[] portraitList = new List<string>[] { new(), new(), new(), new() };
          List<bool>[] portraitVisibility = new List<bool>[] { new(), new(), new(), new() };*/
          


          /*if (portraitTable != null)
            for (int i = 0; i < portraitTable.Count; i += 4)
              for (int j = 0; j < 4; j++)
                try
                {
                  portraitVisibility[j].Add(true);
                  portraitList[j].Add(portraitTable[i + j]);
                }
                catch (Exception)
                {
                  portraitVisibility[j].Add(false);
                  portraitList[j].Add("");
                }

          portraits1.SetBindValues(player.oid, nuiToken.Token, portraitList[0]);
          portraits2.SetBindValues(player.oid, nuiToken.Token, portraitList[1]);
          portraits3.SetBindValues(player.oid, nuiToken.Token, portraitList[2]);
          portraits4.SetBindValues(player.oid, nuiToken.Token, portraitList[3]);

          visible1.SetBindValues(player.oid, nuiToken.Token, portraitVisibility[0]);
          visible2.SetBindValues(player.oid, nuiToken.Token, portraitVisibility[1]);
          visible3.SetBindValues(player.oid, nuiToken.Token, portraitVisibility[2]);
          visible4.SetBindValues(player.oid, nuiToken.Token, portraitVisibility[3]);

          listCount.SetBindValue(player.oid, nuiToken.Token, portraitList[0].Count);*/
        }
      }
    }
  }
}
