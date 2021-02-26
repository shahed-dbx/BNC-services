using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeryCo.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Transactions;
using System.Web;
using System.IO;
using System.Configuration;

namespace BakeryCo.Repositary
{
	public class OrderDetails
	{

		//BakeryCoEntities
		BakeryCo.DataModel.BakeryCoEntities db = new BakeryCo.DataModel.BakeryCoEntities();
		public JObject InsertOrderDetails_New(JObject Json)
		{
			// Json = JsonConvert.SerializeObject(Json);
			string addlcomt = "";
			string AppVersion = "";
			string MerchantTransactionId = "";
			JObject Jobj = new JObject(new JProperty("Failure", "Error Occured"));
			try
			{

				using (TransactionScope objTran = new TransactionScope())
				{

					JObject jobj = Json;// JObject.Parse(Json);

					IEnumerable<JToken> MainItem = jobj.SelectToken("MainItem");

					IEnumerable<JToken> SubItem = jobj.SelectToken("SubItem");

					IEnumerable<JToken> Promotion = jobj.SelectToken("Promotion");

					//OrderAdditional OrAdditonals = new OrderAdditional();
					int _orderId = 0;
					int _itemId = 0;
					int? _userid = 0;
					string InvoiceNo = string.Empty;
					int _OrderItemCakeId = 0;

					foreach (JToken mItem in MainItem)
					{
						OrderInfo ord = JsonConvert.DeserializeObject<OrderInfo>(mItem.ToString());

						// If additionalcomment  there then readi it else keep it empty.
						try
						{
							addlcomt = mItem.SelectToken("additionalcomment").ToString();
						}
						catch (Exception ex)
						{
							addlcomt = "";
						}

						// If AppVersion there then readi it else keep it empty.
						try
						{
							AppVersion = mItem.SelectToken("Comments").ToString();
						}
						catch (Exception ex)
						{
							AppVersion = "";
						}
						// If merchant id there then readi it else keep it empty.
						try
						{
							MerchantTransactionId = mItem.SelectToken("merchantTransactionId").ToString();
						}
						catch (Exception ex)
						{
							MerchantTransactionId = "";
						}

						InvoiceNo = getInvoiceNo(ord.StoreId.ToString());
						ord.Comments = addlcomt;
						ord.AppVersion = AppVersion;
						ord.merchantTransactionId = MerchantTransactionId;
						ord.InvoiceNo = InvoiceNo;
						ord.status = true;
						ord.OrderDate = Common.KSA_DateTime();//DateTime.Now;//Convert.ToDateTime(Common.getCurrentTime());

						#region VatImplementation
						/*
						if (ord.PaymentMode == 2)
						{
							ord.SubTotal = ord.SubTotal == null ? ord.Total_Price : ord.SubTotal;
							ord.VatPercentage = (from vat in db.VATDetails where vat.IsActive == true select new { vat.ID, vat.VatPercentage }).OrderByDescending(x => x.ID).FirstOrDefault().VatPercentage.ToString();
							if (ord.SubTotal == null)
							{
								string vatC = (Convert.ToDouble(ord.SubTotal) * (Convert.ToDouble(ord.VatPercentage)) / 100).ToString();
								var totalCost = Convert.ToDouble(String.Format("{0:0.00}", vatC));
								ord.VatCharges = totalCost.ToString();
							}
							//ord.VatCharges = (Convert.ToDouble(ord.SubTotal) * (Convert.ToDouble(ord.VatPercentage)) / 100).ToString();

							ord.Total_Price = ord.SubTotal == null ? (Convert.ToDouble(ord.SubTotal) + Convert.ToDouble(ord.VatCharges)).ToString() : ord.Total_Price;
						}
						if (ord.PaymentMode == 3)
						{
							ord.VatCharges = ord.SubTotal == null ? "0" : ord.VatCharges;
							ord.SubTotal = ord.SubTotal == null ? ord.Total_Price : ord.SubTotal;

							ord.VatPercentage = (from vat in db.VATDetails where vat.IsActive == true select new { vat.ID, vat.VatPercentage }).OrderByDescending(x => x.ID).FirstOrDefault().VatPercentage.ToString();
							ord.Total_Price = (Convert.ToDouble(ord.SubTotal) + Convert.ToDouble(ord.VatCharges)).ToString();
						}
						if (ord.PaymentMode == 4)
						{
							ord.SubTotal = ord.SubTotal == null ? ord.Total_Price : ord.SubTotal;
							ord.VatCharges = "0";
							ord.VatPercentage = (from vat in db.VATDetails where vat.IsActive == true select new { vat.ID, vat.VatPercentage }).OrderByDescending(x => x.ID).FirstOrDefault().VatPercentage.ToString();
							ord.Total_Price = (Convert.ToDouble(ord.SubTotal) + Convert.ToDouble(ord.VatCharges)).ToString();
						}

						*/
						#endregion

						db.OrderInfoes.Add(ord);
						db.SaveChanges();
						_orderId = ord.OrderId;
						_userid = ord.UserId;

						//insert into TrackOrder table

						OrderTracking oTrack = new OrderTracking();
						oTrack.OrderId = ord.OrderId;
						oTrack.OrderStatus = "Received";
						oTrack.TrackingTime = ord.OrderDate;
						db.OrderTrackings.Add(oTrack);
						db.SaveChanges();

						SaveUserDevicetoken(ord.Device_token, "Ordr", ord.OrderId);

						Registration reg = db.Registrations.Single(usr => usr.UserId == ord.UserId);
						reg.DeviceToken = ord.Device_token;
						db.SaveChanges();

						// Execute GenerateNewOrderPN  this procedure using entity
						// TODO: db.SP_CancelOrder(ord.OrderId, ord.UserId)
						//db.GenerateNewOrderPN(ord.OrderId);

						//  Inserting PNs to all vendors .
						try
						{
							var res = JArray.Parse(JsonConvert.SerializeObject(db.GenerateNewOrderPN(ord.OrderId, ord.UserId, 1)));
						}
						catch (Exception ex)
						{

						}
					}

					if (null != Promotion)
					{
						foreach (JToken pDetails in Promotion)
						{
							userPromotion obj = JsonConvert.DeserializeObject<userPromotion>(pDetails.ToString());
							obj.userId = _userid;
							obj.orderId = _orderId;
							obj.usedDate = Common.KSA_DateTime();//DateTime.Now;
							db.userPromotions.Add(obj);
							db.SaveChanges();

						}
					}

					foreach (JToken sItem in SubItem)
					{
						JToken Jitems = null;
						Jitems = sItem;

						if (null != Jitems)
						{
							//~~~~~~~~~~~Sub Items Reading.~~~~~~~~~~~~~~~~~~~
							IEnumerable<JToken> SubsubItem = Jitems.SelectToken("SubSubItems");
							OrderItem OrItem = JsonConvert.DeserializeObject<OrderItem>(Jitems.ToString());
							OrItem.OrderId = _orderId;
							db.Entry(OrItem).State = System.Data.Entity.EntityState.Added;
							db.SaveChanges();
							_itemId = OrItem.Id;

							if (SubsubItem != null)
							{
								foreach (JToken ssitems in SubsubItem)
								{
									OrderSubItem OrsubItem = JsonConvert.DeserializeObject<OrderSubItem>(ssitems.ToString());
									OrsubItem.OrderItemId = _itemId;
									db.Entry(OrsubItem).State = System.Data.Entity.EntityState.Added;
									db.SaveChanges();
								}
							}
							//End Sub Item Reading.....
							//~~~~~Special cake Data Reading.~~~~~~~~~~~~~~~~~~~~~~
							IEnumerable<JToken> SpclCake = Jitems.SelectToken("SpecialCakeObj");
							if (SpclCake != null)
							{
								foreach (JToken SpCake in SpclCake)
								{
									JToken SpJTokens = null;
									SpJTokens = SpCake;

									IEnumerable<JToken> Flavors = SpJTokens.SelectToken("flavorDetails");

									OrderItemCake OrdItmCake = JsonConvert.DeserializeObject<OrderItemCake>(SpJTokens.ToString());
									OrdItmCake.OrderItemId = _itemId;
									OrdItmCake.OrderId = _orderId;
									OrdItmCake.Status = true;
									OrdItmCake.CreatedOn = Common.KSA_DateTime();
									db.Entry(OrdItmCake).State = System.Data.Entity.EntityState.Added;
									db.SaveChanges();
									_OrderItemCakeId = OrdItmCake.Id;

									// Special cake Layers data reading..
									if (Flavors != null)
									{
										foreach (JToken Flvr in Flavors)
										{
											OrderItemCakeLayer OrdItmCakeLyr = JsonConvert.DeserializeObject<OrderItemCakeLayer>(Flvr.ToString());
											OrdItmCakeLyr.OrderItemCakeId = _OrderItemCakeId;
											OrdItmCakeLyr.OrderItemId = _itemId;
											OrdItmCakeLyr.OrderId = _orderId;
											OrdItmCakeLyr.Status = true;
											OrdItmCakeLyr.CreatedOn = Common.KSA_DateTime();
											db.Entry(OrdItmCakeLyr).State = System.Data.Entity.EntityState.Added;
											db.SaveChanges();
										}
									}
								}
							}
						}
					}
					// db.SaveChanges();
					objTran.Complete();
					Jobj = new JObject(new JProperty("Success", _orderId + "," + InvoiceNo));
					return Jobj;
				}
			}
			catch (Exception ex)
			{

				Jobj = new JObject(new JProperty("Failure", ex.Message));
				return Jobj;
			}
		}

		public JObject InsertOrderDetails(JObject Json)
		{
			// Json = JsonConvert.SerializeObject(Json);
			string addlcomt = "";
			string AppVersion = "";
			string MerchantTransactionId = "";
			JObject Jobj = new JObject(new JProperty("Failure", "Error Occured"));
			try
			{

				using (TransactionScope objTran = new TransactionScope())
				{

					JObject jobj = Json;// JObject.Parse(Json);

					IEnumerable<JToken> MainItem = jobj.SelectToken("MainItem");

					IEnumerable<JToken> SubItem = jobj.SelectToken("SubItem");

					IEnumerable<JToken> Promotion = jobj.SelectToken("Promotion");

					//OrderAdditional OrAdditonals = new OrderAdditional();
					int _orderId = 0;
					int _itemId = 0;
					int? _userid = 0;
					string InvoiceNo = string.Empty;
					// 
					foreach (JToken mItem in MainItem)
					{
						OrderInfo ord = JsonConvert.DeserializeObject<OrderInfo>(mItem.ToString());

						// If additionalcomment  there then readi it else keep it empty.
						try
						{
							addlcomt = mItem.SelectToken("additionalcomment").ToString();
						}
						catch (Exception ex)
						{
							addlcomt = "";
						}

						// If AppVersion there then readi it else keep it empty.
						try
						{
							AppVersion = mItem.SelectToken("Comments").ToString();
						}
						catch (Exception ex)
						{
							AppVersion = "";
						}
						// If merchant id there then readi it else keep it empty.
						try
						{
							MerchantTransactionId = mItem.SelectToken("merchantTransactionId").ToString();
						}
						catch (Exception ex)
						{
							MerchantTransactionId = "";
						}

						InvoiceNo = getInvoiceNo(ord.StoreId.ToString());
						ord.Comments = addlcomt;
						ord.AppVersion = AppVersion;
						ord.merchantTransactionId = MerchantTransactionId;
						ord.InvoiceNo = InvoiceNo;
						ord.status = true;
						ord.OrderDate = Common.KSA_DateTime();//DateTime.Now;//Convert.ToDateTime(Common.getCurrentTime());

						#region VatImplementation
						/*
						if (ord.PaymentMode == 2)
						{
							ord.SubTotal = ord.SubTotal == null ? ord.Total_Price : ord.SubTotal;
							ord.VatPercentage = (from vat in db.VATDetails where vat.IsActive == true select new { vat.ID, vat.VatPercentage }).OrderByDescending(x => x.ID).FirstOrDefault().VatPercentage.ToString();
							if (ord.SubTotal == null)
							{
								string vatC = (Convert.ToDouble(ord.SubTotal) * (Convert.ToDouble(ord.VatPercentage)) / 100).ToString();
								var totalCost = Convert.ToDouble(String.Format("{0:0.00}", vatC));
								ord.VatCharges = totalCost.ToString();
							}
							//ord.VatCharges = (Convert.ToDouble(ord.SubTotal) * (Convert.ToDouble(ord.VatPercentage)) / 100).ToString();

							ord.Total_Price = ord.SubTotal == null ? (Convert.ToDouble(ord.SubTotal) + Convert.ToDouble(ord.VatCharges)).ToString() : ord.Total_Price;
						}
						if (ord.PaymentMode == 3)
						{
							ord.VatCharges = ord.SubTotal == null ? "0" : ord.VatCharges;
							ord.SubTotal = ord.SubTotal == null ? ord.Total_Price : ord.SubTotal;

							ord.VatPercentage = (from vat in db.VATDetails where vat.IsActive == true select new { vat.ID, vat.VatPercentage }).OrderByDescending(x => x.ID).FirstOrDefault().VatPercentage.ToString();
							ord.Total_Price = (Convert.ToDouble(ord.SubTotal) + Convert.ToDouble(ord.VatCharges)).ToString();
						}
						if (ord.PaymentMode == 4)
						{
							ord.SubTotal = ord.SubTotal == null ? ord.Total_Price : ord.SubTotal;
							ord.VatCharges = "0";
							ord.VatPercentage = (from vat in db.VATDetails where vat.IsActive == true select new { vat.ID, vat.VatPercentage }).OrderByDescending(x => x.ID).FirstOrDefault().VatPercentage.ToString();
							ord.Total_Price = (Convert.ToDouble(ord.SubTotal) + Convert.ToDouble(ord.VatCharges)).ToString();
						}

						*/
						#endregion

						db.OrderInfoes.Add(ord);
						db.SaveChanges();
						_orderId = ord.OrderId;
						_userid = ord.UserId;

						SaveUserDevicetoken(ord.Device_token, "Ordr", ord.OrderId);
						//insert into TrackOrder table

						OrderTracking oTrack = new OrderTracking();
						oTrack.OrderId = ord.OrderId;
						oTrack.OrderStatus = "Received";
						oTrack.TrackingTime = ord.OrderDate;
						db.OrderTrackings.Add(oTrack);
						db.SaveChanges();

						Registration reg = db.Registrations.Single(usr => usr.UserId == ord.UserId);
						reg.DeviceToken = ord.Device_token;
						db.SaveChanges();

						// Execute GenerateNewOrderPN  this procedure using entity
						// TODO: db.SP_CancelOrder(ord.OrderId, ord.UserId)
						//db.GenerateNewOrderPN(ord.OrderId);

						//  Inserting PNs to all vendors .
						try
						{
							var res = JArray.Parse(JsonConvert.SerializeObject(db.GenerateNewOrderPN(ord.OrderId, ord.UserId, 1)));
						}
						catch (Exception ex)
						{

						}
					}

					if (null != Promotion)
					{
						foreach (JToken pDetails in Promotion)
						{
							userPromotion obj = JsonConvert.DeserializeObject<userPromotion>(pDetails.ToString());
							obj.userId = _userid;
							obj.orderId = _orderId;
							obj.usedDate = Common.KSA_DateTime();//DateTime.Now;
							db.userPromotions.Add(obj);
							db.SaveChanges();

						}
					}

					foreach (JToken sItem in SubItem)
					{
						// bool additional = false;
						JToken Jitems = null;
						// JToken Jadditionals = null;
						//foreach (JToken subItems in sItem)
						//{
						//    if (!additional)
						//    {
						//Jitems = subItems;
						Jitems = sItem;
						//additional = true;
						//}
						//else
						//{
						//    Jadditionals = subItems;
						//}
						//}
						if (null != Jitems)
						{
							//var itemProperties = Jitems.Children<JProperty>().ToList();
							IEnumerable<JToken> SubsubItem = Jitems.SelectToken("SubSubItems");
							OrderItem OrItem = JsonConvert.DeserializeObject<OrderItem>(Jitems.ToString());
							OrItem.OrderId = _orderId;
							db.Entry(OrItem).State = System.Data.Entity.EntityState.Added;
							db.SaveChanges();
							_itemId = OrItem.Id;

							if (SubsubItem != null)
							{
								foreach (JToken ssitems in SubsubItem)
								{
									OrderSubItem OrsubItem = JsonConvert.DeserializeObject<OrderSubItem>(ssitems.ToString());
									OrsubItem.OrderItemId = _itemId;
									db.Entry(OrsubItem).State = System.Data.Entity.EntityState.Added;
									db.SaveChanges();
								}
							}

						}

					}
					// db.SaveChanges();
					objTran.Complete();
					Jobj = new JObject(new JProperty("Success", _orderId + "," + InvoiceNo));
					return Jobj;
				}
			}
			catch (Exception ex)
			{

				Jobj = new JObject(new JProperty("Failure", ex.Message));
				return Jobj;
			}

		}

		public JObject TrackOrder_New(int orderid, int userid)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Order Found"));
			try
			{
				List<JProperty> lstProp = new List<JProperty>();
				List<string> oStatus = new List<string>();
				oStatus.Add("Accepted");
				oStatus.Add("DriverCancel");

				// Shahed created list for if orderstatus ='Rejected' then it shows cancels.

				List<string> OrStatusForRej = new List<string>();
				OrStatusForRej.Add("Rejected");

				//End Shahed updated

				if (orderid == -1)
				{
					var or = ((from x in db.OrderInfoes where x.UserId == userid && x.status == true select new { x.OrderId }).OrderByDescending(odate => odate.OrderId).Take(1)).ToList();

					foreach (var odrid in or)
					{
						orderid = Convert.ToInt32(odrid.OrderId);
					}
				}

				var OdrInfo = db.SP_Get_OrderTracking(userid, orderid).OrderBy(odate => odate.Id).Distinct().ToList();

				#region oldcode

				//var OdrInfo = (from oInfo in db.OrderInfoes
				//               join oTrack in db.OrderTrackings on oInfo.OrderId equals oTrack.OrderId into OrderT

				//               join oStores in db.StoreInformations on oInfo.StoreId equals oStores.StoreId
				//               join oStrWork in db.StoreWorkDatas on oInfo.StoreId equals oStrWork.StoreId
				//               join oItems in db.OrderItems on oInfo.OrderId equals oItems.OrderId
				//               join ItSize in db.ItemSizes on oItems.Size equals ItSize.TypeId

				//              // from oSubItm in db.OrderSubItems.Where(ra => ra.OrderItemId == oItems.Id).DefaultIfEmpty()
				//               //join oSubItm in db.OrderSubItems on oItems.Id equals oSubItm.OrderItemId  //sub Items in Items

				//               //join subCat in db.SubCategories on oItems.ItemId equals subCat.SubCategoryId
				//               join Itm in db.ItemsDetails on oItems.ItemId equals Itm.ItemId

				//              // from ItmS in db.ItemsDetails.Where(ra => ra.ItemId == oSubItm.OrderSubItemId).DefaultIfEmpty()
				//             //join ItmS in db.ItemsDetails on oSubItm.OrderSubItemId equals ItmS.ItemId

				//               // join oAdd in db.OrderAdditionals on oItems.Id equals oAdd.ItemId
				//               //join addDet in db.AdditionalInfoLists on oAdd.AdditionalID equals addDet.AdditionalId

				//               join oPromo in db.userPromotions on oInfo.OrderId equals oPromo.orderId into odrPromo
				//               join oAddress in db.UserAddressDetails on oInfo.AddressID equals oAddress.Id into UserAddress
				//               from usrAddress in UserAddress.DefaultIfEmpty()
				//               from ordTrack in OrderT.DefaultIfEmpty()
				//               from Promo in odrPromo.DefaultIfEmpty()
				//                   //join driver in db.Registrations on  ordTrack.AcceptedBy equals driver.UserId into dDetails
				//                   //from drvDetails in dDetails.DefaultIfEmpty()
				//               from rat in db.UserRatings.Where(ra => ra.UserId == userid && ra.OrderId == orderid).DefaultIfEmpty()
				//               where oInfo.OrderId == orderid && oInfo.status == true
				//               group new { ordTrack, oInfo, Promo, usrAddress, oStrWork, oStores, oItems, Itm }//, oAdd, addDet }
				//      by new
				//      {
				//          oInfo.UserId,
				//          oInfo.OrderId,
				//          oInfo.OrderDate,
				//          oInfo.OrderType,
				//          oInfo.ExpectedTime,
				//          VatPercentage = (oInfo.VatPercentage == null ? "0" : oInfo.VatPercentage),
				//          VatCharges = (oInfo.VatCharges == null ? "0" : oInfo.VatCharges),
				//          SubTotal = (oInfo.SubTotal == null ? oInfo.Total_Price : oInfo.SubTotal),
				//          oInfo.Total_Price,
				//          oInfo.Comments,
				//         //subCat.ItemName,
				//         Itm.ItemId,
				//          Itm.ItemName,
				//          Itm.ItemName_Ar,


				//          SubItemId = 1, //oItems.Id,
				//       SubItem = "",//ItmS.ItemName,
				//       SubItemAr= "",// ItmS.ItemName_Ar,
				//      subItemQty =  "", // oSubItm.Qty,
				//      subItemPrice = "",// oSubItm.ItemPrice,
				//         //ItemId = subCat.SubCategoryId,
				//         //subCat.ItemName_Ar,
				//         oItems.ItemPrice,
				//          oItems.Qty,
				//         ItemType = "",// oItems.ItemType==1 ? "Whole":"Slices",
				//          oItems.Size,

				//          SizeName = ItSize.TypeName,
				//          orderitem_orderid = oItems.OrderId,
				//          oItemId = oItems.Id,
				//          oInfo.InvoiceNo,
				//          Address = (oInfo.AddressID == null ? string.Empty : usrAddress.Address),
				//          oStores.StoreAddress,
				//          oStores.StoreAddress_ar,
				//          oStores.StoreName,
				//          oStores.StoreName_ar,
				//          oStores.Latitude,
				//          oStores.Longitude,
				//          ordTrack.TrackingTime,
				//          ordTrack.OrderStatus,
				//          ordTrack.Id,
				//          oStrWork.phone,
				//          ordTrack.AcceptedBy,
				//          PromoAmt = (Promo.BonusAmt == null ? 0 : Promo.BonusAmt),

				//          Ratings = rat.Ratings == null ? 0 : rat.Ratings
				//         //  oAdd.AdditionalPrice,
				//         //oAdd.AdditionalID,
				//         //addDet.AdditionalName,
				//         //addDet.AdditionalName_Ar

				//     } into odrItems
				//               select new
				//               {
				//                   odrItems.Key.UserId,
				//                   odrItems.Key.OrderId,
				//                   odrItems.Key.OrderDate,
				//                   odrItems.Key.OrderType,
				//                   odrItems.Key.ExpectedTime,
				//                   odrItems.Key.VatCharges,
				//                   odrItems.Key.SubTotal,
				//                   odrItems.Key.VatPercentage,
				//                   odrItems.Key.Total_Price,
				//                   odrItems.Key.orderitem_orderid,
				//                   odrItems.Key.oItemId,

				//                   odrItems.Key.SubItemId,
				//                   odrItems.Key.SubItem,
				//                   odrItems.Key.SubItemAr,
				//                   odrItems.Key.subItemQty,
				//                   odrItems.Key.subItemPrice,

				//                   odrItems.Key.InvoiceNo,
				//                   odrItems.Key.Comments,
				//                   Address = odrItems.Key.Address,//(oInfo.AddressID == null ? string.Empty : usrAddress.Address),
				//                   odrItems.Key.StoreAddress,
				//                   odrItems.Key.StoreAddress_ar,
				//                   odrItems.Key.StoreName,
				//                   odrItems.Key.StoreName_ar,
				//                   odrItems.Key.Latitude,
				//                   odrItems.Key.Longitude,
				//                   odrItems.Key.TrackingTime,
				//                   odrItems.Key.OrderStatus,
				//                   odrItems.Key.Id,
				//                   odrItems.Key.phone,
				//                   odrItems.Key.ItemName,
				//                   odrItems.Key.ItemName_Ar,
				//                   odrItems.Key.ItemId,
				//                   odrItems.Key.Qty,
				//                   odrItems.Key.SizeName,
				//                   odrItems.Key.ItemType,
				//                   odrItems.Key.AcceptedBy,
				//                   PromoAmt = odrItems.Key.PromoAmt,//(Promo.BonusAmt ==null? 0:Promo.BonusAmt)
				//                                                    //  odrItems.Key.AdditionalPrice,
				//                                                    //odrItems.Key.AdditionalID,
				//                                                    //odrItems.Key.AdditionalName,
				//                                                    //odrItems.Key.AdditionalName_Ar,
				//                   odrItems.Key.ItemPrice,
				//                   odrItems.Key.Ratings
				//               }).OrderBy(odate => odate.Id).Distinct().ToList();
				#endregion
				JProperty JordDet;
				//string Status = "";
				foreach (var ordetails in OdrInfo.AsEnumerable())  // order by added addtional for tracking descending
				{

					Jobj = new JObject(new JProperty("Failure", "Order Under Process"));

					if (lstProp.Count == 0)
					{
						if (!oStatus.Contains(ordetails.OrderStatus))
						{
							oStatus.Add(ordetails.OrderStatus);

							var itemid = OdrInfo.Select(i => i.Id).Distinct();

							var distintItems = (from fOrder in OdrInfo
												where itemid.Contains(fOrder.Id)
												select new
												{
													fOrder.ItemName,
													fOrder.ItemName_Ar,
													fOrder.ItemPrice,
													fOrder.ActualPrice,
													fOrder.Qty,
													fOrder.ItemId,
													fOrder.OrderId,
													fOrder.oItemId,
													fOrder.SizeName,
													fOrder.ItemType,
													fOrder.OrderItemId
												}).Distinct().ToList();

							var subItems = (from SubItm in OdrInfo select new { SubItm.OrderId, SubItm.oItemId, SubItm.SubItem, SubItm.SubItemAr, SubItm.ItemName_Ar, SubItm.subItemPrice, SubItm.subItemQty }).Distinct().ToList();

							var rating = (from rat in db.Ratings where rat.OrderType == ordetails.OrderType && rat.IsActive == true select new { Rating = rat.Rating1 }).Distinct().OrderByDescending(x => x.Rating).ToList();

							var specialCake = (from SpOrder in OdrInfo
											   where itemid.Contains(SpOrder.Id)
											   select new
											   {
												   SpOrder.OrderItemCake_ID,
												   SpOrder.OrderItemId,
												   SpOrder.OrderId,
												   SpOrder.MessageOnCake,
												   SpOrder.RefImage,
												   SpOrder.ImsgrOnCake,
												   SpOrder.Shape,
												   SpOrder.Layers
											   }).Distinct().ToList();

							var specialCakeLyr = (from SpLyrOrder in OdrInfo
												  where itemid.Contains(SpLyrOrder.Id)
												  select new
												  {
													  SpLyrOrder.OrderId,
													  SpLyrOrder.OrderItemId,
													  SpLyrOrder.OrderItemCakeId,
													  SpLyrOrder.FlavorId,
													  SpLyrOrder.FlavorName,
													  SpLyrOrder.FlavorImage,
													  SpLyrOrder.LayerNo
												  }).Distinct().ToList();

							JObject Jobjrating = new JObject();

							foreach (var item in rating)
							{
								JObject Other = new JObject(new JProperty("RatingId", 0), new JProperty("Comment_En", "Other"), new JProperty("Comment_Ar", "الآخرين"), new JProperty("Rating", item.Rating));

								var rate = (from ra in db.Ratings where ra.Rating1 == item.Rating && ra.OrderType == ordetails.OrderType select new { ra.RatingId, ra.Comment_Ar, ra.Comment_En, ra.OrderType, Rating = ra.Rating1 }).Distinct().OrderBy(x => x.Rating).ToList();

								JArray jaRate = new JArray(JArray.Parse(JsonConvert.SerializeObject(rate)));
								jaRate.Add(Other);
								JProperty RJobject = new JProperty(item.Rating.ToString(), jaRate);
								Jobjrating.Add(RJobject);

							}
							JArray ratArr = new JArray();
							ratArr.Add(Jobjrating);

							int ItemQCount = (from ord in distintItems select ord.Qty).Sum();

							var CurrentOrderStatus = OdrInfo.OrderByDescending(x => x.TrackingTime).FirstOrDefault();

							string PaymentModeName = "";
							if (CurrentOrderStatus.PaymentMode == 1)
							{
								PaymentModeName = "Wallet";
							}
							else if (CurrentOrderStatus.PaymentMode == 2)
							{
								PaymentModeName = "Cash";
							}
							else if (CurrentOrderStatus.PaymentMode == 3)
							{
								PaymentModeName = "Online";
							}
							else if (CurrentOrderStatus.PaymentMode == 4)
							{
								PaymentModeName = "STC_Pay";
							}




							JordDet = new JProperty(ordetails.OrderStatus, new JArray(new JObject(
													   new JProperty("InvoiceNo", ordetails.InvoiceNo),
														 new JProperty("OrderDate", ordetails.OrderDate),
														   new JProperty("OrderId", ordetails.OrderId),
															 //new JProperty("OrderStatus", ordetails.OrderStatus),
															 // new JProperty("OrderStatus", OrStatusForRej.Contains(CurrentOrderStatus.OrderStatus) ? "Cancel" : CurrentOrderStatus.OrderStatus),
															 new JProperty("OrderStatus", CurrentOrderStatus.OrderStatus),
															 new JProperty("PaymentMode", PaymentModeName),
															  new JProperty("Total_Price", ordetails.Total_Price),
															  new JProperty("DeliveryCharges", ordetails.DeliveryCharges),
															  new JProperty("VatCharges", ordetails.VatCharges),
															  new JProperty("VatPercentage", ordetails.VatPercentage),
															  new JProperty("SubTotal", ordetails.SubTotal),
															  new JProperty("discountAmount", ordetails.discountAmount),
															  new JProperty("NetTotal", ordetails.NetTotal),
															 new JProperty("Comments", ordetails.Comments),
															 new JProperty("PromoAmt", ordetails.PromoAmt),
															   new JProperty("OrderType", ordetails.OrderType),
														   new JProperty("StoreAddress", ordetails.StoreAddress),
															 new JProperty("StoreAddress_ar", ordetails.StoreAddress_ar),
															  new JProperty("StoreName", ordetails.StoreName),
															   new JProperty("StoreName_ar", ordetails.StoreName_ar),
															   new JProperty("Latitude", ordetails.Latitude),
															   new JProperty("Longitude", ordetails.Longitude),
															   new JProperty("phone", ordetails.phone),
														   new JProperty("TrackingTime", ordetails.TrackingTime),
															 // new JProperty("DriverName", ordetails.AcceptedBy == null ? "" : JsonConvert.SerializeObject(db.Registrations.Where(reg => reg.UserId == ordetails.AcceptedBy).Select(regName => new { regName.FullName }).ToList())),
															 //new JProperty("DriverMobile", ordetails.driverMobile),
															 new JProperty("UserId", ordetails.UserId),
															 new JProperty("Address", ordetails.Address),
															 new JProperty("Itemcnt", (from fOrder in OdrInfo
																					   where fOrder.OrderId == ordetails.OrderId
																					   select fOrder.ItemId).Distinct().Count()),

																new JProperty("QuantityCount", ItemQCount), // Talha
																new JProperty("Rating", ordetails.Ratings),// Talha
																new JProperty("FoodRating", ordetails.OrderRat),
																 new JProperty("Ratings", ratArr),  // Talha
															 new JProperty("ExpectedTime", ordetails.ExpectedTime),
															 new JProperty("items", from fOrder in distintItems
																					where fOrder.OrderId == ordetails.OrderId
																					select new JObject(
																						new JProperty("ItmName", fOrder.ItemName),
																						new JProperty("ItmName_ar", fOrder.ItemName_Ar),
																						new JProperty("ItemQty", fOrder.Qty),
																						 new JProperty("ItemType", fOrder.ItemType == 1 ? "Whole" : fOrder.ItemType == 2 ? "Slices" : "Whole"),
																						new JProperty("SizeName", fOrder.SizeName),
																						new JProperty("price", fOrder.ItemPrice),
																						new JProperty("ActualPrice", fOrder.ActualPrice),
																							new JProperty("oAdd", from sbt in subItems
																												  where sbt.oItemId == fOrder.oItemId && fOrder.OrderId == ordetails.OrderId
																												  select new JObject(
																														 new JProperty("SubItem", sbt.SubItem),
																													  new JProperty("SubItemQty", sbt.subItemQty))

																						 ),
																						 new JProperty("SpecialCake", from SPOrder in specialCake
																													  where SPOrder.OrderItemId == fOrder.OrderItemId && fOrder.OrderId == ordetails.OrderId
																													  select new JObject(
																														  new JProperty("MessageOnCake", SPOrder.MessageOnCake),
																														  new JProperty("RefImage", SPOrder.RefImage),
																														  new JProperty("ImgOnCake", SPOrder.ImsgrOnCake),
																														   new JProperty("Shape", SPOrder.Shape),
																														  new JProperty("Layers", SPOrder.Layers),
																															  new JProperty("LayerDetails", from SpCakeLr in specialCakeLyr
																																							where SpCakeLr.OrderItemCakeId == SPOrder.OrderItemCake_ID && SpCakeLr.OrderItemId == fOrder.OrderItemId
																																							select new JObject(
																																							new JProperty("FlavorId", SpCakeLr.FlavorId),
																																							new JProperty("FlavorName", SpCakeLr.FlavorName),
																																							new JProperty("FlavorImage", SpCakeLr.FlavorImage),
																																							new JProperty("LayerNo", SpCakeLr.LayerNo))

																																																   )))

																						 ))

																						 )));//));
							lstProp.Add(JordDet);
							break;
						}
					}
					else
					{ }


				}

				var oStauts = OdrInfo.Select(odr => new { odr.TrackingTime, odr.UserId, odr.OrderStatus, odr.AcceptedBy }).Distinct();
				//string Dname = string.Empty;
				//string Dmobile = string.Empty;
				// int cnt = 0;
				List<string> status = new List<string>();
				foreach (var result in oStauts)
				{
					//if(Status!=result.OrderStatus ||!oStatus.Contains(result.OrderStatus))
					if (!oStatus.Contains(result.OrderStatus))
					{
						if (!status.Contains(result.OrderStatus))
						{
							status.Add(result.OrderStatus);

							//JordDet = new JProperty(OrStatusForRej.Contains(result.OrderStatus) ? "Cancel" : result.OrderStatus, new JArray(new JObject(
							JordDet = new JProperty(result.OrderStatus, new JArray(new JObject(
															 new JProperty("TrackingTime", result.TrackingTime),
															 // result.AcceptedBy == null ? new  JProperty("FullName","") :  JProperty.Parse(JsonConvert.SerializeObject(db.Registrations.Where(reg => reg.UserId == result.AcceptedBy).Select(regName => new { regName.FullName }).ToList()))),
															 // new JProperty("DMobile", result.AcceptedBy == null ? "" : JsonConvert.SerializeObject(db.Registrations.Where(reg => reg.UserId == result.AcceptedBy).Select(regName => new { regName.Mobile }).ToList().ToString())),
															 // Commented
															 //new JProperty("DriverName", result.AcceptedBy == null ? "" : JProperty.Parse(JsonConvert.SerializeObject(db.Registrations.Where(reg => reg.UserId == result.AcceptedBy).Select(regName => new { regName.FullName, regName.Mobile }).ToList()))),
															 new JProperty("DriverName", result.AcceptedBy == null ? JArray.Parse("[]") : JProperty.Parse(JsonConvert.SerializeObject(db.Registrations.Where(reg => reg.UserId == result.AcceptedBy).Select(regName => new { regName.FullName, regName.Mobile }).ToList()))),

															   new JProperty("UserId", result.UserId))));
							lstProp.Add(JordDet);
						}
					}
				}
				return OdrInfo.Count > 0 ? Jobj = new JObject(lstProp) : Jobj;
			}
			catch (Exception ex)
			{
				return Jobj;
			}
		}

		public JObject TrackOrder(int orderid, int userid)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Order Found"));
			try
			{
				List<JProperty> lstProp = new List<JProperty>();
				List<string> oStatus = new List<string>();
				oStatus.Add("Accepted");
				oStatus.Add("DriverCancel");

				// Shahed created list for if orderstatus ='Rejected' then it shows cancels.

				List<string> OrStatusForRej = new List<string>();
				OrStatusForRej.Add("Rejected");

				//End Shahed updated

				if (orderid == -1)
				{
					var or = ((from x in db.OrderInfoes where x.UserId == userid && x.status == true select new { x.OrderId }).OrderByDescending(odate => odate.OrderId).Take(1)).ToList();

					foreach (var odrid in or)
					{
						orderid = Convert.ToInt32(odrid.OrderId);
					}
				}

				var OdrInfo = db.SP_Get_OrderTracking(userid, orderid).OrderBy(odate => odate.Id).Distinct().ToList();

				#region oldcode

				//var OdrInfo = (from oInfo in db.OrderInfoes
				//               join oTrack in db.OrderTrackings on oInfo.OrderId equals oTrack.OrderId into OrderT

				//               join oStores in db.StoreInformations on oInfo.StoreId equals oStores.StoreId
				//               join oStrWork in db.StoreWorkDatas on oInfo.StoreId equals oStrWork.StoreId
				//               join oItems in db.OrderItems on oInfo.OrderId equals oItems.OrderId
				//               join ItSize in db.ItemSizes on oItems.Size equals ItSize.TypeId

				//              // from oSubItm in db.OrderSubItems.Where(ra => ra.OrderItemId == oItems.Id).DefaultIfEmpty()
				//               //join oSubItm in db.OrderSubItems on oItems.Id equals oSubItm.OrderItemId  //sub Items in Items

				//               //join subCat in db.SubCategories on oItems.ItemId equals subCat.SubCategoryId
				//               join Itm in db.ItemsDetails on oItems.ItemId equals Itm.ItemId

				//              // from ItmS in db.ItemsDetails.Where(ra => ra.ItemId == oSubItm.OrderSubItemId).DefaultIfEmpty()
				//             //join ItmS in db.ItemsDetails on oSubItm.OrderSubItemId equals ItmS.ItemId

				//               // join oAdd in db.OrderAdditionals on oItems.Id equals oAdd.ItemId
				//               //join addDet in db.AdditionalInfoLists on oAdd.AdditionalID equals addDet.AdditionalId

				//               join oPromo in db.userPromotions on oInfo.OrderId equals oPromo.orderId into odrPromo
				//               join oAddress in db.UserAddressDetails on oInfo.AddressID equals oAddress.Id into UserAddress
				//               from usrAddress in UserAddress.DefaultIfEmpty()
				//               from ordTrack in OrderT.DefaultIfEmpty()
				//               from Promo in odrPromo.DefaultIfEmpty()
				//                   //join driver in db.Registrations on  ordTrack.AcceptedBy equals driver.UserId into dDetails
				//                   //from drvDetails in dDetails.DefaultIfEmpty()
				//               from rat in db.UserRatings.Where(ra => ra.UserId == userid && ra.OrderId == orderid).DefaultIfEmpty()
				//               where oInfo.OrderId == orderid && oInfo.status == true
				//               group new { ordTrack, oInfo, Promo, usrAddress, oStrWork, oStores, oItems, Itm }//, oAdd, addDet }
				//      by new
				//      {
				//          oInfo.UserId,
				//          oInfo.OrderId,
				//          oInfo.OrderDate,
				//          oInfo.OrderType,
				//          oInfo.ExpectedTime,
				//          VatPercentage = (oInfo.VatPercentage == null ? "0" : oInfo.VatPercentage),
				//          VatCharges = (oInfo.VatCharges == null ? "0" : oInfo.VatCharges),
				//          SubTotal = (oInfo.SubTotal == null ? oInfo.Total_Price : oInfo.SubTotal),
				//          oInfo.Total_Price,
				//          oInfo.Comments,
				//         //subCat.ItemName,
				//         Itm.ItemId,
				//          Itm.ItemName,
				//          Itm.ItemName_Ar,


				//          SubItemId = 1, //oItems.Id,
				//       SubItem = "",//ItmS.ItemName,
				//       SubItemAr= "",// ItmS.ItemName_Ar,
				//      subItemQty =  "", // oSubItm.Qty,
				//      subItemPrice = "",// oSubItm.ItemPrice,
				//         //ItemId = subCat.SubCategoryId,
				//         //subCat.ItemName_Ar,
				//         oItems.ItemPrice,
				//          oItems.Qty,
				//         ItemType = "",// oItems.ItemType==1 ? "Whole":"Slices",
				//          oItems.Size,

				//          SizeName = ItSize.TypeName,
				//          orderitem_orderid = oItems.OrderId,
				//          oItemId = oItems.Id,
				//          oInfo.InvoiceNo,
				//          Address = (oInfo.AddressID == null ? string.Empty : usrAddress.Address),
				//          oStores.StoreAddress,
				//          oStores.StoreAddress_ar,
				//          oStores.StoreName,
				//          oStores.StoreName_ar,
				//          oStores.Latitude,
				//          oStores.Longitude,
				//          ordTrack.TrackingTime,
				//          ordTrack.OrderStatus,
				//          ordTrack.Id,
				//          oStrWork.phone,
				//          ordTrack.AcceptedBy,
				//          PromoAmt = (Promo.BonusAmt == null ? 0 : Promo.BonusAmt),

				//          Ratings = rat.Ratings == null ? 0 : rat.Ratings
				//         //  oAdd.AdditionalPrice,
				//         //oAdd.AdditionalID,
				//         //addDet.AdditionalName,
				//         //addDet.AdditionalName_Ar

				//     } into odrItems
				//               select new
				//               {
				//                   odrItems.Key.UserId,
				//                   odrItems.Key.OrderId,
				//                   odrItems.Key.OrderDate,
				//                   odrItems.Key.OrderType,
				//                   odrItems.Key.ExpectedTime,
				//                   odrItems.Key.VatCharges,
				//                   odrItems.Key.SubTotal,
				//                   odrItems.Key.VatPercentage,
				//                   odrItems.Key.Total_Price,
				//                   odrItems.Key.orderitem_orderid,
				//                   odrItems.Key.oItemId,

				//                   odrItems.Key.SubItemId,
				//                   odrItems.Key.SubItem,
				//                   odrItems.Key.SubItemAr,
				//                   odrItems.Key.subItemQty,
				//                   odrItems.Key.subItemPrice,

				//                   odrItems.Key.InvoiceNo,
				//                   odrItems.Key.Comments,
				//                   Address = odrItems.Key.Address,//(oInfo.AddressID == null ? string.Empty : usrAddress.Address),
				//                   odrItems.Key.StoreAddress,
				//                   odrItems.Key.StoreAddress_ar,
				//                   odrItems.Key.StoreName,
				//                   odrItems.Key.StoreName_ar,
				//                   odrItems.Key.Latitude,
				//                   odrItems.Key.Longitude,
				//                   odrItems.Key.TrackingTime,
				//                   odrItems.Key.OrderStatus,
				//                   odrItems.Key.Id,
				//                   odrItems.Key.phone,
				//                   odrItems.Key.ItemName,
				//                   odrItems.Key.ItemName_Ar,
				//                   odrItems.Key.ItemId,
				//                   odrItems.Key.Qty,
				//                   odrItems.Key.SizeName,
				//                   odrItems.Key.ItemType,
				//                   odrItems.Key.AcceptedBy,
				//                   PromoAmt = odrItems.Key.PromoAmt,//(Promo.BonusAmt ==null? 0:Promo.BonusAmt)
				//                                                    //  odrItems.Key.AdditionalPrice,
				//                                                    //odrItems.Key.AdditionalID,
				//                                                    //odrItems.Key.AdditionalName,
				//                                                    //odrItems.Key.AdditionalName_Ar,
				//                   odrItems.Key.ItemPrice,
				//                   odrItems.Key.Ratings
				//               }).OrderBy(odate => odate.Id).Distinct().ToList();
				#endregion
				JProperty JordDet;
				//string Status = "";
				foreach (var ordetails in OdrInfo.AsEnumerable())  // order by added addtional for tracking descending
				{

					Jobj = new JObject(new JProperty("Failure", "Order Under Process"));

					if (lstProp.Count == 0)
					{
						if (!oStatus.Contains(ordetails.OrderStatus))
						{
							oStatus.Add(ordetails.OrderStatus);

							var itemid = OdrInfo.Select(i => i.Id).Distinct();

							var distintItems = (from fOrder in OdrInfo
												where itemid.Contains(fOrder.Id)
												select new
												{
													fOrder.ItemName,
													fOrder.ItemName_Ar,
													fOrder.ItemPrice,
													fOrder.Qty,
													fOrder.ItemId,
													fOrder.OrderId,
													fOrder.oItemId,
													fOrder.SizeName,

													fOrder.ItemType
												}).Distinct().ToList();

							var subItems = (from SubItm in OdrInfo select new { SubItm.OrderId, SubItm.oItemId, SubItm.SubItem, SubItm.SubItemAr, SubItm.ItemName_Ar, SubItm.subItemPrice, SubItm.subItemQty }).Distinct().ToList();

							var rating = (from rat in db.Ratings where rat.OrderType == ordetails.OrderType && rat.IsActive == true select new { Rating = rat.Rating1 }).Distinct().OrderByDescending(x => x.Rating).ToList();

							JObject Jobjrating = new JObject();

							foreach (var item in rating)
							{
								JObject Other = new JObject(new JProperty("RatingId", 0), new JProperty("Comment_En", "Other"), new JProperty("Comment_Ar", "الآخرين"), new JProperty("Rating", item.Rating));

								var rate = (from ra in db.Ratings where ra.Rating1 == item.Rating && ra.OrderType == ordetails.OrderType select new { ra.RatingId, ra.Comment_Ar, ra.Comment_En, ra.OrderType, Rating = ra.Rating1 }).Distinct().OrderBy(x => x.Rating).ToList();

								JArray jaRate = new JArray(JArray.Parse(JsonConvert.SerializeObject(rate)));
								jaRate.Add(Other);
								JProperty RJobject = new JProperty(item.Rating.ToString(), jaRate);
								Jobjrating.Add(RJobject);

							}
							JArray ratArr = new JArray();
							ratArr.Add(Jobjrating);

							int ItemQCount = (from ord in distintItems select ord.Qty).Sum();

							var CurrentOrderStatus = OdrInfo.OrderByDescending(x => x.TrackingTime).FirstOrDefault();

							string PaymentModeName = "";
							if (CurrentOrderStatus.PaymentMode == 1)
							{
								PaymentModeName = "Wallet";
							}
							else if (CurrentOrderStatus.PaymentMode == 2)
							{
								PaymentModeName = "Cash";
							}
							else if (CurrentOrderStatus.PaymentMode == 3)
							{
								PaymentModeName = "Online";
							}
							else if (CurrentOrderStatus.PaymentMode == 4)
							{
								PaymentModeName = "STC_Pay";
							}
							//Shahed write code
							//Shahed write code
							string SubTotal = ordetails.SubTotal.ToString();
							string VatCharges = ordetails.VatCharges.ToString();
							string VatPercentage = ordetails.VatPercentage.ToString();
							string Total_Price = ordetails.Total_Price.ToString();
							//End shahed Code
							//End shahed Code


							JordDet = new JProperty(ordetails.OrderStatus, new JArray(new JObject(
													   new JProperty("InvoiceNo", ordetails.InvoiceNo),
														 new JProperty("OrderDate", ordetails.OrderDate),
														   new JProperty("OrderId", ordetails.OrderId),
															 //new JProperty("OrderStatus", ordetails.OrderStatus),
															 // new JProperty("OrderStatus", OrStatusForRej.Contains(CurrentOrderStatus.OrderStatus) ? "Cancel" : CurrentOrderStatus.OrderStatus),
															 new JProperty("OrderStatus", CurrentOrderStatus.OrderStatus),
															 new JProperty("PaymentMode", PaymentModeName),
															 new JProperty("SubTotal", SubTotal),
															 new JProperty("VatCharges", VatCharges),
															 new JProperty("VatPercentage", VatPercentage),
															 new JProperty("Total_Price", Total_Price),
															 new JProperty("Comments", ordetails.Comments),
															 new JProperty("PromoAmt", ordetails.PromoAmt),
															   new JProperty("OrderType", ordetails.OrderType),
														   new JProperty("StoreAddress", ordetails.StoreAddress),
															 new JProperty("StoreAddress_ar", ordetails.StoreAddress_ar),
															  new JProperty("StoreName", ordetails.StoreName),
															   new JProperty("StoreName_ar", ordetails.StoreName_ar),
															   new JProperty("Latitude", ordetails.Latitude),
															   new JProperty("Longitude", ordetails.Longitude),
															   new JProperty("phone", ordetails.phone),
														   new JProperty("TrackingTime", ordetails.TrackingTime),
															 // new JProperty("DriverName", ordetails.AcceptedBy == null ? "" : JsonConvert.SerializeObject(db.Registrations.Where(reg => reg.UserId == ordetails.AcceptedBy).Select(regName => new { regName.FullName }).ToList())),
															 //new JProperty("DriverMobile", ordetails.driverMobile),
															 new JProperty("UserId", ordetails.UserId),
															 new JProperty("Address", ordetails.Address),
															 new JProperty("Itemcnt", (from fOrder in OdrInfo
																					   where fOrder.OrderId == ordetails.OrderId
																					   select fOrder.ItemId).Distinct().Count()),

																new JProperty("QuantityCount", ItemQCount), // Talha
																new JProperty("Rating", ordetails.Ratings),// Talha
																new JProperty("FoodRating", ordetails.OrderRat),
																 new JProperty("Ratings", ratArr),  // Talha
															 new JProperty("ExpectedTime", ordetails.ExpectedTime),
															 new JProperty("items", from fOrder in distintItems
																					where fOrder.OrderId == ordetails.OrderId
																					select new JObject(
																						new JProperty("ItmName", fOrder.ItemName),
																						new JProperty("ItmName_ar", fOrder.ItemName_Ar),
																						new JProperty("ItemQty", fOrder.Qty),
																						 new JProperty("ItemType", fOrder.ItemType == 1 ? "Whole" : fOrder.ItemType == 2 ? "Slices" : "Whole"),
																						new JProperty("SizeName", fOrder.SizeName),
																						new JProperty("price", fOrder.ItemPrice),
																							new JProperty("oAdd", from sbt in subItems
																												  where sbt.oItemId == fOrder.oItemId && fOrder.OrderId == ordetails.OrderId
																												  select new JObject(
																														 new JProperty("SubItem", sbt.SubItem),
																													  new JProperty("SubItemQty", sbt.subItemQty))

																						 ))))));//));
							lstProp.Add(JordDet);
							break;
						}
					}
					else
					{ }


				}

				var oStauts = OdrInfo.Select(odr => new { odr.TrackingTime, odr.UserId, odr.OrderStatus, odr.AcceptedBy }).Distinct();
				//string Dname = string.Empty;
				//string Dmobile = string.Empty;
				// int cnt = 0;
				List<string> status = new List<string>();
				foreach (var result in oStauts)
				{
					//if(Status!=result.OrderStatus ||!oStatus.Contains(result.OrderStatus))
					if (!oStatus.Contains(result.OrderStatus))
					{
						if (!status.Contains(result.OrderStatus))
						{
							status.Add(result.OrderStatus);
							//status.Add(OrStatusForRej.Contains(result.OrderStatus) ? "Cancel" : result.OrderStatus);

							JordDet = new JProperty(OrStatusForRej.Contains(result.OrderStatus) ? "Cancel" : result.OrderStatus, new JArray(new JObject(
															 new JProperty("TrackingTime", result.TrackingTime),
															 // result.AcceptedBy == null ? new  JProperty("FullName","") :  JProperty.Parse(JsonConvert.SerializeObject(db.Registrations.Where(reg => reg.UserId == result.AcceptedBy).Select(regName => new { regName.FullName }).ToList()))),
															 // new JProperty("DMobile", result.AcceptedBy == null ? "" : JsonConvert.SerializeObject(db.Registrations.Where(reg => reg.UserId == result.AcceptedBy).Select(regName => new { regName.Mobile }).ToList().ToString())),
															 // Commented
															 //new JProperty("DriverName", result.AcceptedBy == null ? "" : JProperty.Parse(JsonConvert.SerializeObject(db.Registrations.Where(reg => reg.UserId == result.AcceptedBy).Select(regName => new { regName.FullName, regName.Mobile }).ToList()))),
															 new JProperty("DriverName", result.AcceptedBy == null ? JArray.Parse("[]") : JProperty.Parse(JsonConvert.SerializeObject(db.Registrations.Where(reg => reg.UserId == result.AcceptedBy).Select(regName => new { regName.FullName, regName.Mobile }).ToList()))),

															   new JProperty("UserId", result.UserId))));
							lstProp.Add(JordDet);
						}
					}
				}
				return OdrInfo.Count > 0 ? Jobj = new JObject(lstProp) : Jobj;
			}
			catch (Exception ex)
			{
				return Jobj;
			}
		}

		public JObject GetOrderHistory_New(int UserId)
		{
			List<string> oStatus = new List<string>();
			oStatus.Add("Accepted");
			oStatus.Add("Pending");
			oStatus.Add("DriverCancel");
			List<JObject> obj = new List<JObject>();
			JObject JobjItems;
			JObject Jobj = new JObject(new JProperty("Failure", false));

			var res = (from oItems in db.OrderItems
					   join oInfo in db.OrderInfoes
					   on oItems.OrderId equals oInfo.OrderId
					   join subCat in db.ItemsDetails
					   on oItems.ItemId equals subCat.ItemId
					   join oAddress in db.UserAddressDetails on oInfo.AddressID equals oAddress.Id into UserAddress
					   from usrAddress in UserAddress.DefaultIfEmpty()
					   join o in db.StoreInformations
					   on oInfo.StoreId equals o.StoreId
					   join OrdRat in db.OrderRatings on oInfo.OrderId equals OrdRat.OrderId into orderrat
					   from ordrating in orderrat.DefaultIfEmpty()
					   where oInfo.UserId == UserId && oInfo.status == true && oInfo.OrderStatus != "Wait"//&& oInfo.IsFavorite == true
					   group new { oItems, oInfo, subCat, usrAddress, o, ordrating }
						   by new
						   {
							   oInfo.OrderId,
							   oInfo.FavoriteName,
							   oInfo.StoreId,
							   oInfo.IsFavorite,
							   oInfo.InvoiceNo,
							   oInfo.OrderStatus,
							   oInfo.OrderType,
							   oInfo.Comments,
							   oItems.Qty,
							   o.StoreName,
							   o.StoreName_ar,
							   oInfo.Total_Price,
							   oInfo.NetTotal,
							   subCat.ItemName,
							   subCat.ItemName_Ar,
							   oInfo.OrderDate,
							   usrAddress.Address,
							   ordrating.OrderRating1,
							   ordrating.OrderComment,
							   ordrating.DriverRating,
							   ordrating.DriverComment
						   } into odrItems
					   select new
					   {
						   odrId = odrItems.Key.OrderId,
						   Fname = odrItems.Key.FavoriteName,
						   Sname = odrItems.Key.StoreName,
						   InvoiceNo = odrItems.Key.InvoiceNo,
						   odrItems.Key.Comments,
						   OrderStatus = odrItems.Key.OrderStatus,
						   OrderType = odrItems.Key.OrderType,
						   Sname_ar = odrItems.Key.StoreName_ar,
						   ItmName = odrItems.Key.Qty + " " + odrItems.Key.ItemName,// odrItems.Count() + " " + odrItems.Key.ItemName,
						   ItmName_ar = odrItems.Key.Qty + " " + odrItems.Key.ItemName_Ar,//odrItems.Count() + " " + odrItems.Key.ItemName_Ar,
						   UserAddress = (odrItems.Key.Address == null ? string.Empty : odrItems.Key.Address),
						   TotPrice = odrItems.Key.NetTotal == null ? odrItems.Key.Total_Price : odrItems.Key.NetTotal,
						   Odate = odrItems.Key.OrderDate,
						   ordratig = (odrItems.Key.OrderRating1 == null ? 0 : odrItems.Key.OrderRating1),
						   ordraCmt = (odrItems.Key.OrderComment == null ? string.Empty : odrItems.Key.OrderComment),
						   Drivratig = (odrItems.Key.DriverRating == null ? 0 : odrItems.Key.DriverRating),
						   DrivCmt = (odrItems.Key.DriverComment == null ? string.Empty : odrItems.Key.DriverComment),
					   }).OrderByDescending(odate => odate.Odate).ToList();
			int oInfoId = 0;
			decimal? TotPri;
			foreach (var favOrder in res.AsEnumerable())
			{
				if (oInfoId != favOrder.odrId)
				{

					TotPri = favOrder.TotPrice;
					oInfoId = favOrder.odrId;
					JobjItems = new JObject(
												new JProperty("odrId", favOrder.odrId),
											   new JProperty("Fname", favOrder.Fname),
											  new JProperty("Sname", favOrder.Sname),
											   new JProperty("Sname_ar", favOrder.Sname_ar),
												new JProperty("Comments", favOrder.Comments),
												new JProperty("UserAddress", favOrder.UserAddress),
												//new JProperty("TotPrice", favOrder.TotPrice),
												new JProperty("TotPrice", TotPri),
												new JProperty("InvoiceNo", favOrder.InvoiceNo),
											   // new JProperty("OrderStatus", favOrder.OrderStatus),//=="Accepted"|| favOrder.OrderStatus=="PCancel" ? "" :favOrder.OrderStatus),
											   new JProperty("OrderStatus", oStatus.Contains(favOrder.OrderStatus) ? "Accepted" : favOrder.OrderStatus),//=="Accepted"|| favOrder.OrderStatus=="PCancel" ? "" :favOrder.OrderStatus),
												new JProperty("OrderType", favOrder.OrderType),
											   new JProperty("Odate", String.Format("{0:g}", favOrder.Odate)),
											   new JProperty("FoodRating", favOrder.ordratig),
											   new JProperty("FoodComment", favOrder.ordraCmt),
												new JProperty("DriverRating", favOrder.Drivratig),
											   new JProperty("DriverComment", favOrder.DrivCmt),
									   new JProperty("items", from fOrder in res
															  where fOrder.odrId == oInfoId
															  select new JObject(
																  new JProperty("ItmName", fOrder.ItmName),
																  new JProperty("ItmName_ar", fOrder.ItmName_ar))



					));


					obj.Add(JobjItems);

				}
			}
			//return Jobj = new JObject(new JProperty("Success", obj));
			return obj.Count > 0 ? Jobj = new JObject(new JProperty("Success", obj)) : Jobj;
		}

		public JObject GetOrderHistory(int UserId)
		{
			List<string> oStatus = new List<string>();
			oStatus.Add("Accepted");
			oStatus.Add("Pending");
			oStatus.Add("DriverCancel");
			List<JObject> obj = new List<JObject>();
			JObject JobjItems;
			JObject Jobj = new JObject(new JProperty("Failure", false));

			var res = (from oItems in db.OrderItems
					   join oInfo in db.OrderInfoes
					   on oItems.OrderId equals oInfo.OrderId
					   join subCat in db.ItemsDetails
					   on oItems.ItemId equals subCat.ItemId
					   join oAddress in db.UserAddressDetails on oInfo.AddressID equals oAddress.Id into UserAddress
					   from usrAddress in UserAddress.DefaultIfEmpty()
					   join o in db.StoreInformations
					   on oInfo.StoreId equals o.StoreId
					   join OrdRat in db.OrderRatings on oInfo.OrderId equals OrdRat.OrderId into orderrat
					   from ordrating in orderrat.DefaultIfEmpty()
					   where oInfo.UserId == UserId && oInfo.status == true && oInfo.OrderStatus != "Wait"//&& oInfo.IsFavorite == true
					   group new { oItems, oInfo, subCat, usrAddress, o, ordrating }
						   by new
						   {
							   oInfo.OrderId,
							   oInfo.FavoriteName,
							   oInfo.StoreId,
							   oInfo.IsFavorite,
							   oInfo.InvoiceNo,
							   oInfo.OrderStatus,
							   oInfo.OrderType,
							   oInfo.Comments,
							   oItems.Qty,
							   o.StoreName,
							   o.StoreName_ar,
							   oInfo.Total_Price,
							   oInfo.NetTotal,
							   subCat.ItemName,
							   subCat.ItemName_Ar,
							   oInfo.OrderDate,
							   usrAddress.Address,
							   ordrating.OrderRating1,
							   ordrating.OrderComment,
							   ordrating.DriverRating,
							   ordrating.DriverComment
						   } into odrItems
					   select new
					   {
						   odrId = odrItems.Key.OrderId,
						   Fname = odrItems.Key.FavoriteName,
						   Sname = odrItems.Key.StoreName,
						   InvoiceNo = odrItems.Key.InvoiceNo,
						   odrItems.Key.Comments,
						   OrderStatus = odrItems.Key.OrderStatus,
						   OrderType = odrItems.Key.OrderType,
						   Sname_ar = odrItems.Key.StoreName_ar,
						   ItmName = odrItems.Key.Qty + " " + odrItems.Key.ItemName,// odrItems.Count() + " " + odrItems.Key.ItemName,
						   ItmName_ar = odrItems.Key.Qty + " " + odrItems.Key.ItemName_Ar,//odrItems.Count() + " " + odrItems.Key.ItemName_Ar,
						   UserAddress = (odrItems.Key.Address == null ? string.Empty : odrItems.Key.Address),
						   TotPrice = odrItems.Key.NetTotal == null ? odrItems.Key.Total_Price : odrItems.Key.NetTotal,
						   Odate = odrItems.Key.OrderDate,
						   ordratig = (odrItems.Key.OrderRating1 == null ? 0 : odrItems.Key.OrderRating1),
						   ordraCmt = (odrItems.Key.OrderComment == null ? string.Empty : odrItems.Key.OrderComment),
						   Drivratig = (odrItems.Key.DriverRating == null ? 0 : odrItems.Key.DriverRating),
						   DrivCmt = (odrItems.Key.DriverComment == null ? string.Empty : odrItems.Key.DriverComment),
					   }).OrderByDescending(odate => odate.Odate).ToList();
			int oInfoId = 0;
			string TotPrice = "0.00";
			foreach (var favOrder in res.AsEnumerable())
			{
				if (oInfoId != favOrder.odrId)
				{
					TotPrice = favOrder.TotPrice.ToString();
					oInfoId = favOrder.odrId;
					JobjItems = new JObject(
												new JProperty("odrId", favOrder.odrId),
											   new JProperty("Fname", favOrder.Fname),
											  new JProperty("Sname", favOrder.Sname),
											   new JProperty("Sname_ar", favOrder.Sname_ar),
												new JProperty("Comments", favOrder.Comments),
												new JProperty("UserAddress", favOrder.UserAddress),
												//new JProperty("TotPrice", favOrder.TotPrice),
												new JProperty("TotPrice", TotPrice),
												new JProperty("InvoiceNo", favOrder.InvoiceNo),
											   // new JProperty("OrderStatus", favOrder.OrderStatus),//=="Accepted"|| favOrder.OrderStatus=="PCancel" ? "" :favOrder.OrderStatus),
											   new JProperty("OrderStatus", oStatus.Contains(favOrder.OrderStatus) ? "Accepted" : favOrder.OrderStatus),//=="Accepted"|| favOrder.OrderStatus=="PCancel" ? "" :favOrder.OrderStatus),
												new JProperty("OrderType", favOrder.OrderType),
											   new JProperty("Odate", String.Format("{0:g}", favOrder.Odate)),
											   new JProperty("FoodRating", favOrder.ordratig),
											   new JProperty("FoodComment", favOrder.ordraCmt),
												new JProperty("DriverRating", favOrder.Drivratig),
											   new JProperty("DriverComment", favOrder.DrivCmt),
									   new JProperty("items", from fOrder in res
															  where fOrder.odrId == oInfoId
															  select new JObject(
																  new JProperty("ItmName", fOrder.ItmName),
																  new JProperty("ItmName_ar", fOrder.ItmName_ar))



					));


					obj.Add(JobjItems);

				}
			}
			//return Jobj = new JObject(new JProperty("Success", obj));
			return obj.Count > 0 ? Jobj = new JObject(new JProperty("Success", obj)) : Jobj;
		}

		public JObject GetOrderDetails(int orderid, int userid)
		{
			JObject jobj = new JObject(new JProperty("Failure", "Record Not Found"));

			try
			{
				int? oInfoId = 0;
				int? ItemId = 0;
				List<JArray> Jlist = new List<JArray>();
				JArray JorderInfo = new JArray();
				JObject obj = new JObject();
				if (orderid == -1)
				{

					var or = ((from x in db.OrderInfoes where x.UserId == userid && x.status == true select new { x.OrderId }).OrderByDescending(odate => odate.OrderId).Take(1)).ToList();
					// (db.OrderInfoes.Where(x => x.UserId == 2).Select(Ores => new { Ores.OrderId }).OrderByDescending(odate => odate.OrderId).Take(1).Cast<int>());
					//= Convert.ToInt32(or);
					foreach (var odrid in or)
					{
						orderid = Convert.ToInt32(odrid.OrderId);

					}
				}

				//  var res = db.SP_Get_OrderTracking(userid, orderid).OrderBy(odate => odate.Id).Distinct().ToList();
				var res = (from oInfo in db.OrderInfoes
						   join oItems in db.OrderItems on oInfo.OrderId equals oItems.OrderId
						   from itemsize in db.ItemSizes.Where(x => x.TypeId == oItems.Size).DefaultIfEmpty()
						   from Promo in db.userPromotions.Where(x => x.orderId == oInfo.OrderId).DefaultIfEmpty() // into odrPromo

						   join oItemsName in db.ItemsDetails on oItems.ItemId equals oItemsName.ItemId
						   join oSubcat in db.SubCategories on oItemsName.SubCatId equals oSubcat.SubCategoryId

						   //join osubItems in db.OrderSubItems on oItems.Id equals osubItems.OrderItemId 
						   from odrsbItems in db.OrderSubItems.Where(w => w.OrderItemId == oItems.Id).DefaultIfEmpty()
						   from osubItmName in db.ItemsDetails.Where(w => w.ItemId == odrsbItems.OrderSubItemId).DefaultIfEmpty()

							   //join osubItems in db.OrderSubItems on oItems.Id equals osubItems.OrderItemId into oSubItems
							   //from odrsbItems in oSubItems.DefaultIfEmpty()

							   //join osubItemName in db.ItemsDetails on odrsbItems.OrderSubItemId equals osubItemName.ItemId into osubitm
							   // from osubItmName in osubitm.DefaultIfEmpty()

							   //from Promo in odrPromo.DefaultIfEmpty()

						   where oInfo.OrderId == orderid && oInfo.status == true
						   select new
						   {
							   oInfo.UserId,
							   oInfo.OrderId,
							   oInfo.StoreId,
							   oItems.Comments,
							   oInfo.OrderDate,
							   oInfo.OrderType,
							   oInfo.ExpectedTime,
							   oInfo.OrderStatus,
							   oInfo.Total_Price,
							   oInfo.SubTotal,
							   oInfo.VatCharges,
							   oInfo.VatPercentage,
							   oInfo.IsFavorite,
							   oInfo.FavoriteName,
							   oInfo.PaymentMode,
							   oInfo.InvoiceNo,
							   oItems.Id,
							   oItems.ItemId,
							   oItems.ItemPrice,
							   oItems.Qty,
							   oItems.Size,
							   ItemType = oItems.ItemType == 0 ? "" : oItems.ItemType == 1 ? "Whole" : oItems.ItemType == 2 ? "Piece" : "",
							   itemsize.SizeName,
							   itemsize.SizeName_Ar, // added later
													 //addites.ItemId,
													 //addites.AdditionalID,
													 //addites.AdditionalPrice,
							   oItemsName.ItemName,
							   oItemsName.ItemName_Ar,
							   //oItemsName.Images,
							   oItemsName.ItemDesc,
							   oItemsName.ItemDesc_Ar,
							   oItemsName.SubCatId,
							   oSubcat.CategoryId,
							   //additionals.AdditionalName,
							   //additionals.AdditionalName_Ar,
							   PromoAmt = (Promo.BonusAmt == null ? 0 : Promo.BonusAmt),

							   OItemId = (odrsbItems.OrderItemId == null ? 0 : odrsbItems.OrderItemId),
							   OSubItemId = (odrsbItems.OrderSubItemId == null ? 0 : odrsbItems.OrderSubItemId),
							   OSubItemName = osubItmName.ItemName,
							   OSubItemName_Ar = osubItmName.ItemName_Ar,
							   OSubItemImage = osubItmName.images,
							   OSubItemSeq = (osubItmName.ItemSeq == null ? 0 : osubItmName.ItemSeq),
							   OSubItemQuantity = (odrsbItems.Qty == null ? 0 : odrsbItems.Qty),
							   OSubItemSize = (odrsbItems.Size == null ? "" : "Peice"),
							   OSubItemComments = odrsbItems.Comments,
							   OSubItemPrice = odrsbItems.ItemPrice
							   //oAdditionalName.Images




						   }).OrderByDescending(odate => odate.OrderDate).ToList();

				if (res.Count > 0)
				{
					int TotalItems = 0;
					foreach (var oItems in res.AsEnumerable())
					{

						if (oInfoId != oItems.OrderId)
						{

							oInfoId = oItems.OrderId;


							obj = new JObject(new JProperty("UserId", oItems.UserId),
															   new JProperty("StoreId", oItems.StoreId),
															   new JProperty("OrderId", oItems.OrderId),
															   new JProperty("OrderDate", String.Format("{0:g}", oItems.OrderDate)),
															   new JProperty("OrderType", oItems.OrderType),
															   new JProperty("ExpectedTime", oItems.ExpectedTime),
															   new JProperty("OrderStatus", oItems.OrderStatus),
															   new JProperty("Total_Price", oItems.Total_Price),
																new JProperty("VatCharges", oItems.VatCharges),
															   new JProperty("VatPercentage", oItems.VatPercentage),
															   new JProperty("SubTotal", oItems.SubTotal),

															   new JProperty("PromoAmt", oItems.PromoAmt),
															   new JProperty("IsFavorite", oItems.IsFavorite),
																 new JProperty("FavoriteName", oItems.FavoriteName),
																   new JProperty("PaymentMode", oItems.PaymentMode),
																	 new JProperty("InvoiceNo", oItems.InvoiceNo));


						}
						JArray jsubItem = new JArray();
						if (ItemId != oItems.Id)
						{
							TotalItems = ++TotalItems;
							ItemId = oItems.Id;
							int subItemId = 0;
							foreach (var subItems in res.AsEnumerable().OrderBy(x => x.OItemId).ToList())
							{
								if (subItemId != subItems.OSubItemId && subItems.Id == oItems.OItemId && subItems.OItemId == ItemId)
								{
									subItemId = subItems.Id;

									JObject jaitems = new JObject(new JProperty("SubItemId", subItems.OSubItemId),
																  new JProperty("SubItemName", subItems.OSubItemName),
																  new JProperty("SubItemName_Ar", subItems.OSubItemName_Ar),

																  new JProperty("SubItemImage", subItems.OSubItemImage),
																  new JProperty("OSubItemQuantity", subItems.OSubItemQuantity),
																  new JProperty("Size", subItems.OSubItemSize),
																  new JProperty("Comments", subItems.OSubItemComments),
																  new JProperty("Price", subItems.OSubItemPrice)
															  );
									jsubItem.Add(jaitems);


								}
							}

							//  JProperty jp = new JProperty(oItems.Id.ToString(),
							JArray jr = new JArray(new JObject(
											new JProperty("ItemId", oItems.ItemId),
											new JProperty("ItemName", oItems.ItemName),
											new JProperty("ItemName_Ar", oItems.ItemName_Ar),
											 new JProperty("Description", oItems.ItemDesc),
											new JProperty("Description_Ar", oItems.ItemDesc_Ar),
											 new JProperty("Comments", oItems.Comments),
											 new JProperty("Images", ""),//oItems.Images),
											new JProperty("ItemPrice", oItems.ItemPrice),
											new JProperty("Qty", oItems.Qty),
											new JProperty("CategoryId", oItems.CategoryId),
											 new JProperty("SubCatId", oItems.SubCatId),
											new JProperty("SizeId", oItems.Size),
											  new JProperty("Size", oItems.SizeName),
											   new JProperty("SizeName_Ar", oItems.SizeName_Ar),
											  new JProperty("ItemType", oItems.ItemType),
											new JProperty("SubSubItems", jsubItem)));

							//new JArray(from odrAdd in res
							//           where odrAdd.Id == oItems.Id && odrAdd.AdditionalID != null
							//           select new JObject(
							//               new JProperty("AdditionalID", odrAdd.AdditionalID),
							//               new JProperty("AdditionalPrice", odrAdd.AdditionalPrice),
							//              new JProperty("AdditionalName", odrAdd.AdditionalName),
							//               new JProperty("AdditionalName_Ar", odrAdd.AdditionalName_Ar))));

							Jlist.Add(jr);
						}
					}

					obj.Property("InvoiceNo").AddAfterSelf(new JProperty("TotalItems", TotalItems));


					JorderInfo = new JArray(obj);



					jobj = new JObject(new JProperty("MainItem", JorderInfo),
										new JProperty("SubItem", Jlist));
				}

				return jobj.Count != 0 ? jobj : new JObject(new JProperty("Failure", "No Record Found"));
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public JObject CancelOrder(JObject Json)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "InvalidOrder"));

			IEnumerable<JToken> MainItem = Json.SelectToken("CancelOrder");
			try
			{
				foreach (JToken mItem in MainItem)
				{
					OrderInfo ord = JsonConvert.DeserializeObject<OrderInfo>(mItem.ToString());

					var res = JArray.Parse(JsonConvert.SerializeObject(db.SP_CancelOrder(ord.OrderId, ord.UserId)));
					Jobj = new JObject(new JProperty("Success", res));
					//SendPushNotification.SendNotification(res.ToString(), ord.Device_token, Convert.ToInt32(ord.UserId));
					// Common.InsertPushMsg(Convert.ToInt32(ord.UserId), res.ToString(), ord.Device_token, (int)Common.NotificationType.Rejecet);

				}
				return Jobj;
			}
			catch
			{
				return Jobj;

			}

		}

		public JObject CustomerPnList(JObject Json)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "InvalidUser"));

			IEnumerable<JToken> MainItem = Json.SelectToken("PnList");
			try
			{
				foreach (JToken mItem in MainItem)
				{
					OrderInfo ord = JsonConvert.DeserializeObject<OrderInfo>(mItem.ToString());

					var res = JArray.Parse(JsonConvert.SerializeObject(db.SP_Get_PushNotificationMessages(ord.UserId, ord.StoreId)));
					Jobj = new JObject(new JProperty("Success", res));
					//SendPushNotification.SendNotification(res.ToString(), ord.Device_token, Convert.ToInt32(ord.UserId));
					// Common.InsertPushMsg(Convert.ToInt32(ord.UserId), res.ToString(), ord.Device_token, (int)Common.NotificationType.Rejecet);

				}
				return Jobj;
			}
			catch
			{
				return Jobj;

			}

		}

		public JObject UpdatePnlist(JObject Json)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "PleaseTry"));

			IEnumerable<JToken> MainItem = Json.SelectToken("UpdatePnList");
			try
			{
				foreach (JToken mItem in MainItem)
				{
					OrderInfo ord = JsonConvert.DeserializeObject<OrderInfo>(mItem.ToString());

					try
					{
						var res = JArray.Parse(JsonConvert.SerializeObject(db.SP_Delete_PushNotificationMsgCustomer(ord.UserId, ord.StoreId)));

						string Message = Convert.ToString(res);
						Message = Message.Substring(6);
						Message = Message.Substring(0, 1);
						string txtMessga = "Please try again.!";
						string txtMessgaAr = "Please try again Ar.!";
						bool status = false;
						if (Message == "1")
						{
							txtMessga = "Successfull readed pushnotification";
							txtMessgaAr = "Successfull readed pushnotification Ar";
							status = true;
						}
						else if (Message == "2")
						{
							txtMessga = "Successfull deleted pushnotification";
							txtMessgaAr = "Successfull deleted pushnotification Ar";
							status = true;
						}
						else
						{
							txtMessga = "please try again later.!";
							txtMessgaAr = "please try again later Ar.!";
							status = false;
						}
						Jobj = new JObject(new JProperty("Status", status),
											 (new JProperty("Message", txtMessga)),
											 (new JProperty("MessageAr", txtMessgaAr)),
											 (new JProperty("Data", "[]")));

					}
					catch (Exception Ex)
					{

					}

					//Jobj = new JObject(new JProperty("Success", res));

				}
				return Jobj;
			}
			catch
			{
				return Jobj;

			}

		}

		public JArray DeleteFavoriteOrder(int OrderId)
		{
			JArray jar = new JArray(new JValue(false));
			try
			{

				OrderInfo _orderInfo = db.OrderInfoes.Single(o => o.OrderId == OrderId);
				_orderInfo.IsFavorite = false;
				//  _orderInfo.FavoriteName = FavoriteName;
				db.Entry(_orderInfo).State = System.Data.Entity.EntityState.Modified;
				db.SaveChanges();
				jar = new JArray(new JValue(true));
				return jar;
			}
			catch
			{
				return jar;
			}
		}

		public JObject DeleteOrder(int OrderId)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "InvalidOrder"));
			try
			{
				OrderInfo orderinfo = db.OrderInfoes.Single(odr => odr.OrderId == OrderId);
				orderinfo.status = false;
				db.Entry(orderinfo).State = System.Data.Entity.EntityState.Modified;
				db.SaveChanges();
				Jobj = new JObject(new JProperty("Success", "Deleted "));
				return Jobj;

			}
			catch
			{
				return Jobj;

			}
		}

		public JObject GetFavouriteOrder(int UserId)
		{
			List<JObject> obj = new List<JObject>();
			JObject JobjItems;
			JObject Jobj = new JObject(new JProperty("Failure", false));

			var res = (from oItems in db.OrderItems
					   join oInfo in db.OrderInfoes
					   on oItems.OrderId equals oInfo.OrderId
					   join subCat in db.ItemsDetails
					   on oItems.ItemId equals subCat.ItemId
					   join oAddress in db.UserAddressDetails on oInfo.AddressID equals oAddress.Id into UserAddress
					   from usrAddress in UserAddress.DefaultIfEmpty()
					   join o in db.StoreInformations
					   on oInfo.StoreId equals o.StoreId
					   where oInfo.UserId == UserId && oInfo.IsFavorite == true && oInfo.status == true
					   group new { oItems, oInfo, subCat, usrAddress, o }
					  by new
					  {
						  oInfo.OrderId,
						  oInfo.FavoriteName,
						  oInfo.StoreId,
						  oInfo.OrderType,
						  oInfo.IsFavorite,
						  o.StoreName,
						  o.StoreName_ar,
						  oInfo.Total_Price,
						  subCat.ItemName,
						  subCat.ItemName_Ar,
						  oInfo.OrderDate,
						  usrAddress.Address
					  } into odrItems
					   select new
					   {
						   odrId = odrItems.Key.OrderId,
						   odrType = odrItems.Key.OrderType,
						   Fname = odrItems.Key.FavoriteName,
						   Sname = odrItems.Key.StoreName,
						   Sname_ar = odrItems.Key.StoreName_ar,
						   ItmName = odrItems.Count() + " " + odrItems.Key.ItemName,
						   ItmName_ar = odrItems.Count() + " " + odrItems.Key.ItemName_Ar,
						   TotPrice = odrItems.Key.Total_Price,
						   UserAddress = (odrItems.Key.Address == null ? string.Empty : odrItems.Key.Address),
						   Odate = odrItems.Key.OrderDate
					   }).OrderByDescending(odate => odate.Odate).ToList();
			int oInfoId = 0;
			foreach (var favOrder in res.AsEnumerable())
			{
				if (oInfoId != favOrder.odrId)
				{

					oInfoId = favOrder.odrId;
					JobjItems = new JObject(
												new JProperty("odrId", favOrder.odrId),
											   new JProperty("Fname", favOrder.Fname),
											  new JProperty("Sname", favOrder.Sname),
											  new JProperty("OrderType", favOrder.odrType),
											   new JProperty("Sname_ar", favOrder.Sname_ar),
											   new JProperty("UserAddress", favOrder.UserAddress),
												new JProperty("TotPrice", favOrder.TotPrice),
											   new JProperty("Odate", String.Format("{0:g}", favOrder.Odate)),
									   new JProperty("items", from fOrder in res
															  where fOrder.odrId == oInfoId
															  select new JObject(
																  new JProperty("ItmName", fOrder.ItmName),
																  new JProperty("ItmName_ar", fOrder.ItmName_ar))



					));


					obj.Add(JobjItems);

				}
			}
			//return Jobj = new JObject(new JProperty("Success", obj));
			return obj.Count > 0 ? Jobj = new JObject(new JProperty("Success", obj)) : Jobj;
		}

		public JObject InsertFavorieOrder(int orderID, string FavoriteName)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "Sorry You Don't Have any Favorite Order"));
			try
			{

				OrderInfo _orderInfo = db.OrderInfoes.Single(o => o.OrderId == orderID);
				_orderInfo.IsFavorite = true;
				_orderInfo.FavoriteName = FavoriteName;
				db.Entry(_orderInfo).State = System.Data.Entity.EntityState.Modified;
				db.SaveChanges();
				Jobj = new JObject(new JProperty("Success", "Updated"));
				return Jobj;
			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Failure", ex.Message));
			}
		}


		public JObject GetOrderLocation(int orderid)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "Order InProgress"));
			try
			{
				var odrLoc = (from oLoc in db.OrderAssignDetails
							  join sInfo in db.StoreInformations on oLoc.StoreId equals sInfo.StoreId
							  join sWD in db.StoreWorkDatas on oLoc.StoreId equals sWD.StoreId
							  join dLoc in db.DriverLocations on oLoc.DriverId equals dLoc.DriverId
							  join oInfo in db.OrderInfoes on oLoc.OrderId equals oInfo.OrderId
							  join oAddress in db.UserAddressDetails on oInfo.AddressID equals oAddress.Id
							  join reg in db.Registrations on oLoc.DriverId equals reg.UserId
							  where oLoc.OrderId == orderid
							  select new
							  {
								  oLoc.DriverId,
								  sLatitude = sInfo.Latitude,
								  sLongitude = sInfo.Longitude,
								  sname = sInfo.StoreName,
								  sname_ar = sInfo.StoreName_ar,
								  sInfo.StoreAddress,
								  sInfo.StoreAddress_ar,
								  sPhone = sWD.phone,
								  dLatitude = dLoc.Latitude,
								  dLongitude = dLoc.Longitude,
								  dName = reg.FullName,
								  dPhone = reg.Mobile,
								  uLatitude = oAddress.Latitude,
								  uLongitude = oAddress.Longitude,
								  uaddress = oAddress.Address

							  }).ToList();


				return odrLoc.Count > 0 ? Jobj = new JObject(new JProperty("Success", JProperty.Parse(JsonConvert.SerializeObject(odrLoc)))) : Jobj;

				//return Jobj;

			}
			catch
			{
				return Jobj;

			}

		}

		public JObject InsertCorporateRequest(JObject Json)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "Error Occured"));
			try
			{

				using (TransactionScope objTran = new TransactionScope())
				{


					IEnumerable<JToken> CorporateRequest = Json.SelectToken("CorporateRequest");
					//foreach (JToken cReq in CorporateRequest)
					//{
					CorporateClient CCreq = JsonConvert.DeserializeObject<CorporateClient>(CorporateRequest.ToString());
					CCreq.CreatedDate = Common.KSA_DateTime();
					CCreq.ReqApproved = false;
					CCreq.Status = false;
					CCreq.Comment = "";

					//  db.Entry(CCreq).State = System.Data.Entity.EntityState.Added;
					db.CorporateClients.Add(CCreq);
					db.SaveChanges();
					objTran.Complete();
					// }
					return new JObject(new JProperty("Success", "Request Submitted..."));
				}
			}
			catch (Exception ex)
			{
				return new JObject(new JProperty("Failure", ex.Message));
			}
		}


		private string getInvoiceNo(string storeId)
		{
			int rno = Common.getRandomNumber();
			TimeZoneInfo AST = TimeZoneInfo.FindSystemTimeZoneById("Arabic Standard Time");
			DateTime astTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, AST);
			string dt = astTime.ToString("ddMMyy");
			string invoiceNo = storeId + dt + "-" + rno;
			//string orderNo = "45020216-5349";

			int countOrderNo = (from order in db.OrderInfoes
								where order.InvoiceNo == invoiceNo
								select order.OrderId).Count();
			if (countOrderNo > 0)
			{
				invoiceNo = getInvoiceNo(storeId);
				return invoiceNo;
			}
			else
			{
				return invoiceNo;
			}
		}

		private int getRandomNumber()
		{
			Random r = new Random();
			return r.Next(1000, 9999);
		}

		#region Dispose Object

		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					db.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion


		public JObject SaveUsrRating(JObject JsonUsrRating)
		{
			try
			{
				string usrRating = JsonUsrRating.SelectToken("usrRating").ToString();
				List<UserRating> _rating = JsonConvert.DeserializeObject<List<UserRating>>(usrRating);

				foreach (var item in _rating)
				{
					item.CreatedDate = Common.KSA_DateTime();
					item.IsActive = true;
					db.UserRatings.Add(item);
					db.SaveChanges();
				}


				return new JObject(new JProperty("Success", "Thanks For Rating"));

			}


			catch (Exception ex)
			{
				return new JObject(new JProperty("Failure", ex.Message));
			}
		}

		public JObject OrderRating(JObject OrderRating)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "PleaseTry"));

			try
			{
				IEnumerable<JToken> MainOrder = OrderRating.SelectToken("OrderRating");

				foreach (JToken mOrder in MainOrder)
				{
					OrderRating ord = JsonConvert.DeserializeObject<OrderRating>(mOrder.ToString());

					try
					{
						var res = JArray.Parse(JsonConvert.SerializeObject(db.Save_OrderRating(ord.OrderId, ord.OrderRating1, ord.OrderComment, ord.DriverRating, ord.DriverComment)));

						string Message = Convert.ToString(res);
						Message = Message.Substring(6);
						Message = Message.Substring(0, 1);
						string txtMessga = "Please try again.!";
						string txtMessgaAr = "Please try again Ar.!";
						bool status = false;
						if (Message == "1")
						{
							txtMessga = "Order rating inserted successfull.";
							txtMessgaAr = "order rating inserted successfull Ar.";
							status = true;
						}
						else
						{
							txtMessga = "failed, Please try again.!";
							txtMessgaAr = "Failed, please try again Ar.!";
							status = false;
						}
						Jobj = new JObject(new JProperty("Status", status),
											 (new JProperty("Message", txtMessga)),
											 (new JProperty("MessageAr", txtMessgaAr)),
											 (new JProperty("Data", "[]")));

					}
					catch (Exception Ex)
					{

					}
				}
				return Jobj;
			}
			catch
			{
				return Jobj = new JObject(new JProperty("Status", false),
											 (new JProperty("Message", "failed, Please try again.!")),
											 (new JProperty("MessageAr", "Failed, please try again Ar.! ")),
											 (new JProperty("Data", "[]")));
			}
		}

		public JObject GetOrderRatingDetails(int UserId, int OrderId)
		{

			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));

			try
			{

				if (OrderId == -1)
				{

					OrderId = (from x in db.OrderInfoes where x.UserId == UserId && x.status == true select x).OrderByDescending(x => x.OrderId).FirstOrDefault().OrderId;

				}

				string OrderStatus = string.Empty;
				int? UserRatings = 0;
				var Result = (from ordInfo in db.OrderInfoes


							  from rat in db.UserRatings.Where(ra => ra.UserId == UserId && ra.OrderId == OrderId).DefaultIfEmpty()
							  where ordInfo.UserId == UserId && ordInfo.OrderId == OrderId && ordInfo.status == true

							  select new
							  {
								  ordInfo.UserId,
								  ordInfo.OrderId,
								  ordInfo.OrderStatus,
								  RatingId = rat.RatingId == null ? 0 : rat.RatingId,
								  Ratings = rat.Ratings == null ? 0 : rat.Ratings,
								  ordInfo.OrderType
							  }).FirstOrDefault();

				JArray ratArr = new JArray();

				if (Result != null)
				{
					OrderStatus = Result.OrderStatus;
					UserRatings = Result.Ratings;
					var rating = (from rat in db.Ratings where rat.OrderType == Result.OrderType && rat.IsActive == true select new { Rating = rat.Rating1 }).Distinct().OrderByDescending(x => x.Rating).ToList();

					JObject Jobjrating = new JObject();

					foreach (var item in rating)
					{
						JObject Other = new JObject(new JProperty("RatingId", 0), new JProperty("Comment_En", "Other"), new JProperty("Comment_Ar", "الآخرين"), new JProperty("Rating", item.Rating));

						var rate = (from ra in db.Ratings where ra.Rating1 == item.Rating && ra.OrderType == Result.OrderType select new { ra.RatingId, ra.Comment_Ar, ra.Comment_En, ra.OrderType, Rating = ra.Rating1 }).Distinct().OrderBy(x => x.Rating).ToList();

						JArray jaRate = new JArray(JArray.Parse(JsonConvert.SerializeObject(rate)));
						jaRate.Add(Other);
						JProperty RJobject = new JProperty(item.Rating.ToString(), jaRate);
						Jobjrating.Add(RJobject);

					}

					ratArr.Add(Jobjrating);
				}


				var banner = (from ban in db.Banners where ban.Status == true select ban).ToList();

				JArray JBanner = new JArray(JArray.Parse(JsonConvert.SerializeObject(banner)));

				JObject JobjectRes = new JObject(new JProperty("Banner", JBanner),

													 new JProperty("Ratings", ratArr),
					new JProperty("Userid", UserId), new JProperty("OrderId", OrderId),
					new JProperty("OrderStatus", OrderStatus),
					new JProperty("UserRating", UserRatings));

				return JobjectRes;

			}

			catch (Exception)
			{

				return Jobj;
			}


		}


		//Temporary created order
		//public JObject InsertordDetail(JObject Json,string str)
		//{
		//	// Json = JsonConvert.SerializeObject(Json);
		//	JObject Jobj = new JObject(new JProperty("Failure", "Error Occured"));
		//	try
		//	{

		//		using (TransactionScope objTran = new TransactionScope())
		//		{

		//			JObject jobj = Json;// JObject.Parse(Json);

		//			IEnumerable<JToken> MainItem = jobj.SelectToken("MainItem");

		//			IEnumerable<JToken> SubItem = jobj.SelectToken("SubItem");

		//			IEnumerable<JToken> Promotion = jobj.SelectToken("Promotion");

		//			//OrderAdditional OrAdditonals = new OrderAdditional();
		//			int _orderId = 0;
		//			int _itemId = 0;
		//			int? _userid = 0;
		//			string InvoiceNo = string.Empty;
		//			// 
		//			foreach (JToken mItem in MainItem)
		//			{
		//				OrderInfo ord = JsonConvert.DeserializeObject<OrderInfo>(mItem.ToString());

		//				InvoiceNo = getInvoiceNo(ord.StoreId.ToString());
		//				ord.InvoiceNo = InvoiceNo;
		//				ord.status = true;
		//				ord.OrderDate = Common.KSA_DateTime();//DateTime.Now;//Convert.ToDateTime(Common.getCurrentTime());

		//				#region VatImplementation
		//				if (ord.PaymentMode == 2)
		//				{
		//					ord.SubTotal = ord.SubTotal == null ? ord.Total_Price : ord.SubTotal;
		//					ord.VatPercentage = (from vat in db.VATDetails where vat.IsActive == true select new { vat.ID, vat.VatPercentage }).OrderByDescending(x => x.ID).FirstOrDefault().VatPercentage.ToString();
		//					if (ord.SubTotal == null)
		//					{
		//						string vatC = (Convert.ToDouble(ord.SubTotal) * (Convert.ToDouble(ord.VatPercentage)) / 100).ToString();
		//						var totalCost = Convert.ToDouble(String.Format("{0:0.00}", vatC));
		//						ord.VatCharges = totalCost.ToString();
		//					}
		//					//ord.VatCharges = (Convert.ToDouble(ord.SubTotal) * (Convert.ToDouble(ord.VatPercentage)) / 100).ToString();

		//					ord.Total_Price = ord.SubTotal == null ? (Convert.ToDouble(ord.SubTotal) + Convert.ToDouble(ord.VatCharges)).ToString() : ord.Total_Price;
		//				}
		//				if (ord.PaymentMode == 3)
		//				{
		//					ord.VatCharges = ord.SubTotal == null ? "0" : ord.VatCharges;
		//					ord.SubTotal = ord.SubTotal == null ? ord.Total_Price : ord.SubTotal;

		//					ord.VatPercentage = (from vat in db.VATDetails where vat.IsActive == true select new { vat.ID, vat.VatPercentage }).OrderByDescending(x => x.ID).FirstOrDefault().VatPercentage.ToString();
		//					ord.Total_Price = (Convert.ToDouble(ord.SubTotal) + Convert.ToDouble(ord.VatCharges)).ToString();
		//				}
		//				if (ord.PaymentMode == 4)
		//				{
		//					ord.SubTotal = ord.SubTotal == null ? ord.Total_Price : ord.SubTotal;
		//					ord.VatCharges = "0";
		//					ord.VatPercentage = (from vat in db.VATDetails where vat.IsActive == true select new { vat.ID, vat.VatPercentage }).OrderByDescending(x => x.ID).FirstOrDefault().VatPercentage.ToString();
		//					ord.Total_Price = (Convert.ToDouble(ord.SubTotal) + Convert.ToDouble(ord.VatCharges)).ToString();
		//				}

		//				#endregion

		//				db.OrderInfoes.Add(ord);
		//				db.SaveChanges();
		//				_orderId = ord.OrderId;
		//				_userid = ord.UserId;

		//				//insert into TrackOrder table

		//				OrderTracking oTrack = new OrderTracking();
		//				oTrack.OrderId = ord.OrderId;
		//				oTrack.OrderStatus = "Received";
		//				oTrack.TrackingTime = ord.OrderDate;
		//				db.OrderTrackings.Add(oTrack);
		//				db.SaveChanges();

		//				Registration reg = db.Registrations.Single(usr => usr.UserId == ord.UserId);
		//				reg.DeviceToken = ord.Device_token;
		//				db.SaveChanges();
		//			}

		//			if (null != Promotion)
		//			{
		//				foreach (JToken pDetails in Promotion)
		//				{
		//					userPromotion obj = JsonConvert.DeserializeObject<userPromotion>(pDetails.ToString());
		//					obj.userId = _userid;
		//					obj.orderId = _orderId;
		//					obj.usedDate = Common.KSA_DateTime();//DateTime.Now;
		//					db.userPromotions.Add(obj);
		//					db.SaveChanges();

		//				}
		//			}

		//			foreach (JToken sItem in SubItem)
		//			{
		//				// bool additional = false;
		//				JToken Jitems = null;
		//				// JToken Jadditionals = null;
		//				//foreach (JToken subItems in sItem)
		//				//{
		//				//    if (!additional)
		//				//    {
		//				//Jitems = subItems;
		//				Jitems = sItem;
		//				//additional = true;
		//				//}
		//				//else
		//				//{
		//				//    Jadditionals = subItems;
		//				//}
		//				//}
		//				if (null != Jitems)
		//				{
		//					//var itemProperties = Jitems.Children<JProperty>().ToList();
		//					IEnumerable<JToken> SubsubItem = Jitems.SelectToken("SubSubItems");
		//					OrderItem OrItem = JsonConvert.DeserializeObject<OrderItem>(Jitems.ToString());
		//					OrItem.OrderId = _orderId;
		//					db.Entry(OrItem).State = System.Data.Entity.EntityState.Added;
		//					db.SaveChanges();
		//					_itemId = OrItem.Id;

		//					if (SubsubItem != null)
		//					{
		//						foreach (JToken ssitems in SubsubItem)
		//						{
		//							OrderSubItem OrsubItem = JsonConvert.DeserializeObject<OrderSubItem>(ssitems.ToString());
		//							OrsubItem.OrderItemId = _itemId;
		//							db.Entry(OrsubItem).State = System.Data.Entity.EntityState.Added;
		//							db.SaveChanges();
		//						}
		//					}

		//				}

		//			}
		//			// db.SaveChanges();
		//			objTran.Complete();
		//			Jobj = new JObject(new JProperty("Success", _orderId + "," + InvoiceNo));
		//			return Jobj;
		//		}
		//	}
		//	catch (Exception ex)
		//	{

		//		Jobj = new JObject(new JProperty("Failure", ex.Message));
		//		return Jobj;
		//	}

		//}

		public bool SaveUserDevicetoken(string deviceToken, string source, int sourceId)
		{
			List<UserDeviceToken> lstUserToken = db.UserDeviceTokens.Where(i => i.DeviceToken == deviceToken).ToList();
			if (lstUserToken.Count == 0)
			{
				try
				{
					UserDeviceToken newToken = new UserDeviceToken();
					newToken.DeviceToken = deviceToken;
					newToken.Source = source;
					newToken.SourceId = sourceId;

					db.UserDeviceTokens.Add(newToken);
					db.SaveChanges();
					return true;
				}
				catch (Exception Ex)
				{
					return false;
				}

			}
			return true;
		}

		public JObject SaveSpecialImages(JObject JsonData)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "PleaseTry"));

			try
			{
				JObject jobj = JsonData;// JObject.Parse(Json);
				string RefName = "";
				string RefImg64 = "";
				string ImgOnCakeNm = "";
				string ImgOnCake64 = "";
				string RefReName = "";
				string ImgOnReName = "";
				bool status = false;
				string MessageEn = "";
				string MessageAr = "";
				string BaseURL = ConfigurationManager.AppSettings["BaseURL"];


				IEnumerable<JToken> ReferanceImage = jobj.SelectToken("ReferanceImage");
				IEnumerable<JToken> ImageOnCake = jobj.SelectToken("ImageOnCake");

				JObject RefImg = (JObject)JToken.FromObject(ReferanceImage);
				JObject ImgCake = (JObject)JToken.FromObject(ImageOnCake);

				//Referance image details
				RefName = Convert.ToString(RefImg.SelectToken("RefImgName"));
				RefImg64 = Convert.ToString(RefImg.SelectToken("RefBase64"));

				if (RefName != null && RefImg64 != "")
				{
					bool IsSuccess = SaveImage(RefName, RefImg64);
					if (IsSuccess)
					{
						RefReName = BaseURL + RefName;  // local system URL.
														//RefReName = " http://csadms.com/bncservices" + RefName;  // CSADMS.COM URL.
														//RefReName = "http://localhost:6608/Uploads/CakeImages" + RefName;  // LIVE URL.
						status = true;
					}
					else
					{
						RefReName = "";
					}
				}
				else
				{
					RefReName = "";
				}

				//Cake On Image Details
				ImgOnCakeNm = Convert.ToString(ImgCake.SelectToken("ImageOnName"));
				ImgOnCake64 = Convert.ToString(ImgCake.SelectToken("ImageOnBase64"));

				if (ImgOnCakeNm != null && ImgOnCake64 != "")
				{
					bool IsSuccess = SaveImage(ImgOnCakeNm, ImgOnCake64);
					if (IsSuccess)
					{
						ImgOnReName = BaseURL + ImgOnCakeNm;  // local system URL.
															  //ImgOnReName = " http://csadms.com/bncservices" + RefName;  // CSADMS.COM URL.
															  //ImgOnReName = "http://localhost:6608/Uploads/CakeImages" + RefName;  // LIVE URL.
						status = true;
					}
					else
					{
						ImgOnReName = "";
					}
				}
				else
				{
					ImgOnReName = "";
				}


				if (status)
				{
					MessageEn = "image save successfull.";
					MessageAr = "image save successfull Ar.";
				}
				else
				{
					MessageEn = "Failed to save image";
					MessageAr = "failed to save image Ar";
				}
				JObject res1 = new JObject(new JProperty("Referance", RefReName),
											new JProperty("ImageOnCake", ImgOnReName));

				JObject res = new JObject(new JProperty("Status", status),
										 (new JProperty("Message", MessageEn)),
										 (new JProperty("MessageAr", MessageAr)),
										 (new JProperty("Data", res1)));
				return res;
			}
			catch (Exception ex)
			{
				return Jobj = new JObject(new JProperty("Status", false),
											 (new JProperty("Message", "failed, Please try again.!")),
											 (new JProperty("MessageAr", "Failed, please try again Ar.! ")),
											 (new JProperty("Data", "[]")));
			}
		}

		public bool SaveImage(string ImgName, string ImgStr)
		{
			String path = HttpContext.Current.Server.MapPath("~/Uploads/CakeImages"); //Path
																					  //Check if directory exist
			if (!System.IO.Directory.Exists(path))
			{
				System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
			}
			//string imageName = ImgName + ".jpg";
			string imageName = ImgName;

			//set the image path
			string imgPath = Path.Combine(path, imageName);
			byte[] imageBytes = Convert.FromBase64String(ImgStr);
			File.WriteAllBytes(imgPath, imageBytes);

			return true;
		}
	}
}
