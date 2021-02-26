using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BakeryCo.Repositary;

namespace BakeryCo.Controllers
{
    public class ItemDetailsController : ApiController
    {
		 
        ItemRepositary obj = new ItemRepositary();

        [HttpGet]
        public HttpResponseMessage GetSubCategoryItemsDetails(int mainCatId)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, obj.GetSubCategoryDetails(mainCatId));

                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetSubCategoryItemsDetails()
        {
            try
            {

				//var response = Request.CreateResponse(HttpStatusCode.Created, obj.GetSubCategoryItemsDetails());
				var response = Request.CreateResponse(HttpStatusCode.Created, obj.GetSubCategoryItemsDetailsNew());
				// HttpStatusCode.Created, obj.GetSubCategoryDetails());

				return response;

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

		[HttpGet]
		public HttpResponseMessage GetSubCategoryItemsDetailswithBanners()
		{
			try
			{

				//var response = Request.CreateResponse(HttpStatusCode.Created, obj.GetSubCategoryItemsDetails());
				var response = Request.CreateResponse(HttpStatusCode.Created, obj.GetSubCategoryItemsDetailsWithBanners());
				// HttpStatusCode.Created, obj.GetSubCategoryDetails());

				return response;

			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}

		//Shahed created service for promotion list in website.

		[HttpGet]
		public HttpResponseMessage GetPromotionsList()
		{
			try
			{
				var response = Request.CreateResponse(HttpStatusCode.Created, obj.GetPromotionsList());
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
		}
	}
}
