using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private readonly NuiGroup menuGroup = new() { Id = "menuGroup", Border = true };
        private readonly NuiRow menuRow = new() { Margin = 0.0f, Children = new List<NuiElement>() };

        private void LoadTopMenuLayout()
        {
          rootChildren.Clear();
          rootChildren.Add(menuGroup);
        }
      }
    }
  }
}
