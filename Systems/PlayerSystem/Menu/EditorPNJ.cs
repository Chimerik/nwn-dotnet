using System;
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
        private readonly List<NwFeat> availableFeats = new();
        private readonly List<NwFeat> acquiredFeats = new();
        private List<NwFeat> availableFeatSearcher = new();
        private List<NwFeat> acquiredFeatSearcher = new();

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

          window = new NuiWindow(rootGroup, $"Modification de {targetCreature.Name}")
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
            case NuiEventType.MouseUp:

              switch (nuiEvent.ElementId)
              {
                case "availableFeatDescription":

                  if (player.windows.ContainsKey("featDescription"))
                    ((FeatDescriptionWindow)player.windows["featDescription"]).CreateWindow(availableFeatSearcher[nuiEvent.ArrayIndex]);
                  else
                    player.windows.Add("featDescription", new FeatDescriptionWindow(player, availableFeatSearcher[nuiEvent.ArrayIndex]));

                  break;

                case "acquiredFeatDescription":

                  if (player.windows.ContainsKey("featDescription"))
                    ((FeatDescriptionWindow)player.windows["featDescription"]).CreateWindow(acquiredFeatSearcher[nuiEvent.ArrayIndex]);
                  else
                    player.windows.Add("featDescription", new FeatDescriptionWindow(player, acquiredFeatSearcher[nuiEvent.ArrayIndex]));

                  break;
              }

              break;

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

                case "feats":
                  LoadFeatsLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.WindowToken, layoutColumn);
                  LoadFeatsBinding();
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

                case "selectFeat":
                  NwFeat acquiredFeat = availableFeatSearcher[nuiEvent.ArrayIndex];

                  targetCreature.AddFeat(acquiredFeat);
                  
                  if(!acquiredFeats.Contains(acquiredFeat))
                    acquiredFeats.Add(acquiredFeat);
                  
                  if(!acquiredFeatSearcher.Contains(acquiredFeat))
                    acquiredFeatSearcher.Add(acquiredFeat);

                  availableFeats.Remove(acquiredFeat);
                  availableFeatSearcher.Remove(acquiredFeat);

                  var tempIcon = availableFeatIcons.GetBindValues(player.oid, token);
                  tempIcon.RemoveAt(nuiEvent.ArrayIndex);
                  var tempName = availableFeatNames.GetBindValues(player.oid, token);
                  tempName.RemoveAt(nuiEvent.ArrayIndex);

                  availableFeatIcons.SetBindValues(player.oid, token, tempIcon);
                  availableFeatNames.SetBindValues(player.oid, token, tempName);
                  listCount.SetBindValue(player.oid, token, tempName.Count);

                  tempIcon = acquiredFeatIcons.GetBindValues(player.oid, token);
                  tempIcon.Add(acquiredFeat.IconResRef);
                  tempName = acquiredFeatNames.GetBindValues(player.oid, token);
                  tempName.Add(acquiredFeat.Name.ToString());

                  acquiredFeatIcons.SetBindValues(player.oid, token, tempIcon);
                  acquiredFeatNames.SetBindValues(player.oid, token, tempName);
                  listAcquiredFeatCount.SetBindValue(player.oid, token, tempName.Count);

                  break;

                case "removeFeat":
                  NwFeat removedFeat = acquiredFeatSearcher[nuiEvent.ArrayIndex];

                  targetCreature.RemoveFeat(removedFeat);

                  if(!availableFeats.Contains(removedFeat))
                    availableFeats.Add(removedFeat);

                  if(!availableFeatSearcher.Contains(removedFeat))
                    availableFeatSearcher.Add(removedFeat);
                  
                  acquiredFeats.Remove(removedFeat);
                  acquiredFeatSearcher.Remove(removedFeat);

                  var tempIconList = acquiredFeatIcons.GetBindValues(player.oid, token);
                  tempIconList.RemoveAt(nuiEvent.ArrayIndex);
                  var tempNameList = acquiredFeatNames.GetBindValues(player.oid, token);
                  tempNameList.RemoveAt(nuiEvent.ArrayIndex);

                  acquiredFeatIcons.SetBindValues(player.oid, token, tempIconList);
                  acquiredFeatNames.SetBindValues(player.oid, token, tempNameList);
                  listAcquiredFeatCount.SetBindValue(player.oid, token, tempNameList.Count);

                  tempIconList = availableFeatIcons.GetBindValues(player.oid, token);
                  tempIconList.Add(removedFeat.IconResRef);
                  tempNameList = availableFeatNames.GetBindValues(player.oid, token);
                  tempNameList.Add(removedFeat.Name.ToString());

                  availableFeatIcons.SetBindValues(player.oid, token, tempIconList);
                  availableFeatNames.SetBindValues(player.oid, token, tempNameList);
                  listCount.SetBindValue(player.oid, token, tempNameList.Count);

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

                case "acquiredFeatSearch":
                  string acquiredFSearch = acquiredFeatSearch.GetBindValue(player.oid, token).ToLower();

                  acquiredFeatSearcher = string.IsNullOrEmpty(acquiredFSearch) ? acquiredFeats : acquiredFeats.Where(f => f.Name.ToString().ToLower().Contains(acquiredFSearch)).ToList();

                  acquiredFeatIcons.SetBindValues(player.oid, token, acquiredFeatSearcher.Select(f => f.IconResRef));
                  acquiredFeatNames.SetBindValues(player.oid, token, acquiredFeatSearcher.Select(f => f.Name.ToString().Replace("’", "'")));
                  listAcquiredFeatCount.SetBindValue(player.oid, token, acquiredFeatSearcher.Count);

                  break;

                case "availableFeatSearch":
                  string availableFSearch = availableFeatSearch.GetBindValue(player.oid, token).ToLower();

                  availableFeatSearcher = string.IsNullOrEmpty(availableFSearch) ? availableFeats : availableFeats.Where(f => f.Name.ToString().ToLower().Contains(availableFSearch)).ToList();

                  availableFeatIcons.SetBindValues(player.oid, token, availableFeatSearcher.Select(f => f.IconResRef));
                  availableFeatNames.SetBindValues(player.oid, token, availableFeatSearcher.Select(f => f.Name.ToString().Replace("’", "'")));
                  listCount.SetBindValue(player.oid, token, availableFeatSearcher.Count);
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
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Strength, (byte)(newStrength - targetCreature.Race.GetAbilityAdjustment(Ability.Strength)));
                    strengthModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Strength).ToString());
                  }
                  break;

                case "dexterity":
                  if (byte.TryParse(dexterity.GetBindValue(player.oid, token), out byte newDeterity))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Dexterity, (byte)(newDeterity - targetCreature.Race.GetAbilityAdjustment(Ability.Dexterity)));
                    reflex.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Reflex).ToString());
                    dodgeChance.SetBindValue(player.oid, token, Utils.GetDodgeChance(targetCreature).ToString());
                    dexterityModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Dexterity).ToString());
                    reflexTooltip.SetBindValue(player.oid, token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Reflex) + targetCreature.GetAbilityModifier(Ability.Dexterity)}");
                  }
                  break;

                case "constitution":
                  if (byte.TryParse(constitution.GetBindValue(player.oid, token), out byte newConstitution))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Constitution, (byte)(newConstitution - targetCreature.Race.GetAbilityAdjustment(Ability.Constitution)));
                    fortitude.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Fortitude).ToString());
                    constitutionModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Constitution).ToString());
                    fortitudeTooltip.SetBindValue(player.oid, token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Fortitude) + targetCreature.GetAbilityModifier(Ability.Constitution)}");
                  }
                  break;

                case "intelligence":
                  if (byte.TryParse(intelligence.GetBindValue(player.oid, token), out byte newIntelligence))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Intelligence, (byte)(newIntelligence - targetCreature.Race.GetAbilityAdjustment(Ability.Intelligence)));
                    intelligenceModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Intelligence).ToString());
                  }
                  break;

                case "wisdom":
                  if (byte.TryParse(wisdom.GetBindValue(player.oid, token), out byte newWisdom))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Wisdom, (byte)(newWisdom - targetCreature.Race.GetAbilityAdjustment(Ability.Wisdom)));
                    will.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Will).ToString());
                    wisdomModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Wisdom).ToString());
                    willTooltip.SetBindValue(player.oid, token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Will) + targetCreature.GetAbilityModifier(Ability.Wisdom)}");
                  }
                  break;

                case "charisma":
                  if (byte.TryParse(charisma.GetBindValue(player.oid, token), out byte newCharisma))
                  {
                    targetCreature.SetsRawAbilityScore(Ability.Charisma, (byte)(newCharisma - targetCreature.Race.GetAbilityAdjustment(Ability.Charisma)));
                    charismaModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Charisma).ToString());
                  }
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

                case "armorPenetration":
                  if (byte.TryParse(armorPenetration.GetBindValue(player.oid, token), out byte newAP))
                  {
                    int previousAttackPerRound = targetCreature.BaseAttackCount;
                    targetCreature.BaseAttackBonus = newAP;

                    if (targetCreature.BaseAttackCount != previousAttackPerRound)
                      targetCreature.BaseAttackCount = previousAttackPerRound;
                  }
                  break;

                case "attackPerRound":
                  if (int.TryParse(attackPerRound.GetBindValue(player.oid, token), out int newNBAttack))
                  {
                    if(newNBAttack < 1 || newNBAttack > 6)
                    {
                      newNBAttack = newNBAttack < 1 ? 1 : newNBAttack;
                      newNBAttack = newNBAttack > 6 ? 6 : newNBAttack;

                      attackPerRound.SetBindValue(player.oid, token, newNBAttack.ToString());
                    }
                    else                   
                      targetCreature.BaseAttackCount = newNBAttack;
                  }
                  break;

                case "spellCasterLevel":
                  if (int.TryParse(armorPenetration.GetBindValue(player.oid, token), out int newCasterLevel))
                  {
                    if (newCasterLevel < 1)
                      spellCasterLevel.SetBindValue(player.oid, token, "1");
                    else
                      targetCreature.GetObjectVariable<LocalVariableInt>("_CREATURE_CASTER_LEVEL").Value = newCasterLevel;
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
              new NuiButton("Stats") { Id = "stats", Height = 35, Width = 60 },
              new NuiButton("Dons") { Id = "feats", Height = 35, Width = 60 }
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
          armorPenetration.SetBindWatch(player.oid, token, false);
          attackPerRound.SetBindWatch(player.oid, token, false);
          spellCasterLevel.SetBindWatch(player.oid, token, false);
          movementRateSelected.SetBindWatch(player.oid, token, false);

          availableFeatSearch.SetBindWatch(player.oid, token, false);
          acquiredFeatSearch.SetBindWatch(player.oid, token, false);
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

          strengthModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Strength).ToString());
          dexterityModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Dexterity).ToString());
          constitutionModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Constitution).ToString());
          intelligenceModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Intelligence).ToString());
          wisdomModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Wisdom).ToString());
          charismaModifier.SetBindValue(player.oid, token, targetCreature.GetAbilityModifier(Ability.Charisma).ToString());

          fortitude.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Fortitude).ToString());
          reflex.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Reflex).ToString());
          will.SetBindValue(player.oid, token, targetCreature.GetBaseSavingThrow(SavingThrow.Will).ToString());

          fortitudeTooltip.SetBindValue(player.oid, token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Fortitude) + targetCreature.GetAbilityModifier(Ability.Constitution)}");
          reflexTooltip.SetBindValue(player.oid, token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Reflex) + targetCreature.GetAbilityModifier(Ability.Dexterity)}");
          willTooltip.SetBindValue(player.oid, token, $"Total avec modificateur : {targetCreature.GetBaseSavingThrow(SavingThrow.Will) + targetCreature.GetAbilityModifier(Ability.Wisdom)}");

          naturalAC.SetBindValue(player.oid, token, targetCreature.BaseAC.ToString());
          naturalACTooltip.SetBindValue(player.oid, token, $"La créature subit naturellement {(Utils.GetDamageMultiplier(targetCreature.AC) * 100).ToString("0.##")} % des dégâts");
          armorPenetration.SetBindValue(player.oid, token, targetCreature.BaseAttackBonus.ToString());
          attackPerRound.SetBindValue(player.oid, token, targetCreature.BaseAttackCount.ToString());
          spellCasterLevel.SetBindValue(player.oid, token, targetCreature.GetObjectVariable<LocalVariableInt>("_CREATURE_CASTER_LEVEL").Value.ToString());

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
          armorPenetration.SetBindWatch(player.oid, token, true);
          attackPerRound.SetBindWatch(player.oid, token, true);
          spellCasterLevel.SetBindWatch(player.oid, token, true);
          hitPoints.SetBindWatch(player.oid, token, true);
          movementRateSelected.SetBindWatch(player.oid, token, true);
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

          columnsChildren.Add( new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Dons disponibles", availableFeatSearch, 20, false) { Width = 190 } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 190  } } }
            }
          });

          columnsChildren.Add(new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Dons acquis", acquiredFeatSearch, 20, false) { Width = 190 } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplateAcquiredFeats, listAcquiredFeatCount) { RowHeight = 35, Width = 190 } } }
            }
          });
        }
        private void LoadFeatsBinding()
        {
          StopAllWatchBindings();

          availableFeats.Clear();
          acquiredFeats.Clear();

          availableFeatSearch.SetBindValue(player.oid, token, "");
          availableFeatSearch.SetBindWatch(player.oid, token, true);
          acquiredFeatSearch.SetBindValue(player.oid, token, "");
          acquiredFeatSearch.SetBindWatch(player.oid, token, true);

          List<string> availableIconsList = new();
          List<string> availableNamesList = new();
          List<string> acquiredIconsList = new();
          List<string> acquiredNamesList = new();

          foreach (Feat feat in (Feat[])Enum.GetValues(typeof(Feat)))
          {
            NwFeat baseFeat = NwFeat.FromFeatType(feat);

            if(targetCreature.KnowsFeat(baseFeat))
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

          availableFeatIcons.SetBindValues(player.oid, token, availableIconsList);
          availableFeatNames.SetBindValues(player.oid, token, availableNamesList);
          acquiredFeatIcons.SetBindValues(player.oid, token, acquiredIconsList);
          acquiredFeatNames.SetBindValues(player.oid, token, acquiredNamesList);
          listCount.SetBindValue(player.oid, token, availableFeats.Count);
          listAcquiredFeatCount.SetBindValue(player.oid, token, acquiredFeats.Count);

          availableFeatSearcher = availableFeats;
          acquiredFeatSearcher = acquiredFeats;
        }
      }
    }
  }
}
