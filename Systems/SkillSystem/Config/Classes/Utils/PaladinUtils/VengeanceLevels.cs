using System.Security.Cryptography;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Paladin
  {
    public static void HandleVengeanceLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Serment de Vengeance");
          player.oid.SetTextureOverride("paladin", "vengeance");

          player.learnableSkills.TryAdd(CustomSkill.PaladinVoeuHostile, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinVoeuHostile], player));
          player.learnableSkills[CustomSkill.PaladinVoeuHostile].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinVoeuHostile].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.PaladinPuissanceInquisitrice, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinPuissanceInquisitrice], player));
          player.learnableSkills[CustomSkill.PaladinPuissanceInquisitrice].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinPuissanceInquisitrice].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.PaladinConspuerEnnemi, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinConspuerEnnemi], player));
          player.learnableSkills[CustomSkill.PaladinConspuerEnnemi].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinConspuerEnnemi].source.Add(Category.Class);

          PaladinUtils.LearnSermentSpell(player, CustomSpell.MarqueDuChasseur);
          PaladinUtils.LearnSermentSpell(player, (int)Spell.Bane);

          break;

        case 5:

          PaladinUtils.LearnSermentSpell(player, CustomSpell.FouleeBrumeuse);
          PaladinUtils.LearnSermentSpell(player, (int)Spell.HoldPerson);

          break;


        case 7:

          player.learnableSkills.TryAdd(CustomSkill.PaladinVengeurImplacable, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinVengeurImplacable], player));
          player.learnableSkills[CustomSkill.PaladinVengeurImplacable].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinVengeurImplacable].source.Add(Category.Class);

          player.oid.LoginCreature.OnCreatureAttack -= PaladinUtils.OnAttackVengeurImplacable;
          player.oid.LoginCreature.OnCreatureAttack += PaladinUtils.OnAttackVengeurImplacable;

          break;

        case 9:

          PaladinUtils.LearnSermentSpell(player, (int)Spell.Haste);
          PaladinUtils.LearnSermentSpell(player, (int)Spell.ProtectionFromElements);

          break;

        case 13:

          PaladinUtils.LearnSermentSpell(player, CustomSpell.Bannissement);
          PaladinUtils.LearnSermentSpell(player, CustomSpell.PorteDimensionnelle);

          break;

        case 17:

          PaladinUtils.LearnSermentSpell(player, (int)Spell.HoldMonster);
          PaladinUtils.LearnSermentSpell(player, CustomSpell.Scrutation);

          break;

        case 20:

          player.learnableSkills.TryAdd(CustomSkill.AngeDeLaVengeance, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.AngeDeLaVengeance], player));
          player.learnableSkills[CustomSkill.AngeDeLaVengeance].LevelUp(player);
          player.learnableSkills[CustomSkill.AngeDeLaVengeance].source.Add(Category.Class);

          break;
      }
    }
  }
}
