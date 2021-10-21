using System;
using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

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
          
          chatRowChildren.Add(
            new NuiButton("")
            {
              Id = chatLine.playerName,
              Height = 28,
              Width = 19,
              DrawList = new List<NuiDrawListItem>()
              {
                new NuiDrawListImage(chatLine.portrait, new NuiRect(10, 13, 190, 10)) { Fill = false, Aspect = NuiAspect.ExactScaled  },
                new NuiDrawListImage(chatLine.portrait, new NuiRect(70, 13, 190, 10)) { Fill = true, Aspect = NuiAspect.ExactScaled  },
                new NuiDrawListImage(chatLine.portrait, new NuiRect(130, 13, 190, 10)) { Fill = false, Aspect = NuiAspect.Exact  },
                new NuiDrawListImage(chatLine.portrait, new NuiRect(190, 13, 190, 10)) { Fill = true, Aspect = NuiAspect.Exact  },
                new NuiDrawListImage(chatLine.portrait, new NuiRect(10, 200, 190, 10)) { Fill = false, Aspect = NuiAspect.Fit100  },
                new NuiDrawListImage(chatLine.portrait, new NuiRect(50, 200, 190, 10)) { Fill = true, Aspect = NuiAspect.Fit100  },
                new NuiDrawListImage(chatLine.portrait, new NuiRect(90, 200, 20, 300)) { Fill = false, Aspect = NuiAspect.Fit  },
                new NuiDrawListImage(chatLine.portrait, new NuiRect(130, 200, 190, 10)) { Fill = true, Aspect = NuiAspect.Fit },
              }
            }
          );
          
          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = chatLine.name.Length * 8, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Top, ForegroundColor = new NuiColor(143, 127, 255) });
          NuiColor color = player.chatColors.ContainsKey(chatLine.channel) ? player.chatColors[chatLine.channel] : new NuiColor(255, 255, 255);

          float textWidth = (windowRectangle.Width - 30 - chatLine.name.Length * 8) * 0.96f;
          int modulo = (int)(chatLine.text.Length * 8 / textWidth);

          Log.Info($"testWidth = {textWidth} - text length = {chatLine.text.Length}");

          NuiSpacer chatSpacer = new NuiSpacer() { Width = textWidth, Height = modulo == 0 ? 20 : modulo * 25 };

          Log.Info($"modulo = {modulo} - Height = {chatSpacer.Height}");

          chatRowChildren.Add(chatSpacer);
          colChidren.Add(chatRow);

          if(modulo == 0)
          {
            chatSpacer.DrawList = new List<NuiDrawListItem>()
            { 
              new NuiDrawListText(color, new NuiRect(0, 2, textWidth, 20), chatLine.text)
            };
          }
          else
          {
            List<NuiDrawListItem> chatBreakerDrawList = new List<NuiDrawListItem>();
            chatSpacer.DrawList = chatBreakerDrawList;
            string chatBreaker = "";
            int divider = (int)(textWidth / 8);

            for (int i = 0; i <= modulo; i++)
            {
              if(divider + i * divider > chatLine.text.Length)
                chatBreaker = chatLine.text.Substring(i * divider);
              else
                chatBreaker = chatLine.text.Substring(i * divider, divider);

              if(i == 0)
                chatBreakerDrawList.Add(new NuiDrawListText(color, new NuiRect(0, 2 + i * 23, textWidth, 20), chatBreaker) { Fill = true });
              else
                chatBreakerDrawList.Add(new NuiDrawListText(color, new NuiRect(-chatLine.name.Length * 8, 2 + i * 23, textWidth, 20), chatBreaker) { Fill = true });
            }
          }
        }

        return chatReaderGroup;
      }
    }
  }
}
