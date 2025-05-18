using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private readonly NuiBind<string> title = new("title");
        private readonly NuiBind<string> description = new("description");

        private readonly NuiBind<int> descriptionsListCount = new("descriptionsListCount");
        private readonly NuiBind<string> descriptionTitles = new("descriptionTitles");

        private readonly List<NuiListTemplateCell> descriptionsTemplate = new();

        private void LoadDescriptionLayout()
        {
          LoadTopMenuLayout();

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiTextEdit("Titre", title, 100, false) { Height = windowHeight / 20 }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiTextEdit("Description", description, 5000, true) { Height = windowHeight * 0.6f }
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ir_examine") { Id = "applyDescription", Tooltip = "Applique la description au personnage actuellement contrôlé.", Height = windowWidth / 12, Width = windowWidth / 12 },
            new NuiSpacer(),
            new NuiButtonImage("ir_empytqs") { Id = "saveDescription", Height = windowWidth / 12, Width = windowWidth / 12 },
            new NuiSpacer(),
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiList(descriptionsTemplate, descriptionsListCount) { RowHeight = windowWidth / 12 } } });

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
      }
    }
  }
}
