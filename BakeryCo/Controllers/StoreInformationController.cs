using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BakeryCo.Repositary;
using Newtonsoft.Json.Linq;

namespace BakeryCo.Controllers
{
    public class StoreInformationController : ApiController
    {

        StoreInformationRepository SIR = new StoreInformationRepository();
        // GET: api/StoreInformationApi
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/StoreInformationApi/5


        /// <summary>
        /// Returns Store Timings perticular store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
      //  [ActionName("StoreTiming")]
        public HttpResponseMessage GetStoreTimings(int storeId)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, SIR.getStoreTimings(storeId));

                return response;

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }



        /// <summary>
        /// Returns All Store Details Based up on Day
        /// </summary>
        /// <param name="DAY"></param>
        /// <returns></returns>


			//Make sure every new parameter need to be optional ever and forever.......
        public HttpResponseMessage GetStoreDetails(string day, string UserVersion="", string  AppType="")
        {

            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, SIR.getStoresInfo(day, UserVersion, AppType));

                return response;

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }
        //public JObject getCurrentTime()
        //{


        //    JObject jobj = new JObject(new JProperty("DateTime", CgetCurrentTime()));
        //    return jobj;
        //}

    }
}
