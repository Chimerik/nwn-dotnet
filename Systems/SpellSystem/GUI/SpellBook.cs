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
      public class SpellBookWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiGroup classGroup = new() { Id = "classGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiRow classRow = new();
        private readonly NuiGroup levelGroup = new() { Id = "levelGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiRow levelRow = new();
        private readonly NuiGroup cantripGroup = new() { Id = "cantripGroup", Border = true, Padding = 0, Margin = 0 };
        private readonly NuiRow cantripRow = new();
        private readonly NuiGroup preparedSpellGroup = new() { Id = "preparedSpellGroup", Border = true, Padding = 0, Margin = 0 };
        private readonly NuiRow preparedSpellRow = new();
        private readonly NuiGroup spellGroup = new() { Id = "spellGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiRow spellRow = new();

        private readonly NuiBind<string> className = new("className");
        private readonly NuiBind<string> castingAbility = new("castingAbility");
        private readonly NuiBind<string> castingAbilityIcon = new("castingAbilityIcon");
        private readonly NuiBind<string> spellDC = new("spellDC");
        private readonly NuiBind<string> spellAttack = new("spellAttack");

        private NwClass selectedClass;
        private int selectedSpellLevel;

        public SpellBookWindow(Player player) : base(player)
        {
          windowId = "spellBook";

          selectedClass = player.oid.LoginCreature.Classes.FirstOrDefault(c => c.Class.IsSpellCaster)?.Class;
          selectedSpellLevel = 0;

          rootGroup.Layout = rootColumn;
          rootColumn.Children = rootChildren;

          classGroup.Height = 50;
          classGroup.Layout = classRow;
          rootChildren.Add(classGroup);
          
          rootChildren.Add(new NuiRow() { Children = new()
          {
            new NuiSpacer(),
            new NuiGroup() { Layout = new NuiColumn() { Children = new() { new NuiLabel(className) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle } } }, Height = 45, Width = 90, Border = true },
            new NuiGroup() { Layout = new NuiColumn() { Children = new() { new NuiImage(castingAbilityIcon) { Height = 40, Width = 40, ImageAspect = NuiAspect.Fit } } }, Height = 45, Width = 45, Tooltip = castingAbility, Border = true, Scrollbars = NuiScrollbars.None },
            new NuiGroup() { Layout = new NuiColumn() { Children = new() { new NuiLabel(spellDC) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle } } }, Height = 45, Width = 45, Tooltip = "Degré de difficulté des jets de sauvegarde des sorts", Border = true },
            new NuiGroup() { Layout = new NuiColumn() { Children = new() { new NuiLabel(spellAttack) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle } } }, Height = 45, Width = 45, Tooltip = "Bonus d'attaque des sorts", Border = true },
            new NuiSpacer()
          } });

          levelGroup.Height = 50;
          levelGroup.Layout = levelRow;
          rootChildren.Add(levelGroup);

          cantripGroup.Layout = cantripRow;
          preparedSpellGroup.Layout = preparedSpellRow;
          spellGroup.Layout = spellRow;

          CreateWindow();
        }
        public void CreateWindow()
        {
          if(selectedClass is null)
          {
            player.oid.SendServerMessage("Aucune de vos classes ne dispose de livre de sorts", ColorConstants.Orange);
            return;
          }

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiWidth * 0.2f, player.guiHeight * 0.05f, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f);
          window = new NuiWindow(rootGroup, "Livre de sorts")
          {
            Geometry = geometry,
            Closable = true,
            Border = true,
            Resizable = false,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleLearnableEvents;

            SetClassLayout();
            SetLevelLayout();

            selectedSpellLevel = player.learnableSpells.Values.Where(s => s.currentLevel > 0 && s.learntFromClasses.Contains(selectedClass.Id))
              .Min(s => NwSpell.FromSpellId(s.id).GetSpellLevelForClass(selectedClass));

            SetPreparedSpellLayout();
            SetCantripLayout();
            SetSpellLayout();            

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if (int.TryParse(nuiEvent.ElementId, out int classId))
              {
                selectedClass = NwClass.FromClassId(classId);

                SetClassLayout();
                SetLevelLayout();
                SetPreparedSpellLayout();
                SetCantripLayout();
                SetSpellLayout();  

                return;
              }
              else if (nuiEvent.ElementId.Contains("description"))
              {
                NwSpell spell = NwSpell.FromSpellId(int.Parse(nuiEvent.ElementId.Split("_")[1]));

                if (!player.windows.TryGetValue("spellDescription", out var spellDescription)) player.windows.Add("spellDescription", new SpellDescriptionWindow(player, spell));
                else ((SpellDescriptionWindow)spellDescription).CreateWindow(spell);
              }
              else if (nuiEvent.ElementId.Contains("level"))
              {
                if (int.TryParse(nuiEvent.ElementId.Split("_")[1], out int level))
                {
                  selectedSpellLevel = level;

                  SetLevelLayout();
                  SetPreparedSpellLayout();
                  SetCantripLayout();
                  SetSpellLayout();
                }
              }
              else if (nuiEvent.ElementId.Contains("remove"))
              {
                NwSpell spell = NwSpell.FromSpellId(int.Parse(nuiEvent.ElementId.Split("_")[1]));
                int spellLevel = spell.GetSpellLevelForClass(selectedClass);
                player.oid.LoginCreature.GetClassInfo(selectedClass).KnownSpells[spellLevel].Remove(spell);
                SetPreparedSpellLayout();
                SetSpellLayout();
              }
              else if (nuiEvent.ElementId.Contains("add"))
              {
                NwSpell spell = NwSpell.FromSpellId(int.Parse(nuiEvent.ElementId.Split("_")[1]));
                int spellLevel = spell.GetSpellLevelForClass(selectedClass);
                player.oid.LoginCreature.GetClassInfo(selectedClass).KnownSpells[spellLevel].Add(spell);
                SetPreparedSpellLayout();
                SetSpellLayout();
              }

              return;
          }
        }
        private void SetClassLayout()
        {
          List<NuiElement> classChildren = new() { new NuiSpacer() };

          foreach (var playerClass in player.oid.LoginCreature.Classes.Where(c => c.Class.IsSpellCaster))
            classChildren.Add(new NuiButtonImage(playerClass.Class.IconResRef) { Id = playerClass.Class.Id.ToString(),
              Tooltip = $"{playerClass.Class.Name.ToString()} ({playerClass.Level})", Height = 40, Width = 40,
              Encouraged = playerClass.Class.Id == selectedClass.Id});
          
          classChildren.Add(new NuiSpacer());

          classRow.Children = classChildren;
          classGroup.SetLayout(player.oid, nuiToken.Token, classRow);

          className.SetBindValue(player.oid, nuiToken.Token, selectedClass.Name.ToString());
          castingAbility.SetBindValue(player.oid, nuiToken.Token, StringUtils.TranslateAttributeToFrench(selectedClass.SpellCastingAbility));
          castingAbilityIcon.SetBindValue(player.oid, nuiToken.Token, selectedClass.SpellCastingAbility.ToString().ToLower());
          spellDC.SetBindValue(player.oid, nuiToken.Token, SpellUtils.GetCasterSpellDC(player.oid.LoginCreature, selectedClass.SpellCastingAbility).ToString());
          spellAttack.SetBindValue(player.oid, nuiToken.Token, (NativeUtils.GetCreatureProficiencyBonus(player.oid.LoginCreature) + player.oid.LoginCreature.GetAbilityModifier(selectedClass.SpellCastingAbility)).ToString());
        }
        private void SetLevelLayout()
        {
          List<NuiElement> levelChildren = new() { new NuiSpacer() };
          byte maxSlotKnown = SpellUtils.GetMaxSpellSlotLevelKnown(player.oid.LoginCreature, selectedClass.ClassType);

          foreach (var spellLevel in player.learnableSpells.Values.Where(s => s.currentLevel > 0 && s.learntFromClasses.Contains(selectedClass.Id) && NwSpell.FromSpellId(s.id).GetSpellLevelForClass(selectedClass) <= maxSlotKnown)
            .Select(s => NwSpell.FromSpellId(s.id).GetSpellLevelForClass(selectedClass)).Distinct().Order())
          {
              string icon = spellLevel switch
              {
                0 => "ir_cantrips",
                1 => "ir_level1",
                2 => "ir_level2",
                3 => "ir_level3",
                4 => "ir_level4",
                5 => "ir_level5",
                6 => "ir_level6",
                _ => "ir_level789",
              };

              levelChildren.Add(new NuiButtonImage(icon)
              {
                Id = $"level_{spellLevel}",
                Tooltip = $"Vos sorts de niveau {spellLevel}",
                Height = 40,
                Width = 40,
                Encouraged = spellLevel == selectedSpellLevel
              });
          }
            
          levelChildren.Add(new NuiSpacer());

          levelRow.Children = levelChildren;
          levelGroup.SetLayout(player.oid, nuiToken.Token, levelRow);
        }

        private void SetCantripLayout()
        {
          var classInfo = player.oid.LoginCreature.GetClassInfo(selectedClass);

          if (selectedSpellLevel > 0 || classInfo.KnownSpells[0].Count < 1)
          {
            rootColumn.Children.Remove(cantripGroup);
            rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
            return;
          }

          List<NuiElement> cantripChildren = new();

          cantripRow.Children = new()
          {
            new NuiColumn() { Children = new() { new NuiSpacer(), new NuiImage("ir_cantrips") { Tooltip = "Tours de magie", Height = 40, Width = 40, ImageAspect = NuiAspect.Fit }, new NuiSpacer() }, Width = 50 },
            new NuiSpacer(),
            new NuiColumn { Children = cantripChildren, Width = 500 },
            new NuiSpacer()
          };

          List<NuiElement> spellChildren = new();
          float layoutHeight = 0;

          for (int i = 0; i < classInfo.KnownSpells[0].Count; i++)
          {
            if (i % 12 == 0)
            {
              spellChildren = new();
              cantripChildren.Add(new NuiRow() { Children = spellChildren });
              layoutHeight += 50;
            }

            NwSpell spell = classInfo.KnownSpells[0][i];
            spellChildren.Add(new NuiButtonImage(spell.IconResRef) { Id = $"description_{spell.Id}", Tooltip = spell.Name.ToString(), Height = 40, Width = 40 });
          }

          cantripGroup.Height = layoutHeight;

          if (!rootColumn.Children.Contains(cantripGroup))
            rootColumn.Children.Add(cantripGroup);

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
          cantripGroup.SetLayout(player.oid, nuiToken.Token, cantripRow);
        }
        private void SetPreparedSpellLayout()
        {
          switch(selectedClass.Id)
          {
            case CustomClass.Cleric:
            case CustomClass.Druid:
            case CustomClass.Paladin:
            case CustomClass.Wizard:

              List<NuiElement> cantripChildren = new();

              preparedSpellRow.Children = new()
              {
                new NuiColumn() { Children = new() { new NuiSpacer(), new NuiImage("ir_splbook") { Tooltip = "Votre grimoire de sorts préparés", Height = 40, Width = 40, ImageAspect = NuiAspect.Fit }, new NuiSpacer() }, Width = 50 },
                new NuiSpacer(),
                new NuiColumn { Children = cantripChildren, Width = 500 },
                new NuiSpacer()
              };

              var classInfo = player.oid.LoginCreature.GetClassInfo(selectedClass);
              int preparedSpells = CreatureUtils.GetPreparableSpellsCount(player.oid.LoginCreature, selectedClass);

              if(preparedSpells < 1)
                preparedSpells = 1; 
              
              List<NuiElement> spellChildren = new();
              List<NwSpell> readySpells = new();
              float layoutHeight = 0;

              foreach (var spellLevel in classInfo.KnownSpells.Skip(1))
                foreach (var spell in spellLevel)
                  readySpells.Add(spell);

              for (int i = 0; i < preparedSpells; i++)
              {
                if (i % 12 == 0)
                {
                  spellChildren = new();
                  cantripChildren.Add(new NuiRow() { Children = spellChildren });
                  layoutHeight += 70;
                }

                if(readySpells.Count > 0)
                {
                  NwSpell spell = readySpells.First();
                  List<NuiElement> buttonsChildren = new();
                  spellChildren.Add(new NuiColumn { Children = buttonsChildren });

                  List<NuiElement> guiSpacersChildren = new();
                  guiSpacersChildren.Add(new NuiSpacer());

                  buttonsChildren.Add(new NuiButtonImage(spell.IconResRef) { Id = $"description_{spell.Id}", Tooltip = spell.Name.ToString(), Height = 40, Width = 40 });
                  
                  buttonsChildren.Add(new NuiRow { Children = guiSpacersChildren });
                  guiSpacersChildren.Add(new NuiButtonImage("gui_arrow_down") { Id = $"remove_{spell.Id}", Tooltip = "Retirer", Height = 14, Width = 22 });

                  guiSpacersChildren.Add(new NuiSpacer());

                  readySpells.Remove(spell);               
                }
                else
                {
                  spellChildren.Add(new NuiButtonImage("ir_dis_trap ") { Tooltip = "Emplacement vide", Encouraged = true, Height = 40, Width = 40 });
                }
              }

              preparedSpellGroup.Height = layoutHeight;

              if (!rootColumn.Children.Contains(preparedSpellGroup))
                  rootColumn.Children.Add(preparedSpellGroup);

              rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
              preparedSpellGroup.SetLayout(player.oid, nuiToken.Token, preparedSpellRow);

              break;

            default:

                rootColumn.Children.Remove(preparedSpellGroup);
                rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);

              break;
          }
        }
        private void SetSpellLayout()
        {
          var spellList = player.learnableSpells.Values.Where(s => s.currentLevel > 0 && s.multiplier == selectedSpellLevel + 1 && s.learntFromClasses.Contains(selectedClass.Id));

          if(selectedSpellLevel < 1 || !spellList.Any())
          {
            rootColumn.Children.Remove(spellGroup);
            rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
            return;
          }

          string icon = selectedSpellLevel switch
          {
            0 => "ir_cantrips",
            1 => "ir_level1",
            2 => "ir_level2",
            3 => "ir_level3",
            4 => "ir_level4",
            5 => "ir_level5",
            6 => "ir_level6",
            _ => "ir_level789",
          };

          List<NuiElement> rowChildren = new();

          spellRow.Children = new()
          {
            new NuiColumn() { Children = new() { new NuiSpacer(), new NuiImage(icon) { Tooltip = $"Sorts connus de niveau {selectedSpellLevel}", Height = 40, Width = 40, ImageAspect = NuiAspect.Fit }, new NuiSpacer() }, Width = 50 },
            new NuiSpacer(),
            new NuiColumn { Children = rowChildren, Width = 500 },
            new NuiSpacer()
          };

          List<NuiElement> spellChildren = new();
          float layoutHeight = 0;
          bool canPrepareSpells = false;
          string disabledTooltipMessage = "Aucun emplacement de préparation libre";

          if (!EffectUtils.HasTaggedEffect(player.oid.LoginCreature, EffectSystem.CanPrepareSpellsEffectTag))
          {
            canPrepareSpells = false;
            disabledTooltipMessage = "Vous devez prendre un repos long avant de pouvoir modifier votre mémorisation de sorts";
          }
          else if (CreatureUtils.GetKnownSpellsCount(player.oid.LoginCreature, player.oid.LoginCreature.GetClassInfo(selectedClass)) <
            CreatureUtils.GetPreparableSpellsCount(player.oid.LoginCreature, selectedClass))
          {
            canPrepareSpells = true;
            disabledTooltipMessage = "Sort déjà préparé";
          }

         

          for (int i = 0; i < spellList.Count(); i++)
          {
            if (i % 12 == 0)
            {
              spellChildren = new();
              rowChildren.Add(new NuiRow() { Children = spellChildren });
              layoutHeight += 65;
            }

            NwSpell spell = NwSpell.FromSpellId(spellList.ElementAt(i).id);
            
            List<NuiElement> buttonsChildren = new();
            spellChildren.Add(new NuiColumn { Children = buttonsChildren });
            
            switch (selectedClass.Id)
            {
              case CustomClass.Cleric:
              case CustomClass.Druid:
              case CustomClass.Paladin:
              case CustomClass.Wizard:

                List<NuiElement> guiSpacersChildren = new();

                buttonsChildren.Add(new NuiRow { Children = guiSpacersChildren });
                guiSpacersChildren.Add(new NuiSpacer());

                guiSpacersChildren.Add(new NuiButtonImage("gui_arrow_up") { Id = $"add_{spell.Id}", Tooltip = "Ajouter",
                  DisabledTooltip = disabledTooltipMessage, Height = 14, Width = 22,
                  Enabled = canPrepareSpells && !player.oid.LoginCreature.GetClassInfo(selectedClass).KnownSpells[selectedSpellLevel].Contains(spell) });
                
                guiSpacersChildren.Add(new NuiSpacer());

                break;
            }

            buttonsChildren.Add(new NuiButtonImage(spell.IconResRef) { Id = $"description_{spell.Id}", Tooltip = spell.Name.ToString(),
              Height = 40, Width = 40 });
          }

          spellGroup.Height = layoutHeight;

          if (!rootColumn.Children.Contains(spellGroup))
            rootColumn.Children.Add(spellGroup);

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
          spellGroup.SetLayout(player.oid, nuiToken.Token, spellRow);
        }
        /*private void InsertSpellGroup(int spellLevel, NuiGroup levelSpellGroup)
        {
          int insertPosition = 2;

          if (rootColumn.Children.Contains(cantripGroup))
            insertPosition += 1;

          if (rootColumn.Children.Contains(preparedSpellGroup))
            insertPosition += 1;

          if(spellLevel == 9)
          {
            rootColumn.Children.Insert(insertPosition, levelSpellGroup);
            return;
          }

          if (rootColumn.Children.Contains(level9SpellGroup))
            insertPosition += 1;

          if (spellLevel == 8)
          {
            rootColumn.Children.Insert(insertPosition, levelSpellGroup);
            return;
          }

          if (rootColumn.Children.Contains(level8SpellGroup))
            insertPosition += 1;

          if (spellLevel == 7)
          {
            rootColumn.Children.Insert(insertPosition, levelSpellGroup);
            return;
          }

          if (rootColumn.Children.Contains(level7SpellGroup))
            insertPosition += 1;

          if (spellLevel == 6)
          {
            rootColumn.Children.Insert(insertPosition, levelSpellGroup);
            return;
          }

          if (rootColumn.Children.Contains(level6SpellGroup))
            insertPosition += 1;

          if (spellLevel == 5)
          {
            rootColumn.Children.Insert(insertPosition, levelSpellGroup);
            return;
          }

          if (rootColumn.Children.Contains(level5SpellGroup))
            insertPosition += 1;

          if (spellLevel == 4)
          {
            rootColumn.Children.Insert(insertPosition, levelSpellGroup);
            return;
          }

          if (rootColumn.Children.Contains(level4SpellGroup))
            insertPosition += 1;

          if (spellLevel == 3)
          {
            rootColumn.Children.Insert(insertPosition, levelSpellGroup);
            return;
          }

          if (rootColumn.Children.Contains(level3SpellGroup))
            insertPosition += 1;

          if (spellLevel == 2)
          {
            rootColumn.Children.Insert(insertPosition, levelSpellGroup);
            return;
          }

          if (rootColumn.Children.Contains(level2SpellGroup))
            insertPosition += 1;

          if (spellLevel == 1)
          {
            rootColumn.Children.Insert(insertPosition, levelSpellGroup);
            return;
          }
        }*/
      }
    }
  }
}
