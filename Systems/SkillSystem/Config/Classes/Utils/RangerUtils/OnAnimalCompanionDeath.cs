using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnAnimalCompanionDeath(CreatureEvents.OnDeath onDeath)
    {
      if(onDeath.KilledCreature.Master is not null && onDeath.KilledCreature.Master.IsValid)
      {
        onDeath.KilledCreature.Master.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Delete();

        onDeath.KilledCreature.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireBear, 0);
        onDeath.KilledCreature.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireBoar, 0);
        onDeath.KilledCreature.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireDireRaven, 0);
        onDeath.KilledCreature.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpider, 0);
        onDeath.KilledCreature.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireWolf, 0);

        onDeath.KilledCreature.Master.GetObjectVariable<PersistentVariableInt>(CreatureUtils.AnimalCompanionVariable).Value = -1;
      }

      ClearAnimalCompanion(onDeath.KilledCreature);
    }
  }
}
