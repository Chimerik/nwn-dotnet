using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplySmallRaceSlow()
      {
        switch(oid.LoginCreature.Race.Id)
        {
          case CustomRace.Gnome:
          case CustomRace.RockGnome:
          case CustomRace.ForestGnome:
          case CustomRace.DeepGnome:
          case CustomRace.Dwarf:
          case CustomRace.GoldDwarf:
          case CustomRace.ShieldDwarf:
          case CustomRace.Duergar:
          case CustomRace.Halfling:
          case CustomRace.LightfootHalfling:
          case CustomRace.StrongheartHalfling:

            if (!learnableSkills.ContainsKey(CustomSkill.AgiliteDesCourtsSurPattes) && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.DwarfSlowEffectTag))
              oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.dwarfSlow);

            return;

          default: return;
        }
      }
    }
  }
}
