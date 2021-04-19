﻿using System.Collections.Generic;
using System.Linq;
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
    public CreateSkillbook(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string>() {
        "Quel don ?"
        };

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        await NwModule.Instance.WaitForObjectContext();
        HandleCreateSkillbook(player.setValue, player.oid);
        player.setValue = Config.invalidInput;
        player.menu.Close();
      });

      player.menu.choices.Add(("Retour", () => CommandSystem.DrawDMCommandList(player)));
      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void HandleCreateSkillbook(int skillId, NwPlayer oPC)
    {
      NwItem skillBook = NwItem.Create("skillbookgeneriq", oPC, 1, "skillbook");
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
    }
  }
}
