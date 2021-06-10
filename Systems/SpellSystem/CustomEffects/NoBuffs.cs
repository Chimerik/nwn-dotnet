using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  static class NoBuff
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= NoBuffMalus;
      oTarget.OnSpellCast += NoBuffMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      foreach (Effect effect in oTarget.ActiveEffects.Where(e => e.Tag != "_ARENA_CUTSCENE_PARALYZE_EFFECT"))
        oTarget.RemoveEffect(effect);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDispelDisjunction));
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnSpellCast -= NoBuffMalus;
    }
    private static void NoBuffMalus(OnSpellCast onSpellCast)
    {
      switch (onSpellCast.Spell)
      {
        case Spell.Aid:
        case Spell.Amplify:
        case Spell.Auraofglory:
        case Spell.AuraOfVitality:
        case Spell.Awaken:
        case Spell.Barkskin:
        case Spell.Battletide:
        case Spell.BladeThirst:
        case Spell.Bless:
        case Spell.BlessWeapon:
        case Spell.BloodFrenzy:
        case Spell.BullsStrength:
        case Spell.Camoflage:
        case Spell.CatsGrace:
        case Spell.Charger:
        case Spell.ClairaudienceAndClairvoyance:
        case Spell.Clarity:
        case Spell.CloakOfChaos:
        case Spell.ContinualFlame:
        case Spell.Darkfire:
        case Spell.Darkvision:
        case Spell.DeathArmor:
        case Spell.DeathWard:
        case Spell.Displacement:
        case Spell.DivineFavor:
        case Spell.DivineMight:
        case Spell.DivinePower:
        case Spell.DivineShield:
        case Spell.EagleSpledor:
        case Spell.ElementalShield:
        case Spell.Endurance:
        case Spell.EndureElements:
        case Spell.EnergyBuffer:
        case Spell.EntropicShield:
        case Spell.EpicMageArmor:
        case Spell.Etherealness:
        case Spell.EtherealVisage:
        case Spell.ExpeditiousRetreat:
        case Spell.FlameWeapon:
        case Spell.FoxsCunning:
        case Spell.FreedomOfMovement:
        case Spell.GhostlyVisage:
        case Spell.GlobeOfInvulnerability:
        case Spell.GlyphOfWarding:
        case Spell.GreaterBullsStrength:
        case Spell.GreaterCatsGrace:
        case Spell.GreaterEagleSplendor:
        case Spell.GreaterEndurance:
        case Spell.GreaterFoxsCunning:
        case Spell.GreaterMagicFang:
        case Spell.GreaterMagicWeapon:
        case Spell.GreaterOwlsWisdom:
        case Spell.GreaterShadowConjurationMinorGlobe:
        case Spell.GreaterShadowConjurationMirrorImage:
        case Spell.GreaterSpellMantle:
        case Spell.GreaterStoneskin:
        case Spell.Haste:
        case Spell.HolyAura:
        case Spell.HolySword:
        case Spell.ImprovedInvisibility:
        case Spell.Invisibility:
        case Spell.InvisibilitySphere:
        case Spell.Ironguts:
        case Spell.KeenEdge:
        case Spell.LesserMindBlank:
        case Spell.LesserSpellMantle:
        case Spell.Light:
        case Spell.MageArmor:
        case Spell.MagicCircleAgainstChaos:
        case Spell.MagicCircleAgainstEvil:
        case Spell.MagicCircleAgainstGood:
        case Spell.MagicCircleAgainstLaw:
        case Spell.MagicFang:
        case Spell.MagicVestment:
        case Spell.MagicWeapon:
        case Spell.MassCamoflage:
        case Spell.MassHaste:
        case Spell.MestilsAcidSheath:
        case Spell.MindBlank:
        case Spell.MinorGlobeOfInvulnerability:
        case Spell.MonstrousRegeneration:
        case Spell.NegativeEnergyProtection:
        case Spell.OneWithTheLand:
        case Spell.OwlsInsight:
        case Spell.OwlsWisdom:
        case Spell.PolymorphSelf:
        case Spell.Prayer:
        case Spell.Premonition:
        case Spell.ProtectionFromChaos:
        case Spell.ProtectionFromElements:
        case Spell.ProtectionFromEvil:
        case Spell.ProtectionFromGood:
        case Spell.ProtectionFromLaw:
        case Spell.ProtectionFromSpells:
        case Spell.Regenerate:
        case Spell.Resistance:
        case Spell.ResistElements:
        case Spell.Sanctuary:
        case Spell.SeeInvisibility:
        case Spell.ShadesStoneskin:
        case Spell.ShadowConjurationInivsibility:
        case Spell.ShadowConjurationMageArmor:
        case Spell.ShadowShield:
        case Spell.Shapechange:
        case Spell.Shield:
        case Spell.ShieldOfFaith:
        case Spell.ShieldOfLaw:
        case Spell.Silence:
        case Spell.SpellMantle:
        case Spell.SpellResistance:
        case Spell.StoneBones:
        case Spell.StoneToFlesh:
        case Spell.TensersTransformation:
        case Spell.TimeStop:
        case Spell.TrueSeeing:
        case Spell.TrueStrike:
        case Spell.TymorasSmile:
        case Spell.Virtue:
        case Spell.WarCry:
        case Spell.WoundingWhispers:
          onSpellCast.PreventSpellCast = true;

          if (onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oPC)
            oPC.ControllingPlayer.SendServerMessage("L'interdiction d'usage de magie défensive est en vigueur !", ColorConstants.Red);
          break;
      }
    }
  }
}
