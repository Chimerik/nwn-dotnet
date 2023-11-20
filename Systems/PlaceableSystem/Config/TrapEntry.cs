using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public sealed class TrapEntry : ITwoDimArrayEntry
    {
      public int RowIndex { get; init; }
      public DamageType damageType { get; private set; }
      public int damageDice { get; private set; }
      public int numDice { get; private set; }
      public int baseDC { get; private set; }
      public int duration { get; private set; }
      public int aoeSize { get; private set; }
      public VfxType damageVFX { get; private set; }
      public Ability savingThrowAbility { get; private set; }

      public void InterpretEntry(TwoDimArrayEntry entry)
      {
        damageType = (DamageType)entry.GetInt("DamageType").GetValueOrDefault(4096);
        damageVFX = (VfxType)entry.GetInt("VfxType").GetValueOrDefault(-1);
        damageDice = entry.GetInt("DamageDice").GetValueOrDefault(0);
        numDice = entry.GetInt("NumDice").GetValueOrDefault(0);
        baseDC = entry.GetInt("baseDC").GetValueOrDefault(0);
        duration = entry.GetInt("Duration").GetValueOrDefault(0);
        aoeSize = entry.GetInt("AoeSize").GetValueOrDefault(0);
        //savingThrowAbility = (Ability)entry.GetInt("SavingThrow").GetValueOrDefault(-1);
      }
    }
  }
}
