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
        private void LoadPortraitLayout()
        {
          LoadTopMenuLayout();

          List<string> portraitTable = new();

          if (Portraits2da.playerCustomPortraits.TryGetValue(player.oid.PlayerName, out var value))
            portraitTable.AddRange(value);

          int baseRaceId = CreatureUtils.GetBaseRaceIdFromCustomRace(player.oid.LoginCreature.Race.Id);

          if (baseRaceId == CustomRace.HalfElf)
          {
            portraitTable.AddRange(Portraits2da.portraitFilteredEntries[CustomRace.Human, (int)player.oid.LoginCreature.Gender]);
            portraitTable.AddRange(Portraits2da.portraitFilteredEntries[CustomRace.Elf, (int)player.oid.LoginCreature.Gender]);
          }
          else
            portraitTable.AddRange(Portraits2da.portraitFilteredEntries[baseRaceId, (int)player.oid.LoginCreature.Gender]);

          int nbPortrait = 0;

          var portraitRow = new NuiRow() { Width = windowWidth / 1.1f, /*Height = windowWidth / 3.5f,*/ Children = new() };
          portraitRow.Children.Add(new NuiSpacer());
          rootChildren.Add(portraitRow);
          
          foreach(var portrait in portraitTable)
          {
            portraitRow.Children.Add(new NuiColumn() { Width = windowWidth / 5.5f, /*Height = windowWidth / 4.44f,*/ Children = new List<NuiElement>()
            {
              new NuiImage(portrait) { Id = portrait, Tooltip = portrait, Aspect = 0.65f, ImageAspect = NuiAspect.Fill, Width = windowWidth / 6, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Top }
            } });

            nbPortrait += 1;

            if(nbPortrait >= 4)
            {
              nbPortrait = 0;
              portraitRow.Children.Add(new NuiSpacer());
              portraitRow = new NuiRow() { Width = windowWidth / 1.1f, Height = windowWidth / 4.44f, Children = new() };
              portraitRow.Children.Add(new NuiSpacer());
              rootChildren.Add(portraitRow);
            }
          }

          portraitRow.Children.Add(new NuiSpacer());
          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
      }
    }
  }
}
