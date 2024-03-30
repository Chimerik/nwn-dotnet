using System.Linq;
using Anvil.API;
using NWN.Native.API;
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
    public static bool IsIncapacitated(CNWSCreature creature)
    {
      if (creature.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Knockdown
          || (EffectTrueType)e.m_nType == EffectTrueType.Petrify || (EffectTrueType)e.m_nType == EffectTrueType.Sanctuary
          || (EffectTrueType)e.m_nType == EffectTrueType.Timestop || (EffectTrueType)e.m_nType == EffectTrueType.Pacify
          || ((EffectTrueType)e.m_nType == EffectTrueType.SetState && (e.GetInteger(0) == 6 || e.GetInteger(0) == 1
          || e.GetInteger(0) == 2 || e.GetInteger(0) == 3 || e.GetInteger(0) == 7 || e.GetInteger(0) == 8 || e.GetInteger(0) == 9))))
        return true;

      return false;
    }
  }
}
