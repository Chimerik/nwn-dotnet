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

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.MarqueDuChasseur, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Bane, CustomClass.Paladin);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FouleeBrumeuse, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.HoldPerson, CustomClass.Paladin);

          break;


        case 7:

          player.learnableSkills.TryAdd(CustomSkill.PaladinVengeurImplacable, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PaladinVengeurImplacable], player));
          player.learnableSkills[CustomSkill.PaladinVengeurImplacable].LevelUp(player);
          player.learnableSkills[CustomSkill.PaladinVengeurImplacable].source.Add(Category.Class);

          player.oid.LoginCreature.OnCreatureAttack -= PaladinUtils.OnAttackVengeurImplacable;
          player.oid.LoginCreature.OnCreatureAttack += PaladinUtils.OnAttackVengeurImplacable;

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Haste, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ProtectionFromElements, CustomClass.Paladin);

          break;

        case 13:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Bannissement, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.PorteDimensionnelle, CustomClass.Paladin);

          break;

        case 17:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.HoldMonster, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Scrutation, CustomClass.Paladin);

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
