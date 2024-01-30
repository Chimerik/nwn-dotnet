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
      public class ManoeuvreChoiceWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> weaponName = new("weaponName");
        private readonly NuiBind<bool> weaponChecked = new("weaponChecked");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<int> rowCount = new("rowCount");

        private readonly List<LearnableSkill> manoeuvres = new();
        private int nbManoeuvres;

        public ManoeuvreChoiceWindow(Player player, int nbManoeuvres) : base(player)
        {
          windowId = "manoeuvreChoice";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> abilitiesTemplate = new List<NuiListTemplateCell>
          {
            new(new NuiSpacer()),
            new(new NuiButtonImage(icon) { Id = "description" }) { Width = 40 },
            new(new NuiLabel(weaponName) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 160 },
            new(new NuiCheck("", weaponChecked) { Tooltip = "Apprendre la maîtrise de cette manoeuvre", Margin = 0.0f }) { Width = 40 },
            new(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(abilitiesTemplate, rowCount) { RowHeight = 40 } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80 }, new NuiSpacer() } });
          
          CreateWindow(nbManoeuvres);
        }
        public void CreateWindow(int nbManoeuvres)
        {
          this.nbManoeuvres = nbManoeuvres;
          manoeuvres.Clear();

          foreach (var manoeuvre in SkillSystem.learnableDictionary.Values.Where(l => l is LearnableSkill learnable && learnable.category == SkillSystem.Category.Manoeuvre))
            if (!player.learnableSkills.TryGetValue(manoeuvre.id, out LearnableSkill learnable) || learnable.currentLevel < 1)
              manoeuvres.Add((LearnableSkill)manoeuvre);

          if (manoeuvres.Count < 1)
          {
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Delete();
            CloseWindow();
            return;
          }

          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiScaledWidth * 0.4f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, $"Choisissez {nbManoeuvres} manoeuvre(s)")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = true,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleWeaponMasterEvents;

            LoadWeaponList();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleWeaponMasterEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":

                  var checkedList = weaponChecked.GetBindValues(player.oid, nuiToken.Token);
                  List<LearnableSkill> toLearn = new();

                  for (int i = 0; i < checkedList.Count; i++)
                    if (checkedList[i])
                      toLearn.Add(manoeuvres[i]);

                  if (toLearn.Count > nbManoeuvres)
                  {
                    player.oid.SendServerMessage($"Veuillez sélectionner jusqu'à {nbManoeuvres} maîtrises");
                    return;
                  }

                  if (toLearn.Count == nbManoeuvres || manoeuvres.Count == toLearn.Count || manoeuvres.Count < 1)
                  {
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Delete();

                    foreach (var learnable in toLearn)
                    {
                      player.learnableSkills.TryAdd(learnable.id, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[learnable.id], player, (int)SkillSystem.Category.Class));
                      player.learnableSkills[learnable.id].LevelUp(player);
                    }

                    player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));

                    CloseWindow();
                  }
                  else
                  {
                    int nbMaitrises = manoeuvres.Count >= nbManoeuvres ? nbManoeuvres : manoeuvres.Count;
                    player.oid.SendServerMessage($"Veuillez sélectionner jusqu'à {nbMaitrises} manoeuvres avant de valider");
                  }

                  return;
              }

              int learnableId = manoeuvres.ElementAt(nuiEvent.ArrayIndex).id;

              if (!player.windows.ContainsKey("learnableDescription")) player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, learnableId));
              else ((LearnableDescriptionWindow)player.windows["learnableDescription"]).CreateWindow(learnableId);

              break;
          }
        }
        private void LoadWeaponList()
        {
          List<string> iconList = new List<string>();
          List<string> manoeuvreNameList = new();
          List<bool> manoeuvreCheckList = new();
          List<bool> manoeuvreCheckBinding = weaponChecked?.GetBindValues(player.oid, nuiToken.Token);
          int i = 0;

          foreach (var manoeuvre in manoeuvres)
          {
            iconList.Add(manoeuvre.icon);
            manoeuvreNameList.Add(manoeuvre.name);
            manoeuvreCheckList.Add(manoeuvreCheckBinding is not null && manoeuvreCheckBinding[i]);
          }

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          weaponName.SetBindValues(player.oid, nuiToken.Token, manoeuvreNameList);
          weaponChecked.SetBindValues(player.oid, nuiToken.Token, manoeuvreCheckList);
          rowCount.SetBindValue(player.oid, nuiToken.Token, manoeuvres.Count);
        }
      }
    }
  }
}
