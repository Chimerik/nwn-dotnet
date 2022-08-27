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
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiBind<string> npcText = new ("npcText");
        private readonly NuiBind<string> materiaNames = new ("materiaNames");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly NuiBind<string> materiaIcon = new ("materiaIcon");

        public MateriaStorageWindow(Player player, NwCreature oNPC) : base(player)
        {
          windowId = "materiaStorage";
          rootColumn.Children = rootChidren;
          
          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiImage(oNPC.PortraitResRef + "M") { ImageAspect = NuiAspect.ExactScaled, Width = 60, Height = 100, VerticalAlign = NuiVAlign.Middle },
            new NuiText(npcText) { Width = 450, Height = 140 }
          } });

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ir_drop") { Id = "store", Height = 40, Width = 40, Tooltip = "Déposer toutes mes matérias" },
            new NuiSpacer(),
            new NuiButton("ir_sell") { Id = "withdraw", Height = 40, Width = 40, Tooltip = "Retirer de la matéria" },
            new NuiSpacer()
          } });

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButtonImage(materiaIcon) { Height = 40, Width = 40 }) { Width = 40 },
            new NuiListTemplateCell(new NuiLabel(materiaNames) { VerticalAlign = NuiVAlign.Middle }),
          };

          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 40 } );
          
          CreateWindow();
        }
        public void CreateWindow()
        {
          string tempText = "Salut, de la matéria à stocker ? Ici, à l'entrepôt des portes, on s'occupe de la mettre en sécurité.\n\n" +
            "Nos ouvriers vous la font même ensuite parvenir où vous avez besoin sans risque de contamination !\n\n" +
            "Bien sur, ces services vous coûteront 5 %. Alors, qu'est ce qu'on peut faire pour vous ?";

          NuiRect windowRectangle = /*player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] :*/ new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, 400);

          window = new NuiWindow(rootColumn, "Rano Snyders")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleMateriaStorageEvents;
            player.oid.OnServerSendArea += OnAreaChangeCloseWindow;

            npcText.SetBindValue(player.oid, nuiToken.Token, tempText);
            LoadMateriaList();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleMateriaStorageEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "store":

                  double promenadeTaxes = player.learnableSkills.ContainsKey(CustomSkill.ConnectionsPromenade) ? player.learnableSkills[CustomSkill.ConnectionsPromenade].bonusMultiplier / 100 : 0.95;

                  foreach (NwItem item in player.oid.ControlledCreature.Inventory.Items.Where(i => i.Tag == "craft_resource"))
                  {
                    ResourceType type = (ResourceType)Enum.Parse(typeof(ResourceType), item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value);
                    int grade = item.GetObjectVariable<LocalVariableInt>("CRAFT_GRADE").Value;

         

                    CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == type && r.grade == grade);

                    if (resource != null)
                    {
                      resource.quantity += (int)(item.StackSize * promenadeTaxes);
                      item.Destroy();
                    }
                    else
                    {
                      resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == (ResourceType)Enum.Parse(typeof(ResourceType), item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value)
                      && r.grade == item.GetObjectVariable<LocalVariableInt>("CRAFT_GRADE").Value);

                      if (resource != null)
                      {
                        player.craftResourceStock.Add(new CraftResource(resource, (int)(item.StackSize * promenadeTaxes)));
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
                  npcText.SetBindValue(player.oid, nuiToken.Token, "Retirer ? Et pour quoi faire ? Vous ne pouvez pas entrer en ville avec de toute façon.\n\n" +
                    "Si vous voulez vendre du stock à quelqu'un, passez plutôt par un contrat du Juge du Changement.");
                  return;

                case "exit":
                  CloseWindow();
                  return;
              }
              break;
          }
        }
        private void LoadMateriaList()
        {
          List<string> materiaNamesList = new();
          List<string> materiaIconList = new();

          foreach (CraftResource resource in player.craftResourceStock)
          {
            materiaIconList.Add(resource.iconString);
            materiaNamesList.Add($"{resource.name} (x{resource.quantity})");
          }

          materiaIcon.SetBindValues(player.oid, nuiToken.Token, materiaIconList);
          materiaNames.SetBindValues(player.oid, nuiToken.Token, materiaNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, player.craftResourceStock.Count);
        }
      }
    }
  }
}
