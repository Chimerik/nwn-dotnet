using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class AppearanceTable : ITwoDimArray
  {
    private readonly Dictionary<AppearanceType, Entry> entries = new Dictionary<AppearanceType, Entry>();
    private static Dictionary<string, int[]> randomAppearanceDictionary = new Dictionary<string, int[]>()
    {
      { "plage", new int[] { 1984, 1985, 1986, 3160, 3155, 3156, 1956, 1957, 1958, 1959, 1960, 1961, 1962, 291, 292, 1964, 4310, 1428, 1430, 1980, 1981, 3261, 3262, 3263 } },
      { "cave", new int[] { 3197, 3198, 3199, 3200, 3202, 3204, 3205, 3206, 3207, 3208, 3209, 3210, 3999, 6425, 6426, 6427, 6428, 6429, 6430, 6431, 6432, 6433, 6434, 6435, 6436, 3397, 3398, 3400, 3434 } },
      { "city", new int[] { 1983, 1390, 1391, 1392, 1393, 1394, 1395, 1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403, 1404, 1405, 1406, 1407, 1408, 1409, 1410, 1411, 4385, 4408, 4112, 4113, 2505 } },
      { "civilian", new int[] { 220, 221, 222, 224, 225, 226, 227, 228, 229, 231, 4357, 4358, 238, 239, 240, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 278, 279, 280, 281, 282, 283, 284, 212, 4379 } },
      { "generic", new int[] { 3259, 1181, 1182, 1183, 1184, 4370, 4371, 1794, 1988, 3213, 3214, 3215, 3216, 3222, 3223, 1339, 3445, 3520, 4338, 1335, 1336, 1966, 1967, 1968, 1969, 1970, 1971, 1972, 4221, 1025, 1026, 1797, 3237, 3238, 3239, 3240, 3241, 1328, 1941, 1330, 1438, 496, 509, 522, 535, 1784, 1785, 1787, 1788, 1789, 1791, 1855, 1856, 1857, 1858, 1859, 1860, 2589, 1334, 1973, 1974, 4309, 4310, 4320, 4321, 4322, 34, 142, 1796, 1340, 3192, 3193, 3194, 3195, 3196, 1341, 1005, 1006, 1007, 1008, 1009, 1010, 1011, 1013, 1014, 1015, 1016, 1017, 1019, 1020, 1255, 3152, 3153, 3157, 3158, 3159, 3161, 3162, 3163, 3164, 3165, 3166, 3167, 3168, 3154, 3148, 3149, 1975, 1976, 1977, 1978, 1979, 2506, 3043, 3044, 3045, 3046, 3047, 1275, 1947, 1949, 1950, 1951, 1952, 6365, 6408, 31, 145, 144, 3305, 4364, 1982, 1749, 1750, 1751, 1332, 1333, 1987, 1863, 1337, 1295, 1329, 3310, 3311, 1802, 1803, 1804, 1805, 8, 35, 37, 4115, 4116, 4117, 4118, 4119, 4120, 4121, 4122, 4123, 3138, 1338 } }
    };

    public Entry GetAppearanceDataEntry(AppearanceType type)
    {
      return entries[type];
    }
    public async void SetRandomAppearance(NwCreature creature)
    {
      PlayerSystem.Log.Info("Set random appearance on");
      string appearance = creature.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").HasValue ? creature.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").Value : "city";
      int[] appearanceArray = randomAppearanceDictionary[appearance];
      AppearanceType appearanceType = (AppearanceType)appearanceArray[Utils.random.Next(0, appearanceArray.Length)];

      Entry entry = entries[appearanceType];
      creature.CreatureAppearanceType = appearanceType;
      creature.PortraitResRef = entry.portrait;

      await creature.ActionRandomWalk();

      if (appearance == "civilian")
        return;

      creature.Name = entry.name;
      if (creature.Name == "Créature")
        Utils.LogMessageToDMs($"Apparence {creature.CreatureAppearanceType} - Nom non défini.");

      PlayerSystem.Log.Info("Set random appearance off");
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint strRef = uint.TryParse(twoDimEntry("STRING_REF"), out strRef) ? strRef : 0;
      string name = strRef == 0 ? name = "Créature" : name = Appearance2da.tlkTable.GetSimpleString(strRef);
      string portrait = twoDimEntry("PORTRAIT") == "****" ? portrait = "po_hu_m_99" : portrait = twoDimEntry("PORTRAIT");
      string race = twoDimEntry("RACE");

      entries.Add((AppearanceType)rowIndex, new Entry(name, portrait, race));
    }
    public readonly struct Entry
    {
      public readonly string name;
      public readonly string portrait;
      public readonly string race;

      public Entry(string name, string portrait, string race)
      {
        this.name = name;
        this.portrait = portrait;
        this.race = race;
      }
    }
  }

  [ServiceBinding(typeof(Appearance2da))]
  public class Appearance2da
  {
    public static TlkTable tlkTable;
    public static AppearanceTable appearanceTable;
    public Appearance2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      appearanceTable = twoDimArrayFactory.Get2DA<AppearanceTable>("appearance");
    }
  }
}
