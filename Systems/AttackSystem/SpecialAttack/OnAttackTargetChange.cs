using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Anvil.API;
using Anvil.Services;
using NWN.Core.NWNX;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class AttackSystem
  {
    [ScriptHandler("on_target_change")]
    private void OnAttackTargetChange(CallInfo callInfo)
    {
      var newTarget = NWScript.StringToObject(EventsPlugin.GetEventData("NEW_TARGET_OBJECT_ID"));

      if (newTarget != NWScript.OBJECT_INVALID)
      {
        var target = newTarget.ToNwObject<NwGameObject>();
        callInfo.ObjectSelf.GetObjectVariable<LocalVariableObject<NwGameObject>>(CreatureUtils.CurrentAttackTarget).Value = target;
      }
      else
      {
        callInfo.ObjectSelf.GetObjectVariable<LocalVariableObject<NwGameObject>>(CreatureUtils.CurrentAttackTarget).Delete();
      }
    }
  }
}
