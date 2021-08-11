using System.Collections.Generic;
using NLog;
using Anvil.API;
using Anvil.Services;
using Utils;

namespace NWN.Systems
{
  [ServiceBinding(typeof(SkillSystem))]
  public class SkillSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();

    public SkillSystem()
    {
      //TempLearnablesJsonification();
    }
    private void TempLearnablesJsonification()
    {
      List<int> charIds = new List<int>();

      var result = SqLiteUtils.SelectQuery("playerCharacters",
          new List<string>() { { "ROWID" } },
          new List<string[]>() { });

      foreach (var characterId in result.Results)
        charIds.Add(characterId.GetInt(0));

      foreach (int charId in charIds)
      {
        Dictionary<string, Learnable> tempDic = new Dictionary<string, Learnable>();
        
        var featResult = SqLiteUtils.SelectQuery("playerLearnableSkills",
          new List<string>() { { "skillId" }, { "skillPoints" }, { "trained" }, { "active" } },
          new List<string[]>() { { new string[] { "characterId", charId.ToString() } } });

        foreach (var skill in featResult.Results)
        {
          int id = skill.GetInt(0);
          Feat skillId = (Feat)id;
          int currentSkillPoints = skill.GetInt(1);
          bool active = skill.GetInt(3) == 1 ? true : false;

          if (skill.GetInt(2) == 0)
            tempDic.Add($"F{id}", new Learnable(LearnableType.Feat, id, currentSkillPoints, active));
        }

        var spellResult = SqLiteUtils.SelectQuery("playerLearnableSpells",
          new List<string>() { { "skillId" }, { "skillPoints" }, { "nbScrolls" }, { "active" } },
          new List<string[]>() { { new string[] { "characterId", charId.ToString().ToString() } }, { new string[] { "trained", "0" } } });

        foreach (var spell in spellResult.Results)
        {
          bool active = spell.GetInt(3) == 1 ? true : false;
          tempDic.Add($"S{spell.GetInt(0)}", new Learnable(LearnableType.Spell, spell.GetInt(0), spell.GetInt(1), active, spell.GetInt(2)));
        }

        SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "serializedLearnables", Newtonsoft.Json.JsonConvert.SerializeObject(tempDic) } },
          new List<string[]>() { new string[] { "rowid", charId.ToString() } });
      }
    }
  }
}
