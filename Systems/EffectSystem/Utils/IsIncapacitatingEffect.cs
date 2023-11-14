using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static bool IsIncapacitatingEffect(Effect eff)
    {
      if (eff.Tag == EffectSystem.KnockdownEffectTag)
        return true;

      return eff.EffectType switch
      {
        EffectType.Charmed or EffectType.Confused or EffectType.CutsceneImmobilize 
        or EffectType.CutsceneParalyze or EffectType.Dazed or EffectType.Dominated
        or EffectType.Ethereal or EffectType.Paralyze or EffectType.Stunned
        or EffectType.Petrify or EffectType.Sanctuary or EffectType.Sleep or EffectType.Turned => true,
        _ => false,
      };
    }
  }
}
