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
        private readonly NuiGroup resistanceGroup = new() { Id = "resistanceGroup", Border = true };
        private readonly NuiRow resistanceRow = new() { Margin = 5.0f, Padding = 5.0f, Children = new List<NuiElement>() };

        private readonly NuiBind<int> conditionsListCount = new("conditionsListCount");
        private readonly NuiBind<string> conditionIcon = new("conditionIcon");
        private readonly NuiBind<string> conditionName = new("conditionName");

        private readonly List<NuiListTemplateCell> conditionsTemplate = new();

        private void LoadConditionsLayout()
        {
          LoadTopMenuLayout();

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Résistances") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          resistanceRow.Height = windowWidth / 10;
          resistanceGroup.Height = resistanceRow.Height;
          resistanceGroup.Layout = resistanceRow;
          rootChildren.Add(resistanceGroup);          
          
          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Conditions") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiList(conditionsTemplate, conditionsListCount) { RowHeight = windowWidth / 12 } } });

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
      }
    }
  }
}
