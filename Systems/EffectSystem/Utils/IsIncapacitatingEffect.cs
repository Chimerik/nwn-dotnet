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
      if (eff.Tag is not null && eff.Tag == EffectSystem.KnockdownEffectTag)
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
      if (creature.m_appliedEffects.Any(e => In((EffectTrueType)e.m_nType, EffectTrueType.Knockdown, EffectTrueType.Petrify, 
          EffectTrueType.Pacify, EffectTrueType.Timestop, EffectTrueType.Sanctuary)
        || ((EffectTrueType)e.m_nType == EffectTrueType.SetState && Utils.In(e.GetInteger(0), EffectState.Etourdi, EffectState.Charme, 
          EffectState.Endormi, EffectState.Confus, EffectState.Effroi, EffectState.Domine, EffectState.Paralyse))))
        return true;

      return false;
    }
  }
}
