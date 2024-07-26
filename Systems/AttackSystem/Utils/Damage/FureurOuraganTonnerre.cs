using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static async void HandleFureurTonnerreFoudre(CNWSObject target, CNWSCombatAttackData data)
    {
      if (data.m_nAttackResult == 4 || data.m_bRangedAttack.ToBool() 
        || !target.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.FureurOuraganTonnerreExoTag).ToBool()))
        return;

      await NwTask.NextFrame();
      EffectUtils.RemoveTaggedEffect(target, EffectSystem.FureurOuraganTonnerreExoTag);
    }
  }
}
