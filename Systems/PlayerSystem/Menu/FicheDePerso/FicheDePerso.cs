using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;
using static Anvil.API.Events.PlaceableEvents;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false };

        private NwCreature target;
        private Player targetPlayer;

        public FicheDePersoWindow(Player player, NwCreature target) : base(player)
        {
          windowId = "ficheDePerso";
          
          rootColumn.Children = rootChildren;
          rootGroup.Layout = rootColumn;
          menuGroup.Layout = menuRow;

          CreateWindow(target);
        }

        public void CreateWindow(NwCreature target)
        {
          this.target = target;
          if(!Players.TryGetValue(target, out targetPlayer))
            targetPlayer = player;

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(0, 0, windowWidth, windowHeight);

          LoadMainLayout();

          window = new NuiWindow(rootGroup, $"{target.Name} - Fiche de perso")
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
            nuiToken.OnNuiEvent += HandleCharacterSheetEvents;

            MainBindings();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }

          //if (!player.windows.TryGetValue("craftWorkshop", out var craftwindow)) player.windows.Add("craftWorkshop", new PlayerSystem.Player.WorkshopWindow(player, "", null));
          //else ((PlayerSystem.Player.WorkshopWindow)craftwindow).CreateWindow("", null);
        }
        private void HandleCharacterSheetEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          //ModuleSystem.Log.Info(nuiEvent.EventType);
          //ModuleSystem.Log.Info(nuiEvent.ElementId);

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "sheetMainView":

                  LoadMainLayout();
                  MainBindings();

                  break;

                case "sheetLearnables":

                  LoadLearnablesLayout();
                  LearnablesBindings();

                  break;

                case "sheetConditions":

                  LoadConditionsLayout();
                  ConditionsBindings();

                  break;

                case "sheetSkills":

                  LoadSkillsLayout();
                  SkillsBindings();

                  break;

                case "sheetWeapons":

                  LoadWeaponsLayout();
                  WeaponsBindings();

                  break;

                case "sheetDescription":

                  LoadDescriptionLayout();
                  DescriptionsBindings();

                  break;

                case "applyDescription":

                  player.oid.LoginCreature.Description = description.GetBindValue(player.oid, nuiToken.Token);

                  break;

                case "saveDescription":

                  string currentTitle = title.GetBindValue(player.oid, nuiToken.Token);
                  string currentDescription = description.GetBindValue(player.oid, nuiToken.Token);

                  foreach (var desc in player.descriptions)
                  {
                    if (desc.name == currentTitle)
                    {
                      desc.description = currentDescription;
                      SetDescriptionListBindings();
                      return;
                    }
                  }

                  player.descriptions.Add(new CharacterDescription(currentTitle, currentDescription));
                  SetDescriptionListBindings();

                  break;

                case "deleteDescription":

                  player.descriptions.RemoveAt(nuiEvent.ArrayIndex);
                  SetDescriptionListBindings();

                  break;

                case "cancelJob":
                  if (player.craftJob is not null)
                    player.craftJob.HandleCraftJobCancellation(player);
                  break;

                case "examineJobItem":

                  if (!string.IsNullOrEmpty(player.craftJob.serializedCraftedItem))
                  {
                    NwItem item = NwItem.Deserialize(player.craftJob.serializedCraftedItem.ToByteArray());

                    if (!player.windows.TryGetValue("itemExamine", out var value)) player.windows.Add("itemExamine", new ItemExamineWindow(player, item));
                    else ((ItemExamineWindow)value).CreateWindow(item);

                    ItemUtils.ScheduleItemForDestruction(item, 300);
                  }

                  break;
              }

              break;

            case NuiEventType.MouseUp:

              switch (nuiEvent.ElementId)
              {
                case "sheetPortrait":

                  LoadPortraitLayout();
                  PortraitBindings();

                  break;

                case "selectDescription":

                  title.SetBindValue(player.oid, nuiToken.Token, player.descriptions[nuiEvent.ArrayIndex].name);
                  description.SetBindValue(player.oid, nuiToken.Token, player.descriptions[nuiEvent.ArrayIndex].description);

                  break;
              }

              if (nuiEvent.ElementId.StartsWith("po_"))
                player.oid.LoginCreature.PortraitResRef = nuiEvent.ElementId.Remove(nuiEvent.ElementId.Length - 1);

              break;
          }
        }
      }
    }
  }
}
