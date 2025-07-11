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
        private readonly NuiBind<bool> chatimentLevelSelection = new("chatimentLevelSelection");
        private readonly NuiBind<string> touchLabel = new("touchLabel");
        private readonly NuiBind<bool> touch = new("touch");
        private readonly NuiBind<string> walkLabel = new("walkLabel");
        private readonly NuiBind<bool> walk = new("walk");
        private readonly NuiBind<bool> instantLearn = new("instantLearn");
        private readonly NuiBind<string> instantLearnLabel = new("instantLearnLabel");

        private void LoadConfigLayout()
        {
          LoadTopMenuLayout();

          if (Utils.In(player.oid.LoginCreature.Area?.Tag, "Alphazone01", "Alphazone02"))
            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Sacrifier 20 % de points vie (zone de test)") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "sacrificeHP", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

          if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChatimentDivin))
            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Châtiment Divin - Niveau de sort") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", chatimentLevelSelection) { Id = "chatimentLevelSelection", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

          if(player.learnableSkills.Any(l => l.Value.category == SkillSystem.Category.Language))
            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Ouvrir la fenêtre de sélection des langues") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "langue", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("S'asseoir n'importe où") { Tooltip = "Permet de s'asseoir partout. Attention, seule la position affichée change. La position réelle du personnage reste la même", Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "sit", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel(touchLabel) { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", touch) { Id = "touch", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel(walkLabel) { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", walk) { Id = "walk", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Suivre la créature ciblée") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "suivre", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          if (AreaSystem.areaDescriptions.ContainsKey(player.oid.ControlledCreature?.Area.Name))
            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Examiner les environs") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "examineArea", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Gestion des barres de raccourcis") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "quickbars", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          if (player.bonusRolePlay > 3)
            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Recommander un joueur") { Tooltip = "Recommander un joueur pour la qualité de son roleplay et son implication sur le module", Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "commend", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Gestion des apparences d'objets") { Tooltip = "Enregistrer ou charger une apparence d'objet", Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "itemAppearance", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Gestion des couleurs du chat") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "chat", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Tentative de déblocage du décor") { Tooltip = "Succès non garanti", Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "unstuck", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Réinitialiser la position affichée") { Tooltip = "A utiliser en cas de problème avec le système d'assise", Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "reinitPositionDisplay", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Afficher ma clé publique") { Tooltip = "Permet d'obtenir la clé publique de votre compte, utile pour lier le compte Discord au compte Never", Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "publicKey", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiLabel("Supprimer ce personnage") { Tooltip = "Ce personnage n'apparaîtra plus dans votre liste, mais pourra être réactivé sur demande", Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
            new NuiCheck("", false) { Id = "delete", Height = windowHeight / 24, Width = windowHeight / 24 },
            new NuiSpacer()
          } });

          if(player.oid.IsDM || Utils.In(player.oid.LoginCreature.Area?.Tag, "Alphazone01", "Alphazone02"))
          { 
            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Gérer mes effets visuels (test)") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "visualEffects", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel("Gérer mes effets visuels en AOE (test)") { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", false) { Id = "aoeVisualEffects", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });

            rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiLabel(instantLearnLabel) { Height = windowHeight / 24, Width = windowWidth / 1.25f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
              new NuiCheck("", instantLearn) { Id = "instantLearn", Height = windowHeight / 24, Width = windowHeight / 24 },
              new NuiSpacer()
            } });
          }

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
      }
    }
  }
}
