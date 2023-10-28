using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDeepGnomePackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Gnome, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Gnome])))
          learnableSkills[CustomSkill.Gnome].LevelUp(this);

        learnableSkills[CustomSkill.Gnome].source.Add(Category.Race);

        oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.dwarfSlow);
        // TODO : Penser à gérer l'avantage sur les jets de Furtivité
      }
    }
  }
}
