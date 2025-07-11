using System.Collections.Generic;
using System.Linq;
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

          rootColumn.Width = windowWidth;
          rootGroup.Width = rootColumn.Width;

          classRow.Height = windowWidth / 4;
          classRow.Width = windowWidth / 1.1f;
          classGroup.Height = classRow.Height;
          classGroup.Layout = classRow;

          menuRow.Height = windowWidth / 10;
          menuGroup.Height = menuRow.Height;

          menuRow.Children.Clear();

          menuRow.Children.Add(new NuiSpacer());
          menuRow.Children.Add(new NuiButtonImage("sheet_main_view") { Id = "sheetMainView", Height = windowWidth / 12, Width = windowWidth / 12 });

          if (!Utils.In(target.Area?.Tag, "entry_scene_in", "entry_scene_out"))
            menuRow.Children.Add(new NuiButtonImage("sheet_learnables") { Id = "sheetLearnables", Height = windowWidth / 12, Width = windowWidth / 12 });

          menuRow.Children.Add(new NuiButtonImage("sheet_conditions") { Id = "sheetConditions", Height = windowWidth / 12, Width = windowWidth / 12 });

          if (target.Classes.Any(c => c.Class.IsSpellCaster))
            menuRow.Children.Add(new NuiButtonImage("sheet_spellbook") { Id = "sheetSpellbook", Height = windowWidth / 12, Width = windowWidth / 12 });
          
          menuRow.Children.Add(new NuiButtonImage("sheet_skills") { Id = "sheetSkills", Height = windowWidth / 12, Width = windowWidth / 12 });
          menuRow.Children.Add(new NuiButtonImage("sheet_weapons") { Id = "sheetWeapons", Height = windowWidth / 12, Width = windowWidth / 12 });
          menuRow.Children.Add(new NuiButtonImage("sheet_desc") { Id = "sheetDescription", Height = windowWidth / 12, Width = windowWidth / 12 });

          if (targetPlayer.subscriptions.Any(s => s.type == Utils.SubscriptionType.MailDistantAccess))
            menuRow.Children.Add(new NuiButtonImage("ir_animalemp") { Id = "sheetMailBox", Height = windowWidth / 12, Width = windowWidth / 12 });
         
          menuRow.Children.Add(new NuiButtonImage("sheet_config") { Id = "sheetConfig", Height = windowWidth / 12, Width = windowWidth / 12 });

          if (player.oid.IsDM || Utils.In(player.oid.PlayerName, "Chim",  "dodozik", "WingsOfJoy"))
            menuRow.Children.Add(new NuiButtonImage("ife_possess") { Id = "sheetDmTools", Height = windowWidth / 12, Width = windowWidth / 12 });
         
          menuRow.Children.Add(new NuiSpacer());

          conditionsTemplate.Clear();
          conditionsTemplate.Add(new(new NuiButtonImage(conditionIcon) { }) { Width = windowWidth / 12 });
          conditionsTemplate.Add(new(new NuiSpacer()));
          conditionsTemplate.Add(new(new NuiLabel(conditionName) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Tooltip = conditionName }) { Width = windowWidth / 1.5f });
          conditionsTemplate.Add(new(new NuiSpacer()));

          descriptionsTemplate.Clear();
          descriptionsTemplate.Add(new(new NuiLabel(descriptionTitles) { Id = "selectDescription", VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Left, Tooltip = "Selectionner" }) { Width = windowWidth / 1.5f });
          descriptionsTemplate.Add(new(new NuiSpacer()));
          descriptionsTemplate.Add(new(new NuiButtonImage("ir_ban") { Id = "deleteDescription", Height = windowWidth / 12 }) { Width = windowWidth / 12 });
        }
      }
    }
  }
}
