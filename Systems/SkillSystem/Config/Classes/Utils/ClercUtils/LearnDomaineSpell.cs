using System;
using Anvil.API;


namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static void LearnDomaineSpell(PlayerSystem.Player player, int spellId)
    { 
      if (player.learnableSpells.TryGetValue(spellId, out var learnable))
      {
        learnable.learntFromClasses.Add(CustomClass.Clerc);
        learnable.clericDomain = true;

        if (learnable.currentLevel < 1)
          learnable.LevelUp(player);
      }
      else
      {
        LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[spellId], CustomClass.Clerc) { clericDomain = true };
        player.learnableSpells.Add(learnableSpell.id, learnableSpell);
        learnableSpell.LevelUp(player);
      }

      NwSpell spell = NwSpell.FromSpellId(spellId);
      int spellLevel = spell.GetSpellLevelForClass(ClassType.Cleric);

      try
      {
        player.oid.LoginCreature.GetClassInfo(ClassType.Cleric).KnownSpells[spellLevel].Add(spell);
      }
      catch (Exception)
      {
        player.oid.SendServerMessage($"Attention - Sort de domaine incorrectement configuré pour clerc : {spell.Name.ToString()}", ColorConstants.Red);
      }
    }
  }
}
