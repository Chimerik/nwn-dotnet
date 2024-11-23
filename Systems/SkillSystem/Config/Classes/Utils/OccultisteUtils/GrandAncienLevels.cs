using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Occultiste
  {
    public static void HandleGrandAncienLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(player.oid, "Mécène Grand Ancien");
          player.oid.SetTextureOverride("occultiste", "warlock_ancien");

          player.LearnAlwaysPreparedSpell((int)Spell.TashasHideousLaughter, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.ForceFantasmagorique, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.MurmuresDissonnants, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.DetectionDesPensees, CustomClass.Occultiste);
          

          player.learnableSkills.TryAdd(CustomSkill.EspritEveille, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EspritEveille], player));
          player.learnableSkills[CustomSkill.EspritEveille].LevelUp(player);
          player.learnableSkills[CustomSkill.EspritEveille].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.SortsPsychiques, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SortsPsychiques], player));
          player.learnableSkills[CustomSkill.SortsPsychiques].LevelUp(player);
          player.learnableSkills[CustomSkill.SortsPsychiques].source.Add(Category.Class);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.VoraciteDhadar, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.ClairaudienceAndClairvoyance, CustomClass.Occultiste);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.CombattantClairvoyant, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.CombattantClairvoyant], player));
          player.learnableSkills[CustomSkill.CombattantClairvoyant].LevelUp(player);
          player.learnableSkills[CustomSkill.CombattantClairvoyant].source.Add(Category.Class);

          break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.Confusion, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.InvocationDaberration, CustomClass.Occultiste);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell(CustomSpell.AlterationMemorielle, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.Telekinesie, CustomClass.Occultiste);

          break;

        case 10:

          player.LearnAlwaysPreparedSpell((int)Spell.BestowCurse, CustomClass.Occultiste);

          player.learnableSkills.TryAdd(CustomSkill.BouclierPsychique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BouclierPsychique], player));
          player.learnableSkills[CustomSkill.BouclierPsychique].LevelUp(player);
          player.learnableSkills[CustomSkill.BouclierPsychique].source.Add(Category.Class);

          break;
      }
    }
  }
}
