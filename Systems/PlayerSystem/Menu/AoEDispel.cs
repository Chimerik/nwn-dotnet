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
      public class AoEDispelWindow : PlayerWindow
      {
        private readonly NuiColumn rootRow = new NuiColumn();
        private readonly List<NuiElement> rootChildren = new List<NuiElement>();
        private readonly List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>();

        private readonly NuiBind<string> aoeIcons = new NuiBind<string>("aoeIcons");
        private readonly NuiBind<string> aoeName = new NuiBind<string>("aoeName");
        private readonly NuiBind<string> areaNames = new NuiBind<string>("areaNames");
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");

        public List<NwAreaOfEffect> currentList;

        public AoEDispelWindow(Player player) : base(player)
        {
          windowId = "aoeDispel";

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(aoeIcons) { Id = "examine", Tooltip = aoeName, Height = 35 }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(areaNames) { Height = 35, VerticalAlign = NuiVAlign.Middle }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("menu_exit") { Id = "delete", Tooltip = "Dissiper", Height = 35 }) { Width = 35 });

          rootRow.Children = rootChildren;
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 400, 650);

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
          player.oid.OnNuiEvent += HandleDispelAoEEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          UpdateAoEList();
        }

        private void HandleDispelAoEEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

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
              }

              break;
          }
        }
        public void UpdateAoEList()
        {
          currentList = NwObject.FindObjectsWithTag<NwAreaOfEffect>(player.oid.CDKey).ToList();
          LoadPlayerAoEList(currentList);
        }
        private void LoadPlayerAoEList(List<NwAreaOfEffect> aoeList)
        {
          List<string> aoeIconList = new List<string>();
          List<string> aoeAreaList = new List<string>();
          List<string> aoeNameList = new List<string>();

          foreach (NwAreaOfEffect aoe in aoeList)
          {
            NwSpell spell = NwSpell.FromSpellId(aoe.GetObjectVariable<LocalVariableInt>("SPELL_ID").Value);
            aoeIconList.Add(spell.IconResRef);
            aoeAreaList.Add(aoe.Area.Name);
            aoeNameList.Add(spell.Name);
          }

          aoeIcons.SetBindValues(player.oid, token, aoeIconList);
          areaNames.SetBindValues(player.oid, token, aoeAreaList);
          aoeName.SetBindValues(player.oid, token, aoeNameList);
          listCount.SetBindValue(player.oid, token, aoeList.Count());
        }
      }
    }
  }
}
