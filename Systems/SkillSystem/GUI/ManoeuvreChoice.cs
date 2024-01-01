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
        private readonly NuiBind<int> rowCount = new("rowCount");

        private readonly List<LearnableSkill> manoeuvres = new();
        private int weaponsChecked;
        private int nbManoeuvres;

        public ManoeuvreChoiceWindow(Player player, int nbManoeuvres) : base(player)
        {
          windowId = "manoeuvreChoice";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> abilitiesTemplate = new List<NuiListTemplateCell>
          {
            new(new NuiSpacer()),
            new(new NuiLabel(weaponName) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 120 },
            new(new NuiCheck("", weaponChecked) { Tooltip = "Apprendre la maîtrise de cette manoeuvre", Margin = 0.0f }) { Width = 40 },
            new(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiLabel("Choisissez trois manoeuvres") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(abilitiesTemplate, rowCount) { RowHeight = 40, Scrollbars = NuiScrollbars.None } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80 }, new NuiSpacer() } });
          
          CreateWindow(nbManoeuvres);
        }
        public void CreateWindow(int nbManoeuvres)
        {
          this.nbManoeuvres = nbManoeuvres;
          manoeuvres.Clear();

          foreach (var manoeuvre in SkillSystem.learnableDictionary.Values.Where(l => ((LearnableSkill)l).category == SkillSystem.Category.Manoeuvre))
            if (!player.learnableSkills.TryGetValue(manoeuvre.id, out LearnableSkill learnable) || learnable.currentLevel < 1)
              manoeuvres.Add((LearnableSkill)manoeuvre);

          if (manoeuvres.Count < 1)
          {
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Delete();
            CloseWindow();
            return;
          }

          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiScaledWidth * 0.4f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, $"Choisissez {nbManoeuvres} manoeuvres")
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

                  if (weaponsChecked > nbManoeuvres)
                  {
                    player.oid.SendServerMessage($"Veuillez sélectionner jusqu'à {nbManoeuvres} maîtrises");
                    return;
                  }

                  if (weaponsChecked >= nbManoeuvres || manoeuvres.Count == weaponsChecked || manoeuvres.Count < 1)
                  {
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MANOEUVRE_CHOICE").Delete();
                    CloseWindow();

                    var checkedList = weaponChecked.GetBindValues(player.oid, nuiToken.Token);

                    for (int i = 0; i < checkedList.Count; i++)
                    {
                      if (checkedList[i])
                      {
                        LearnableSkill manoeuvre = manoeuvres[i]; 
                        player.learnableSkills.TryAdd(manoeuvre.id, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ClubProficiency], (int)SkillSystem.Category.Class));
                        player.learnableSkills[manoeuvre.id].LevelUp(player);
                      }
                    }

                    player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
                  }
                  else
                  {
                    int nbMaitrises = manoeuvres.Count > 3 ? 4 : manoeuvres.Count;
                    player.oid.SendServerMessage($"Veuillez sélectionner jusqu'à {nbMaitrises} manoeuvres avant de valider");
                  }

                  return;
              }

              break;
          }
        }
        private void LoadWeaponList()
        {
          List<string> manoeuvreNameList = new();
          List<bool> manoeuvreCheckList = new();
          List<bool> manoeuvreCheckBinding = weaponChecked?.GetBindValues(player.oid, nuiToken.Token);
          int i = 0;
          weaponsChecked = 0;

          foreach (var manoeuvre in manoeuvres)
          {
            manoeuvreNameList.Add(manoeuvre.name);
            if (manoeuvreCheckBinding is not null && manoeuvreCheckBinding[i])
            {
              weaponsChecked++;
              manoeuvreCheckList.Add(true);
            }
            else
              manoeuvreCheckList.Add(false);
          }

          weaponName.SetBindValues(player.oid, nuiToken.Token, manoeuvreNameList);
          weaponChecked.SetBindValues(player.oid, nuiToken.Token, manoeuvreCheckList);
          rowCount.SetBindValue(player.oid, nuiToken.Token, manoeuvres.Count);
        }
      }
    }
  }
}
