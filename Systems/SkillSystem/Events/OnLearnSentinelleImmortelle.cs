using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnSentinelleImmortelle(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.SentinelleImmortelle);
      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.SentinelleImmortelleVariable).Value = 1;

      return true;
    }
  }
}
