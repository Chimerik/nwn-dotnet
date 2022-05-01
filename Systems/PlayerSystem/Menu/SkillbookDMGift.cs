using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class SkillBookDMGiftWindow : PlayerWindow
      {
        private readonly NuiColumn rootRow = new ();
        private readonly List<NuiElement> rootChildren = new ();
        private readonly List<NuiListTemplateCell> rowTemplate = new ();

        private readonly NuiBind<string> skillbookNames = new ("skillbookNames");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly NuiBind<string> skillbookIcon = new ("skillbookIcon");
        private readonly NuiBind<string> search = new ("search");

        public List<Learnable> currentList;
        Player targetPlayer;

        public SkillBookDMGiftWindow(Player player, Player targetPlayer) : base(player)
        {
          windowId = "skillbookDMGift";

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(skillbookIcon) { Id = "give", Tooltip = skillbookNames, Height = 35 }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(skillbookNames) { VerticalAlign = NuiVAlign.Middle }));

          rootRow.Children = rootChildren;
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 320 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow(targetPlayer);
        }
        public void CreateWindow(Player target)
        {
          this.targetPlayer = target;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 350, 650);

          window = new NuiWindow(rootRow, $"Don de skillbook : {target.oid.LoginCreature.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleSkillbookGiftEvents;
          player.oid.OnNuiEvent += HandleSkillbookGiftEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          search.SetBindValue(player.oid, token, "");
          search.SetBindWatch(player.oid, token, true);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          currentList = SkillSystem.learnableDictionary.Values.Where(s => s is LearnableSkill skill && skill.category != SkillSystem.Category.StartingTraits).ToList();
          LoadSkillbookList(currentList);
        }

        private void HandleSkillbookGiftEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "give":

                  if (targetPlayer.oid.LoginCreature == null)
                  {
                    CloseWindow();
                    player.oid.SendServerMessage("La cible du don n'est plus valide.", ColorConstants.Red);
                    return;
                  }

                  NwItem skillBook = NwItem.Create("skillbookgeneriq", player.oid.ControlledCreature.Location);
                  skillBook.Tag = "skillbook";
                  ItemUtils.CreateShopSkillBook(skillBook, currentList[nuiEvent.ArrayIndex].id);
                  targetPlayer.oid.LoginCreature.AcquireItem(skillBook);

                  player.oid.SendServerMessage($"Don du skillbook {skillBook.Name} à {targetPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} terminé avec succès !", new Color(32, 255, 32));
                  targetPlayer.oid.SendServerMessage($"{player.oid.LoginCreature.Name.ColorString(ColorConstants.White)} vient de vous faire don du livre de compétence {skillBook.Name}.", new Color(32, 255, 32));

                  CloseWindow();

                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":

                  string currentSearch = search.GetBindValue(player.oid, token).ToLower();
                  currentList = string.IsNullOrEmpty(currentSearch) ? SkillSystem.learnableDictionary.Values.Where(s => s is LearnableSkill skill && skill.category != SkillSystem.Category.StartingTraits).ToList() : SkillSystem.learnableDictionary.Values.Where(s => s is LearnableSkill skill && skill.category != SkillSystem.Category.StartingTraits).Where(s => s.name.ToLower().Contains(currentSearch)).ToList();
                  LoadSkillbookList(currentList);

                  break;
              }

              break;
          }
        }
        private void LoadSkillbookList(List<Learnable> skillList)
        {
          List<string> skillbookNameList = new List<string>();
          List<string> skillbookIconList = new List<string>();

          foreach (LearnableSkill skillBook in skillList)
          {
            skillbookNameList.Add(skillBook.name);
            skillbookIconList.Add(skillBook.icon);
          }

          skillbookNames.SetBindValues(player.oid, token, skillbookNameList);
          skillbookIcon.SetBindValues(player.oid, token, skillbookIconList);
          listCount.SetBindValue(player.oid, token, skillbookNameList.Count());
        }
      }
    }
  }
}
