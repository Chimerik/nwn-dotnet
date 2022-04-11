using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class AreaWindSettings : PlayerWindow
      {
        private readonly NuiRow rootRow = new NuiRow();
        private readonly List<NuiElement> rootChildren = new List<NuiElement>();

        private readonly NuiBind<string> directionX = new NuiBind<string>("directionX");
        private readonly NuiBind<string> directionY = new NuiBind<string>("directionY");
        private readonly NuiBind<string> directionZ = new NuiBind<string>("directionZ");
        private readonly NuiBind<string> magnitude = new NuiBind<string>("magnitude");

        public AreaWindSettings(Player player) : base(player)
        {
          windowId = "areaWindSettings";

          rootRow.Children = rootChildren;

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

          window = new NuiWindow(rootRow, $"Vent : {player.oid.ControlledCreature.Area.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleAreaWindSettingsEvents;
          player.oid.OnNuiEvent += HandleAreaWindSettingsEvents;
          player.oid.OnServerSendArea -= OnAreaChangeCloseWindow;
          player.oid.OnServerSendArea += OnAreaChangeCloseWindow;

          token = player.oid.CreateNuiWindow(window, windowId);

          NwArea area = player.oid.ControlledCreature.Area;

          directionX.SetBindValue(player.oid, token, area.GetObjectVariable<LocalVariableFloat>("WIND_X").Value.ToString());
          directionY.SetBindValue(player.oid, token, area.GetObjectVariable<LocalVariableFloat>("WIND_Y").Value.ToString());
          directionZ.SetBindValue(player.oid, token, area.GetObjectVariable<LocalVariableFloat>("WIND_Z").Value.ToString());
          magnitude.SetBindValue(player.oid, token, area.GetObjectVariable<LocalVariableFloat>("WIND_M").Value.ToString());

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);
        }
        private void HandleAreaWindSettingsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":
                  Vector3 direction = new Vector3(float.TryParse(directionX.GetBindValue(player.oid, token), out float x) ? x : 0, float.TryParse(directionY.GetBindValue(player.oid, token), out float y) ? y : 0, float.TryParse(directionZ.GetBindValue(player.oid, token), out float z) ? z : 0);
                  float m = float.TryParse(directionX.GetBindValue(player.oid, token), out m) ? m : 0;
                  AreaSystem.RegisterAreaWind(player.oid.ControlledCreature.Area, direction, m);
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
