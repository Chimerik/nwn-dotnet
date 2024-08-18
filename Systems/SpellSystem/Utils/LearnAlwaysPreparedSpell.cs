using System;
using Anvil.API;


namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void LearnAlwaysPreparedSpell(PlayerSystem.Player player, int spellId, int classId)
    { 
      if (player.learnableSpells.TryGetValue(spellId, out var learnable))
      {
        learnable.learntFromClasses.Add(classId);
        learnable.alwaysPrepared = true;

        if (learnable.currentLevel < 1)
          learnable.LevelUp(player);
      }
      else
      {
        LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[spellId], classId) { alwaysPrepared = true };
        player.learnableSpells.Add(learnableSpell.id, learnableSpell);
        learnableSpell.LevelUp(player);
      }

      NwSpell spell = NwSpell.FromSpellId(spellId);
      int spellLevel = spell.GetSpellLevelForClass((ClassType)classId);

      try
      {
        player.oid.LoginCreature.GetClassInfo((ClassType)classId).KnownSpells[spellLevel].Add(spell);
      }
      catch (Exception)
      {
        player.oid.SendServerMessage($"Attention - Sort toujours préparé incorrectement configuré pour {(ClassType)classId} : {spell.Name.ToString()}", ColorConstants.Red);
      }
    }
  }
}
