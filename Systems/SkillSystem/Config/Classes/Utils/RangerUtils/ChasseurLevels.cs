using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class Ranger
  {
    public static void HandleChasseurLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(14).SetPlayerOverride(player.oid, "Conclave des Chasseurs");
          player.oid.SetTextureOverride("ranger", "chasseur");

          player.LearnClassSkill(CustomSkill.ChasseurMythes);
          player.LearnClassSkill(CustomSkill.ChasseurProie);

          break;

        case 7: player.LearnClassSkill(CustomSkill.ChasseurTactiquesDefensives); break;

        case 11: 

          player.LearnClassSkill(CustomSkill.ChasseurVolee);
          player.oid.LoginCreature.OnCreatureDamage -= RangerUtils.OnDamageVolee;
          player.oid.LoginCreature.OnCreatureDamage += RangerUtils.OnDamageVolee;

          break;

        case 15:

          player.LearnClassSkill(CustomSkill.ChasseurDefenseSuperieure);
          player.oid.LoginCreature.OnDamaged -= RangerUtils.OnDamageDefenseSuperieure;
          player.oid.LoginCreature.OnDamaged += RangerUtils.OnDamageDefenseSuperieure;

          break;
      }
    }
  }
}
