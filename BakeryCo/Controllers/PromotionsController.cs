using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BakeryCo.Repositary;

namespace BakeryCo.Controllers
{
    public class PromotionsController : ApiController
    {

        PromotionsRepository obj = new PromotionsRepository();
        public HttpResponseMessage GetPromotions(int userId, string DToken)
        {

            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, obj.GetPromotions(userId, DToken));

                return response;

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
