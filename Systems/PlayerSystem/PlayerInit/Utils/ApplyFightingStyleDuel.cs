using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyFightingStyleDuel()
      {
        if (learnableSkills.ContainsKey(CustomSkill.FighterCombatStyleDuel))
        {
          oid.LoginCreature.OnCreatureDamage -= DamageUtils.HandleDuellingDamage;
          oid.LoginCreature.OnCreatureDamage += DamageUtils.HandleDuellingDamage;
        }
      }
    }
  }
}
