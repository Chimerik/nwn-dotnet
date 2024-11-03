using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyRacePackage(NwRace race, int bonusSelection)
      {
        if(oid.LoginCreature.Race.Id == CustomRace.GoldDwarf)
          oid.LoginCreature.LevelInfo.FirstOrDefault().HitDie -= 1;

        oid.LoginCreature.Race = race;
        oid.LoginCreature.Appearance = NwGameTables.AppearanceTable[Races2da.raceTable[race.Id].appearanceId];

        switch (race.Id)
        {
          case CustomRace.Human: ApplyHumanPackage(bonusSelection); return;
          case CustomRace.HighElf: ApplyHighElfPackage(bonusSelection); return;
          case CustomRace.WoodElf: ApplyWoodElfPackage(); return;
          case CustomRace.Drow: ApplyDrowPackage(); return;
          case CustomRace.HighHalfElf: ApplyHighHalfElfPackage(bonusSelection); return;
          case CustomRace.WoodHalfElf: ApplyWoodHalfElfPackage(); return;
          case CustomRace.DrowHalfElf: ApplyDrowHalfElfPackage(); return;
          case CustomRace.HalfOrc: ApplyHalfOrcPackage(); return;
          case CustomRace.GoldDwarf: ApplyDwarfPackage(); return;
          case CustomRace.Duergar: ApplyDuergarPackage(); return;
          case CustomRace.Halfelin: ApplyHalfelinPackage(); return;
          case CustomRace.DeepGnome: ApplyDeepGnomePackage(); return;
          case CustomRace.ForestGnome: ApplyForestGnomePackage(); return;
          case CustomRace.RockGnome: ApplyRockGnomePackage(); return;
          case CustomRace.InfernalThiefling: ApplyInfernalPackage(); return;
          case CustomRace.AbyssalThiefling: ApplyAbyssalPackage(); return;
          case CustomRace.ChtonicThiefling: ApplyChtonicPackage(); return;
          case CustomRace.Aasimar: ApplyAasimarPackage(); return;
        }
      }
    }
  }
}
