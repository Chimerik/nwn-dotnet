namespace NWN.Systems
{
  public class CreatureStats
  {
    public int HP { get; set; }
    public int AC { get; set; }
    public int minDamage { get; set; }
    public int maxDamage { get; set; }
    public int critChance { get; set; }
    public int nbAttacks { get; set; }

    public CreatureStats(int HP, int AC, int minDamage, int maxDamage, int critChance, int nbAttacks)
    {
      this.HP = HP;
      this.AC = AC;
      this.minDamage = minDamage;
      this.maxDamage = maxDamage;
      this.critChance = critChance;
      this.nbAttacks = nbAttacks;
    }
    public CreatureStats()
    {

    }
  }
}
