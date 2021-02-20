using NWN.Core;
using NWN.Services;
using NWN.API;
using NWN.API.Constants;
using System.Linq;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    [ScriptHandler("on_ench_cast")]
    private void HandleItemEnchantement(CallInfo callInfo)
    {
      NwItem oTarget = NWScript.GetSpellTargetObject().ToNwObject<NwItem>();

      if (!(callInfo.ObjectSelf is NwPlayer) || oTarget == null || !PlayerSystem.Players.TryGetValue(callInfo.ObjectSelf, out PlayerSystem.Player player))
        return;

      NwPlayer oCaster = (NwPlayer)callInfo.ObjectSelf;

      if (!player.craftJob.CanStartJob(oCaster, NWScript.OBJECT_INVALID, Craft.Job.JobType.Enchantement))
        return;

      if(oTarget.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").HasNothing)
      {
        oCaster.SendServerMessage($"{oTarget.Name} ne dispose d'aucun emplacement d'enchantement disponible !");
        return;
      }

      player.craftJob.Start(Craft.Job.JobType.Enchantement, null, player, NWScript.OBJECT_INVALID, oTarget, NWScript.GetSpellId().ToString());
    }
  }
}
