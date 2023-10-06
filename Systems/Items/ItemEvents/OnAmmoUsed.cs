using System.Security.Cryptography;
using Anvil.API;
using Anvil.Services;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    [ScriptHandler("on_ammo_used")]
    private void OnAmmoUsed(CallInfo callInfo)
    {
      switch(callInfo.ObjectSelf.ObjectId.ToNwObject<NwItem>().BaseItem.ItemType)
      {
        case BaseItemType.Arrow:
        case BaseItemType.Bolt:
        case BaseItemType.Bullet:
          EventsPlugin.SkipEvent();
          break;
      }
    }
  }
}
