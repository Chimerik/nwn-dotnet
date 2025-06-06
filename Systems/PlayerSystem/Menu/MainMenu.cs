﻿using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

using Google.Apis.Drive.v3.Data;

using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class MainMenuWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiBind<string> buttonName = new("buttonName");
        private readonly NuiBind<string> buttonTooltip = new("buttonTooltip");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> search = new("search");

        private NwObject selectionTarget;

        private Dictionary<string, Utils.MainMenuCommand> myCommandList = new();
        private Dictionary<string, Utils.MainMenuCommand> currentList;

        public MainMenuWindow(Player player) : base(player)
        {
          windowId = "mainMenu";

          rootColumn.Children = rootChildren;
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton(buttonName) { Id = "command", Tooltip = buttonTooltip, Height = 35 }) { VariableSize = true });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 370 } } });
          rootChildren.Add(new NuiRow() { Height = 385, Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          myCommandList.Clear();

          if (player.oid.PlayerName == "Chim" || player.oid.PlayerName == "dodozik" || player.oid.PlayerName == "WingsOfJoy")
            myCommandList = Utils.mainMenuCommands.ToDictionary(m => m.Key, m => m.Value);
          else if (player.oid.IsDM)
            myCommandList = Utils.mainMenuCommands.Where(m => m.Value.rank < Utils.CommandRank.Admin).ToDictionary(m => m.Key, m => m.Value);
          else
          {
            myCommandList = Utils.mainMenuCommands.Where(m => m.Value.rank < Utils.CommandRank.DM).ToDictionary(m => m.Key, m => m.Value);

            if (player.bonusRolePlay < 4)
              myCommandList.Remove("commend");
          }

          if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION").HasValue)
            myCommandList.Remove("learnables");

          if (player.craftJob == null)
            myCommandList.Remove("currentJob");

          if (!player.subscriptions.Any(s => s.type == Utils.SubscriptionType.MailDistantAccess))
            myCommandList.Remove("mailBox");

          if (!player.oid.LoginCreature.Classes.Any(c => c.Class.IsSpellCaster))
            myCommandList.Remove("spellBook");

          if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChatimentDivin))
            myCommandList.Remove("chatimentLevelSelection");

          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, 500);

          window = new NuiWindow(rootColumn, "Menu principal")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = collapsed,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleMainMenuEvents;

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);

            collapsed.SetBindValue(player.oid, nuiToken.Token, false);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
            
            if (!AreaSystem.areaDescriptions.ContainsKey(player.oid.ControlledCreature?.Area.Name))
              myCommandList.Remove("examineArea");
            else
              myCommandList.TryAdd("examineArea", Utils.mainMenuCommands["examineArea"]);

            if (myCommandList.TryGetValue("instantLearn", out var instantLearn))
              instantLearn.label = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_INSTANT_LEARN").HasValue ?
                "Désactiver l'apprentissage instantané (Alpha)" : "Activer l'apprentissage instantané (Alpha)";

            currentList = myCommandList;
            LoadMenu(currentList);
          }
        }
        private void HandleMainMenuEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          try
          {
            switch (nuiEvent.EventType)
            {
              case NuiEventType.Click:

                if (nuiEvent.ArrayIndex < 0)
                  return;

                switch (currentList.Keys.ElementAt(nuiEvent.ArrayIndex))
                {
                  case "mailBox":

                    int areaLevel = player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 1 ? player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value : 0;
                    List<Mail> mailsToDelete = new();

                    foreach (var mail in player.mails.Where(m => !m.read && m.fromCharacterId != player.characterId))
                      if (Utils.random.Next(101) <= areaLevel)
                        mailsToDelete.Add(mail);

                    foreach (var mail in mailsToDelete)
                      player.mails.Remove(mail);

                    if (!player.windows.ContainsKey("mailBox")) player.windows.Add("mailBox", new MailBox(player));
                    else ((MailBox)player.windows["mailBox"]).CreateWindow();

                    break;

                  case "touch":

                    var effectList = player.oid.ControlledCreature.ActiveEffects.Where(e => e.EffectType == EffectType.CutsceneGhost);

                    if (!player.oid.ControlledCreature.ActiveEffects.Any(e => e.EffectType == EffectType.CutsceneGhost))
                    {
                      player.oid.ControlledCreature.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneGhost());
                      player.oid.SendServerMessage("Activation du mode toucher", ColorConstants.Orange);
                    }
                    else
                    {
                      foreach (var eff in player.oid.ControlledCreature.ActiveEffects.Where(e => e.EffectType == EffectType.CutsceneGhost && e.Tag != EffectSystem.AgiliteHalfelinEffectTag))
                        player.oid.ControlledCreature.RemoveEffect(eff);

                      player.oid.SendServerMessage("Désactivation du mode toucher", ColorConstants.Orange);
                    }

                    break;

                  case "walk":

                    if (player.oid.ControlledCreature.AlwaysWalk)
                    {
                      player.oid.ControlledCreature.AlwaysWalk = false;
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableBool>("_ALWAYS_WALK").Delete();
                      player.oid.SendServerMessage("Désactivation du mode marche.", ColorConstants.Orange);
                    }
                    else
                    {
                      player.oid.ControlledCreature.AlwaysWalk = true;
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableBool>("_ALWAYS_WALK").Value = true;
                      player.oid.SendServerMessage("Activation du mode marche.", ColorConstants.Orange);
                    }

                    break;

                  case "sit":

                    if (!player.windows.TryGetValue("sitAnywhere", out var value)) player.windows.Add("sitAnywhere", new SitAnywhereWindow(player));
                    else ((SitAnywhereWindow)value).CreateWindow();

                    break;

                  case "examineArea":

                    if (!player.windows.ContainsKey("areaDescription")) player.windows.Add("areaDescription", new AreaDescriptionWindow(player, player.oid.ControlledCreature.Area));
                    else ((AreaDescriptionWindow)player.windows["areaDescription"]).CreateWindow(player.oid.ControlledCreature.Area);

                    break;

                  case "grimoire":

                    if (!player.windows.ContainsKey("grimoires")) player.windows.Add("grimoires", new GrimoiresWindow(player));
                    else ((GrimoiresWindow)player.windows["grimoires"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "quickbars":

                    if (!player.windows.ContainsKey("quickbars")) player.windows.Add("quickbars", new QuickbarsWindow(player));
                    else ((QuickbarsWindow)player.windows["quickbars"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "itemAppearance":

                    if (!player.windows.ContainsKey("itemAppearances")) player.windows.Add("itemAppearances", new ItemAppearancesWindow(player));
                    else ((ItemAppearancesWindow)player.windows["itemAppearances"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "description":

                    if (!player.windows.ContainsKey("descriptions")) player.windows.Add("descriptions", new DescriptionsWindow(player));
                    else ((DescriptionsWindow)player.windows["descriptions"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "unstuck":

                    NWScript.AssignCommand(player.oid.ControlledCreature, () => NWScript.JumpToLocation(NWScript.GetLocation(player.oid.ControlledCreature)));
                    player.oid.SendServerMessage("Tentative de déblocage effectuée.", ColorConstants.Orange);

                    break;

                  case "reinitPositionDisplay":

                    Utils.ResetVisualTransform(player.oid.ControlledCreature);
                    player.oid.SendServerMessage("Affichage réinitialisé.", ColorConstants.Orange);

                    break;

                  case "publicKey":
                    player.oid.SendServerMessage($"Votre clef publique est : {player.oid.CDKey.ColorString(ColorConstants.White)}", ColorConstants.Pink);
                    break;

                  case "delete":
                    AdminPlugin.DeletePlayerCharacter(player.oid.LoginCreature, 1, $"Le personnage {player.oid.LoginCreature.Name} a été supprimé.");
                    break;

                  case "chat":

                    if (!player.windows.TryGetValue("chatColors", out var chatColors)) player.windows.Add("chatColors", new ChatColorsWindow(player));
                    else ((ChatColorsWindow)chatColors).CreateWindow();

                    CloseWindow();

                    break;

                  case "wind":

                    if (!player.windows.TryGetValue("areaWindSettings", out var wind)) player.windows.Add("areaWindSettings", new AreaWindSettings(player));
                    else ((AreaWindSettings)wind).CreateWindow();

                    CloseWindow();

                    break;

                  case "dm":
                    player.oid.IsPlayerDM = !player.oid.IsPlayerDM;
                    break;

                  case "dmRename":
                    player.oid.SendServerMessage("Veuillez sélectionner la cible à renommer");
                    player.oid.EnterTargetMode(RenameTarget, Config.selectItemTargetMode);
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
                    //player.oid.SendServerMessage("Veuillez sélectionner la cible de l'apprentissage instantanné.");
                    //player.oid.EnterTargetMode(SelectLearnTarget, Config.selectCreatureTargetMode);

                    if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_INSTANT_LEARN").HasValue)
                    {
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_INSTANT_LEARN").Delete();
                      myCommandList["instantLearn"].label = "Activer l'apprentissage instantanné (Alpha)";
                      buttonName.SetBindValues(player.oid, nuiToken.Token, myCommandList.Values.Select(c => c.label));
                    }
                    else
                    {
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_INSTANT_LEARN").Value = 1;
                      myCommandList["instantLearn"].label = "Désactiver l'apprentissage instantanné (Alpha)";
                      buttonName.SetBindValues(player.oid, nuiToken.Token, myCommandList.Values.Select(c => c.label));
                    }
                    
                    CloseWindow();
                    break;

                  case "instantCraft":
                    player.oid.SendServerMessage("Veuillez sélectionner la cible du craft instantanné.");
                    player.oid.EnterTargetMode(SelectCraftTarget, Config.selectCreatureTargetMode);
                    CloseWindow();
                    break;

                  case "giveResources":
                    player.oid.SendServerMessage("Veuillez sélectionner la cible du don.");
                    player.oid.EnterTargetMode(SelectGiveResourcesTarget, Config.selectCreatureTargetMode);
                    CloseWindow();
                    break;

                  case "giveSkillbook":
                    player.oid.SendServerMessage("Veuillez sélectionner la cible du don.");
                    player.oid.EnterTargetMode(SelectGiveSkillbookTarget, Config.selectCreatureTargetMode);
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

                  case "dispelAoE":

                    if (!player.windows.ContainsKey("aoeDispel")) player.windows.Add("aoeDispel", new AoEDispelWindow(player));
                    else ((AoEDispelWindow)player.windows["aoeDispel"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "effectDispel":

                    if (!player.windows.ContainsKey("effectDispel")) player.windows.Add("effectDispel", new PlayerEffectDispelWindow(player));
                    else ((PlayerEffectDispelWindow)player.windows["effectDispel"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "follow":

                    if (player.oid.ControlledCreature.MovementRate == MovementRate.Immobile
                      || Encumbrance2da.IsCreatureHeavilyEncumbred(player.oid.ControlledCreature))
                    {
                      player.oid.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", ColorConstants.Red);
                      return;
                    }

                    player.oid.EnterTargetMode(FollowTarget, Config.selectItemTargetMode);

                    CloseWindow();

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

                  case "learnables":

                    if (!player.windows.TryGetValue("learnables", out var learnables)) player.windows.Add("learnables", new LearnableWindow(player));
                    else ((LearnableWindow)learnables).CreateWindow();

                    CloseWindow();

                    break;

                  case "currentJob":

                    if (player.craftJob != null)
                    {
                      if (!player.windows.TryGetValue("activeCraftJob", out var job)) player.windows.Add("activeCraftJob", new ActiveCraftJobWindow(player));
                      else ((ActiveCraftJobWindow)job).CreateWindow();

                      CloseWindow();
                    }

                    break;

                  case "language":

                    if (!player.windows.TryGetValue("languageSelection", out var language)) player.windows.Add("languageSelection", new LanguageSelectionWindow(player));
                    else ((LanguageSelectionWindow)language).CreateWindow();

                    CloseWindow();

                    break;

                  /*case "healthManaBars":

                    if (!player.windows.ContainsKey("healthBar")) player.windows.Add("healthBar", new HealthBarWindow(player));
                    else ((HealthBarWindow)player.windows["healthBar"]).CreateWindow();

                    if (!player.windows.ContainsKey("energyBar")) player.windows.Add("energyBar", new EnergyBarWindow(player));
                    else ((EnergyBarWindow)player.windows["energyBar"]).CreateWindow();

                    CloseWindow();

                    break;*/

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

                  case "cooldownPosition":

                    if (!player.windows.TryGetValue("cooldownPosition", out var cooldown)) player.windows.Add("cooldownPosition", new CooldownPositionSetter(player));
                    else ((CooldownPositionSetter)cooldown).CreateWindow();

                    CloseWindow();

                    break;

                  case "lootEditor":

                    if (!player.windows.TryGetValue("lootEditor", out var lootEditor)) player.windows.Add("lootEditor", new LootEditorWindow(player));
                    else ((LootEditorWindow)lootEditor).CreateWindow();

                    CloseWindow();

                    break;

                  case "sacrificeHP": NWScript.AssignCommand(player.oid.ControlledCreature, () => player.oid.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.Damage((int)(player.oid.ControlledCreature.MaxHP * 0.2)))); break;

                  case "spellBook":

                    if (!player.windows.TryGetValue("spellBook", out var spellBook)) player.windows.Add("spellBook", new SpellBookWindow(player));
                    else ((SpellBookWindow)spellBook).CreateWindow();

                    break;

                  case "chatimentLevelSelection":

                    if (!player.windows.TryGetValue("chatimentLevelSelection", out var chatiment)) 
                      player.windows.Add("chatimentLevelSelection", new ChatimentLevelSelectionWindow(player));
                    else if (((ChatimentLevelSelectionWindow)chatiment).IsOpen)
                      ((ChatimentLevelSelectionWindow)chatiment).CloseWindow();
                    else
                      ((ChatimentLevelSelectionWindow)chatiment).CreateWindow();

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

                  case "shortRest":

                    //CreatureUtils.HandleShortRest(player);
                    //SpellUtils.DispelConcentrationEffects(player.oid.LoginCreature);

                    break;

                  case "longRest":

                    /*var nbInspiHeroique = player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.InspirationHeroique);

                    if (Utils.In(player.oid.LoginCreature.Race.RacialType, RacialType.Human, (RacialType)CustomRace.WoodHalfElf, (RacialType)CustomRace.DrowHalfElf, (RacialType)CustomRace.HighHalfElf)
                      && nbInspiHeroique < 3)
                      nbInspiHeroique += 1;

                    player.shortRest = 0;

                    player.oid.LoginCreature.ForceRest();
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableString>(CreatureUtils.RegardHypnotiqueTargetListVariable).Delete();
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(EffectSystem.EvocateurSurchargeVariable).Delete();
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ILLUSION_SEE_INVI_COOLDOWN").Delete();
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(Wizard.TransmutationStoneVariable).Delete();
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableString>(CreatureUtils.CharmeInstinctifVariable).Delete();

                    RangerUtils.RestoreEnnemiJure(player.oid.LoginCreature);
                    FighterUtils.RestoreManoeuvres(player.oid.LoginCreature);
                    FighterUtils.RestoreTirArcanique(player.oid.LoginCreature);
                    FighterUtils.RestoreSecondSouffle(player.oid.LoginCreature);
                    BarbarianUtils.RestoreBarbarianRage(player.oid.LoginCreature);
                    MonkUtils.RestoreKi(player.oid.LoginCreature);
                    WizardUtils.RestaurationArcanique(player.oid.LoginCreature);
                    DruideUtils.RecuperationNaturelle(player.oid.LoginCreature);
                    RangerUtils.RegenerationNaturelle(player.oid.LoginCreature);
                    WizardUtils.ResetAbjurationWard(player.oid.LoginCreature);
                    WizardUtils.ResetPresage(player.oid);
                    FighterUtils.RestoreEldritchKnight(player.oid.LoginCreature);
                    BardUtils.RestoreInspirationBardique(player.oid.LoginCreature);
                    PaladinUtils.RestorePaladinCharges(player.oid.LoginCreature);
                    ClercUtils.RestoreConduitDivin(player.oid.LoginCreature);
                    ClercUtils.RestoreInterventionDivine(player.oid.LoginCreature);
                    ClercUtils.RestoreClercDomaine(player.oid.LoginCreature);
                    EnsoUtils.RestoreSorcerySource(player.oid.LoginCreature);
                    DruideUtils.RestoreFormeSauvage(player.oid.LoginCreature);
                    OccultisteUtils.RestoreFouleeFeerique(player.oid.LoginCreature);
                    OccultisteUtils.RestoreLueurDeGuerison(player.oid.LoginCreature);
                    OccultisteUtils.HandleResilienceCeleste(player.oid.LoginCreature);
                    player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.DonDuProtecteur, (byte)(player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 1 ? player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) : 1));

                    if (player.oid.LoginCreature.Race.Id == CustomRace.HalfOrc)
                    {
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).Value = 1;
                      player.ApplyHalfOrcEndurance();
                    }

                    if (player.learnableSkills.ContainsKey(CustomSkill.PaladinSentinelleImmortelle))
                    {
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.SentinelleImmortelleVariable).Value = 1;
                      player.ApplySentinelleImmortelle();
                    }

                    if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BersekerFrenziedStrike))
                      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.BersekerFrenziedStrike, 0);

                    if (player.oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.ClassType, ClassType.Fighter, (ClassType)CustomClass.EldritchKnight) && c.Level < 17))
                      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FighterSurge, 1);
                     
                    player.oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.CanPrepareSpells, NwTimeSpan.FromHours(1));

                    foreach (var classInfo in player.oid.LoginCreature.Classes.Where(c => c.Class.IsSpellCaster))
                    {
                      var spellGainTable = classInfo.Class.SpellGainTable[classInfo.Level - 1];
                      byte i = 0;

                      foreach(var spellGain in spellGainTable)
                      {
                        classInfo.SetRemainingSpellSlots(i, spellGain);
                        i++;
                      }
                    }

                    if(player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChatimentDivin))
                    {
                      byte chatimentLevel = (byte)(player.windows.TryGetValue("chatimentLevelSelection", out var chatimentWindow) 
                        ? ((ChatimentLevelSelectionWindow)chatimentWindow).selectedSpellLevel : 1);
                      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentDivin, player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(chatimentLevel));
                    }

                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ClercMartialVariable).Delete();

                    EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.DivinationVisionEffectTag);
                    SpellUtils.DispelConcentrationEffects(player.oid.LoginCreature);

                    if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChatimentOcculte))
                      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentOcculte, (byte)(player.oid.LoginCreature.GetClassInfo((ClassType)CustomClass.Occultiste).GetRemainingSpellSlots(1)));

                    player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercIncantationPuissante, 0);
                    player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.InspirationHeroique, nbInspiHeroique);
                    */
                    break;
                }
                break;

              case NuiEventType.Watch:

                switch (nuiEvent.ElementId)
                {
                  case "search":

                    string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                    currentList = string.IsNullOrEmpty(currentSearch) ? myCommandList : myCommandList.Where(v => v.Value.label.ToLower().Contains(currentSearch)).ToDictionary(c => c.Key, c => c.Value);
                    LoadMenu(currentList);

                    break;
                }

                break;
            }
          }
          catch(Exception e)
          {
            Utils.LogMessageToDMs($"{e.Message}\n{e.StackTrace}\narray index : {nuiEvent.ArrayIndex} - key collection count {currentList.Count}");
          }
        }
        private void LoadMenu(Dictionary<string, Utils.MainMenuCommand> commandList)
        {
          buttonName.SetBindValues(player.oid, nuiToken.Token, commandList.Values.Select(c => c.label));
          buttonTooltip.SetBindValues(player.oid, nuiToken.Token, commandList.Values.Select(c => c.tooltip));
          listCount.SetBindValue(player.oid, nuiToken.Token, commandList.Count);
        }
        private static bool AreaDescriptionExists(NwPlayer oPlayer)
        {
          try
          {
            var request = ModuleSystem.googleDriveService.Files.List();
            request.Q = $"name = '{oPlayer.ControlledCreature.Area.Name.Replace("'", "")}'";

            LogUtils.LogMessage($"Area - {oPlayer.ControlledCreature.Area.Name} - Description request - {oPlayer.ControlledCreature.Name} - {request.Q}", LogUtils.LogType.AreaManagement);

            FileList list = request.Execute();

            if (list.Files.Count > 0)
              return true;
            else
              return false;
          }
          catch(Exception e)
          {
            Utils.LogMessageToDMs($"{e.Message}\n{e.StackTrace}");
            return false;
          }
        }
        private void RenameTarget(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || !Players.TryGetValue(selection.Player.LoginCreature, out Player player) || selection.TargetObject == null)
            return;

          selectionTarget = selection.TargetObject;

          if (player.windows.ContainsKey("playerInput"))
            ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Quel nom ?", DmRename, selection.TargetObject.Name);
          else
            player.windows.Add("playerInput", new PlayerInputWindow(player, "Quel nom ?", DmRename, selection.TargetObject.Name));
        }
        private bool DmRename(string inputValue)
        {
          if (selectionTarget == null)
            return false;

          player.oid.SendServerMessage($"{selectionTarget.Name.ColorString(ColorConstants.White)} a été renommé {inputValue.ColorString(ColorConstants.White)}.", ColorConstants.Green);
          selectionTarget.Name = inputValue;

          return true;
        }
        private void SelectLearnTarget(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || !selection.TargetObject.IsPlayerControlled(out NwPlayer oPC) || !Players.TryGetValue(oPC.LoginCreature, out Player targetPlayer))
            return;

          Learnable learnable = targetPlayer.GetActiveLearnable();

          if (learnable != null)
          {
            LogUtils.LogMessage($"{selection.Player.PlayerName} utilise apprentissage instantanné sur {oPC.LoginCreature.Name} ({learnable.name} - {learnable.currentLevel})", LogUtils.LogType.DMAction);
            learnable.acquiredPoints = learnable.pointsToNextLevel;
          }
          else
            selection.Player.SendServerMessage($"{targetPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} ne dispose pas d'apprentissage en cours.", ColorConstants.Orange);
        }

        private void SelectCraftTarget(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || !selection.TargetObject.IsPlayerControlled(out NwPlayer oPC) || !Players.TryGetValue(oPC.LoginCreature, out Player targetPlayer))
            return;

          if (targetPlayer.craftJob != null)
          {
            targetPlayer.craftJob.remainingTime = 1;
            LogUtils.LogMessage($"{selection.Player.PlayerName} utilise craft instantanné sur {oPC.LoginCreature.Name} ({targetPlayer.craftJob.type})", LogUtils.LogType.DMAction);
          }
        }
        private void SelectGiveResourcesTarget(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || !selection.TargetObject.IsPlayerControlled(out NwPlayer oPC) || !Players.TryGetValue(oPC.LoginCreature, out Player targetPlayer))
            return;

          if (player.windows.ContainsKey("resourceDMGift"))
            ((ResourceDMGiftWindow)player.windows["resourceDMGift"]).CreateWindow(targetPlayer);
          else
            player.windows.Add("resourceDMGift", new ResourceDMGiftWindow(player, targetPlayer));
        }
        private void SelectGiveSkillbookTarget(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || !selection.TargetObject.IsPlayerControlled(out NwPlayer oPC) || !Players.TryGetValue(oPC.LoginCreature, out Player targetPlayer))
            return;

          if (player.windows.ContainsKey("skillbookDMGift"))
            ((SkillBookDMGiftWindow)player.windows["skillbookDMGift"]).CreateWindow(targetPlayer);
          else
            player.windows.Add("skillbookDMGift", new SkillBookDMGiftWindow(player, targetPlayer));
        }
      }
      private static async void FollowTarget(ModuleEvents.OnPlayerTarget selection)
      {
        if (selection.IsCancelled || selection.TargetObject is not NwCreature target)
          return;

        if (selection.Player.ControlledCreature.MovementRate == MovementRate.Immobile
            || Encumbrance2da.IsCreatureHeavilyEncumbred(selection.Player.ControlledCreature))
        {
          selection.Player.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", ColorConstants.Red);
          return;
        }

        if (selection.Player.ControlledCreature.Area != target.Area || selection.Player.ControlledCreature.DistanceSquared(target) > 2000)
        {
          selection.Player.SendServerMessage($"{target.Name.ColorString(ColorConstants.White)} est trop éloigné pour que vous puissiez le suivre.", ColorConstants.Red);
          return;
        }

        await selection.Player.ControlledCreature.ActionForceFollowObject(target, 3.0f);
      }
    }
  }
}
