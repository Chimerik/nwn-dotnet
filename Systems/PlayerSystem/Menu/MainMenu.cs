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
        private readonly NuiColumn rootColumn = new NuiColumn();
        private readonly List<NuiElement> rootChidren = new List<NuiElement>();
        private readonly List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>();
        private readonly NuiBind<string> buttonName = new NuiBind<string>("buttonName");
        private readonly List<string> buttonIds = new List<string>();
        private readonly NuiBind<string> buttonTooltip = new NuiBind<string>("buttonTooltip");
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");
        private NwObject selectionTarget; 

        public MainMenuWindow(Player player) : base(player)
        {
          windowId = "mainMenu";

          rootColumn.Children = rootChidren;
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton(buttonName) { Id = "command", Tooltip = buttonTooltip, Height = 35 }) { VariableSize = true });
          rootChidren.Add(new NuiRow() { Height = 450, Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, 500);

          window = new NuiWindow(rootColumn, "Menu principal")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleMainMenuEvents;
          player.oid.OnNuiEvent += HandleMainMenuEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          LoadPlayerMenu();

          player.openedWindows[windowId] = token;
        }
        private async void HandleMainMenuEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (buttonIds[nuiEvent.ArrayIndex])
              {
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

                  LoadPlayerMenu();

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

                  LoadPlayerMenu();

                  break;

                case "examineArea":

                  if (player.windows.ContainsKey("areaDescription"))
                    ((AreaDescriptionWindow)player.windows["areaDescription"]).CreateWindow(player.oid.ControlledCreature.Area);
                  else
                    player.windows.Add("areaDescription", new AreaDescriptionWindow(player, player.oid.ControlledCreature.Area));

                  break;

                case "grimoire":

                  if (player.windows.ContainsKey("grimoires"))
                    ((GrimoiresWindow)player.windows["grimoires"]).CreateWindow();
                  else
                    player.windows.Add("grimoires", new GrimoiresWindow(player));

                  CloseWindow();

                  break;

                case "quickbars":

                  if (player.windows.ContainsKey("quickbars"))
                    ((QuickbarsWindow)player.windows["quickbars"]).CreateWindow();
                  else
                    player.windows.Add("quickbars", new QuickbarsWindow(player));

                  CloseWindow();

                  break;

                case "commend":

                  player.oid.SendServerMessage("Veuillez sélectionner le joueur que vous souhaitez recommander.", ColorConstants.Orange);
                  player.oid.EnterTargetMode(OnTargetSelected, ObjectTypes.Creature, MouseCursor.Magic);
                  CloseWindow();

                  break;

                case "itemAppearance":

                  if (player.windows.ContainsKey("itemAppearances"))
                    ((ItemAppearancesWindow)player.windows["itemAppearances"]).CreateWindow();
                  else
                    player.windows.Add("itemAppearances", new ItemAppearancesWindow(player));

                  CloseWindow();

                  break;

                case "description":

                  if (player.windows.ContainsKey("description"))
                    ((DescriptionsWindow)player.windows["description"]).CreateWindow();
                  else
                    player.windows.Add("description", new DescriptionsWindow(player));

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

                  if (player.windows.ContainsKey("chatColors"))
                    ((ChatColorsWindow)player.windows["chatColors"]).CreateWindow();
                  else
                    player.windows.Add("chatColors", new ChatColorsWindow(player));

                  CloseWindow();

                  break;

                case "wind":

                  if (player.windows.ContainsKey("areaWindSettings"))
                    ((AreaWindSettings)player.windows["areaWindSettings"]).CreateWindow();
                  else
                    player.windows.Add("areaWindSettings", new AreaWindSettings(player));

                  CloseWindow();

                  break;

                case "dm":
                  LoadDMMenu();
                  break;

                case "pj":
                  LoadPlayerMenu();
                  break;

                case "goDM":
                  player.oid.IsPlayerDM = !player.oid.IsPlayerDM;
                  LoadPlayerMenu();
                  break;

                case "listenAll":

                  if (player.listened.Count > 0)
                  {
                    player.oid.SendServerMessage("Ecoute globale désactivée.", ColorConstants.Cyan);
                    player.listened.Clear();
                  }
                  else
                  {
                    foreach (NwPlayer oPC in NwModule.Instance.Players.Where(p => !p.IsDM))
                      player.listened.Add(oPC);

                    player.oid.SendServerMessage("Ecoute globale activée.", ColorConstants.Cyan);
                  }

                  LoadDMMenu();
                  break;

                case "listen":
                  player.oid.SendServerMessage("Veuillez sélectionnner le joueur à écouter.", ColorConstants.Pink);
                  player.oid.EnterTargetMode(OnListenTargetSelected, ObjectTypes.Creature, MouseCursor.Magic);
                  CloseWindow();
                  break; 

                case "dmRename":
                  player.oid.SendServerMessage("Veuillez sélectionner la cible à renommer");
                  player.oid.EnterTargetMode(RenameTarget, ObjectTypes.All, MouseCursor.CreateDown);
                  CloseWindow();
                  break;

                case "persistentPlaceables":

                  if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_PERSIST").HasValue)
                  {
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_PERSIST").Delete();
                    player.oid.SendServerMessage("Persistance des placeables créés par DM désactivée.", ColorConstants.Blue);
                  }
                  else
                  {
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_PERSIST").Value = 1;
                    player.oid.SendServerMessage("Persistance des placeables créés par DM activée.", ColorConstants.Blue);
                  }

                  LoadDMMenu();

                  break;

                case "reboot":

                  NwServer.Instance.PlayerPassword = "REBOOTINPROGRESS";

                  CloseWindow();                    

                  foreach (NwPlayer connectingPlayer in NwModule.Instance.Players.Where(p => p.LoginCreature == null))
                    connectingPlayer.BootPlayer("Navré, le module est en cours de redémarrage. Vous pourrez vous reconnecter dans une minute.");

                  foreach (Player connectedPlayer in Players.Values.Where(p => p.pcState != PcState.Offline))
                    player.windows.Add("rebootCountdown", new RebootCountdownWindow(player));

                  var scheduler = ModuleSystem.scheduler.Schedule(() =>
                  {
                    SqLiteUtils.UpdateQuery("moduleInfo",
                      new List<string[]>() { new string[] { "year", NwDateTime.Now.Year.ToString() }, new string[] { "month", NwDateTime.Now.Month.ToString() }, new string[] { "day", NwDateTime.Now.DayInTenday.ToString() }, new string[] { "hour", NwDateTime.Now.Hour.ToString() }, new string[] { "minute", NwDateTime.Now.Minute.ToString() }, new string[] { "second", NwDateTime.Now.Second.ToString() } },
                      new List<string[]>() { new string[] { "rowid", "1" } });
                  }, TimeSpan.FromSeconds(31));

                  var schedulerReboot = ModuleSystem.scheduler.Schedule(() => { NwServer.Instance.ShutdownServer(); }, TimeSpan.FromSeconds(35));

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

                  if (player.windows.ContainsKey("DMVisualEffects"))
                    ((DMVisualEffectsWindow)player.windows["DMVisualEffects"]).CreateWindow();
                  else
                    player.windows.Add("DMVisualEffects", new DMVisualEffectsWindow(player));

                  CloseWindow();
                  break;
              }
              break;
          }
        }
        private void LoadPlayerMenu()
        {
          List<string> buttonNameList = new List<string>();
          List<string> tooltipList = new List<string>();
          buttonIds.Clear();

          if (player.oid.IsDM || player.oid.PlayerName == "Chim")
          {
            buttonNameList.Add("Afficher menu DM");
            tooltipList.Add("");
            buttonIds.Add("dm");

            buttonNameList.Add(player.oid.IsPlayerDM ? "Désactiver mode DM" : "Activer mode DM");
            tooltipList.Add("");
            buttonIds.Add("goDM");
          }

          buttonNameList.Add(player.oid.ControlledCreature.ActiveEffects.Any(e => e.EffectType == EffectType.CutsceneGhost) ? "Désactiver Mode Toucher" : "Activer Mode Toucher");
          tooltipList.Add("Permet d'éviter les collisions entre personnages (non utilisable en combat)");
          buttonIds.Add("touch");

          buttonNameList.Add(!player.oid.ControlledCreature.AlwaysWalk ? "Activer Mode Marche" : "Désactiver Mode Marche");
          tooltipList.Add("Permet d'avoir l'air moins ridicule en ville.");
          buttonIds.Add("walk");

          if (AreaDescriptionExists(player.oid.ControlledCreature.Area.Name))
          {
            buttonNameList.Add("Examiner les environs");
            tooltipList.Add("Obtenir une description de la zone.");
            buttonIds.Add("examineArea");
          }

          buttonNameList.Add("Gestion des grimoires");
          tooltipList.Add("Enregistrer ou charger un grimoire de sorts.");
          buttonIds.Add("grimoire");

          buttonNameList.Add("Gestion des barres de raccourcis");
          tooltipList.Add("Enregistrer ou charger une barre de raccourcis.");
          buttonIds.Add("quickbars");

          if (player.bonusRolePlay > 3) // TODO : Il faudra intégrer cette action dans la fenêtre d'examen des joueurs plutôt
          {
            buttonNameList.Add("Recommander un joueur");
            tooltipList.Add("Recommander un joueur pour la qualité de son roleplay et son implication sur le module.");
            buttonIds.Add("commend");
          }

          buttonNameList.Add("Gestion des apparences d'objets");
          tooltipList.Add("Enregistrer ou charger une apparence d'objet.");
          buttonIds.Add("itemAppearance");

          buttonNameList.Add("Gestion des descriptions");
          tooltipList.Add("Enregistrer ou charger une description de personnage.");
          buttonIds.Add("description");

          buttonNameList.Add("Gestion des couleurs du chat");
          tooltipList.Add("Personnaliser les couleurs du chat.");
          buttonIds.Add("chat");

          buttonNameList.Add("Déblocage du décor");
          tooltipList.Add("Tentative de déblocage du décor (succès non garanti).");
          buttonIds.Add("unstuck");

          buttonNameList.Add("Réinitialiser la position affichée");
          tooltipList.Add("Réinitialise la position affichée du personnage (à utiliser en cas de problème avec le système d'assise).");
          buttonIds.Add("reinitPositionDisplay");

          buttonNameList.Add("Afficher ma clé publique");
          tooltipList.Add("Permet d'obtenir la clé publique de votre compte, utile pour lier le compte Discord au compte Never.");
          buttonIds.Add("publicKey");

          buttonNameList.Add("Supprimer ce personnage");
          tooltipList.Add("Attention, la suppression est définitive.");
          buttonIds.Add("delete");

          buttonName.SetBindValues(player.oid, token, buttonNameList);
          buttonTooltip.SetBindValues(player.oid, token, tooltipList);
          listCount.SetBindValue(player.oid, token, buttonNameList.Count());
        }
        private void LoadDMMenu()
        {
          List<string> buttonNameList = new List<string>();
          List<string> tooltipList = new List<string>();
          buttonIds.Clear();

          buttonNameList.Add("Afficher menu PJ");
          tooltipList.Add("");
          buttonIds.Add("pj");

          buttonNameList.Add("Gestion du vent");
          tooltipList.Add("Permet de modifier la configuration du vent de cette zone");
          buttonIds.Add("wind");

          buttonNameList.Add(player.listened.Count > 0 ? "Désactiver l'écoute globale" : "Activer l'écoute globale");
          tooltipList.Add("Permet d'écouter tous les joueurs, où qu'ils fussent");
          buttonIds.Add("listenAll");

          buttonNameList.Add("Ajouter/Retirer un joueur de la liste d'écoute"); // TODO : A intégrer au OnExamine des joueurs
          tooltipList.Add("Permet d'écouter le joueur sélectionné, où qu'il soit.");
          buttonIds.Add("listen");

          buttonNameList.Add("Changer le nom de la cible"); // TODO : A intégrer aux divers OnExamine custom pour DM
          tooltipList.Add("Permet de modifier le nom de n'importe quel objet.");
          buttonIds.Add("dmRename");

          buttonNameList.Add(player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_PERSIST").HasValue ? "Désactiver la persistance des placeables" : "Activer la persistance des placeables"); 
          tooltipList.Add("Permettre de rendre persistant les placeables créés, même après reboot");
          buttonIds.Add("persistentPlaceables");

          buttonNameList.Add("Gérer mes effets visuels");
          tooltipList.Add("Permet d'utiliser et de gérer les effets visuels personnalisés.");
          buttonIds.Add("visualEffects");

          if (player.oid.IsDM || player.oid.PlayerName == "Chim")
          {
            if(NwServer.Instance.PlayerPassword != "REBOOTINPROGRESS")
            {
              buttonNameList.Add("Reboot");
              tooltipList.Add("");
              buttonIds.Add("reboot");
            }

            buttonNameList.Add("Refill ressources");
            tooltipList.Add("");
            buttonIds.Add("refill");

            buttonNameList.Add("Instant Learn"); // TODO : Ajouter à OnExamine Player
            tooltipList.Add("");
            buttonIds.Add("instantLearn");

            buttonNameList.Add("Instant Craft"); // TODO : ajouter à OnExamine Player
            tooltipList.Add("");
            buttonIds.Add("instantCraft");

            buttonNameList.Add("Don de ressources"); // TODO : ajouter à OnExamine Player
            tooltipList.Add("");
            buttonIds.Add("giveResources");

            buttonNameList.Add("Don de skillbook"); // TODO : ajouter à OnExamine Player
            tooltipList.Add("");
            buttonIds.Add("giveSkillbook");
          }

          buttonName.SetBindValues(player.oid, token, buttonNameList);
          buttonTooltip.SetBindValues(player.oid, token, tooltipList);
          listCount.SetBindValue(player.oid, token, buttonNameList.Count());
        }
        private bool AreaDescriptionExists(string areaName)
        {
          var request = ModuleSystem.googleDriveService.Files.List();
          request.Q = $"name = '{areaName}'";
          FileList list = request.Execute();

          if (list.Files.Count > 0)
            return true;
          else
            return false;
        }
        private void OnTargetSelected(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject.IsPlayerControlled(out NwPlayer oPC) || oPC == null || !PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out Player commendTarget))
            return;

          if (commendTarget.bonusRolePlay < 4)
          {
            commendTarget.oid.SendServerMessage("Vous venez d'obtenir une recommandation pour une augmentation de bonus roleplay !", ColorConstants.Rose);

            if (commendTarget.bonusRolePlay == 1)
            {
              commendTarget.bonusRolePlay = 2;
              commendTarget.oid.SendServerMessage("Votre bonus roleplay est désormais de 2", new Color(32, 255, 32));

              SqLiteUtils.UpdateQuery("PlayerAccounts",
              new List<string[]>() { new string[] { "bonusRolePlay", commendTarget.bonusRolePlay.ToString() } },
              new List<string[]>() { new string[] { "rowid", commendTarget.accountId.ToString() } });
            }

            Utils.LogMessageToDMs($"{selection.Player.LoginCreature.Name} vient de recommander {oPC.LoginCreature.Name} pour une augmentation de bonus roleplay.");
          }

          commendTarget.oid.SendServerMessage($"Vous venez de recommander {oPC.LoginCreature.Name.ColorString(ColorConstants.White)} pour une augmentation de bonus roleplay !", ColorConstants.Rose);
        }
        private void OnListenTargetSelected(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || !Players.TryGetValue(selection.Player.LoginCreature, out Player player))
            return;

          if (!(selection.TargetObject is NwCreature oPC) || oPC.ControllingPlayer.IsDM)
          {
            selection.Player.SendServerMessage("La cible de l'écoute doit être un joueur.", ColorConstants.Orange);
            return;
          }

          if (player.listened.Contains(oPC.ControllingPlayer))
          {
            player.listened.Remove(oPC.ControllingPlayer);
            selection.Player.SendServerMessage($"{oPC.ControllingPlayer.PlayerName.ColorString(ColorConstants.White)} vient d'être retiré de votre liste d'écoute.", ColorConstants.Rose);
          }
          else
          {
            player.listened.Add(oPC.ControllingPlayer);
            selection.Player.SendServerMessage($"{oPC.ControllingPlayer.PlayerName.ColorString(ColorConstants.White)} vient d'être ajouté à votre liste d'écoute.", ColorConstants.Rose);
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
            learnable.acquiredPoints = learnable.GetPointsToNextLevel();
          else
            selection.Player.SendServerMessage($"{targetPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} ne dispose pas d'apprentissage en cours.", ColorConstants.Orange);
        }
        
        private void SelectCraftTarget(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || !selection.TargetObject.IsPlayerControlled(out NwPlayer oPC) || !Players.TryGetValue(oPC.LoginCreature, out Player targetPlayer))
            return;

          if (targetPlayer.craftJob != null)
            targetPlayer.craftJob.remainingTime = 1;
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
    }
  }
}
