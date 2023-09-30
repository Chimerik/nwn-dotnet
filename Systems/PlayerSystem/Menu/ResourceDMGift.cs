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
      public class ResourceDMGiftWindow : PlayerWindow
      {
        private readonly NuiColumn rootRow = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<string> myResourceNames = new("myResourceNames");
        private readonly NuiBind<int> myListCount = new("myListCount");
        private readonly NuiBind<string> myResourceIcon = new("myResourceIcon");
        private readonly NuiBind<string> myQuantity = new("myQuantity");

        Player targetPlayer;

        public ResourceDMGiftWindow(Player player, Player targetPlayer) : base(player)
        {
          windowId = "resourceDMGift";
          rootRow.Children = rootChildren;

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(myResourceIcon) { Id = "send", Tooltip = myResourceNames, Height = 35 }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(myResourceNames) { VerticalAlign = NuiVAlign.Middle }));

          rootChildren.Add(new NuiRow() { Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiTextEdit("", myQuantity, 10, false) { Width = 100 }, new NuiSpacer() } });
          rootChildren.Add(new NuiRow() { Height = 350, Children = new List<NuiElement>() { new NuiList(rowTemplate, myListCount) { RowHeight = 35 } } });

          CreateWindow(targetPlayer);
        }
        public void CreateWindow(Player target)
        {
          this.targetPlayer = target;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 350, 550);

          window = new NuiWindow(rootRow, $"Don de ressources : {target.oid.LoginCreature.Name}")
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
            nuiToken.OnNuiEvent += HandleResourceGiftEvents;
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            LoadResourceList();
          }
        }

        private void HandleResourceGiftEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "send":

                  if (targetPlayer.oid.LoginCreature == null)
                  {
                    CloseWindow();
                    player.oid.SendServerMessage("La cible du don n'est plus valide.", ColorConstants.Red);
                    return;
                  }

                  var inputQuantity = myQuantity.GetBindValue(player.oid, nuiToken.Token);

                  if (int.TryParse(inputQuantity, out int quantity) && quantity > 0)
                  {
                    CraftResource resource = Craft.Collect.System.craftResourceArray[nuiEvent.ArrayIndex];
                    CraftResource myResource = targetPlayer.craftResourceStock.FirstOrDefault(r => r.type == resource.type);

                    if (myResource != null)
                      myResource.quantity += quantity;
                    else
                      targetPlayer.craftResourceStock.Add(new CraftResource(resource, quantity));

                    LogUtils.LogMessage($"{player.oid.PlayerName} fait un don à {targetPlayer.oid.LoginCreature.Name} ({resource.type} - {quantity})", LogUtils.LogType.DMAction);

                    player.oid.SendServerMessage($"Don de {quantity} unité(s) de {resource.name} à {targetPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} terminé avec succès !", new Color(32, 255, 32));
                    targetPlayer.oid.SendServerMessage($"{player.oid.LoginCreature.Name.ColorString(ColorConstants.White)} vient de vous faire don de {quantity} unité(s) de {resource.name}.", new Color(32, 255, 32));
                    targetPlayer.oid.ExportCharacter();
                  }
                  else
                    player.oid.SendServerMessage($"Quantité saisie invalide", ColorConstants.Red);

                  break;
              }

              break;
          }
        }
        private void LoadResourceList()
        {
          List<string> resourceNameList = new();
          List<string> resourceIconList = new();

          foreach (CraftResource resource in Craft.Collect.System.craftResourceArray)
          {
            resourceNameList.Add(resource.name);
            resourceIconList.Add(resource.iconString);
          }

          myResourceNames.SetBindValues(player.oid, nuiToken.Token, resourceNameList);
          myResourceIcon.SetBindValues(player.oid, nuiToken.Token, resourceIconList);
          myListCount.SetBindValue(player.oid, nuiToken.Token, resourceNameList.Count);
        }
      }
    }
  }
}
