using System;
using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.PlayerSystem;
using System.Linq;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void SpellSwitch(NwGameObject oCaster, NwSpell spell, NwFeat feat, SpellEntry spellEntry, NwGameObject target, Location targetLocation, NwClass castingClass)
    {
      NwCreature castingCreature = null;
      bool castingFamiliar = false;

      if (oCaster is NwCreature caster)
      {
        castingCreature = caster;

        if (castingCreature.Tag == EffectSystem.repliqueTag)
        {
          castingCreature = castingCreature.Master;
          castingFamiliar = true;
        } 
      }

      List<NwGameObject> concentrationTargets = new();

      switch (spell.SpellType)
      {
        case Spell.AcidSplash:
          SpellSystem.AcidSplash(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.AbilityEpicCurseSong:
          SpellSystem.MoquerieVicieuse(oCaster, spell, spellEntry, target, castingClass, feat);
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
          SpellSystem.Light(oCaster, spell, spellEntry, target, castingClass, feat);
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

        case Spell.Grease:
          SpellSystem.Graisse(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Flare:
          SpellSystem.SacredFlame(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Sleep:
          concentrationTargets.AddRange(SpellSystem.Sommeil(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

         case Spell.Virtue:
           SpellSystem.SouffleDeGuerison(oCaster, spell, spellEntry, target, castingClass);
           oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
           break;

        case Spell.HealingCircle:
          SpellSystem.PriereDeGuerison(oCaster, spell, spellEntry, castingClass, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.CureSeriousWounds:
          SpellSystem.SouffleDeGuerisonDeGroupe(oCaster, target, spell, spellEntry, castingClass);
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

        case Spell.Darkvision:
          if (castingCreature is not null && feat is not null && feat.Id == CustomSkill.MonkDarkVision)
          {
            castingCreature.IncrementRemainingFeatUses(feat.FeatType);
            FeatUtils.DecrementKi(castingCreature, 2);
          }
          break;

        case Spell.BurningHands:
          SpellSystem.BurningHands(oCaster, spell, spellEntry, castingClass, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ConeOfCold:
          SpellSystem.ConeDeFroid(oCaster, spell, spellEntry, target, castingClass, targetLocation);
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
          concentrationTargets.AddRange(SpellSystem.ImmobilisationDePersonne(oCaster, spell, spellEntry, target, castingClass));
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

        case Spell.CharmPersonOrAnimal:
          SpellSystem.CharmAnimal(oCaster, spell, spellEntry, target, castingClass);
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
          SpellSystem.FaveurDivine(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.MagicWeapon:
          SpellSystem.ArmeMagique(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ShelgarnsPersistentBlade:
          SpellSystem.ArmeSpirituelle(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Firebrand:
          SpellSystem.RayonArdent(oCaster, spell, spellEntry, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Fireball:
          SpellSystem.BouleDeFeu(oCaster, spell, spellEntry, castingClass, target is null ? targetLocation : target.Location);
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
          concentrationTargets.AddRange(SpellSystem.Lenteur(oCaster, spell, spellEntry, target, castingClass));
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
          concentrationTargets.AddRange(SpellSystem.Bourrasque(oCaster, spell, spellEntry, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.CallLightning:
          if (oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.AppelDeLaFoudreEffectTag))
            concentrationTargets.AddRange(SpellSystem.AppelDeLaFoudre(oCaster, spell, spellEntry, castingClass, target is null ? targetLocation : target.Location));
          else
            SpellSystem.AppelDeLaFoudreRecast(oCaster, spell, spellEntry, castingClass, target is null ? targetLocation : target.Location);

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
        case CustomSpell.Aide:
          SpellSystem.Aide(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ProjectileMagique:
          SpellSystem.ProjectileMagique(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DechargeOcculte:
          SpellSystem.DechargeOcculte(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Assistance:
          concentrationTargets.AddRange(SpellSystem.Assistance(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Enchevetrement:
          concentrationTargets.AddRange(SpellSystem.Enchevetrement(oCaster, spell, spellEntry, target is null ? targetLocation :  target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ToileDaraignee:
          concentrationTargets.AddRange(SpellSystem.ToileDaraignee(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.VerrouArcanique:
          SpellSystem.VerrouArcanique(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Resistance:
        case CustomSpell.ResistanceAcide:
        case CustomSpell.ResistanceContondant:
        case CustomSpell.ResistanceElec:
        case CustomSpell.ResistanceFeu:
        case CustomSpell.ResistanceFroid:
        case CustomSpell.ResistancePercant:
        case CustomSpell.ResistancePoison:
        case CustomSpell.ResistanceTranchant:
          concentrationTargets.AddRange(SpellSystem.Resistance(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.AmeliorationForce:
        case CustomSpell.AmeliorationDexterite:
        case CustomSpell.AmeliorationConstitution:
        case CustomSpell.AmeliorationIntelligence:
        case CustomSpell.AmeliorationSagesse:
        case CustomSpell.AmeliorationCharisme:
          concentrationTargets.AddRange(SpellSystem.AmeliorationCaracteristique(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Augure:
          SpellSystem.Augure(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.TexteIllusoire:
          SpellSystem.TexteIllusoire(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.AppelDeFamilierBat:
        case CustomSpell.AppelDeFamilierCat:
        case CustomSpell.AppelDeFamilierRat:
        case CustomSpell.AppelDeFamilierSpider:
        case CustomSpell.AppelDeFamilierOwl:
        case CustomSpell.AppelDeFamilierRaven:
        case CustomSpell.AppelDeFamilierCrapaud:
        case CustomSpell.PacteDeLaChaineDiablotin:
        case CustomSpell.PacteDeLaChaineEspritFollet:
        case CustomSpell.PacteDeLaChainePseudoDragon:
        case CustomSpell.PacteDeLaChaineQuasit:
        case CustomSpell.PacteDeLaChaineSerpent:
        case CustomSpell.PacteDeLaChaineSphinx:
        case CustomSpell.PacteDeLaChaineSquelette:
        case CustomSpell.PacteDeLaChaineTetard:
          SpellSystem.AppelDeFamilier(oCaster, spell, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.AppelDeFamilierRename:
          SpellSystem.AssociateRename(oCaster, AssociateType.Familiar);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PurificationDeNourritureEtBoisson:
          SpellSystem.PurificationDeNourritureEtBoisson(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DomestiqueInvisible:
          SpellSystem.DomestiqueInvisible(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FeuilleMorte:
          SpellSystem.FeuilleMorte(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Bouclier:
          SpellSystem.Bouclier(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Elementalisme:
          SpellSystem.Elementalisme(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DetectionDuBienEtDuMal:
          SpellSystem.DetectionDuBienEtDuMal(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Message:
          SpellSystem.Message(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CoupDeTonnerre:
          SpellSystem.CoupDeTonnerre(oCaster, spell, spellEntry, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MotRayonnant:
          SpellSystem.MotRayonnant(oCaster, spell, spellEntry, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CouleursDansantes:
          SpellSystem.CouleursDansantes(oCaster, spell, spellEntry, castingClass, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CeciteSurdite:
          SpellSystem.CeciteSurdite(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SonneLeGlas:
          SpellSystem.SonneLeGlas(oCaster, spell, spellEntry, target, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DuelForce:
          SpellSystem.DuelForce(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Blessure:
          SpellSystem.Blessure(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
          
        case CustomSpell.FractureMentale:
          SpellSystem.FractureMentale(oCaster, spell, spellEntry, target, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ArmuredAgathys:
          SpellSystem.ArmuredAgathys(oCaster, spell, spellEntry);
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

        case CustomSpell.Heroisme:
          concentrationTargets.AddRange(SpellSystem.Heroisme(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MaleficeForce:
        case CustomSpell.MaleficeDexterite:
        case CustomSpell.MaleficeConstitution:
        case CustomSpell.MaleficeIntelligence:
        case CustomSpell.MaleficeSagesse:
        case CustomSpell.MaleficeCharisme:
          concentrationTargets.AddRange(SpellSystem.Malefice(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RetraiteExpeditive:
          concentrationTargets.AddRange(SpellSystem.RetraiteExpeditive(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.GrandeFoulee:
          SpellSystem.GrandeFoulee(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ArmureDeMage:
          SpellSystem.ArmureDeMage(oCaster, spell, spellEntry, target);
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

        case CustomSpell.ProtectionContreLesLames:
          concentrationTargets.AddRange(SpellSystem.BladeWard(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FireBolt:
          SpellSystem.FireBolt(oCaster, spell, spellEntry, target, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Friends:
          concentrationTargets.AddRange(SpellSystem.Friends(oCaster, spell, spellEntry, target, castingClass, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BoneChill:
          SpellSystem.BoneChill(oCaster, spell, spellEntry, target, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PoisonSpray:
          SpellSystem.PoisonSpray(oCaster, spell, spellEntry, target, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FouRireDeTasha:
          SpellSystem.FouRireDeTasha(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FaerieFire:
          concentrationTargets.AddRange(SpellSystem.FaerieFire(oCaster, spell, spellEntry, target, targetLocation, castingClass, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Agrandissement:
        case CustomSpell.Rapetissement:
          concentrationTargets.AddRange(SpellSystem.Agrandissement(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.TraitEnsorcele:
          concentrationTargets.AddRange(SpellSystem.TraitEnsorcele(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SpeakAnimal:
          SpellSystem.SpeakAnimal(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RadianceDelAube:
          SpellSystem.RadianceDelAube(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.LienDeGarde:
          SpellSystem.LienDeGarde(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ProduceFlame:
          SpellSystem.ProduceFlame(oCaster, spell, spellEntry, target, castingClass, feat);
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

        case CustomSpell.LameArdente:
          concentrationTargets.AddRange(SpellSystem.LameArdente(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FlecheDeFoudre:
          SpellSystem.FlecheDeFoudre(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SearingSmite:
          SpellSystem.SearingSmite(oCaster, spell, spellEntry, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ChatimentTonitruant:
          SpellSystem.ChatimentTonitruant(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ChatimentDuCourroux:
          SpellSystem.ChatimentDuCourroux(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ChatimentAveuglant:
          SpellSystem.ChatimentAveuglant(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BrandingSmite:
          SpellSystem.BrandingSmite(oCaster, spell, spellEntry, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FlecheAcideDeMelf:
          SpellSystem.FlecheAcideDeMelf(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Eclair:
          SpellSystem.Eclair(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FeintTrepas:
          SpellSystem.FeintTrepas(oCaster, spell, spellEntry, target);
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

        case CustomSpell.ProtectionContreLePoison:
          SpellSystem.ProtectionContreLePoison(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Deblocage:
          SpellSystem.Deblocage(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.VisionNocturne:
          SpellSystem.VisionNocturne(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DissipationDeLaMagie:
          SpellSystem.DissipationDeLaMagie(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
        case CustomSpell.DetectionDeLaMagie:
          concentrationTargets.AddRange(SpellSystem.DetectionDeLaMagie(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DouxRepos:
          SpellSystem.DouxRepos(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.LocalisationDanimauxOuDePlantes:
          SpellSystem.LocalisationDanimauxOuDePlantes(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ZoneDeVerite:
          SpellSystem.ZoneDeVerite(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PassageSansTrace:
          concentrationTargets.AddRange(SpellSystem.PassageSansTrace(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.InvocationBestiale:
          concentrationTargets.AddRange(SpellSystem.InvocationBestiale(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ConvocationEspritSpectral:
        case CustomSpell.ConvocationEspritPutride:
        case CustomSpell.ConvocationEspritSquelettique:
          concentrationTargets.AddRange(SpellSystem.ConvocationDeMortVivant(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ConvocationFeerique:
          concentrationTargets.AddRange(SpellSystem.ConvocationFeerique(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DetectionDesPensees:
          concentrationTargets.AddRange(SpellSystem.DetectionDesPensees(oCaster, spell, feat, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PattesDaraignee:
          concentrationTargets.AddRange(SpellSystem.PattesDaraignee(oCaster, spell, spellEntry, target));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.AffluxMental:
          concentrationTargets.AddRange(SpellSystem.AffluxMental(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RegardHypnotique:
          concentrationTargets.AddRange(SpellSystem.RegardHypnotique(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MotifHypnotique:
          concentrationTargets.AddRange(SpellSystem.MotifHypnotique(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.IllusionMineure:
          SpellSystem.IllusionMineure(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ImageSuperieure:
          SpellSystem.ImageSuperieure(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FusionAvecLaPierre:
          SpellSystem.FusionAvecLaPierre(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Clairvoyance:
          SpellSystem.Clairvoyance(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CommunicationAvecLaFlore:
          SpellSystem.CommunicationAvecLaFlore(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CommunicationDistante:
          SpellSystem.CommunicationDistante(oCaster, spell);
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

        case CustomSpell.DetectionDeLinvisibilite:
          SpellSystem.DetectionDeLinvisibilite(oCaster, spell, spellEntry);
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
          SpellSystem.ArmeSacree(oCaster, spell, spellEntry);
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
          SpellSystem.ConspuerLennemi(oCaster, spell, spellEntry);
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
          SpellSystem.FouleeBrumeuse(oCaster, spell, spellEntry, targetLocation, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PorteDimensionnelle:
          SpellSystem.PorteDimensionnelle(oCaster, spell, spellEntry, targetLocation);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RayonDeLune:
          concentrationTargets.AddRange(SpellSystem.RayonDeLune(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RayonDeLuneDeplacement:
          SpellSystem.RayonDeLuneDeplacement(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.NuageNauseabond:
          concentrationTargets.AddRange(SpellSystem.NuageNauseabond(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
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

        case CustomSpell.FormeSauvageBlaireau:
        case CustomSpell.FormeSauvageAraignee:
        case CustomSpell.FormeSauvageChat:
        case CustomSpell.FormeSauvageDilophosaure:
        case CustomSpell.FormeSauvageLoup:
        case CustomSpell.FormeSauvageOursHibou:
        case CustomSpell.FormeSauvagePanthere:
        case CustomSpell.FormeSauvageRothe:
          SpellSystem.FormeSauvage(oCaster, spell, spellEntry);
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
          concentrationTargets.AddRange(SpellSystem.MarqueDuChasseur(oCaster, spell, spellEntry, target, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Bannissement:
          concentrationTargets.AddRange(SpellSystem.Bannissement(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RepliqueInvoquee:
          SpellSystem.RepliqueInvoquee(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MoveRepliqueDuplicite:
          SpellSystem.MoveRepliqueDuplicite(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ApparitionAnimale:
          concentrationTargets.AddRange(SpellSystem.ApparitionAnimale(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ApparitionAnimaleDeplacement:
          SpellSystem.MoveApparitionAnimale(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
        
        case CustomSpell.ImageMiroir:
          SpellSystem.ImageMiroir(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Flou:
          concentrationTargets.AddRange(SpellSystem.Flou(oCaster, spell, spellEntry));
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
        case CustomSpell.MaledictionCharisme:
          concentrationTargets.AddRange(SpellSystem.MaledictionCaracteristique(oCaster, spell, spellEntry, target, castingClass, spell.Id));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SavoirAncestralDexterite:
        case CustomSpell.SavoirAncestralConstitution:
        case CustomSpell.SavoirAncestralIntelligence:
        case CustomSpell.SavoirAncestralSagesse:
        case CustomSpell.SavoirAncestralCharisme:
        case CustomSpell.SavoirAncestralForce:
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

        case CustomSpell.Sanctuaire:
          SpellSystem.Sanctuaire(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SphereDeFeu:
          concentrationTargets.AddRange(SpellSystem.SphereDeFeu(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SphereDeFeuDeplacement:
          SpellSystem.SphereDeFeuDeplacement(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.NueeDeDagues:
          concentrationTargets.AddRange(SpellSystem.NueeDeDagues(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.NueeDeDaguesDeplacement:
          SpellSystem.NueeDeDaguesDeplacement(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CordonDeFleches:
          SpellSystem.CordonDeFleches(oCaster, spell, spellEntry, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MetalBrulant:
          concentrationTargets.AddRange(SpellSystem.MetalBrulant(oCaster, spell, spellEntry, target, castingClass));
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

        case CustomSpell.Fascination:
          SpellSystem.Fascination(oCaster, spell, spellEntry, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.VagueDestructrice:
        case CustomSpell.VagueDestructriceRadiant:
        case CustomSpell.VagueDestructriceNecrotique:
          SpellSystem.VagueDestructrice(oCaster, spell, spellEntry, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RestaurationAveuglement:
        case CustomSpell.RestaurationSurdite:
        case CustomSpell.RestaurationParalysie:
        case CustomSpell.RestaurationEmpoisonnement:
          SpellSystem.RestaurationPartielle(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DelivranceDesMaledictions:
          SpellSystem.DelivranceDesMaledictions(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SouffleDuDragonAcide:
        case CustomSpell.SouffleDuDragonFroid:
        case CustomSpell.SouffleDuDragonFeu:
        case CustomSpell.SouffleDuDragonElec:
        case CustomSpell.SouffleDuDragonPoison:
          concentrationTargets.AddRange(SpellSystem.SouffleDuDragon(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SouffleDuDragonAcideDon:
        case CustomSpell.SouffleDuDragonFroidDon:
        case CustomSpell.SouffleDuDragonFeuDon:
        case CustomSpell.SouffleDuDragonElecDon:
        case CustomSpell.SouffleDuDragonPoisonDon:
          SpellSystem.SouffleDuDragonDon(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, feat);
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

        case CustomSpell.Silence:
          concentrationTargets.AddRange(SpellSystem.Silence(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location));
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
          
        case CustomSpell.BondSuperieur:
          SpellSystem.BondSuperieur(oCaster, spell, spellEntry, castingClass, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.TirDeBarrage:
          SpellSystem.TirDeBarrage(oCaster, spell, spellEntry, castingClass, target is null ? targetLocation : target.Location);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FureurDelOuraganFoudre:
        case CustomSpell.FureurDelOuraganTonnerre:
          SpellSystem.FureurDelOuragan(oCaster, spell, spellEntry);
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

        case CustomSpell.BrasdHadar:
          SpellSystem.BrasdHadar(oCaster, spell, spellEntry, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.GreleDepines:
          SpellSystem.GreleDepines(oCaster, spell, spellEntry, castingClass);
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
          concentrationTargets.AddRange(SpellSystem.NappeDeBrouillard(oCaster, spell, spellEntry, targetLocation));
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
          concentrationTargets.AddRange(SpellSystem.MurDePierre(oCaster, spell, spellEntry, castingClass, target is null ? targetLocation : target.Location, feat));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.EclatSorcier:
        case CustomSpell.EclatSorcierAcide:
        case CustomSpell.EclatSorcierElec:
        case CustomSpell.EclatSorcierFeu:
        case CustomSpell.EclatSorcierFroid:
        case CustomSpell.EclatSorcierPoison:
        case CustomSpell.EclatSorcierTonnerre:
        case CustomSpell.EclatSorcierPsychique:
          SpellSystem.EclatSorcier(oCaster, spell, spellEntry, target, castingClass, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.OrbeChromatique:
        case CustomSpell.OrbeChromatiqueAcide:
        case CustomSpell.OrbeChromatiqueFoudre:
        case CustomSpell.OrbeChromatiqueFeu:
        case CustomSpell.OrbeChromatiqueFroid:
        case CustomSpell.OrbeChromatiquePoison:
        case CustomSpell.OrbeChromatiqueTonnerre:
          SpellSystem.OrbeChromatique(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ArmeElementaireAcide:
        case CustomSpell.ArmeElementaireFroid:
        case CustomSpell.ArmeElementaireFeu:
        case CustomSpell.ArmeElementaireElec:
        case CustomSpell.ArmeElementaireTonnerre:
          SpellSystem.ArmeElementaire(oCaster, spell, spellEntry, target);
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

        case CustomSpell.RayonAffaiblissant:
          concentrationTargets.AddRange(SpellSystem.RayonAffaiblissant(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RepresaillesInfernales:
          SpellSystem.RepresaillesInfernales(oCaster, spell, spellEntry, target, castingClass);
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

        case CustomSpell.ContreSort:
          SpellSystem.ContreSort(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.TrancheVue:
          SpellSystem.TrancheVue(oCaster);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ToucherVampirique:

          if (oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.ToucherVampiriqueEffectTag))
            concentrationTargets.AddRange(SpellSystem.ToucherVampirique(oCaster, spell, spellEntry, castingClass, target));
          else
            SpellSystem.ToucherVampiriqueRecast(oCaster, spell, spellEntry, castingClass, target);

          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.AuraDeVitalite:

          if (oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeVitaliteEffectTag))
            concentrationTargets.AddRange(SpellSystem.AuraDeVitalite(oCaster, spell, spellEntry, castingClass));
          else
            SpellSystem.AuraDeVitaliteHeal(oCaster);
          
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

        case CustomSpell.MessagerAnimal:
          SpellSystem.MessagerAnimal(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FlammeEternelle:
          SpellSystem.FlammeEternelle(oCaster, spell, spellEntry, target);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.VigueurArcanique:
          SpellSystem.VigueurArcanique(oCaster, spell, spellEntry);
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

        case CustomSpell.BuveuseDeVieRadiant:
        case CustomSpell.BuveuseDeVieNecrotique:
        case CustomSpell.BuveuseDeViePsychique:
          SpellSystem.BuveuseDeVie(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FaveurDuMalin:
        case CustomSpell.FaveurDuMalinAttaque:
        case CustomSpell.FaveurDuMalinDegats:
        case CustomSpell.FaveurDuMalinJDS:
        case CustomSpell.FaveurDuMalinRP:
          SpellSystem.FaveurDuMalin(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ChangementDapparence:
          concentrationTargets.AddRange(SpellSystem.ChangementDapparence(oCaster, spell, spellEntry, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ImageSilencieuse:
          concentrationTargets.AddRange(SpellSystem.ImageSilencieuse(oCaster, spell, spellEntry));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Suggestion:
          concentrationTargets.AddRange(SpellSystem.Suggestion(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Quete:
          SpellSystem.Quete(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ForceFantasmagorique:
          concentrationTargets.AddRange(SpellSystem.ForceFantasmagorique(oCaster, spell, spellEntry, target, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Clignotement:
          SpellSystem.Clignotement(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.EclairTracant:
          SpellSystem.EclairTracant(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MurmuresDissonnants:
          SpellSystem.MurmuresDissonnants(oCaster, spell, spellEntry, target, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.VoraciteDhadar:
          concentrationTargets.AddRange(SpellSystem.VoraciteDhadar(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.InvocationDaberration:
          concentrationTargets.AddRange(SpellSystem.InvocationDaberration(oCaster, spell, spellEntry, target is null ? targetLocation : target.Location, castingClass));
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.Vol:
        case CustomSpell.VolAngelique:
          SpellSystem.Vol(oCaster, spell, spellEntry, feat);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RayonnementInterieur:
          SpellSystem.RayonnementInterieur(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.VoileNecrotique:
          SpellSystem.VoileNecrotique(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.RageDuBarbare:
        case CustomSpell.RageSauvage:
        case CustomSpell.RageSauvageOurs:
        case CustomSpell.RageSauvageAigle:
        case CustomSpell.RageSauvageLoup:
        case CustomSpell.PuissanceSauvageOurs:
        case CustomSpell.PuissanceSauvageAigle:
        case CustomSpell.PuissanceSauvageLoup:
        case CustomSpell.PuissanceSauvageFaucon:
        case CustomSpell.PuissanceSauvageTigre:
        case CustomSpell.PuissanceSauvageBelier:
          SpellSystem.RageDuBarbare(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
          
        case CustomSpell.AspectSauvageChouette:
        case CustomSpell.AspectSauvageSaumon:
        case CustomSpell.AspectSauvagePanthere:
          SpellSystem.AspectSauvage(oCaster, spell);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.EtincelleDivineSoins:
        case CustomSpell.EtincelleDivineRadiant:
        case CustomSpell.EtincelleDivineNecrotique:
          SpellSystem.EtincelleDivine(oCaster, target, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FrappeDivineRadiant:
        case CustomSpell.FrappeDivineNecrotique:
          SpellSystem.FrappeDivine(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.MaitreDeLaChaineRadiant:
        case CustomSpell.MaitreDeLaChaineNecrotique:
          SpellSystem.MaitreDeLaChaine(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.SecondSouffle:
          SpellSystem.SecondSouffle(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.EspritTactique:
          SpellSystem.EspritTactique(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ImpositionDesMainsMineure:
          SpellSystem.ImpositionDesMainsMineure(oCaster, target, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ImpositionDesMainsMajeure:
          SpellSystem.ImpositionDesMainsMajeure(oCaster, target, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ImpositionDesMainsGuerison:
          SpellSystem.ImpositionDesMainsGuerison(oCaster, target, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PourfendeurDeColosses:
          SpellSystem.PourfendeurDeColosses(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BriseurDeHordes:
          SpellSystem.BriseurDeHordes(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.EchapperALaHorde:
          SpellSystem.EchapperALaHorde(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.DefenseAdaptative:
          SpellSystem.DefenseAdaptative(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BelluaireOurs:
        case CustomSpell.BelluaireCorbeau:
        case CustomSpell.BelluaireLoup:
        case CustomSpell.BelluaireAraignee:
        case CustomSpell.BelluaireSanglier:
          SpellSystem.CompagnonAnimal(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.BelluaireRename:
          SpellSystem.AssociateRename(oCaster, AssociateType.AnimalCompanion);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PacteDeLaLameLier:
          SpellSystem.PacteDeLaLameLier(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PacteDeLaLameInvoquer:
          SpellSystem.PacteDeLaLameInvoquer(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.PacteDeLaLameRadiant:
        case CustomSpell.PacteDeLaLameNecrotique:
        case CustomSpell.PacteDeLaLamePsychique:
        case CustomSpell.PacteDeLaLameNormal:
          SpellSystem.PacteDeLaLameDamageType(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.FrappeRuseePoison:
        case CustomSpell.FrappeRuseeBousculade:
        case CustomSpell.FrappeRuseeRetraite:
        case CustomSpell.FrappePerfidePoison:
        case CustomSpell.FrappePerfideBousculade:
        case CustomSpell.FrappePerfideRetraite:
        case CustomSpell.FrappePerfideHebeter:
        case CustomSpell.FrappePerfideObscurcir:
        case CustomSpell.FrappePerfideAssommer:
          SpellSystem.FrappeRusee(oCaster, spell, spellEntry);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.CoupAuButNormal:
        case CustomSpell.CoupAuButRadiant:
          SpellSystem.CoupAuBut(oCaster, spell, spellEntry, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case CustomSpell.ShillelaghNormal:
        case CustomSpell.ShillelaghForce:
          SpellSystem.Shillelagh(oCaster, spell, spellEntry, castingClass);
          oCaster.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
      }

      if (castingCreature is not null)
      {
        if (castingClass is not null)
        {
          byte spellLevel = spell.MasterSpell is null ? spell.GetSpellLevelForClass(castingClass) : spell.MasterSpell.GetSpellLevelForClass(castingClass);
          SpellSystem.OnSpellCastAbjurationWard(castingCreature, spell, spellLevel);
          SpellSystem.OnSpellCastDivinationExpert(castingCreature, spell, castingClass);
          SpellSystem.OnSpellCastInvocationPermutation(castingCreature, spell, spellLevel);
          SpellSystem.OnSpellCastTransmutationStone(castingCreature, spell, spellLevel);
          WizardUtils.HandleEvocateurSurchargeSelfDamage(castingCreature, spellLevel, spell.SpellSchool);
          EnsoUtils.HandleMagieTempetueuse(castingCreature, spellLevel);
          OccultisteUtils.IncrementFouleeFeerique(castingCreature, spell.SpellSchool, spellLevel, feat);
          EnsoUtils.HandleCoeurDeLaTempete(castingCreature, spellEntry.damageType);

          if (castingClass.ClassType == ClassType.Paladin && Players.TryGetValue(castingCreature, out Player player))
          {
            byte chatimentLevel = (byte)(player.windows.TryGetValue("chatimentLevelSelection", out var chatimentWindow)
            ? ((ChatimentLevelSelectionWindow)chatimentWindow).selectedSpellLevel : 1);

            if(spellLevel == chatimentLevel)
              player.oid.LoginCreature.DecrementRemainingFeatUses((Feat)CustomSkill.ChatimentDivin);
          }
          
          if (castingClass.ClassType == (ClassType)CustomClass.Occultiste)
          {
            if (spell.MasterSpell != null && !Utils.In(spell.MasterSpell.Id, CustomSpell.AppelDeFamilier, CustomSpell.PacteDeLaChaine))
            {
              byte occultisteSpellLevel = spell.GetSpellLevelForClass((ClassType)CustomClass.Occultiste);
              if (0 < occultisteSpellLevel && occultisteSpellLevel < 6)
              {
                var occultisteClass = castingCreature.GetClassInfo((ClassType)CustomClass.Occultiste);
                byte remainingSlots = occultisteClass.GetRemainingSpellSlots(1);
                byte consumedSlots = (byte)(occultisteSpellLevel > 1 ? remainingSlots - 1 : remainingSlots);

                for (byte i = 1; i < 10; i++)
                  if (i != occultisteSpellLevel)
                    occultisteClass.SetRemainingSpellSlots(i, consumedSlots);

                castingCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentOcculte, consumedSlots);
              }
            }
          }
          else if (castingFamiliar)
          {
            var casterClassInfo = castingCreature.GetClassInfo(castingClass);
            casterClassInfo.SetRemainingSpellSlots(spellLevel, (byte)(casterClassInfo.GetRemainingSpellSlots(spellLevel) - 1));
          }
        }

        OccultisteUtils.DecrementFouleeFeerique(castingCreature, feat);

        if (spellEntry.requiresConcentration)
          EffectSystem.ApplyConcentrationEffect(castingCreature, spell.Id, concentrationTargets, GetSpellDuration(oCaster, spellEntry));
      }

      oCaster.GetObjectVariable<LocalVariableInt>(SpellConfig.CurrentSpellVariable).Delete();
      oCaster.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;
    }
  }
}
