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
        private readonly NuiBind<string> dmModeLabel = new("dmModeLabel");
        private readonly NuiBind<bool> dmMode = new("dmMode");
        private readonly NuiBind<bool> instantCraft = new("instantCraft");

        private void LoadDmToolsLayout()
        {
          LoadTopMenuLayout();

          if(Utils.In(player.oid.PlayerName, "Chim", "dodozik", "WingsOfJoy"))
            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel(dmModeLabel) { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", dmMode) { Id = "dmMode", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

          if(Utils.In(player.oid.PlayerName, "Chim"))
            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Ajout de classe (test Chim)") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "addClass", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Palette des créatures") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "creaturePalette", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Palette des objets") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "itemPalette", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Palette des placeables") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "placeablePalette", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Gérer les placeables de la zone") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "placeableManager", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Gestion du vent") { Tooltip = "Permet de modifier la configuration du vent de cette zone", Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "wind", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Modifier le nom de la cible") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "dmRename", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Modifier la sélection musicale de la zone") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "areaMusicEditor", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Modifier l'écran de chargement de la zone") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "areaLoadScreenEditor", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          if (Utils.In(player.oid.PlayerName, "Chim", "dodozik", "WingsOfJoy"))
          {
            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Reboot") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "reboot", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Refill ressources") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "refill", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Craft Instantanné") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", instantCraft) { Id = "instantCraft", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Don de ressources") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "giveResources", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Don de skillbook") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "giveSkillbook", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });
          }

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
      }
    }
  }
}
