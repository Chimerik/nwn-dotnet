using System;
using System.Collections.Generic;
using System.Linq;
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
      public class PlayerEffectDispelWindow : PlayerWindow
      {
        private readonly NuiColumn rootRow = new NuiColumn();
        private readonly List<NuiElement> rootChildren = new List<NuiElement>();
        private readonly List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>();

        private readonly NuiBind<string> spellIcons = new ("spellIcons");
        private readonly NuiBind<string> spellName = new ("spellName");
        private readonly NuiBind<string> targetNames = new ("targetNames");
        private readonly NuiBind<string> spellRemainingDuration = new ("spellRemainingDuration");
        private readonly NuiBind<int> listCount = new ("listCount");

        public List<NwGameObject> currentList;
        private readonly List<NwSpell> spells = new();
        private readonly List<NwGameObject> targets = new();
        private ScheduledTask listRefresher;

        public PlayerEffectDispelWindow(Player player) : base(player)
        {
          windowId = "effectDispel";

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(spellIcons) { Id = "examine", Tooltip = spellName, Height = 35 }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(targetNames) { Height = 35, VerticalAlign = NuiVAlign.Middle }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(spellRemainingDuration) { Height = 35, VerticalAlign = NuiVAlign.Middle }) { Width = 40 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("menu_exit") { Id = "delete", Tooltip = "Dissiper", Height = 35 }) { Width = 35 });

          rootRow.Children = rootChildren;
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, 600);

          window = new NuiWindow(rootRow, "Dissipation des sorts")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleEffectEvents;
          player.oid.OnNuiEvent += HandleEffectEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          UpdateEffectList();

          listRefresher = player.scheduler.ScheduleRepeating(() =>
          {
            if (player.pcState == PcState.Offline || player.oid.ControlledCreature == null || !player.openedWindows.ContainsKey(windowId))
            {
              listRefresher.Dispose();
              return;
            }

            UpdateEffectList();

          }, TimeSpan.FromSeconds(1));
        }

        private void HandleEffectEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "delete":

                  NwSpell spell = spells[nuiEvent.ArrayIndex];
                  NwGameObject target = targets[nuiEvent.ArrayIndex];

                  foreach (Effect eff in target.ActiveEffects.Where(e => e.Spell == spell && e.Tag == player.oid.CDKey))
                    target.RemoveEffect(eff);

                  break;

                case "examine":

                  int spellId = (int)spells[nuiEvent.ArrayIndex].Id;

                  if (player.openedWindows.ContainsKey("learnableDescription"))
                    player.windows["learnableDescription"].CloseWindow();

                  if (player.windows.ContainsKey("learnableDescription"))
                    ((LearnableDescriptionWindow)player.windows["learnableDescription"]).CreateWindow(spellId);
                  else
                    player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, spellId));
                  
                  break;
              }

              break;
          }
        }
        public void UpdateEffectList()
        {
          currentList = player.effectTargets;
          LoadPlayerAoEList(currentList);
        }
        private void LoadPlayerAoEList(List<NwGameObject> targetList)
        {
          List<string> aoeIconList = new List<string>();
          List<string> aoeAreaList = new List<string>();
          List<string> aoeNameList = new List<string>();
          List<string> aoeDurationList = new List<string>();
          spells.Clear();
          targets.Clear();

          foreach (NwGameObject target in targetList)
          {
            bool foundEffect = false;

            foreach (var group in target.ActiveEffects.GroupBy(e => new { e.Spell, e.Tag }))
            {
              Effect latestEffect = group.OrderByDescending(e => e.DurationRemaining).FirstOrDefault();

              if (latestEffect.Tag == player.oid.CDKey)
              {
                foundEffect = true;

                aoeAreaList.Add(target.Name);
                aoeIconList.Add(latestEffect.Spell.IconResRef);
                aoeNameList.Add(latestEffect.Spell.Name.ToString());
                aoeDurationList.Add(latestEffect.DurationRemaining.ToString());
                spells.Add(latestEffect.Spell);
                targets.Add(target);
              }
            }

            if (!foundEffect)
            {
              Task delayedRemoval = NwTask.Run(async () =>
              {
                NwGameObject targetToRemove = target;
                await NwTask.Delay(TimeSpan.FromMilliseconds(10));
                player.effectTargets.Remove(target);
              });
            }
          }

          spellIcons.SetBindValues(player.oid, token, aoeIconList);
          targetNames.SetBindValues(player.oid, token, aoeAreaList);
          spellName.SetBindValues(player.oid, token, aoeNameList);
          spellRemainingDuration.SetBindValues(player.oid, token, aoeDurationList);
          listCount.SetBindValue(player.oid, token, aoeIconList.Count());
        }
      }
    }
  }
}
