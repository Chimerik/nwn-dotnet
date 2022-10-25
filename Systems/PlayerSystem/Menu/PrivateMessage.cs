using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using static System.Net.Mime.MediaTypeNames;
using static Anvil.API.Events.ModuleEvents;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class PrivateMessageWindow : PlayerWindow
      {
        private readonly float characterWidth = 10 /* oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100*/;
        private readonly float spaceWidth = 4;
        private readonly float characterHeight = 18 /* oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100*/;
        private readonly NuiGroup chatReaderGroup = new() { Id = "chatReaderGroup", Border = false, Scrollbars = NuiScrollbars.Y };
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiColumn colChatLog = new();
        private readonly List<NuiElement> colChatLogChildren = new();
        private readonly NuiBind<string> writingChat = new("writingChat");
        public bool read { get; set; }

        public PrivateMessageWindow(Player player, NwPlayer receiver) : base(player)
        {
          windowId = receiver.PlayerName;

          rootColumn.Children = rootChidren;
          colChatLog.Children = colChatLogChildren;
          chatReaderGroup.Layout = colChatLog;

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { chatReaderGroup } });
          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("", writingChat, 3000, true) { Id = "chatWriter", Height = 60, Tooltip = "Espace + entrée pour ajouter un saut de ligne." } } });

          CreateWindow();
        }

        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) && player.windowRectangles[windowId].Width > 0 && player.windowRectangles[windowId].Width < player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          window = new NuiWindow(rootColumn, windowId)
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandlePrivateMessageEvents;

            writingChat.SetBindValue(player.oid, nuiToken.Token, "");

            writingChat.SetBindWatch(player.oid, nuiToken.Token, true);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            UpdateChat();

            read = true;
          }
        }

        private void HandlePrivateMessageEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.ElementId)
          {
            case "geometry":

              if (!IsOpen)
                return;

              NuiRect rectangle = geometry.GetBindValue(nuiEvent.Player, nuiToken.Token);

              if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;
;
              UpdateChat();

              return;

            case "chatWriter":

              switch (nuiEvent.EventType)
              {
                case NuiEventType.Focus:

                  string command = writingChat.GetBindValue(nuiEvent.Player, nuiToken.Token);
                  Effect visualMark = Effect.VisualEffect((VfxType)1249);
                  visualMark.Tag = "VFX_PRIVATE_MESSAGE_MARK";
                  visualMark.SubType = EffectSubType.Supernatural;
                  nuiEvent.Player.ControlledCreature.ApplyEffect(EffectDuration.Permanent, visualMark);

                  break;

                case NuiEventType.Blur:
                  foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_PRIVATE_MESSAGE_MARK"))
                    nuiEvent.Player.ControlledCreature.RemoveEffect(eff);
                  break;
              }

              break;

            case "writingChat":

              if (nuiEvent.EventType != NuiEventType.Watch)
                return;

              string chatText = writingChat.GetBindValue(nuiEvent.Player, nuiToken.Token);

              if (chatText.Length < 1)
                return;

              if (chatText.Contains(Environment.NewLine))
              {
                int pos = 0;

                foreach (char c in chatText)
                {
                  if (c == '\n' && pos != 0 && chatText[pos - 1] != ' ')
                  {
                    ChatWriterSendMessage(chatText.Remove(pos, 1));

                    foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_PRIVATE_MESSAGE_MARK"))
                      nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

                    writingChat.SetBindWatch(nuiEvent.Player, nuiToken.Token, false);
                    writingChat.SetBindValue(nuiEvent.Player, nuiToken.Token, "");
                    writingChat.SetBindWatch(nuiEvent.Player, nuiToken.Token, true);
                  }

                  pos++;
                }
              }

              break;
          }
        }

        private void ChatWriterSendMessage(string message)
        {
          if (message.Length < 1)
            return;

          NwPlayer target = NwModule.Instance.Players.FirstOrDefault(p => p.PlayerName == windowId);

          if (target == null)
          {
            player.oid.SendServerMessage($"{windowId.ColorString(ColorConstants.White)} n'est actuellement pas connecté.", ColorConstants.Red);
            return;
          }

          ChatSystem.chatService.SendMessage(!player.oid.IsDM ? ChatChannel.PlayerTell : ChatChannel.DmTell, message, player.oid.ControlledCreature, target);

          UpdateChat();
        }
        public void UpdateChat()
        {
          colChatLogChildren.Clear();

          foreach (ChatLine chatLine in player.readChatLines)
          {
            if (chatLine.category == ChatLine.ChatCategory.Private && !string.IsNullOrEmpty(chatLine.text)
              && (chatLine.playerName == player.oid.PlayerName && chatLine.receiverPlayerName == windowId || (chatLine.playerName == windowId && chatLine.receiverPlayerName == player.oid.PlayerName)))
            {
              Log.Info(chatLine.text);
              AddNewChat(chatLine);
            }
          }

          colChatLogChildren.Reverse();
          chatReaderGroup.SetLayout(player.oid, nuiToken.Token, colChatLog);
        }
        private void AddNewChat(ChatLine chatLine)
        {
          string chatText = $"{chatLine.name} : {chatLine.text}";
          float rectWidth = geometry.GetBindValue(player.oid, nuiToken.Token).Width - 40;
          int nblines = 0;

          if (chatText.Contains("\n"))
          {
            foreach (string text in chatText.Split("\n"))
              nblines += 1 + (int)((text.Length * characterWidth) / rectWidth);

            nblines -= 1;
          }
          else
            nblines = (int)((chatText.Length * characterWidth) / rectWidth);

          if (nblines < 1)
            nblines = 1;

          /*Log.Info($"nbLines : {nblines}");
          Log.Info($"Height : {nblines * characterHeight}");

          Log.Info($"RectWidth : {rectWidth}");
          Log.Info($"text * char : {chatText.Length * characterWidth}");
          Log.Info($"calcul : {(chatText.Length * characterWidth) / rectWidth}");*/

          colChatLogChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiText($"{chatLine.playerName} : {chatLine.text}") { Height = 25 + nblines * characterHeight, Scrollbars = NuiScrollbars.None, Border = false } } });
        }
      }
    }
  }
}
