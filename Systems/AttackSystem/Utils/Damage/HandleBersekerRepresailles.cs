using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleBersekerRepresailles(CNWSCreature attacker, CNWSCreature target)
    {
      if (target is null || target.m_nCurrentAction != (ushort)Action.AttackObject 
        || target.m_ScriptVars.GetInt(CreatureUtils.BersekerRepresaillesVariableExo) < 1)
        return;

      CNWSItem weapon = target.m_pInventory.GetItemInSlot((uint)Native.API.InventorySlot.RightHand);

      if (weapon is null || !ItemUtils.IsMeleeWeapon(NwBaseItem.FromItemId((int)weapon.m_nBaseItem))
        || Vector3.DistanceSquared(attacker.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector()) > 2)
        return;

      target.m_ScriptVars.DestroyInt(CreatureUtils.BersekerRepresaillesVariableExo);
      target.m_ScriptVars.SetObject(CreatureUtils.BersekerRepresaillesVariableExo, attacker.m_idSelf);
    }
  }
}
