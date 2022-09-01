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
      public class ChatWriterWindow : PlayerWindow
      {
        private readonly NuiBind<string> writingChat = new ("writingChat");
        private readonly NuiBind<int> channel = new ("channel");
        private readonly NuiBind<int> language = new ("language");
        private readonly NuiBind<bool> makeStatic = new ("static");
        NuiGroup chatWriterGroup { get; }
        NuiRow rootRow { get; }
        NuiTextEdit textEdit { get; }
        bool isChatHRP { get; set; }

        public ChatWriterWindow(Player player) : base(player)
        {
          windowId = "chat";
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) && player.windowRectangles[windowId].Width > 0 && player.windowRectangles[windowId].Width <= player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          List<NuiComboEntry> comboValues = new List<NuiComboEntry>
          {
            new NuiComboEntry("Parler", 1),
            new NuiComboEntry("Chuchoter", 3),
            new NuiComboEntry("Groupe", 6),
            new NuiComboEntry("MD", 14),
            new NuiComboEntry("Crier", 2)
          };

          List<NuiComboEntry> languageValues = new();

          languageValues.Add(new NuiComboEntry("Commun", 0));

          foreach (var language in player.learnableSkills.Values)
            if(language.category == SkillSystem.Category.Language && language.currentLevel > 0)
              languageValues.Add(new NuiComboEntry(language.name, language.id));

          textEdit = new NuiTextEdit("", writingChat, 3000, true) { Id = "chatWriter", Height = (windowRectangle.Height - 160) * 0.96f, Width = windowRectangle.Width * 0.96f };

          rootRow = new NuiRow { Children = new List<NuiElement> { textEdit } };

          chatWriterGroup = new NuiGroup { Id = "chatWriterGroup", Border = false, Padding = 0, Margin = 0, Layout = rootRow };

          NuiColumn col = new NuiColumn
          {
            Children = new List<NuiElement>
            {
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new NuiCombo { Entries = comboValues, Selected = channel },
                  new NuiSpacer(),
                  new NuiCombo { Entries = languageValues, Selected = language },
                  new NuiSpacer(),
                  new NuiCheck("Figer", makeStatic) { Id = "fix", Tooltip = "Permet d'ancrer la fenêtre à l'écran", Width = 60 }
                }
              },
              chatWriterGroup,
            }
          };

          window = new NuiWindow(col, "")
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
            nuiToken.OnNuiEvent += HandleChatWriterEvents;

            writingChat.SetBindValue(player.oid, nuiToken.Token, writingChat.GetBindValue(player.oid, nuiToken.Token));
            channel.SetBindValue(player.oid, nuiToken.Token, 0);
            language.SetBindValue(player.oid, nuiToken.Token, 0);
            language.SetBindWatch(player.oid, nuiToken.Token, true);
            makeStatic.SetBindValue(player.oid, nuiToken.Token, false);
            resizable.SetBindValue(player.oid, nuiToken.Token, true);
            closable.SetBindValue(player.oid, nuiToken.Token, true);
            writingChat.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleChatWriterEvents(ModuleEvents.OnNuiEvent nuiEvent)
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

              break;

            case "chatWriter":

              switch (nuiEvent.EventType)
              {
                case NuiEventType.Focus:

                  int channelValue = channel.GetBindValue(nuiEvent.Player, nuiToken.Token);
                  string command = writingChat.GetBindValue(nuiEvent.Player, nuiToken.Token);
                  Effect visualMark;

                  if ((channelValue == 1 || channelValue == 3) && !(command.Trim().StartsWith("/") || command.Trim().StartsWith("!") || command.Trim().StartsWith("(")))
                  {
                    visualMark = Effect.VisualEffect((VfxType)1248);
                    isChatHRP = false;
                  }
                  else
                  {
                    visualMark = Effect.VisualEffect((VfxType)1249);
                    isChatHRP = true;
                  }

                  visualMark.Tag = "VFX_SPEAKING_MARK";
                  visualMark.SubType = EffectSubType.Supernatural;
                  nuiEvent.Player.ControlledCreature.ApplyEffect(EffectDuration.Permanent, visualMark);

                  break;

                case NuiEventType.Blur:
                  foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
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

              int chatChannel = channel.GetBindValue(nuiEvent.Player, nuiToken.Token);

              if ((chatChannel == 1 || chatChannel == 3) && !(chatText.Trim().StartsWith("/") || chatText.Trim().StartsWith("!") || chatText.Trim().StartsWith("(")))
              {
                if (isChatHRP)
                {
                  foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
                    nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

                  Effect visualMark = Effect.VisualEffect((VfxType)1248);
                  visualMark.Tag = "VFX_SPEAKING_MARK";
                  visualMark.SubType = EffectSubType.Supernatural;
                  nuiEvent.Player.ControlledCreature.ApplyEffect(EffectDuration.Permanent, visualMark);

                  isChatHRP = false;
                }
              }
              else
              {
                if (!isChatHRP)
                {
                  foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
                    nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

                  Effect visualMark = Effect.VisualEffect((VfxType)1249);
                  visualMark.Tag = "VFX_SPEAKING_MARK";
                  visualMark.SubType = EffectSubType.Supernatural;
                  nuiEvent.Player.ControlledCreature.ApplyEffect(EffectDuration.Permanent, visualMark);

                  isChatHRP = true;
                }
              }

              if (chatText.Contains(Environment.NewLine))
              {
                int pos = 0;

                foreach(char c in chatText)
                {
                  if (c == '\n' && pos != 0  && chatText[pos - 1] != ' ')
                  {
                    chatText = chatText.Remove(pos, 1);

                    ChatWriterSendMessage(chatText, channel.GetBindValue(nuiEvent.Player, nuiToken.Token));

                    foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
                      nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

                    writingChat.SetBindWatch(nuiEvent.Player, nuiToken.Token, false);
                    writingChat.SetBindValue(nuiEvent.Player, nuiToken.Token, "");
                    writingChat.SetBindWatch(nuiEvent.Player, nuiToken.Token, true);
                  }

                  pos++;
                }
              }

              break;

            case "geometry":

              NuiRect rectangle = geometry.GetBindValue(nuiEvent.Player, nuiToken.Token);

              if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

              geometry.SetBindWatch(nuiEvent.Player, nuiToken.Token, false);
              textEdit.Width = rectangle.Width * 0.96f;
              chatWriterGroup.SetLayout(player.oid, nuiToken.Token, rootRow);
              geometry.SetBindWatch(nuiEvent.Player, nuiToken.Token, true);

              break;

            case "language":
              if (nuiEvent.EventType == NuiEventType.Watch)
                player.currentLanguage = language.GetBindValue(player.oid, nuiToken.Token);
              break;
          }
        }
        private void ChatWriterSendMessage(string message, int channel)
        {
          if (message.Length < 1)
            return;

          int iChannel = 1;
          NwPlayer target = null;
          string messageTarget = "";

          if (!message.StartsWith("/"))
            iChannel = channel;
          else
          {
            switch (message[..message.IndexOf(" ")])
            {
              case "/tk":
                iChannel = 1;
                message = message.Replace("/tk", "");
                break;
              case "/s":
                iChannel = 2;
                message = message.Replace("/s", "");
                break;
              case "/w":
                iChannel = 3;
                message = message.Replace("/<", "");
                break;
              case "/p":
                iChannel = 6;
                message = message.Replace("/p", "");
                break;
              case "/dm":
                iChannel = 14;
                message = message.Replace("/dm", "");
                break;
              case "/t":
              case "/tp":
                iChannel = 4;

                try
                {
                  messageTarget = message.Split('"', 2)[1];
                  target = NwModule.Instance.Players.FirstOrDefault(p => p.PlayerName == messageTarget || p.LoginCreature.Name == messageTarget || p.ControlledCreature.Name == messageTarget);
                  message = message.Remove(0, message.IndexOf('"', message.IndexOf('"') + 1) + 1);
                }
                catch (Exception)
                {
                  messageTarget = "error";
                  player.oid.SendServerMessage("Commande incorrectement formée. Impossible d'envoyer le message", ColorConstants.Red);
                }

                break;
            }
          }

          if (player.oid.IsDM)
            iChannel += 16;

          if (messageTarget != "error")
            ChatSystem.chatService.SendMessage((ChatChannel)iChannel, message, player.oid.ControlledCreature, target);
        }
      }
    }
  }
}
