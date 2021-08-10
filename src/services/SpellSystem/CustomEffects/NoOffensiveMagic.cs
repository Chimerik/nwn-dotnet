using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  static class NoOffensiveMagic
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= NoOffensiveSpellMalus;
      oTarget.OnSpellCast += NoOffensiveSpellMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= NoOffensiveSpellMalus;
    }
    private static void NoOffensiveSpellMalus(OnSpellCast onSpellCast)
    {
      switch (onSpellCast.Spell)
      {
        case Spell.AcidSplash:
        case Spell.BallLightning:
        case Spell.BigbysClenchedFist:
        case Spell.BigbysCrushingHand:
        case Spell.BigbysForcefulHand:
        case Spell.BigbysGraspingHand:
        case Spell.BladeBarrier:
        case Spell.Bombardment:
        case Spell.BurningHands:
        case Spell.CallLightning:
        case Spell.ChainLightning:
        case Spell.CircleOfDeath:
        case Spell.CircleOfDoom:
        case Spell.Cloudkill:
        case Spell.Combust:
        case Spell.ConeOfCold:
        case Spell.Crumble:
        case Spell.DelayedBlastFireball:
        case Spell.Destruction:
        case Spell.Drown:
        case Spell.Earthquake:
        case Spell.ElectricJolt:
        case Spell.EpicHellball:
        case Spell.EpicRuin:
        case Spell.EnergyDrain:
        case Spell.Enervation:
        case Spell.EvardsBlackTentacles:
        case Spell.FingerOfDeath:
        case Spell.Fireball:
        case Spell.Firebrand:
        case Spell.FireStorm:
        case Spell.FlameArrow:
        case Spell.FlameLash:
        case Spell.FlameStrike:
        case Spell.FleshToStone:
        case Spell.GedleesElectricLoop:
        case Spell.GreaterShadowConjurationAcidArrow:
        case Spell.HammerOfTheGods:
        case Spell.Harm:
        case Spell.HorizikaulsBoom:
        case Spell.HorridWilting:
        case Spell.IceDagger:
        case Spell.IceStorm:
        case Spell.Implosion:
        case Spell.IncendiaryCloud:
        case Spell.Inferno:
        case Spell.InflictCriticalWounds:
        case Spell.InflictLightWounds:
        case Spell.InflictMinorWounds:
        case Spell.InflictModerateWounds:
        case Spell.InflictSeriousWounds:
        case Spell.IsaacsGreaterMissileStorm:
        case Spell.IsaacsLesserMissileStorm:
        case Spell.LightningBolt:
        case Spell.MagicMissile:
        case Spell.MelfsAcidArrow:
        case Spell.MestilsAcidBreath:
        case Spell.MeteorSwarm:
        case Spell.NegativeEnergyBurst:
        case Spell.NegativeEnergyRay:
        case Spell.PhantasmalKiller:
        case Spell.Poison:
        case Spell.PowerWordKill:
        case Spell.Quillfire:
        case Spell.RayOfFrost:
        case Spell.SearingLight:
        case Spell.ShadesConeOfCold:
        case Spell.ShadesFireball:
        case Spell.ShadesWallOfFire:
        case Spell.ShadowConjurationMagicMissile:
        case Spell.SlayLiving:
        case Spell.SoundBurst:
        case Spell.SpikeGrowth:
        case Spell.StormOfVengeance:
        case Spell.Sunbeam:
        case Spell.Sunburst:
        case Spell.UndeathsEternalFoe:
        case Spell.UndeathToDeath:
        case Spell.VampiricTouch:
        case Spell.WailOfTheBanshee:
        case Spell.WallOfFire:
        case Spell.Weird:
        case Spell.WordOfFaith:
          onSpellCast.PreventSpellCast = true;

          if (onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oPC)
            oPC.ControllingPlayer.SendServerMessage("L'interdiction d'usage de magie offensive est en vigueur.", ColorConstants.Red);
          break;
      }
    }
  }
}
