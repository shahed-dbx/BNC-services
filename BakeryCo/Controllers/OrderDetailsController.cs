using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;
using BakeryCo.DataModel;
using BakeryCo.Repositary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BakeryCo.Controllers
{
	public class OrderDetailsController : ApiController
	{
		OrderDetails obj = new OrderDetails();

		[HttpGet]
		public HttpResponseMessage GetOrderDetails(int userId, int orderId)
		{
			try
			{

				var response = Request.CreateResponse(
							HttpStatusCode.Created, obj.GetOrderDetails(orderId, userId));//.GetUsrAddressDetails(userid));

				return response;


			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage InsertOrder_New(Newtonsoft.Json.Linq.JObject Json)
		{
			try
			{
				var response = Request.CreateResponse(
							HttpStatusCode.Created, obj.InsertOrderDetails_New(Json));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage InsertOrder(Newtonsoft.Json.Linq.JObject Json)
		{
			try
			{

				var response = Request.CreateResponse(
							HttpStatusCode.Created, obj.InsertOrderDetails(Json));



				return response;


			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage InsertCorporateRequest(Newtonsoft.Json.Linq.JObject Json)
		{
			try
			{

				var response = Request.CreateResponse(
							HttpStatusCode.Created, obj.InsertCorporateRequest(Json));

				return response;

			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}

		[HttpPut]
		public HttpResponseMessage CancelOrder(Newtonsoft.Json.Linq.JObject JSON)
		{
			try
			{

				var response = Request.CreateResponse(
							HttpStatusCode.Created, obj.CancelOrder(JSON));


				return response;

			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}

		}

		[HttpPut]
		public HttpResponseMessage CustomerPnList(Newtonsoft.Json.Linq.JObject JSON)
		{
			try
			{

				var response = Request.CreateResponse(
							HttpStatusCode.Created, obj.CustomerPnList(JSON));


				return response;

			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}

		}

		[HttpPut]
		public HttpResponseMessage UpdatePnlist(Newtonsoft.Json.Linq.JObject JSON)
		{
			try
			{

				var response = Request.CreateResponse(
							HttpStatusCode.Created, obj.UpdatePnlist(JSON));


				return response;

			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}

		}

		[HttpGet]
		public HttpResponseMessage DeleteOrder(int orderId)
		{
			try
			{

				var response = Request.CreateResponse(
							HttpStatusCode.Created, obj.DeleteOrder(orderId));


				return response;

			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}


		[HttpGet]
		public HttpResponseMessage GetFaviorateOrder(int userid)
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.Created, obj.GetFavouriteOrder(userid));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}

		[HttpPut]
		public HttpResponseMessage DeleteFaviorateOrder(int orderId)
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.Created, obj.DeleteFavoriteOrder(orderId));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}

		}

		[HttpPut]
		public HttpResponseMessage InsertFaviorateOrder(int orderId, String FName)
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.Created, obj.InsertFavorieOrder(orderId, FName));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}

		[HttpGet]
		public HttpResponseMessage GetOrderHistory_New(int userId)
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.OK, obj.GetOrderHistory_New(userId));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}

		}

		[HttpGet]
		public HttpResponseMessage GetOrderHistory(int userId)
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.OK, obj.GetOrderHistory(userId));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}

		}

		[HttpGet]
		public HttpResponseMessage TrackOrder_New(int orderId, int userId)
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.OK, obj.TrackOrder_New(orderId, userId));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}

		[HttpGet]
		public HttpResponseMessage TrackOrder(int orderId, int userId)
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.OK, obj.TrackOrder(orderId, userId));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}


		[HttpGet]
		public HttpResponseMessage GetOrderLocation(int orderId)
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.OK, obj.GetOrderLocation(orderId));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage PostInsertRating(JObject userRatings)
		{
			try
			{


				var response = Request.CreateResponse(HttpStatusCode.Created, obj.SaveUsrRating(userRatings));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage OrderRating(JObject OrderRating)
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.Created, obj.OrderRating(OrderRating));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		[HttpGet]
		public HttpResponseMessage OrderDetailsRatings(int uId, int oId)
		{
			try
			{

				return Request.CreateResponse(HttpStatusCode.Created, obj.GetOrderRatingDetails(uId, oId));
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage SaveSpecialImages(JObject Json)
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.Created, obj.SaveSpecialImages(Json));
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

	}
}
