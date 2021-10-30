using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateChatLineHistoryWindow(List<string> chatLineHistory)
      {
        string windowId = "chatLineHistory";
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 && windowRectangles[windowId].Width < oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        List<NuiElement> colChildren = new List<NuiElement>();
        NuiColumn root = new NuiColumn() { Children = colChildren };

        int historyCount = chatLineHistory.Count - 1;

        while (historyCount >= 0)
        {
          string lineHistory = chatLineHistory[historyCount];

          NuiRow row = new NuiRow
          {
            Children = new List<NuiElement>
            {
              new NuiTextEdit("", lineHistory, (ushort)lineHistory.Length, true)
            }
          };

          colChildren.Add(row);
        }

        NuiWindow window = new NuiWindow(root, "Itérations précédentes de la ligne de chat")
        {
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = false,
          Border = true,
        };

        int token = oid.CreateNuiWindow(window, windowId);
        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }
    }
  }
}
