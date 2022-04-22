using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ChatLineHistoryWindow : PlayerWindow
      {
        private readonly ChatLine chatLine;
        private readonly NuiColumn root;
        private readonly NuiBind<string> lineHistory = new ("lineHistory");
        private readonly NuiBind<int> listCount = new ("listCount");
        public ChatLineHistoryWindow(Player player, ChatLine chatLine) : base(player)
        {
          windowId = "chatLineHistory";
          this.chatLine = chatLine;

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell> { new NuiListTemplateCell(new NuiTextEdit("", lineHistory, 3000, true)) };

          root = new NuiColumn() 
          { 
            Children = new List<NuiElement>()
            {
              new NuiList(rowTemplate, listCount),
            } 
          };

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          window = new NuiWindow(root, "Itérations précédentes de la ligne de chat")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          token = player.oid.CreateNuiWindow(window, windowId);
          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          int historyCount = chatLine.textHistory.Count - 1;
          List<string> historyList = new List<string>();

          while (historyCount != 0)
          {
            historyList.Add(chatLine.textHistory[historyCount]);
            historyCount--;
          }

          lineHistory.SetBindValues(player.oid, token, historyList);
          listCount.SetBindValue(player.oid, token, chatLine.textHistory.Count);
        }
      }
    }
  }
}
