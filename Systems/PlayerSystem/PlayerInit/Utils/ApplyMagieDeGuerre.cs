namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMagieDeGuerre()
      {
        if (learnableSkills.ContainsKey(CustomSkill.EldritchKnightMagieDeGuerre))
        {
          oid.LoginCreature.OnSpellAction -= FighterUtils.OnSpellCastMagieDeGuerre;
          oid.LoginCreature.OnSpellAction += FighterUtils.OnSpellCastMagieDeGuerre;

          oid.LoginCreature.OnCreatureAttack -= FighterUtils.OnAttackMagieDeGuerre;
          oid.LoginCreature.OnCreatureAttack += FighterUtils.OnAttackMagieDeGuerre;
        }
      }
    }
  }
}
