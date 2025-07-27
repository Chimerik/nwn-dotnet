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
        private readonly NuiBind<string> learnableIcon = new("learnableIcon");
        private readonly NuiBind<NuiRect> learnableDrawListRect = new("learnableDrawListRect");
        private readonly NuiBind<string> learnableName = new("learnableName");
        public readonly NuiBind<string> learnableETA = new("learnableETA");
        public readonly NuiBind<string> learnableLevel = new("learnableLevel");

        private readonly NuiBind<string> jobIcon = new("jobIcon");
        private readonly NuiBind<NuiRect> jobDrawListRect = new("jobDrawListRect");
        private readonly NuiBind<string> jobName = new("jobName");
        public readonly NuiBind<string> jobETA = new("jobETA");

        private void LoadLearnablesLayout()
        {
          LoadTopMenuLayout();

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Apprentissage") { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
              new NuiButtonImage(learnableIcon) { Width = windowWidth / 12, Height = windowWidth / 12 },
              new NuiLabel(learnableName) { Tooltip = learnableName, Width = windowWidth * (8 / 12), Height = windowWidth / 24, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
              new NuiDrawListText(StringUtils.white, learnableDrawListRect, learnableETA) } },
              new NuiLabel("Niveau/Max") { Width = windowWidth / 5, Height = windowWidth / 24, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
              new NuiDrawListText(StringUtils.white, learnableDrawListRect, learnableLevel) } }
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Travail Artisanal") { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
              new NuiButtonImage(jobIcon) { Id = "examineJobItem", Width = windowWidth / 12, Height = windowWidth / 12 },
              new NuiLabel(jobName) { Tooltip = jobName, Width = windowWidth * (8 / 12), Height = windowWidth / 24, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
              new NuiDrawListText(StringUtils.white, jobDrawListRect, jobETA) } },
              new NuiButtonImage("ir_abort") { Id = "cancelJob", Tooltip = "En cas d'annulation, le travail en cours sera perdu. L'objet d'origine vous sera restitué.", Width = windowWidth / 12, Height = windowWidth / 12 },
              new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiButton("Ouvrir le journal") { Id = "learnables", Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
      }
    }
  }
}
