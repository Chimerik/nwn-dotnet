using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class EditorItemName : PlayerWindow
      {
        private NwItem targetItem;
        private readonly NuiColumn layoutColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> name = new("name");
        private readonly NuiBind<string> itemDescription = new("itemDescription");

        public EditorItemName(Player player, NwItem targetItem) : base(player)
        {
          windowId = "editorItemName";

          layoutColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Nom", name, 50, false) { Height = 35, Width = 780 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Description", itemDescription, 999, true) { Height = 170, Width = 780 } }});
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiButtonImage("ir_empytqs") { Id = "saveDescription", Tooltip = "Enregistrer les changements", Height = 35, Width = 35 }, new NuiSpacer() } });

          CreateWindow(targetItem);
        }
        public void CreateWindow(NwItem targetItem)
        {
          this.targetItem = targetItem;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 820, 300);

          window = new NuiWindow(layoutColumn, $"Modification de {targetItem.Name}")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            nuiToken.OnNuiEvent += HandleEditorItemNameEvents;

            name.SetBindValue(player.oid, nuiToken.Token, targetItem.Name);
            itemDescription.SetBindValue(player.oid, nuiToken.Token, targetItem.Description);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleEditorItemNameEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (targetItem == null || !targetItem.IsValid || (targetItem.Possessor != player.oid.ControlledCreature && !player.IsDm()))
          {
            player.oid.SendServerMessage("L'objet édité n'est plus valide ou n'est plus en votre possession.", ColorConstants.Red);
            CloseWindow();
            return;
          }

          if(nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "saveDescription")
          {
            targetItem.Description = itemDescription.GetBindValue(player.oid, nuiToken.Token);
            targetItem.Name = name.GetBindValue(player.oid, nuiToken.Token);
            player.oid.SendServerMessage($"La description et le commentaire de l'objet {targetItem.Name.ColorString(ColorConstants.White)} ont bien été enregistrées.", new Color(32, 255, 32));
          }
        }
      }
    }
  }
}
