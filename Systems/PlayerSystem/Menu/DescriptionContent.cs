using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class DescriptionContentWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren;
        private readonly NuiBind<string> descriptionName = new ("descriptionName");
        private readonly NuiBind<string> descriptionText = new ("descriptionText");
        private readonly NuiBind<bool> saveEnabled = new ("saveEnabled");
        private CharacterDescription description { get; set; }

        public DescriptionContentWindow(Player player, CharacterDescription description = null) : base(player)
        {
          windowId = "descriptionContent";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          CreateWindow(description);
        }
        public void CreateWindow(CharacterDescription description = null)
        {
          this.description = description;

          rootChidren.Clear();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.4f);

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiTextEdit("Nom de la description", descriptionName, 30, false) { Height = 35, Width = 160, Tooltip = "Le nom doit obligatoirement être renseigné pour sauvegarder la description." },
              new NuiSpacer()
            }
          });

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Contenu de la description", descriptionText, 15000, true) { Width = windowRectangle.Width - 20, Height = windowRectangle.Height - 90 },
            }
          });

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Enregistrer") { Id = "save", Enabled = saveEnabled, Width = 80, Height = 35 },
              new NuiSpacer(),
              new NuiButton("Appliquer") { Id = "apply", Tooltip = "Applique la description au personnage actuellement contrôlé.", Width = 80, Height = 35 },
              new NuiSpacer(),
            }
          });

          string title = description != null ? description.name : "Nouvelle description";

          window = new NuiWindow(rootGroup, title)
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleDescriptionContentEvents;
          player.oid.OnNuiEvent += HandleDescriptionContentEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          if(description != null)
          {
            descriptionName.SetBindValue(player.oid, token, description.name);
            descriptionText.SetBindValue(player.oid, token, description.description);
            saveEnabled.SetBindValue(player.oid, token, true);
          }
          else
          {
            descriptionName.SetBindValue(player.oid, token, "");
            descriptionText.SetBindValue(player.oid, token, "");
            saveEnabled.SetBindValue(player.oid, token, false);
          }

          descriptionName.SetBindWatch(player.oid, token, true);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void HandleDescriptionContentEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "save":

                  if (description == null)
                    player.descriptions.Add(new CharacterDescription(descriptionName.GetBindValue(player.oid, token), descriptionText.GetBindValue(player.oid, token)));
                  else
                  {
                    description.name = descriptionName.GetBindValue(player.oid, token);
                    description.description = descriptionText.GetBindValue(player.oid, token);
                  }

                  player.oid.SendServerMessage("Votre description a bien été enregistrée.", ColorConstants.Orange);

                  break;

                case "apply":

                  player.oid.ControlledCreature.Description = descriptionText.GetBindValue(player.oid, token);
                  player.oid.SendServerMessage($"Votre description a bien été appliquée à {player.oid.ControlledCreature.Name.ColorString(ColorConstants.White)}.", ColorConstants.Orange);

                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "descriptionName":
                  saveEnabled.SetBindValue(player.oid, token, descriptionName.GetBindValue(player.oid, token).Length > 0);
                  break;
              }

              break;

            case NuiEventType.Close:

              if (player.windows.ContainsKey("description"))
                ((DescriptionsWindow)player.windows["description"]).CreateWindow();
              else
                player.windows.Add("description", new DescriptionsWindow(player));

              break;
          }
        }
      }
    }
  }
}
