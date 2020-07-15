using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeryCo.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Transactions;

namespace BakeryCo.Repositary
{
	public class DriverAppServices
	{
		BakeryCoEntities db = new BakeryCoEntities();
		public JObject UpdateDriverLocation(JObject Json)//(decimal Latitude, decimal Longitude, int driverid)
		{
			JObject jobj = new JObject(new JProperty("Failure", "No Recode Found"));
			try
			{
				IEnumerable<JToken> driverLoc = Json.SelectToken("DriverLoc");
				foreach (JToken dl in driverLoc)
				{

					DriverLocation _dLoc = JsonConvert.DeserializeObject<DriverLocation>(dl.ToString());
					DriverLocation dLocation = db.DriverLocations.Where(dLoc => dLoc.DriverId == _dLoc.DriverId).FirstOrDefault();
					//DriverLocation dLocation = db.DriverLocations.Single(dLoc => dLoc.DriverId == _dLoc.DriverId);
					if (dLocation != null)
					{
						dLocation.Latitude = _dLoc.Latitude;
						dLocation.Longitude = _dLoc.Longitude;
						dLocation.CurrentDT = Common.KSA_DateTime();
						if (_dLoc.IsActive == false) dLocation.IsActive = _dLoc.IsActive;
					}
					else
					{
						DriverLocation newdLocation = new DriverLocation();
						newdLocation.Latitude = _dLoc.Latitude;
						newdLocation.Longitude = _dLoc.Longitude;
						newdLocation.CurrentDT = Common.KSA_DateTime();
						if (_dLoc.IsActive == false) dLocation.IsActive = _dLoc.IsActive;
						newdLocation.DriverId = _dLoc.DriverId;
						db.DriverLocations.Add(newdLocation);
					}
					db.SaveChanges();

					//  db.SP_UpdateDriverLocation(_dLoc.Latitude, _dLoc.Longitude, _dLoc.DriverId);
					jobj = new JObject(new JProperty("Success", "Updated"));
				}

				return jobj;

			}
			catch(Exception Ex)
			{
				return jobj;
			}


		}

		public JObject GetAssignOrders(int DriverId)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{
				var res = JArray.Parse(JsonConvert.SerializeObject(db.SP_Get_AssignOrderDetails(DriverId)));

				return res.Count > 0 ? Jobj = new JObject(new JProperty("Success", res)) : Jobj;
			}
			catch(Exception ex)
			{
				return Jobj;
			}
		}

		public JObject GetDriverOrderHistory(int DriverId)
		{

			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{
				var res = JArray.Parse(JsonConvert.SerializeObject(db.SP_Get_OrderDeliveryHistory(DriverId)));

				return res.Count > 0 ? Jobj = new JObject(new JProperty("Success", res)) : Jobj;
			}
			catch
			{
				return Jobj;
			}
		}

		public JObject UpdateOrderStatus(int DriverId, int OrderId, string OrderStauts, string Comment)
		{
			JObject Jobj = new JObject(new JProperty("Success", OrderStauts));
			try
			{
				string msg = "";
				string DToken = string.Empty;
				int userid = 0;

				using (TransactionScope objTran = new TransactionScope())
				{

					//OrderTracking odrTrack = db.OrderTrackings.Single(OId => OId.OrderId == OrderId);
					OrderInfo oI = db.OrderInfoes.Single(oInfo => oInfo.OrderId == OrderId);
					if (OrderStauts == "OnTheWay")
					{
						if (oI.OrderStatus != "Ready")
						{
							return Jobj = new JObject(new JProperty("Failure", "Order " + oI.InvoiceNo + " Is not Ready"));
						}
					}
					if (OrderStauts != "Accepted")
					{
						oI.OrderStatus = (OrderStauts == "Delivered" ? "Close" : OrderStauts);
						db.SaveChanges();
					}

					OrderTracking odrTrack = new OrderTracking();
					odrTrack.OrderId = OrderId;
					odrTrack.OrderStatus = OrderStauts;
					odrTrack.TrackingTime = Common.KSA_DateTime();//DateTime.Now;/// Convert.ToDateTime(Common.getCurrentTime());
					odrTrack.AcceptedBy = DriverId;
					db.OrderTrackings.Add(odrTrack);
					db.SaveChanges();
					if (OrderStauts == "Accepted")
					{
						OrderAssignDetail OAD = db.OrderAssignDetails.Where(oid => oid.OrderId == OrderId && oid.DriverId == DriverId).OrderByDescending(x => x.OA_Id).FirstOrDefault();
						if(OAD!=null)
						{
							OAD.Comment = Comment;
							OAD.Order_AR = Common.KSA_DateTime();//DateTime.Now;//Convert.ToDateTime(Common.getCurrentTime());
							OAD.IsAccept = true;
						}
						
						db.SaveChanges();

					}

					if (OrderStauts == "OnTheWay")
					{

						DriverLocation dl = db.DriverLocations.Single(Driver => Driver.DriverId == DriverId);
						if(dl!=null)
								dl.IsActive = false;
						db.SaveChanges();


						msg = "Your Order " + oI.InvoiceNo + " is On The Way to " + oI.UserAddressDetail.Address;
						DToken = oI.Device_token;
						userid = Convert.ToInt32(oI.UserId);
					}
					else if (OrderStauts == "Delivered")
					{
						DriverLocation dl = db.DriverLocations.Single(Driver => Driver.DriverId == DriverId);
						if (dl != null)
							dl.IsActive = true;
						db.SaveChanges();
						msg = "Your Order " + oI.InvoiceNo + " Deliverd at " + oI.UserAddressDetail.Address;
						DToken = oI.Device_token;
						userid = Convert.ToInt32(oI.UserId);
					}

					if (OrderStauts == "OnTheWay" || OrderStauts == "Delivered")
					{
						try
						{
							if (OrderStauts == "OnTheWay")
							{
								var res = JArray.Parse(JsonConvert.SerializeObject(db.GenerateNewOrderPN(OrderId, null, 2)));  //2 flag for on the way PN insertions
							}
							else if (OrderStauts == "Delivered")
							{
								var res1 = JArray.Parse(JsonConvert.SerializeObject(db.GenerateNewOrderPN(OrderId, null, 3))); // 3 flag for delivered PN insertion 
							}
						}
						catch (Exception ex)
						{

						}
						//Common.InsertPushMsg(userid, msg, DToken, OrderStauts == "Delivered" ? (int)Common.NotificationType.Delivered : (int)Common.NotificationType.OnTheWay);

					}



					objTran.Complete();

				}
				//if (OrderStauts == "OnTheWay" || OrderStauts == "Delivered")
				//{
				//    // SendPushNotification.SendNotification(msg + oI.UserAddressDetail.Address, oI.Device_token, Convert.ToInt32(oI.UserId));
				//   // SendPushNotification.SendNotification(msg, DToken, userid);
				//    //Common.InsertPushMsg(Convert.ToInt32(oI.UserId), msg + oI.UserAddressDetail.Address, oI.Device_token, oI.OrderStatus == "Close" ? (int)Common.NotificationType.Delivered : (int)Common.NotificationType.OnTheWay);
				//   // Common.InsertPushMsg(userid, msg, DToken, OrderStauts == "Delivered" ? (int)Common.NotificationType.Delivered : (int)Common.NotificationType.OnTheWay);
				//    //Common.InsertPushMsg(userid, msg, DToken, OrderStauts == "Delivered" ? 6 : 5);
				//}

				return Jobj;

			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Failure", "ErrorOccured"));

			}

		}




	}
}
