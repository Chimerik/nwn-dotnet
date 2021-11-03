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
        NuiBind<string> writingChat { get; }
        NuiBind<int> channel { get; }
        NuiBind<bool> makeStatic { get; }
        NuiBind<bool> multiline { get; }
        NuiGroup chatWriterGroup { get; set; }
        NuiTextEdit textEdit { get; set; }

        public ChatWriterWindow(Player player, string windowId) : base(player, windowId)
        {
          writingChat = new NuiBind<string>("writingChat");
          channel = new NuiBind<int>("channel");
          makeStatic = new NuiBind<bool>("static");
          multiline = new NuiBind<bool>("multiLine");
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) && player.windowRectangles[windowId].Width > 0 && player.windowRectangles[windowId].Width <= player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          List<NuiComboEntry> comboValues = new List<NuiComboEntry>
          {
            new NuiComboEntry("Parler", 1),
            new NuiComboEntry("Chuchoter", 3),
            new NuiComboEntry("Groupe", 6),
            new NuiComboEntry("MD", 14),
            new NuiComboEntry("Crier", 2)
          };

          textEdit = new NuiTextEdit("", writingChat, 3000, true) { Id = "chatWriter", Height = (windowRectangle.Height - 160) * 0.96f, Width = windowRectangle.Width * 0.96f };

          chatWriterGroup = new NuiGroup
          {
            Id = "chatWriterGroup", Border = false, Padding = 0, Margin = 0,
            Children = new List<NuiElement> { new NuiRow { Children = new List<NuiElement> { textEdit } } }
          };

          NuiColumn col = new NuiColumn
          {
            Children = new List<NuiElement>
            {
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new NuiCombo
                  {
                    Entries = comboValues,
                    Selected = channel
                  },
                  new NuiSpacer(),
                  new NuiCheck("Figer", makeStatic) { Id = "fix", Tooltip = "Permet d'ancrer la fenêtre à l'écran", Width = 60 }
                }
              },
              chatWriterGroup,
              new NuiRow
              {
                Children = new List<NuiElement>
                {
                  new NuiSpacer(),
                  new NuiCheck("Multi-ligne", multiline) { Tooltip = "Si multi-ligne est coché, permet d'écrire un texte sur plusieurs lignes. Si la case est décochée, entrée envoie le texte."},
                  new NuiSpacer()
                }
              }
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

          player.oid.OnNuiEvent -= HandleChatWriterEvents;
          player.oid.OnNuiEvent += HandleChatWriterEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          writingChat.SetBindValue(player.oid, token, player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_CURRENT_CHAT").Value);
          channel.SetBindValue(player.oid, token, 0);
          makeStatic.SetBindValue(player.oid, token, false);
          resizable.SetBindValue(player.oid, token, true);
          closable.SetBindValue(player.oid, token, true);
          multiline.SetBindValue(player.oid, token, false);

          writingChat.SetBindWatch(player.oid, token, true);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void HandleChatWriterEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "chat")
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

              break;

            case "chatWriter":

              switch (nuiEvent.EventType)
              {
                case NuiEventType.Focus:

                  int channelValue = channel.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
                  string command = writingChat.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
                  Effect visualMark;

                  if ((channelValue == 1 || channelValue == 3) && !(command.Trim().StartsWith("/") || command.Trim().StartsWith("!") || command.Trim().StartsWith("(")))
                  {
                    visualMark = Effect.VisualEffect((VfxType)1248);
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").Delete();
                  }
                  else
                  {
                    visualMark = Effect.VisualEffect((VfxType)1249);
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").Value = true;
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

              string chatText = writingChat.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

              if (chatText.Length < 1)
                return;

              int chatChannel = channel.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

              if ((chatChannel == 1 || chatChannel == 3) && !(chatText.Trim().StartsWith("/") || chatText.Trim().StartsWith("!") || chatText.Trim().StartsWith("(")))
              {
                if (player.oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").HasValue)
                {
                  foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
                    nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

                  Effect visualMark = Effect.VisualEffect((VfxType)1248);
                  visualMark.Tag = "VFX_SPEAKING_MARK";
                  visualMark.SubType = EffectSubType.Supernatural;
                  nuiEvent.Player.ControlledCreature.ApplyEffect(EffectDuration.Permanent, visualMark);

                  player.oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").Delete();
                }
              }
              else
              {
                if (player.oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").HasNothing)
                {
                  foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
                    nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

                  Effect visualMark = Effect.VisualEffect((VfxType)1249);
                  visualMark.Tag = "VFX_SPEAKING_MARK";
                  visualMark.SubType = EffectSubType.Supernatural;
                  nuiEvent.Player.ControlledCreature.ApplyEffect(EffectDuration.Permanent, visualMark);

                  player.oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").Value = true;
                }
              }

              player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_CURRENT_CHAT").Value = chatText;

              if (multiline.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken))
                return;

              if (chatText.Substring(chatText.Length - 1).Contains(Environment.NewLine))
              {
                chatText = chatText.Substring(0, chatText.Length - 1);

                ChatWriterSendMessage(chatText, channel.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
                player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_CURRENT_CHAT").Delete();

                foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
                  nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

                writingChat.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
                writingChat.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, "");
                writingChat.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);
              }

              break;

            case "geometry":

              NuiRect rectangle = geometry.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

              if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

              geometry.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
              textEdit.Width = rectangle.Width * 0.96f;
              nuiEvent.Player.NuiSetGroupLayout(nuiEvent.WindowToken, chatWriterGroup.Id, chatWriterGroup);

              geometry.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);
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
            switch (message.Substring(0, message.IndexOf(" ")))
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
