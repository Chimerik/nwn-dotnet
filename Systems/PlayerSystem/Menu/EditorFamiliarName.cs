using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class EditorFamiliarName : PlayerWindow
      {
        private readonly NuiColumn layoutColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> name = new("name");
        //private readonly NuiBind<string> itemDescription = new("itemDescription");
        private AssociateType associateType;
        private string nameType;

        public EditorFamiliarName(Player player, AssociateType associateType) : base(player)
        {
          windowId = "editorFamiliarName";

          layoutColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Nom", name, 30, false) { Height = 35, Width = 180 } } });
          //rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Description", itemDescription, 999, true) { Height = 170, Width = 780 } }});
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiButtonImage("ir_empytqs") { Id = "saveDescription", Tooltip = "Enregistrer les changements", Height = 35, Width = 35 }, new NuiSpacer() } });

          CreateWindow(associateType);
        }
        public void CreateWindow(AssociateType associateType)
        {
          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 240, 100);
          this.associateType = associateType;

          nameType = associateType switch
          {
            AssociateType.AnimalCompanion => "Compagnon Animal",
            _ => "Familier",
          };

          window = new NuiWindow(layoutColumn, $"Nom de votre {nameType}")
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

            nuiToken.OnNuiEvent += HandleEditorFamiliarNameEvents;

            name.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.FamiliarName);
            //itemDescription.SetBindValue(player.oid, nuiToken.Token, targetItem.Description);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleEditorFamiliarNameEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if(nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "saveDescription")
          {
            switch(associateType)
            {
              case AssociateType.AnimalCompanion: 

                player.oid.LoginCreature.AnimalCompanionName = name.GetBindValue(player.oid, nuiToken.Token);

                var animal = player.oid.LoginCreature.GetAssociate(AssociateType.AnimalCompanion);
                  
                if(animal is not null)                  
                    animal.Name = player.oid.LoginCreature.AnimalCompanionName;

                break;
              default: 

                player.oid.LoginCreature.FamiliarName = name.GetBindValue(player.oid, nuiToken.Token);

                var familiar = player.oid.LoginCreature.GetAssociate(AssociateType.Familiar);

                if (familiar is not null)
                  familiar.Name = player.oid.LoginCreature.FamiliarName;

                break;
            }
            
            player.oid.SendServerMessage($"Votre {nameType} est désormais nommé {player.oid.LoginCreature.FamiliarName.ColorString(ColorConstants.White)}", new Color(32, 255, 32));
            CloseWindow();
          }
        }
      }
    }
  }
}
