using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  class PortraitDemo
  {
    public readonly NuiBind<string> portraitId;
    public readonly NuiBind<string> portraitResRef;
    public readonly NuiBind<bool> btnPrevEnabled;
    public readonly NuiBind<bool> btnSetEnabled;
    public readonly NuiBind<bool> btnNextEnabled;
    public readonly NuiBind<int> portraitCategory;
    public readonly NuiBind<bool> collapsed;
    public readonly NuiBind<NuiRect> geometry;
    public readonly NuiWindow window;

    public PortraitDemo()
    {
      portraitId = new NuiBind<string>("po_id");
      portraitResRef = new NuiBind<string>("po_resref");
      btnPrevEnabled = new NuiBind<bool>("btnpreve");
      btnSetEnabled = new NuiBind<bool>("btnoke");
      btnNextEnabled = new NuiBind<bool>("btnnexte");
      portraitCategory = new NuiBind<int>("po_category");
      collapsed = new NuiBind<bool>("collapsed");
      geometry = new NuiBind<NuiRect>("geometry");
      
      List<NuiComboEntry> comboValues = new List<NuiComboEntry>
      {
        new NuiComboEntry("Cats (164-167)", 0),
        new NuiComboEntry("Dragonos !! (191-200)", 1)
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
              new NuiLabel
              {
                Value = portraitResRef,
                TextColor = new NuiColor(255, 100, 0)
              },
              new NuiSpacer()
            }
          },
          new NuiRow
          {
            Height = 20.0f,
            Children = new List<NuiElement>
            {
              new NuiSpacer(),
              new NuiLabel
              {
                Value = portraitId,
              },
              new NuiSpacer()
            }
          },
          new NuiRow
          {
            Children = new List<NuiElement>
            {
              new NuiSpacer(),
              new NuiGroup
              {
                Width = 256.0f,
                Height = 400.0f,
                Children = new List<NuiElement>
                {
                  new NuiImage
                  {
                    ResRef = portraitResRef,
                    ImageAspect = NuiAspect.Fill,
                    HorizontalAlign = NuiHAlign.Center,
                    VerticalAlign = NuiVAlign.Middle
                  }
                }
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
              new NuiButton
              {
                Id = "btnprev",
                Label = "<",
                Enabled = btnPrevEnabled,
                Width = 80.0f
              },
              new NuiSpacer(),
              new NuiButton
              {
                Id = "btnok",
                Label = "Set",
                Enabled = btnSetEnabled,
                Width = 80.0f
              },
              new NuiSpacer(),
              new NuiButton
              {
                Id = "btnnext",
                Label = ">",
                Enabled = btnNextEnabled,
                Width = 80.0f
              },
            }
          }
        }
      };

      window = new NuiWindow
      {
        Root = root,
        Title = "Portrait démo",
        Geometry = new NuiRect(420.0f, 10.0f, 400.0f, 600.0f),
        Resizable = true,
        Collapsed = collapsed,
        Closable = true,
        Transparent = false,
        Border = true,
      };
    }
    public void CreateNewWindowForPlayer(NwPlayer player)
    {
      int token = player.CreateNuiWindow(MenuSystem.portraitDemo.window, "portrait_demo");

      int id = 164;

      string resRef = "po_" + Portraits2da.portraitsTable.GetDataEntry(id).resRef + "h";
      MenuSystem.portraitDemo.portraitId.SetBindValue(player, token, id.ToString());
      MenuSystem.portraitDemo.portraitResRef.SetBindValue(player, token, resRef);
      MenuSystem.portraitDemo.btnPrevEnabled.SetBindValue(player, token, false);
      MenuSystem.portraitDemo.btnNextEnabled.SetBindValue(player, token, true);
      MenuSystem.portraitDemo.portraitCategory.SetBindValue(player, token, 0);
      MenuSystem.portraitDemo.portraitCategory.SetBindWatch(player, token, true);
      MenuSystem.portraitDemo.collapsed.SetBindWatch(player, token, true);
    }
  }
}
