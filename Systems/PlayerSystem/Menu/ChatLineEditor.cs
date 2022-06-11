using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ChatLineEditorWindow : PlayerWindow
      {
        private readonly NuiBind<string> writingChat = new ("writingChat");
        private readonly ChatLine chatLine;
        private readonly NuiTextEdit chatWriter;
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn layoutColumn;

        public ChatLineEditorWindow(Player player, ChatLine chatLine) : base (player)
        {
          windowId = "chatLineEditor";
          this.chatLine = chatLine;

          chatWriter = new NuiTextEdit("", writingChat, 3000, false) { Id = "chatWriter" };

          layoutColumn = new NuiColumn
          {
            Children = new List<NuiElement>
              {
                new NuiRow { Children = new List<NuiElement> { chatWriter } },
                new NuiRow
                {
                  Children = new List<NuiElement>
                  {
                    new NuiSpacer(),
                    new NuiButton("Corriger") { Id = "sendText" },
                    new NuiSpacer()
                  }
                }
              }
          };

          rootGroup = new NuiGroup { Id = "chatWriterGroup", Border = false, Padding = 0, Margin = 0, Layout = layoutColumn };

          CreateWindow();
        }

        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);
          chatWriter.Width = windowRectangle.Width * 0.96f;
          chatWriter.Height = (windowRectangle.Height - 160) * 0.96f;

          NuiWindow window = new NuiWindow(rootGroup, "Correcteur de chat")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleChatLineEditorEvents;

            writingChat.SetBindValue(player.oid, nuiToken.Token, chatLine.text.Replace("[modifié]", ""));
            writingChat.SetBindWatch(player.oid, nuiToken.Token, true);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandleChatLineEditorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.ElementId)
          {
            case "sendText":

              if (nuiEvent.EventType != NuiEventType.Click)
                return;

              string chatText = writingChat.GetBindValue(nuiEvent.Player, nuiToken.Token);

              CloseWindow();

              if (chatText == chatLine.text)
                return;

              chatLine.textHistory.Add(chatLine.text);
              chatLine.text = chatText + "[modifié]";

              foreach (Player target in Players.Values.Where(p => p.readChatLines.Contains(chatLine) && p.TryGetOpenedWindow("chatReader", out PlayerWindow chatWindow)))
                ((ChatReaderWindow)target.windows["chatReader"]).UpdateChat();

              break;

            case "geometry":

              NuiRect rectangle = geometry.GetBindValue(nuiEvent.Player, nuiToken.Token);

              if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

              geometry.SetBindWatch(nuiEvent.Player, nuiToken.Token, false);
              chatWriter.Width = rectangle.Width * 0.96f;
              chatWriter.Height = (rectangle.Height - 160) * 0.96f;
              rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
              geometry.SetBindWatch(nuiEvent.Player, nuiToken.Token, true);
              break;
          }
        }
      }
    }
  }
}
