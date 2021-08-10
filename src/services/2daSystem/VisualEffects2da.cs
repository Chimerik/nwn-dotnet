using System;
using System.Collections.Generic;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class VisualEffectsTable : ITwoDimArray
  {
    private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();

    public void TestVFX(int row, NwPlayer player)
    {
      Entry entry = entries[row];

      if (entry.durationtype == EffectDuration.Temporary && entry.duration > 0)
        player.ControlledCreature.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect((VfxType)row), TimeSpan.FromSeconds(entry.duration));
      else 
        player.ApplyInstantVisualEffectToObject((VfxType)row, player.ControlledCreature);
      //else
      //player.ShowVisualEffect((VfxType)value, ctx.oSender.ControlledCreature.Position);

      player.SendServerMessage(entry.name);
    }
    public Entry GetDataEntry(int row)
    {
      return entries[row];
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      string name = twoDimEntry("Label");
      string type = twoDimEntry("Type_FD");
      EffectDuration durationType = EffectDuration.Instant;
      int duration = int.TryParse(twoDimEntry("ProgFX_Duration"), out duration) ? duration : 0;

      if (type == "D" && duration > 0)
        durationType = EffectDuration.Temporary;

      entries.Add(rowIndex, new Entry(name, durationType, duration / 100));
    }
    public readonly struct Entry
    {
      public readonly string name;
      public readonly EffectDuration durationtype;
      public readonly int duration;

      public Entry(string name, EffectDuration durationtype, int duration)
      {
        this.name = name;
        this.duration = duration;
        this.durationtype = durationtype;
      }
    }
  }

  [ServiceBinding(typeof(VisualEfects2da))]
  public class VisualEfects2da
  {
    public static VisualEffectsTable vfxTable;
    public VisualEfects2da(TwoDimArrayFactory twoDimArrayFactory)
    {
      vfxTable = twoDimArrayFactory.Get2DA<VisualEffectsTable>("visualeffects");
    }
  }
}
