using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems
{
    public partial class Garden : NWPlaceable
    {
        public int id { get; set; }
        public DateTime? datePlantage { get; set; }
        public string? type { get; set; }
        public string? tag { get; set; }

        public static Dictionary<int, Garden> Potagers = new Dictionary<int, Garden>();

        public Garden(uint nwobj, DateTime dateStored, string type, string tag) : base(nwobj)
        {
            this.id = nwobj.AsPlaceable().Locals.Int.Get("id");
            this.datePlantage = dateStored;
            this.type = type;
            this.tag = tag;
            Potagers[this.id] = this;
        }

        static Garden()
        {

        }

        public static void Init()
        {
            var oPotager = NWScript.GetObjectByTag("potager").AsPlaceable();
            int i = 0;
            while (oPotager.IsValid)
            {
                var sql = $"SELECT * FROM sql_potager WHERE id=@id LIMIT 1;";

                using (var connection = MySQL.GetConnection())
                {
                    var potager = connection.QueryFirstOrDefault<Potager.Models.PotagerSql>(sql, new { id = oPotager.Locals.Int.Get("id") });
                    if (potager != null)
                    {
                        var sPlanteType = potager.type;
                        var iPlanteState = (DateTime.Now - potager.datePlantage).TotalSeconds;

                        if (iPlanteState > 486000)
                        {
                            oPotager.Name = $"Plant de {sPlanteType} fâné";
                            oPotager.Locals.Int.Set("_PLANTE_STATE", 3);
                            oPotager.Appearance = 571;
                        }
                        else if (iPlanteState > 324000)
                        {
                            oPotager.Name = $"Plant de {sPlanteType} prêt pour la récolte";
                            oPotager.Locals.Int.Set("_PLANTE_STATE", 2);
                            oPotager.Appearance = 4340;
                            NWScript.DelayCommand((float)(486000 - iPlanteState), () => FanerPlante(oPotager, sPlanteType));
                        }
                        else if (iPlanteState > 0)
                        {
                            oPotager.Name = $"Plant de {sPlanteType} en cours de pousse";
                            oPotager.Locals.Int.Set("_PLANTE_STATE", 1);
                            oPotager.Appearance = 8791;
                            NWScript.DelayCommand((float)(324000 - iPlanteState), () => PousserPlante(oPotager, sPlanteType));
                        }

                        Garden myPotager = new Garden(oPotager, potager.datePlantage, potager.type, potager.tag);
                    }
                    else
                    {
                        Garden myPotager = new Garden(oPotager, DateTime.MinValue, "", "");
                    }
                }
                i += 1;
                oPotager = NWScript.GetObjectByTag("potager", i).AsPlaceable();
            }
        }
  
        private static void FanerPlante(NWPlaceable oPotager, string sPlanteType)
        {
            if (oPotager.IsValid)
            {
                oPotager.Name = $"Plant de  {sPlanteType} fâné";
                oPotager.Appearance = 517;
                oPotager.Locals.Int.Set("_PLANTE_STATE", 3);
            }
        }

        private static void PousserPlante(NWPlaceable oPotager, string sPlanteType)
        {
            if (oPotager.IsValid)
            {
                oPotager.Name = "Plant de {sPlanteType} prêt pour la récolte";
                oPotager.Appearance = 4340;
                oPotager.Locals.Int.Set("_PLANTE_STATE", 2);
                NWScript.DelayCommand(162000.0f, () => FanerPlante(oPotager, sPlanteType));
            }
        }

        public void PlanterFruit(string FruitName, string FruitTag)
        {
            this.Name = $"Plant de {FruitName} en cours de pousse";
            this.Locals.Int.Set("_PLANTE_STATE", 1);
            this.Appearance = 8791;
            NWScript.DelayCommand(324000.0f, () => PousserPlante(this, FruitName));

            var sql = $"REPLACE INTO sql_potager (id, type, datePlantage, tag) VALUES (@id, @type, @datePlantage, @tag);";

            using (var connection = MySQL.GetConnection())
            {
                    connection.Execute(sql, new { id = this.id, datePlantage = DateTime.Now, type = FruitName, tag = FruitTag });
            }
        }
    }
}
