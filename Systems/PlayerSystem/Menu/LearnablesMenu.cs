using System;
using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateLearnablesWindow()
      {
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey("learnables") ? windowRectangles["learnables"] : new NuiRect(420.0f, 10.0f, 600.0f, 400.0f);

        NuiBind<string> name = new NuiBind<string>("name");
        NuiBind<string> currentLevel = new NuiBind<string>("currentLevel");
        NuiBind<string> iconResRef = new NuiBind<string>("icon");
        NuiBind<DateTime> trainingEndTime = new NuiBind<DateTime>("trainingEndTime");
        NuiBind<string> remainingTimeDisplayText = new NuiBind<string>("remainingTimeDisplayText");

        NuiBind<int> learnableCategory = new NuiBind<int>("learnableCategory");
        List<NuiComboEntry> categories = new List<NuiComboEntry>
          {
            new NuiComboEntry("Craft", 1),
            new NuiComboEntry("Combat", 3),
            new NuiComboEntry("Magie", 6)
          };

        // Construct the window layout.
        NuiCol root = new NuiCol
        {
          Children = new List<NuiElement>
          {
            new NuiRow
            {
              Height = 20.0f,
              Children = new List<NuiElement>
              {
                new NuiSpacer(),
                new NuiButton
                {
                  Id = "feats",
                  Label = "Dons",
                },
                new NuiButton
                {
                  Id = "spells",
                  Label = "Sorts",
                },
                new NuiSpacer()
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiSpacer(),
                new NuiCombo
                {
                  Entries = categories,
                  Selected = learnableCategory
                },
                new NuiSpacer()
              }
            },
            new NuiGroup
            {
              Border = true,
              Height = 60,
              Children = new List<NuiElement>
              {
                new NuiRow
                {
                  Children = new List<NuiElement>
                  {
                    new NuiButtonImage
                    {
                      ResRef = iconResRef,
                      Height = 50,
                      Width = 50,
                      Tooltip = "Description"
                    },
                    new NuiLabel
                    {
                      Value = name, 
                    },
                    new NuiLabel
                    {
                      Value = currentLevel,
                    },
                    new NuiLabel
                    {
                      Value = remainingTimeDisplayText,
                    },
                    new NuiButton
                    {
                      Id = "learn",
                      Label = "Apprendre",
                      Height = 50
                    }
                  }
                },
              }
            }
          }
        };

        NuiWindow window = new NuiWindow
        {
          Root = root,
          Title = "Apprentissage",
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = false,
          Border = true,
        };

        int token = oid.CreateNuiWindow(window, "learnables");

        name.SetBindValue(oid, token, "Port d'armure lourde");
        currentLevel.SetBindValue(oid, token, 1.ToString());
        iconResRef.SetBindValue(oid, token, "ife_armor_h");
        learnableCategory.SetBindValue(oid, token, 0);

        DateTime timeLeft = DateTime.Now.AddMinutes(3);
        trainingEndTime.SetBindValue(oid, token, timeLeft);
        remainingTimeDisplayText.SetBindValue(oid, token, Utils.FormatTimeSpan(timeLeft - DateTime.Now));

        //timeToNextLevel.SetBindWatch(oid, token, true);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);

        ModuleSystem.scheduler.ScheduleRepeating(() => 
        {
          timeLeft.AddSeconds(-1);
          remainingTimeDisplayText.SetBindValue(oid, token, Utils.FormatTimeSpan(timeLeft - DateTime.Now)); 
        }
        , TimeSpan.FromSeconds(1));
      }
    }
  }
}
