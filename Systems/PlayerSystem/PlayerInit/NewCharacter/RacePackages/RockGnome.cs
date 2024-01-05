using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyRockGnomePackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Gnome, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Gnome], this)))
          learnableSkills[CustomSkill.Gnome].LevelUp(this);

        learnableSkills[CustomSkill.Gnome].source.Add(Category.Race);

        oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.dwarfSlow);
        // TODO : Penser à doubler le bonus de maîtrise sur les jets d'Histoire
      }
    }
  }
}
