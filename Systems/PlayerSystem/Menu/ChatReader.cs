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
        private readonly NuiBind<bool> makeStatic = new("static");
        private readonly float characterWidth = 8 /* oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100*/;
        private readonly float spaceWidth = 4;
        private readonly float characterHeight = 18 /* oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) / 100*/;

        NuiGroup chatReaderGroup { get; }
        NuiColumn rootColumn { get; }
        NuiColumn colChatLog { get; }
        List<NuiElement> colChatLogChidren { get; }
        NuiOptions rpCategory { get; }
        NuiOptions hrpCategory { get; }
        NuiOptions mpCategory { get; }
        private readonly List<NuiRow> rpRow = new();
        private readonly List<NuiRow> hrpRow = new();
        private readonly List<NuiRow> mpRow = new();
        NuiRow settingsRow { get; }
        ChatLine.ChatCategory currentCategory { get; set; }

        public ChatReaderWindow(Player player) : base(player)
        {
          windowId = "chatReader";

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

          rpCategory = new NuiOptions() { Id = "rpCategory", Tooltip = "Affiche le canal roleplay", Direction = NuiDirection.Horizontal, Options = { "RP" }, ForegroundColor = new Color(142, 146, 151), Width = 60, Height = characterHeight, Margin = 7, Selection = 0 };
          hrpCategory = new NuiOptions() { Id = "hrpCategory", Tooltip = "Affiche le canal hors roleplay", Direction = NuiDirection.Horizontal, Options = { "HRP" }, ForegroundColor = new Color(142, 146, 151), Width = 60, Height = characterHeight, Margin = 7 };
          mpCategory = new NuiOptions() { Id = "mpCategory", Tooltip = "Affiche le canal roleplay", Direction = NuiDirection.Horizontal, Options = { "MP" }, ForegroundColor = new Color(142, 146, 151), Width = 60, Height = characterHeight, Margin = 7 };

          settingsRowChildren.Add(rpCategory);
          settingsRowChildren.Add(hrpCategory);
          settingsRowChildren.Add(mpCategory);

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

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleChatReaderEvents;

            makeStatic.SetBindValue(player.oid, nuiToken.Token, false);
            resizable.SetBindValue(player.oid, nuiToken.Token, true);
            closable.SetBindValue(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandleChatReaderEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.ElementId)
          {
            case "fix":

              switch (makeStatic.GetBindValue(nuiEvent.Player, nuiToken.Token))
              {
                case false:
                  closable.SetBindValue(nuiEvent.Player, nuiToken.Token, true);
                  resizable.SetBindValue(nuiEvent.Player, nuiToken.Token, true);
                  break;

                case true:
                  closable.SetBindValue(nuiEvent.Player, nuiToken.Token, false);
                  resizable.SetBindValue(nuiEvent.Player, nuiToken.Token, false);
                  break;
              }

              return;

            case "geometry":

              if (!player.openedWindows.ContainsKey(windowId))
                return;

              NuiRect rectangle = geometry.GetBindValue(nuiEvent.Player, nuiToken.Token);

              if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

              NuiProperty<Color> rpPreviousColor = rpCategory.ForegroundColor;
              NuiProperty<Color> hrpPreviousColor = hrpCategory.ForegroundColor;
              NuiProperty<Color> mpPreviousColor = mpCategory.ForegroundColor;

              colChatLogChidren.Clear();
              colChatLogChidren.Add(settingsRow);

              if (currentCategory == ChatLine.ChatCategory.Private)
                CreatePMRows();
              else
                CreateChatRows();

              rpCategory.ForegroundColor = rpPreviousColor;
              hrpCategory.ForegroundColor = hrpPreviousColor;
              mpCategory.ForegroundColor = mpPreviousColor;

              chatReaderGroup.SetLayout(player.oid, nuiToken.Token, colChatLog);

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

                NuiProperty<Color> previousHrp = hrpCategory.ForegroundColor;
                NuiProperty<Color> previousMP = mpCategory.ForegroundColor;

                CreateChatRows();

                rpCategory.ForegroundColor = new Color(142, 146, 151);
                hrpCategory.ForegroundColor = previousHrp;
                mpCategory.ForegroundColor = previousMP;

                chatReaderGroup.SetLayout(player.oid, nuiToken.Token, colChatLog);
              }
              return;

            case "hrpCategory":
              if (nuiEvent.EventType == NuiEventType.MouseDown && currentCategory != ChatLine.ChatCategory.HorsRolePlay)
              {
                currentCategory = ChatLine.ChatCategory.HorsRolePlay;

                hrpCategory.Selection = 0;
                rpCategory.Selection = 1;
                mpCategory.Selection = 1;

                NuiProperty<Color> previousRp = rpCategory.ForegroundColor;
                NuiProperty<Color> previousMP = mpCategory.ForegroundColor;

                colChatLogChidren.Clear();
                colChatLogChidren.Add(settingsRow);

                CreateChatRows();

                hrpCategory.ForegroundColor = new Color(142, 146, 151);
                rpCategory.ForegroundColor = previousRp;
                mpCategory.ForegroundColor = previousMP;

                chatReaderGroup.SetLayout(player.oid, nuiToken.Token, colChatLog);
              }
              return;

            case "mpCategory":

              if (nuiEvent.EventType == NuiEventType.MouseDown && currentCategory != ChatLine.ChatCategory.Private)
              {
                currentCategory = ChatLine.ChatCategory.Private;

                mpCategory.Selection = 0;
                rpCategory.Selection = 1;
                hrpCategory.Selection = 1;

                NuiProperty<Color> previousRp = rpCategory.ForegroundColor;
                NuiProperty<Color> previousHrp = hrpCategory.ForegroundColor;

                colChatLogChidren.Clear();
                colChatLogChidren.Add(settingsRow);

                CreatePMRows();

                mpCategory.ForegroundColor = new Color(142, 146, 151);
                rpCategory.ForegroundColor = previousRp;
                hrpCategory.ForegroundColor = previousHrp;

                chatReaderGroup.SetLayout(player.oid, nuiToken.Token, colChatLog);
              }

              return;
          }

          if (nuiEvent.ElementId.StartsWith("chat_"))
          {
            if (nuiEvent.EventType != NuiEventType.MouseDown)
              return;

            ChatLine chatLine = player.readChatLines[int.Parse(nuiEvent.ElementId.Remove(0, 5))];

            if (chatLine.playerName == nuiEvent.Player.PlayerName)
            {
              if (player.windows.ContainsKey("chatLineEditor"))
                ((ChatLineEditorWindow)player.windows["chatLineEditor"]).CreateWindow();
              else
                player.windows.Add("chatLineEditor", new ChatLineEditorWindow(player, chatLine));
            }
            else if (chatLine.textHistory.Count > 1)
            {
              if (player.windows.ContainsKey("chatLineHistory"))
                ((ChatLineHistoryWindow)player.windows["chatLineHistory"]).CreateWindow();
              else
                player.windows.Add("chatLineHistory", new ChatLineHistoryWindow(player, chatLine));
            }
          }
          else if (nuiEvent.ElementId.StartsWith("pm_"))
          {
            if (nuiEvent.EventType != NuiEventType.MouseDown)
              return;

            string targetName = nuiEvent.ElementId.Remove(0, 3);

            if (player.windows.ContainsKey(targetName))
            {
              ((PrivateMessageWindow)player.windows[targetName]).CreateWindow();
              ((NuiRow)colChatLogChidren[1]).Children.FirstOrDefault(c => c.Id == nuiEvent.ElementId && c is NuiLabel).ForegroundColor = new Color(142, 146, 151);
              chatReaderGroup.SetLayout(player.oid, nuiToken.Token, colChatLog);
            }
            else
            {
              NwPlayer target = NwModule.Instance.Players.FirstOrDefault(p => p.PlayerName == nuiEvent.ElementId.Remove(0, 3));

              if (target != null)
              {
                player.windows.Add(target.PlayerName, new PrivateMessageWindow(player, target));
                ((NuiRow)colChatLogChidren[1]).Children.FirstOrDefault(c => c.Id == nuiEvent.ElementId && c is NuiLabel).ForegroundColor = new Color(142, 146, 151);
                chatReaderGroup.SetLayout(player.oid, nuiToken.Token, colChatLog);
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
          chatReaderGroup.SetLayout(player.oid, nuiToken.Token, colChatLog);
        }

        public void UpdateChat()
        {
          colChatLogChidren.Clear();
          colChatLogChidren.Add(settingsRow);

          CreateChatRows();

          chatReaderGroup.SetLayout(player.oid, nuiToken.Token, colChatLog);
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
            Color white = new Color(255, 255, 255);
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
          chatRowChildren.Add(new NuiLabel(chatLine.name) { Width = nameWidth, Id = chatLine.playerName, VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Left, ForegroundColor = new Color(143, 127, 255), DrawList = updatedDrawList });

          Color color = new Color(255, 255, 255);

          if (player.chatColors.ContainsKey((int)chatLine.channel))
          {
            byte[] colorArray = player.chatColors[(int)chatLine.channel];
            color = new Color(colorArray[0], colorArray[1], colorArray[2], colorArray[3]);
          }

          if (chatLine.channel == Anvil.Services.ChatChannel.PlayerTell || chatLine.channel == Anvil.Services.ChatChannel.DmTell)
            color = new Color(32, 255, 32);

          float textWidth;

          try
          {
            textWidth = (geometry.GetBindValue(player.oid, nuiToken.Token).Width - 19 - nameWidth) * 0.9f;
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
            currentLine = remainingText.Length > nbCharPerLine ? remainingText[..nbCharPerLine] : remainingText;

            int breakPosition;

            if (remainingText.Length <= nbCharPerLine)
            {
              breakPosition = currentLine.Contains(Environment.NewLine) ? currentLine.IndexOf(Environment.NewLine) : currentLine.Length;
              currentLine = currentLine[..breakPosition];
            }
            else
            {
              breakPosition = currentLine.Contains(' ') ? currentLine.LastIndexOf(" ") : currentLine.Length;
              int newLinePosition = currentLine.Contains(Environment.NewLine) ? currentLine.IndexOf(Environment.NewLine) : currentLine.Length;

              if (newLinePosition > 0 && newLinePosition < breakPosition)
                breakPosition = newLinePosition;

              currentLine = currentLine[..breakPosition];
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

        private void CreatePMRows()
        {
          if (!player.readChatLines.Where(c => c.category == ChatLine.ChatCategory.Private).Any())
            return;

          List<ChatLine> pmSenders = new List<ChatLine>();

          foreach (var target in player.readChatLines.Where(c => c.category == ChatLine.ChatCategory.Private).GroupBy(p => p.playerName))
            if (!pmSenders.Any(c => c.receiverPlayerName == target.FirstOrDefault().playerName))
              pmSenders.Add(target.FirstOrDefault());

          foreach (ChatLine pmSender in pmSenders)
          {
            string playerDisplayName = pmSender.playerName != player.oid.PlayerName ? pmSender.playerName : pmSender.receiverPlayerName;
            string playerDisplayPortrait = pmSender.playerName != player.oid.PlayerName ? pmSender.portrait : pmSender.receiverPortrait;

            colChatLogChidren.Add(new NuiRow()
            {
              Children = new List<NuiElement>()
              {
                new NuiImage(playerDisplayPortrait) { Id = $"pm_{playerDisplayName}", Height = 24, Width = 19, ImageAspect = NuiAspect.ExactScaled },
                new NuiLabel(playerDisplayName) { Id = $"pm_{playerDisplayName}", VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Left, ForegroundColor = player.windows.ContainsKey(playerDisplayName) && ((PrivateMessageWindow)player.windows[playerDisplayName]).read == true ? new Color(142, 146, 151) : new Color(143, 127, 255) }
              }
            });
          }
        }

        public void HandleNewPM(string senderName)
        {
          if (player.windows.ContainsKey(senderName) && !player.openedWindows.ContainsKey(senderName))
            ((PrivateMessageWindow)player.windows[senderName]).read = false;

          if (currentCategory != ChatLine.ChatCategory.Private)
            mpCategory.ForegroundColor = new Color(255, 255, 255);
          else
          {
            colChatLogChidren.Clear();
            colChatLogChidren.Add(settingsRow);
            CreatePMRows();
          }

          chatReaderGroup.SetLayout(player.oid, nuiToken.Token, colChatLog);
        }
      }
    }
  }
}
