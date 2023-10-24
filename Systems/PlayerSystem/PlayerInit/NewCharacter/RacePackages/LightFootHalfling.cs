using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyLightFootPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Halfelin, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Halfelin])))
          learnableSkills[CustomSkill.Halfelin].LevelUp(this);

        learnableSkills[CustomSkill.Halfelin].source.Add(Category.Race);

        oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.dwarfSlow);

        // TODO : Penser à gérer l'avantage sur les jets de Furtivité
        // TODO : Penser à gérer l'avantage sur les JDS contre la peur et la terreur
        // TODO : Penser à gérer le rejeu des jets de compétences ou de sauvegarde en cas de 1 
      }
    }
  }
}
