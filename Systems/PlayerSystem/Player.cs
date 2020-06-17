using NWN.Enums.VisualEffect;
using System;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public class Player
    {
      public readonly uint oid;
      public Menu menu { get; }

      private uint blockingBoulder;

      public Player(uint oid)
      {
        this.oid = oid;
        this.menu = new PrivateMenu(this);
      }

      public void EmitKeydown(KeydownEventArgs e)
      {
        OnKeydown?.Invoke(this, e);
      }

      public event EventHandler<KeydownEventArgs> OnKeydown;

      public class KeydownEventArgs : EventArgs
      {
        public string key { get; }

        public KeydownEventArgs(string key)
        {
          this.key = key;
        }
      }

      public void BoulderBlock ()
      {
        BoulderUnblock();
        var location = NWScript.GetLocation(oid);
        blockingBoulder = NWScript.CreateObject(Enums.ObjectType.Placeable, "plc_boulder", location, false);
        NWNX.Object.SetPosition(oid, NWScript.GetPositionFromLocation(location));
        NWScript.ApplyEffectToObject(
          Enums.DurationType.Permanent,
          NWScript.EffectVisualEffect((VisualEffect)Temporary.CutsceneInvisibility),
          blockingBoulder
        );
      }

      public void BoulderUnblock ()
      {
        NWScript.DestroyObject(blockingBoulder);
      }
    }
  }
}
