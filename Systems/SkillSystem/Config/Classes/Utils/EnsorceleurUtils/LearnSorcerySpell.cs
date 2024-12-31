using System;
using System.Security.Cryptography;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static void LearnSorcerySpell(PlayerSystem.Player player, int spellId)
    { 
      if (player.learnableSpells.TryGetValue(spellId, out var learnable))
      {
        learnable.learntFromClasses.Add(CustomClass.Ensorceleur);

        if (learnable.currentLevel < 1)
          learnable.LevelUp(player);
      }
      else
      {
        LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[spellId], CustomClass.Ensorceleur);
        player.learnableSpells.Add(learnableSpell.id, learnableSpell);
        learnableSpell.LevelUp(player);
      }

      NwSpell spell = NwSpell.FromSpellId(spellId);
      int spellLevel = spell.GetSpellLevelForClass(ClassType.Sorcerer);

      var knownSpells = player.oid.LoginCreature.GetClassInfo(ClassType.Sorcerer).KnownSpells[spellLevel];

      if (!knownSpells.Contains(spell))
      {
        try
        {
          player.oid.LoginCreature.GetClassInfo(ClassType.Sorcerer).KnownSpells[spellLevel].Add(spell);
        }
        catch (Exception)
        {
          player.oid.SendServerMessage($"Attention - Sort de sorcellerie incorrectement configuré pour enso : {spell.Name.ToString()}", ColorConstants.Red);
        }
      }
    }
  }
}
