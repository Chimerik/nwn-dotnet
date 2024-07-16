using System;
using Anvil.API;


namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static void LearnSermentSpell(PlayerSystem.Player player, int spellId)
    {
      if (player.learnableSpells.TryGetValue(spellId, out var learnable))
      {
        learnable.learntFromClasses.Add(CustomClass.Paladin);
        learnable.paladinSerment = true;

        if (learnable.currentLevel < 1)
          learnable.LevelUp(player);
      }
      else
      {
        LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[spellId], CustomClass.Paladin) { paladinSerment = true };
        player.learnableSpells.Add(learnableSpell.id, learnableSpell);
        learnableSpell.LevelUp(player);
      }

      NwSpell spell = NwSpell.FromSpellId(spellId);
      int spellLevel = spell.GetSpellLevelForClass(ClassType.Paladin);

      try
      {
        player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).KnownSpells[spellLevel].Add(spell);
      }
      catch(Exception)
      {
        player.oid.SendServerMessage($"Attention - Sort de serment incorrectement configuré pour paladin : {spell.Name.ToString()}", ColorConstants.Red);
      }
    }
  }
}
