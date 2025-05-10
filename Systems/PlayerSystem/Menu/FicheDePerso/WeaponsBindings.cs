using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private void WeaponsBindings()
        {
          proficiency.SetBindValue(player.oid, nuiToken.Token, $"Bonus de maîtrise (+{NativeUtils.GetCreatureProficiencyBonus(target)})");

          shieldVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          towerShieldVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          lightArmorVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          mediumArmorVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          heavyArmorVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          clubVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          daggerVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          spearVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          shortbowVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          longBowVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          longswordVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          shortbowVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          rapierVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          lightHammerVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          battleAxeVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          handAxeVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          dwarvenAxeVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          shurikenVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          doubleAxeVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          doubleBladeVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          direMaceVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          lightMaceVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          quarterStaffVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          sickleVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          lightCrossbowVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          dartVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          lightFlailVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          morningstarVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          slingVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          greatAxeVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          greatSwordVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          scimitarVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          halberdVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          heavyFlailVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          throwingAxeVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          warHammerVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          heavyCrossbowVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          bastardSwordVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          scytheVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          kamaVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          kukriVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          whipVisibility.SetBindValue(player.oid, nuiToken.Token, true);
          
          if (target.KnowsFeat((Feat)CustomSkill.MaitreBouclier))
            shield.SetBindValue(player.oid, nuiToken.Token, "expertise"); 
          else if(target.KnowsFeat((Feat)CustomSkill.ShieldProficiency))
            shield.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            shieldVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.ProtectionLegere, out var expertise) && expertise.currentLevel > 0)
            lightArmor.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.LightArmorProficiency))
            lightArmor.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            lightArmorVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (targetPlayer.learnableSkills.TryGetValue(CustomSkill.MaitreArmureIntermediaire, out var master) && master.currentLevel > 0)
            mediumArmor.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.MediumArmorProficiency))
            mediumArmor.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            mediumArmorVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.MaitreArmureLourde))
            heavyArmor.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.HeavyArmorProficiency))
            heavyArmor.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            heavyArmorVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.MaitreBouclier))
            towerShield.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else
            towerShieldVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseGourdin))
            club.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.ClubProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            club.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            clubVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseDague))
            dagger.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.DaggerProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            dagger.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            daggerVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseHachette))
            handAxe.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.HandAxeProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            handAxe.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            handAxeVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseMarteauLeger))           
            lightHammer.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.LightHammerProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            lightHammer.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            lightHammerVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseBaton))
            quarterStaff.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.QuarterstaffProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            quarterStaff.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            quarterStaffVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseSerpe))
            sickle.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.SickleProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            sickle.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            sickleVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseArbaleteLegere))
            lightCrossbow.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.LightCrossbowProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            lightCrossbow.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            lightCrossbowVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseDard))
            dart.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.DartProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            dart.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            dartVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseFleauLeger))
            lightFlail.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.LightFlailProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            lightFlail.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            lightFlailVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseMasseLegere))
            lightMace.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.LightMaceProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            lightMace.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            lightMaceVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseMorgenstern))
            morningstar.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.MorningstarProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            morningstar.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            morningstarVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseFronde))
            sling.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.SlingProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            sling.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            slingVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseLance))
            spear.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.SpearProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            spear.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            spearVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseArcCourt))
            shortbow.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.ShortBowProficiency) || target.KnowsFeat((Feat)CustomSkill.SimpleWeaponProficiency))
            shortbow.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            shortbowVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseHacheDeGuerre))
            battleAxe.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.BattleaxeProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            battleAxe.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            battleAxeVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseHacheDarmes))
            greatAxe.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.GreataxeProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            greatAxe.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            greatAxeVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseHacheDeLancer))
            throwingAxe.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.ThrowingAxeProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            throwingAxe.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            throwingAxeVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseEpeeCourte))
            shortSword.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.ShortSwordProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            shortSword.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            shortSwordVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseEpeeLongue))
            longsword.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.LongSwordProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            longsword.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            longswordVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseEspadon))
            greatSword.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.GreatswordProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            greatSword.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            greatSwordVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseCimeterre))
            scimitar.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.ScimitarProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            scimitar.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            scimitarVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseRapiere))
            rapier.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.RapierProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            rapier.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            rapierVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseFleauLourd))
            heavyFlail.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.HeavyFlailProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            heavyFlail.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            heavyFlailVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseMarteauDeGuerre))
            warHammer.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.WarHammerProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            warHammer.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            warHammerVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseHallebarde))
            halberd.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.HalberdProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            halberd.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            halberdVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseFouet))
            whip.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.WhipProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            whip.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            whipVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseArbaleteLourde))
            heavyCrossbow.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.HeavyCrossbowProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            heavyCrossbow.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            heavyCrossbowVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseArcLong))
            longBow.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.LongBowProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            longBow.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            longBowVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseShuriken))
            shuriken.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.ShurikenProficiency) || target.KnowsFeat((Feat)CustomSkill.MartialWeaponProficiency))
            shuriken.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            shurikenVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseEpeeBatarde))
            bastardSword.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.BastardswordProficiency) || target.KnowsFeat((Feat)CustomSkill.ExoticWeaponProficiency))
            bastardSword.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            bastardSwordVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseKatana))
            katana.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.KatanaProficiency) || target.KnowsFeat((Feat)CustomSkill.ExoticWeaponProficiency))
            katana.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            katanaVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseEpeeBatarde))
            bastardSword.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.BastardswordProficiency) || target.KnowsFeat((Feat)CustomSkill.ExoticWeaponProficiency))
            bastardSword.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            bastardSwordVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseLameDouble))
            doubleBlade.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.DoubleBladeProficiency) || target.KnowsFeat((Feat)CustomSkill.ExoticWeaponProficiency))
            doubleBlade.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            doubleBladeVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseHacheDouble))
            doubleAxe.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.DoubleAxeProficiency) || target.KnowsFeat((Feat)CustomSkill.ExoticWeaponProficiency))
            doubleAxe.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            doubleAxeVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseMasseDouble))
            direMace.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.DireMaceProficiency) || target.KnowsFeat((Feat)CustomSkill.ExoticWeaponProficiency))
            direMace.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            direMaceVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseFaux))
            scythe.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.ScytheProficiency) || target.KnowsFeat((Feat)CustomSkill.ExoticWeaponProficiency))
            scythe.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            scytheVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseHacheNaine))
            dwarvenAxe.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.DwarvenAxeProficiency) || target.KnowsFeat((Feat)CustomSkill.ExoticWeaponProficiency))
            dwarvenAxe.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            dwarvenAxeVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseKama))
            kama.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.KamaProficiency) || target.KnowsFeat((Feat)CustomSkill.ExoticWeaponProficiency))
            kama.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            kamaVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          if (target.KnowsFeat((Feat)CustomSkill.ExpertiseKukri))
            kukri.SetBindValue(player.oid, nuiToken.Token, "expertise");
          else if (target.KnowsFeat((Feat)CustomSkill.KukriProficiency) || target.KnowsFeat((Feat)CustomSkill.ExoticWeaponProficiency))
            kukri.SetBindValue(player.oid, nuiToken.Token, "proficiency");
          else
            kukriVisibility.SetBindValue(player.oid, nuiToken.Token, false);
        }
      }
    }
  }
}
