using System;
using Anvil.API;


namespace NWN.Systems
{
  public static partial class StringUtils
  {
    public static async void DelayLocalVariableDeletion<T>(NwGameObject target, string variable, TimeSpan timespan) where T : ObjectVariable, new()
    {
      await NwTask.Delay(timespan);
      target.GetObjectVariable<T>(variable).Delete();
    }
  }
}
