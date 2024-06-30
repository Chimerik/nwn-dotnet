namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyVengeurImplacable()
      {
        if (learnableSkills.ContainsKey(CustomSkill.PaladinVengeurImplacable))
        {
          oid.LoginCreature.OnCreatureAttack -= PaladinUtils.OnAttackVengeurImplacable;
          oid.LoginCreature.OnCreatureAttack += PaladinUtils.OnAttackVengeurImplacable;
        }
      }
    }
  }
}
