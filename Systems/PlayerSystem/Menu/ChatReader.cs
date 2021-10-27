using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public int CreateChatReaderWindow()
      {
        string windowId = "chatReader";

        NuiBind<bool> closable = new NuiBind<bool>("closable");
        NuiBind<bool> resizable = new NuiBind<bool>("resizable");
        NuiBind<bool> makeStatic = new NuiBind<bool>("static");

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 && windowRectangles[windowId].Width < oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        NuiWindow window = new NuiWindow(BuildChatReaderWindow(windowRectangle), "")
        {
          Geometry = geometry,
          Resizable = resizable,
          Collapsed = false,
          Closable = closable,
          Transparent = true,
          Border = false,
        };

        oid.OnNuiEvent -= HandleChatReaderEvents;
        oid.OnNuiEvent += HandleChatReaderEvents;

        int token = oid.CreateNuiWindow(window, windowId);

        makeStatic.SetBindValue(oid, token, false);
        resizable.SetBindValue(oid, token, true);
        closable.SetBindValue(oid, token, true);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);

        return token;
      }
      private void HandleChatReaderEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "chatReader" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
          return;
        
        switch (nuiEvent.ElementId)
        {
          case "fix":

            NuiBind<bool> fixWidgetValue = new NuiBind<bool>("static");
            NuiBind<bool> closableWidget = new NuiBind<bool>("closable");
            NuiBind<bool> resizableWidget = new NuiBind<bool>("resizable");

            switch (fixWidgetValue.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken))
            {
              case false:

                closableWidget.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, true);
                resizableWidget.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, true);

                break;

              case true:

                closableWidget.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, false);
                resizableWidget.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, false);

                break;
            }

            break;

          case "geometry":

            if (!player.openedWindows.ContainsKey("chatReader"))
              return;

            NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
            NuiRect rectangle = geometry.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

            if (rectangle.Width <= 0 || rectangle.Height <= 0)
              return;

            geometry.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
            UpdatePlayerChatLog(rectangle);
            geometry.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);
            break;
        }
      }

      public NuiColumn BuildChatReaderWindow(NuiRect windowRectangle)
      {
        NuiBind<bool> makeStatic = new NuiBind<bool>("static");

        List<NuiElement> colChidren = new List<NuiElement>();
        NuiColumn col = new NuiColumn() { Children = colChidren };

        List<NuiElement> groupChidren = new List<NuiElement>();
        NuiGroup chatReaderGroup = new NuiGroup() { Id = "chatReaderGroup", Border = false, Children = groupChidren };
        colChidren.Add(chatReaderGroup);

        List<NuiElement> colChatLogChidren = new List<NuiElement>();
        NuiColumn colChatLog = new NuiColumn() { Children = colChatLogChidren };
        groupChidren.Add(colChatLog);

        colChatLogChidren.Add(new NuiCheck("Figer", makeStatic) { Id = "fix", Tooltip = "Permet d'ancrer la fenêtre à l'écran", Width = 60 });

        int chatCount = readChatLines.Count - 1;

        while(chatCount >= 0)
        {
          ChatLine chatLine = readChatLines[chatCount];
          List<NuiElement> chatRowChildren = new List<NuiElement>();
          NuiRow chatRow = new NuiRow() { Children = chatRowChildren };
          
          chatRowChildren.Add(new NuiImage(chatLine.portrait) { Id = chatLine.playerName, Height = 25, Width = 19, ImageAspect = NuiAspect.ExactScaled } );
          
          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = chatLine.name.Length * 9, Id = chatLine.playerName, VerticalAlign = NuiVAlign.Top, ForegroundColor = new NuiColor(143, 127, 255) });
          NuiColor color = chatColors.ContainsKey(chatLine.channel) ? chatColors[chatLine.channel] : new NuiColor(255, 255, 255);

          if (chatLine.channel == Anvil.Services.ChatChannel.PlayerTell || chatLine.channel == Anvil.Services.ChatChannel.DmTell)
            color = new NuiColor(32, 255, 32);

          float textWidth = (windowRectangle.Width - 30 - chatLine.name.Length * 8) * 0.96f;
          int modulo = (int)(chatLine.text.Length * 8 / textWidth);

          NuiSpacer chatSpacer = new NuiSpacer() { Id = chatLine.playerName, Width = textWidth, Height = modulo == 0 ? 20 : modulo * 25 };

          chatRowChildren.Add(chatSpacer);
          colChatLogChidren.Add(chatRow);

          List<NuiDrawListItem> chatBreakerDrawList = new List<NuiDrawListItem>();
          chatSpacer.DrawList = chatBreakerDrawList;
          string remainingText = chatLine.text;
          int nbCharPerLine = (int)(textWidth / 8);
          int posXDisplay = 0;
          int i = 0;

          do
          {
            string currentLine = remainingText.Length > nbCharPerLine ? remainingText.Substring(0, (int)(textWidth / 8)) : remainingText;

            int breakPosition;

            if (remainingText.Length <= nbCharPerLine)
              breakPosition = currentLine.Length - 1;
            else
            {
              breakPosition = currentLine.Contains(" ") ? currentLine.LastIndexOf(" ") : currentLine.Length - 1;
              currentLine = currentLine.Substring(0, breakPosition);
            }

            chatBreakerDrawList.Add(new NuiDrawListText(color, new NuiRect(posXDisplay, 2 + i * 23, textWidth, 20), currentLine) { Fill = true });
            remainingText = remainingText.Remove(0, breakPosition);

            posXDisplay = -chatLine.name.Length * 11;
            i++;

          } while (remainingText.Length > 1);

          chatCount--;
        }

        return col;
      }
      public void UpdatePlayerChatLog(NuiRect windowRectangle)
      {
        NuiBind<bool> makeStatic = new NuiBind<bool>("static");

        List<NuiElement> groupChidren = new List<NuiElement>();
        NuiGroup chatReaderGroup = new NuiGroup() { Id = "chatReaderGroup", Border = false, Children = groupChidren };

        List<NuiElement> colChatLogChidren = new List<NuiElement>();
        NuiColumn colChatLog = new NuiColumn() { Children = colChatLogChidren };
        groupChidren.Add(colChatLog);

        colChatLogChidren.Add(new NuiCheck("Figer", makeStatic) { Id = "fix", Tooltip = "Permet d'ancrer la fenêtre à l'écran", Width = 60 });

        int chatCount = readChatLines.Count - 1;

        while (chatCount >= 0)
        {
          ChatLine chatLine = readChatLines[chatCount];
          List<NuiElement> chatRowChildren = new List<NuiElement>();
          NuiRow chatRow = new NuiRow() { Children = chatRowChildren };

          chatRowChildren.Add(new NuiImage(chatLine.portrait) { Id = chatLine.playerName, Height = 25, Width = 19, ImageAspect = NuiAspect.ExactScaled });

          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = chatLine.name.Length * 9, Id = chatLine.playerName, VerticalAlign = NuiVAlign.Top, ForegroundColor = new NuiColor(143, 127, 255) });
          NuiColor color = chatColors.ContainsKey(chatLine.channel) ? chatColors[chatLine.channel] : new NuiColor(255, 255, 255);

          if (chatLine.channel == Anvil.Services.ChatChannel.PlayerTell || chatLine.channel == Anvil.Services.ChatChannel.DmTell)
            color = new NuiColor(32, 255, 32);

          float textWidth = (windowRectangle.Width - 30 - chatLine.name.Length * 8) * 0.96f;
          int nbLines = (int)(chatLine.text.Length * 8 / textWidth);

          NuiSpacer chatSpacer = new NuiSpacer() { Id = chatLine.playerName, Width = textWidth, Height = nbLines == 0 ? 20 : nbLines * 25 };

          chatRowChildren.Add(chatSpacer);
          colChatLogChidren.Add(chatRow);

          List<NuiDrawListItem> chatBreakerDrawList = new List<NuiDrawListItem>();
          chatSpacer.DrawList = chatBreakerDrawList;
          string remainingText = chatLine.text;
          int nbCharPerLine = (int)(textWidth / 8);
          int posXDisplay = 0;
          int i = 0;

          do
          {
            string currentLine = remainingText.Length > nbCharPerLine ? remainingText.Substring(0, (int)(textWidth / 8)) : remainingText;

            int breakPosition;

            if (remainingText.Length <= nbCharPerLine)
              breakPosition = currentLine.Length - 1;
            else
            {
              breakPosition = currentLine.Contains(" ") ? currentLine.LastIndexOf(" ") : currentLine.Length - 1;
              currentLine = currentLine.Substring(0, breakPosition);
            }

            chatBreakerDrawList.Add(new NuiDrawListText(color, new NuiRect(posXDisplay, 2 + i * 23, textWidth, 20), currentLine) { Fill = true });
            remainingText = remainingText.Remove(0, breakPosition);

            posXDisplay = -chatLine.name.Length * 11;
            i++;

          } while (remainingText.Length > 1);

          chatCount--;
        }

        oid.NuiSetGroupLayout(openedWindows["chatReader"], "chatReaderGroup", chatReaderGroup);
      }
    }
  }
}
