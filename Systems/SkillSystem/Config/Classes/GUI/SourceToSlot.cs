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
      public class SourceToSlotWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<bool> enabled1 = new("enabled1");
        private readonly NuiBind<bool> enabled2 = new("enabled2");
        private readonly NuiBind<bool> enabled3 = new("enabled3");
        private readonly NuiBind<bool> enabled4 = new("enabled4");
        private readonly NuiBind<bool> enabled5 = new("enabled5");

        public byte selectedSpellLevel { get; set; }

        public SourceToSlotWindow(Player player) : base(player)
        {
          windowId = "sourceToSlot";
          windowWidth = player.guiScaledWidth * 0.18f;
          windowHeight = player.guiScaledHeight * 0.1f;
          selectedSpellLevel = 1;

          rootColumn.Children = rootChildren;

          List<NuiElement> rowChildren = new();
          NuiRow row = new() { Children = rowChildren };
          rootChildren.Add(row);

          rowChildren.Add(new NuiButtonImage("ir_level1") { Id = "1", Tooltip = "Utiliser 2 source pour obtenir un emplacement de sort de niveau 1", DisabledTooltip = "Source de magie insuffisante", Height = 40, Width = 40, Enabled = enabled1 });
          rowChildren.Add(new NuiButtonImage("ir_level2") { Id = "2", Tooltip = "Utiliser 3 source pour obtenir un emplacement de sort de niveau 2", DisabledTooltip = "Source de magie insuffisante", Height = 40, Width = 40, Enabled = enabled2 });
          rowChildren.Add(new NuiButtonImage("ir_level3") { Id = "3", Tooltip = "Utiliser 5 source pour obtenir un emplacement de sort de niveau 3", DisabledTooltip = "Source de magie insuffisante", Height = 40, Width = 40, Enabled = enabled3 });
          rowChildren.Add(new NuiButtonImage("ir_level4") { Id = "4", Tooltip = "Utiliser 6 source pour obtenir un emplacement de sort de niveau 4", DisabledTooltip = "Source de magie insuffisante", Height = 40, Width = 40, Enabled = enabled4 });
          rowChildren.Add(new NuiButtonImage("ir_level5") { Id = "5", Tooltip = "Utiliser 7 source pour obtenir un emplacement de sort de niveau 5", DisabledTooltip = "Source de magie insuffisante", Height = 40, Width = 40, Enabled = enabled5 });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiWidth * 0.35f, player.guiHeight * 0.85f, windowWidth, windowHeight);

          window = new NuiWindow(rootColumn, "")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = true,
            Border = false
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleSlotToSourceEvents;

            RefreshWindow();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandleSlotToSourceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              int nbSources = EnsoUtils.GetSorcerySource(player.oid.LoginCreature);
              selectedSpellLevel = byte.Parse(nuiEvent.ElementId);

              byte sourceCost = selectedSpellLevel switch
              {
                2 => 3,
                3 => 5,
                4 => 6,
                5 => 7,
                _ => 2,
              };

              if (nbSources < sourceCost)
              {
                RefreshWindow();
                player.oid.SendServerMessage($"Source de magie insuffisante pour générer un emplacement de niveau {selectedSpellLevel}", ColorConstants.Red);
                return;
              }


              EnsoUtils.DecrementSorcerySource(player.oid.LoginCreature, sourceCost);
              byte remainingSlots = player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(selectedSpellLevel);
              player.oid.LoginCreature.GetClassInfo(ClassType.Sorcerer).SetRemainingSpellSlots(selectedSpellLevel, (byte)(remainingSlots + 1));
              player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));

              RefreshWindow();

              return;
          }
        }
        private void RefreshWindow()
        {
          int nbSources = EnsoUtils.GetSorcerySource(player.oid.LoginCreature);

          enabled1.SetBindValue(player.oid, nuiToken.Token, nbSources > 1);
          enabled2.SetBindValue(player.oid, nuiToken.Token, nbSources > 2);
          enabled3.SetBindValue(player.oid, nuiToken.Token, nbSources > 4);
          enabled4.SetBindValue(player.oid, nuiToken.Token, nbSources > 5);
          enabled5.SetBindValue(player.oid, nuiToken.Token, nbSources > 6);
        }
      }
    }
  }
}
