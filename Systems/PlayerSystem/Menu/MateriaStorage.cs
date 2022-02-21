using System;
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
      public class MateriaStorageWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren = new List<NuiElement>();
        private readonly NuiRow introTextRow;
        private readonly NuiRow storeRow = new NuiRow() 
        { 
          Children = new List<NuiElement>() 
          { 
            new NuiSpacer(),
            new NuiButton("Déposer toutes mes matérias") { Id = "store", Height = 30 },
            new NuiButton("Retirer de la matéria") { Id = "withdraw", Height = 30 },
            new NuiButton("S'éloigner") { Id = "exit", Height = 30 },
            new NuiSpacer()
          } 
        };
        private readonly NuiText widgetNPCText;
        private readonly NuiBind<string> npcText = new NuiBind<string>("npcText");
        private readonly NuiBind<string> materiaNames = new NuiBind<string>("materiaNames");
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");
        private readonly NuiBind<string> materiaIcon = new NuiBind<string>("materiaIcon");

        public MateriaStorageWindow(Player player, NwCreature oNPC) : base(player)
        {
          windowId = "materiaStorage";

          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          widgetNPCText = new NuiText(npcText) { Width = 450, Height = 140 };

          introTextRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiImage(oNPC.PortraitResRef + "M") { ImageAspect = NuiAspect.ExactScaled, Width = 60, Height = 100 },
              widgetNPCText
            }
          };

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiImage(materiaIcon) { Height = 40, Width = 40 }) { Width = 45 },
            new NuiListTemplateCell(new NuiLabel(materiaNames) { VerticalAlign = NuiVAlign.Middle }),
          };

          rootChidren.Add(introTextRow);
          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 40 } );
          rootChidren.Add(storeRow);
          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChidren.Clear();

          string tempText = "Salut, de la matéria à stocker ? Ici, à l'entrepôt des portes, on s'occupe de la mettre en sécurité.\n\n" +
            "Nos ouvriers vous la font même nsuite parvenir où vous avez besoin sans risque de contamination !\n\n" +
            "Bien sur, ces services vous coûteront 5 %. Alors, qu'est ce qu'on peut faire pour vous ?";

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, 600);

          window = new NuiWindow(rootGroup, "Rano Snyders")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleMateriaStorageEvents;
          player.oid.OnNuiEvent += HandleMateriaStorageEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          npcText.SetBindValue(player.oid, token, tempText);
          LoadMateriaList();

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void HandleMateriaStorageEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "store":

                  foreach(NwItem item in player.oid.ControlledCreature.Inventory.Items.Where(i => i.Tag == "craft_resource"))
                  {
                    ResourceType type = (ResourceType)Enum.Parse(typeof(ResourceType), item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value);
                    int grade = item.GetObjectVariable<LocalVariableInt>("CRAFT_GRADE").Value;

                    CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == type && r.grade == grade);

                    if (resource != null)
                    {
                      resource.quantity += item.StackSize;
                      item.Destroy();
                    }
                    else
                    {
                      resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == (ResourceType)Enum.Parse(typeof(ResourceType), item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value)
                      && r.grade == item.GetObjectVariable<LocalVariableInt>("CRAFT_GRADE").Value);

                      if (resource != null)
                      {
                        player.craftResourceStock.Add(new CraftResource(resource, item.StackSize));
                        item.Destroy();
                      }
                      else
                      {
                        player.oid.SendServerMessage($"{item.Name} est taggué ressource de craft mais n'a pas pu être reconnu par le système. Le staff a été informé du problème.");
                        Utils.LogMessageToDMs($"ERRROR - CraftResource : {item.Name} unrecognized - Player {player.oid.LoginCreature.Name}");
                      }
                    }
                  }

                  LoadMateriaList();

                  return;

                case "withdraw":
                  npcText.SetBindValue(player.oid, token, "Retirer ? Et pour quoi faire ? Vous ne pouvez pas entrer en ville avec de toute façon.\n\n" +
                    "Si vous voulez vendre du stock à quelqu'un, passez plutôt par un contrat du Juge du Changement.");
                  return;

                case "exit":
                  player.oid.NuiDestroy(token);
                  return;
              }
              break;
          }
        }
        private void LoadMateriaList()
        {
          List<string> materiaNamesList = new List<string>();
          List<string> materiaIconList = new List<string>();

          foreach (CraftResource resource in player.craftResourceStock)
          {
            materiaIconList.Add(resource.iconString);
            materiaNamesList.Add($"{resource.name} (x{resource.quantity})");
          }

          materiaIcon.SetBindValues(player.oid, token, materiaIconList);
          materiaNames.SetBindValues(player.oid, token, materiaNamesList);
          listCount.SetBindValue(player.oid, token, player.craftResourceStock.Count);
        }
      }
    }
  }
}
