using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void JeuDeJambe(NwCreature caster)
    {
      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterJeuDeJambe;

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Jeu de Jambe", StringUtils.gold);
      FeatUtils.DecrementManoeuvre(caster);
    }
  }
}
