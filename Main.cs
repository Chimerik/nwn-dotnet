using System;
using NWN.Core;
using NWN.Systems;

namespace NWN
{
  public class ServerCore
  {
    public static int Bootstrap(IntPtr intPtr, int length)
    {
      CoreGameManager coreGameManager = new CoreGameManager();
      coreGameManager.OnSignal += OnSignal;
      coreGameManager.OnRunScript += OnRunScript;
      coreGameManager.OnServerLoop += OnServerLoop;

      return NWNCore.Init(intPtr, length, coreGameManager);
    }

    public static void OnRunScript(string scriptName, uint objectSelf, out int scriptHandleResult)
    {
      // This switch statement illustrates how the template prompts individual script calls. Most
      // implementations will split this logic into a Dictionary or other sensible arrangement. Note that
      // a script does not need to be in the module for its name to be assigned. Many DotNET modules
      // have no .nss or .ncs files at all. Note that script names must always be shorter than 16
      // characters by an internal engine limitation.

      Func<uint, int> handler;
      if (ModuleSystem.Register.TryGetValue(scriptName, out handler))
      {
        try
        {
          if (scriptName != "module_heartbeat" && scriptName != "event_validate_equip_items_before" && scriptName != "os_statuemaker" && scriptName != "pc_acquire_it" && scriptName != "ondeath_quaranti" && scriptName != "on_chat") 

            Console.WriteLine($"script : {scriptName}");
            
          DateTime time = DateTime.Now;
          Module.currentScript = scriptName;
          scriptHandleResult = handler.Invoke(objectSelf);

          if (scriptName != "module_heartbeat" && scriptName != "event_validate_equip_items_before" && scriptName != "os_statuemaker" && scriptName != "pc_acquire_it" && scriptName != "ondeath_quaranti" && scriptName != "on_chat")
          {
            Console.WriteLine($"execution time : {(DateTime.Now - time).TotalSeconds}");
          }
        }
        catch (Exception e)
        {
          Utils.LogException(e);
        }
      }

      // default return value
      scriptHandleResult = -1;
    }

    private static void OnSignal(string signal)
    {
      switch (signal)
      {
        case "ON_MODULE_LOAD_FINISH":
          break;
        case "ON_DESTROY_SERVER":
          break;
      }
    }

    private static void OnServerLoop(ulong frame)
    {
    }
  }
}
