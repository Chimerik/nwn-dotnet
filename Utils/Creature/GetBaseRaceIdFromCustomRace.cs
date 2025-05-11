using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static byte GetBaseRaceIdFromCustomRace(int customRace)
    {
      return customRace switch
      {
        CustomRace.DeepGnome or CustomRace.RockGnome or CustomRace.ForestGnome => CustomRace.Gnome,
        CustomRace.Drow or CustomRace.HighElf or CustomRace.WoodElf => CustomRace.Elf,
        CustomRace.DrowHalfElf or CustomRace.HighHalfElf or CustomRace.WoodHalfElf => CustomRace.HalfElf,
        CustomRace.GoldDwarf or CustomRace.Duergar => CustomRace.Dwarf,
        CustomRace.Halfelin => CustomRace.Halfling,
        CustomRace.HalfOrc => CustomRace.HalfOrc,
        _ => CustomRace.Human,
      };
    }
  }
}
