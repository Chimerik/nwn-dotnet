using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

using Google.Apis.Drive.v3.Data;

using NWN.Core;

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

        private readonly Dictionary<string, Utils.MainMenuCommand> myCommandList;
        private Dictionary<string, Utils.MainMenuCommand> currentList;

        public MainMenuWindow(Player player) : base(player)
        {
          windowId = "mainMenu";

          rootColumn.Children = rootChildren;
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton(buttonName) { Id = "command", Tooltip = buttonTooltip, Height = 35 }) { VariableSize = true });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 370 } } });
          rootChildren.Add(new NuiRow() { Height = 385, Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

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

          if (player.craftJob == null)
            myCommandList.Remove("currentJob");

          if(!player.subscriptions.Any(s => s.type == Utils.SubscriptionType.MailDistantAccess))
            myCommandList.Remove("mailBox");

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, 500);

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
            
            if (!AreaDescriptionExists(player.oid))
              myCommandList.Remove("examineArea");
            else
              myCommandList.TryAdd("examineArea", Utils.mainMenuCommands["examineArea"]);

            currentList = myCommandList;
            LoadMenu(currentList);
          }
        }
        private async void HandleMainMenuEvents(ModuleEvents.OnNuiEvent nuiEvent)
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
                      foreach (var eff in player.oid.ControlledCreature.ActiveEffects.Where(e => e.EffectType == EffectType.CutsceneGhost))
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

                    if (!player.windows.ContainsKey("sitAnywhere")) player.windows.Add("sitAnywhere", new SitAnywhereWindow(player));
                    else ((SitAnywhereWindow)player.windows["sitAnywhere"]).CreateWindow();

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
                    await player.oid.Delete($"Le personnage {player.oid.LoginCreature.Name} a été supprimé.");
                    break;

                  case "chat":

                    if (!player.windows.ContainsKey("chatColors")) player.windows.Add("chatColors", new ChatColorsWindow(player));
                    else ((ChatColorsWindow)player.windows["chatColors"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "wind":

                    if (!player.windows.ContainsKey("areaWindSettings")) player.windows.Add("areaWindSettings", new AreaWindSettings(player));
                    else ((AreaWindSettings)player.windows["areaWindSettings"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "dm":
                    player.oid.IsPlayerDM = !player.oid.IsPlayerDM;
                    break;

                  case "dmRename":
                    player.oid.SendServerMessage("Veuillez sélectionner la cible à renommer");
                    player.oid.EnterTargetMode(RenameTarget, ObjectTypes.All, MouseCursor.CreateDown);
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
                    player.oid.SendServerMessage("Veuillez sélectionner la cible de l'apprentissage instantanné.");
                    player.oid.EnterTargetMode(SelectLearnTarget, ObjectTypes.Creature, MouseCursor.CreateDown);
                    CloseWindow();
                    break;

                  case "instantCraft":
                    player.oid.SendServerMessage("Veuillez sélectionner la cible du craft instantanné.");
                    player.oid.EnterTargetMode(SelectCraftTarget, ObjectTypes.Creature, MouseCursor.CreateDown);
                    CloseWindow();
                    break;

                  case "giveResources":
                    player.oid.SendServerMessage("Veuillez sélectionner la cible du don.");
                    player.oid.EnterTargetMode(SelectGiveResourcesTarget, ObjectTypes.Creature, MouseCursor.CreateDown);
                    CloseWindow();
                    break;

                  case "giveSkillbook":
                    player.oid.SendServerMessage("Veuillez sélectionner la cible du don.");
                    player.oid.EnterTargetMode(SelectGiveSkillbookTarget, ObjectTypes.Creature, MouseCursor.CreateDown);
                    CloseWindow();
                    break;

                  case "visualEffects":

                    if (!player.windows.ContainsKey("DMVisualEffects")) player.windows.Add("DMVisualEffects", new DMVisualEffectsWindow(player));
                    else ((DMVisualEffectsWindow)player.windows["DMVisualEffects"]).CreateWindow();

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

                    player.oid.EnterTargetMode(FollowTarget, ObjectTypes.Creature, MouseCursor.Follow);

                    CloseWindow();

                    break;

                  case "creaturePalette":

                    if (!player.windows.ContainsKey("paletteCreature")) player.windows.Add("paletteCreature", new PaletteCreatureWindow(player));
                    else ((PaletteCreatureWindow)player.windows["paletteCreature"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "itemPalette":

                    if (!player.windows.ContainsKey("paletteItem")) player.windows.Add("paletteItem", new PaletteItemWindow(player));
                    else ((PaletteItemWindow)player.windows["paletteItem"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "placeablePalette":

                    if (!player.windows.ContainsKey("palettePlaceable")) player.windows.Add("palettePlaceable", new PalettePlaceableWindow(player));
                    else ((PalettePlaceableWindow)player.windows["palettePlaceable"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "placeableManager":

                    if (!player.windows.ContainsKey("placeableManager")) player.windows.Add("placeableManager", new PlaceableManagerWindow(player));
                    else ((PlaceableManagerWindow)player.windows["placeableManager"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "learnables":

                    if (!player.windows.ContainsKey("learnables")) player.windows.Add("learnables", new LearnableWindow(player));
                    else ((LearnableWindow)player.windows["learnables"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "currentJob":

                    if (player.craftJob != null)
                    {
                      if (!player.windows.ContainsKey("activeCraftJob")) player.windows.Add("activeCraftJob", new ActiveCraftJobWindow(player));
                      else ((ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();

                      CloseWindow();
                    }

                    break;

                  case "language":

                    if (!player.windows.ContainsKey("languageSelection")) player.windows.Add("languageSelection", new LanguageSelectionWindow(player));
                    else ((LanguageSelectionWindow)player.windows["languageSelection"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "healthManaBars":

                    if (!player.windows.ContainsKey("healthBar")) player.windows.Add("healthBar", new HealthBarWindow(player));
                    else ((HealthBarWindow)player.windows["healthBar"]).CreateWindow();

                    if (!player.windows.ContainsKey("energyBar")) player.windows.Add("energyBar", new EnergyBarWindow(player));
                    else ((EnergyBarWindow)player.windows["energyBar"]).CreateWindow();

                    CloseWindow();

                    break;

                  case "areaMusicEditor":

                    if (!player.windows.ContainsKey("areaMusicEditor")) player.windows.Add("areaMusicEditor", new AreaMusicEditorWindow(player, player.oid.ControlledCreature.Area));
                    else ((AreaMusicEditorWindow)player.windows["areaMusicEditor"]).CreateWindow(player.oid.ControlledCreature.Area);

                    CloseWindow();

                    break;

                  case "areaLoadScreenEditor":

                    if (!player.windows.ContainsKey("areaLoadScreenEditor")) player.windows.Add("areaLoadScreenEditor", new AreaLoadScreenEditorWindow(player, player.oid.ControlledCreature.Area));
                    else ((AreaLoadScreenEditorWindow)player.windows["areaLoadScreenEditor"]).CreateWindow(player.oid.ControlledCreature.Area);

                    CloseWindow();

                    break;

                  case "cooldownPosition":

                    if (!player.windows.ContainsKey("cooldownPosition")) player.windows.Add("cooldownPosition", new CooldownPositionSetter(player));
                    else ((CooldownPositionSetter)player.windows["cooldownPosition"]).CreateWindow();

                    CloseWindow();

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
