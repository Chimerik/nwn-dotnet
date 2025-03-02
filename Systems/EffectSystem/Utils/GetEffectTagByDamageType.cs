using Anvil.API;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static int GetImmunityEffectTagByDamageType(DamageType damageType)
    {
      return damageType switch
      {
        DamageType.Acid => (int)CustomEffectIcon.AcidImmunity,
        DamageType.Bludgeoning => (int)CustomEffectIcon.BludgeoningImmunity,
        DamageType.Piercing => (int)CustomEffectIcon.PiercingImmunity,
        DamageType.Slashing => (int)CustomEffectIcon.SlashingImmunity,
        DamageType.Magical => (int)CustomEffectIcon.ForceImmunity,
        DamageType.Cold => (int)CustomEffectIcon.ColdImmunity,
        DamageType.Divine => (int)CustomEffectIcon.RadiantImmunity,
        DamageType.Electrical => (int)CustomEffectIcon.ElectricalImmunity,
        DamageType.Sonic => (int)CustomEffectIcon.TonnerreImmunity,
        CustomDamageType.Necrotic => (int)CustomEffectIcon.NecrotiqueImmunity,
        CustomDamageType.Poison => (int)CustomEffectIcon.PoisonImmunity,
        CustomDamageType.Psychic => (int)CustomEffectIcon.PsychiqueImmunity,
        _ => (int)CustomEffectIcon.FireImmunity,
      };
    }

    public static int GetResistanceEffectTagByDamageType(DamageType damageType)
    {
      return damageType switch
      {
        DamageType.Acid => (int)CustomEffectIcon.AcidResistance,
        DamageType.Bludgeoning => (int)CustomEffectIcon.BludgeoningResistance,
        DamageType.Piercing => (int)CustomEffectIcon.PiercingResistance,
        DamageType.Slashing => (int)CustomEffectIcon.SlashingResistance,
        DamageType.Magical => (int)CustomEffectIcon.ForceResistance,
        DamageType.Cold => (int)CustomEffectIcon.ColdResistance,
        DamageType.Divine => (int)CustomEffectIcon.RadiantResistance,
        DamageType.Electrical => (int)CustomEffectIcon.ElectricalResistance,
        DamageType.Sonic => (int)CustomEffectIcon.TonnerreResistance,
        CustomDamageType.Necrotic => (int)CustomEffectIcon.NecrotiqueResistance,
        CustomDamageType.Poison => (int)CustomEffectIcon.PoisonResistance,
        CustomDamageType.Psychic => (int)CustomEffectIcon.PsychiqueResistance,
        _ => (int)CustomEffectIcon.FireResistance,
      };
    }

    public static int GetVulnerabilityEffectTagByDamageType(DamageType damageType)
    {
      return damageType switch
      {
        DamageType.Acid => (int)CustomEffectIcon.AcidVulnerability,
        DamageType.Bludgeoning => (int)CustomEffectIcon.BludgeoningVulnerability,
        DamageType.Piercing => (int)CustomEffectIcon.PiercingVulnerability,
        DamageType.Slashing => (int)CustomEffectIcon.SlashingVulnerability,
        DamageType.Magical => (int)CustomEffectIcon.ForceVulnerability,
        DamageType.Cold => (int)CustomEffectIcon.ColdVulnerability,
        DamageType.Divine => (int)CustomEffectIcon.RadiantVulnerability,
        DamageType.Electrical => (int)CustomEffectIcon.ElectricalVulnerability,
        DamageType.Sonic => (int)CustomEffectIcon.TonnerreVulnerability,
        CustomDamageType.Necrotic => (int)CustomEffectIcon.NecrotiqueVulnerability,
        CustomDamageType.Poison => (int)CustomEffectIcon.PoisonVulnerability,
        CustomDamageType.Psychic => (int)CustomEffectIcon.PsychiqueVulnerability,
        _ => (int)CustomEffectIcon.FireVulnerability,
      };
    }
  }
}
