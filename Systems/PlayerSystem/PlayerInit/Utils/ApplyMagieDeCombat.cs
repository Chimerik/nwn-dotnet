namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMagieDeCombat()
      {
        if (learnableSkills.TryGetValue(CustomSkill.BardCollegeDeLaVaillance, out var learnable) && learnable.currentLevel > 13)
        {
          oid.LoginCreature.OnSpellCast -= SpellSystem.OnSpellCastMagieDeCombat;
          oid.LoginCreature.OnSpellCast += SpellSystem.OnSpellCastMagieDeCombat;
        }
      }
    }
  }
}
