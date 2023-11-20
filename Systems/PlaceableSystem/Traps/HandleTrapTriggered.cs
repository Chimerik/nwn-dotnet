using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void HandleTrapTriggered(List<NwCreature> targets, TrapBaseType type, NwGameObject trap)
    {
      // JDS + dégâts + avantage si expert en donjon

      switch (type) 
      {
        case TrapBaseType.MinorSpike:
        case TrapBaseType.AverageSpike:
        case TrapBaseType.StrongSpike:
        case TrapBaseType.DeadlySpike: 
        case TrapBaseType.MinorHoly: 
        case TrapBaseType.AverageHoly:
        case TrapBaseType.StrongHoly: 
        case TrapBaseType.DeadlyHoly:
          SpikeTrap(targets, trap, trapTable.GetRow((int)type)); return;
        case TrapBaseType.MinorAcid:
        case TrapBaseType.AverageAcid:
        case TrapBaseType.StrongAcid:
        case TrapBaseType.DeadlyAcid: AcidTrap(targets, trap, trapTable.GetRow((int)type)); return;
        case TrapBaseType.MinorTangle:
        case TrapBaseType.AverageTangle:
        case TrapBaseType.StrongTangle:
        case TrapBaseType.DeadlyTangle: SlowTrap(trap, trapTable.GetRow((int)type)); return;
        case TrapBaseType.MinorFire:
        case TrapBaseType.AverageFire:
        case TrapBaseType.StrongFire:
        case TrapBaseType.DeadlyFire: FireTrap(trap, trapTable.GetRow((int)type)); return;
        case TrapBaseType.MinorElectrical:
        case TrapBaseType.AverageElectrical:
        case TrapBaseType.StrongElectrical:
        case TrapBaseType.DeadlyElectrical: ElecTrap(trap, trapTable.GetRow((int)type)); return;
        case TrapBaseType.MinorGas:
        case TrapBaseType.AverageGas:
        case TrapBaseType.StrongGas:
        case TrapBaseType.DeadlyGas: GasTrap(trap, trapTable.GetRow((int)type)); return;

      }
    }
  }
}
