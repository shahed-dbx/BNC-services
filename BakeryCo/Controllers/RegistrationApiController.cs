using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BakeryCo.Repositary;

namespace BakeryCo.Controllers
{
    public class RegistrationApiController : ApiController
    {
        RegistrationRepository objRegistration = new RegistrationRepository();
       
        [HttpGet]
        public bool ChangePassword(int id, string OldPsw, string NewPsw)
        {
            bool result = false;
            try
            {
                result = objRegistration.ChangePassword(id, OldPsw, NewPsw);

                return result;
            }

            catch (Exception ex)
            {
                return result;
            }

        }

        [HttpGet]
        public HttpResponseMessage SaveUserAdderss(int id, string UsrAddress, string HouseNo, string LandMark, string AddressType, float Latitude, float Longitude, string HouseName, string ContactPerson = "", string ContactNo = "")
        {
            try
            {
                //float lat = float.Parse(Latitude);
                //float log = float.Parse(Longitude);
                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.SaveAddress(id, UsrAddress, HouseNo, LandMark, AddressType, Latitude, Longitude, HouseName, ContactPerson, ContactNo));

                return response;

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }

        [HttpPost]
        public HttpResponseMessage SaveUserAddress(Newtonsoft.Json.Linq.JObject Json)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.SaveUserAddress(Json));



                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Validate username and password SIGN IN
        /// </summary>
        /// <param name="username"></param>
        /// <param name="psw"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Signin(string id, string psw, string dtoken, string lan)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.ValidateUser(id, psw, lan, dtoken));

                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage VerifyMobileNo(string MobileNo, int OTP)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.MobileVerification(MobileNo, OTP));

                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAddressDetails(int userid)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.GetUsrAddressDetails(userid));
                
                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }


        [HttpPost]
        public HttpResponseMessage RegisterUser(Newtonsoft.Json.Linq.JObject Json)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.UserRegistration(Json));



                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPut]
        public HttpResponseMessage UpdateUserAddress(Newtonsoft.Json.Linq.JObject Json)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.UpdateAddressDetails(Json));



                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpPut]
        public HttpResponseMessage UpdateUserProfile(Newtonsoft.Json.Linq.JObject Jobj)
        {
            try
            {
                var response = Request.CreateResponse(HttpStatusCode.Created, objRegistration.UpdateUserProfile(Jobj));
                return response;

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

       [HttpGet]
        public HttpResponseMessage DeleteUserAddress(int addressid)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.DeleteAddressDetails(addressid));

                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

        }

        [HttpGet]

       public HttpResponseMessage ForgetPassword(string Username)
       {
           try
           {

               var response = Request.CreateResponse(
                           HttpStatusCode.Created, objRegistration.ForgetPassword(Username));

               return response;


           }
           catch (Exception ex)
           {
               return Request.CreateResponse(HttpStatusCode.BadRequest);
           }
       }

        [HttpGet]
        public HttpResponseMessage ForgetPassword(string Username, string psw)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.ForgetPassword(Username, psw));

                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }


        [HttpGet]
        public HttpResponseMessage SendOTP(string userid)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.SendOTPManually(userid));



                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public HttpResponseMessage CheckMobileNo(string MobileNo)
        {
            try
            {

                var response = Request.CreateResponse(
                            HttpStatusCode.Created, objRegistration.CheckMobileNo(MobileNo));

                return response;


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
