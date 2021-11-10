using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreatePortraitDemoWindow()
      {
        NuiBind<string> portraitId = new NuiBind<string>("po_id");
        NuiBind<string> portraitResRef = new NuiBind<string>("po_resref");
        NuiBind<bool> btnPrevEnabled = new NuiBind<bool>("btnpreve");
        NuiBind<bool> btnSetEnabled = new NuiBind<bool>("btnoke");
        NuiBind<bool> btnNextEnabled = new NuiBind<bool>("btnnexte");
        NuiBind<int> portraitCategory = new NuiBind<int>("po_category");
        NuiBind<bool> collapsed = new NuiBind<bool>("collapsed");
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");

        List<NuiComboEntry> comboValues = new List<NuiComboEntry>
        {
          new NuiComboEntry("Cats (164-167)", 0),
          new NuiComboEntry("Dragonos !! (191-200)", 1)
        };

        // Construct the window layout.
        NuiColumn root = new NuiColumn
        {
          Children = new List<NuiElement>
          {
            new NuiRow
            {
              Height = 20.0f,
              Children = new List<NuiElement>
              {
                new NuiSpacer(),
                new NuiLabel(portraitResRef) { ForegroundColor = new NuiColor(255, 100, 0) },
                new NuiSpacer()
              }
            },
            new NuiRow
            {
              Height = 20.0f,
              Children = new List<NuiElement>
              {
                new NuiSpacer(),
                new NuiLabel(portraitId),
                new NuiSpacer()
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiSpacer(),
                /*new NuiGroup
                {
                  Width = 256.0f,
                  Height = 400.0f,
                  Children = new List<NuiElement>
                  {
                    new NuiImage(portraitResRef)
                    {
                      ImageAspect = NuiAspect.Fill,
                      HorizontalAlign = NuiHAlign.Center,
                      VerticalAlign = NuiVAlign.Middle
                    }
                  }
                },*/
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
                  Entries = comboValues,
                  Selected = portraitCategory
                },
                new NuiSpacer()
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiButton("<")
                {
                  Id = "btnprev",
                  Enabled = btnPrevEnabled,
                  Width = 80.0f
                },
                new NuiSpacer(),
                new NuiButton("Set")
                {
                  Id = "btnok",
                  Enabled = btnSetEnabled,
                  Width = 80.0f
                },
                new NuiSpacer(),
                new NuiButton(">")
                {
                  Id = "btnnext",
                  Enabled = btnNextEnabled,
                  Width = 80.0f
                },
              }
            }
          }
        };

        NuiWindow window = new NuiWindow(root, "Portrait démo")
        {
          Geometry = new NuiRect(420.0f, 10.0f, 400.0f, 600.0f),
          Resizable = true,
          Collapsed = collapsed,
          Closable = true,
          Transparent = false,
          Border = true,
        };

        oid.OnNuiEvent += HandlePortraitDemoEvents;

        int token = oid.CreateNuiWindow(window, "portrait_demo");

        int id = 164;

        string resRef = "po_" + Portraits2da.portraitsTable.GetDataEntry(id).resRef + "h";
        portraitId.SetBindValue(oid, token, id.ToString());
        portraitResRef.SetBindValue(oid, token, resRef);
        btnPrevEnabled.SetBindValue(oid, token, false);
        btnNextEnabled.SetBindValue(oid, token, true);
        portraitCategory.SetBindValue(oid, token, 0);
        portraitCategory.SetBindWatch(oid, token, true);
        collapsed.SetBindWatch(oid, token, true);
      }
    }
  }
}
