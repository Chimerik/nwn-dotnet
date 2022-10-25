using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class JukeBoxCurrentSongWindow : PlayerWindow
      {
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiColumn rootColumn = new() { Margin = 0.0f };
        public readonly NuiBind<string> song = new("song");
        public readonly NuiBind<string> askedBy = new("askedBy");

        public JukeBoxCurrentSongWindow(Player player, NwArea area) : base(player)
        {
          windowId = "jukeBoxCurrentSong";
          rootColumn.Children = rootChidren;

          rootChidren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(song) { HorizontalAlign = NuiHAlign.Center } } });
          rootChidren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(askedBy) { HorizontalAlign = NuiHAlign.Center } } });

          CreateWindow(area);
        }
        public void CreateWindow(NwArea area)
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? windowRectangle = new NuiRect(player.windowRectangles[windowId].X, player.windowRectangles[windowId].Y, 400, 80) : new NuiRect(10, 10, 400, 80);

          window = new NuiWindow(rootColumn, area.Name)
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            song.SetBindValue(player.oid, nuiToken.Token, $"Chanson en cours : {AmbientMusic2da.ambientMusicTable.GetRow(area.MusicBackgroundDayTrack).name}");
            string requester = area.GetObjectVariable<LocalVariableString>("_LAST_REQUEST_BY").HasValue ? area.GetObjectVariable<LocalVariableString>("_LAST_REQUEST_BY").Value : "L'aubergiste";
            askedBy.SetBindValue(player.oid, nuiToken.Token, $"Demandée par : {requester}");

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
      }
    }
  }
}
