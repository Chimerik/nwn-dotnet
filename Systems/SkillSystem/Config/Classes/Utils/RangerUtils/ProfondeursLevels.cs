using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Ranger
  {
    public static void HandleProfondeursLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(14).SetPlayerOverride(player.oid, "Conclave des Profondeurs");
          player.oid.SetTextureOverride("ranger", "profondeurs");

          player.learnableSkills.TryAdd(CustomSkill.TraqueurRedoutable, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TraqueurRedoutable], player));
          player.learnableSkills[CustomSkill.TraqueurRedoutable].LevelUp(player);
          player.learnableSkills[CustomSkill.TraqueurRedoutable].source.Add(Category.Class);

          if (player.learnableSpells.TryGetValue(CustomSpell.Deguisement, out var learnable))
          {
            learnable.learntFromClasses.Add(CustomClass.Ranger);

            if (learnable.currentLevel < 1)
              learnable.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.Deguisement], CustomClass.Ranger);
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          player.oid.SendServerMessage($"Vous apprenez le sort {StringUtils.ToWhitecolor("Déguisement")}", ColorConstants.Orange);

          break;

        case 5:

          player.learnableSkills.TryAdd(CustomSkill.EscrimeBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EscrimeBonusAttack], player));
          player.learnableSkills[CustomSkill.EscrimeBonusAttack].LevelUp(player);
          player.learnableSkills[CustomSkill.EscrimeBonusAttack].source.Add(Category.Class);

          player.oid.LoginCreature.BaseAttackCount += 1;

          if (player.learnableSpells.TryGetValue(CustomSpell.CordeEnchantee, out var corde))
          {
            corde.learntFromClasses.Add(CustomClass.Ranger);

            if (corde.currentLevel < 1)
              corde.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.CordeEnchantee], CustomClass.Ranger);
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          player.oid.SendServerMessage($"Vous apprenez le sort {StringUtils.ToWhitecolor("Corde Enchantée")}", ColorConstants.Orange);

          break;

        case 7:

          player.learnableSkills.TryAdd(CustomSkill.WisdomSavesProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WisdomSavesProficiency], player));
          player.learnableSkills[CustomSkill.WisdomSavesProficiency].LevelUp(player);
          player.learnableSkills[CustomSkill.WisdomSavesProficiency].source.Add(Category.Class);

          break;

        case 9:

          player.oid.LoginCreature.BaseAttackCount += 1;

          if (player.learnableSpells.TryGetValue((int)Spell.GlyphOfWarding, out var glyphe))
          {
            glyphe.learntFromClasses.Add(CustomClass.Ranger);

            if (glyphe.currentLevel < 1)
              glyphe.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.GlyphOfWarding], CustomClass.Ranger);
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          player.oid.SendServerMessage($"Vous apprenez le sort {StringUtils.ToWhitecolor("Glyphe de Garde")}", ColorConstants.Orange);

          break;

        case 11:

          player.learnableSkills.TryAdd(CustomSkill.TraqueurRafale, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TraqueurRafale], player));
          player.learnableSkills[CustomSkill.TraqueurRafale].LevelUp(player);
          player.learnableSkills[CustomSkill.TraqueurRafale].source.Add(Category.Class);

          break;

        case 13:

          if (player.learnableSpells.TryGetValue((int)Spell.ImprovedInvisibility, out var invi))
          {
            invi.learntFromClasses.Add(CustomClass.Ranger);

            if (invi.currentLevel < 1)
              invi.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[(int)Spell.ImprovedInvisibility], CustomClass.Ranger);
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          player.oid.SendServerMessage($"Vous apprenez le sort {StringUtils.ToWhitecolor("Invisibilité Suprême")}", ColorConstants.Orange);

          break;

        case 15:

          player.learnableSkills.TryAdd(CustomSkill.TraqueurEsquive, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.TraqueurEsquive], player));
          player.learnableSkills[CustomSkill.TraqueurEsquive].LevelUp(player);
          player.learnableSkills[CustomSkill.TraqueurEsquive].source.Add(Category.Class);

          break;

        case 17:

          if (player.learnableSpells.TryGetValue(CustomSpell.ApparencesTrompeuses, out var app))
          {
            app.learntFromClasses.Add(CustomClass.Ranger);

            if (app.currentLevel < 1)
              app.LevelUp(player);
          }
          else
          {
            LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)learnableDictionary[CustomSpell.ApparencesTrompeuses], CustomClass.Ranger);
            player.learnableSpells.Add(learnableSpell.id, learnableSpell);
            learnableSpell.LevelUp(player);
          }

          player.oid.SendServerMessage($"Vous apprenez le sort {StringUtils.ToWhitecolor("Apparences Trompeuses")}", ColorConstants.Orange);


          break;
      }
    }
  }
}
