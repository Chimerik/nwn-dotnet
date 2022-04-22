using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ItemAppearancesWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren;
        private readonly NuiBind<string> buttonText = new ("buttonText");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly NuiBind<string> appearanceName = new ("appearanceName");
        private readonly NuiBind<bool> saveAppearanceEnabled = new ("saveappearanceEnabled");
        private readonly List<string> appearanceNamesList = new List<string>();
        private int selectedIndex { get; set; }

        public ItemAppearancesWindow(Player player) : base(player)
        {
          windowId = "itemAppearances";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Nom d'une nouvelle apparence d'objet", appearanceName, 40, false) { Width = 300, Tooltip = "Afin d'enregistrer une nouvelle apparence d'objet, un nom doit être renseigné." },
              new NuiButton("Enregistrer") { Id = "new", Width = 80, Enabled = saveAppearanceEnabled, Tooltip = "Enregistre l'apparence de l'objet sélectionné, à condition d'en être l'artisan d'origine." }
            }
          });

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButton(buttonText) { Id = "load", Height = 35 }) { Width = 300 },
            new NuiListTemplateCell(new NuiButton("Supprimer") { Id = "delete", Height = 35 }) { Width = 60 },
          };

          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 35 });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.4f);
          selectedIndex = -1;

          window = new NuiWindow(rootGroup, "Apparences d'objets - Sélection")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleItemAppearancesEvents;
          player.oid.OnNuiEvent += HandleItemAppearancesEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          foreach (ItemAppearance appearance in player.itemAppearances)
            appearanceNamesList.Add(appearance.name);

          buttonText.SetBindValues(player.oid, token, appearanceNamesList);
          listCount.SetBindValue(player.oid, token, appearanceNamesList.Count);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          appearanceName.SetBindValue(player.oid, token, "");
          appearanceName.SetBindWatch(player.oid, token, true);

          saveAppearanceEnabled.SetBindValue(player.oid, token, false);

          player.openedWindows[windowId] = token;
        }
        private void HandleItemAppearancesEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "new":
                  player.oid.SendServerMessage("Veuillez sélectionner l'objet dont vous souhaitez sauvegarder l'apparence.", ColorConstants.Orange);
                  player.oid.EnterTargetMode(OnItemSelectedSaveAppearance, ObjectTypes.Item, MouseCursor.Create);
                  break;

                case "delete":

                  DeleteAppearance(nuiEvent.ArrayIndex);
                  player.oid.SendServerMessage("Apparence d'objet supprimée.", ColorConstants.Orange);

                  break;

                case "load":

                  player.oid.SendServerMessage("Veuillez sélectionner l'objet sur lequel vous souhaitez appliquer l'apparence sauvegardée.", ColorConstants.Orange);
                  selectedIndex = nuiEvent.ArrayIndex;
                  player.oid.EnterTargetMode(OnItemSelectedLoadAppearance, ObjectTypes.Item, MouseCursor.Create);

                  break;

              }
              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "quickbarName":

                  if (appearanceName.GetBindValue(player.oid, token).Length > 0)
                    saveAppearanceEnabled.SetBindValue(player.oid, token, true);
                  else
                    saveAppearanceEnabled.SetBindValue(player.oid, token, false);

                  break;

              }

              break;
          }
        }
        private void DeleteAppearance(int index)
        {
          player.itemAppearances.RemoveAt(index);
          appearanceNamesList.RemoveAt(index);
          buttonText.SetBindValues(player.oid, token, appearanceNamesList);
          listCount.SetBindValue(player.oid, token, appearanceNamesList.Count);
        }

        private void OnItemSelectedSaveAppearance(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject is null || !(selection.TargetObject is NwItem item) || item.Possessor != player.oid.ControlledCreature)
            return;

          string name = appearanceName.GetBindValue(player.oid, token);

          if (!player.openedWindows.ContainsKey("itemAppearances") || name.Length < 1)
          {
            player.oid.SendServerMessage("Le menu de gestion des apparences doit être ouvert et le nom de l'apparence ne doit pas être vide.", ColorConstants.Red);
            return;
          }

          // TODO : ajouter un métier permettant de modifier n'importe quelle tenue
          if (item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").HasValue && item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value != player.oid.LoginCreature.Name)
          {
            player.oid.SendServerMessage($"Impossible de sauvegarder cette apparence. Il est indiqué : Pour tout modification, s'adresser à {item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value.ColorString(ColorConstants.White)}", ColorConstants.Orange);
            return;
          }

          player.itemAppearances.Add(new ItemAppearance(name, item.Serialize().ToBase64EncodedString(), (int)item.BaseItem.ItemType, item.BaseItem.ItemType == BaseItemType.Armor ? item.BaseACValue : -1));

          appearanceNamesList.Add(name);
          buttonText.SetBindValues(player.oid, token, appearanceNamesList);
          listCount.SetBindValue(player.oid, token, appearanceNamesList.Count);

          player.oid.SendServerMessage($"L'apparence de votre {selection.TargetObject.Name.ColorString(ColorConstants.White)} a été sauvegardée sous le nom {name.ColorString(ColorConstants.White)}.", ColorConstants.Orange);
        }
        private void OnItemSelectedLoadAppearance(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject is null || !(selection.TargetObject is NwItem item) || item.Possessor != player.oid.ControlledCreature || selectedIndex < 0)
          {
            selectedIndex = -1;
            return;
          }

          // TODO : ajouter un métier permettant de modifier n'importe quelle tenue
          if (item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").HasValue && item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value != player.oid.LoginCreature.Name)
          {
            player.oid.SendServerMessage($"Impossible de modifier l'apparence de cet objet. Il est indiqué : Pour tout modification, s'adresser à {item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").Value.ColorString(ColorConstants.White)}", ColorConstants.Orange);
            selectedIndex = -1;
            return;
          }

          ItemAppearance appearance = player.itemAppearances[selectedIndex];

          if(item.BaseItem.ItemType != (BaseItemType)appearance.baseItem)
          {
            selectedIndex = -1;
            player.oid.SendServerMessage("Cette apparence ne peut pas être appliquée sur ce type d'objet !", ColorConstants.Red);
            return;
          }

          if (item.BaseItem.ItemType == BaseItemType.Armor && item.BaseACValue != appearance.ACValue)
          {
            selectedIndex = -1;
            player.oid.SendServerMessage("Cette apparence ne peut pas être appliquée sur ce type d'armure !", ColorConstants.Red);
            return;
          }


          ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemReceived, player.oid);
          ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.ItemLost, player.oid);
          ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.EquipWeaponSwappedOut, player.oid);
          ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.EquipSkillSpellModifiers, player.oid);
          ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.InventoryFull, player.oid);
          ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedToRun, player.oid);
          ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedWalkSlow, player.oid);
          ItemSystem.feedbackService.AddFeedbackMessageFilter(FeedbackMessage.SendMessageToPc, player.oid);

          item.Appearance.Deserialize(appearance.serializedAppearance);
          NwItem newItem = item.Clone(player.oid.ControlledCreature);
          item.Destroy();
          
          for (int i = 0; i <= (int)InventorySlot.Bolts; i++)
          {
            if (player.oid.ControlledCreature.GetItemInSlot((InventorySlot)i) == item)
            {
              player.oid.ControlledCreature.RunEquip(newItem, (InventorySlot)i);
              break;
            }
          }

          Task waitDestruction = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.4));
            ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemReceived, player.oid);
            ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.ItemLost, player.oid);
            ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.EquipWeaponSwappedOut, player.oid);
            ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.EquipSkillSpellModifiers, player.oid);
            ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.InventoryFull, player.oid);
            ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedToRun, player.oid);
            ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.WeightTooEncumberedWalkSlow, player.oid);
            ItemSystem.feedbackService.RemoveFeedbackMessageFilter(FeedbackMessage.SendMessageToPc, player.oid);

            player.oid.SendServerMessage($"L'apparence de votre {item.Name.ColorString(ColorConstants.White)} a bien été modifiée.", ColorConstants.Green);
          });
        }
      } 
    }
  }
}
