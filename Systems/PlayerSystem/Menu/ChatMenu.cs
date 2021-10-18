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
      public void CreateChatWindow()
      {
        string windowId = "chat";
        NuiBind<string> writingChat = new NuiBind<string>("writingChat");
        NuiBind<string> receivedChat = new NuiBind<string>("receivedChat");
        NuiBind<int> channel = new NuiBind<int>("channel");
        NuiBind<bool> closable = new NuiBind<bool>("closable");
        NuiBind<bool> resizable = new NuiBind<bool>("resizable");
        NuiBind<bool> makeStatic = new NuiBind<bool>("static");
        NuiBind<bool> multiline = new NuiBind<bool>("multiLine");

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 && windowRectangles[windowId].Width <= oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        List<NuiComboEntry> comboValues = new List<NuiComboEntry>
          {
            new NuiComboEntry("Parler", 1),
            new NuiComboEntry("Chuchoter", 3),
            new NuiComboEntry("Groupe", 6),
            new NuiComboEntry("MD", 14),
            new NuiComboEntry("Crier", 2)
          };

        NuiWindow window = new NuiWindow(BuildChatWriterWindow(windowRectangle), "")
        {
          Geometry = geometry,
          Resizable = resizable,
          Collapsed = false,
          Closable = closable,
          Transparent = true,
          Border = false,
        };

        oid.OnNuiEvent -= HandleChatWriterEvents;
        oid.OnNuiEvent += HandleChatWriterEvents;

        int token = oid.CreateNuiWindow(window, windowId);

        receivedChat.SetBindValue(oid, token, "");
        writingChat.SetBindValue(oid, token, oid.LoginCreature.GetObjectVariable<LocalVariableString>("_CURRENT_CHAT").Value);
        channel.SetBindValue(oid, token, 0);
        makeStatic.SetBindValue(oid, token, false);
        resizable.SetBindValue(oid, token, true);
        closable.SetBindValue(oid, token, true);
        multiline.SetBindValue(oid, token, false);

        writingChat.SetBindWatch(oid, token, true);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }
      private void HandleChatWriterEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "chat" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
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

          case "chatWriter":

            switch (nuiEvent.EventType)
            {
              case NuiEventType.Focus:

                int channel = new NuiBind<int>("channel").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
                string command = new NuiBind<string>("writingChat").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
                Effect visualMark;

                if ((channel == 1 || channel == 3) && !(command.Trim().StartsWith("/") || command.Trim().StartsWith("!") || command.Trim().StartsWith("(")))
                {
                  visualMark = Effect.VisualEffect((VfxType)1248);
                  oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").Delete();
                }
                else
                {
                  visualMark = Effect.VisualEffect((VfxType)1249);
                  oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").Value = true;
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

            NuiBind<string> chatBind = new NuiBind<string>("writingChat");
            string chatText = chatBind.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

            if (chatText.Length < 1)
              return;

            int chatChannel = new NuiBind<int>("channel").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

            if ((chatChannel == 1 || chatChannel == 3) && !(chatText.Trim().StartsWith("/") || chatText.Trim().StartsWith("!") || chatText.Trim().StartsWith("(")))
            {
              if (oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").HasValue)
              {
                foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
                  nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

                Effect visualMark = Effect.VisualEffect((VfxType)1248);
                visualMark.Tag = "VFX_SPEAKING_MARK";
                visualMark.SubType = EffectSubType.Supernatural;
                nuiEvent.Player.ControlledCreature.ApplyEffect(EffectDuration.Permanent, visualMark);

                oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").Delete();
              }
            }
            else
            {
              if (oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").HasNothing)
              {
                foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
                  nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

                Effect visualMark = Effect.VisualEffect((VfxType)1249);
                visualMark.Tag = "VFX_SPEAKING_MARK";
                visualMark.SubType = EffectSubType.Supernatural;
                nuiEvent.Player.ControlledCreature.ApplyEffect(EffectDuration.Permanent, visualMark);

                oid.LoginCreature.GetObjectVariable<LocalVariableBool>("_CURRENT_CHAT_HRP").Value = true;
              }
            }

            oid.LoginCreature.GetObjectVariable<LocalVariableString>("_CURRENT_CHAT").Value = chatText;

            if (new NuiBind<bool>("multiLine").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken))
              return;

            if (chatText.Substring(chatText.Length - 1).Contains(Environment.NewLine))
            {
              chatText = chatText.Substring(0, chatText.Length - 1);

              ChatWriterSendMessage(chatText, new NuiBind<int>("channel").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
              oid.LoginCreature.GetObjectVariable<LocalVariableString>("_CURRENT_CHAT").Delete();

              foreach (Effect eff in nuiEvent.Player.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
                nuiEvent.Player.ControlledCreature.RemoveEffect(eff);

              chatBind.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
              chatBind.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, "");
              chatBind.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);
            }

            break;

         case "geometry":

            NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
            NuiRect rectangle = geometry.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

            if (rectangle.Width <= 0 || rectangle.Height <= 0)
              return;

            geometry.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
            NuiGroup chatWriterGroup = BuildChatWriterWindow(rectangle);
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
                oid.SendServerMessage("Commande incorrectement formée. Impossible d'envoyer le message", ColorConstants.Red);
              }

              break;
          }
        }

        if (oid.IsDM)
          iChannel += 16;

        if (messageTarget != "error")
          ChatSystem.chatService.SendMessage((ChatChannel)iChannel, message, oid.ControlledCreature, target);
      }
      private NuiGroup BuildChatWriterWindow(NuiRect windowRectangle)
      {
        NuiBind<string> writingChat = new NuiBind<string>("writingChat");
        NuiBind<int> channel = new NuiBind<int>("channel");
        NuiBind<bool> makeStatic = new NuiBind<bool>("static");
        NuiBind<bool> multiline = new NuiBind<bool>("multiLine");

        List<NuiComboEntry> comboValues = new List<NuiComboEntry>
          {
            new NuiComboEntry("Parler", 1),
            new NuiComboEntry("Chuchoter", 3),
            new NuiComboEntry("Groupe", 6),
            new NuiComboEntry("MD", 14),
            new NuiComboEntry("Crier", 2)

          };

        Log.Info("-----------------------------------------------------------------------------------------------------------");
        Log.Info($"width : {windowRectangle.Width} - height : {windowRectangle.Height}");

        return new NuiGroup
        {
          Id = "chatWriterGroup",
          Border = false, Padding = 0, Margin = 0,
          Children = new List<NuiElement>
          {
            new NuiColumn
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
                new NuiRow
                {
                  Children = new List<NuiElement>
                  {
                    new NuiTextEdit("", writingChat, 3000, true) { Id = "chatWriter", Height = (windowRectangle.Height - 160) * 0.96f, Width = windowRectangle.Width * 0.96f }
                  }
                },
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
            }
          }
        };
      }
    }
  }
}
