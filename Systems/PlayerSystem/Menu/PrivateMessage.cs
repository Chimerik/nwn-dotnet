﻿using System;
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
      public class PrivateMessageWindow : PlayerWindow
      {
        private readonly float characterWidth = 8 /* oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100*/;
        private readonly float spaceWidth = 4;
        private readonly float characterHeight = 18 /* oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100*/;
        private readonly NuiGroup chatReaderGroup;
        private readonly NuiColumn rootColumn;
        private readonly NuiColumn colChatLog;
        private readonly List<NuiElement> colChatLogChidren;
        private readonly NuiRow settingsRow;
        private readonly NuiBind<string> writingChat = new ("writingChat");
        private readonly NuiBind<bool> makeStatic = new ("static");
        public bool read { get; set; }

        public PrivateMessageWindow(Player player, NwPlayer receiver) : base(player)
        {
          windowId = receiver.PlayerName;

          List<NuiElement> colChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = colChidren };
          colChatLog = new NuiColumn() { Children = colChatLogChidren };
          colChatLogChidren = new List<NuiElement>();

          chatReaderGroup = new NuiGroup() { Id = "chatReaderGroup", Border = false, Layout = colChatLog, Scrollbars = NuiScrollbars.Y };
          colChidren.Add(chatReaderGroup);

          List<NuiElement> settingsRowChildren = new List<NuiElement>();
          settingsRow = new NuiRow() { Children = settingsRowChildren };
          colChatLogChidren.Add(settingsRow);

          settingsRowChildren.Add(new NuiCheck("Figer", makeStatic) { Id = "fix", Tooltip = "Permet d'ancrer la fenêtre à l'écran", Width = 60 });
          settingsRowChildren.Add(new NuiTextEdit("", writingChat, 3000, true) { Id = "chatWriter", Height = 45, Tooltip = "Espace + entrée pour ajouter un saut de ligne." });

          CreateChatRows();
          CreateWindow();
        }

        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) && player.windowRectangles[windowId].Width > 0 && player.windowRectangles[windowId].Width < player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          window = new NuiWindow(rootColumn, "")
          {
            Geometry = geometry,
            Resizable = resizable,
            Collapsed = false,
            Closable = closable,
            Transparent = true,
            Border = false,
          };

          player.oid.OnNuiEvent -= HandlePrivateMessageEvents;
          player.oid.OnNuiEvent += HandlePrivateMessageEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          makeStatic.SetBindValue(player.oid, token, false);
          resizable.SetBindValue(player.oid, token, true);
          closable.SetBindValue(player.oid, token, true);
          writingChat.SetBindValue(player.oid, token, writingChat.GetBindValue(player.oid, token));

          writingChat.SetBindWatch(player.oid, token, true);
          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          read = true;
          player.openedWindows[windowId] = token;
        }

        private void HandlePrivateMessageEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.ElementId)
          {
            case "fix":

              switch (makeStatic.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken))
              {
                case false:
                  closable.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, true);
                  resizable.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, true);
                  break;

                case true:
                  closable.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, false);
                  resizable.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, false);
                  break;
              }

              return;

            case "geometry":

              if (!player.openedWindows.ContainsKey(windowId))
                return;

              NuiRect rectangle = geometry.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

              if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

              colChatLogChidren.Clear();
              colChatLogChidren.Add(settingsRow);
              CreateChatRows();
              chatReaderGroup.SetLayout(player.oid, token, colChatLog);

              return;

            case "chatWriter":

              switch (nuiEvent.EventType)
              {
                case NuiEventType.Focus:

                  string command = writingChat.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
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

              string chatText = writingChat.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

              if (chatText.Length < 1)
                return;

              if (chatText.Contains(Environment.NewLine))
              {
                int pos = 0;

                foreach(char c in chatText)
                {
                  if (c == '\n' && pos != 0  && chatText[pos - 1] != ' ')
                  {
                    chatText = chatText.Remove(pos, 1);

                    ChatWriterSendMessage(chatText);

                    foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_PRIVATE_MESSAGE_MARK"))
                      nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

                    writingChat.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
                    writingChat.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, "");
                    writingChat.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);
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
          
          if(target == null)
          {
            player.oid.SendServerMessage($"{windowId.ColorString(ColorConstants.White)} n'est actuellement pas connecté.", ColorConstants.Red);
            return;
          }

          ChatSystem.chatService.SendMessage(!player.oid.IsDM ? ChatChannel.PlayerTell : ChatChannel.DmTell, message, player.oid.ControlledCreature, target);
        }

        private void CreateChatRows()
        {
          int chatId = 0;
          foreach (ChatLine chatLine in player.readChatLines)
          {
            AddNewChat(chatLine, chatId);
            chatId++;
          }
        }
        public void InsertNewChatInWindow(ChatLine chatLine)
        {
          read = true;
          AddNewChat(chatLine, player.readChatLines.Count - 1);
          chatReaderGroup.SetLayout(player.oid, token, colChatLog);
        }

        public void UpdateChat()
        {
          colChatLogChidren.Clear();
          colChatLogChidren.Add(settingsRow);

          CreateChatRows();

          chatReaderGroup.SetLayout(player.oid, token, colChatLog);
        }

        private void AddNewChat(ChatLine chatLine, int chatId)
        {
          int nbSpaceInName = chatLine.name.Count(s => s == ' ');
          float nameWidth = (chatLine.name.Length - nbSpaceInName) * characterWidth + nbSpaceInName * spaceWidth;
          List<NuiElement> chatRowChildren = new List<NuiElement>();
          NuiRow chatRow = new NuiRow() { Children = chatRowChildren };

          if (string.IsNullOrEmpty(chatLine.receiverPlayerName) || (chatLine.playerName != windowId && chatLine.receiverPlayerName != windowId))
            return;

          colChatLogChidren.Insert(1, chatRow);

          chatRowChildren.Add(new NuiImage(chatLine.portrait) { Id = chatLine.playerName, Height = 24, Width = 19, ImageAspect = NuiAspect.ExactScaled });
          List<NuiDrawListItem> updatedDrawList = new List<NuiDrawListItem>();
          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = nameWidth, Id = chatLine.playerName, VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Left, ForegroundColor = new Color(143, 127, 255), DrawList = updatedDrawList });

          Color color = new Color(255, 255, 255);

          if (player.chatColors.ContainsKey((int)chatLine.channel))
          {
            byte[] colorArray = player.chatColors[(int)chatLine.channel];
            color = new Color(colorArray[0], colorArray[1], colorArray[2], colorArray[3]);
          }

          float textWidth;

          try
          {
            textWidth = (geometry.GetBindValue(player.oid, token).Width - 19 - nameWidth) * 0.9f;
          }
          catch(Exception)
          {
            NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) && player.windowRectangles[windowId].Width > 0 && player.windowRectangles[windowId].Width < player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);
            textWidth = (windowRectangle.Width - 19 - nameWidth) * 0.9f;
          }

          if (textWidth < 160)
            textWidth = 160;

          int nbLines = (int)(chatLine.text.Length * characterWidth / textWidth);
          string remainingText = chatLine.text;

          bool updatedText = chatLine.text.Contains("[modifié]");
          if (updatedText)
          {
            nbLines += 1;
            remainingText = remainingText.Replace("[modifié]", "");
          }

          NuiSpacer chatSpacer = new NuiSpacer() { Id = $"chat_{chatId}", Width = textWidth, Height = characterHeight };
          chatRowChildren.Add(chatSpacer);

          List<NuiDrawListItem> chatBreakerDrawList = new List<NuiDrawListItem>();
          chatSpacer.DrawList = chatBreakerDrawList;
          int nbCharPerLine = (int)(textWidth / characterWidth);
          int i = 0;
          string currentLine;

          do
          {
            currentLine = remainingText.Length > nbCharPerLine ? remainingText.Substring(0, nbCharPerLine) : remainingText;

            int breakPosition;

            if (remainingText.Length <= nbCharPerLine)
            {
              breakPosition = currentLine.Contains(Environment.NewLine) ? currentLine.IndexOf(Environment.NewLine) : currentLine.Length;
              currentLine = currentLine.Substring(0, breakPosition);
            }
            else
            {
              breakPosition = currentLine.Contains(" ") ? currentLine.LastIndexOf(" ") : currentLine.Length;
              int newLinePosition = currentLine.Contains(Environment.NewLine) ? currentLine.IndexOf(Environment.NewLine) : currentLine.Length;

              if (newLinePosition > 0 && newLinePosition < breakPosition)
                breakPosition = newLinePosition;

              currentLine = currentLine.Substring(0, breakPosition);
            }

            if (i == 0)
              chatBreakerDrawList.Add(new NuiDrawListText(color, new NuiRect(0, 2, textWidth, characterHeight), currentLine) { Fill = true });
            else
            {
              if (currentLine == "\n")
                currentLine = " ";

              NuiRow additionnalLines = new NuiRow()
              {
                Children = new List<NuiElement>()
                {
                  new NuiSpacer() { Width = 20 + nameWidth, Height = characterHeight },
                  new NuiSpacer() { Id = $"chat_{chatId}", Width = textWidth, Height = characterHeight,
                    DrawList = new List<NuiDrawListItem>()
                    {
                      new NuiDrawListText(color, new NuiRect(0, -10, textWidth, characterHeight), currentLine) { Fill = true }
                    }
                  }
                }
              };

              colChatLogChidren.Insert(1 + i, additionnalLines);
            }

            remainingText = remainingText.Remove(0, breakPosition);
            remainingText = remainingText.TrimStart();

            i++;
          } while (remainingText.Length > 1);

          if (updatedText)
            updatedDrawList.Add(new NuiDrawListText(new Color(200, 200, 200, 150), new NuiRect(0, characterHeight, 9 * characterWidth, characterHeight), "[modifié]") { Fill = true });
        }
      }
    }
  }
}
