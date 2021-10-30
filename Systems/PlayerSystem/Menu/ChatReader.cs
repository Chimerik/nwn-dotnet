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
          case "-":
            Log.Info($"size before : {oid.LoginCreature.GetObjectVariable<LocalVariableFloat>("letterSize").Value}");
            oid.LoginCreature.GetObjectVariable<LocalVariableFloat>("letterSize").Value -= 0.1f;
            Log.Info($"size after : {oid.LoginCreature.GetObjectVariable<LocalVariableFloat>("letterSize").Value}");
            player.UpdatePlayerChatLog(player.windowRectangles["chatReader"]);
            break;

          case "+":
            Log.Info($"size before : {oid.LoginCreature.GetObjectVariable<LocalVariableFloat>("letterSize").Value}");
            oid.LoginCreature.GetObjectVariable<LocalVariableFloat>("letterSize").Value += 0.1f;
            Log.Info($"size after : {oid.LoginCreature.GetObjectVariable<LocalVariableFloat>("letterSize").Value}");
            player.UpdatePlayerChatLog(player.windowRectangles["chatReader"]);
            break;

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

            return;

          case "geometry":

            if (!player.openedWindows.ContainsKey("chatReader"))
              return;

            NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
            NuiRect rectangle = geometry.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

            if (rectangle.Width <= 0 || rectangle.Height <= 0)
              return;

            Log.Info($"geometry width: {rectangle.Width}");

            geometry.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
            UpdatePlayerChatLog(rectangle);
            geometry.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);
            return;
        }

        if (nuiEvent.ElementId.StartsWith("chat_"))
        {
          int chatLineId = int.Parse(nuiEvent.ElementId.Remove(0, 5));
          ChatLine chatLine = player.readChatLines[chatLineId];

          if (chatLine.playerName == nuiEvent.Player.PlayerName)
            CreateChatLineEditorWindow(chatLine.text, chatLineId);
          else if(chatLine.textHistory.Count > 0)
            CreateChatLineHistoryWindow(chatLine.textHistory);
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
        colChatLogChidren.Add(new NuiButton("-") { Id = "-" });
        colChatLogChidren.Add(new NuiButton("+") { Id = "+" });
        NuiButton button = new NuiButton("") { Id = "size", Width = 30, Height = 20,  };

        List<NuiDrawListItem> testDrawListItems = new List<NuiDrawListItem>();
        button.DrawList = testDrawListItems;
        testDrawListItems.Add(new NuiDrawListText(new NuiColor(255, 255, 255), new NuiRect(0,0, 30, 20), "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm") { Fill = true });
        colChatLogChidren.Add(button);

        oid.LoginCreature.GetObjectVariable<LocalVariableFloat>("letterSize").Value = 20;


        int chatCount = readChatLines.Count - 1;

        while(chatCount >= 0)
        {
          ChatLine chatLine = readChatLines[chatCount];
          List<NuiElement> chatRowChildren = new List<NuiElement>();
          NuiRow chatRow = new NuiRow() { Children = chatRowChildren };
          
          chatRowChildren.Add(new NuiImage(chatLine.portrait) { Id = chatLine.playerName, Height = 24, Width = 19, ImageAspect = NuiAspect.ExactScaled } );
          
          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = chatLine.name.Length * 8.2f, Id = chatLine.playerName, VerticalAlign = NuiVAlign.Top, ForegroundColor = new NuiColor(143, 127, 255) });
          NuiColor color = chatColors.ContainsKey(chatLine.channel) ? chatColors[chatLine.channel] : new NuiColor(255, 255, 255);

          if (chatLine.channel == Anvil.Services.ChatChannel.PlayerTell || chatLine.channel == Anvil.Services.ChatChannel.DmTell)
            color = new NuiColor(32, 255, 32);

          float textWidth = (windowRectangle.Width - 30 - chatLine.name.Length * 8) * 0.96f;
          int nbLines = (int)(chatLine.text.Length * 8 / textWidth);
          string remainingText = chatLine.text;

          bool updatedText = chatLine.text.Contains("[modifié]");
          if (updatedText)
          {
            nbLines += 1;
            remainingText = remainingText.Replace("[modifié]", "");
          }

          NuiSpacer chatSpacer = new NuiSpacer() { Id = $"chat_{chatCount}", Width = textWidth, Height = nbLines == 0 ? 20 : nbLines * 25 };

          List<NuiDrawListItem> chatBreakerDrawList = new List<NuiDrawListItem>();
          chatSpacer.DrawList = chatBreakerDrawList;
          int nbCharPerLine = (int)(textWidth / 8);
          float posXDisplay = 0;
          int i = 0;
          string currentLine = "";
          float uiScale = oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100;

          do
          {
            currentLine = remainingText.Length > nbCharPerLine ? remainingText.Substring(0, (int)(textWidth / 8)) : remainingText;

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

            posXDisplay = -chatLine.name.Length * (8 * uiScale);
            i++;

          } while (remainingText.Length > 1);

          if (updatedText)
            chatBreakerDrawList.Add(new NuiDrawListText(new NuiColor(200, 200, 200, 150), new NuiRect(posXDisplay, +i * 21 * uiScale, 90, 20), "[modifié]") { Fill = true });

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
        colChatLogChidren.Add(new NuiButton("-") { Id = "-" });
        colChatLogChidren.Add(new NuiButton("+") { Id = "+" });

        NuiButton button = new NuiButton("") { Id = "size", Width = oid.LoginCreature.GetObjectVariable<LocalVariableFloat>("letterSize").Value, Height = 20 };

        colChatLogChidren.Add(button);

        List<NuiDrawListItem> testDrawListItems = new List<NuiDrawListItem>();
        button.DrawList = testDrawListItems;
        testDrawListItems.Add(new NuiDrawListText(new NuiColor(255, 255, 255), new NuiRect(2, 0, oid.LoginCreature.GetObjectVariable<LocalVariableFloat>("letterSize").Value, 20), "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm") { Fill = true });

        int chatCount = readChatLines.Count - 1;

        while (chatCount >= 0)
        {
          ChatLine chatLine = readChatLines[chatCount];
          List<NuiElement> chatRowChildren = new List<NuiElement>();
          NuiRow chatRow = new NuiRow() { Children = chatRowChildren };
          
          chatRowChildren.Add(new NuiImage(chatLine.portrait) { Id = chatLine.playerName, Height = 24, Width = 19, ImageAspect = NuiAspect.ExactScaled });

          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = chatLine.name.Length * 8.2f, Id = chatLine.playerName, VerticalAlign = NuiVAlign.Top, ForegroundColor = new NuiColor(143, 127, 255) });
          NuiColor color = chatColors.ContainsKey(chatLine.channel) ? chatColors[chatLine.channel] : new NuiColor(255, 255, 255);
          Log.Info($"name width : {chatLine.name.Length * 8.2f}");
          if (chatLine.channel == Anvil.Services.ChatChannel.PlayerTell || chatLine.channel == Anvil.Services.ChatChannel.DmTell)
            color = new NuiColor(32, 255, 32);

          float textWidth = (windowRectangle.Width - 30 - chatLine.name.Length * 8) * 0.96f;
          int nbLines = (int)(chatLine.text.Length * 8 / textWidth);
          string remainingText = chatLine.text;

          bool updatedText = chatLine.text.Contains("[modifié]");
          if (updatedText)
          {
            nbLines += 1;
            remainingText = remainingText.Replace("[modifié]", "");
          }

          NuiSpacer chatSpacer = new NuiSpacer() { Id = $"chat_{chatCount}", Width = textWidth, Height = nbLines == 0 ? 20 : nbLines * 25 };

          chatRowChildren.Add(chatSpacer);
          colChatLogChidren.Add(chatRow);

          List<NuiDrawListItem> chatBreakerDrawList = new List<NuiDrawListItem>();
          chatSpacer.DrawList = chatBreakerDrawList;
          int nbCharPerLine = (int)(textWidth / 8);
          float posXDisplay = 0;
          int i = 0;
          string currentLine = "";
          float uiScale = oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100;

          do
          {
            currentLine = remainingText.Length > nbCharPerLine ? remainingText.Substring(0, (int)(textWidth / 8)) : remainingText;

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

            posXDisplay = -chatLine.name.Length * (8 /* uiScale*/);
            i++;

          } while (remainingText.Length > 1);

          if (updatedText)
            chatBreakerDrawList.Add(new NuiDrawListText(new NuiColor(200, 200, 200, 150), new NuiRect(posXDisplay, i * 23 * uiScale, 90, 20), "[modifié]") { Fill = true });

          chatCount--;
        }

        oid.NuiSetGroupLayout(openedWindows["chatReader"], "chatReaderGroup", chatReaderGroup);
      }
    }
  }
}
