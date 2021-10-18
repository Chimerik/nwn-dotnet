using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateChatReaderWindow()
      {
        string windowId = "chatReader";

        NuiBind<bool> closable = new NuiBind<bool>("closable");
        NuiBind<bool> resizable = new NuiBind<bool>("resizable");
        NuiBind<bool> makeStatic = new NuiBind<bool>("static");

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 && windowRectangles[windowId].Width <= oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

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
        }
      }
      
      private NuiGroup BuildChatReaderWindow(NuiRect windowRectangle)
      {
        if (!Players.TryGetValue(oid.LoginCreature, out Player player))
          return null;

        NuiBind<bool> makeStatic = new NuiBind<bool>("static");
        NuiBind<bool> multiline = new NuiBind<bool>("multiLine");

        List<NuiElement> groupChidren = new List<NuiElement>();
        NuiGroup chatReaderGroup = new NuiGroup() { Id = "chatReaderGroup", Border = false, Children = groupChidren };
        List<NuiElement> colChidren = new List<NuiElement>();
        NuiColumn col = new NuiColumn() { Children = colChidren };
        groupChidren.Add(col);

        List<NuiElement> fixRowChildren = new List<NuiElement>();
        NuiRow fixRow = new NuiRow() { Children = fixRowChildren };
        fixRowChildren.Add(new NuiCheck("Figer", makeStatic) { Id = "fix", Tooltip = "Permet d'ancrer la fenêtre à l'écran", Width = 60 });
        colChidren.Add(fixRow);

        foreach (ChatLine chatLine in player.readChatLines)
        {
          List<NuiElement> chatRowChildren = new List<NuiElement>();
          NuiRow chatRow = new NuiRow() { Children = chatRowChildren };
          chatRowChildren.Add(new NuiImage(chatLine.portrait) { Id = chatLine.playerName, Height = 40, Width = 25, ImageAspect = NuiAspect.Exact });
          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = chatLine.name.Length * 10, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle });

          NuiColor color = player.chatColors.ContainsKey(chatLine.channel) ? player.chatColors[chatLine.channel] : new NuiColor(125, 100, 75);

          float textWidth = (windowRectangle.Width - 30 - chatLine.name.Length * 10) * 0.96f;

          if (chatLine.text.Length < textWidth / 10)
          {
            chatRowChildren.Add(new NuiLabel(chatLine.text) { ForegroundColor = color, Width = textWidth, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle });
            colChidren.Add(chatRow);
          }
          else
          {
            int modulo = (int)((chatLine.text.Length * 10) / textWidth);
            int i = 0;

            chatRowChildren.Add(new NuiLabel(chatLine.text.Substring(i * (int)(textWidth / 10), (int)(textWidth / 10))) { ForegroundColor = color, Width = textWidth, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle });
            colChidren.Add(chatRow);

            for (i = 1; i < modulo; i++)
            {
              chatRowChildren = new List<NuiElement>();
              chatRow = new NuiRow() { Children = chatRowChildren };
              chatRowChildren.Add(new NuiSpacer() { Width = 25 + chatLine.name.Length * 10 });

              try
              {
                chatRowChildren.Add(new NuiLabel(chatLine.text.Substring(i * (int)(textWidth / 10), (int)(textWidth / 10))) { ForegroundColor = color, Width = textWidth, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle });
              }
              catch (Exception)
              {
                chatRowChildren.Add(new NuiLabel(chatLine.text.Substring(i * (int)(textWidth / 10))) { ForegroundColor = color, Width = textWidth, HorizontalAlign = NuiHAlign.Left, VerticalAlign = NuiVAlign.Middle });
              }

              colChidren.Add(chatRow);
            }
          }
        }

        return chatReaderGroup;
      }
    }
  }
}
