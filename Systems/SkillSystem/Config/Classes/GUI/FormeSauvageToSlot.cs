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
      public class FormeSauvageToSlotWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<bool> enabled1 = new("enabled1");
        private readonly NuiBind<bool> enabled2 = new("enabled2");
        private readonly NuiBind<bool> enabled3 = new("enabled3");
        private readonly NuiBind<bool> enabled4 = new("enabled4");
        private readonly NuiBind<bool> enabled5 = new("enabled5");

        public byte selectedSpellLevel { get; set; }

        public FormeSauvageToSlotWindow(Player player) : base(player)
        {
          windowId = "formeSauvageToSlot";
          windowWidth = player.guiScaledWidth * 0.18f;
          windowHeight = player.guiScaledHeight * 0.1f;
          selectedSpellLevel = 1;

          rootColumn.Children = rootChildren;

          List<NuiElement> rowChildren = new();
          NuiRow row = new() { Children = rowChildren };
          rootChildren.Add(row);

          rowChildren.Add(new NuiButtonImage("ir_level1") { Id = "1", Tooltip = "Utiliser 1 Forme Sauvage pour obtenir 2 emplacements de sort de niveau 1", DisabledTooltip = "Forme Sauvage insuffisante", Height = 40, Width = 40, Enabled = enabled1 });
          rowChildren.Add(new NuiButtonImage("ir_level2") { Id = "2", Tooltip = "Utiliser 1 Forme Sauvage pour obtenir 1 emplacement de sort de niveau 2", DisabledTooltip = "Forme Sauvage insuffisante", Height = 40, Width = 40, Enabled = enabled2 });
          rowChildren.Add(new NuiButtonImage("ir_level3") { Id = "3", Tooltip = "Utiliser 2 Formes Sauvages pour obtenir 1 emplacement de sort de niveau 3 et un emplacement de sort de niveau 1", DisabledTooltip = "Forme Sauvage insuffisante", Height = 40, Width = 40, Enabled = enabled3 });
          rowChildren.Add(new NuiButtonImage("ir_level4") { Id = "4", Tooltip = "Utiliser 2 Formes Sauvages pour obtenir 1 emplacement de sort de niveau 4", DisabledTooltip = "Forme Sauvage insuffisante", Height = 40, Width = 40, Enabled = enabled4 });
          rowChildren.Add(new NuiButtonImage("ir_level5") { Id = "5", Tooltip = "Utiliser 3 Formes Sauvages pour obtenir un emplacement de sort de niveau 5 et un emplacement de sort de niveau 1", DisabledTooltip = "Forme Sauvage insuffisante", Height = 40, Width = 40, Enabled = enabled5 });

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
            nuiToken.OnNuiEvent += HandleFormeSauvageToSlotEvents;

            RefreshWindow();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandleFormeSauvageToSlotEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              int nbSources = player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.DruideCompagnonSauvage);
              selectedSpellLevel = byte.Parse(nuiEvent.ElementId);

              byte sourceCost = selectedSpellLevel switch
              {
                2 => 1,
                3 => 2,
                4 => 2,
                5 => 3,
                _ => 1,
              };

              if (nbSources < sourceCost)
              {
                RefreshWindow();
                player.oid.SendServerMessage("Forme Sauvage insuffisante pour générer les emplacements sélectionnés", ColorConstants.Red);
                return;
              }


              DruideUtils.DecrementFormeSauvage(player.oid.LoginCreature, sourceCost);
              byte remainingSlots = player.oid.LoginCreature.GetClassInfo(ClassType.Druid).GetRemainingSpellSlots(selectedSpellLevel);
              player.oid.LoginCreature.GetClassInfo(ClassType.Druid).SetRemainingSpellSlots(selectedSpellLevel, (byte)(remainingSlots + 1));
              player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadNature));

              RefreshWindow();

              return;
          }
        }
        private void RefreshWindow()
        {
          int nbSources = player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.DruideCompagnonSauvage);

          enabled1.SetBindValue(player.oid, nuiToken.Token, nbSources > 1);
          enabled2.SetBindValue(player.oid, nuiToken.Token, nbSources > 1);
          enabled3.SetBindValue(player.oid, nuiToken.Token, nbSources > 2);
          enabled4.SetBindValue(player.oid, nuiToken.Token, nbSources > 2);
          enabled5.SetBindValue(player.oid, nuiToken.Token, nbSources > 3);
        }
      }
    }
  }
}
