using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class BodyColorWindow : PlayerWindow
      {
        NuiBind<string> currentColor { get; }
        NuiBind<int> channelSelection { get; }
        List<NuiComboEntry> comboChannel { get; }
        NuiBind<string>[] colorBindings { get; }

        public BodyColorWindow(Player player) : base(player)
        {
          windowId = "bodyColorsModifier";

          currentColor = new NuiBind<string>("currentColor");
          channelSelection = new NuiBind<int>("channelSelection");

          comboChannel = new List<NuiComboEntry>
          {
            new NuiComboEntry("Cheveux", 1),
            new NuiComboEntry("Peau", 0),
            new NuiComboEntry("Yeux / Tattoo 1", 3),
            new NuiComboEntry("Lèvres / Tattoo 2", 2),
          };

          colorBindings = new NuiBind<string>[176];
          CreateWindow();
        }
        public void CreateWindow()
        {
          player.DisableItemAppearanceFeedbackMessages();

          List<NuiElement> colChildren = new List<NuiElement>();

          NuiRow comboRow = new NuiRow()
          {
            Children = new List<NuiElement>
            {
              new NuiSpacer { },
              new NuiLabel("Actuelle") { Width = 65, Height = 35, VerticalAlign = NuiVAlign.Middle },
              new NuiButtonImage(currentColor) { Margin = 10, Width = 25, Height = 25 },
              new NuiCombo
              {
                Id = "colorChannel", Width = 240,
                Entries = comboChannel,
                Selected = channelSelection
              },
              new NuiSpacer { }
            }
          };

          colChildren.Add(comboRow);

          int nbButton = 0;

          for (int i = 0; i < 11; i++)
          {
            NuiRow row = new NuiRow();
            List<NuiElement> rowChildren = new List<NuiElement>();

            for (int j = 0; j < 16; j++)
            {
              NuiButtonImage button = new NuiButtonImage(colorBindings[nbButton])
              {
                Id = $"{nbButton}",
                Width = 25,
                Height = 25
              };

              rowChildren.Add(button);
              nbButton++;
            }

            row.Children = rowChildren;
            colChildren.Add(row);
          }

          NuiRow buttonRow = new NuiRow()
          {
            Children = new List<NuiElement>
            {
              new NuiSpacer {},
              new NuiButton("Modifications corporelles") { Id = "openBodyAppearance", Height = 35, Width = 350 },
              new NuiSpacer {}
            }
          };

          colChildren.Add(buttonRow);

          NuiColumn root = new NuiColumn { Children = colChildren };

          window = new NuiWindow(root, "Vous contemplez votre reflet dans le miroir")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleBodyColorsEvents;
          player.oid.OnNuiEvent += HandleBodyColorsEvents;

          if (player.oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").HasNothing)
          {
            PlayerPlugin.ApplyLoopingVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 173);
            player.oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").Value = true;
          }

          token = player.oid.CreateNuiWindow(window, windowId);

          currentColor.SetBindValue(player.oid, token, $"hair{player.oid.ControlledCreature.GetColor(ColorChannel.Hair) + 1}");
          channelSelection.SetBindValue(player.oid, token, 1);
          channelSelection.SetBindWatch(player.oid, token, true);

          for (int i = 0; i < 176; i++)
            colorBindings[i].SetBindValue(player.oid, token, NWScript.ResManGetAliasFor($"hair{i + 1}", NWScript.RESTYPE_TGA) != "" ? $"hair{i + 1}" : $"leather{i + 1}");

          player.openedWindows[windowId] = token;
        }
        private void HandleBodyColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (player.oid.NuiGetWindowId(token) != windowId)
            return;

          if (nuiEvent.EventType == NuiEventType.Close)
          {
            if (player.oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").HasValue)
            {
              PlayerPlugin.ApplyLoopingVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 173);
              player.oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").Delete();
            }

            player.EnableItemAppearanceFeedbackMessages();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if (nuiEvent.ElementId == "openBodyAppearance")
              {
                player.oid.NuiDestroy(token);

                if (player.windows.ContainsKey("bodyAppearanceModifier"))
                  ((BodyAppearanceWindow)player.windows["bodyAppearanceModifier"]).CreateWindow();
                else
                  player.windows.Add("bodyAppearanceModifier", new BodyAppearanceWindow(player));

                return;
              }

              player.oid.ControlledCreature.SetColor((ColorChannel)channelSelection.GetBindValue(player.oid, token), int.Parse(nuiEvent.ElementId));

              string chanChoice = "hair";
              if (channelSelection.GetBindValue(player.oid, token) != 1)
                chanChoice = "skin";

              currentColor.SetBindValue(player.oid, token, NWScript.ResManGetAliasFor($"{chanChoice}{int.Parse(nuiEvent.ElementId) + 1}", NWScript.RESTYPE_TGA) != "" ? $"{chanChoice}{int.Parse(nuiEvent.ElementId) + 1}" : $"leather{int.Parse(nuiEvent.ElementId) + 1}");

              break;

            case NuiEventType.Watch:

              if (nuiEvent.ElementId == "channelSelection")
              {
                string channelChoice = "hair";
                ColorChannel selectedChannel = (ColorChannel)channelSelection.GetBindValue(player.oid, token);
                if (selectedChannel != ColorChannel.Hair)
                  channelChoice = "skin";

                for (int i = 0; i < 4; i++)
                  colorBindings[i].SetBindValue(player.oid, token, NWScript.ResManGetAliasFor($"{channelChoice}{i + 1}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{i + 1}" : $"leather{i + 1}");

                int newCurrentColor = player.oid.ControlledCreature.GetColor(selectedChannel) + 1;
                currentColor.SetBindValue(player.oid, token, NWScript.ResManGetAliasFor($"{channelChoice}{newCurrentColor}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{newCurrentColor}" : $"leather{newCurrentColor}");
              }
              break;
          }
        }
      }
    }
  }
}
