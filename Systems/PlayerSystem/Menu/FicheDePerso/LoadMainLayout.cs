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
        private readonly NuiGroup classGroup = new() { Id = "classGroup", Border = false };
        private readonly NuiRow classRow = new() { Margin = 0.0f, Children = new List<NuiElement>() };

        private readonly NuiBind<string> portrait = new("portrait");

        private readonly NuiBind<string> str = new("str");
        private readonly NuiBind<string> dex = new("dex");
        private readonly NuiBind<string> con = new("con");
        private readonly NuiBind<string> intel = new("intel");
        private readonly NuiBind<string> wis = new("wis");
        private readonly NuiBind<string> cha = new("cha");

        private readonly NuiBind<string> hp = new("hp");
        private readonly NuiBind<string> ac = new("ac");
        private readonly NuiBind<string> init = new("init");
        private readonly NuiBind<string> proficiency = new("proficiency");
        private readonly NuiBind<string> mainDamage = new("mainDamage");
        private readonly NuiBind<string> mainCriticalRange = new("mainCriticalRange");
        private readonly NuiBind<string> secondaryDamage = new("secondaryDamage");
        private readonly NuiBind<string> secondaryCriticalRange = new("secondaryCriticalRange");

        private readonly NuiBind<string> strSave = new("strSave");
        private readonly NuiBind<string> dexSave = new("dexSave");
        private readonly NuiBind<string> conSave = new("conSave");
        private readonly NuiBind<string> intSave = new("intSave");
        private readonly NuiBind<string> wisSave = new("wisSave");
        private readonly NuiBind<string> chaSave = new("chaSave");

        private void LoadMainLayout()
        {
          LoadTopMenuLayout();

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = windowWidth / 2.5f, Children = new List<NuiElement>() 
          {
            new NuiSpacer(),
            new NuiImage(portrait) { Aspect = 0.65f, ImageAspect = NuiAspect.Fill, Width = windowWidth / 4 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Classes") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          classRow.Height = windowWidth / 4;
          classGroup.Height = windowWidth / 4;
          classGroup.Layout = classRow;
          rootChildren.Add(classGroup);

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Caractéristiques") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() {  Margin = 0.0f, Height = windowWidth / 5, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>() 
              { 
                new NuiButtonImage("strength") { Id = "strRoll", Tooltip = "Force" },
                new NuiLabel(str) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiButtonImage("dexterity") { Id = "dexRoll", Tooltip = "Dextérité" },
                new NuiLabel(dex) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiButtonImage("constitution") { Id = "conRoll", Tooltip = "Constitution" },
                new NuiLabel(con) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiButtonImage("intelligence") { Id = "intRoll", Tooltip = "Intelligence" },
                new NuiLabel(intel) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiButtonImage("wisdom") { Id = "wisRoll", Tooltip = "Sagesse" },
                new NuiLabel(wis) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiButtonImage("charisma") { Id = "chaRoll", Tooltip = "Charisme" },
                new NuiLabel(cha) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Attributs") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Children = new List<NuiElement>()
          {
            new NuiColumn() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(hp) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 20 } }  },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(init) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 20 } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(mainDamage) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 20 } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(secondaryDamage) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 20 } } }
            } },
            new NuiColumn() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(ac) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 20 } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(proficiency) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 20 } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(mainCriticalRange) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 20 } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(secondaryCriticalRange) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 20 } } }
            } }
          } });

          rootChildren.Add(new NuiRow() { Margin = 5.0f, Height = windowWidth / 20, Width = windowWidth / 1.1f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiImage("menu_separator_l") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8,  VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center},
            new NuiSpacer(),
            new NuiLabel("Jets de Sauvegardes") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Width = windowWidth / 3 },
            new NuiSpacer(),
            new NuiImage("menu_separator_r") { ImageAspect = NuiAspect.Fit, Width = windowWidth / 8, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiSpacer()
          } });
          
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = windowWidth / 5, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("strength") { Id = "strSaveRoll", Tooltip = "Force" },
              new NuiLabel(strSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("dexterity") { Id = "dexSaveRoll", Tooltip = "Dextérité" },
              new NuiLabel(dexSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("constitution") { Id = "conSaveRoll", Tooltip = "Constitution" },
              new NuiLabel(conSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("intelligence") { Id = "intSaveRoll", Tooltip = "Intelligence" },
              new NuiLabel(intSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("wisdom") { Id = "wisSaveRoll", Tooltip = "Sagesse" },
              new NuiLabel(wisSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("charisma") { Id = "chaSaveRoll", Tooltip = "Charisme" },
              new NuiLabel(chaSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiSpacer()
          } });

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
      }
    }
  }
}
