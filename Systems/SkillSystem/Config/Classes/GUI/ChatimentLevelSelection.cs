using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ChatimentLevelSelectionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<bool> encouraged1 = new("encouraged1");
        private readonly NuiBind<bool> encouraged2 = new("encouraged2");
        private readonly NuiBind<bool> encouraged3 = new("encouraged3");
        private readonly NuiBind<bool> encouraged4 = new("encouraged4");
        private readonly NuiBind<bool> encouraged5 = new("encouraged5");
        private readonly NuiBind<bool> enabled1 = new("enabled1");
        private readonly NuiBind<bool> enabled2 = new("enabled2");
        private readonly NuiBind<bool> enabled3 = new("enabled3");
        private readonly NuiBind<bool> enabled4 = new("enabled4");
        private readonly NuiBind<bool> enabled5 = new("enabled5");

        public int selectedSpellLevel { get; set; }

        public ChatimentLevelSelectionWindow(Player player) : base(player)
        {
          windowId = "chatimentLevelSelection";
          windowWidth = player.guiScaledWidth * 0.18f;
          windowHeight = player.guiScaledHeight * 0.1f;
          selectedSpellLevel = 1;

          rootColumn.Children = rootChildren;

          List<NuiElement> rowChildren = new();
          NuiRow row = new() { Children = rowChildren };
          rootChildren.Add(row);

          rowChildren.Add(new NuiButtonImage("ir_level1") { Id = "1", Tooltip = "Vos châtiments utiliseront un emplacement de niveau 1", DisabledTooltip = "Aucun emplacement de niveau 1 disponible", Height = 40, Width = 40, Encouraged = encouraged1, Enabled = enabled1 });
          rowChildren.Add(new NuiButtonImage("ir_level2") { Id = "2", Tooltip = "Vos châtiments utiliseront un emplacement de niveau 2", DisabledTooltip = "Aucun emplacement de niveau 2 disponible", Height = 40, Width = 40, Encouraged = encouraged2, Enabled = enabled2 });
          rowChildren.Add(new NuiButtonImage("ir_level3") { Id = "3", Tooltip = "Vos châtiments utiliseront un emplacement de niveau 3", DisabledTooltip = "Aucun emplacement de niveau 3 disponible", Height = 40, Width = 40, Encouraged = encouraged3, Enabled = enabled3 });
          rowChildren.Add(new NuiButtonImage("ir_level4") { Id = "4", Tooltip = "Vos châtiments utiliseront un emplacement de niveau 4", DisabledTooltip = "Aucun emplacement de niveau 4 disponible", Height = 40, Width = 40, Encouraged = encouraged4, Enabled = enabled4 });
          rowChildren.Add(new NuiButtonImage("ir_level5") { Id = "5", Tooltip = "Vos châtiments utiliseront un emplacement de niveau 5", DisabledTooltip = "Aucun emplacement de niveau 5 disponible", Height = 40, Width = 40, Encouraged = encouraged5, Enabled = enabled5 });

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
            nuiToken.OnNuiEvent += HandleChatimentLevelSelectionEvents;

            encouraged1.SetBindValue(player.oid, nuiToken.Token, selectedSpellLevel == 1);
            encouraged2.SetBindValue(player.oid, nuiToken.Token, selectedSpellLevel == 2);
            encouraged3.SetBindValue(player.oid, nuiToken.Token, selectedSpellLevel == 3);
            encouraged4.SetBindValue(player.oid, nuiToken.Token, selectedSpellLevel == 4);
            encouraged5.SetBindValue(player.oid, nuiToken.Token, selectedSpellLevel == 5);

            enabled1.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(1) > 0);
            enabled2.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(2) > 0);
            enabled3.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(3) > 0);
            enabled4.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(4) > 0);
            enabled5.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(5) > 0);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandleChatimentLevelSelectionEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              selectedSpellLevel = int.Parse(nuiEvent.ElementId);
              player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentDivin, player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots((byte)selectedSpellLevel));

              encouraged1.SetBindValue(player.oid, nuiToken.Token, selectedSpellLevel == 1);
              encouraged2.SetBindValue(player.oid, nuiToken.Token, selectedSpellLevel == 2);
              encouraged3.SetBindValue(player.oid, nuiToken.Token, selectedSpellLevel == 3);
              encouraged4.SetBindValue(player.oid, nuiToken.Token, selectedSpellLevel == 4);
              encouraged5.SetBindValue(player.oid, nuiToken.Token, selectedSpellLevel == 5);

              return;
          }
        }
      }
    }
  }
}
