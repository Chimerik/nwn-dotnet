using System;
using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.PlayerSystem;
namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void SpellSwitch(NwGameObject oCaster, NwSpell spell, NwFeat feat, SpellEntry spellEntry, NwGameObject target, Location targetLocation, NwClass castingClass)
    {
      List<NwGameObject> concentrationTargets = new();
      
      switch (spell.SpellType)
      {
        case Spell.AcidSplash:
          SpellSystem.AcidSplash(oCaster, spell, spellEntry, target, targetLocation, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Enervation:
          SpellSystem.Fletrissement(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ElectricJolt:
          SpellSystem.ElectricJolt(oCaster, spell, spellEntry, target, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Light:
          SpellSystem.Light(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.RayOfFrost:
          SpellSystem.RayOfFrost(oCaster, spell, spellEntry, target, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.IceDagger:
          SpellSystem.DagueDeGivre(oCaster, spell, spellEntry, target, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.TrueStrike:
          concentrationTargets.AddRange(SpellSystem.TrueStrike(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Flare:
          SpellSystem.SacredFlame(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Sleep:
          SpellSystem.Sommeil(oCaster, spell, spellEntry, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

         case Spell.Virtue:
           SpellSystem.SouffleDeGuerison(oCaster, spell, spellEntry, target, castingClass);
           oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
           break;

        case Spell.HealingCircle:
          SpellSystem.PriereDeGuerison(oCaster, spell, spellEntry, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.CureSeriousWounds:
          SpellSystem.SouffleDeGuerisonDeGroupe(oCaster, spell, spellEntry, castingClass, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.MassHeal:
          SpellSystem.SoinsDeGroupe(oCaster, spell, spellEntry, castingClass, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.RaiseDead:
          SpellSystem.Rejuvenation(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.DeathWard:
          SpellSystem.ProtectionContreLaMort(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Invisibility:
          concentrationTargets.AddRange(SpellSystem.Invisibility(oCaster, spell, spellEntry, target, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ImprovedInvisibility:
          SpellSystem.ImprovedInvisibility(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.InflictMinorWounds:
          SpellSystem.SimulacreDeVie(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        /*case Spell.FleshToStone:
          Petrify(onSpellCast);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;*/

        case Spell.Darkness:
          concentrationTargets.AddRange(SpellSystem.Darkness(oCaster, spell, feat, spellEntry, target, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.EpicDragonKnight:
          concentrationTargets.AddRange(SpellSystem.InvocationDespritDragon(oCaster, spell, spellEntry, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Silence:
          if(oCaster is NwCreature caster && feat is not null && feat.Id == CustomSkill.MonkSilence)
          {
            caster.IncrementRemainingFeatUses(feat.FeatType);
            FeatUtils.DecrementKi(caster, 2);
          }
          break;

        case Spell.Darkvision:
          if (oCaster is NwCreature castCreature && feat is not null && feat.Id == CustomSkill.MonkDarkVision)
          {
            castCreature.IncrementRemainingFeatUses(feat.FeatType);
            FeatUtils.DecrementKi(castCreature, 2);
          }
          break;

        case Spell.BurningHands:
          SpellSystem.BurningHands(oCaster, spell, spellEntry, target, castingClass, targetLocation, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ConeOfCold:
          SpellSystem.ConeDeFroid(oCaster, spell, spellEntry, target, castingClass, targetLocation, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Balagarnsironhorn:
          SpellSystem.VagueTonnante(oCaster, spell, spellEntry, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.FlameStrike:
          SpellSystem.FlameStrike(oCaster, spell, spellEntry, target, castingClass, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Stoneskin:
          concentrationTargets.AddRange(SpellSystem.PeauDePierre(oCaster, spell, spellEntry, target, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.IceStorm:
          SpellSystem.TempeteDeGrele(oCaster, spell, spellEntry, target, castingClass, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Bane:
          concentrationTargets.AddRange(SpellSystem.Fleau(oCaster, spell, spellEntry, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Bless:
          concentrationTargets.AddRange(SpellSystem.Benediction(oCaster, spell, spellEntry, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Confusion:
          concentrationTargets.AddRange(SpellSystem.Confusion(oCaster, spell, spellEntry, targetLocation, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.HoldPerson:
          concentrationTargets.AddRange(SpellSystem.ImmobilisationDePersonne(oCaster, spell, spellEntry, target, castingClass, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.HoldMonster:
          concentrationTargets.AddRange(SpellSystem.ImmobilisationDeMonstre(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
          
        case Spell.Haste:
          concentrationTargets.AddRange(SpellSystem.Hate(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.AbilityTurnUndead:
          SpellSystem.RenvoiDesMortsVivants(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.CharmPerson:
          SpellSystem.CharmePersonne(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Fear:
          concentrationTargets.AddRange(SpellSystem.Terreur(oCaster, spell, spellEntry, target, castingClass, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ShieldOfFaith:
          concentrationTargets.AddRange(SpellSystem.BouclierDeLaFoi(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.DivineFavor:
          concentrationTargets.AddRange(SpellSystem.FaveurDivine(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.MagicWeapon:
          SpellSystem.ArmeMagique(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ShelgarnsPersistentBlade:
          SpellSystem.ArmeSpirituelle(oCaster, spell, spellEntry, targetLocation, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Firebrand:
          SpellSystem.RayonArdent(oCaster, spell, spellEntry, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Fireball:
          SpellSystem.BouleDeFeu(oCaster, spell, spellEntry, target, castingClass, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.WallOfFire:
          concentrationTargets.AddRange(SpellSystem.MurDeFeu(oCaster, spell, spellEntry, castingClass, targetLocation, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Barkskin:
          concentrationTargets.AddRange(SpellSystem.PeauDecorce(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.DominateAnimal:
          concentrationTargets.AddRange(SpellSystem.DominationAnimale(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Slow:
          concentrationTargets.AddRange(SpellSystem.Lenteur(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Identify:
          SpellSystem.Identification(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.LegendLore:
          SpellSystem.MythesEtLegendes(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.GustOfWind:
          concentrationTargets.AddRange(SpellSystem.Bourrasque(oCaster, spell, spellEntry, castingClass, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.CallLightning:
          concentrationTargets.AddRange(SpellSystem.AppelDeLaFoudre(oCaster, spell, spellEntry, castingClass, target is null ? targetLocation : target.Location));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.CureModerateWounds:
          SpellSystem.Soins(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Heal:
          SpellSystem.Guerison(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.LesserRestoration:
          SpellSystem.RestaurationPartielle(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.GreaterRestoration:
          SpellSystem.RestaurationSupreme(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.NaturesBalance:
          SpellSystem.AssistanceTerrestre(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
      }

      switch (spell.Id)
      {
        case CustomSpell.DechargeOcculte:
          SpellSystem.DechargeOcculte(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Augure:
          SpellSystem.Augure(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        /*case CustomSpell.ModificationDapparence:
          concentrationTargets.AddRange(SpellSystem.ModificationDapparence(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;*/

        case CustomSpell.FormeGazeuse:
          SpellSystem.FormeGazeuse(oCaster, spell, spellEntry, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Antidetection:
          SpellSystem.Antidetection(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.OeilMagique:
          SpellSystem.OeilMagique(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ProtectionContreLeMalEtLeBien:
          concentrationTargets.AddRange(SpellSystem.ProtectionContreLeMalEtLeBien(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BladeWard:
          SpellSystem.BladeWard(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FireBolt:
          SpellSystem.FireBolt(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Friends:
          concentrationTargets.AddRange(SpellSystem.Friends(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BoneChill:
          SpellSystem.BoneChill(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PoisonSpray:
          SpellSystem.PoisonSpray(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FaerieFire:
          concentrationTargets.AddRange(SpellSystem.FaerieFire(oCaster, spell, spellEntry, target, targetLocation, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Enlarge:
          concentrationTargets.AddRange(SpellSystem.Enlarge(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SpeakAnimal:
          SpellSystem.SpeakAnimal(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ProduceFlame:
          SpellSystem.ProduceFlame(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MageHand:
          SpellSystem.MageHand(oCaster, spell, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Thaumaturgy:
          SpellSystem.Thaumaturgy(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Serenity:
          SpellSystem.Serenity(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Sprint:
          SpellSystem.Sprint(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Stealth:
          SpellSystem.Stealth(oCaster);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Disengage:
          SpellSystem.Disengage(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Dodge:
          SpellSystem.Dodge(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.TirPerforant:
          SpellSystem.TirPerforant(oCaster, spell, spellEntry, target, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PresenceIntimidante:
          SpellSystem.PresenceIntimidante(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FlameBlade:
          concentrationTargets.AddRange(SpellSystem.FlameBlade(oCaster, spell));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SearingSmite:
          SpellSystem.SearingSmite(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BrandingSmite:
          concentrationTargets.AddRange(SpellSystem.BrandingSmite(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SensAnimal:
          concentrationTargets.AddRange(SpellSystem.SensAnimal(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CommunionAvecLaNature:
          SpellSystem.CommunionAvecLaNature(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PassageParLesArbres:
          SpellSystem.PassageParLesArbres(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.HurlementGalvanisant:
          SpellSystem.HurlementGalvanisant(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PassageSansTrace:
          concentrationTargets.AddRange(SpellSystem.PassageSansTrace(oCaster, spell, feat, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DetectionDesPensees:
          concentrationTargets.AddRange(SpellSystem.DetectionDesPensees(oCaster, spell, feat, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RegardHypnotique:
          concentrationTargets.AddRange(SpellSystem.RegardHypnotique(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.IllusionMineure:
          SpellSystem.IllusionMineure(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.InvocationPermutation:
          SpellSystem.InvocationPermutation(oCaster, spell, spellEntry, target, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Deguisement:
          SpellSystem.Deguisement(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Scrutation:
          SpellSystem.Scrutation(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CordeEnchantee:
          SpellSystem.CordeEnchantee(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ApparencesTrompeuses:
          SpellSystem.ApparencesTrompeuses(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MauvaisAugure:
          SpellSystem.MauvaisAugure(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SpiderCocoon:
          SpellSystem.SpiderCocoon(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.WildMagicTeleportation:
          SpellSystem.WildMagicTeleportation(oCaster, spell, spellEntry, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DevotionArmeSacree:
          SpellSystem.ArmeSacree(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DevotionRenvoiDesImpies:
          SpellSystem.RenvoiDesImpies(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.AnciensRenvoiDesInfideles:
          SpellSystem.RenvoiDesInfideles(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ConspuerLennemi:
          SpellSystem.ConspuerLennemi(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.LueurDespoir:
          concentrationTargets.AddRange(SpellSystem.LueurDespoir(oCaster, spell, spellEntry, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.AnciensCourrouxDeLaNature:
          SpellSystem.CourrouxDeLaNature(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FrappePiegeuse:
          concentrationTargets.AddRange(SpellSystem.FrappePiegeuse(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Communion:
          SpellSystem.Communion(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FouleeBrumeuse:
          SpellSystem.FouleeBrumeuse(oCaster, spell, spellEntry, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PorteDimensionnelle:
          SpellSystem.PorteDimensionnelle(oCaster, spell, spellEntry, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RayonDeLune:
          concentrationTargets.AddRange(SpellSystem.RayonDeLune(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ProtectionContreLacide:
        case CustomSpell.ProtectionContreLeFroid:
        case CustomSpell.ProtectionContreLeFeu:
        case CustomSpell.ProtectionContreLelectricite:
        case CustomSpell.ProtectionContreLeTonnerre:
          concentrationTargets.AddRange(SpellSystem.ProtectionContreLenergie(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CroissanceVegetale:
          concentrationTargets.AddRange(SpellSystem.CroissanceVegetale(oCaster, spell, spellEntry, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CroissanceDepines:
          concentrationTargets.AddRange(SpellSystem.CroissanceDepines(oCaster, spell, spellEntry, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MarqueDuChasseur:
          concentrationTargets.AddRange(SpellSystem.MarqueDuChasseur(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Bannissement:
          concentrationTargets.AddRange(SpellSystem.Bannissement(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BenedictionEscroc:
          concentrationTargets.AddRange(SpellSystem.BenedictionEscroc(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RepliqueInvoquee:
          concentrationTargets.AddRange(SpellSystem.RepliqueInvoquee(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ImageMiroir:
          SpellSystem.ImageMiroir(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MaledictionAttaque:
          concentrationTargets.AddRange(SpellSystem.MaledictionAttaque(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MaledictionDegats:
          concentrationTargets.AddRange(SpellSystem.MaledictionDegats(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MaledictionEffroi:
          concentrationTargets.AddRange(SpellSystem.MaledictionEffroi(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MaledictionForce:
        case CustomSpell.MaledictionDexterite:
        case CustomSpell.MaledictionConstitution:
        case CustomSpell.MaledictionIntelligence:
        case CustomSpell.MaledictionSagesse:
          concentrationTargets.AddRange(SpellSystem.MaledictionCaracteristique(oCaster, spell, spellEntry, target, castingClass, spell.Id));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SavoirAncestralDexterite:
        case CustomSpell.SavoirAncestralConstitution:
        case CustomSpell.SavoirAncestralIntelligence:
        case CustomSpell.SavoirAncestralSagesse:
        case CustomSpell.SavoirAncestralCharisme:
          SpellSystem.SavoirAncestral(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.EspritsGardiens:
        case CustomSpell.EspritsGardiensRadiant:
        case CustomSpell.EspritsGardiensNecrotique:
          concentrationTargets.AddRange(SpellSystem.EspritsGardiens(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CapeDuCroise:
          concentrationTargets.AddRange(SpellSystem.CapeDuCroise(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SphereDeFeu:
          concentrationTargets.AddRange(SpellSystem.SphereDeFeu(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.LumiereDuJour:
          SpellSystem.LumiereDuJour(oCaster, spell, spellEntry, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SphereResilienteDotiluke:
          SpellSystem.SphereResilienteDotiluke(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Telekinesie:
          SpellSystem.Telekinesie(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.VagueDestructrice:
        case CustomSpell.VagueDestructriceRadiant:
        case CustomSpell.VagueDestructriceNecrotique:
          SpellSystem.VagueDestructrice(oCaster, spell, spellEntry, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.AmitieAnimale:
          SpellSystem.AmitieAnimale(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CharmePlanteEtAnimaux:
          SpellSystem.CharmePlanteEtAnimaux(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FurieElementaire:
        case CustomSpell.FurieElementaireFeu:
        case CustomSpell.FurieElementaireFoudre:
        case CustomSpell.FurieElementaireFroid:
          SpellSystem.FurieElementaire(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.TempeteDeNeige:
          concentrationTargets.AddRange(SpellSystem.TempeteDeNeige(oCaster, spell, spellEntry, targetLocation, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.LianeAvide:
          concentrationTargets.AddRange(SpellSystem.LianeAvide(oCaster, spell, spellEntry, castingClass, targetLocation));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Levitation:
          concentrationTargets.AddRange(SpellSystem.Levitation(oCaster, spell, spellEntry, castingClass, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FleauDinsectes:
          concentrationTargets.AddRange(SpellSystem.FleauDinsectes(oCaster, spell, spellEntry, targetLocation, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Injonction:
          SpellSystem.Injonction(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Apaisement:
          concentrationTargets.AddRange(SpellSystem.Apaisement(oCaster, spell, spellEntry, targetLocation, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CommunicationAvecLesMorts:
          SpellSystem.CommunicationAvecLesMorts(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.NappeDeBrouillard:
          SpellSystem.NappeDeBrouillard(oCaster, spell, spellEntry, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Fracassement:
          SpellSystem.Fracassement(oCaster, spell, spellEntry, castingClass, targetLocation, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ControleDeLeau:
          SpellSystem.ControleDeLeau(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PoingDeLair:
          SpellSystem.PoingDeLair(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FouetDeLonde:
          SpellSystem.FouetDeLonde(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MurDePierre:
          concentrationTargets.AddRange(SpellSystem.MurDePierre(oCaster, spell, spellEntry, castingClass, targetLocation, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.OrbeChromatique:
        case CustomSpell.OrbeChromatiqueAcide:
        case CustomSpell.OrbeChromatiqueFoudre:
        case CustomSpell.OrbeChromatiqueFeu:
        case CustomSpell.OrbeChromatiqueFroid:
        case CustomSpell.OrbeChromatiquePoison:
          SpellSystem.OrbeChromatique(oCaster, spell, spellEntry, target, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Terrassement:
          SpellSystem.Terrassement(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Charge:
          SpellSystem.Charge(oCaster, spell, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MorsureAlaJugulaire:
          SpellSystem.MorsureAlaJugulaire(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Rupture:
          SpellSystem.Rupture(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RageDelOurs:
          SpellSystem.RageDelOurs(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ExcretionCorrosive:
          SpellSystem.ExcretionCorrosive(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RayonEmpoisonne:
          SpellSystem.RayonEmpoisonne(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.LueurEtoilee:
          SpellSystem.LueurEtoilee(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RugissementProvoquant:
          SpellSystem.RugissementProvoquant(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.TrancheVue:
          SpellSystem.TrancheVue(oCaster);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.VortexDechaine:
          SpellSystem.VortexDechaine(oCaster, targetLocation, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.GlaiseMetallisee:
          SpellSystem.GlaiseMetallisee(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.VapeurElementaire:
          SpellSystem.VapeurElementaire(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
          
        case CustomSpell.ImmolationVolontaire:
          concentrationTargets.AddRange(SpellSystem.ImmolationVolontaire(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RespirationAquatique:
          SpellSystem.RespirationAquatique(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PuitsDeLune:
          concentrationTargets.AddRange(SpellSystem.PuitsDeLune(oCaster, spell, spellEntry, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
      }

        if (oCaster is NwCreature castingCreature)
      {
        if (castingClass is not null)
        {
          byte spellLevel = spell.GetSpellLevelForClass(castingClass);
          SpellSystem.OnSpellCastAbjurationWard(castingCreature, spell, spellLevel);
          SpellSystem.OnSpellCastDivinationExpert(castingCreature, spell, castingClass);
          SpellSystem.OnSpellCastInvocationPermutation(castingCreature, spell, spellLevel);
          SpellSystem.OnSpellCastTransmutationStone(castingCreature, spell, spellLevel);
          WizardUtils.HandleEvocateurSurchargeSelfDamage(castingCreature, spellLevel, spell.SpellSchool);
          EnsoUtils.HandleMagieTempetueuse(castingCreature, spellLevel);

          if (castingClass.ClassType == ClassType.Paladin && Players.TryGetValue(castingCreature, out Player player))
          {
            byte chatimentLevel = (byte)(player.windows.TryGetValue("chatimentLevelSelection", out var chatimentWindow)
            ? ((ChatimentLevelSelectionWindow)chatimentWindow).selectedSpellLevel : 1);

            if(spellLevel == chatimentLevel)
              player.oid.LoginCreature.DecrementRemainingFeatUses((Feat)CustomSkill.ChatimentDivin);
          }

          if(castingClass.ClassType == (ClassType)CustomClass.Occultiste)
          {
            var occultisteClass = castingCreature.GetClassInfo((ClassType)CustomClass.Occultiste);
            byte remainingSlots = occultisteClass.GetRemainingSpellSlots(1);
            byte consumedSlots = (byte)(remainingSlots - 1);

            for (byte i = 1; i < 10; i++)
              if(i != spell.GetSpellLevelForClass((ClassType)CustomClass.Occultiste))
                occultisteClass.SetRemainingSpellSlots(i, consumedSlots);

            castingCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentOcculte, consumedSlots);
          }
        }

        if(spellEntry.requiresConcentration)
          EffectSystem.ApplyConcentrationEffect(castingCreature, spell.Id, concentrationTargets, spellEntry.duration);
      }

      oCaster.GetObjectVariable<LocalVariableInt>(SpellConfig.CurrentSpellVariable).Delete();
      oCaster.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;
    }
  }
}
