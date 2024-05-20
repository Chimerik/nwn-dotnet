using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnDisconnectBelluaire(ModuleEvents.OnClientLeave onLeave)
    {
      if (onLeave.Player.LoginCreature.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasNothing)
        return;

      var player = onLeave.Player.LoginCreature;
      var companion = player.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

      player.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Delete();

      companion.OnDeath -= OnAnimalCompanionDeath;
      companion.Destroy();
      companion.VisibilityOverride = Anvil.Services.VisibilityMode.Hidden;

      ClearAnimalCompanion(companion);
    }
  }
}
