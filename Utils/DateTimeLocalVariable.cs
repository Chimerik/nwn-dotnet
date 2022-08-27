using System;

using Anvil.API;

namespace NWN
{
  public class DateTimeLocalVariable : LocalVariable<DateTime>
  {
    public override DateTime Value
    {
      get => DateTime.UnixEpoch + TimeSpan.FromSeconds(Object.GetObjectVariable<LocalVariableInt>(Name).Value);
      set
      {
        Object.GetObjectVariable<LocalVariableInt>(Name).Value = (int)(value - DateTime.UnixEpoch).TotalSeconds;
      }
    }

    public override void Delete()
    {
      Object.GetObjectVariable<LocalVariableInt>(Name).Delete();
    }
  }
}
