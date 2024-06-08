using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyFrappeOcculte()
      {
        if (learnableSkills.ContainsKey(CustomSkill.EldritchKnightFrappeOcculte))
        {
          oid.LoginCreature.OnCreatureAttack -= FighterUtils.OnAttackFrappeOcculte;
          oid.LoginCreature.OnCreatureAttack += FighterUtils.OnAttackFrappeOcculte;
        }
      }
    }
  }
}
