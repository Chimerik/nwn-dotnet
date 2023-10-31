using System.Collections.Generic;

using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class SoundSetEntry : ITwoDimArrayEntry
  {
    public string label { get; private set; }
    public string resRef { get; private set; }
    public bool pcVoiceSet { get; private set; }
    public Gender gender { get; private set; }

    // RowIndex is already populated externally, and we do not need to assign it in InterpretEntry.
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      label = entry.GetString("LABEL");
      resRef = entry.GetStrRef("STRREF").ToString();
      pcVoiceSet = entry.GetInt("TYPE").GetValueOrDefault(-1) == 0;
      gender = (Gender)entry.GetInt("GENDER").GetValueOrDefault(4);
    }
  }

  [ServiceBinding(typeof(SoundSet2da))]
  public class SoundSet2da
  {
    public static readonly TwoDimArray<SoundSetEntry> soundSetTable = NwGameTables.GetTable<SoundSetEntry>("soundset.2da");
    public static readonly List<NuiComboEntry> playerMaleVoiceSet = new();
    public static readonly List<NuiComboEntry> playerFemaleVoiceSet = new();

    public SoundSet2da()
    {
      foreach(var sound in soundSetTable)
      {
        if(sound.pcVoiceSet)
        {
          switch(sound.gender) 
          {
            case Gender.Male: playerMaleVoiceSet.Add(new NuiComboEntry(sound.resRef.ToString(), sound.RowIndex));break;
            case Gender.Female: playerFemaleVoiceSet.Add(new NuiComboEntry(sound.resRef.ToString(), sound.RowIndex)); break;
          }
        }
      }
    }
  }
}
