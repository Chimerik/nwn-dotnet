using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class OccultisteUtils
  {
    public static void PacteDeLaLameOnUnacquire(ModuleEvents.OnUnacquireItem onUnacquire)
    {
      NwItem item = onUnacquire.Item;

      if (item.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Value != onUnacquire.LostBy)
        return;

      onUnacquire.LostBy.ApplyEffect(EffectDuration.Permanent, EffectSystem.PacteDeLaLameDispel);
      onUnacquire.LostBy.OnUnacquireItem -= PacteDeLaLameOnUnacquire;
      onUnacquire.LostBy.OnAcquireItem -= PacteDeLaLameOnAcquire;
      onUnacquire.LostBy.OnAcquireItem += PacteDeLaLameOnAcquire;
    }
    public static void PacteDeLaLameOnAcquire(ModuleEvents.OnAcquireItem onAcquire)
    {
      NwItem item = onAcquire.Item;

      if (item.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Value != onAcquire.AcquiredBy)
        return;

      EffectUtils.RemoveTaggedEffect(onAcquire.AcquiredBy, EffectSystem.PacteDeLaLameDispelEffectTag);
      onAcquire.AcquiredBy.OnUnacquireItem -= PacteDeLaLameOnUnacquire;
      onAcquire.AcquiredBy.OnUnacquireItem += PacteDeLaLameOnUnacquire;
      onAcquire.AcquiredBy.OnAcquireItem -= PacteDeLaLameOnAcquire;
    }
  }
}
