using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class LearnableDescriptionWindow : PlayerWindow
      {
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        List<NuiElement> rootChidren { get; }

        public LearnableDescriptionWindow(Player player, int learnableId) : base(player)
        {
          windowId = "learnableDescription";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "learnableGroup", Border = true, Layout = rootColumn };

          CreateWindow(learnableId);
        }
        public void CreateWindow(int learnableId)
        {
          rootChidren.Clear();

          Learnable learnable = SkillSystem.learnableDictionary[learnableId];

          NuiRow row = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButtonImage(learnable.icon) { Height = 40, Width = 40 },
              new NuiSpacer()
            }
          };

          rootChidren.Add(row);

          row = new NuiRow() { Children = new List<NuiElement>() { new NuiText(learnable.description)} };
          rootChidren.Add(row);

          row = new NuiRow()
          {
            Height = 30, 
            Children = new List<NuiElement>()
            {
                new NuiLabel("Attribut principal : " + StringUtils.TranslateAttributeToFrench(learnable.primaryAbility)) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                new NuiLabel("Attribut secondaire : " + StringUtils.TranslateAttributeToFrench(learnable.secondaryAbility)) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
            }
          };

          rootChidren.Add(row);

          if(learnable is LearnableSkill learnableSkill)
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
          }

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 500, 300);

          window = new NuiWindow(rootGroup, learnable.name)
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          token = player.oid.CreateNuiWindow(window, windowId);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
      }
    }
  }
}
