using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateQuickLootWindow(NwItem oItem)
      {
        NuiBind<string> icon = new NuiBind<string>("icon");
        NuiBind<string> itemName = new NuiBind<string>("itemName");
        NuiBind<int> channel = new NuiBind<int>("channel");
        NuiBind<bool> closable = new NuiBind<bool>("closable");
        NuiBind<bool> resizable = new NuiBind<bool>("resizable");
        NuiBind<bool> makeStatic = new NuiBind<bool>("static");
        NuiBind<uint> item = new NuiBind<uint>("item");
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey("chat") ? windowRectangles["chat"] : new NuiRect(420.0f, 10.0f, 600.0f, 400.0f);

        // Construct the window layout.
        NuiCol root = new NuiCol
        {
          Children = new List<NuiElement>
          {
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
                      Id = "examine",
                      ResRef = icon,
                      Height = 50,
                      Width = 50,
                      Tooltip = "Description"
                    },
                    new NuiLabel
                    {
                      Value = itemName,
                    },
                    new NuiButton
                    {
                      Id = "take",
                      Label = "Prendre",
                      Height = 50
                    },
                    new NuiButton
                    {
                      Id = "steal",
                      Label = "Voler",
                      Height = 50
                    },
                    new NuiButtonImage
                    {
                      Id = "ignore",
                      ResRef = "menu_exit",
                      Height = 50,
                      Width = 50,
                      Tooltip = "Ignorer"
                    },
                  }
                },
              }
            }
          }
        };

        NuiWindow window = new NuiWindow
        {
          Root = root,
          Geometry = geometry,
          Resizable = resizable,
          Collapsed = false,
          Closable = closable,
          Transparent = true,
          Border = false,
        };

        int token = oid.CreateNuiWindow(window, "quickLoot");

        if(oItem.IsValid && oItem.GetObjectVariable<LocalVariableBool>($"{oid.PlayerName}_IGNORE_QUICKLOOT").HasNothing)
        {
          icon.SetBindValue(oid, token, Utils.Util_GetIconResref(oItem));
          itemName.SetBindValue(oid, token, oItem.Name);
          item.SetBindValue(oid, token, oItem);
        }
        /* ex pour complexe icon resref
         * json jImage = NuiImage(NuiBind("image_1"), JsonInt(NUI_ASPECT_EXACTSCALED), JsonInt(NUI_HALIGN_CENTER), JsonInt(NUI_VALIGN_TOP));
        json jRect = NuiRect(0.0f, 0.0f, 64.0f, 192.0f);
        json jDrawList = JsonArray();
            jDrawList = JsonArrayInsert(jDrawList, NuiDrawListImage(NuiBind("complex_item"), NuiBind("image_2"), jRect, JsonInt(NUI_ASPECT_EXACTSCALED), JsonInt(NUI_HALIGN_CENTER), JsonInt(NUI_VALIGN_TOP)));
            jDrawList = JsonArrayInsert(jDrawList, NuiDrawListImage(NuiBind("complex_item"), NuiBind("image_3"), jRect, JsonInt(NUI_ASPECT_EXACTSCALED), JsonInt(NUI_HALIGN_CENTER), JsonInt(NUI_VALIGN_TOP)));
        jImage = NuiDrawList(jImage, JsonBool(TRUE), jDrawList);
        jImage = NuiWidth(jImage, 64.0f);
        jImage = NuiHeight(jImage, 192.0f);
        */ 

        resizable.SetBindValue(oid, token, true);
        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }
    }
  }
}
