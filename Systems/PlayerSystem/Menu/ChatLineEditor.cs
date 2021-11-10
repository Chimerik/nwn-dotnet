using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public int CreateChatLineEditorWindow(string chatLineText, int chatLineId)
      {
        string windowId = "chatLineEditor";
        NuiBind<string> writingChat = new NuiBind<string>("writingChat");
        NuiBind<int> chatLineIdBinding = new NuiBind<int>("chatLineId");

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 && windowRectangles[windowId].Width <= oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        NuiWindow window = new NuiWindow(BuildChatLineEditorWindow(windowRectangle), "Correcteur de chat")
        {
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = false,
          Border = true,
        };

        oid.OnNuiEvent -= HandleChatLineEditorEvents;
        oid.OnNuiEvent += HandleChatLineEditorEvents;

        int token = oid.CreateNuiWindow(window, windowId);

        writingChat.SetBindValue(oid, token, chatLineText.Replace("[modifié]", ""));
        chatLineIdBinding.SetBindValue(oid, token, chatLineId);

        writingChat.SetBindWatch(oid, token, true);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);

        return token;
      }
      private void HandleChatLineEditorEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "chatLineEditor" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
          return;

        switch (nuiEvent.ElementId)
        {
          case "sendText":

            if (nuiEvent.EventType != NuiEventType.Click)
              return;

            string chatText = new NuiBind<string>("writingChat").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
            ChatLine chatLine = player.readChatLines[new NuiBind<int>("chatLineId").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken)];

            oid.NuiDestroy(nuiEvent.WindowToken);

            if (chatText == chatLine.text)
              return;

            chatLine.textHistory.Add(chatLine.text);
            chatLine.text = chatText + "[modifié]";

            foreach (KeyValuePair<uint, Player> kvp in Players.Where(p => p.Value.readChatLines.Contains(chatLine) && p.Value.openedWindows.ContainsKey("chatReader")))
              ((ChatReaderWindow)kvp.Value.windows["chatReader"]).UpdateChat();

            break;

          case "geometry":

            NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
            NuiRect rectangle = geometry.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

            if (rectangle.Width <= 0 || rectangle.Height <= 0)
              return;

            geometry.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
            NuiGroup chatWriterGroup = BuildChatLineEditorWindow(rectangle);
            nuiEvent.Player.NuiSetGroupLayout(nuiEvent.WindowToken, chatWriterGroup.Id, chatWriterGroup);
            geometry.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);
            break;
        }
      }
      private NuiGroup BuildChatLineEditorWindow(NuiRect windowRectangle)
      {
        NuiBind<string> writingChat = new NuiBind<string>("writingChat");

        return new NuiGroup
        {
          Id = "chatWriterGroup",
          Border = false,
          Padding = 0,
          Margin = 0,
          Layout = new NuiColumn
          {
            Children = new List<NuiElement>
            {
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new NuiTextEdit("", writingChat, 3000, false) { Id = "chatWriter", Height = (windowRectangle.Height - 160) * 0.96f, Width = windowRectangle.Width * 0.96f }
                }
              },
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
          }
        };
      }
    }
  }
}
