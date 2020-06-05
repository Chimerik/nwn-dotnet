using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems.Garden
{
    public partial class Garden : NWPlaceable
    {
        public static Dictionary<string, Garden> Potagers = new Dictionary<string, Garden>();

        public Garden(uint nwobj) : base(nwobj)
        {
            Potagers[nwobj.AsPlaceable().uuid] = this;
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
                var sql = $"SELECT * FROM sql_potager WHERE uuid=@uuid LIMIT 1;";

                using (var connection = MySQL.GetConnection())
                {
                    var potager = connection.QueryFirst<Potager.Models.PotagerSql>(sql, new { uuid = oPotager.uuid });
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
                }
                i += 1;
                oPotager = NWScript.GetObjectByTag("potager", i).AsPlaceable();
            }
        }
   
        public static void UpdateForUUID()
        {
            var oPotager = NWScript.GetObjectByTag("potager").AsPlaceable();
            int i = 0;
            while (oPotager.IsValid)
            {
                if(oPotager.Locals.Int.Get("id") > 0)
                {
                    var sql = $"UPDATE sql_potager SET uuid=@uuid, datePlantage=@datePlantage WHERE id=@id;";           

                    using (var connection = MySQL.GetConnection())
                    {
                        connection.Execute(sql, new { id = oPotager.Locals.Int.Get("id"), uuid = oPotager.uuid, datePlantage = DateTime.Now });
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

        void PlanterFruit(uint oPC, uint oFruit)
        {
            string sPlanteType = oFruit.AsItem().Name;
            var oPotager = (NWPlaceable)oPC.AsPlayer().Locals.Object.Get("DDIAG__SPEAKEE");

            oFruit.AsItem().Destroy();
            oPotager.Name = $"Plant de {sPlanteType} en cours de pousse";
            oPotager.Locals.Int.Set("_PLANTE_STATE", 1);
            oPotager.Appearance = 8791;
            NWScript.DelayCommand(324000.0f, () => PousserPlante(oPotager, sPlanteType));
            oPC.AsPlayer().SendMessage($"Vous venez de mettre en terre un plant de {sPlanteType}. Reste à attendre que ça pousse !");

            var sql = $"REPLACE INTO sql_potager (uuid, type, datePlantage, tag) VALUES (@uuid, @type, @datePlantage, @tag);";

            using (var connection = MySQL.GetConnection())
            {
                connection.Execute(sql, new { uuid = oPotager.uuid, datePlantage = DateTime.Now, type = oFruit.AsItem().Name, tag = oFruit.AsItem().Tag});
            }
        }
    }
}
