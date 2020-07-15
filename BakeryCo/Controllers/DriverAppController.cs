using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BakeryCo.Repositary;

namespace BakeryCo.Controllers
{
    public class DriverAppController : ApiController
    {
        DriverAppServices obj = new DriverAppServices();
        public HttpResponseMessage GetUpdateOrderStatus(int DriverId, int OrderId, string OrderStatus,string Comment)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, obj.UpdateOrderStatus(DriverId, OrderId, OrderStatus, Comment));

                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }


        public HttpResponseMessage GetAssignOrders(int DriverId)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, obj.GetAssignOrders(DriverId));

                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

       public HttpResponseMessage GetDriverOrderHistory(int DriverIdForHistory)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, obj.GetDriverOrderHistory(DriverIdForHistory));

                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
       public HttpResponseMessage UpdateDriverLocation(Newtonsoft.Json.Linq.JObject Json)//(decimal Latitude, decimal Longitude, int driverid)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, obj.UpdateDriverLocation(Json));//(Latitude, Longitude, driverid));

                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

    }
}
