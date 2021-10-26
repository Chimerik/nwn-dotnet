﻿using System;
using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

using Newtonsoft.Json;

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
        }
      }

      public NuiColumn BuildChatReaderWindow(NuiRect windowRectangle)
      {
        NuiBind<bool> makeStatic = new NuiBind<bool>("static");

        List<NuiElement> colChidren = new List<NuiElement>();
        NuiColumn col = new NuiColumn() { Children = colChidren };
        
        List<NuiElement> fixRowChildren = new List<NuiElement>();
        NuiRow fixRow = new NuiRow() { Children = fixRowChildren };
        fixRowChildren.Add(new NuiCheck("Figer", makeStatic) { Id = "fix", Tooltip = "Permet d'ancrer la fenêtre à l'écran", Width = 60 });
        colChidren.Add(fixRow);

        List<NuiElement> groupChidren = new List<NuiElement>();
        NuiGroup chatReaderGroup = new NuiGroup() { Id = "chatReaderGroup", Border = false, Children = groupChidren };
        colChidren.Add(chatReaderGroup);

        List<NuiElement> chatLogRowChildren = new List<NuiElement>();
        NuiRow chatLogRow = new NuiRow() { Children = chatLogRowChildren };
        groupChidren.Add(chatLogRow);

        int chatCount = readChatLines.Count - 1;

        while(chatCount >= 0)
        {
          ChatLine chatLine = readChatLines[chatCount];
          List<NuiElement> chatRowChildren = new List<NuiElement>();
          NuiRow chatRow = new NuiRow() { Children = chatRowChildren };
          
          chatRowChildren.Add(new NuiImage(chatLine.portrait) { Id = chatLine.playerName, Height = 25, Width = 19, ImageAspect = NuiAspect.ExactScaled } );
          
          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = chatLine.name.Length * 8, Id = chatLine.playerName, VerticalAlign = NuiVAlign.Top, ForegroundColor = new NuiColor(143, 127, 255) });
          NuiColor color = chatColors.ContainsKey(chatLine.channel) ? chatColors[chatLine.channel] : new NuiColor(255, 255, 255);

          if (chatLine.channel == Anvil.Services.ChatChannel.PlayerTell || chatLine.channel == Anvil.Services.ChatChannel.DmTell)
            color = new NuiColor(32, 255, 32);

          float textWidth = (windowRectangle.Width - 30 - chatLine.name.Length * 8) * 0.96f;
          int modulo = (int)(chatLine.text.Length * 8 / textWidth);

          //Log.Info($"testWidth = {textWidth} - text length = {chatLine.text.Length}");

          NuiSpacer chatSpacer = new NuiSpacer() { Id = chatLine.playerName, Width = textWidth, Height = modulo == 0 ? 20 : modulo * 25 };

          //Log.Info($"modulo = {modulo} - Height = {chatSpacer.Height}");

          chatRowChildren.Add(chatSpacer);
          chatLogRowChildren.Add(chatRow);

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

          chatCount--;
        }

        return col;
      }
      public void UpdatePlayerChatLog(NuiRect windowRectangle)
      {
        List<NuiElement> groupChidren = new List<NuiElement>();
        NuiGroup chatReaderGroup = new NuiGroup() { Id = "chatReaderGroup", Border = false, Children = groupChidren };

        List<NuiElement> chatLogRowChildren = new List<NuiElement>();
        NuiRow chatLogRow = new NuiRow() { Children = chatLogRowChildren };
        groupChidren.Add(chatLogRow);

        int chatCount = readChatLines.Count - 1;

        while (chatCount >= 0)
        {
          ChatLine chatLine = readChatLines[chatCount];
          List<NuiElement> chatRowChildren = new List<NuiElement>();
          NuiRow chatRow = new NuiRow() { Children = chatRowChildren };

          chatRowChildren.Add(new NuiImage(chatLine.portrait) { Id = chatLine.playerName, Height = 25, Width = 19, ImageAspect = NuiAspect.ExactScaled });

          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = chatLine.name.Length * 8, Id = chatLine.playerName, VerticalAlign = NuiVAlign.Top, ForegroundColor = new NuiColor(143, 127, 255) });
          NuiColor color = chatColors.ContainsKey(chatLine.channel) ? chatColors[chatLine.channel] : new NuiColor(255, 255, 255);

          if (chatLine.channel == Anvil.Services.ChatChannel.PlayerTell || chatLine.channel == Anvil.Services.ChatChannel.DmTell)
            color = new NuiColor(32, 255, 32);

          float textWidth = (windowRectangle.Width - 30 - chatLine.name.Length * 8) * 0.96f;
          int modulo = (int)(chatLine.text.Length * 8 / textWidth);

          //Log.Info($"testWidth = {textWidth} - text length = {chatLine.text.Length}");

          NuiSpacer chatSpacer = new NuiSpacer() { Id = chatLine.playerName, Width = textWidth, Height = modulo == 0 ? 20 : modulo * 25 };

          //Log.Info($"modulo = {modulo} - Height = {chatSpacer.Height}");

          chatLogRowChildren.Add(chatSpacer);
          groupChidren.Add(chatRow);

          if (modulo == 0)
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
              if (divider + i * divider > chatLine.text.Length)
                chatBreaker = chatLine.text.Substring(i * divider);
              else
                chatBreaker = chatLine.text.Substring(i * divider, divider);

              if (i == 0)
                chatBreakerDrawList.Add(new NuiDrawListText(color, new NuiRect(0, 2 + i * 23, textWidth, 20), chatBreaker) { Fill = true });
              else
                chatBreakerDrawList.Add(new NuiDrawListText(color, new NuiRect(-chatLine.name.Length * 8, 2 + i * 23, textWidth, 20), chatBreaker) { Fill = true });
            }
          }

          chatCount--;
        }

        oid.NuiSetGroupLayout(openedWindows["chatReader"], "chatReaderGroup", chatReaderGroup);
      }
    }
  }
}
