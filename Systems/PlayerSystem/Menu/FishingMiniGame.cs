using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateFishingMiniGameWindow()
      {
        NuiBind<string> selection = new NuiBind<string>("selection");
        NuiBind<float> progress = new NuiBind<float>("progress");
        NuiBind<int> slider = new NuiBind<int>("slider");
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey("chat") ? windowRectangles["chat"] : new NuiRect(420.0f, 10.0f, 600.0f, 400.0f);


        List<NuiChartSlot> slotTest = new List<NuiChartSlot>();
        NuiChartSlot slot = new NuiChartSlot();
        slot.Color = new NuiColor(255, 100, 0);
        slot.ChartType = NuiChartType.Column;
        slot.Legend = "Super chart !";
        slot.Data = new List<float>() { 37, 50, 12 };
        slotTest.Add(slot);

        slot = new NuiChartSlot();
        slot.Color = new NuiColor(150, 25, 25);
        slot.ChartType = NuiChartType.Column;
        slot.Legend = "Super chart !";
        slot.Data = new List<float>() { 100 };
        slotTest.Add(slot);


        List<NuiChartSlot> slotTest2 = new List<NuiChartSlot>();
        slot = new NuiChartSlot();
        slot.Color = new NuiColor(37, 48, 79);
        slot.ChartType = NuiChartType.Lines;
        slot.Legend = "Super chart 2 !";
        slot.Data = new List<float>() { 20, 100 };
        slotTest2.Add(slot);

        // Construct the window layout.
        NuiCol root = new NuiCol
        {
          Children = new List<NuiElement>
          {
            /*new NuiProgress
            { 
               Value = 1,
               //Color = new NuiColor(150, 25, 25)
            },*/
            new NuiRow
            {
              Height = 10,
              Children = new List<NuiElement>
              {
                new NuiSpacer {},
                new NuiLabel
                {  
                   Value = selection
                },
                new NuiSpacer {}
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiSpacer {},
                new NuiSlider
                {
                   Id = "chooser",
                   Min = 0,
                   Max = 255, Step = 1,  Width = 500,
                   Value = slider
                },
                new NuiSpacer {}
              }
            }
            /*new NuiRow
            { 
              Padding = 0,
            Margin = 0,
              Children = new List<NuiElement>
              { 
                
            new NuiButtonImage
            {
                ResRef = "leather1", Padding = 0, Margin = 0, 
                 Width = 39,
                 Height = 38
                //ImageAspect = NuiAspect.Exact
            },
            new NuiButtonImage
            {
                ResRef = "leather2",Padding = 0, Margin = 0 ,
                Width = 39,
                 Height = 38
                //ImageAspect = NuiAspect.Exact
            },
            new NuiButtonImage
            {
                ResRef = "leather3",Padding = 0, Margin = 0 ,
                Width = 39,
                 Height = 38
                //ImageAspect = NuiAspect.Exact
            }
              }
          }*/
            /*new NuiChart
            {
             ChartSlots = slotTest,
               
              
            },
            new NuiChart
            {
             ChartSlots = slotTest2,


            }*/

          }
        };

        NuiWindow window = new NuiWindow
        {
          Root = root,
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = false,
          Border = true,
        };

        oid.OnNuiEvent += HandleFishingMiniGameEvents;

        int token = oid.CreateNuiWindow(window, "fishingMiniGame");

        float progressValue = 0;
        bool reverse = false;

        selection.SetBindValue(oid, token, "0");
        slider.SetBindValue(oid, token, 0);
        slider.SetBindWatch(oid, token, true);
        progress.SetBindValue(oid, token, progressValue);
        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
        
        /*ModuleSystem.scheduler.ScheduleRepeating(() => 
        {
          if (progressValue > 0.99) reverse = true;
          if (progressValue < 0.01) reverse = false;

          progressValue = reverse ? progressValue - 0.01f : progressValue + 0.01f;
          progress.SetBindValue(oid, token, progressValue);
        }, 
        TimeSpan.FromSeconds(0.1));*/
      }
    }
  }
}
