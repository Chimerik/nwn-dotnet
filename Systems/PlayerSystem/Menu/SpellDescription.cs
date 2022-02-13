using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class SpellDescriptionWindow : PlayerWindow
      {
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        List<NuiElement> rootChidren { get; }

        public SpellDescriptionWindow(Player player, int spellId) : base(player)
        {
          windowId = "spellDescription";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "spellGroup", Border = true, Layout = rootColumn };

          CreateWindow(spellId);
        }
        public void CreateWindow(int spellId)
        {
          rootChidren.Clear();

          NwSpell spell = NwSpell.FromSpellId(spellId);

          NuiRow row = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButtonImage(spell.IconResRef) { Height = 40, Width = 40 },
              new NuiSpacer()
            }
          };

          rootChidren.Add(row);

          row = new NuiRow() { Children = new List<NuiElement>() { new NuiText(spell.Description) } };
          rootChidren.Add(row);

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 500, 300);

          window = new NuiWindow(rootGroup, spell.Name)
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          token = player.oid.CreateNuiWindow(window, windowId);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
      }
    }
  }
}
