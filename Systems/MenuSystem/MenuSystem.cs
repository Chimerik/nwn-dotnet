
using Anvil.Services;

using Newtonsoft.Json;

namespace NWN.Systems
{
  [ServiceBinding(typeof(MenuSystem))]
  class MenuSystem
  {
    public static PortraitDemo portraitDemo;

    public MenuSystem()
    {
      portraitDemo = new PortraitDemo();

      //ModuleSystem.Log.Info(JsonConvert.SerializeObject(chatMenu.window, Formatting.Indented));
    }
  }
}
