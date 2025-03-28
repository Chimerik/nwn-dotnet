﻿using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ReveilSauvageWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<bool> enabled1 = new("enabled1");
        private readonly NuiBind<bool> enabled2 = new("enabled2");
        private readonly NuiBind<bool> enabled3 = new("enabled3");
        private readonly NuiBind<bool> enabled4 = new("enabled4");
        private readonly NuiBind<bool> enabled5 = new("enabled5");

        public byte selectedSpellLevel { get; set; }

        public ReveilSauvageWindow(Player player) : base(player)
        {
          windowId = "reveilSauvage";
          windowWidth = player.guiScaledWidth * 0.18f;
          windowHeight = player.guiScaledHeight * 0.1f;
          selectedSpellLevel = 1;

          rootColumn.Children = rootChildren;

          List<NuiElement> rowChildren = new();
          NuiRow row = new() { Children = rowChildren };
          rootChildren.Add(row);

          rowChildren.Add(new NuiButtonImage("ir_level1") { Id = "1", Tooltip = "Sacrifier un emplacement de niveau 1 pour obtenir 1 utilisation de Forme Sauvage", DisabledTooltip = "Aucun emplacement de niveau 1 disponible", Height = 40, Width = 40, Enabled = enabled1 });
          rowChildren.Add(new NuiButtonImage("ir_level2") { Id = "2", Tooltip = "Sacrifier un emplacement de niveau 2 pour obtenir 1 utilisation de Forme Sauvage", DisabledTooltip = "Aucun emplacement de niveau 2 disponible", Height = 40, Width = 40, Enabled = enabled2 });
          rowChildren.Add(new NuiButtonImage("ir_level3") { Id = "3", Tooltip = "Sacrifier un emplacement de niveau 3 pour obtenir 1 utilisation de Forme Sauvage", DisabledTooltip = "Aucun emplacement de niveau 3 disponible", Height = 40, Width = 40, Enabled = enabled3 });
          rowChildren.Add(new NuiButtonImage("ir_level4") { Id = "4", Tooltip = "Sacrifier un emplacement de niveau 4 pour obtenir 1 utilisation de Forme Sauvage", DisabledTooltip = "Aucun emplacement de niveau 4 disponible", Height = 40, Width = 40, Enabled = enabled4 });
          rowChildren.Add(new NuiButtonImage("ir_level5") { Id = "5", Tooltip = "Sacrifier un emplacement de niveau 5 pour obtenir 1 utilisation de Forme Sauvage", DisabledTooltip = "Aucun emplacement de niveau 5 disponible", Height = 40, Width = 40, Enabled = enabled5 });

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
            nuiToken.OnNuiEvent += HandleReveilSauvageEvents;

            RefreshWindow();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandleReveilSauvageEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if (player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.DruideCompagnonSauvage) > 0)
              {
                player.oid.SendServerMessage("Vous n'avez pas encore épuisé toute vos utilisation de Forme Sauvage", ColorConstants.Red);
                CloseWindow();
                return;
              }

              selectedSpellLevel = byte.Parse(nuiEvent.ElementId);
              byte remainingSlots = player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(selectedSpellLevel);

              if(remainingSlots < 1)
              {
                RefreshWindow();
                player.oid.SendServerMessage($"Emplacements de sort de niveau {selectedSpellLevel} insuffisants", ColorConstants.Red);
                return;
              }

              DruideUtils.RestoreFormeSauvage(player.oid.LoginCreature, 1);
              player.oid.LoginCreature.GetClassInfo(ClassType.Druid).SetRemainingSpellSlots(selectedSpellLevel, (byte)(remainingSlots - 1));
              player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
              CloseWindow();

              return;
          }
        }
        private void RefreshWindow()
        {
          enabled1.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetClassInfo(ClassType.Druid).GetRemainingSpellSlots(1) > 0);
          enabled2.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetClassInfo(ClassType.Druid).GetRemainingSpellSlots(2) > 0);
          enabled3.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetClassInfo(ClassType.Druid).GetRemainingSpellSlots(3) > 0);
          enabled4.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetClassInfo(ClassType.Druid).GetRemainingSpellSlots(4) > 0);
          enabled5.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetClassInfo(ClassType.Druid).GetRemainingSpellSlots(5) > 0);
        }
      }
    }
  }
}
