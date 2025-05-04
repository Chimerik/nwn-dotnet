using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private void ConditionsBindings()
        {
          resistanceRow.Children.Clear();
          resistanceRow.Children.Add(new NuiSpacer());

          foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
          {
            if (target.ActiveEffects.Any(e => e.EffectType == EffectType.Icon && e.IntParams[0] == EffectUtils.GetImmunityEffectTagByDamageType(damageType)))
              resistanceRow.Children.Add(new NuiButtonImage(NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageImmunityEffectIcon[damageType]).Icon) { Width = windowWidth / 12, Height = windowWidth / 12, Tooltip = NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageImmunityEffectIcon[damageType]).StrRef.ToString() });

            if (target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetResistanceEffectTagByDamageType(damageType)))
              resistanceRow.Children.Add(new NuiButtonImage(NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageResistanceEffectIcon[damageType]).Icon) { Width = windowWidth / 12, Height = windowWidth / 12, Tooltip = NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageResistanceEffectIcon[damageType]).StrRef.ToString() });

            if (target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetVulnerabilityEffectTagByDamageType(damageType)))
              resistanceRow.Children.Add(new NuiButtonImage(NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageVulnerabilityEffectIcon[damageType]).Icon) { Width = windowWidth / 12, Height = windowWidth / 12, Tooltip = NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageVulnerabilityEffectIcon[damageType]).StrRef.ToString() });
          }

          resistanceRow.Children.Add(new NuiSpacer());
          resistanceGroup.SetLayout(player.oid, nuiToken.Token, resistanceRow);

          List<string> iconList = new();
          List<string> nameList = new();

          foreach (var eff in target.ActiveEffects)
          {
            if(eff.EffectType == EffectType.Icon 
              && !DamageType2da.damageResistanceEffectIcon.ContainsValue((EffectIcon)eff.IntParams[0])
              && !DamageType2da.damageVulnerabilityEffectIcon.ContainsValue((EffectIcon)eff.IntParams[0])
              && !DamageType2da.damageImmunityEffectIcon.ContainsValue((EffectIcon)eff.IntParams[0]))
            {
              iconList.Add(NwGameTables.EffectIconTable.GetRow(eff.IntParams[0]).Icon);
              nameList.Add(NwGameTables.EffectIconTable.GetRow(eff.IntParams[0]).StrRef.ToString());
            }
          }

          conditionIcon.SetBindValues(player.oid, nuiToken.Token, iconList);
          conditionName.SetBindValues(player.oid, nuiToken.Token, nameList);
          conditionsListCount.SetBindValue(player.oid, nuiToken.Token, iconList.Count);
        }
      }
    }
  }
}
