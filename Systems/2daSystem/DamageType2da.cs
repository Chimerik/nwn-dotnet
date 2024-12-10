using System.Collections.Generic;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public sealed class DamageTypeEntry : ITwoDimArrayEntry
  {
    public StrRef nameRef { get; private set; }
    public string immunityIcon { get; private set; }
    public string resistanceIcon { get; private set; }
    public string vulnerabilityIcon { get; private set; }
    public DamageType damageType { get; private set; }
    public int RowIndex { get; init; }

    public void InterpretEntry(TwoDimArrayEntry entry)
    {
      nameRef = entry.GetStrRef("ResistanceStrRef").GetValueOrDefault();
      resistanceIcon = entry.GetString("ResistanceIcon");
      vulnerabilityIcon = entry.GetString("VulnerabilityIcon");
      immunityIcon = entry.GetString("ImmunityIcon");
      damageType = (DamageType)entry.GetInt("Id").Value;
    }
  }

  [ServiceBinding(typeof(DamageType2da))]
  public class DamageType2da
  {
    public static readonly TwoDimArray<DamageTypeEntry> damageTypeTable = NwGameTables.GetTable<DamageTypeEntry>("damagetypes.2da");
    public static readonly Dictionary<DamageType, EffectIcon> damageImmunityEffectIcon = new();
    public static readonly Dictionary<DamageType, EffectIcon> damageResistanceEffectIcon = new();
    public static readonly Dictionary<DamageType, EffectIcon> damageVulnerabilityEffectIcon = new();
    public DamageType2da()
    {
      foreach (var entry in damageTypeTable) ;

      damageImmunityEffectIcon.Add(DamageType.Bludgeoning, (EffectIcon)196);
      damageResistanceEffectIcon.Add(DamageType.Bludgeoning, (EffectIcon)212);
      damageVulnerabilityEffectIcon.Add(DamageType.Bludgeoning, (EffectIcon)199);

      damageImmunityEffectIcon.Add(DamageType.Piercing, (EffectIcon)197);
      damageResistanceEffectIcon.Add(DamageType.Piercing, (EffectIcon)213);
      damageVulnerabilityEffectIcon.Add(DamageType.Piercing, (EffectIcon)200);

      damageImmunityEffectIcon.Add(DamageType.Slashing, (EffectIcon)195);
      damageResistanceEffectIcon.Add(DamageType.Slashing, (EffectIcon)211);
      damageVulnerabilityEffectIcon.Add(DamageType.Slashing, (EffectIcon)198);

      damageImmunityEffectIcon.Add(DamageType.Magical, (EffectIcon)111);
      damageResistanceEffectIcon.Add(DamageType.Magical, (EffectIcon)204);
      damageVulnerabilityEffectIcon.Add(DamageType.Magical, (EffectIcon)120);

      damageImmunityEffectIcon.Add(DamageType.Acid, (EffectIcon)112);
      damageResistanceEffectIcon.Add(DamageType.Acid, (EffectIcon)205);
      damageVulnerabilityEffectIcon.Add(DamageType.Acid, (EffectIcon)121);

      damageImmunityEffectIcon.Add(DamageType.Cold, (EffectIcon)113);
      damageResistanceEffectIcon.Add(DamageType.Cold, (EffectIcon)206);
      damageVulnerabilityEffectIcon.Add(DamageType.Cold, (EffectIcon)122);

      damageImmunityEffectIcon.Add(DamageType.Divine, (EffectIcon)114);
      damageResistanceEffectIcon.Add(DamageType.Divine, (EffectIcon)207);
      damageVulnerabilityEffectIcon.Add(DamageType.Divine, (EffectIcon)123);

      damageImmunityEffectIcon.Add(DamageType.Electrical, (EffectIcon)115);
      damageResistanceEffectIcon.Add(DamageType.Electrical, (EffectIcon)208);
      damageVulnerabilityEffectIcon.Add(DamageType.Electrical, (EffectIcon)124);

      damageImmunityEffectIcon.Add(DamageType.Fire, (EffectIcon)116);
      damageResistanceEffectIcon.Add(DamageType.Fire, (EffectIcon)209);
      damageVulnerabilityEffectIcon.Add(DamageType.Fire, (EffectIcon)125);

      damageImmunityEffectIcon.Add(DamageType.Sonic, (EffectIcon)119);
      damageResistanceEffectIcon.Add(DamageType.Sonic, (EffectIcon)210);
      damageVulnerabilityEffectIcon.Add(DamageType.Sonic, (EffectIcon)128);

      damageImmunityEffectIcon.Add(DamageType.Custom1, (EffectIcon)174);
      damageResistanceEffectIcon.Add(DamageType.Custom1, (EffectIcon)214);
      damageVulnerabilityEffectIcon.Add(DamageType.Custom1, (EffectIcon)201);

      damageImmunityEffectIcon.Add(DamageType.Custom2, (EffectIcon)175);
      damageResistanceEffectIcon.Add(DamageType.Custom2, (EffectIcon)215);
      damageVulnerabilityEffectIcon.Add(DamageType.Custom2, (EffectIcon)202);

      damageImmunityEffectIcon.Add(DamageType.Custom3, (EffectIcon)176);
      damageResistanceEffectIcon.Add(DamageType.Custom3, (EffectIcon)216);
      damageVulnerabilityEffectIcon.Add(DamageType.Custom3, (EffectIcon)203);
    }
  }
}
