using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeryCo.DataModel;

namespace BakeryCo.Repositary
{
    public class PromotionsRepository
    {


        BakeryCoEntities db = new BakeryCoEntities();



        public JObject GetPromotions(int Userid, string DToken)
        {
            JObject Jobj = new JObject(new JProperty("Failure", "InvalidUser"));
            try
            {
                var rest = db.SP_Get_Promotions(Userid, DToken).ToList();
                var res = JArray.Parse(JsonConvert.SerializeObject(db.SP_Get_Promotions(Userid, DToken)));
                // Jobj 


                // JArray Jobj = JArray.Parse(JsonConvert.SerializeObject(UsrDetails));

                return res.Count > 0 ? Jobj = new JObject(new JProperty("Success", res)) : Jobj;// = new JObject(new JProperty("Failure", "InvalidUser"));
                // return Jobj;
            }
            catch
            {
                return Jobj = new JObject(new JProperty("Failure", "System Exception"));

            }
        }

        //public JObject UpdatePromotions(int userid, int promid, int consumebonus, int promotype)
        //{
        //    JObject Jobj = new JObject(new JProperty("Failure", "InvalidUser"));
        //    try
        //    {

        //        var res = JArray.Parse(JsonConvert.SerializeObject(db.SP_UpdateUserPromoDetails(userid, promid, consumebonus, promotype)));
        //        // Jobj 


        //        // JArray Jobj = JArray.Parse(JsonConvert.SerializeObject(UsrDetails));

        //        return res.Count > 0 ? Jobj = new JObject(new JProperty("Success", res)) : Jobj;// = new JObject(new JProperty("Failure", "InvalidUser"));
        //        // return Jobj;
        //    }
        //    catch
        //    {
        //        return Jobj = new JObject(new JProperty("Failure", "System Exception"));

        //    }
        //}
    }
}
