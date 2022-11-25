using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class AoEDispelWindow : PlayerWindow
      {
        private readonly NuiColumn rootRow = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<string> aoeIcons = new("aoeIcons");
        private readonly NuiBind<string> aoeName = new("aoeName");
        private readonly NuiBind<string> areaNames = new("areaNames");
        private readonly NuiBind<string> aoeRemainingDuration = new("aoeRemainingDuration");
        private readonly NuiBind<int> listCount = new("listCount");

        public List<NwAreaOfEffect> currentList;
        private ScheduledTask listRefresher;

        public AoEDispelWindow(Player player) : base(player)
        {
          windowId = "aoeDispel";

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(aoeIcons) { Id = "examine", Tooltip = aoeName, Height = 35 }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(areaNames) { Height = 35, VerticalAlign = NuiVAlign.Middle }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(aoeRemainingDuration) { Height = 35, VerticalAlign = NuiVAlign.Middle }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("menu_exit") { Id = "delete", Tooltip = "Dissiper", Height = 35 }) { Width = 35 });

          rootRow.Children = rootChildren;
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, 600);

          window = new NuiWindow(rootRow, "Dissipation des sorts à zone d'effet")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleDispelAoEEvents;


          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleDispelAoEEvents;

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            UpdateAoEList();

            if (listRefresher != null)
              listRefresher.Dispose();

            listRefresher = player.scheduler.ScheduleRepeating(() =>
            {
              if (player.pcState == PcState.Offline || player.oid.ControlledCreature == null || !IsOpen)
              {
                listRefresher.Dispose();
                return;
              }

              UpdateAoEList();

            }, TimeSpan.FromSeconds(1));
          }
        }

        private void HandleDispelAoEEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "delete":

                  NwAreaOfEffect aoe = currentList[nuiEvent.ArrayIndex];

                  if (aoe.IsValid)
                  {
                    aoe.Tag = "AOE_DISPOSED";
                    aoe.Destroy();
                  }

                  UpdateAoEList();

                  break;

                case "examine":

                  int spellId = (int)currentList.ElementAt(nuiEvent.ArrayIndex).Spell.Id;

                  if (player.TryGetOpenedWindow("learnableDescription", out PlayerWindow descriptionWindow))
                    descriptionWindow.CloseWindow();

                  if (!player.windows.ContainsKey("learnableDescription")) player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, spellId));
                  else ((LearnableDescriptionWindow)player.windows["learnableDescription"]).CreateWindow(spellId);

                  break;
              }

              break;
          }
        }
        public void UpdateAoEList()
        {
          currentList = NwObject.FindObjectsWithTag<NwAreaOfEffect>($"_PLAYER_{player.characterId}").ToList();
          LoadPlayerAoEList(currentList);
        }
        private void LoadPlayerAoEList(List<NwAreaOfEffect> aoeList)
        {
          List<string> aoeIconList = new List<string>();
          List<string> aoeAreaList = new List<string>();
          List<string> aoeNameList = new List<string>();
          List<string> aoeDurationList = new List<string>();

          foreach (NwAreaOfEffect aoe in aoeList)
          {
            aoeIconList.Add(aoe.Spell.IconResRef);
            aoeAreaList.Add(aoe.Area.Name);
            aoeNameList.Add(aoe.Spell.Name.ToString());
            aoeDurationList.Add(aoe.RemainingDuration.TotalSeconds.ToString());
          }

          aoeIcons.SetBindValues(player.oid, nuiToken.Token, aoeIconList);
          areaNames.SetBindValues(player.oid, nuiToken.Token, aoeAreaList);
          aoeName.SetBindValues(player.oid, nuiToken.Token, aoeNameList);
          aoeRemainingDuration.SetBindValues(player.oid, nuiToken.Token, aoeDurationList);
          listCount.SetBindValue(player.oid, nuiToken.Token, aoeList.Count);
        }
      }
    }
  }
}
