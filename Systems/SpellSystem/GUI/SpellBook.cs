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
        private readonly NuiRow cantripRow = new();
        private readonly NuiRow preparedSpellRow = new();

        private readonly NuiBind<string> className = new("className");
        private readonly NuiBind<string> castingAbility = new("castingAbility");
        private readonly NuiBind<string> castingAbilityIcon = new("castingAbilityIcon");
        private readonly NuiBind<string> spellDC = new("spellDC");
        private readonly NuiBind<string> spellAttack = new("spellAttack");

        private readonly NuiGroup cantripGroup = new() { Id = "cantripGroup", Border = true, Padding = 0, Margin = 0 };
        private readonly NuiGroup preparedSpellGroup = new() { Id = "preparedSpellGroup", Border = true, Padding = 0, Margin = 0 };

        private NwClass selectedClass;

        public SpellBookWindow(Player player) : base(player)
        {
          windowId = "spellBook";

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


          cantripGroup.Layout = cantripRow;
          preparedSpellGroup.Layout = preparedSpellRow;

          rootChildren.Add(cantripGroup);
          rootChildren.Add(preparedSpellGroup);
          CreateWindow();
        }
        public void CreateWindow()
        {
          selectedClass = player.oid.LoginCreature.Classes.FirstOrDefault(c => c.Class.IsSpellCaster)?.Class;

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
            SetCantripLayout();
            SetPreparedSpellLayout();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if(int.TryParse(nuiEvent.ElementId, out int classId))
              {
                selectedClass = NwClass.FromClassId(classId);

                SetClassLayout();
                SetCantripLayout();
                SetPreparedSpellLayout();
                
                return;
              }
              else if(nuiEvent.ElementId.Contains("description"))
              {
                NwSpell spell =  NwSpell.FromSpellId(int.Parse(nuiEvent.ElementId.Split("_")[1]));

                if (!player.windows.TryGetValue("spellDescription", out var spellDescription)) player.windows.Add("spellDescription", new SpellDescriptionWindow(player, spell));
                else ((SpellDescriptionWindow)spellDescription).CreateWindow(spell);
              }
              else if (nuiEvent.ElementId.Contains("remove"))
              {
                NwSpell spell = NwSpell.FromSpellId(int.Parse(nuiEvent.ElementId.Split("_")[1]));
                player.oid.LoginCreature.GetClassInfo(selectedClass).KnownSpells[spell.GetSpellLevelForClass(selectedClass)].Remove(spell);
                SetPreparedSpellLayout();
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

        private void SetCantripLayout()
        {
          var classInfo = player.oid.LoginCreature.GetClassInfo(selectedClass);

          if (classInfo.KnownSpells[0].Count < 1)
          {
            rootColumn.Children.Remove(cantripGroup);
            rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
            return;
          }

          List<NuiElement> cantripChildren = new();

          cantripRow.Children = new()
          {
            new NuiColumn() { Children = new() { new NuiSpacer(), new NuiImage("ir_cantrips") { Tooltip = "Tours de magie", Height = 40, Width = 40, ImageAspect = NuiAspect.Fit }, new NuiSpacer() }, Width = 50 },
            new NuiColumn { Children = cantripChildren }
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
            rootColumn.Children.Insert(2, cantripGroup);

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
                new NuiColumn { Children = cantripChildren }
              };

              var classInfo = player.oid.LoginCreature.GetClassInfo(selectedClass);
              int preparedSpells = player.oid.LoginCreature.GetAbilityModifier(selectedClass.SpellCastingAbility)
                + player.oid.LoginCreature.GetClassInfo(selectedClass).Level;
              
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
                  cantripChildren.Add(new NuiColumn() { Children = spellChildren });
                  layoutHeight += 50;
                }

                if(readySpells.Count > 0)
                {
                  NwSpell spell = readySpells.First();
                  spellChildren.Add(new NuiButtonImage(spell.IconResRef) { Id = $"description_{spell.Id}", Tooltip = spell.Name.ToString(), Height = 40, Width = 40 });
                  spellChildren.Add(new NuiButtonImage("ir_emptytqs") { Id = $"remove_{spell.Id}", Tooltip = "Retirer", Height = 10, Width = 10 });
                  readySpells.Remove(spell);
                }
                else
                {
                  spellChildren.Add(new NuiButtonImage("ir_dis_trap ") { Tooltip = "Emplacement vide", Encouraged = true, Height = 40, Width = 40 });
                }
              }

              preparedSpellGroup.Height = layoutHeight;

              if (!rootColumn.Children.Contains(preparedSpellGroup))
              {
                if (rootColumn.Children.Contains(cantripGroup))
                  rootColumn.Children.Insert(3, preparedSpellGroup);
                else
                  rootColumn.Children.Insert(2, preparedSpellGroup);
              }

              rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
              preparedSpellGroup.SetLayout(player.oid, nuiToken.Token, preparedSpellRow);

              break;

            default:

                rootColumn.Children.Remove(preparedSpellGroup);
                rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);

              break;
          }
        }
      }
    }
  }
}
// ir_action

//var spellList = player.learnableSpells.Values.Where(c => c.currentLevel > 0 && c.multiplier == 1 && c.classes.Contains(selectedClass));
