using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Anvil.API;

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

        public LearnableDescriptionWindow(Player player, int learnableId) : base(player)
        {
          windowId = "learnableDescription";
          rootColumn.Children = rootChidren;

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() {
            new NuiSpacer(),
            new NuiButtonImage(icon) { Tooltip = name, Height = 35, Width = 35 },
            new NuiSpacer()
          }});

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>() {
            new NuiSpacer(),
            new NuiButtonImage(primaryAbilityIcon) { Tooltip = primaryAbility, Height = 35, Width = 35 },
            new NuiButtonImage(secondaryAbilityIcon) { Tooltip = secondaryAbility, Height = 35, Width = 35 },
            new NuiSpacer()
          }
          });

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

          CreateWindow(learnableId, 1000);
        }
        public void CreateWindow(int learnableId, int delay = 0)
        {
          CloseWindow();

          /*if(learnable is LearnableSkill learnableSkill)
          {
            if(learnableSkill.attackBonusPrerequisite > 0)
            {
              row = new NuiRow()
              {
                Height = 25,
                Children = new List<NuiElement>()
                {
                    new NuiText("Bonus d'attaque de base minimum : " + learnableSkill.attackBonusPrerequisite),
                }
              };

              rootChidren.Add(row);
            }

            if (learnableSkill.abilityPrerequisites.Count > 0)
            {
              List<NuiElement> abilityPrereqChildre = new List<NuiElement>();
              row = new NuiRow() { Height = 25, Children = abilityPrereqChildre };

              foreach(var abilityPreReq in learnableSkill.abilityPrerequisites)
                abilityPrereqChildre.Add(new NuiText(StringUtils.TranslateAttributeToFrench(abilityPreReq.Key) + " de base minimum : " + abilityPreReq.Value));

              rootChidren.Add(row);
            }

            if (learnableSkill.skillPrerequisites.Count > 0)
            {
              List<NuiElement> skillPrereqChildre = new List<NuiElement>();
              row = new NuiRow() { Height = 25, Children = skillPrereqChildre };

              foreach (var skillPreReq in learnableSkill.skillPrerequisites)
                skillPrereqChildre.Add(new NuiText($"{SkillSystem.learnableDictionary[skillPreReq.Key].name} niveau {skillPreReq.Value} minimum"));

              rootChidren.Add(row);
            }
          }*/

          Task wait = NwTask.Run(async () =>
          {
            int id = learnableId;
            int waitingTime = delay;

            await NwTask.NextFrame();
            //await NwTask.Delay(TimeSpan.FromMilliseconds(0));

            if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
            {
              nuiToken = tempToken;

              Learnable learnable = SkillSystem.learnableDictionary[id];

              icon.SetBindValue(player.oid, nuiToken.Token, learnable.icon);
              description.SetBindValue(player.oid, nuiToken.Token, learnable.description);
              name.SetBindValue(player.oid, nuiToken.Token, learnable.name);
              primaryAbilityIcon.SetBindValue(player.oid, nuiToken.Token,StringUtils.GetAttributeIcon(learnable.primaryAbility));
              secondaryAbilityIcon.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetAttributeIcon(learnable.secondaryAbility));
              primaryAbility.SetBindValue(player.oid, nuiToken.Token, $"Attribut principal : {StringUtils.TranslateAttributeToFrench(learnable.primaryAbility)}");
              secondaryAbility.SetBindValue(player.oid, nuiToken.Token, $"Attribut secondaire : {StringUtils.TranslateAttributeToFrench(learnable.secondaryAbility)}");

              geometry.SetBindValue(player.oid, nuiToken.Token, player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2 - 500, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) /2 - 300, 500, 300));
              geometry.SetBindWatch(player.oid, nuiToken.Token, true);

              player.openedWindows[windowId] = nuiToken.Token;
            }
            else
              player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
          });
        }
      }
    }
  }
}
