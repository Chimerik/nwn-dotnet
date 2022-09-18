using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class AreaWindSettings : PlayerWindow
      {
        private readonly NuiColumn rootCol = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> directionX = new("directionX");
        private readonly NuiBind<string> directionY = new("directionY");
        private readonly NuiBind<string> directionZ = new("directionZ");
        private readonly NuiBind<string> magnitude = new("magnitude");

        public AreaWindSettings(Player player) : base(player)
        {
          windowId = "areaWindSettings";
          rootCol.Children = rootChildren;
          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Direction") { Width = 60, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("", directionX, 5, false) { Tooltip = "Paramètre X du vecteur", Width = 40 },
              new NuiTextEdit("", directionY, 5, false) { Tooltip = "Paramètre Y du vecteur", Width = 40 },
              new NuiTextEdit("", directionZ, 5, false) { Tooltip = "Paramètre Z du vecteur", Width = 40 },
              new NuiLabel("Magnitude") { Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("", magnitude, 5, false) { Width = 40 },
              new NuiButton("Valider") { Width = 60, Height = 35, Id = "validate" },
            }
          });

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 400, 100);

          window = new NuiWindow(rootCol, $"Vent : {player.oid.ControlledCreature.Area.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleAreaWindSettingsEvents;

            NwArea area = player.oid.ControlledCreature.Area;
            AreaWind wind = AreaPlugin.GetAreaWind(area);
            directionX.SetBindValue(player.oid, nuiToken.Token, wind.vDirection.X.ToString());
            directionY.SetBindValue(player.oid, nuiToken.Token, wind.vDirection.Y.ToString());
            directionZ.SetBindValue(player.oid, nuiToken.Token, wind.vDirection.Z.ToString());
            magnitude.SetBindValue(player.oid, nuiToken.Token, wind.fMagnitude.ToString());

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleAreaWindSettingsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":
                  Vector3 direction = new Vector3(float.TryParse(directionX.GetBindValue(player.oid, nuiToken.Token), out float x) ? x : 0, float.TryParse(directionY.GetBindValue(player.oid, nuiToken.Token), out float y) ? y : 0, float.TryParse(directionZ.GetBindValue(player.oid, nuiToken.Token), out float z) ? z : 0);
                  float m = float.TryParse(directionX.GetBindValue(player.oid, nuiToken.Token), out m) ? m : 0;
                  player.oid.ControlledCreature.Area.SetAreaWind(direction, m, 0, 0);
                  player.oid.SendServerMessage("Nouvelle configuration du vent enregistreée", ColorConstants.Orange);
                  break;
              }

              break;
          }
        }
      }
    }
  }
}
