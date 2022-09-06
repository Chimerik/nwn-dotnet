using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
      public class AreaLoadScreenEditorWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<string> currentLoadScreen = new("currentLoadScreen");
        private readonly NuiBind<string> loadScreenResRef = new("loadScreenResRef");
        private readonly NuiBind<int> listCount = new("listCount");
        private NwArea area { get; set; }
        private bool saveAuthorized { get; set; }
        private int saveScheduled { get; set; }

        public AreaLoadScreenEditorWindow(Player player, NwArea area) : base(player)
        {
          windowId = "areaLoadScreenEditor";
          saveAuthorized = false;
          saveScheduled = 0;
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell> { new NuiListTemplateCell(new NuiButtonImage(loadScreenResRef) { Id = "select", Height = 200 }) };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiButtonImage(currentLoadScreen) { Width = 410, Height = 200 } } });
          rootChildren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 200 });

          CreateWindow(area);
        }

        public void CreateWindow(NwArea area)
        {
          this.area = area;
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, $"Choix d'écran de chargement - {area.Name}")
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
            nuiToken.OnNuiEvent += HandleAreaLoadScreenEditorEvents;
            player.oid.OnServerSendArea += OnAreaChangeCloseWindow;
            
            currentLoadScreen.SetBindValue(player.oid, nuiToken.Token, LoadScreen2da.loadScreenResRef[(int)area.LoadScreen]);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            LoadList();
          }
        }

        private void HandleAreaLoadScreenEditorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "select":

                  var entry = LoadScreen2da.loadScreenResRef.ElementAt(nuiEvent.ArrayIndex);
                  currentLoadScreen.SetBindValue(player.oid, nuiToken.Token, entry.Value);
                  area.LoadScreen = (ushort)entry.Key;

                  if (saveScheduled > 0)
                    saveScheduled += 1;
                  else
                  {
                    saveScheduled = 1;
                    Utils.LogMessageToDMs($"{player.oid.PlayerName} : scheduling {area.Name} loadscreen save in 10s");
                    DebounceSave(area, 1);
                  }

                  break;
              }

              break;
          }
        }
        private void LoadList()
        {
          List<string> nameList = new();

          foreach (var loadscreen in LoadScreen2da.loadScreenResRef)
            nameList.Add(loadscreen.Value);

          loadScreenResRef.SetBindValues(player.oid, nuiToken.Token, nameList);
          listCount.SetBindValue(player.oid, nuiToken.Token, nameList.Count);
        }
        public async void DebounceSave(NwArea areaToSave, int nbDebounces)
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task awaitDebounce = NwTask.WaitUntil(() => saveScheduled != nbDebounces, tokenSource.Token);
          Task awaitSaveAuthorized = NwTask.Delay(TimeSpan.FromSeconds(10), tokenSource.Token);

          await NwTask.WhenAny(awaitDebounce, awaitSaveAuthorized);
          tokenSource.Cancel();

          if (areaToSave == null || !areaToSave.IsValid)
            return;

          if (awaitDebounce.IsCompletedSuccessfully)
          {
            DebounceSave(areaToSave, nbDebounces + 1);
            return;
          }

          if (awaitSaveAuthorized.IsCompletedSuccessfully)
          {
            saveScheduled = 0;
            saveAuthorized = false;
            Utils.LogMessageToDMs($"{player.oid.PlayerName} : {area.Name} saving loadscreen");
            HandleSave(areaToSave);
          }
        }
        public async void HandleSave(NwArea areaToSave)
        {
          await SqLiteUtils.InsertQueryAsync("areaLoadScreens",
          new List<string[]>() {
            new string[] { "areaTag", areaToSave.Tag },
            new string[] { "loadScreen", areaToSave.LoadScreen.ToString() } },
          new List<string>() { "areaTag" },
          new List<string[]>() { new string[] { "loadScreen" } });
        }
      }
    }
  }
}
