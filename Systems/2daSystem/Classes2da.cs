﻿
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class ClassEntry : ITwoDimArrayEntry
  {
    public int RowIndex { get; init; }
    public int classLearnableId { get; private set; }
    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      classLearnableId = RowIndex switch
      {
        (int)ClassType.Fighter => CustomSkill.Fighter,
        (int)ClassType.Barbarian => CustomSkill.Barbarian,
        (int)ClassType.Rogue => CustomSkill.Rogue,
        (int)ClassType.Monk => CustomSkill.Monk,
        _ => CustomSkill.Invalid,
      };
    }
  }

  [ServiceBinding(typeof(Classes2da))]
  public class Classes2da
  {
    public static readonly TwoDimArray<ClassEntry> classTable = NwGameTables.GetTable<ClassEntry>("classes.2da");

    public Classes2da(ModuleSystem moduleSystem)
    {
      foreach (var entry in classTable) ;
    }
  }
}
