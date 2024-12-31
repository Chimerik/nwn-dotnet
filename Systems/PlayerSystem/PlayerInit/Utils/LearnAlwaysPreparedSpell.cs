using Anvil.API;
using System;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void LearnAlwaysPreparedSpell(int spellId, int classId)
      {
        if (learnableSpells.TryGetValue(spellId, out var learnable))
        {
          learnable.learntFromClasses.Add(classId);
          learnable.alwaysPrepared = true;

          if (learnable.currentLevel < 1)
            learnable.LevelUp(this);
        }
        else
        {
          LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[spellId], classId) { alwaysPrepared = true };
          learnableSpells.Add(learnableSpell.id, learnableSpell);
          learnableSpell.LevelUp(this);
        }

        if (Utils.In(classId, CustomClass.Clerc, CustomClass.Druid, CustomClass.Paladin))
        {
          NwSpell spell = NwSpell.FromSpellId(spellId);
          int spellLevel = spell.GetSpellLevelForClass((ClassType)classId);
          var knownSpells = oid.LoginCreature.GetClassInfo((ClassType)classId).KnownSpells[spellLevel];

          if (!knownSpells.Contains(spell))
          {
            try
            {
              knownSpells.Add(spell);
            }
            catch (Exception)
            {
              oid.SendServerMessage($"Attention - Sort toujours préparé incorrectement configuré pour {(ClassType)classId} : {spell.Name.ToString()}", ColorConstants.Red);
            }
          }
        }
      }
    }
  }
}
