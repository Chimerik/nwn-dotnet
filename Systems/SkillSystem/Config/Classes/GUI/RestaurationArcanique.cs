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
      public class RestaurationArcaniqueWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false, Padding = 0, Margin = 0, Scrollbars = NuiScrollbars.None };
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiGroup levelGroup = new() { Id = "levelGroup", Border = false, Padding = 0, Margin = 0, Scrollbars = NuiScrollbars.None };
        private readonly NuiRow levelRow = new();

        public RestaurationArcaniqueWindow(Player player) : base(player)
        {
          windowId = "restaurationArcanique";

          rootGroup.Layout = rootColumn;
          rootColumn.Children = rootChildren;

          levelGroup.Height = 50;
          levelGroup.Layout = levelRow;
          rootChildren.Add(levelGroup);

          CreateWindow();
        }
        public void CreateWindow()
        {
          if(player.oid.LoginCreature.IsInCombat)
          {
            player.oid.SendServerMessage("Non utilisable en combat", ColorConstants.Red);
            return;
          }

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiWidth * 0.3f, player.guiHeight * 0.05f, player.guiScaledWidth * 0.3f, player.guiScaledHeight * 0.1f);
          window = new NuiWindow(rootGroup, "Restauration Arcanique")
          {
            Geometry = geometry,
            Closable = true,
            Border = false,
            Resizable = false,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleLearnableEvents;

            SetLevelLayout();       

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.3f, player.guiScaledHeight * 0.1f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if(player.oid.LoginCreature.IsInCombat)
              {
                player.oid.SendServerMessage("Non utilisable en combat", ColorConstants.Red);
                CloseWindow();
                return;
              }

              if (byte.TryParse(nuiEvent.ElementId.Split("_")[1], out byte level))
              {
                CreatureClassInfo wizardClass = player.oid.LoginCreature.GetClassInfo(ClassType.Wizard);
                wizardClass.SetRemainingSpellSlots(level, (byte)(wizardClass.GetRemainingSpellSlots(level) + 1));

                player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.WizardRestaurationArcanique, (byte)(player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.WizardRestaurationArcanique) - level));

                if(player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.WizardRestaurationArcanique) < 1)
                  CloseWindow();

                player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpMagicalVision));
                StringUtils.DisplayStringToAllPlayersNearTarget(player.oid.LoginCreature, $"{player.oid.LoginCreature.Name.ColorString(ColorConstants.Cyan)} - Restauration Arcanique", StringUtils.gold, true, true);

                SetLevelLayout();
              }
              return;
          }
        }
        private void SetLevelLayout()
        {
          List<NuiElement> levelChildren = new() { new NuiSpacer() };
          CreatureClassInfo wizardClass = player.oid.LoginCreature.GetClassInfo(ClassType.Wizard);
          int wizardLevel = wizardClass.Level;
          var spellGainTable = NwClass.FromClassType(ClassType.Wizard).SpellGainTable;
          int level = 0;

          foreach (var spellLevel in player.learnableSpells.Values.Where(s => s.currentLevel > 0 && s.learntFromClasses.Contains(CustomClass.Wizard)).Select(s => NwSpell.FromSpellId(s.id).GetSpellLevelForClass(ClassType.Wizard)).Distinct().Order())
          {
            if (spellLevel > 5)
              continue;

            string icon = level switch
              {
                0 => "ir_cantrips",
                1 => "ir_level1",
                2 => "ir_level2",
                3 => "ir_level3",
                4 => "ir_level4",
                _ => "ir_level5",
              };

            if (spellLevel > 0 && spellLevel < 7)
            {
              int maxSpellSlots = spellGainTable[wizardLevel - 1][level];

              string disabledTooltip = "";
              bool enabled = false;

              if (wizardClass.GetRemainingSpellSlots((byte)level) >= maxSpellSlots)
                disabledTooltip = "Aucun emplacement à restaurer à ce niveau";
              else if (player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.WizardRestaurationArcanique) < level)
                disabledTooltip = "Restauration restante insuffisante pour ce niveau";
              else
                enabled = true;

              levelChildren.Add(new NuiButtonImage(icon)
              {
                Id = $"level_{level}",
                Tooltip = $"Restaurer un emplacement de niveau {level}",
                DisabledTooltip = disabledTooltip,
                Height = 40,
                Width = 40,
                Enabled = enabled
              });
            }

            level++;
          }
            
          levelChildren.Add(new NuiSpacer());
          levelRow.Children = levelChildren;
          levelGroup.SetLayout(player.oid, nuiToken.Token, levelRow);
        }
      }
    }
  }
}
