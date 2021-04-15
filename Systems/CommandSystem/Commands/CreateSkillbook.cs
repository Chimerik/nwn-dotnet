using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class CreateSkillbook
  {
    PlayerSystem.Player dm;
    public CreateSkillbook(PlayerSystem.Player player)
    {
      dm = player;
      dm.menu.Clear();
      dm.menu.titleLines = new List<string>() {
        "Quel don ?"
        };

      Task playerInput = NwTask.Run(async () =>
      {
        dm.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        dm.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => dm.setValue != Config.invalidInput);

        HandleCreateSkillbook(dm.setValue);
        dm.setValue = Config.invalidInput;
      });

      dm.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(dm)));
      dm.menu.choices.Add(("Quitter", () => dm.menu.Close()));
      dm.menu.Draw();
    }
    private void HandleCreateSkillbook(int skillId)
    {
      NwItem skillBook = NwItem.Create("skillbookgeneriq", dm.oid, 1, "skillbook");
      ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, Utils.random.Next(0, 50));
      skillBook.GetLocalVariable<int>("_SKILL_ID").Value = skillId;

      Feat feat = (Feat)skillId;

      if (SkillSystem.customFeatsDictionnary.ContainsKey(feat))
      {
        skillBook.Name = SkillSystem.customFeatsDictionnary[feat].name;
        skillBook.Description = SkillSystem.customFeatsDictionnary[feat].description;
      }
      else
      {
        if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", skillId), out int nameValue))
          skillBook.Name = NWScript.GetStringByStrRef(nameValue);

        if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", skillId), out int descriptionValue))
          skillBook.Description = NWScript.GetStringByStrRef(descriptionValue);
      }

      dm.menu.Close();
    }
  }
}
