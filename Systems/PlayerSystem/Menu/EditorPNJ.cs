using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core.NWNX;

using SpecialAbility = Anvil.API.SpecialAbility;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class EditorPNJWindow : PlayerWindow
      {
        private enum Tab
        {
          Base,
          Portrait,
          Description,
          Stats,
          Feat,
          Spell,
          Model,
          Patrouille,
          Variables
        }

        private NwCreature targetCreature;
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiColumn layoutColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<string> name = new("name");
        private readonly NuiBind<string> tag = new("tag");
        private readonly NuiBind<string> size = new("size");

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

        private readonly NuiBind<string> strengthModifier = new("strengthModifier");
        private readonly NuiBind<string> dexterityModifier = new("dexterityModifier");
        private readonly NuiBind<string> constitutionModifier = new("constitutionModifier");
        private readonly NuiBind<string> intelligenceModifier = new("intelligenceModifier");
        private readonly NuiBind<string> wisdomModifier = new("wisdomModifier");
        private readonly NuiBind<string> charismaModifier = new("charismaModifier");

        private readonly NuiBind<string> fortitude = new("fortitude");
        private readonly NuiBind<string> reflex = new("reflex");
        private readonly NuiBind<string> will = new("will");
        private readonly NuiBind<string> fortitudeTooltip = new("fortitudeTooltip");
        private readonly NuiBind<string> reflexTooltip = new("reflexTooltip");
        private readonly NuiBind<string> willTooltip = new("willTooltip");

        private readonly NuiBind<string> naturalAC = new("naturalAC"); // TOOLTIP : ajouter % réduction de dégâts
        private readonly NuiBind<string> naturalACTooltip = new("naturalACTooltip");
        private readonly NuiBind<string> dodgeChance = new("dodgeChance");
        private readonly NuiBind<string> hitPoints = new("hitPoints");
        private readonly NuiBind<int> movementRateSelected = new("movementRateSelected");

        private readonly NuiBind<string> armorPenetration = new("armorPenetration");
        private readonly NuiBind<string> attackPerRound = new("attackPerRound");
        private readonly NuiBind<string> spellCasterLevel = new("spellCasterLevel");

        private readonly NuiBind<int> listAcquiredFeatCount = new("listAcquiredFeatCount");
        private readonly NuiBind<string> availableFeatIcons = new("availableFeatIcons");
        private readonly NuiBind<string> acquiredFeatIcons = new("acquiredFeatIcons");
        private readonly NuiBind<string> availableFeatNames = new("availableFeatNames");
        private readonly NuiBind<string> acquiredFeatNames = new("acquiredFeatNames");
        private readonly NuiBind<string> availableFeatSearch = new("availableFeatSearch");
        private readonly NuiBind<string> acquiredFeatSearch = new("acquiredFeatSearch");
        private readonly NuiBind<string> spellQuantity = new("spellQuantity");
        private readonly NuiBind<bool> spellQuantityEnabled = new("spellQuantityEnabled");
        private int spellQuantityIndexSelected;

        private readonly NuiBind<string> creatureDescription = new("creatureDescription");
        private readonly NuiBind<string> creatureComment = new("creatureComment");

        private readonly List<NwFeat> availableFeats = new();
        private readonly List<NwFeat> acquiredFeats = new();
        private List<NwFeat> availableFeatSearcher = new();
        private List<NwFeat> acquiredFeatSearcher = new();

        private readonly List<NwSpell> availableSpells = new();
        private readonly List<NwSpell> acquiredSpells = new();
        private List<NwSpell> availableSpellSearcher = new();
        private List<NwSpell> acquiredSpellSearcher = new();

        private readonly NuiBind<string> variableName = new("variableName");
        private readonly NuiBind<string> variableValue = new("variableValue");
        private readonly NuiBind<int> selectedVariableType = new("selectedVariableType");

        private readonly NuiBind<string> newVariableName = new("newVariableName");
        private readonly NuiBind<string> newVariableValue = new("newVariableValue");
        private readonly NuiBind<int> selectedNewVariableType = new("selectedNewVariableType");

        private readonly NuiBind<bool> permanentSpawn = new("permanentSpawn");
        private readonly NuiBind<int> selectedSpawnType = new("selectedSpawnType");
        private readonly NuiBind<bool> spawnOptionsVisibility = new("spawnOptionsVisibility");
        private int previousType;

        List<NuiComboEntry> filteredEntries;

        Tab currentTab;

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

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 520, 500);

          window = new NuiWindow(rootGroup, $"Modification de {targetCreature.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            currentTab = Tab.Base;

            nuiToken.OnNuiEvent += HandleEditorPNJEvents;

            LoadBaseBinding();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleEditorPNJEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (targetCreature == null || !targetCreature.IsValid)
          {
            player.oid.SendServerMessage("La créature éditée n'est plus valide.", ColorConstants.Red);
            CloseWindow();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.MouseUp:

              switch (nuiEvent.ElementId)
              {
                case "availableFeatDescription":

                  switch (currentTab)
                  {
                    case Tab.Feat:

                      if (!player.windows.ContainsKey("featDescription")) player.windows.Add("featDescription", new FeatDescriptionWindow(player, availableFeatSearcher[nuiEvent.ArrayIndex]));
                      else ((FeatDescriptionWindow)player.windows["featDescription"]).CreateWindow(availableFeatSearcher[nuiEvent.ArrayIndex]);

                      break;

                    case Tab.Spell:

                      if (!player.windows.ContainsKey("spellDescription")) player.windows.Add("spellDescription", new SpellDescriptionWindow(player, availableSpellSearcher[nuiEvent.ArrayIndex]));
                      else ((SpellDescriptionWindow)player.windows["spellDescription"]).CreateWindow(availableSpellSearcher[nuiEvent.ArrayIndex]);

                      break;
                  }

                  break;

                case "acquiredFeatDescription":

                  switch (currentTab)
                  {
                    case Tab.Feat:

                      if (!player.windows.ContainsKey("featDescription")) player.windows.Add("featDescription", new FeatDescriptionWindow(player, acquiredFeatSearcher[nuiEvent.ArrayIndex]));
                      else ((FeatDescriptionWindow)player.windows["featDescription"]).CreateWindow(acquiredFeatSearcher[nuiEvent.ArrayIndex]);

                      break;

                    case Tab.Spell:

                      if (!player.windows.ContainsKey("spellDescription")) player.windows.Add("spellDescription", new SpellDescriptionWindow(player, acquiredSpellSearcher[nuiEvent.ArrayIndex]));
                      else ((SpellDescriptionWindow)player.windows["spellDescription"]).CreateWindow(acquiredSpellSearcher[nuiEvent.ArrayIndex]);

                      break;
                  }


                  break;

                case "spellQuantityText":
                  spellQuantityIndexSelected = nuiEvent.ArrayIndex;
                  break;
              }

              break;

            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "base":
                  currentTab = Tab.Base;
                  LoadBaseLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadBaseBinding();
                  break;

                case "portrait":
                  currentTab = Tab.Portrait;
                  LoadPortraitLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadPortraitBinding();
                  break;

                case "description":
                  currentTab = Tab.Description;
                  LoadDescriptionLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadDescriptionBinding();
                  break;

                case "stats":
                  currentTab = Tab.Stats;
                  LoadStatsLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadStatsBinding();
                  break;

                case "feats":
                  currentTab = Tab.Feat;
                  LoadFeatsLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadFeatsBinding();
                  break;

                case "spells":
                  currentTab = Tab.Spell;
                  LoadSpellsLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadSpellsBinding();
                  break;

                case "appearance":

                  currentTab = Tab.Model;
                  LoadModelLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadModelBinding();

                  break;

                case "appearanceDynamic":

                  if (targetCreature.Appearance.Race.Length > 1 && targetCreature.Appearance.Race != "S")
                  {
                    player.oid.SendServerMessage($"Le modèle actuellement sélectionné ({targetCreature.Appearance.Label}) n'est pas dynamique. Il n'est pas possible de le personnaliser davantage.", ColorConstants.Red);
                    return;
                  }

                  if (targetCreature.Gender != Gender.Male && targetCreature.Gender != Gender.Female)
                  {
                    player.oid.SendServerMessage($"Le genre de la créature doit être masculin ou féminin afin de pouvoir utiliser la modification corporelle.", ColorConstants.Red);
                    return;
                  }

                  if (!player.windows.ContainsKey("bodyAppearanceModifier")) player.windows.Add("bodyAppearanceModifier", new BodyAppearanceWindow(player, targetCreature));
                  else ((BodyAppearanceWindow)player.windows["bodyAppearanceModifier"]).CreateWindow(targetCreature);

                  break;

                case "variables":

                  currentTab = Tab.Variables;
                  LoadVariablesLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadVariablesBinding();

                  break;

                case "appearancePrev": HandleAppearanceSearch(-1); break;
                case "appearanceNext": HandleAppearanceSearch(1); break;

                case "portraitSelect1":
                  targetCreature.PortraitResRef = portraits1.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];
                  targetCreature.PortraitResRef = targetCreature.PortraitResRef.Remove(targetCreature.PortraitResRef.Length - 1);
                  break;

                case "portraitSelect2":
                  targetCreature.PortraitResRef = portraits2.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];
                  targetCreature.PortraitResRef = targetCreature.PortraitResRef.Remove(targetCreature.PortraitResRef.Length - 1);
                  break;

                case "portraitSelect3":
                  targetCreature.PortraitResRef = portraits3.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];
                  targetCreature.PortraitResRef = targetCreature.PortraitResRef.Remove(targetCreature.PortraitResRef.Length - 1);
                  break;

                case "selectFeat":
                  NwFeat acquiredFeat = availableFeatSearcher[nuiEvent.ArrayIndex];

                  targetCreature.AddFeat(acquiredFeat);

                  if (!acquiredFeats.Contains(acquiredFeat))
                    acquiredFeats.Add(acquiredFeat);

                  if (!acquiredFeatSearcher.Contains(acquiredFeat))
                    acquiredFeatSearcher.Add(acquiredFeat);

                  availableFeats.Remove(acquiredFeat);
                  availableFeatSearcher.Remove(acquiredFeat);

                  var tempIcon = availableFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempIcon.RemoveAt(nuiEvent.ArrayIndex);
                  var tempName = availableFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempName.RemoveAt(nuiEvent.ArrayIndex);

                  availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempIcon);
                  availableFeatNames.SetBindValues(player.oid, nuiToken.Token, tempName);
                  listCount.SetBindValue(player.oid, nuiToken.Token, tempName.Count);

                  tempIcon = acquiredFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempIcon.Add(acquiredFeat.IconResRef);
                  tempName = acquiredFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempName.Add(acquiredFeat.Name.ToString());

                  acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempIcon);
                  acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, tempName);
                  listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, tempName.Count);

                  break;

                case "removeFeat":
                  NwFeat removedFeat = acquiredFeatSearcher[nuiEvent.ArrayIndex];

                  targetCreature.RemoveFeat(removedFeat);

                  if (!availableFeats.Contains(removedFeat))
                    availableFeats.Add(removedFeat);

                  if (!availableFeatSearcher.Contains(removedFeat))
                    availableFeatSearcher.Add(removedFeat);

                  acquiredFeats.Remove(removedFeat);
                  acquiredFeatSearcher.Remove(removedFeat);

                  var tempIconList = acquiredFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempIconList.RemoveAt(nuiEvent.ArrayIndex);
                  var tempNameList = acquiredFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempNameList.RemoveAt(nuiEvent.ArrayIndex);

                  acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempIconList);
                  acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, tempNameList);
                  listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, tempNameList.Count);

                  tempIconList = availableFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempIconList.Add(removedFeat.IconResRef);
                  tempNameList = availableFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempNameList.Add(removedFeat.Name.ToString());

                  availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempIconList);
                  availableFeatNames.SetBindValues(player.oid, nuiToken.Token, tempNameList);
                  listCount.SetBindValue(player.oid, nuiToken.Token, tempNameList.Count);

                  break;

                case "selectSpell":

                  spellQuantity.SetBindWatch(player.oid, nuiToken.Token, false);

                  NwSpell acquiredSpell = availableSpellSearcher[nuiEvent.ArrayIndex];

                  targetCreature.AddSpecialAbility(new SpecialAbility(acquiredSpell, 1));

                  if (!acquiredSpells.Contains(acquiredSpell))
                    acquiredSpells.Add(acquiredSpell);

                  if (!acquiredSpellSearcher.Contains(acquiredSpell))
                    acquiredSpellSearcher.Add(acquiredSpell);

                  availableSpells.Remove(acquiredSpell);
                  availableSpellSearcher.Remove(acquiredSpell);

                  var tempSpellIcon = availableFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellIcon.RemoveAt(nuiEvent.ArrayIndex);
                  var tempSpellName = availableFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellName.RemoveAt(nuiEvent.ArrayIndex);

                  availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempSpellIcon);
                  availableFeatNames.SetBindValues(player.oid, nuiToken.Token, tempSpellName);
                  listCount.SetBindValue(player.oid, nuiToken.Token, tempSpellName.Count);

                  tempSpellIcon = acquiredFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellIcon.Add(acquiredSpell.IconResRef);
                  tempSpellName = acquiredFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellName.Add(acquiredSpell.Name.ToString());
                  var tempSpellQuantity = spellQuantity.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellQuantity.Add("1");

                  var tempSpellQuantityEnabled = spellQuantityEnabled.GetBindValues(player.oid, nuiToken.Token);

                  if (SpellUtils.IsSpellBuff(acquiredSpell))
                    tempSpellQuantityEnabled.Add(false);
                  else
                    tempSpellQuantityEnabled.Add(true);

                  acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempSpellIcon);
                  acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, tempSpellName);
                  spellQuantity.SetBindValues(player.oid, nuiToken.Token, tempSpellQuantity);
                  spellQuantityEnabled.SetBindValues(player.oid, nuiToken.Token, tempSpellQuantityEnabled);
                  listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, tempSpellName.Count);

                  spellQuantity.SetBindWatch(player.oid, nuiToken.Token, true);

                  break;

                case "removeSpell":

                  spellQuantity.SetBindWatch(player.oid, nuiToken.Token, false);

                  NwSpell removedSpell = acquiredSpellSearcher[nuiEvent.ArrayIndex];
                  int i = 0;

                  foreach (SpecialAbility spell in targetCreature.SpecialAbilities)
                  {
                    if (spell.Spell == removedSpell)
                    {
                      Task resetClassOnNextFrame = NwTask.Run(async () =>
                      {
                        await NwTask.Delay(TimeSpan.FromMilliseconds(10));
                        targetCreature.RemoveSpecialAbilityAt(i);
                      });

                      i++;
                    }
                  }

                  if (!availableSpells.Contains(removedSpell))
                    availableSpells.Add(removedSpell);

                  if (!availableSpellSearcher.Contains(removedSpell))
                    availableSpellSearcher.Add(removedSpell);

                  acquiredSpells.Remove(removedSpell);
                  acquiredSpellSearcher.Remove(removedSpell);

                  var tempSpellIconList = acquiredFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellIconList.RemoveAt(nuiEvent.ArrayIndex);
                  var tempSpellNameList = acquiredFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellNameList.RemoveAt(nuiEvent.ArrayIndex);
                  var tempSpellQuantityList = spellQuantity.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellQuantityList.RemoveAt(nuiEvent.ArrayIndex);
                  var tempSpellQuantityEnabledList = spellQuantityEnabled.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellQuantityEnabledList.RemoveAt(nuiEvent.ArrayIndex);

                  acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempSpellIconList);
                  acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, tempSpellNameList);
                  spellQuantity.SetBindValues(player.oid, nuiToken.Token, tempSpellQuantityList);
                  spellQuantityEnabled.SetBindValues(player.oid, nuiToken.Token, tempSpellQuantityEnabledList);
                  listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, tempSpellNameList.Count);

                  tempSpellIconList = availableFeatIcons.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellIconList.Add(removedSpell.IconResRef);
                  tempSpellNameList = availableFeatNames.GetBindValues(player.oid, nuiToken.Token);
                  tempSpellNameList.Add(removedSpell.Name.ToString());

                  availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, tempSpellIconList);
                  availableFeatNames.SetBindValues(player.oid, nuiToken.Token, tempSpellNameList);
                  listCount.SetBindValue(player.oid, nuiToken.Token, tempSpellNameList.Count);

                  spellQuantity.SetBindWatch(player.oid, nuiToken.Token, true);

                  break;

                case "saveDescription":
                  targetCreature.Description = creatureDescription.GetBindValue(player.oid, nuiToken.Token);
                  targetCreature.GetObjectVariable<LocalVariableString>("_COMMENT").Value = creatureComment.GetBindValue(player.oid, nuiToken.Token);
                  player.oid.SendServerMessage($"La description et le commentaire de la créature {targetCreature.Name.ColorString(ColorConstants.White)} ont bien été enregistrées.", new Color(32, 255, 32));
                  break;

                case "saveNewVariable":
                  Utils.ConvertLocalVariable(newVariableName.GetBindValue(player.oid, nuiToken.Token), newVariableValue.GetBindValue(player.oid, nuiToken.Token), selectedNewVariableType.GetBindValue(player.oid, nuiToken.Token), targetCreature, player.oid);
                  LoadVariablesBinding();
                  break;

                case "saveVariable":
                  Utils.ConvertLocalVariable(variableName.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex], variableValue.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex], selectedVariableType.GetBindValue(player.oid, nuiToken.Token), targetCreature, player.oid);
                  LoadVariablesBinding();
                  break;

                case "deleteVariable":
                  targetCreature.LocalVariables.ElementAt(nuiEvent.ArrayIndex).Delete();
                  LoadVariablesBinding();
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "raceSearch":
                  string rSearch = raceSearch.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  race.SetBindValue(player.oid, nuiToken.Token, string.IsNullOrEmpty(rSearch) ? Utils.raceList : Utils.raceList.Where(v => v.Label.ToLower().Contains(rSearch)).ToList());
                  break;

                case "apparenceSearch":
                  string aSearch = apparenceSearch.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  filteredEntries = string.IsNullOrEmpty(aSearch) ? Utils.appearanceEntries : Utils.appearanceEntries.Where(v => v.Label.ToLower().Contains(aSearch)).ToList();
                  apparence.SetBindValue(player.oid, nuiToken.Token, filteredEntries);
                  break;

                case "acquiredFeatSearch":

                  string acquiredFSearch = acquiredFeatSearch.GetBindValue(player.oid, nuiToken.Token).ToLower();

                  switch (currentTab)
                  {
                    case Tab.Feat:
                      acquiredFeatSearcher = string.IsNullOrEmpty(acquiredFSearch) ? acquiredFeats : acquiredFeats.Where(f => f.Name.ToString().ToLower().Contains(acquiredFSearch)).ToList();

                      acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, acquiredFeatSearcher.Select(f => f.IconResRef));
                      acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, acquiredFeatSearcher.Select(f => f.Name.ToString().Replace("’", "'")));
                      listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, acquiredFeatSearcher.Count);
                      break;

                    case Tab.Spell:
                      acquiredSpellSearcher = string.IsNullOrEmpty(acquiredFSearch) ? acquiredSpells : acquiredSpells.Where(s => s.Name.ToString().ToLower().Contains(acquiredFSearch)).ToList();

                      acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, acquiredSpellSearcher.Select(s => s.IconResRef));
                      acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, acquiredSpellSearcher.Select(s => s.Name.ToString().Replace("’", "'")));
                      listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, acquiredSpellSearcher.Count);
                      break;
                  }

                  break;

                case "availableFeatSearch":
                  string availableFSearch = availableFeatSearch.GetBindValue(player.oid, nuiToken.Token).ToLower();

                  switch (currentTab)
                  {
                    case Tab.Feat:
                      availableFeatSearcher = string.IsNullOrEmpty(availableFSearch) ? availableFeats : availableFeats.Where(f => f.Name.ToString().ToLower().Contains(availableFSearch)).ToList();

                      availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, availableFeatSearcher.Select(f => f.IconResRef));
                      availableFeatNames.SetBindValues(player.oid, nuiToken.Token, availableFeatSearcher.Select(f => f.Name.ToString().Replace("’", "'")));
                      listCount.SetBindValue(player.oid, nuiToken.Token, availableFeatSearcher.Count);
                      break;

                    case Tab.Spell:
                      availableSpellSearcher = string.IsNullOrEmpty(availableFSearch) ? availableSpells : availableSpells.Where(s => s.Name.ToString().ToLower().Contains(availableFSearch)).ToList();

                      availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, availableSpellSearcher.Select(s => s.IconResRef));
                      availableFeatNames.SetBindValues(player.oid, nuiToken.Token, availableSpellSearcher.Select(s => s.Name.ToString().Replace("’", "'")));
                      listCount.SetBindValue(player.oid, nuiToken.Token, availableSpellSearcher.Count);
                      break;
                  }


                  break;

                case "racePortraitSelected":
                case "genderPortraitSelected":
                  UpdatePortraitList();
                  break;

                case "name":
                  targetCreature.Name = name.GetBindValue(player.oid, nuiToken.Token);
                  //targetCreature.OriginalFirstName = name.GetBindValue(player.oid, nuiToken.Token);
                  //targetCreature.OriginalLastName = name.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "tag":
                  targetCreature.Tag = tag.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "size":

                  if (float.TryParse(size.GetBindValue(player.oid, nuiToken.Token), out float newSize))
                  {
                    targetCreature.VisualTransform.Scale = newSize > 100 ? 100 : newSize;

                    if (targetCreature.GetObjectVariable<LocalVariableFloat>("_CREATURE_PERSONNAL_SPACE").HasValue)
                    {
                      CreaturePlugin.SetCreaturePersonalSpace(targetCreature, targetCreature.GetObjectVariable<LocalVariableFloat>("_CREATURE_PERSONNAL_SPACE").Value * newSize);
                      CreaturePlugin.SetHeight(targetCreature, targetCreature.GetObjectVariable<LocalVariableFloat>("_HEIGHT").Value * newSize);
                      CreaturePlugin.SetHitDistance(targetCreature, targetCreature.GetObjectVariable<LocalVariableFloat>("_HIT_DISTANCE").Value * newSize);
                      CreaturePlugin.SetPersonalSpace(targetCreature, targetCreature.GetObjectVariable<LocalVariableFloat>("_PERSONNAL_SPACE").Value * newSize); ;
                    }
                  }

                  break;

                case "challengeRating":
                  if (float.TryParse(challengeRating.GetBindValue(player.oid, nuiToken.Token), out float newCR))
                  {
                    targetCreature.ChallengeRating = newCR;
                    SetChallengeRatingTooltip();
                  }
                  break;

                case "raceSelected":
                  targetCreature.Race = NwRace.FromRaceId(raceSelected.GetBindValue(player.oid, nuiToken.Token));
                  break;

                case "genderSelected":
                  targetCreature.Gender = (Gender)genderSelected.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "apparenceSelected":
                  //Log.Info($"selectedAppearance : {apparenceSelected.GetBindValue(player.oid, nuiToken.Token)}");
                  targetCreature.Appearance = NwGameTables.AppearanceTable[apparenceSelected.GetBindValue(player.oid, nuiToken.Token)];
                  break;

                case "factionSelected":
                  targetCreature.Faction = NwFaction.FromFactionId(factionSelected.GetBindValue(player.oid, nuiToken.Token));
                  break;

                case "soundSetSelected":
                  targetCreature.SoundSet = (ushort)soundSetSelected.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "strength":
                  if (byte.TryParse(strength.GetBindValue(player.oid, nuiToken.Token), out byte newStrength))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Strength, (byte)(newStrength - targetCreature.Race.GetAbilityAdjustment(Ability.Strength)));
                    strengthModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Strength).ToString());
                  }
                  break;

                case "dexterity":
                  if (byte.TryParse(dexterity.GetBindValue(player.oid, nuiToken.Token), out byte newDeterity))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Dexterity, (byte)(newDeterity - targetCreature.Race.GetAbilityAdjustment(Ability.Dexterity)));
                    reflex.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetBaseSavingThrow(SavingThrow.Reflex).ToString());
                    dodgeChance.SetBindValue(player.oid, nuiToken.Token, Utils.GetDodgeChance(targetCreature).ToString());
                    dexterityModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Dexterity).ToString());
                    reflexTooltip.SetBindValue(player.oid, nuiToken.Token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Reflex) + targetCreature.GetAbilityModifier(Ability.Dexterity)}");
                  }
                  break;

                case "constitution":
                  if (byte.TryParse(constitution.GetBindValue(player.oid, nuiToken.Token), out byte newConstitution))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(newConstitution - targetCreature.Race.GetAbilityAdjustment(Ability.Constitution)));
                    fortitude.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetBaseSavingThrow(SavingThrow.Fortitude).ToString());
                    constitutionModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Constitution).ToString());
                    fortitudeTooltip.SetBindValue(player.oid, nuiToken.Token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Fortitude) + targetCreature.GetAbilityModifier(Ability.Constitution)}");
                  }
                  break;

                case "intelligence":
                  if (byte.TryParse(intelligence.GetBindValue(player.oid, nuiToken.Token), out byte newIntelligence))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Intelligence, (byte)(newIntelligence - targetCreature.Race.GetAbilityAdjustment(Ability.Intelligence)));
                    intelligenceModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Intelligence).ToString());
                  }
                  break;

                case "wisdom":
                  if (byte.TryParse(wisdom.GetBindValue(player.oid, nuiToken.Token), out byte newWisdom))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Wisdom, (byte)(newWisdom - targetCreature.Race.GetAbilityAdjustment(Ability.Wisdom)));
                    will.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetBaseSavingThrow(SavingThrow.Will).ToString());
                    wisdomModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Wisdom).ToString());
                    willTooltip.SetBindValue(player.oid, nuiToken.Token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Will) + targetCreature.GetAbilityModifier(Ability.Wisdom)}");
                  }
                  break;

                case "charisma":
                  if (byte.TryParse(charisma.GetBindValue(player.oid, nuiToken.Token), out byte newCharisma))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Charisma, (byte)(newCharisma - targetCreature.Race.GetAbilityAdjustment(Ability.Charisma)));
                    charismaModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Charisma).ToString());
                  }
                  break;

                case "fortitude":
                  if (sbyte.TryParse(fortitude.GetBindValue(player.oid, nuiToken.Token), out sbyte newFortitude))
                    targetCreature.SetBaseSavingThrow(SavingThrow.Fortitude, newFortitude);
                  break;

                case "reflex":
                  if (sbyte.TryParse(reflex.GetBindValue(player.oid, nuiToken.Token), out sbyte newReflex))
                    targetCreature.SetBaseSavingThrow(SavingThrow.Reflex, newReflex);
                  break;

                case "will":
                  if (sbyte.TryParse(will.GetBindValue(player.oid, nuiToken.Token), out sbyte newWill))
                    targetCreature.SetBaseSavingThrow(SavingThrow.Will, newWill);
                  break;

                case "naturalAC":
                  if (sbyte.TryParse(naturalAC.GetBindValue(player.oid, nuiToken.Token), out sbyte newAC))
                  {
                    targetCreature.BaseAC = newAC;
                    naturalACTooltip.SetBindValue(player.oid, nuiToken.Token, $"La créature subit naturellement {Utils.GetDamageMultiplier(targetCreature.AC) * 100:0.##} % des dégâts");
                  }
                  break;

                case "armorPenetration":
                  if (byte.TryParse(armorPenetration.GetBindValue(player.oid, nuiToken.Token), out byte newAP))
                  {
                    int previousAttackPerRound = targetCreature.BaseAttackCount;
                    targetCreature.BaseAttackBonus = newAP;

                    if (targetCreature.BaseAttackCount != previousAttackPerRound)
                      targetCreature.BaseAttackCount = previousAttackPerRound;
                  }
                  break;

                case "attackPerRound":
                  if (int.TryParse(attackPerRound.GetBindValue(player.oid, nuiToken.Token), out int newNBAttack))
                  {
                    if (newNBAttack < 1 || newNBAttack > 6)
                    {
                      newNBAttack = newNBAttack < 1 ? 1 : newNBAttack;
                      newNBAttack = newNBAttack > 6 ? 6 : newNBAttack;

                      attackPerRound.SetBindValue(player.oid, nuiToken.Token, newNBAttack.ToString());
                    }
                    else
                      targetCreature.BaseAttackCount = newNBAttack;
                  }
                  break;

                case "spellCasterLevel":
                  if (int.TryParse(spellCasterLevel.GetBindValue(player.oid, nuiToken.Token), out int newCasterLevel))
                  {
                    if (newCasterLevel < 1)
                      spellCasterLevel.SetBindValue(player.oid, nuiToken.Token, "1");
                    else
                      targetCreature.GetObjectVariable<LocalVariableInt>("_CREATURE_CASTER_LEVEL").Value = newCasterLevel;
                  }
                  break;

                case "hitPoints":
                  if (int.TryParse(hitPoints.GetBindValue(player.oid, nuiToken.Token), out int newHP))
                    targetCreature.MaxHP = newHP;
                  break;

                case "movementRateSelected":
                  targetCreature.MovementRate = (MovementRate)movementRateSelected.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "permanentSpawn":

                  bool spawnOn = permanentSpawn.GetBindValue(player.oid, nuiToken.Token);

                  if (player.QueryAuthorized())
                  {
                    if (spawnOn)
                    {
                      HandleSelectSpawnType();

                      targetCreature.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").Value = player.oid.PlayerName;
                      CreatureUtils.HandleSpawnPointCreation(targetCreature);

                      player.oid.SendServerMessage($"{targetCreature.Name.ColorString(ColorConstants.White)} a été ajouté au système de spawn permanent.\nVeuillez sélectionner la nouvelle créature pour poursuivre l'édition.", ColorConstants.Orange);
                      CloseWindow();
                      return;
                    }
                    else
                    {
                      targetCreature.OnDeath -= CreatureUtils.OnMobDeathResetSpawn;
                      HandlePermanentSpawnDeletion();
                      player.oid.SendServerMessage($"{targetCreature.Name.ColorString(ColorConstants.White)} a été retiré du système de spawn.", ColorConstants.Orange);
                    }
                  }
                  else
                  {
                    permanentSpawn.SetBindWatch(player.oid, nuiToken.Token, false);
                    permanentSpawn.SetBindValue(player.oid, nuiToken.Token, !spawnOn);
                    permanentSpawn.SetBindWatch(player.oid, nuiToken.Token, true);
                  }

                  spawnOptionsVisibility.SetBindValue(player.oid, nuiToken.Token, permanentSpawn.GetBindValue(player.oid, nuiToken.Token));

                  break;

                case "selectedSpawnType":
                  HandleSelectSpawnType();
                  break;

                case "spellQuantity":

                  if (int.TryParse(spellQuantity.GetBindValues(player.oid, nuiToken.Token)[spellQuantityIndexSelected], out int newSpellQuantity))
                  {
                    if (newSpellQuantity < 1 || newSpellQuantity > 10)
                    {
                      newSpellQuantity = newSpellQuantity < 1 ? 1 : newSpellQuantity;
                      newSpellQuantity = newSpellQuantity > 10 ? 10 : newSpellQuantity;

                      var tempSpellQuantity = spellQuantity.GetBindValues(player.oid, nuiToken.Token);
                      tempSpellQuantity[spellQuantityIndexSelected] = newSpellQuantity.ToString();
                      spellQuantity.SetBindValues(player.oid, nuiToken.Token, tempSpellQuantity);
                    }
                    else
                    {
                      SpecialAbility special = new(acquiredSpellSearcher[spellQuantityIndexSelected], 1);
                      int spellCount = targetCreature.SpecialAbilities.Count(s => s.Spell == special.Spell);

                      if (spellCount < newSpellQuantity)
                        for (int i = 0; i < newSpellQuantity - spellCount; i++)
                          targetCreature.AddSpecialAbility(special);
                      else if (spellCount > newSpellQuantity)
                        for (int i = 0; i < spellCount - newSpellQuantity; i++)
                        {
                          int j = 0;

                          foreach (SpecialAbility spell in targetCreature.SpecialAbilities)
                          {
                            if (spell.Spell == special.Spell)
                            {
                              targetCreature.RemoveSpecialAbilityAt(j);
                              break;
                            }

                            j++;
                          }
                        }
                    }
                  }

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
              new NuiSpacer(),
              new NuiButton("Base") { Id = "base", Height = 35, Width = 60 },
              new NuiButton("Portrait") { Id = "portrait", Height = 35, Width = 60 },
              new NuiButton("Stats") { Id = "stats", Height = 35, Width = 60 },
              new NuiButton("Description") { Id = "description", Height = 35, Width = 60 },
              new NuiButton("Dons") { Id = "feats", Height = 35, Width = 60 },
              new NuiButton("Sorts") { Id = "spells", Height = 35, Width = 60 },
              new NuiButton("Modèle") { Id = "appearance", Height = 35, Width = 60 },
              new NuiButton("Variables") { Id = "variables", Height = 35, Width = 60 },
              new NuiSpacer()
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

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCheck("Spawn Permanent", permanentSpawn) { Tooltip = "Si cette option est cochée, la créature sera intégrée au système de spawn et persistera après reboot" } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiOptions() { Selection = selectedSpawnType, Direction = NuiDirection.Horizontal, Options = { "mob", "pnj fixe", "neutral" }, Tooltip = "mob = monstre hostile. PNJ fixe = immobile. Neutral = créature neutre qui se balade aléatoirement", Visible = spawnOptionsVisibility } } });
        }
        private void StopAllWatchBindings()
        {
          name.SetBindWatch(player.oid, nuiToken.Token, false);
          tag.SetBindWatch(player.oid, nuiToken.Token, false);
          size.SetBindWatch(player.oid, nuiToken.Token, false);
          challengeRating.SetBindWatch(player.oid, nuiToken.Token, false);

          raceSelected.SetBindWatch(player.oid, nuiToken.Token, false);
          raceSearch.SetBindWatch(player.oid, nuiToken.Token, false);
          apparenceSelected.SetBindWatch(player.oid, nuiToken.Token, false);
          apparenceSearch.SetBindWatch(player.oid, nuiToken.Token, false);
          genderSelected.SetBindWatch(player.oid, nuiToken.Token, false);
          factionSelected.SetBindWatch(player.oid, nuiToken.Token, false);
          soundSetSelected.SetBindWatch(player.oid, nuiToken.Token, false);

          racePortraitSelected.SetBindWatch(player.oid, nuiToken.Token, false);
          genderPortraitSelected.SetBindWatch(player.oid, nuiToken.Token, false);

          strength.SetBindWatch(player.oid, nuiToken.Token, false);
          dexterity.SetBindWatch(player.oid, nuiToken.Token, false);
          constitution.SetBindWatch(player.oid, nuiToken.Token, false);
          intelligence.SetBindWatch(player.oid, nuiToken.Token, false);
          wisdom.SetBindWatch(player.oid, nuiToken.Token, false);
          charisma.SetBindWatch(player.oid, nuiToken.Token, false);

          fortitude.SetBindWatch(player.oid, nuiToken.Token, false);
          reflex.SetBindWatch(player.oid, nuiToken.Token, false);
          will.SetBindWatch(player.oid, nuiToken.Token, false);

          naturalAC.SetBindWatch(player.oid, nuiToken.Token, false);
          hitPoints.SetBindWatch(player.oid, nuiToken.Token, false);
          armorPenetration.SetBindWatch(player.oid, nuiToken.Token, false);
          attackPerRound.SetBindWatch(player.oid, nuiToken.Token, false);
          spellCasterLevel.SetBindWatch(player.oid, nuiToken.Token, false);
          movementRateSelected.SetBindWatch(player.oid, nuiToken.Token, false);

          availableFeatSearch.SetBindWatch(player.oid, nuiToken.Token, false);
          acquiredFeatSearch.SetBindWatch(player.oid, nuiToken.Token, false);

          spellQuantity.SetBindWatch(player.oid, nuiToken.Token, false);

          permanentSpawn.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedSpawnType.SetBindWatch(player.oid, nuiToken.Token, false);
        }
        private void LoadBaseBinding()
        {
          StopAllWatchBindings();

          name.SetBindValue(player.oid, nuiToken.Token, targetCreature.Name);
          name.SetBindWatch(player.oid, nuiToken.Token, true);
          tag.SetBindValue(player.oid, nuiToken.Token, targetCreature.Tag);
          tag.SetBindWatch(player.oid, nuiToken.Token, true);
          challengeRating.SetBindValue(player.oid, nuiToken.Token, targetCreature.ChallengeRating.ToString());
          SetChallengeRatingTooltip();
          challengeRating.SetBindWatch(player.oid, nuiToken.Token, true);

          race.SetBindValue(player.oid, nuiToken.Token, Utils.raceList);
          raceSelected.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.Race.RacialType);
          raceSelected.SetBindWatch(player.oid, nuiToken.Token, true);
          raceSearch.SetBindWatch(player.oid, nuiToken.Token, true);

          genderSelected.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.Gender);
          genderSelected.SetBindWatch(player.oid, nuiToken.Token, true);

          factionSelected.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.Faction.StandardFactionType);
          factionSelected.SetBindWatch(player.oid, nuiToken.Token, true);

          soundSetSelected.SetBindValue(player.oid, nuiToken.Token, targetCreature.SoundSet);
          soundSetSelected.SetBindWatch(player.oid, nuiToken.Token, true);

          permanentSpawn.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetObjectVariable<LocalVariableBool>("_SPAWN_ID").HasValue);
          permanentSpawn.SetBindWatch(player.oid, nuiToken.Token, true);
          spawnOptionsVisibility.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetObjectVariable<LocalVariableBool>("_SPAWN_ID").HasValue);
          selectedSpawnType.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_TYPE").Value);
          selectedSpawnType.SetBindWatch(player.oid, nuiToken.Token, true);
          previousType = targetCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_TYPE").Value;
        }
        private void LoadPortraitLayout()
        {
          rootChildren.Clear();
          rowTemplate.Clear();

          LoadButtons();

          rowTemplate.Add(new NuiListTemplateCell(new NuiSpacer()) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits1) { Id = "portraitSelect1", Tooltip = portraits1 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits2) { Id = "portraitSelect2", Tooltip = portraits2 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(portraits3) { Id = "portraitSelect3", Tooltip = portraits3 }) { Width = 64 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiSpacer()) { VariableSize = true });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Race") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = race, Selected = racePortraitSelected },
              new NuiSpacer()
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Genre") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
              new NuiCombo() { Height = 35, Width = 200, Entries = Utils.genderList, Selected = genderPortraitSelected },
              new NuiSpacer()
            }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 128 } } });
        }
        private void LoadPortraitBinding()
        {
          StopAllWatchBindings();

          race.SetBindValue(player.oid, nuiToken.Token, Utils.raceList);
          racePortraitSelected.SetBindValue(player.oid, nuiToken.Token, targetCreature.Race.Id);
          racePortraitSelected.SetBindWatch(player.oid, nuiToken.Token, true);

          genderPortraitSelected.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.Gender);
          genderPortraitSelected.SetBindWatch(player.oid, nuiToken.Token, true);

          UpdatePortraitList();
        }
        private void UpdatePortraitList()
        {
          List<string>[] portraitList = new List<string>[] { new(), new(), new() };
          List<string> portraitTable = Portraits2da.portraitFilteredEntries[racePortraitSelected.GetBindValue(player.oid, nuiToken.Token), genderPortraitSelected.GetBindValue(player.oid, nuiToken.Token)];

          if (portraitTable != null)
            for (int i = 0; i < portraitTable.Count; i += 3)
              for (int j = 0; j < 3; j++)
                try { portraitList[j].Add(portraitTable[i + j]); }
                catch (Exception) { portraitList[j].Add(""); }

          portraits1.SetBindValues(player.oid, nuiToken.Token, portraitList[0]);
          portraits2.SetBindValues(player.oid, nuiToken.Token, portraitList[1]);
          portraits3.SetBindValues(player.oid, nuiToken.Token, portraitList[2]);
          listCount.SetBindValue(player.oid, nuiToken.Token, portraitList[0].Count);
        }
        private void LoadStatsLayout()
        {
          rootChildren.Clear();
          LoadButtons();

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButtonImage("ief_inc_str") { Height = 35, Width = 35, Tooltip = "Force" },
              new NuiTextEdit("Force", strength, 3, false) { Height = 35, Width = 45, Tooltip = $"Bonus racial : {targetCreature.Race.GetAbilityAdjustment(Ability.Strength)}" },
              new NuiTextEdit("Modificateur", strengthModifier, 3, false) { Height = 35, Width = 45, Enabled = false },
              new NuiButtonImage("ir_guard") { Height = 35, Width = 35, Tooltip = "Classe d'armure naturelle" },
              new NuiTextEdit("Classe d'Armure Naturelle", naturalAC, 3, false) { Height = 35, Width = 45, Tooltip = naturalACTooltip },
              new NuiButtonImage("ief_acdecr") { Height = 35, Width = 35, Tooltip = "Pénétration d'armure" },
              new NuiTextEdit("Pénétration d'armure", armorPenetration, 3, false) { Height = 35, Width = 45, Tooltip = "Les attaques de cette créature pénètre l'armure de la cible de 1 % par point" },
              new NuiButtonImage("ir_flurry") { Height = 35, Width = 35, Tooltip = "Attaques par round" },
              new NuiTextEdit("Attaques par round", attackPerRound, 3, false) { Height = 35, Width = 45, Tooltip = "Nombre d'attaques par round de la créature" }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButtonImage("ief_inc_dex") { Height = 35, Width = 35, Tooltip = "Dextérité" },
              new NuiTextEdit("Dextérité", dexterity, 3, false) { Height = 35, Width = 45, Tooltip = $"Bonus racial : {targetCreature.Race.GetAbilityAdjustment(Ability.Dexterity)}" },
              new NuiTextEdit("Modificateur", dexterityModifier, 3, false) { Height = 35, Width = 45, Enabled = false },
              new NuiButtonImage("ir_flee") { Height = 35, Width = 35, Tooltip = "Réflexes de base" },
              new NuiTextEdit("Réflexes", reflex, 3, false) { Height = 35, Width = 45, Tooltip = reflexTooltip },
              new NuiButtonImage("ife_dodge") { Height = 35, Width = 35, Tooltip = "Pourcentage de chance d'éviter totalement une attaque ou un sort" },
              new NuiLabel(dodgeChance) { Height = 35, Width = 45, Tooltip = "Toute créature a 5 % de chance supplémentaire d'éviter une attaque d'une créature plus grande", VerticalAlign = NuiVAlign.Middle }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButtonImage("ief_inc_con") { Height = 35, Width = 35, Tooltip = "Constitution" },
              new NuiTextEdit("Constitution", constitution, 3, false) { Height = 35, Width = 45, Tooltip = $"Bonus racial : {targetCreature.Race.GetAbilityAdjustment(Ability.Dexterity)}" },
              new NuiTextEdit("Modificateur", constitutionModifier, 3, false) { Height = 35, Width = 45, Enabled = false },
              new NuiButtonImage("ir_rage") { Height = 35, Width = 35, Tooltip = "Vigueur de base" },
              new NuiTextEdit("Vigueur", fortitude, 3, false) { Height = 35, Width = 45, Tooltip = fortitudeTooltip },
              new NuiButtonImage("ief_temphp") { Height = 35, Width = 35, Tooltip = "Hit Points" },
              new NuiTextEdit("Hit Points", hitPoints, 4, false) { Height = 35, Width = 45 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButtonImage("ief_inc_int") { Height = 35, Width = 35, Tooltip = "Intelligence" },
              new NuiTextEdit("Intelligence", intelligence, 3, false) { Height = 35, Width = 45, Tooltip = $"Bonus racial : {targetCreature.Race.GetAbilityAdjustment(Ability.Intelligence)}" },
              new NuiTextEdit("Modificateur", intelligenceModifier, 3, false) { Height = 35, Width = 45, Enabled = false },
              new NuiButtonImage("ir_dcaster") { Height = 35, Width = 35, Tooltip = "Niveau de lanceur de sorts" },
              new NuiTextEdit("Niveau de lanceur de sorts", spellCasterLevel, 2, false) { Height = 35, Width = 45, Tooltip = "Définit la puissance et la durée des sorts lancés par cette créature" }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButtonImage("ief_inc_wis") { Height = 35, Width = 35, Tooltip = "Sagesse" },
              new NuiTextEdit("Sagesse", wisdom, 3, false) { Height = 35, Width = 45, Tooltip = $"Bonus racial : {targetCreature.Race.GetAbilityAdjustment(Ability.Wisdom)}" },
              new NuiTextEdit("Modificateur", wisdomModifier, 3, false) { Height = 35, Width = 45, Enabled = false },
              new NuiButtonImage("ir_reldom") { Height = 35, Width = 35, Tooltip = "Volonté de base" },
              new NuiTextEdit("Volonté", will, 3, false) { Height = 35, Width = 45, Tooltip = willTooltip }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButtonImage("ief_inc_cha") { Height = 35, Width = 35, Tooltip = "Charisme" },
              new NuiTextEdit("Charisme", charisma, 3, false) { Height = 35, Width = 45, Tooltip = $"Bonus racial : {targetCreature.Race.GetAbilityAdjustment(Ability.Charisma)}" },
              new NuiTextEdit("Modificateur", charismaModifier, 3, false) { Height = 35, Width = 45, Enabled = false },
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButtonImage("ief_moveincr") { Height = 35, Width = 35, Tooltip = "Vitesse de déplacement" },
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

          challengeRatingTooltip.SetBindValue(player.oid, nuiToken.Token, $"Effectue {damageMultiplier} de ses dégâts de base. {critChance} de chances de critiques");
        }
        private void LoadStatsBinding()
        {
          StopAllWatchBindings();

          strength.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityScore(Ability.Strength, true).ToString());
          dexterity.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityScore(Ability.Dexterity, true).ToString());
          constitution.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityScore(Ability.Constitution, true).ToString());
          intelligence.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityScore(Ability.Intelligence, true).ToString());
          wisdom.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityScore(Ability.Wisdom, true).ToString());
          charisma.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityScore(Ability.Charisma, true).ToString());

          strengthModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Strength).ToString());
          dexterityModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Dexterity).ToString());
          constitutionModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Constitution).ToString());
          intelligenceModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Intelligence).ToString());
          wisdomModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Wisdom).ToString());
          charismaModifier.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetAbilityModifier(Ability.Charisma).ToString());

          fortitude.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetBaseSavingThrow(SavingThrow.Fortitude).ToString());
          reflex.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetBaseSavingThrow(SavingThrow.Reflex).ToString());
          will.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetBaseSavingThrow(SavingThrow.Will).ToString());

          fortitudeTooltip.SetBindValue(player.oid, nuiToken.Token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Fortitude) + targetCreature.GetAbilityModifier(Ability.Constitution)}");
          reflexTooltip.SetBindValue(player.oid, nuiToken.Token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Reflex) + targetCreature.GetAbilityModifier(Ability.Dexterity)}");
          willTooltip.SetBindValue(player.oid, nuiToken.Token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Will) + targetCreature.GetAbilityModifier(Ability.Wisdom)}");

          naturalAC.SetBindValue(player.oid, nuiToken.Token, targetCreature.BaseAC.ToString());
          naturalACTooltip.SetBindValue(player.oid, nuiToken.Token, $"La créature subit naturellement {Utils.GetDamageMultiplier(targetCreature.AC) * 100:0.##} % des dégâts");
          armorPenetration.SetBindValue(player.oid, nuiToken.Token, targetCreature.BaseAttackBonus.ToString());
          attackPerRound.SetBindValue(player.oid, nuiToken.Token, targetCreature.BaseAttackCount.ToString());
          spellCasterLevel.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetObjectVariable<LocalVariableInt>("_CREATURE_CASTER_LEVEL").Value.ToString());

          dodgeChance.SetBindValue(player.oid, nuiToken.Token, Utils.GetDodgeChance(targetCreature).ToString());
          hitPoints.SetBindValue(player.oid, nuiToken.Token, targetCreature.MaxHP.ToString());
          movementRateSelected.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.MovementRate);

          strength.SetBindWatch(player.oid, nuiToken.Token, true);
          dexterity.SetBindWatch(player.oid, nuiToken.Token, true);
          constitution.SetBindWatch(player.oid, nuiToken.Token, true);
          intelligence.SetBindWatch(player.oid, nuiToken.Token, true);
          wisdom.SetBindWatch(player.oid, nuiToken.Token, true);
          charisma.SetBindWatch(player.oid, nuiToken.Token, true);

          fortitude.SetBindWatch(player.oid, nuiToken.Token, true);
          reflex.SetBindWatch(player.oid, nuiToken.Token, true);
          will.SetBindWatch(player.oid, nuiToken.Token, true);

          naturalAC.SetBindWatch(player.oid, nuiToken.Token, true);
          armorPenetration.SetBindWatch(player.oid, nuiToken.Token, true);
          attackPerRound.SetBindWatch(player.oid, nuiToken.Token, true);
          spellCasterLevel.SetBindWatch(player.oid, nuiToken.Token, true);
          hitPoints.SetBindWatch(player.oid, nuiToken.Token, true);
          movementRateSelected.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void LoadFeatsLayout()
        {
          rootChildren.Clear();
          LoadButtons();
          rowTemplate.Clear();

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(availableFeatIcons) { Id = "selectFeat", Tooltip = "Ajouter" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(availableFeatNames) { Id = "availableFeatDescription", Tooltip = availableFeatNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true });

          List<NuiListTemplateCell> rowTemplateAcquiredFeats = new()
          {
            new NuiListTemplateCell(new NuiButtonImage(acquiredFeatIcons) { Id = "removeFeat", Tooltip = "Supprimer" }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(acquiredFeatNames) { Id = "acquiredFeatDescription", Tooltip = acquiredFeatNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true }
          };

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Dons disponibles", availableFeatSearch, 20, false) { Width = 240 } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 240  } } }
            }
          });

          columnsChildren.Add(new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Dons acquis", acquiredFeatSearch, 20, false) { Width = 240 } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplateAcquiredFeats, listAcquiredFeatCount) { RowHeight = 35, Width = 240 } } }
            }
          });
        }
        private void LoadFeatsBinding()
        {
          StopAllWatchBindings();

          availableFeats.Clear();
          acquiredFeats.Clear();

          availableFeatSearch.SetBindValue(player.oid, nuiToken.Token, "");
          availableFeatSearch.SetBindWatch(player.oid, nuiToken.Token, true);
          acquiredFeatSearch.SetBindValue(player.oid, nuiToken.Token, "");
          acquiredFeatSearch.SetBindWatch(player.oid, nuiToken.Token, true);

          List<string> availableIconsList = new();
          List<string> availableNamesList = new();
          List<string> acquiredIconsList = new();
          List<string> acquiredNamesList = new();

          foreach (Feat feat in (Feat[])Enum.GetValues(typeof(Feat)))
          {
            NwFeat baseFeat = NwFeat.FromFeatType(feat);

            if (targetCreature.KnowsFeat(baseFeat))
            {
              acquiredIconsList.Add(baseFeat.IconResRef);
              acquiredNamesList.Add(baseFeat.Name.ToString().Replace("’", "'"));
              acquiredFeats.Add(baseFeat);
            }
            else
            {
              availableIconsList.Add(baseFeat.IconResRef);
              availableNamesList.Add(baseFeat.Name.ToString().Replace("’", "'"));
              availableFeats.Add(baseFeat);
            }
          }

          availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, availableIconsList);
          availableFeatNames.SetBindValues(player.oid, nuiToken.Token, availableNamesList);
          acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, acquiredIconsList);
          acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, acquiredNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, availableFeats.Count);
          listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, acquiredFeats.Count);

          availableFeatSearcher = availableFeats;
          acquiredFeatSearcher = acquiredFeats;
        }
        private void LoadSpellsLayout()
        {
          rootChildren.Clear();
          LoadButtons();
          rowTemplate.Clear();

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(availableFeatIcons) { Id = "selectSpell", Tooltip = "Ajouter" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(availableFeatNames) { Id = "availableFeatDescription", Tooltip = availableFeatNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true });

          List<NuiListTemplateCell> rowTemplateAcquiredFeats = new()
          {
            new NuiListTemplateCell(new NuiButtonImage(acquiredFeatIcons) { Id = "removeSpell", Tooltip = "Supprimer" }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(acquiredFeatNames) { Id = "acquiredFeatDescription", Tooltip = acquiredFeatNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true },
            new NuiListTemplateCell(new NuiTextEdit("Quantité", spellQuantity, 2, false) { Id = "spellQuantityText", Tooltip = "10 = utilisation illimitée", Enabled = spellQuantityEnabled }) { Width = 35 }
          };

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Sorts disponibles", availableFeatSearch, 20, false) { Width = 240 } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 240  } } }
            }
          });

          columnsChildren.Add(new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Sorts acquis", acquiredFeatSearch, 20, false) { Width = 240 } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplateAcquiredFeats, listAcquiredFeatCount) { RowHeight = 35, Width = 240 } } }
            }
          });
        }
        private void LoadSpellsBinding()
        {
          StopAllWatchBindings();

          availableSpells.Clear();
          acquiredSpells.Clear();

          List<string> availableIconsList = new();
          List<string> availableNamesList = new();
          List<string> acquiredIconsList = new();
          List<string> acquiredNamesList = new();
          List<string> acquiredQuantityList = new();
          List<bool> enabledQuantityList = new();

          foreach (Spell spell in (Spell[])Enum.GetValues(typeof(Spell)))
          {
            if (SpellUtils.IgnoredSpellList(spell))
              continue;

            NwSpell baseSpell = NwSpell.FromSpellType(spell);

            if (targetCreature.SpecialAbilities.Any(s => s.Spell == baseSpell))
            {
              acquiredIconsList.Add(baseSpell.IconResRef);
              acquiredNamesList.Add(baseSpell.Name.ToString().Replace("’", "'"));
              acquiredQuantityList.Add(targetCreature.SpecialAbilities.Count(s => s.Spell == baseSpell).ToString());
              acquiredSpells.Add(baseSpell);

              if (SpellUtils.IsSpellBuff(baseSpell))
                enabledQuantityList.Add(false);
              else
                enabledQuantityList.Add(true);
            }
            else
            {
              availableIconsList.Add(baseSpell.IconResRef);
              availableNamesList.Add(baseSpell.Name.ToString().Replace("’", "'"));
              availableSpells.Add(baseSpell);
            }
          }

          availableFeatIcons.SetBindValues(player.oid, nuiToken.Token, availableIconsList);
          availableFeatNames.SetBindValues(player.oid, nuiToken.Token, availableNamesList);
          acquiredFeatIcons.SetBindValues(player.oid, nuiToken.Token, acquiredIconsList);
          acquiredFeatNames.SetBindValues(player.oid, nuiToken.Token, acquiredNamesList);
          spellQuantity.SetBindValues(player.oid, nuiToken.Token, acquiredQuantityList);
          spellQuantityEnabled.SetBindValues(player.oid, nuiToken.Token, enabledQuantityList);
          listCount.SetBindValue(player.oid, nuiToken.Token, availableSpells.Count);
          listAcquiredFeatCount.SetBindValue(player.oid, nuiToken.Token, acquiredSpells.Count);

          availableSpellSearcher = availableSpells;
          acquiredSpellSearcher = acquiredSpells;

          availableFeatSearch.SetBindValue(player.oid, nuiToken.Token, "");
          availableFeatSearch.SetBindWatch(player.oid, nuiToken.Token, true);
          acquiredFeatSearch.SetBindValue(player.oid, nuiToken.Token, "");
          acquiredFeatSearch.SetBindWatch(player.oid, nuiToken.Token, true);
          spellQuantity.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void LoadModelLayout()
        {
          rootChildren.Clear();
          LoadButtons();

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiLabel("Apparence") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle },
            new NuiButton("<") { Id = "appearancePrev", Height = 35, Width = 35 },
            new NuiCombo() { Height = 35, Width = 200, Entries = apparence, Selected = apparenceSelected },
            new NuiButton(">") { Id = "appearanceNext", Height = 35, Width = 35 },
            new NuiTextEdit("Recherche", apparenceSearch, 20, false) { Height = 35, Width = 100 }
          }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiLabel("Taille") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Tooltip = "Doit être compris entre 0.01 et 99.99" },
            new NuiTextEdit("Taille", size, 5, false) { Height = 35, Width = 200 }
          }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiLabel("Dynamique") { Height = 35, Width = 70, VerticalAlign = NuiVAlign.Middle, Tooltip = "Disponible uniquement pour les apparences de type dynamiques et genre masculin/féminin" },
            new NuiButton("Apparence dynamique") { Id = "appearanceDynamic", Height = 35, Width = 200 }
          }
          });
        }
        private void LoadModelBinding()
        {
          StopAllWatchBindings();

          filteredEntries = Utils.appearanceEntries;
          apparence.SetBindValue(player.oid, nuiToken.Token, Utils.appearanceEntries);
          apparenceSelected.SetBindValue(player.oid, nuiToken.Token, targetCreature.Appearance.RowIndex);
          apparenceSelected.SetBindWatch(player.oid, nuiToken.Token, true);
          apparenceSearch.SetBindWatch(player.oid, nuiToken.Token, true);

          size.SetBindValue(player.oid, nuiToken.Token, targetCreature.VisualTransform.Scale.ToString());
          size.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void SetAppearance(int index)
        {
          apparenceSelected.SetBindWatch(player.oid, nuiToken.Token, false);

          apparenceSelected.SetBindValue(player.oid, nuiToken.Token, index);
          targetCreature.Appearance = NwGameTables.AppearanceTable[index];

          apparenceSelected.SetBindWatch(player.oid, nuiToken.Token, true);
        }

        private void LoadDescriptionLayout()
        {
          rootChildren.Clear();
          LoadButtons();

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Description", creatureDescription, 999, true) { Height = 200, Width = 490 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Commentaire", creatureComment, 999, true) { Height = 200, Width = 490 }
            }
          });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButtonImage("ir_empytqs") { Id = "saveDescription", Tooltip = "Enregistrer la description et le commentaire de cette créature", Height = 35, Width = 35 },
              new NuiSpacer()
            }
          });
        }

        private void LoadDescriptionBinding()
        {
          StopAllWatchBindings();

          creatureDescription.SetBindValue(player.oid, nuiToken.Token, targetCreature.Description);
          creatureComment.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetObjectVariable<LocalVariableString>("_COMMENT").Value);
        }
        private void LoadVariablesLayout()
        {
          rootChildren.Clear();
          LoadButtons();
          rowTemplate.Clear();

          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("Nom", variableName, 20, false) { Tooltip = variableName }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiCombo() { Entries = Utils.variableTypes, Selected = selectedVariableType, Width = 60 }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("Valeur", variableValue, 20, false) { Tooltip = variableValue }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_empytqs") { Id = "saveVariable", Tooltip = "Sauvegarder" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_ban") { Id = "deleteVariable", Tooltip = "Supprimer" }) { Width = 35 });

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiTextEdit("Nom", newVariableName, 20, false) { Tooltip = newVariableName, Width = 120 },
                new NuiCombo() { Entries = Utils.variableTypes, Selected = selectedNewVariableType, Width = 80 },
                new NuiTextEdit("Valeur", newVariableValue, 20, false) { Tooltip = newVariableValue, Width = 120 },
                new NuiButtonImage("ir_empytqs") { Id = "saveNewVariable", Height = 35, Width = 35 },
              }
            },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 490  } } }
            }
          });
        }
        private void LoadVariablesBinding()
        {
          StopAllWatchBindings();

          List<string> variableNameList = new();
          List<int> selectedVariableTypeList = new();
          List<string> variableValueList = new();
          int count = 0;

          foreach (var variable in targetCreature.LocalVariables)
          {
            switch (variable)
            {
              case LocalVariableString stringVar:
                variableValueList.Add(stringVar.Value);
                selectedVariableTypeList.Add(2);
                break;
              case LocalVariableInt intVar:
                variableValueList.Add(intVar.Value.ToString());
                selectedVariableTypeList.Add(1);
                break;
              case LocalVariableFloat floatVar:
                variableValueList.Add(floatVar.Value.ToString());
                selectedVariableTypeList.Add(3);
                break;
              case DateTimeLocalVariable dateVar:
                variableValueList.Add(dateVar.Value.ToString());
                selectedVariableTypeList.Add(4);
                break;
              case PersistentVariableInt intPersistVar:

                if (player.oid.PlayerName != "Chim")
                  continue;

                variableValueList.Add(intPersistVar.Value.ToString());
                selectedVariableTypeList.Add(5);

                break;
              case PersistentVariableString stringPersistVar:

                if (player.oid.PlayerName != "Chim")
                  continue;

                variableValueList.Add(stringPersistVar.Value.ToString());
                selectedVariableTypeList.Add(6);


                break;
              case PersistentVariableFloat floatPersistVar:

                if (player.oid.PlayerName != "Chim")
                  continue;

                variableValueList.Add(floatPersistVar.Value.ToString());
                selectedVariableTypeList.Add(7);
                break;

              default:
                continue;
            }

            variableNameList.Add(variable.Name);
            count++;
          }

          variableName.SetBindValues(player.oid, nuiToken.Token, variableNameList);
          selectedVariableType.SetBindValues(player.oid, nuiToken.Token, selectedVariableTypeList);
          variableValue.SetBindValues(player.oid, nuiToken.Token, variableValueList);
          listCount.SetBindValue(player.oid, nuiToken.Token, count);
        }
        private void HandleSelectSpawnType()
        {
          if (player.QueryAuthorized())
          {
            int type = selectedSpawnType.GetBindValue(player.oid, nuiToken.Token);
            previousType = type;

            switch (type)
            {
              case 1:
                targetCreature.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").Value = "npc";
                break;
              case 2:
                targetCreature.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").Value = "walker";
                break;
            }

            targetCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_TYPE").Value = type;

            if (targetCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").HasNothing)
            {
              SqLiteUtils.InsertQuery("creatureSpawn",
                new List<string[]>() { new string[] { "areaTag", targetCreature.Area.Tag }, new string[] { "position", targetCreature.Position.ToString() }, new string[] { "facing", targetCreature.Rotation.ToString() }, new string[] { "serializedCreature", targetCreature.Serialize().ToBase64EncodedString() } });

              var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT last_insert_rowid()");
              query.Execute();

              targetCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value = query.Result.GetInt(0);
            }
            else
            {
              SqLiteUtils.UpdateQuery("creatureSpawn",
                new List<string[]>() { new string[] { "areaTag", targetCreature.Area.Tag }, new string[] { "position", targetCreature.Position.ToString() }, new string[] { "facing", targetCreature.Rotation.ToString() }, new string[] { "serializedCreature", targetCreature.Serialize().ToBase64EncodedString() } },
                new List<string[]>() { new string[] { "rowid", targetCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value.ToString() } });
            }
          }
          else
          {
            selectedSpawnType.SetBindWatch(player.oid, nuiToken.Token, false);
            selectedSpawnType.SetBindValue(player.oid, nuiToken.Token, previousType);
            selectedSpawnType.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandlePermanentSpawnDeletion()
        {
          if (targetCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").HasValue)
          {
            string spawnId = targetCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value.ToString();

            SqLiteUtils.DeletionQuery("creatureSpawn",
              new Dictionary<string, string>() { { "rowid", spawnId } });

            targetCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Delete();
          }
        }
        private void HandleAppearanceSearch(int index)
        {
          NuiComboEntry entryPrev = filteredEntries.FirstOrDefault(a => a.Value == apparenceSelected.GetBindValue(player.oid, nuiToken.Token));
          int indexPrev = filteredEntries.IndexOf(entryPrev) + index;

          if (indexPrev < 0)
            SetAppearance(filteredEntries.ElementAt(filteredEntries.Count() - 1).Value);
          else if (indexPrev >= filteredEntries.Count())
            SetAppearance(filteredEntries.ElementAt(0).Value);
          else
            SetAppearance(filteredEntries.ElementAt(indexPrev).Value);
        }
      }
    }
  }
}
