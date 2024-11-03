using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyHalfelinPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Halfelin, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Halfelin], this)))
          learnableSkills[CustomSkill.Halfelin].LevelUp(this);

        learnableSkills[CustomSkill.Halfelin].source.Add(Category.Race);

        oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AgiliteHalfelin);

        // TODO : Penser à gérer l'avantage sur les jets de Furtivité
        // TODO : Penser à gérer le rejeu des jets de compétences (lorsque le lanceur de dés pour les anims sera fait !) 
      }
    }
  }
}
