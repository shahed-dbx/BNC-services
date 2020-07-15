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
    public class CurrentTimeController : ApiController
    {
        [HttpGet]
        public JObject getCurrentTime()
        {

            JObject jobj = new JObject(new JProperty("DateTime", Common.KSA_DateTime().ToString("dd'/'MM'/'yyyy hh:mm tt")));
            return jobj;
        }

    }
}
