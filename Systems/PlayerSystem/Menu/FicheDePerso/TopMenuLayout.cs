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

          windowWidth = player.guiScaledWidth * 0.3f;
          windowHeight = player.guiScaledHeight * 0.9f;

          classRow.Height = windowWidth / 4;
          classRow.Width = windowWidth / 1.1f;
          classGroup.Height = classRow.Height;
          classGroup.Layout = classRow;

          menuRow.Height = windowWidth / 10;
          menuGroup.Height = menuRow.Height;

          menuRow.Children.Clear();

          menuRow.Children.Add(new NuiSpacer());
          menuRow.Children.Add(new NuiButtonImage("sheet_main_view") { Id = "sheetMainView", Height = windowWidth / 12, Width = windowWidth / 12 });
          menuRow.Children.Add(new NuiButtonImage("sheet_conditions") { Id = "sheetConditions", Height = windowWidth / 12, Width = windowWidth / 12 });
          menuRow.Children.Add(new NuiButtonImage("sheet_skills") { Id = "sheetSkills", Height = windowWidth / 12, Width = windowWidth / 12 });
          menuRow.Children.Add(new NuiButtonImage("sheet_weapons") { Id = "sheetWeapons", Height = windowWidth / 12, Width = windowWidth / 12 });
          menuRow.Children.Add(new NuiSpacer());

          conditionsTemplate.Clear();
          conditionsTemplate.Add(new(new NuiButtonImage(conditionIcon) { }) { Width = windowWidth / 12 });
          conditionsTemplate.Add(new(new NuiSpacer()));
          conditionsTemplate.Add(new(new NuiLabel(conditionName) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Tooltip = conditionName }) { Width = windowWidth / 1.5f });
          conditionsTemplate.Add(new(new NuiSpacer()));
        }
      }
    }
  }
}
