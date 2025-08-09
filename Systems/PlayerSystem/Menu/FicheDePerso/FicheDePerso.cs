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
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false };

        private NwCreature target;
        private Player targetPlayer;

        public FicheDePersoWindow(Player player, NwCreature target) : base(player)
        {
          windowId = "ficheDePerso";
          
          rootColumn.Children = rootChildren;
          rootGroup.Layout = rootColumn;
          menuGroup.Layout = menuRow;

          CreateWindow(target);
        }

        public void CreateWindow(NwCreature target)
        {
          this.target = target;
          if(!Players.TryGetValue(target, out targetPlayer))
            targetPlayer = player;

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(0, 0, windowWidth, windowHeight);

          LoadMainLayout();

          window = new NuiWindow(rootGroup, $"Fiche de perso - {target.Name}")
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
            nuiToken.OnNuiEvent += HandleCharacterSheetEvents;

            MainBindings();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleCharacterSheetEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          //ModuleSystem.Log.Info(nuiEvent.EventType);
          //ModuleSystem.Log.Info(nuiEvent.ElementId);

          if(targetPlayer is null || target is null)
          {
            player.oid.SendServerMessage("Le joueur dont vous essayé de consulter la fiche de personnage n'est plus en ligne", ColorConstants.Red);
            CloseWindow();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "sheetMainView":

                  LoadMainLayout();
                  MainBindings();

                  break;

                case "sheetLearnables":

                  LoadLearnablesLayout();
                  LearnablesBindings();

                  break;

                case "sheetConditions":

                  LoadConditionsLayout();
                  ConditionsBindings();

                  break;

                case "sheetSkills":

                  LoadSkillsLayout();
                  SkillsBindings();

                  break;

                case "sheetWeapons":

                  LoadWeaponsLayout();
                  WeaponsBindings();

                  break;

                case "sheetDescription":

                  LoadDescriptionLayout();
                  DescriptionsBindings();

                  break;

                case "applyDescription":

                  target.Description = description.GetBindValue(player.oid, nuiToken.Token);

                  break;

                case "saveDescription":

                  string currentTitle = title.GetBindValue(player.oid, nuiToken.Token);
                  string currentDescription = description.GetBindValue(player.oid, nuiToken.Token);

                  foreach (var desc in targetPlayer.descriptions)
                  {
                    if (desc.name == currentTitle)
                    {
                      desc.description = currentDescription;
                      SetDescriptionListBindings();
                      return;
                    }
                  }

                  targetPlayer.descriptions.Add(new CharacterDescription(currentTitle, currentDescription));
                  SetDescriptionListBindings();

                  break;

                case "deleteDescription":

                  targetPlayer.descriptions.RemoveAt(nuiEvent.ArrayIndex);
                  SetDescriptionListBindings();

                  break;

                case "cancelJob":
                  targetPlayer.craftJob?.HandleCraftJobCancellation(player);
                  break;

                case "examineJobItem":

                  if (!string.IsNullOrEmpty(targetPlayer.craftJob.serializedCraftedItem))
                  {
                    NwItem item = NwItem.Deserialize(targetPlayer.craftJob.serializedCraftedItem.ToByteArray());

                    if (!player.windows.TryGetValue("itemExamine", out var value)) player.windows.Add("itemExamine", new ItemExamineWindow(player, item));
                    else ((ItemExamineWindow)value).CreateWindow(item);

                    ItemUtils.ScheduleItemForDestruction(item, 300);
                  }

                  break;

                case "sheetSpellbook":

                  CloseWindow();

                  if (!player.windows.TryGetValue("spellBook", out var spellBook)) player.windows.Add("spellBook", new SpellBookWindow(player, targetPlayer));
                  else ((SpellBookWindow)spellBook).CreateWindow(targetPlayer);

                  break;

                case "sheetConfig":

                  LoadConfigLayout();
                  ConfigBindings();

                  break;

                case "sheetDmTools":

                  LoadDmToolsLayout();
                  DmToolsBindings();

                  break;

                case "sheetMailBox":

                  CloseWindow();

                  int areaLevel = player.oid.IsDM ? 0 : player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 1 
                    ? player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value : 0;
                  
                  List<Mail> mailsToDelete = new();

                  foreach (var mail in targetPlayer.mails.Where(m => !m.read && m.fromCharacterId != targetPlayer.characterId))
                    if (Utils.random.Next(101) <= areaLevel)
                      mailsToDelete.Add(mail);

                  foreach (var mail in mailsToDelete)
                    targetPlayer.mails.Remove(mail);

                  if (!player.windows.TryGetValue("mailBox", out var mailBox)) player.windows.Add("mailBox", new MailBox(player, targetPlayer));
                  else ((MailBox)mailBox).CreateWindow(targetPlayer);

                  break;

                case "strRoll": CreatureUtils.RollAbility(target, Ability.Strength); break;
                case "dexRoll": CreatureUtils.RollAbility(target, Ability.Dexterity); break;
                case "conRoll": CreatureUtils.RollAbility(target, Ability.Constitution); break;
                case "intRoll": CreatureUtils.RollAbility(target, Ability.Intelligence); break;
                case "wisRoll": CreatureUtils.RollAbility(target, Ability.Wisdom); break;
                case "chaRoll": CreatureUtils.RollAbility(target, Ability.Charisma); break;

                case "strSaveRoll": CreatureUtils.GetSavingThrowResult(target, Ability.Strength); break;
                case "dexSaveRoll": CreatureUtils.GetSavingThrowResult(target, Ability.Dexterity); break;
                case "conSaveRoll": CreatureUtils.GetSavingThrowResult(target, Ability.Constitution); break;
                case "intSaveRoll": CreatureUtils.GetSavingThrowResult(target, Ability.Intelligence); break;
                case "wisSaveRoll": CreatureUtils.GetSavingThrowResult(target, Ability.Wisdom); break;
                case "chaSaveRoll": CreatureUtils.GetSavingThrowResult(target, Ability.Charisma); break;

                case "rollAthletics": CreatureUtils.RollAbility(target, Ability.Strength, skill:CustomSkill.AthleticsProficiency); break;
                case "rollAcrobatics": CreatureUtils.RollAbility(target, Ability.Dexterity, skill:CustomSkill.AcrobaticsProficiency); break;
                case "rollEscamotage": CreatureUtils.RollAbility(target, Ability.Dexterity, skill:CustomSkill.SleightOfHandProficiency); break;
                case "rollFurtivite": CreatureUtils.RollAbility(target, Ability.Dexterity, skill:CustomSkill.StealthProficiency); break;
                case "rollArcana": CreatureUtils.RollAbility(target, Ability.Intelligence, skill:CustomSkill.ArcanaProficiency); break;
                case "rollHistory": CreatureUtils.RollAbility(target, Ability.Intelligence, skill:CustomSkill.HistoryProficiency); break;
                case "rollInvestigation": CreatureUtils.RollAbility(target, Ability.Intelligence, skill:CustomSkill.InvestigationProficiency); break;
                case "rollNature": CreatureUtils.RollAbility(target, Ability.Intelligence, skill:CustomSkill.NatureProficiency); break;
                case "rollReligion": CreatureUtils.RollAbility(target, Ability.Intelligence, skill:CustomSkill.ReligionProficiency); break;
                case "rollDressage": CreatureUtils.RollAbility(target, Ability.Wisdom, skill:CustomSkill.AnimalHandlingProficiency); break;
                case "rollIntuition": CreatureUtils.RollAbility(target, Ability.Wisdom, skill:CustomSkill.InsightProficiency); break;
                case "rollMedicine": CreatureUtils.RollAbility(target, Ability.Wisdom, skill:CustomSkill.MedicineProficiency); break;
                case "rollPerception": CreatureUtils.RollAbility(target, Ability.Wisdom, skill:CustomSkill.PerceptionProficiency); break;
                case "rollSurvie": CreatureUtils.RollAbility(target, Ability.Wisdom, skill:CustomSkill.SurvivalProficiency); break;
                case "rollTromperie": CreatureUtils.RollAbility(target, Ability.Charisma, skill:CustomSkill.DeceptionProficiency); break;
                case "rollPersuasion": CreatureUtils.RollAbility(target, Ability.Charisma, skill:CustomSkill.PersuasionProficiency); break;
                case "rollPerformance": CreatureUtils.RollAbility(target, Ability.Charisma, skill:CustomSkill.PerformanceProficiency); break;
                case "rollIntimidation": CreatureUtils.RollAbility(target, Ability.Charisma, skill:CustomSkill.IntimidationProficiency); break;
              }

              break;

            case NuiEventType.MouseUp:

              switch (nuiEvent.ElementId)
              {
                case "sheetPortrait":

                  LoadPortraitLayout();
                  PortraitBindings();

                  break;

                case "selectDescription":

                  title.SetBindValue(player.oid, nuiToken.Token, targetPlayer.descriptions[nuiEvent.ArrayIndex].name);
                  description.SetBindValue(player.oid, nuiToken.Token, targetPlayer.descriptions[nuiEvent.ArrayIndex].description);

                  break;

                case "sacrificeHP": NWScript.AssignCommand(player.oid.ControlledCreature, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage((int)(target.MaxHP * 0.2)))); break;

                case "dmMode":

                  player.oid.IsPlayerDM = !player.oid.IsPlayerDM;

                  if (player.oid.IsPlayerDM)
                    dmModeLabel.SetBindValue(player.oid, nuiToken.Token, "Mode DM");
                  else
                    dmModeLabel.SetBindValue(player.oid, nuiToken.Token, "Sortir du mode DM");

                  break;

                case "chatimentLevelSelection":

                  if (!player.windows.TryGetValue("chatimentLevelSelection", out var chatiment))
                    player.windows.Add("chatimentLevelSelection", new ChatimentLevelSelectionWindow(player));
                  else if (((ChatimentLevelSelectionWindow)chatiment).IsOpen)
                    ((ChatimentLevelSelectionWindow)chatiment).CloseWindow();
                  else
                    ((ChatimentLevelSelectionWindow)chatiment).CreateWindow();

                  break;

                case "creaturePalette":

                  if (!player.windows.TryGetValue("paletteCreature", out var paletteCreature)) player.windows.Add("paletteCreature", new PaletteCreatureWindow(player));
                  else ((PaletteCreatureWindow)paletteCreature).CreateWindow();

                  CloseWindow();

                  break;

                case "itemPalette":

                  if (!player.windows.TryGetValue("paletteItem", out var paletteItem)) player.windows.Add("paletteItem", new PaletteItemWindow(player));
                  else ((PaletteItemWindow)paletteItem).CreateWindow();

                  CloseWindow();

                  break;

                case "placeablePalette":

                  if (!player.windows.TryGetValue("palettePlaceable", out var palettePlc)) player.windows.Add("palettePlaceable", new PalettePlaceableWindow(player));
                  else ((PalettePlaceableWindow)palettePlc).CreateWindow();

                  CloseWindow();

                  break;

                case "placeableManager":

                  if (!player.windows.TryGetValue("placeableManager", out var placeable)) player.windows.Add("placeableManager", new PlaceableManagerWindow(player));
                  else ((PlaceableManagerWindow)placeable).CreateWindow();

                  CloseWindow();

                  break;

                case "language":

                  if (!player.windows.TryGetValue("languageSelection", out var language)) player.windows.Add("languageSelection", new LanguageSelectionWindow(player));
                  else ((LanguageSelectionWindow)language).CreateWindow();

                  CloseWindow();

                  break;

                case "sit":

                  if (!player.windows.TryGetValue("sitAnywhere", out var value)) player.windows.Add("sitAnywhere", new SitAnywhereWindow(player));
                  else ((SitAnywhereWindow)value).CreateWindow();

                  break;

                case "touch":

                  if (target.ActiveEffects.Any(e => e.Tag == EffectSystem.ModeToucherEffectTag))
                  {
                    EffectUtils.RemoveTaggedEffect(target, EffectSystem.ModeToucherEffectTag);
                    touchLabel.SetBindValue(player.oid, nuiToken.Token, "Mode toucher (sans collisions)");
                  }
                  else
                  {
                    NWScript.AssignCommand(target, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.ModeToucher));
                    touchLabel.SetBindValue(player.oid, nuiToken.Token, "Mode toucher (avec collisions)");
                  }

                  break;

                case "walk":

                  if (target.AlwaysWalk)
                  {
                    target.AlwaysWalk = false;
                    walkLabel.SetBindValue(player.oid, nuiToken.Token, "Mode marche");
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableBool>("_ALWAYS_WALK").Delete();
                  }
                  else
                  {
                    target.AlwaysWalk = true;
                    walkLabel.SetBindValue(player.oid, nuiToken.Token, "Mode course");
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableBool>("_ALWAYS_WALK").Value = true;
                  }

                  break;

                case "suivre":

                  if (target.MovementRate == MovementRate.Immobile
                    || Encumbrance2da.IsCreatureHeavilyEncumbred(target))
                  {
                    player.oid.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", ColorConstants.Red);
                    return;
                  }

                  player.oid.EnterTargetMode(FollowTarget, Config.selectItemTargetMode);

                  CloseWindow();

                  break;

                case "examineArea":

                  if (!player.windows.TryGetValue("areaDescription", out var examineArea)) player.windows.Add("areaDescription", new AreaDescriptionWindow(player, player.oid.ControlledCreature.Area));
                  else ((AreaDescriptionWindow)examineArea).CreateWindow(player.oid.ControlledCreature.Area);

                  break;

                case "quickbars":

                  if (!player.windows.TryGetValue("quickbars", out var quickbar)) player.windows.Add("quickbars", new QuickbarsWindow(player));
                  else ((QuickbarsWindow)quickbar).CreateWindow();

                  CloseWindow();

                  break;

                case "commend": player.oid.EnterTargetMode(CommendTarget, Config.selectCreatureTargetMode); break;

                case "itemAppearance":

                  if (!player.windows.TryGetValue("itemAppearances", out var itemAppearances)) player.windows.Add("itemAppearances", new ItemAppearancesWindow(player));
                  else ((ItemAppearancesWindow)itemAppearances).CreateWindow();

                  CloseWindow();

                  break;

                case "chat":

                  if (!player.windows.TryGetValue("chatColors", out var chatColors)) player.windows.Add("chatColors", new ChatColorsWindow(player));
                  else ((ChatColorsWindow)chatColors).CreateWindow();

                  CloseWindow();

                  break;

                case "unstuck":

                  NWScript.AssignCommand(target, () => NWScript.JumpToLocation(NWScript.GetLocation(target)));
                  player.oid.SendServerMessage("Tentative de déblocage effectuée.", ColorConstants.Orange);

                  break;

                case "reinitPositionDisplay":

                  Utils.ResetVisualTransform(target);
                  player.oid.SendServerMessage("Affichage réinitialisé.", ColorConstants.Orange);

                  break;

                case "publicKey": player.oid.SendServerMessage($"La clef publique de {target.Name.ColorString(ColorConstants.White)} est : {targetPlayer.oid.CDKey.ColorString(ColorConstants.White)}", ColorConstants.Pink); break;
                case "delete": _ = player.oid.Delete($"Le personnage {player.oid.LoginCreature.Name} a été supprimé."); break;

                case "wind":

                  if (!player.windows.TryGetValue("areaWindSettings", out var wind)) player.windows.Add("areaWindSettings", new AreaWindSettings(player));
                  else ((AreaWindSettings)wind).CreateWindow();

                  CloseWindow();

                  break;

                case "dmRename":

                  player.oid.SendServerMessage("Veuillez sélectionner la cible à renommer");
                  player.oid.EnterTargetMode(RenameTarget, Config.selectItemTargetMode);
                  CloseWindow();

                  break;

                case "visualEffects":

                  if (!player.windows.TryGetValue("DMVisualEffects", out var vfx)) player.windows.Add("DMVisualEffects", new DMVisualEffectsWindow(player));
                  else ((DMVisualEffectsWindow)vfx).CreateWindow();

                  CloseWindow();
                  break;

                case "aoeVisualEffects":

                  if (!player.windows.TryGetValue("DMAoEVisualEffects", out var aoe)) player.windows.Add("DMAoEVisualEffects", new DMAoEVisualEffectsWindow(player));
                  else ((DMAoEVisualEffectsWindow)aoe).CreateWindow();

                  CloseWindow();
                  break;

                case "areaMusicEditor":

                  if (!player.windows.TryGetValue("areaMusicEditor", out var areaMusic)) player.windows.Add("areaMusicEditor", new AreaMusicEditorWindow(player, player.oid.ControlledCreature.Area));
                  else ((AreaMusicEditorWindow)areaMusic).CreateWindow(player.oid.ControlledCreature.Area);

                  CloseWindow();

                  break;

                case "areaLoadScreenEditor":

                  if (!player.windows.TryGetValue("areaLoadScreenEditor", out var loadScreen)) player.windows.Add("areaLoadScreenEditor", new AreaLoadScreenEditorWindow(player, player.oid.ControlledCreature.Area));
                  else ((AreaLoadScreenEditorWindow)loadScreen).CreateWindow(player.oid.ControlledCreature.Area);

                  CloseWindow();

                  break;

                case "customDiceRolls":

                  if (!player.windows.TryGetValue("customDiceRolls", out var customDiceRolls)) player.windows.Add("customDiceRolls", new CustomDiceRolls(player));
                  else ((CustomDiceRolls)customDiceRolls).CreateWindow();

                  CloseWindow();
                  break;

                case "reboot":

                  NwServer.Instance.PlayerPassword = "REBOOTINPROGRESS";
                  TradeSystem.saveScheduled = true;

                  CloseWindow();

                  foreach (NwPlayer connectingPlayer in NwModule.Instance.Players.Where(p => p.LoginCreature == null))
                    connectingPlayer.BootPlayer("Navré, le module est en cours de redémarrage. Vous pourrez vous reconnecter dans une minute.");

                  foreach (Player connectedPlayer in Players.Values.Where(p => p.pcState != PcState.Offline))
                    connectedPlayer.windows.Add("rebootCountdown", new RebootCountdownWindow(connectedPlayer));

                  var scheduler = player.scheduler.Schedule(() =>
                  {
                    SqLiteUtils.UpdateQuery("moduleInfo",
                      new List<string[]>() { new string[] { "year", NwDateTime.Now.Year.ToString() }, new string[] { "month", NwDateTime.Now.Month.ToString() }, new string[] { "day", NwDateTime.Now.DayInTenday.ToString() }, new string[] { "hour", NwDateTime.Now.Hour.ToString() }, new string[] { "minute", NwDateTime.Now.Minute.ToString() }, new string[] { "second", NwDateTime.Now.Second.ToString() } },
                      new List<string[]>() { new string[] { "rowid", "1" } });
                  }, TimeSpan.FromSeconds(31));

                  var schedulerTradeSystemSave = player.scheduler.Schedule(() => { TradeSystem.SaveToDatabase(); }, TimeSpan.FromSeconds(31));
                  var schedulerReboot = player.scheduler.Schedule(() => { NwServer.Instance.ShutdownServer(); }, TimeSpan.FromSeconds(35));

                  break;

                case "refill":
                  ModuleSystem.SpawnCollectableResources();
                  break;

                case "instantLearn":

                  if (target.GetObjectVariable<PersistentVariableInt>("_INSTANT_LEARN").HasValue)
                  {
                    target.GetObjectVariable<PersistentVariableInt>("_INSTANT_LEARN").Delete();
                    instantLearnLabel.SetBindValue(player.oid, nuiToken.Token, "Activer l'apprentissage instantanné");
                  }
                  else
                  {
                    target.GetObjectVariable<PersistentVariableInt>("_INSTANT_LEARN").Value = 1;
                    instantLearnLabel.SetBindValue(player.oid, nuiToken.Token, "Désactiver l'apprentissage instantanné");
                    LogUtils.LogMessage($"{player.oid.PlayerName} active apprentissage instantanné sur {target.Name}", LogUtils.LogType.DMAction);
                  }

                  break;

                case "rollDM":

                  if (target.GetObjectVariable<PersistentVariableInt>("_ROLL_DM").HasValue)
                  {
                    target.GetObjectVariable<PersistentVariableInt>("_ROLL_DM").Delete();
                  }
                  else
                  {
                    target.GetObjectVariable<PersistentVariableInt>("_ROLL_DM").Value = 1;
                  }

                  break;

                case "rollPrivate":

                  if (target.GetObjectVariable<PersistentVariableInt>("_ROLL_PRIVATE").HasValue)
                  {
                    target.GetObjectVariable<PersistentVariableInt>("_ROLL_PRIVATE").Delete();
                  }
                  else
                  {
                    target.GetObjectVariable<PersistentVariableInt>("_ROLL_PRIVATE").Value = 1;
                  }

                  break;

                case "instantCraft":

                  if (targetPlayer.craftJob != null)
                  {
                    targetPlayer.craftJob.remainingTime = 1;
                    LogUtils.LogMessage($"{player.oid.PlayerName} utilise craft instantanné sur {target.Name} ({targetPlayer.craftJob.type})", LogUtils.LogType.DMAction);
                  }

                  break;

                case "giveResources":

                  if (player.windows.TryGetValue("resourceDMGift", out var giveResources)) ((ResourceDMGiftWindow)giveResources).CreateWindow(targetPlayer);
                  else player.windows.Add("resourceDMGift", new ResourceDMGiftWindow(player, targetPlayer));

                  break;

                case "giveSkillbook":

                  if (player.windows.TryGetValue("skillbookDMGift", out var giveSkillbook)) ((SkillBookDMGiftWindow)giveSkillbook).CreateWindow(targetPlayer);
                  else player.windows.Add("skillbookDMGift", new SkillBookDMGiftWindow(player, targetPlayer));

                  break;

                case "learnables":

                  if (!player.windows.TryGetValue("learnables", out var learnables)) player.windows.Add("learnables", new LearnableWindow(player, targetPlayer));
                  else ((LearnableWindow)learnables).CreateWindow(targetPlayer);

                  break;

                case "addClass":

                  /*if (!player.oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Wizard))
                      player.oid.LoginCreature.ForceLevelUp(NwClass.FromClassType(ClassType.Wizard).Id, 1);

                    if (!player.windows.TryGetValue("spellSelection", out var masterSpell)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 3, 6));
                    else ((SpellSelectionWindow)masterSpell).CreateWindow(ClassType.Wizard, 3, 6);
                    
                    if (!player.oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Sorcerer))
                    {
                      player.oid.LoginCreature.ForceLevelUp(NwClass.FromClassType(ClassType.Sorcerer).Id, 1);
                      return;
                    }

                    if (!player.oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Bard))
                    {
                      player.oid.LoginCreature.ForceLevelUp(NwClass.FromClassType(ClassType.Bard).Id, 1);
                      return;
                    }

                    if (!player.oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Cleric))
                    {
                      player.oid.LoginCreature.ForceLevelUp(NwClass.FromClassType(ClassType.Cleric).Id, 1);
                      return;
                    }

                    player.learnableSpells.Clear();

                    foreach (var playerClass in player.oid.LoginCreature.Classes)
                      foreach (var knownSpell in playerClass.KnownSpells)
                        knownSpell.Clear();

                    foreach (LearnableSpell learnable in SkillSystem.learnableDictionary.Values.Where(l => l is LearnableSpell).Cast<LearnableSpell>())
                    {
                      if (!player.learnableSpells.TryGetValue(learnable.id, out var spell))
                      {
                        List<int> classAvailability = new();
                        foreach (var learnClass in learnable.availableToClasses)
                          classAvailability.Add((int)learnClass);

                        player.learnableSpells.Add(learnable.id, new LearnableSpell(learnable, classAvailability));
                        player.learnableSpells[learnable.id].LevelUp(player);
                      }
                      else if (spell.currentLevel < 1)
                      {
                        spell.LevelUp(player);
                      }
                    }*/

                  break;
              }

              if (nuiEvent.ElementId.StartsWith("po_"))
                player.oid.LoginCreature.PortraitResRef = nuiEvent.ElementId.EndsWith("m2") ? nuiEvent.ElementId[..^2] : nuiEvent.ElementId[..^1];

              break;
          }
        }
      }
    }
  }
}
