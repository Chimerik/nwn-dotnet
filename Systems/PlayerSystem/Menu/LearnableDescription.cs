using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class LearnableDescriptionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> name = new("name");
        private readonly NuiBind<string> description = new("description");
        private readonly NuiBind<string> primaryAbilityIcon = new("primaryAbilityIcon");
        private readonly NuiBind<string> secondaryAbilityIcon = new("secondaryAbilityICon");
        private readonly NuiBind<string> primaryAbility = new("primaryAbility");
        private readonly NuiBind<string> secondaryAbility = new("secondaryAbility");
        private readonly NuiBind<bool> enabled = new("enabled");

        private Learnable learnable { get; set; }

        public LearnableDescriptionWindow(Player player, int learnableId) : base(player)
        {
          windowId = "learnableDescription";
          rootColumn.Children = rootChidren;

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() {
            new NuiSpacer(),
            new NuiButtonImage(icon) { Id = "inscription", Tooltip = name /*"Sélectionner un objet sur lequel calligraphier cette inscription"*/, Height = 35, Width = 35, Enabled = enabled },
            new NuiSpacer()
          }});

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() {
            new NuiSpacer(),
            new NuiButtonImage(primaryAbilityIcon) { Tooltip = primaryAbility, Height = 35, Width = 35 },
            new NuiButtonImage(secondaryAbilityIcon) { Tooltip = secondaryAbility, Height = 35, Width = 35 },
            new NuiSpacer()
          } });

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiText(description) } });

          window = new NuiWindow(rootColumn, name)
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          CreateWindow(learnableId);
        }
        public void CreateWindow(int learnableId)
        {
          CloseWindow();

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            //nuiToken.OnNuiEvent += HandleInscriptionEvents;

            learnable = SkillSystem.learnableDictionary[learnableId];

            icon.SetBindValue(player.oid, nuiToken.Token, learnable.icon);
            description.SetBindValue(player.oid, nuiToken.Token, learnable.description);
            name.SetBindValue(player.oid, nuiToken.Token, learnable.name);
            primaryAbilityIcon.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetAttributeIcon(learnable.primaryAbility));
            secondaryAbilityIcon.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetAttributeIcon(learnable.secondaryAbility));
            primaryAbility.SetBindValue(player.oid, nuiToken.Token, $"Attribut principal : {StringUtils.TranslateAttributeToFrench(learnable.primaryAbility)}");
            secondaryAbility.SetBindValue(player.oid, nuiToken.Token, $"Attribut secondaire : {StringUtils.TranslateAttributeToFrench(learnable.secondaryAbility)}");

            enabled.SetBindValue(player.oid, nuiToken.Token, player.learnableSkills.ContainsKey(learnableId) && player.learnableSkills[learnableId].totalPoints > 0);

            geometry.SetBindValue(player.oid, nuiToken.Token, player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2 - 500, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) /2 - 300, 500, 300));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleInscriptionEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if (nuiEvent.ElementId == "inscription")
              {
                player.oid.SendServerMessage("Sélectionnez l'objet sur lequel vous souhaitez inscrire cette calligraphie.", ColorConstants.Orange);
                player.oid.EnterTargetMode(SelectInventoryItem, Config.selectItemTargetMode);
              }
              break;
          }
        }
        private async void SelectInventoryItem(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject is not NwItem item || item == null || !item.IsValid || item.Possessor != player.oid.LoginCreature)
            return;

          player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CASTING_INSCRIPTION").Value = learnable.id;

          await player.oid.LoginCreature.ActionCastSpellAt(NwSpell.FromSpellId(840), item, MetaMagic.None, true);

          LogUtils.LogMessage($"{player.oid.LoginCreature.Name} ({player.oid.PlayerName}) calligraphie {item.Name} avec l'inscription {learnable.name}", LogUtils.LogType.Craft);
        }
      }
    }
  }
}
