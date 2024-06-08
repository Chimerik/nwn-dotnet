using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using Newtonsoft.Json;

using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class DMVisualEffectsWindow : PlayerWindow
      {
        private readonly NuiColumn rootRow = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<string> vfxNames = new ("vfxNames");
        private readonly NuiBind<string> vfxIds = new ("vfxIds");
        private readonly NuiBind<string> vfxDurations = new ("vfxDurations");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly NuiBind<string> search = new ("search");
        private readonly NuiBind<string> vfxId = new ("vfxId");
        private readonly NuiBind<string> newVFXName = new ("newVFXName");
        private readonly NuiBind<string> newVFXDuration = new ("newVFXDuration");
        private readonly NuiBind<string> x = new("x");
        private readonly NuiBind<string> y = new("y");
        private readonly NuiBind<string> z = new("z");
        private readonly NuiBind<string> tx = new("tx");
        private readonly NuiBind<string> ty = new("ty");
        private readonly NuiBind<string> tz = new("tz");
        private readonly NuiBind<string> scale = new("scale");

        public List<CustomDMVisualEffect> currentList;
        private int selectedVFXId;
        private int selectedVFXDuration;

        public DMVisualEffectsWindow(Player player) : base(player)
        {
          windowId = "DMVisualEffects";

          rowTemplate.Add(new NuiListTemplateCell(new NuiButton(vfxNames) { Id = "useVfx", Tooltip = vfxIds, Height = 35 }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("", vfxDurations, 4, false) { Tooltip = "Durée de l'effet en secondes", Height = 35 }) { Width = 70 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton("Modifier") { Id = "modify", Tooltip = "Enregistrer la durée saisie", Height = 35 }) { Width = 65 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("menu_exit") { Id = "delete", Tooltip = "Supprimer", Height = 35 }) { Width = 35 });

          rootRow.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
          { 
            new NuiTextEdit("Id", vfxId, 5, false) { Width = 70, Height = 35, Tooltip = "Identifiant de l'effet (visualeffects.2da)" },
            new NuiTextEdit("Nom", newVFXName, 20, false) { Width = 100, Height = 35, Tooltip = "Nom sous lequel l'effet sera sauvegardé" },
            new NuiTextEdit("Durée", newVFXDuration, 4, false) { Width = 70, Height = 35, Tooltip = "Durée de l'effet en secondes" },
            new NuiButton("Tester") { Id = "test", Width = 60, Height = 35, Tooltip = "Essayer l'effet sur un objet ou une zone ciblée" },
            new NuiButton("Créer") { Id = "save", Width = 60, Height = 35, Tooltip = "Sauvegarde cet effet dans votre liste" }
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
          { 
            new NuiTextEdit("Rotation X", x, 3, false) { Width = 70, Height = 35, Tooltip = "Rotation X" },
            new NuiTextEdit("Rotation Y", y, 3, false) { Width = 70, Height = 35, Tooltip = "Rotation Y" },
            new NuiTextEdit("Rotation Z", z, 3, false) { Width = 70, Height = 35, Tooltip = "Rotation Z" },
          } });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
          {
            new NuiTextEdit("Translation X", tx, 3, false) { Width = 70, Height = 35, Tooltip = "Translation X" },
            new NuiTextEdit("Translation Y", ty, 3, false) { Width = 70, Height = 35, Tooltip = "Translation Y" },
            new NuiTextEdit("Translation Z", tz, 3, false) { Width = 70, Height = 35, Tooltip = "Translation Z" },
          }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
          { 
            new NuiTextEdit("Taille", scale, 5, false) { Width = 70, Height = 35, Tooltip = "Taille de l'effet" },
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 370 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 400, 650);

          window = new NuiWindow(rootRow, "Gestion des effets visuels")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleVisualEffectsEvents;

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);

            vfxId.SetBindValue(player.oid, nuiToken.Token, "");
            newVFXName.SetBindValue(player.oid, nuiToken.Token, "");
            newVFXDuration.SetBindValue(player.oid, nuiToken.Token, "");

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            currentList = player.customDMVisualEffects;
            LoadVisualEffectList(currentList);
          }            
        }

        private void HandleVisualEffectsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "test":

                  if(!int.TryParse(vfxId.GetBindValue(player.oid, nuiToken.Token), out selectedVFXId) || selectedVFXId < 0)
                  {
                    player.oid.SendServerMessage("Veuillez saisir un identifiant d'effet visuel existant.", ColorConstants.Red);
                    return;
                  }

                  if (!int.TryParse(newVFXDuration.GetBindValue(player.oid, nuiToken.Token), out selectedVFXDuration) || selectedVFXDuration < 0)
                    selectedVFXDuration = 10;

                  player.oid.SendServerMessage("Veuillez sélectionner une cible pour essayer votre effet visuel.", ColorConstants.Orange);
                  player.oid.EnterTargetMode(OnTargetSelected, Config.selectLocationTargetMode);

                  break;

                case "save":

                  if (!int.TryParse(vfxId.GetBindValue(player.oid, nuiToken.Token), out selectedVFXId) || selectedVFXId < 0)
                  {
                    player.oid.SendServerMessage("Veuillez saisir un identifiant d'effet visuel existant.", ColorConstants.Red);
                    return;
                  }

                  string newName = newVFXName.GetBindValue(player.oid, nuiToken.Token);

                  if (string.IsNullOrEmpty(newName))
                  {
                    player.oid.SendServerMessage("Veuillez saisir un nom afin d'enregistrer cet effect visuel.", ColorConstants.Red);
                    return;
                  }

                  if (!int.TryParse(newVFXDuration.GetBindValue(player.oid, nuiToken.Token), out selectedVFXDuration) || selectedVFXDuration < 0)
                    selectedVFXDuration = 10;

                  player.customDMVisualEffects.Add(new CustomDMVisualEffect(selectedVFXId, newName, selectedVFXDuration));

                  LoadVisualEffectList(currentList);
                  SaveVFXToDatabase();

                  break;

                case "modify":

                  if (int.TryParse(newVFXDuration.GetBindValue(player.oid, nuiToken.Token), out selectedVFXDuration) && selectedVFXDuration > 0)
                  {
                    CustomDMVisualEffect vfx = currentList[nuiEvent.ArrayIndex];
                    vfx.duration = selectedVFXDuration;
                    player.oid.SendServerMessage($"La durée de {vfx.name} est désormais de {vfx.duration} seconde(s)", ColorConstants.Orange);
                    SaveVFXToDatabase();
                  }
                  else
                    player.oid.SendServerMessage("Veuillez saisir une durée valide");

                  break;

                case "delete":

                  player.customDMVisualEffects.Remove(currentList[nuiEvent.ArrayIndex]);
                  LoadVisualEffectList(currentList);
                  SaveVFXToDatabase();

                  break;

                case "useVfx":

                  selectedVFXId = currentList[nuiEvent.ArrayIndex].id;

                  player.oid.SendServerMessage("Veuillez sélectionner la cible de votre effet visuel.", ColorConstants.Orange);
                  player.oid.EnterTargetMode(OnTargetSelected, Config.selectLocationTargetMode);

                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":

                  string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  currentList = string.IsNullOrEmpty(currentSearch) ? player.customDMVisualEffects : player.customDMVisualEffects.Where(v => v.name.ToLower().Contains(currentSearch)).ToList();
                  LoadVisualEffectList(currentList);

                  break;
              }

              break;
          }
        }
        private void LoadVisualEffectList(List<CustomDMVisualEffect> vfxList)
        {
          List<string> vfxNameList = new List<string>();
          List<string> vfxDurationList = new List<string>();
          List<string> vfxIdsList = new List<string>();

          foreach (CustomDMVisualEffect vfx in vfxList)
          {
            vfxNameList.Add(vfx.name);
            vfxDurationList.Add(vfx.duration.ToString());
            vfxIdsList.Add($"Utiliser cet effet (id :{vfx.id})");
          }

          vfxNames.SetBindValues(player.oid, nuiToken.Token, vfxNameList);
          vfxDurations.SetBindValues(player.oid, nuiToken.Token, vfxDurationList);
          vfxIds.SetBindValues(player.oid, nuiToken.Token, vfxIdsList);
          listCount.SetBindValue(player.oid, nuiToken.Token, vfxList.Count);
        }
        private void OnTargetSelected(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled)
            return;

          var vfxRow = NwGameTables.VisualEffectTable.GetRow(selectedVFXId);

          if(vfxRow == null)
          {
            player.oid.SendServerMessage($"Aucun effet visuel ne correspond à l'entrée : {selectedVFXId.ToString().ColorString(ColorConstants.White)}", ColorConstants.Red);
            return;
          }

          float rx = float.TryParse(x.GetBindValue(player.oid, nuiToken.Token), out rx) ? rx : 0;
          float ry = float.TryParse(y.GetBindValue(player.oid, nuiToken.Token), out ry) ? ry : 0;
          float rz = float.TryParse(z.GetBindValue(player.oid, nuiToken.Token), out rz) ? rz : 0;
          float vfxtx = float.TryParse(tx.GetBindValue(player.oid, nuiToken.Token), out vfxtx) ? vfxtx : 0;
          float vfxty = float.TryParse(ty.GetBindValue(player.oid, nuiToken.Token), out vfxty) ? vfxty : 0;
          float vfxtz = float.TryParse(tz.GetBindValue(player.oid, nuiToken.Token), out vfxtz) ? vfxtz : 0;
          float vfxScale = float.TryParse(scale.GetBindValue(player.oid, nuiToken.Token), out vfxScale) ? vfxScale : 1;

          EffectDuration durationType = EffectDuration.Temporary;
          TimeSpan duration = TimeSpan.FromSeconds(selectedVFXDuration);
          
          Effect vfx = Effect.VisualEffect((VfxType)selectedVFXId, false, vfxScale, new Vector3(vfxtx, vfxty, vfxtz), new Vector3(rx, ry, rz));

          if (vfxRow.TypeFd == "F")
          {
            durationType = EffectDuration.Instant;
            duration = default;
          }

          if (vfxRow.TypeFd == "B")
            vfx = Effect.Beam((VfxType)selectedVFXId, player.oid.ControlledCreature, BodyNode.Hand);

          if (selection.TargetObject is NwGameObject target)
            target.ApplyEffect(durationType, vfx, duration);
          else
            Location.Create(selection.Player.ControlledCreature.Location.Area, selection.TargetPosition, selection.Player.ControlledCreature.Rotation)
              .ApplyEffect(durationType, vfx, duration);
        }
        private async void SaveVFXToDatabase()
        {
          Task<string> serializeVFX = Task.Run(() => JsonConvert.SerializeObject(player.customDMVisualEffects));
          await serializeVFX;

          SqLiteUtils.UpdateQuery("PlayerAccounts",
            new List<string[]>() { new string[] { "customDMVisualEffects", serializeVFX.Result } },
            new List<string[]>() { new string[] { "rowid", player.accountId.ToString() } });
        }
      }
    }
  }
}
