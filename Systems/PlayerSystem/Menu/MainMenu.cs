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
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren;
        private NuiRect windowRectangle { get; set; }

        public MainMenuWindow(Player player) : base(player)
        {
          windowId = "mainMenu";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          CreateWindow();
        }
        public void CreateWindow()
        {
          RefreshPlayerMenu();

          window = new NuiWindow(rootGroup, "Menu principal")
          {
            Geometry = geometry,
            Resizable = true,
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

          player.openedWindows[windowId] = token;
        }
        private async void HandleMainMenuEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
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

                  RefreshPlayerMenu();
                  rootGroup.SetLayout(player.oid, token, rootColumn);

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

                  Log.Info($"always walk : {player.oid.ControlledCreature.AlwaysWalk}");

                  RefreshPlayerMenu();
                  rootGroup.SetLayout(player.oid, token, rootColumn);

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

                case "contrat":

                  /*if (player.windows.ContainsKey("contrat"))
                    ((ChatColorsWindow)player.windows["contrat"]).CreateWindow();
                  else
                    player.windows.Add("contrat", new ChatColorsWindow(player));

                  CloseWindow();*/

                  break;

                case "dm":
                  RefreshDMMenu();
                  rootGroup.SetLayout(player.oid, token, rootColumn);
                  break;

                case "pj":
                  RefreshPlayerMenu();
                  rootGroup.SetLayout(player.oid, token, rootColumn);
                  break;
              }
              break;
          }
        }
        private void RefreshPlayerMenu()
        {
          windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.4f);

          rootChidren.Clear();

          if(player.oid.IsDM || player.oid.PlayerName == "Chim")
            rootChidren.Add(new NuiRow()
            {
              Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Afficher menu DM")
              { Id = "dm", Width = windowRectangle.Width - 60, Height = 35 } ,
              new NuiSpacer(),
          }
            });

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton(player.oid.ControlledCreature.ActiveEffects.Any(e => e.EffectType == EffectType.CutsceneGhost) ? "Désactiver Mode Toucher" : "Activer Mode Toucher")
              { Id = "touch", Tooltip = "Permet d'éviter les collisions entre personnages (non utilisable en combat)", Width = windowRectangle.Width - 60, Height = 35 } ,
              new NuiSpacer(),
          }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton(!player.oid.ControlledCreature.AlwaysWalk ? "Activer Mode Marche" : "Désactiver Mode Marche")
              { Id = "walk", Tooltip = "Permet d'avoir l'air moins ridicule en ville.", Width = windowRectangle.Width - 60, Height = 35 },
              new NuiSpacer(),
          }});

          if (AreaDescriptionExists(player.oid.ControlledCreature.Area.Name))
            rootChidren.Add(new NuiRow()
            {
              Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("Examiner les environs")
                { Id = "examineArea", Tooltip = "Obtenir une description de la zone.", Width = windowRectangle.Width - 60, Height = 35 },
                new NuiSpacer(),
              }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Gestion des grimoires")
              { Id = "grimoire", Tooltip = "Enregistrer ou charger un grimoire.", Width = windowRectangle.Width - 60, Height = 35 },
              new NuiSpacer(),
            }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Gestion des barres de raccourcis")
              { Id = "quickbars", Tooltip = "Enregistrer ou charger une barre de raccourcis.", Width = windowRectangle.Width - 60, Height = 35 },
              new NuiSpacer(),
            }});

          if(player.bonusRolePlay > 3) // TODO : Il faudra intégrer cette action dans la fenêtre d'examen des joueurs plutôt
            rootChidren.Add(new NuiRow()
            {
              Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("Recommander un joueur")
                { Id = "commend", Tooltip = "Recommander un joueur pour la qualité de son roleplay et son implication sur le module.", Width = windowRectangle.Width - 60, Height = 35 },
                new NuiSpacer(),
              }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Gestion des apparences d'objets")
              { Id = "itemAppearance", Tooltip = "Enregistrer ou charger une apparence d'objet.", Width = windowRectangle.Width - 60, Height = 35 },
              new NuiSpacer(),
            }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Gestion des descriptions")
              { Id = "description", Tooltip = "Enregistrer ou charger une description de personnage.", Width = windowRectangle.Width - 60, Height = 35 },
              new NuiSpacer(),
            }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Gestion des contrats d'échange de ressources.")
              { Id = "contrat", Tooltip = "Permet de créer un contrat d'échange pré-validé par le Juge du Changement.", Width = windowRectangle.Width - 60, Height = 35 },
              new NuiSpacer(),
            }
          });

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Gestion des couleurs du chat")
              { Id = "chat", Tooltip = "Personnaliser les couleurs du chat.", Width = windowRectangle.Width - 60, Height = 35 },
              new NuiSpacer(),
            }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Déblocage du décor")
              { Id = "unstuck", Tooltip = "Tentative de déblocage du décor (succès non garanti).", Width = windowRectangle.Width - 60, Height = 35 },
              new NuiSpacer(),
            }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Réinitialiser la position affichée")
              { Id = "reinitPositionDisplay", Tooltip = "Réinitialise la position affichée du personnage (à utiliser en cas de problème avec le système d'assise).", Width = windowRectangle.Width - 60, Height = 35 },
              new NuiSpacer(),
            }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Afficher ma clé publique")
              { Id = "publicKey", Tooltip = "Permet d'obtenir la clé publique de votre compte, utile pour lier le compte Discord au compte Never.", Width = windowRectangle.Width - 60, Height = 35 },
            new NuiSpacer(),
            }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Supprimer ce personnage")
              { Id = "delete", Tooltip = "Attention, la suppression est définitive.", Width = windowRectangle.Width - 60, Height = 35 },
              new NuiSpacer(),
            }});
        }
        private void RefreshDMMenu()
        {
          windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.4f);

          rootChidren.Clear();

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Afficher menu PJ")
              { Id = "pj", Width = windowRectangle.Width - 60, Height = 35 } ,
              new NuiSpacer(),
            }
          });

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton(player.oid.ControlledCreature.ActiveEffects.Any(e => e.EffectType == EffectType.CutsceneGhost) ? "Désactiver Mode Toucher" : "Activer Mode Toucher")
              { Id = "touch", Tooltip = "Permet d'éviter les collisions entre personnages (non utilisable en combat)", Width = windowRectangle.Width - 60, Height = 35 } ,
              new NuiSpacer(),
            }
          });
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
      }
    }
  }
}
