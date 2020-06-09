using System;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public class Player
    {
      public readonly uint oid;

      public Player(uint oid)
      {
        this.oid = oid;
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
    }
  }
}
