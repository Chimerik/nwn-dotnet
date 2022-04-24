using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class EditorPNJWindow : PlayerWindow
      {
        private NwCreature targetCreature;
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiColumn layoutColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<string> name = new("name");
        private readonly NuiBind<string> tag = new("tag");

        private readonly NuiBind<string> raceSearch = new("raceSearch");
        private readonly NuiBind<List<NuiComboEntry>> race = new("race");
        private readonly NuiBind<int> raceSelected = new("raceSelected");
        private readonly NuiBind<int> racePortraitSelected = new("racePortraitSelected");

        private readonly NuiBind<string> apparenceSearch = new("apparenceSearch");
        private readonly NuiBind<List<NuiComboEntry>> apparence = new("apparence");
        private readonly NuiBind<int> apparenceSelected = new("apparenceSelected");

        private readonly NuiBind<int> genderSelected = new("genderSelected");
        private readonly NuiBind<int> genderPortraitSelected = new("genderPortraitSelected");

        private readonly NuiBind<string> portraits1 = new("portraits1");
        private readonly NuiBind<string> portraits2 = new("portraits2");
        private readonly NuiBind<string> portraits3 = new("portraits3");
        private readonly NuiBind<int> listCount = new("listCount");

        private readonly NuiBind<int> factionSelected = new("factionSelected");
        private readonly NuiBind<int> soundSetSelected = new("soundSetSelected");
        private readonly NuiBind<string> challengeRating = new("challengeRating");
        private readonly NuiBind<string> challengeRatingTooltip = new("challengeRatingTooltip");

        private readonly NuiBind<string> strength = new("strength");
        private readonly NuiBind<string> dexterity = new("dexterity");
        private readonly NuiBind<string> constitution = new("constitution");
        private readonly NuiBind<string> intelligence = new("intelligence");
        private readonly NuiBind<string> wisdom = new("wisdom");
        private readonly NuiBind<string> charisma = new("charisma");

        private readonly NuiBind<string> fortitude = new("fortitude");
        private readonly NuiBind<string> reflex = new("reflex");
        private readonly NuiBind<string> will = new("will");

        private readonly NuiBind<string> naturalAC = new("naturalAC"); // TOOLTIP : ajouter % réduction de dégâts
        private readonly NuiBind<string> naturalACTooltip = new("naturalACTooltip");
        private readonly NuiBind<string> dodgeChance = new("dodgeChance");
        private readonly NuiBind<string> hitPoints = new("hitPoints");
        private readonly NuiBind<int> movementRateSelected = new("movementRateSelected");

        public EditorPNJWindow(Player player, NwCreature targetCreature) : base(player)
        {
          windowId = "editorPNJ";

          rootGroup.Layout = layoutColumn;
          layoutColumn.Children = rootChildren;

          CreateWindow(targetCreature);
        }
        public void CreateWindow(NwCreature targetCreature)
        {
          this.targetCreature = targetCreature;
          LoadBaseLayout();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, 500);

          window = new NuiWindow(rootGroup, "Editeur de PNJ")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleEditorPNJEvents;
          player.oid.OnNuiEvent += HandleEditorPNJEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          LoadBaseBinding();

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void HandleEditorPNJEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "base":
                  LoadBaseLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.WindowToken, layoutColumn);
                  LoadBaseBinding();
                  break;

                case "portrait":
                  LoadPortraitLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.WindowToken, layoutColumn);
                  LoadPortraitBinding();
                  break;

                case "stats":
                  LoadStatsLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.WindowToken, layoutColumn);
                  LoadStatsBinding();
                  break;

                case "portraitSelect1":
                  targetCreature.PortraitResRef = portraits1.GetBindValues(player.oid, token)[nuiEvent.ArrayIndex];
                  targetCreature.PortraitResRef = targetCreature.PortraitResRef.Remove(targetCreature.PortraitResRef.Length - 1);
                  break;

                case "portraitSelect2":
                  targetCreature.PortraitResRef = portraits2.GetBindValues(player.oid, token)[nuiEvent.ArrayIndex];
                  targetCreature.PortraitResRef = targetCreature.PortraitResRef.Remove(targetCreature.PortraitResRef.Length - 1);
                  break;

                case "portraitSelect3":
                  targetCreature.PortraitResRef = portraits3.GetBindValues(player.oid, token)[nuiEvent.ArrayIndex];
                  targetCreature.PortraitResRef = targetCreature.PortraitResRef.Remove(targetCreature.PortraitResRef.Length - 1);
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "raceSearch":
                  string rSearch = raceSearch.GetBindValue(player.oid, token).ToLower();
                  race.SetBindValue(player.oid, token, string.IsNullOrEmpty(rSearch) ? Utils.raceList : Utils.raceList.Where(v => v.Label.ToLower().Contains(rSearch)).ToList());
                break;

                case "apparenceSearch":
                  string aSearch = apparenceSearch.GetBindValue(player.oid, token).ToLower();
                  apparence.SetBindValue(player.oid, token, string.IsNullOrEmpty(aSearch) ? Utils.apparenceList : Utils.apparenceList.Where(v => v.Label.ToLower().Contains(aSearch)).ToList());
                break;

                case "racePortraitSelected":
                case "genderPortraitSelected":
                  UpdatePortraitList();
                  break;

                case "name":
                  targetCreature.Name = name.GetBindValue(player.oid, token);
                  break;

                case "tag":
                  targetCreature.Tag = tag.GetBindValue(player.oid, token);
                  break;

                case "challengeRating":
                  if (float.TryParse(challengeRating.GetBindValue(player.oid, token), out float newCR))
                  {
                    targetCreature.ChallengeRating = newCR;
                    SetChallengeRatingTooltip();
                  }
                  break;

                case "raceSelected":
                  targetCreature.Race = NwRace.FromRaceId(raceSelected.GetBindValue(player.oid, token));
                  break;

                case "apparenceSelected":
                  targetCreature.Appearance = NwGameTables.AppearanceTable[apparenceSelected.GetBindValue(player.oid, token)];
                  break;

                case "factionSelected":
                  targetCreature.Faction = NwFaction.FromFactionId(factionSelected.GetBindValue(player.oid, token));
                  break;

                case "soundSetSelected":
                  targetCreature.SoundSet = (ushort)soundSetSelected.GetBindValue(player.oid, token);
                  break;

                case "strength":
                  if (byte.TryParse(strength.GetBindValue(player.oid, token), out byte newStrength))
                    targetCreature.SetsRawAbilityScore(Ability.Strength, (byte)(newStrength - targetCreature.Race.GetAbilityAdjustment(Ability.Strength)));
                    break;

                case "dexterity":
                  if (byte.TryParse(dexterity.GetBindValue(player.oid, token), out byte newDeterity))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Dexterity, (byte)(newDeterity - targetCreature.Race.GetAbilityAdjustment(Ability.Dexterity)));
                    reflex.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Reflex).ToString());
                    dodgeChance.SetBindValue(player.oid, token, Utils.GetDodgeChance(targetCreature).ToString());
                  }
                  break;

                case "constitution":
                  if (byte.TryParse(constitution.GetBindValue(player.oid, token), out byte newConstitution))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(newConstitution - targetCreature.Race.GetAbilityAdjustment(Ability.Constitution)));
                    fortitude.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Fortitude).ToString());
                  }
                  break;

                case "intelligence":
                  if (byte.TryParse(intelligence.GetBindValue(player.oid, token), out byte newIntelligence))
                    targetCreature.SetsRawAbilityScore(Ability.Intelligence, (byte)(newIntelligence - targetCreature.Race.GetAbilityAdjustment(Ability.Intelligence)));
                  break;

                case "wisdom":
                  if (byte.TryParse(wisdom.GetBindValue(player.oid, token), out byte newWisdom))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Wisdom, (byte)(newWisdom - targetCreature.Race.GetAbilityAdjustment(Ability.Wisdom)));
                    will.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Will).ToString());
                  }
                  break;

                case "charisma":
                  if (byte.TryParse(charisma.GetBindValue(player.oid, token), out byte newCharisma))
                    targetCreature.SetsRawAbilityScore(Ability.Charisma, (byte)(newCharisma - targetCreature.Race.GetAbilityAdjustment(Ability.Charisma)));
                  break;

                case "fortitude":
                  if (sbyte.TryParse(fortitude.GetBindValue(player.oid, token), out sbyte newFortitude))
                    targetCreature.SetBaseSavingThrow(SavingThrow.Fortitude, newFortitude);
                  break;

                case "reflex":
                  if (sbyte.TryParse(reflex.GetBindValue(player.oid, token), out sbyte newReflex))
                    targetCreature.SetBaseSavingThrow(SavingThrow.Reflex, newReflex);
                  break;

                case "will":
                  if (sbyte.TryParse(will.GetBindValue(player.oid, token), out sbyte newWill))
                    targetCreature.SetBaseSavingThrow(SavingThrow.Will, newWill);
                  break;

                case "naturalAC":
                  if (sbyte.TryParse(naturalAC.GetBindValue(player.oid, token), out sbyte newAC))
                  {
                    targetCreature.BaseAC = newAC;
                    naturalACTooltip.SetBindValue(player.oid, token, $"La créature subit naturellement {(Utils.GetDamageMultiplier(targetCreature.AC) * 100).ToString("0.##")} % des dégâts");
                  }
                  break;

                case "hitPoints":
                  if (int.TryParse(hitPoints.GetBindValue(player.oid, token), out int newHP))
                    targetCreature.MaxHP = newHP;
                  break;

                case "movementRateSelected":
                  targetCreature.MovementRate = (MovementRate)movementRateSelected.GetBindValue(player.oid, token);
                  break;
              }

              break;
          }
        }

        private void LoadButtons()
        {
          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButton("Base") { Id = "base", Height = 35, Width = 60 },
              new NuiButton("Portrait") { Id = "portrait", Height = 35, Width = 60 },
              new NuiButton("Stats") { Id = "stats", Height = 35, Width = 60 }
            }
          });
        }

        private void LoadBaseLayout()
        {
          rootChildren.Clear();

          LoadButtons();

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Nom") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Nom", name, 25, false) { Height = 35, Width = 200 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Tag") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Tag", tag, 30, false) { Height = 35, Width = 200 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Race") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = race, Selected = raceSelected },
              new NuiTextEdit("Recherche", raceSearch, 20, false) { Height = 35, Width = 100 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Apparence") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = apparence, Selected = apparenceSelected },
              new NuiTextEdit("Recherche", apparenceSearch, 20, false) { Height = 35, Width = 100 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Genre") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = Utils.genderList, Selected = genderSelected },
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Faction") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = Utils.factionList, Selected = factionSelected },
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Set sonore") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = Utils.soundSetList, Selected = soundSetSelected },
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("FP") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Tooltip = "Facteur de Puissance" },
              new NuiTextEdit("Facteur de Puissance", challengeRating, 2, false) { Height = 35, Width = 200, Tooltip = challengeRatingTooltip }
            }
          });
        }
        private void StopAllWatchBindings()
        {
          name.SetBindWatch(player.oid, token, false);
          tag.SetBindWatch(player.oid, token, false);
          challengeRating.SetBindWatch(player.oid, token, false);

          raceSelected.SetBindWatch(player.oid, token, false);
          raceSearch.SetBindWatch(player.oid, token, false);
          apparenceSelected.SetBindWatch(player.oid, token, false);
          apparenceSearch.SetBindWatch(player.oid, token, false);
          genderSelected.SetBindWatch(player.oid, token, false);
          factionSelected.SetBindWatch(player.oid, token, false);
          soundSetSelected.SetBindWatch(player.oid, token, false);

          racePortraitSelected.SetBindWatch(player.oid, token, false);
          genderPortraitSelected.SetBindWatch(player.oid, token, false);

          strength.SetBindWatch(player.oid, token, false);
          dexterity.SetBindWatch(player.oid, token, false);
          constitution.SetBindWatch(player.oid, token, false);
          intelligence.SetBindWatch(player.oid, token, false);
          wisdom.SetBindWatch(player.oid, token, false);
          charisma.SetBindWatch(player.oid, token, false);

          fortitude.SetBindWatch(player.oid, token, false);
          reflex.SetBindWatch(player.oid, token, false);
          will.SetBindWatch(player.oid, token, false);

          naturalAC.SetBindWatch(player.oid, token, false);
          hitPoints.SetBindWatch(player.oid, token, false);
          movementRateSelected.SetBindWatch(player.oid, token, false);
        }
        private void LoadBaseBinding()
        {
          StopAllWatchBindings();

          name.SetBindValue(player.oid, token, targetCreature.Name);
          name.SetBindWatch(player.oid, token, true);
          tag.SetBindValue(player.oid, token, targetCreature.Tag);
          tag.SetBindWatch(player.oid, token, true);
          challengeRating.SetBindValue(player.oid, token, targetCreature.ChallengeRating.ToString());
          SetChallengeRatingTooltip();
          challengeRating.SetBindWatch(player.oid, token, true);

          race.SetBindValue(player.oid, token, Utils.raceList);
          raceSelected.SetBindValue(player.oid, token, (int)targetCreature.Race.RacialType);
          raceSelected.SetBindWatch(player.oid, token, true);
          raceSearch.SetBindWatch(player.oid, token, true);

          apparence.SetBindValue(player.oid, token, Utils.apparenceList);
          apparenceSelected.SetBindValue(player.oid, token, targetCreature.Appearance.RowIndex);
          apparenceSelected.SetBindWatch(player.oid, token, true);
          apparenceSearch.SetBindWatch(player.oid, token, true);

          genderSelected.SetBindValue(player.oid, token, (int)targetCreature.Gender);
          genderSelected.SetBindWatch(player.oid, token, true);

          factionSelected.SetBindValue(player.oid, token, (int)targetCreature.Faction.StandardFactionType);
          factionSelected.SetBindWatch(player.oid, token, true);

          soundSetSelected.SetBindValue(player.oid, token, targetCreature.SoundSet);
          soundSetSelected.SetBindWatch(player.oid, token, true);
        }
        private void LoadPortraitLayout()
        {
          rootChildren.Clear();
          rowTemplate.Clear();

          LoadButtons();

          rowTemplate.Add(new NuiListTemplateCell(new NuiSpacer()) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits1) { Id = "portraitSelect1", Tooltip = portraits1 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits2) { Id = "portraitSelect2", Tooltip = portraits2 }) { Width = 64  });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits3) { Id = "portraitSelect3", Tooltip = portraits3 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiSpacer()) { VariableSize = true });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Race") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = race, Selected = racePortraitSelected }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Genre") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = Utils.genderList, Selected = genderPortraitSelected },
            }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 128 } } });
        }
        private void LoadPortraitBinding()
        {
          StopAllWatchBindings();

          race.SetBindValue(player.oid, token, Utils.raceList);
          racePortraitSelected.SetBindValue(player.oid, token, targetCreature.Race.Id);
          racePortraitSelected.SetBindWatch(player.oid, token, true);

          genderPortraitSelected.SetBindValue(player.oid, token, (int)targetCreature.Gender);
          genderPortraitSelected.SetBindWatch(player.oid, token, true);

          UpdatePortraitList();
        }
        private void UpdatePortraitList()
        {
          List<string>[] portraitList = new List<string>[] { new(), new(), new()};
          List<string> portraitTable = Portraits2da.portraitFilteredEntries[racePortraitSelected.GetBindValue(player.oid, token), genderPortraitSelected.GetBindValue(player.oid, token)];

          if(portraitTable != null)
            for (int i = 0; i < portraitTable.Count; i+=3)
              for(int j = 0; j < 3; j++)
                try { portraitList[j].Add(portraitTable[i + j]); }
                catch(Exception) { portraitList[j].Add(""); }

          portraits1.SetBindValues(player.oid, token, portraitList[0]);
          portraits2.SetBindValues(player.oid, token, portraitList[1]);
          portraits3.SetBindValues(player.oid, token, portraitList[2]);
          listCount.SetBindValue(player.oid, token, portraitList[0].Count);
        }
        private void LoadStatsLayout()
        {
          rootChildren.Clear();
          LoadButtons();

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Force") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Force", strength, 3, false) { Height = 35, Width = 70 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Dextérité") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Dextérité", dexterity, 3, false) { Height = 35, Width = 70 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Constitution") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Constitution", constitution, 3, false) { Height = 35, Width = 70 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Intelligence") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Intelligence", intelligence, 3, false) { Height = 35, Width = 70 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Sagesse") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Sagesse", wisdom, 3, false) { Height = 35, Width = 70 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Charisme") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Charisme", charisma, 3, false) { Height = 35, Width = 70 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Vigueur") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Vigueur", fortitude, 3, false) { Height = 35, Width = 70 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Réflexes") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Réflexes", reflex, 3, false) { Height = 35, Width = 70 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Volonté") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Volonté", will, 3, false) { Height = 35, Width = 70 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("CA Naturelle") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiTextEdit("Classe d'Armure Naturelle", naturalAC, 3, false) { Height = 35, Width = 70, Tooltip = naturalACTooltip }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Esquive") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle, Tooltip = "Pourcentage de chance d'éviter totalement une attaque ou un sort" },
              new NuiLabel(dodgeChance) { Height = 35, Width = 70, Tooltip = "Toute créature a 5 % de chance supplémentaire d'éviter une attaque d'une créature plus grande", VerticalAlign = NuiVAlign.Middle }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("HP") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle, Tooltip = "Hit Points" },
              new NuiTextEdit("Hit Points", hitPoints, 4, false) { Height = 35, Width = 70 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiLabel("Vitesse") { Height = 35, Width = 100, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = Utils.movementRateList, Selected = movementRateSelected }
            }
          });
        }
        private void SetChallengeRatingTooltip()
        {
          string critChance;
          string damageMultiplier;

          if (targetCreature.ChallengeRating < 11)
            critChance = "5 %";
          else
            critChance = $"{(int)targetCreature.ChallengeRating - 5} %";

          if (targetCreature.ChallengeRating < 1)
            damageMultiplier = "10 %";
          else
            damageMultiplier = $"{targetCreature.ChallengeRating * 10} %";

          challengeRatingTooltip.SetBindValue(player.oid, token, $"Effectue {damageMultiplier} de ses dégâts de base. {critChance} de chances de critiques");
        }
        private void LoadStatsBinding()
        {
          StopAllWatchBindings();

          strength.SetBindValue(player.oid, token, targetCreature.GetAbilityScore(Ability.Strength, true).ToString());
          dexterity.SetBindValue(player.oid, token, targetCreature.GetAbilityScore(Ability.Dexterity, true).ToString());
          constitution.SetBindValue(player.oid, token, targetCreature.GetAbilityScore(Ability.Constitution, true).ToString());
          intelligence.SetBindValue(player.oid, token, targetCreature.GetAbilityScore(Ability.Intelligence, true).ToString());
          wisdom.SetBindValue(player.oid, token, targetCreature.GetAbilityScore(Ability.Wisdom, true).ToString());
          charisma.SetBindValue(player.oid, token, targetCreature.GetAbilityScore(Ability.Charisma, true).ToString());

          fortitude.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Fortitude).ToString());
          reflex.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Reflex).ToString());
          will.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Will).ToString());

          naturalAC.SetBindValue(player.oid, token, targetCreature.BaseAC.ToString());
          naturalACTooltip.SetBindValue(player.oid, token, $"La créature subit naturellement {(Utils.GetDamageMultiplier(targetCreature.AC) * 100).ToString("0.##")} % des dégâts");

          dodgeChance.SetBindValue(player.oid, token, Utils.GetDodgeChance(targetCreature).ToString());
          hitPoints.SetBindValue(player.oid, token, targetCreature.MaxHP.ToString());
          movementRateSelected.SetBindValue(player.oid, token, (int)targetCreature.MovementRate);

          strength.SetBindWatch(player.oid, token, true);
          dexterity.SetBindWatch(player.oid, token, true);
          constitution.SetBindWatch(player.oid, token, true);
          intelligence.SetBindWatch(player.oid, token, true);
          wisdom.SetBindWatch(player.oid, token, true);
          charisma.SetBindWatch(player.oid, token, true);

          fortitude.SetBindWatch(player.oid, token, true);
          reflex.SetBindWatch(player.oid, token, true);
          will.SetBindWatch(player.oid, token, true);

          naturalAC.SetBindWatch(player.oid, token, true);
          hitPoints.SetBindWatch(player.oid, token, true);
          movementRateSelected.SetBindWatch(player.oid, token, true);
        }
      }
    }
  }
}
