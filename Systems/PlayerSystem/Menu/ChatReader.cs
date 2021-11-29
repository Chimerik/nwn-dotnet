using System;
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
      public class ChatReaderWindow : PlayerWindow
      {
        NuiBind<bool> makeStatic { get; }
        float characterWidth = 8 /* oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100*/;
        float spaceWidth = 4;
        float characterHeight = 18 /* oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100*/;

        NuiGroup chatReaderGroup { get; }
        NuiColumn rootColumn { get; }
        NuiColumn colChatLog { get; }
        List<NuiElement> colChatLogChidren { get; }
        NuiOptions rpCategory { get; }
        NuiOptions hrpCategory { get; }
        NuiOptions mpCategory { get; }
        List<NuiRow> rpRow { get; }
        List<NuiRow> hrpRow { get; }
        List<NuiRow> mpRow { get; }
        NuiRow settingsRow { get; }
        ChatLine.ChatCategory currentCategory { get; set; }

        public ChatReaderWindow(Player player) : base(player)
        {
          windowId = "chatReader";
          makeStatic = new NuiBind<bool>("static");

          List<NuiElement> colChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = colChidren };

          colChatLogChidren = new List<NuiElement>();
          colChatLog = new NuiColumn() { Children = colChatLogChidren };

          chatReaderGroup = new NuiGroup() { Id = "chatReaderGroup", Border = false, Layout = colChatLog };
          colChidren.Add(chatReaderGroup);

          List<NuiElement> settingsRowChildren = new List<NuiElement>();
          settingsRow = new NuiRow() { Children = settingsRowChildren };
          colChatLogChidren.Add(settingsRow);

          settingsRowChildren.Add(new NuiCheck("Figer", makeStatic) { Id = "fix", Tooltip = "Permet d'ancrer la fenêtre à l'écran", Width = 60 });

          rpCategory = new NuiOptions() { Id = "rpCategory", Tooltip = "Affiche le canal roleplay", Direction = NuiDirection.Horizontal, Options = { "RP" }, ForegroundColor = new NuiColor(142, 146, 151), Width = 60, Height = characterHeight, Margin = 7, Selection = 0 };
          hrpCategory = new NuiOptions() { Id = "hrpCategory", Tooltip = "Affiche le canal hors roleplay", Direction = NuiDirection.Horizontal, Options = { "HRP" }, ForegroundColor = new NuiColor(142, 146, 151), Width = 60, Height = characterHeight, Margin = 7 };
          mpCategory = new NuiOptions() { Id = "mpCategory", Tooltip = "Affiche le canal roleplay", Direction = NuiDirection.Horizontal, Options = { "MP" }, ForegroundColor = new NuiColor(142, 146, 151), Width = 60, Height = characterHeight, Margin = 7 };

          settingsRowChildren.Add(rpCategory);
          settingsRowChildren.Add(hrpCategory);
          settingsRowChildren.Add(mpCategory);

          rpRow = new List<NuiRow>();
          hrpRow = new List<NuiRow>();
          mpRow = new List<NuiRow>();
          currentCategory = ChatLine.ChatCategory.RolePlay;

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

          player.oid.OnNuiEvent -= HandleChatReaderEvents;
          player.oid.OnNuiEvent += HandleChatReaderEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          makeStatic.SetBindValue(player.oid, token, false);
          resizable.SetBindValue(player.oid, token, true);
          closable.SetBindValue(player.oid, token, true);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }

        private void HandleChatReaderEvents(ModuleEvents.OnNuiEvent nuiEvent)
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

              NuiProperty<NuiColor> rpPreviousColor = rpCategory.ForegroundColor;
              NuiProperty<NuiColor> hrpPreviousColor = hrpCategory.ForegroundColor;
              NuiProperty<NuiColor> mpPreviousColor = mpCategory.ForegroundColor;

              colChatLogChidren.Clear();
              colChatLogChidren.Add(settingsRow);

              if (currentCategory == ChatLine.ChatCategory.Private)
                CreatePMRows();
              else
                CreateChatRows();

              rpCategory.ForegroundColor = rpPreviousColor;
              hrpCategory.ForegroundColor = hrpPreviousColor;
              mpCategory.ForegroundColor = mpPreviousColor;

              chatReaderGroup.SetLayout(player.oid, token, colChatLog);

              return;

            case "rpCategory":
              if (nuiEvent.EventType == NuiEventType.MouseDown && currentCategory != ChatLine.ChatCategory.RolePlay)
              {
                currentCategory = ChatLine.ChatCategory.RolePlay;
                
                rpCategory.Selection = 0;
                hrpCategory.Selection = 1;
                mpCategory.Selection = 1;

                colChatLogChidren.Clear();
                colChatLogChidren.Add(settingsRow);

                NuiProperty<NuiColor> previousHrp = hrpCategory.ForegroundColor;
                NuiProperty<NuiColor> previousMP = mpCategory.ForegroundColor;

                CreateChatRows();

                rpCategory.ForegroundColor = new NuiColor(142, 146, 151);
                hrpCategory.ForegroundColor = previousHrp;
                mpCategory.ForegroundColor = previousMP;

                chatReaderGroup.SetLayout(player.oid, token, colChatLog);
              }
              return;

            case "hrpCategory":
              if (nuiEvent.EventType == NuiEventType.MouseDown && currentCategory != ChatLine.ChatCategory.HorsRolePlay)
              {
                currentCategory = ChatLine.ChatCategory.HorsRolePlay;
                
                hrpCategory.Selection = 0;
                rpCategory.Selection = 1;
                mpCategory.Selection = 1;

                NuiProperty<NuiColor> previousRp = rpCategory.ForegroundColor;
                NuiProperty<NuiColor> previousMP = mpCategory.ForegroundColor;

                colChatLogChidren.Clear();
                colChatLogChidren.Add(settingsRow);

                CreateChatRows();

                hrpCategory.ForegroundColor = new NuiColor(142, 146, 151);
                rpCategory.ForegroundColor = previousRp;
                mpCategory.ForegroundColor = previousMP;

                chatReaderGroup.SetLayout(player.oid, token, colChatLog);
              }
              return;

            case "mpCategory":

              if (nuiEvent.EventType == NuiEventType.MouseDown && currentCategory != ChatLine.ChatCategory.Private)
              {
                currentCategory = ChatLine.ChatCategory.Private;

                mpCategory.Selection = 0;
                rpCategory.Selection = 1;
                hrpCategory.Selection = 1;

                NuiProperty<NuiColor> previousRp = rpCategory.ForegroundColor;
                NuiProperty<NuiColor> previousHrp = hrpCategory.ForegroundColor;

                colChatLogChidren.Clear();
                colChatLogChidren.Add(settingsRow);

                CreatePMRows();

                mpCategory.ForegroundColor = new NuiColor(142, 146, 151);
                rpCategory.ForegroundColor = previousRp;
                hrpCategory.ForegroundColor = previousHrp;

                chatReaderGroup.SetLayout(player.oid, token, colChatLog);
              }

              return;
          }

          if (nuiEvent.ElementId.StartsWith("chat_"))
          {
            if (nuiEvent.EventType != NuiEventType.MouseDown)
              return;

            int chatLineId = int.Parse(nuiEvent.ElementId.Remove(0, 5));
            ChatLine chatLine = player.readChatLines[chatLineId];

            if (chatLine.playerName == nuiEvent.Player.PlayerName)
              player.CreateChatLineEditorWindow(chatLine.text, chatLineId);
            else if (chatLine.textHistory.Count > 1)
              player.CreateChatLineHistoryWindow(chatLine.textHistory);
          }
          else if (nuiEvent.ElementId.StartsWith("pm_"))
          {
            if (nuiEvent.EventType != NuiEventType.MouseDown)
              return;

            string targetName = nuiEvent.ElementId.Remove(0, 3);

            if (player.windows.ContainsKey(targetName))
            {
              ((PrivateMessageWindow)player.windows[targetName]).CreateWindow();
              ((NuiRow)colChatLogChidren[1]).Children.FirstOrDefault(c => c.Id == nuiEvent.ElementId && c is NuiLabel).ForegroundColor = new NuiColor(142, 146, 151);
              chatReaderGroup.SetLayout(player.oid, token, colChatLog);
            }
            else
            {
              NwPlayer target = NwModule.Instance.Players.FirstOrDefault(p => p.PlayerName == nuiEvent.ElementId.Remove(0, 3));

              if (target != null)
              {
                player.windows.Add(target.PlayerName, new PrivateMessageWindow(player, target));
                ((NuiRow)colChatLogChidren[1]).Children.FirstOrDefault(c => c.Id == nuiEvent.ElementId && c is NuiLabel).ForegroundColor = new NuiColor(142, 146, 151);
                chatReaderGroup.SetLayout(player.oid, token, colChatLog);
              }
              else
                player.oid.SendServerMessage($"Le joueur  {nuiEvent.ElementId.Remove(0, 3).ColorString(ColorConstants.White)} n'est pas connecté. Impossible d'ouvrir un canal privé.", ColorConstants.Red);
            }
          }
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

          if (chatLine.category == ChatLine.ChatCategory.RolePlay)
            rpRow.Add(chatRow);
          else
            hrpRow.Add(chatRow);

          if (chatLine.category == currentCategory)
            colChatLogChidren.Insert(1, chatRow);
          else
          {
            NuiColor white = new NuiColor(255, 255, 255);
            switch (chatLine.category)
            {
              case ChatLine.ChatCategory.RolePlay:
                rpCategory.ForegroundColor = white;
                break;
              case ChatLine.ChatCategory.HorsRolePlay:
                hrpCategory.ForegroundColor = white;
                break;
              case ChatLine.ChatCategory.Private:
                mpCategory.ForegroundColor = white;
                break;
            }

            return;
          }

          chatRowChildren.Add(new NuiImage(chatLine.portrait) { Id = chatLine.playerName, Height = 24, Width = 19, ImageAspect = NuiAspect.ExactScaled });
          List<NuiDrawListItem> updatedDrawList = new List<NuiDrawListItem>();
          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = nameWidth, Id = chatLine.playerName, VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Left, ForegroundColor = new NuiColor(143, 127, 255), DrawList = updatedDrawList });
          NuiColor color = player.chatColors.ContainsKey(chatLine.channel) ? player.chatColors[chatLine.channel] : new NuiColor(255, 255, 255);

          if (chatLine.channel == Anvil.Services.ChatChannel.PlayerTell || chatLine.channel == Anvil.Services.ChatChannel.DmTell)
            color = new NuiColor(32, 255, 32);

          float textWidth;

          try
          {
            textWidth = (geometry.GetBindValue(player.oid, token).Width - 19 - nameWidth) * 0.9f;
          }
          catch (Exception)
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
              breakPosition = currentLine.Contains(Environment.NewLine) ? currentLine.IndexOf(Environment.NewLine) : currentLine.Length ;
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
            updatedDrawList.Add(new NuiDrawListText(new NuiColor(200, 200, 200, 150), new NuiRect(0, characterHeight, 9 * characterWidth, characterHeight), "[modifié]") { Fill = true });
        }

        private void CreatePMRows()
        {
          if (player.readChatLines.Where(c => c.category == ChatLine.ChatCategory.Private).Count() < 1)
            return;

          List<ChatLine> pmSenders = new List<ChatLine>();

          foreach (var target in player.readChatLines.Where(c => c.category == ChatLine.ChatCategory.Private).GroupBy(p => p.playerName))
            if(!pmSenders.Any(c => c.receiverPlayerName == target.FirstOrDefault().playerName))
              pmSenders.Add(target.FirstOrDefault());

          foreach(ChatLine pmSender in pmSenders)
          {
            string playerDisplayName = pmSender.playerName != player.oid.PlayerName ? pmSender.playerName : pmSender.receiverPlayerName;
            string playerDisplayPortrait = pmSender.playerName != player.oid.PlayerName ? pmSender.portrait : pmSender.receiverPortrait;

            colChatLogChidren.Add(new NuiRow()
            {
              Children = new List<NuiElement>()
              {
                new NuiImage(playerDisplayPortrait) { Id = $"pm_{playerDisplayName}", Height = 24, Width = 19, ImageAspect = NuiAspect.ExactScaled },
                new NuiLabel(playerDisplayName) { Id = $"pm_{playerDisplayName}", VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Left, ForegroundColor = player.windows.ContainsKey(playerDisplayName) && ((PrivateMessageWindow)player.windows[playerDisplayName]).read == true ? new NuiColor(142, 146, 151) : new NuiColor(143, 127, 255) }
              }
            });
          }
        }

        public void HandleNewPM(string senderName)
        {
          if(player.windows.ContainsKey(senderName) && !player.openedWindows.ContainsKey(senderName))
            ((PrivateMessageWindow)player.windows[senderName]).read = false;

          if (currentCategory != ChatLine.ChatCategory.Private)
            mpCategory.ForegroundColor = new NuiColor(255, 255, 255);
          else
          {
            colChatLogChidren.Clear();
            colChatLogChidren.Add(settingsRow);
            CreatePMRows();
          }

          chatReaderGroup.SetLayout(player.oid, token, colChatLog);
        }
      }
    }
  }
}
