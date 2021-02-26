using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeryCo.DataModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Transactions;
using System.Net.Mail;
using System.Security.Cryptography;
using System.IO;
using BakeryCo.Repositary.ws.jawalbsms.dll;
namespace BakeryCo.Repositary
{
	public class RegistrationRepository
	{
		BakeryCoEntities db = new BakeryCoEntities();

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

		public JObject UserRegistration(JObject Json)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "Mobile Already Registered "));
			try
			{

				using (TransactionScope objTran = new TransactionScope())
				{

					JObject jobj = Json;// JObject.Parse(Json);

					IEnumerable<JToken> UsrDetails = jobj.SelectToken("UserDetails");


					foreach (JToken usr in UsrDetails)
					{
						Registration reg = JsonConvert.DeserializeObject<Registration>(usr.ToString());

						var validate = db.Registrations.Where(isvalid => isvalid.Mobile == reg.Mobile && isvalid.IsVerified == true).Select(usrId => new { usrId.UserId }).FirstOrDefault();

						if (validate == null)
						{
							int _userId;
							var _existingUser = db.Registrations.Where(isvalid => isvalid.Mobile == reg.Mobile && isvalid.IsVerified == false).FirstOrDefault();

							if (_existingUser == null)
							{
								reg.City = string.Empty;
								reg.CountryId = 0;
								reg.CreationDate = Common.KSA_DateTime();
								reg.FullAddress = string.Empty;
								reg.IsBlocked = false;
								reg.Phone = string.Empty;
								reg.Status = true;
								reg.RandomNumber = Common.getRandomNumber();
								reg.IsVerified = false;
								db.Registrations.Add(reg);
								db.SaveChanges();
								if (reg.UserId != 0)
								{
									#region SMS
									SendOTP(reg.UserId, reg.Mobile, reg.RandomNumber);
									#endregion
									Jobj = new JObject(new JProperty("Success", new JArray(new JObject(
													 new JProperty("UserId", reg.UserId),
													new JProperty("FullName", reg.FullName),
													new JProperty("Email", reg.Email),
													 new JProperty("Mobile", reg.Mobile),
													  new JProperty("IsVerified", reg.IsVerified),
													   new JProperty("Language", reg.Language),
														new JProperty("FamilyName", reg.FamilyName),
													  new JProperty("NickName", reg.NickName),
													   new JProperty("Gender", reg.Gender),
													   new JProperty("IsDriver", reg.IsDriver),
													   new JProperty("Password", reg.Password)))));
								}
								var res = JArray.Parse(JsonConvert.SerializeObject(db.GenerateNewOrderPN(null, reg.UserId, 4)));

							}
							else
							{
								_userId = _existingUser.UserId;
								_existingUser.FullName = reg.FullName;
								_existingUser.Email = reg.Email;
								_existingUser.Language = reg.Language;
								_existingUser.FamilyName = reg.FamilyName;
								_existingUser.NickName = reg.NickName;
								_existingUser.Gender = reg.Gender;
								_existingUser.Password = reg.Password;
								_existingUser.RandomNumber = Common.getRandomNumber();
								db.SaveChanges();
								if (_existingUser.UserId != 0)
								{
									#region SMS
									SendOTP(_existingUser.UserId, _existingUser.Mobile, _existingUser.RandomNumber);
									#endregion
									Jobj = new JObject(new JProperty("Success", new JArray(new JObject(
													 new JProperty("UserId", _existingUser.UserId),
													new JProperty("FullName", _existingUser.FullName),
													new JProperty("Email", _existingUser.Email),
													 new JProperty("Mobile", _existingUser.Mobile),
													  new JProperty("IsVerified", _existingUser.IsVerified),
													   new JProperty("Language", _existingUser.Language),
														new JProperty("FamilyName", _existingUser.FamilyName),
													  new JProperty("NickName", _existingUser.NickName),
													   new JProperty("Gender", _existingUser.Gender),
													   new JProperty("IsDriver", _existingUser.IsDriver),
													   new JProperty("Password", _existingUser.Password)))));
								}
								var res = JArray.Parse(JsonConvert.SerializeObject(db.GenerateNewOrderPN(null, _existingUser.UserId, 4)));
							}
						}
						objTran.Complete();
					}

					#region old code
					//foreach (JToken usr in UsrDetails)
					//{
					//	Registration reg = JsonConvert.DeserializeObject<Registration>(usr.ToString());

					//	var validate = db.Registrations.Where(isvalid => isvalid.Mobile == reg.Mobile && isvalid.IsVerified == true).Select(usrId => new { usrId.UserId }).FirstOrDefault();
					//	if (validate == null)
					//	{
					//		//  reg.FullName = Name;
					//		reg.City = string.Empty;
					//		reg.CountryId = 0;
					//		reg.CreationDate = Common.KSA_DateTime();// DateTime.Now;//Convert.ToDateTime(Common.getCurrentTime());
					//												 //  reg.DeviceType = string.Empty;
					//												 //reg.FamilyName = string.Empty;
					//		reg.FullAddress = string.Empty;
					//		// reg.Gender = string.Empty;
					//		reg.IsBlocked = false;
					//		// reg.NickName = string.Empty;
					//		reg.Phone = string.Empty;
					//		reg.Status = true;
					//		//  reg.Email = Email;
					//		// reg.Password = Psw;
					//		// reg.DeviceToken = DToken;
					//		// reg.Mobile = Mobile;
					//		// reg.Language = Lang;
					//		reg.RandomNumber = Common.getRandomNumber();
					//		reg.IsVerified = false;
					//		db.Registrations.Add(reg);
					//		db.SaveChanges();

					//		if (reg.UserId != 0)
					//		{
					//			#region SMS

					//			SendOTP(reg.UserId, reg.Mobile, reg.RandomNumber);

					//			#endregion


					//			Jobj = new JObject(new JProperty("Success", new JArray(new JObject(
					//							 new JProperty("UserId", reg.UserId),
					//							new JProperty("FullName", reg.FullName),
					//							new JProperty("Email", reg.Email),
					//							 new JProperty("Mobile", reg.Mobile),
					//							  new JProperty("IsVerified", reg.IsVerified),
					//							   new JProperty("Language", reg.Language),
					//								new JProperty("FamilyName", reg.FamilyName),
					//							  new JProperty("NickName", reg.NickName),
					//							   new JProperty("Gender", reg.Gender),
					//							   new JProperty("IsDriver", reg.IsDriver),
					//							   new JProperty("Password", reg.Password)))));



					//		}

					//		var res = JArray.Parse(JsonConvert.SerializeObject(db.GenerateNewOrderPN(null, reg.UserId, 4)));

					//	}



					//	objTran.Complete();




					//}
					#endregion
				}
				return Jobj;
			}
			catch (Exception ex)
			{


				return Jobj;
			}

		}


		// old user registraion servcie code commented.
		//public JObject UserRegistration(JObject Json)
		//{
		//	JObject Jobj = new JObject(new JProperty("Failure", "Mobile Already Registered "));
		//	try
		//	{

		//		using (TransactionScope objTran = new TransactionScope())
		//		{

		//			JObject jobj = Json;// JObject.Parse(Json);

		//			IEnumerable<JToken> UsrDetails = jobj.SelectToken("UserDetails");
		//			foreach (JToken usr in UsrDetails)
		//			{
		//				Registration reg = JsonConvert.DeserializeObject<Registration>(usr.ToString());

		//				var validate = db.Registrations.Where(isvalid => isvalid.Mobile == reg.Mobile && isvalid.IsVerified == true).Select(usrId => new { usrId.UserId }).FirstOrDefault();
		//				if (validate == null)
		//				{
		//					//  reg.FullName = Name;
		//					reg.City = string.Empty;
		//					reg.CountryId = 0;
		//					reg.CreationDate = Common.KSA_DateTime();// DateTime.Now;//Convert.ToDateTime(Common.getCurrentTime());
		//															 //  reg.DeviceType = string.Empty;
		//															 //reg.FamilyName = string.Empty;
		//					reg.FullAddress = string.Empty;
		//					// reg.Gender = string.Empty;
		//					reg.IsBlocked = false;
		//					// reg.NickName = string.Empty;
		//					reg.Phone = string.Empty;
		//					reg.Status = true;
		//					//  reg.Email = Email;
		//					// reg.Password = Psw;
		//					// reg.DeviceToken = DToken;
		//					// reg.Mobile = Mobile;
		//					// reg.Language = Lang;
		//					reg.RandomNumber = Common.getRandomNumber();
		//					reg.IsVerified = false;
		//					db.Registrations.Add(reg);
		//					db.SaveChanges();


		//					SaveUserDevicetoken(reg.DeviceToken, "Regd", reg.UserId);

		//					if (reg.UserId != 0)
		//					{
		//						#region SMS

		//						SendOTP(reg.UserId, reg.Mobile, reg.RandomNumber);

		//						#endregion


		//						Jobj = new JObject(new JProperty("Success", new JArray(new JObject(
		//										 new JProperty("UserId", reg.UserId),
		//										new JProperty("FullName", reg.FullName),
		//										new JProperty("Email", reg.Email),
		//										 new JProperty("Mobile", reg.Mobile),
		//										  new JProperty("IsVerified", reg.IsVerified),
		//										   new JProperty("Language", reg.Language),
		//											new JProperty("FamilyName", reg.FamilyName),
		//										  new JProperty("NickName", reg.NickName),
		//										   new JProperty("Gender", reg.Gender),
		//										   new JProperty("IsDriver", reg.IsDriver),
		//										   new JProperty("Password", reg.Password)))));



		//					}

		//					var res = JArray.Parse(JsonConvert.SerializeObject(db.GenerateNewOrderPN(null, reg.UserId, 4)));

		//				}



		//				objTran.Complete();




		//			}
		//		}
		//		return Jobj;
		//	}
		//	catch (Exception ex)
		//	{


		//		return Jobj;
		//	}

		//}

		public JObject ValidateUser(string username, string psw, string lan, string dtoken)
		{

			JObject Jobj = new JObject(new JProperty("Failure", "Invalid Mobile/Password"));

			try
			{

				if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(psw))
				{

					var result = db.Registrations.Where(Usr => (Usr.Mobile.Trim() == username) &&
												  Usr.Password == psw && Usr.IsBlocked == false).Select(user => new { user.UserId, user.IsVerified, user.RandomNumber }).FirstOrDefault();

					if (result != null)
					{
						if (result.IsVerified == true)
						{
							Registration _reg = db.Registrations.Single(reg => reg.Mobile.Trim() == username && reg.IsVerified == true);
							//  if(_reg.UserId !=0)

							_reg.DeviceToken = dtoken;
							_reg.Language = lan;
							db.Entry(_reg).State = System.Data.Entity.EntityState.Modified;
							db.SaveChanges();

							SaveUserDevicetoken(_reg.DeviceToken, "Regd", _reg.UserId);
							if (_reg.IsDriver == true)
							{
								// DriverLocation dLocation = db.DriverLocations.Single(dLoc => dLoc.DriverId == _reg.UserId);
								///  dLocation.IsActive = true;
								//  db.SaveChanges();
							}
							Jobj = new JObject(new JProperty("Success", JArray.Parse(JsonConvert.SerializeObject(db.Registrations.Where(
												 usr => usr.UserId == result.UserId).Select(UDetails =>
													 new
													 {
														 UDetails.UserId,
														 UDetails.Email,
														 UDetails.FullName,
														 UDetails.Mobile,
														 IsDriver = UDetails.IsDriver == null ? false : UDetails.IsDriver,
														 UDetails.Language,
														 UDetails.IsVerified,

														 UDetails.Password,
														 UDetails.FamilyName,
														 UDetails.NickName,
														 UDetails.Gender
													 }).ToList()))));


						}
						else
						{
							if (result.IsVerified == false)
							{
								try
								{

									SendOTP(result.UserId, username, result.RandomNumber);
									Jobj = new JObject(new JProperty("Success", JArray.Parse(JsonConvert.SerializeObject(db.Registrations.Where(
												usr => usr.UserId == result.UserId).Select(UDetails =>
													new
													{
														UDetails.UserId,
														UDetails.Email,
														UDetails.FullName,
														UDetails.Mobile,
														IsDriver = UDetails.IsDriver == null ? false : UDetails.IsDriver,
														UDetails.Language,
														UDetails.IsVerified,
														UDetails.Password,
														UDetails.FamilyName,
														UDetails.NickName,
														UDetails.Gender
													}).ToList()))));
								}
								catch
								{

								}

							}

						}


					}
				}
				return Jobj;
			}
			catch (Exception ex)
			{

				return Jobj;
			}

		}

		public JObject MobileVerification(string MobileNo, int OTP)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "Invalid OTP"));
			try
			{
				//var res = db.Registrations.Where(usr => usr.Mobile == MobileNo && usr.RandomNumber == OTP).Select(user => new { user.UserId });
				Registration _reg = db.Registrations.Single(reg => reg.Mobile.Trim() == MobileNo && reg.RandomNumber == OTP);
				if (_reg.UserId != 0)
				{
					using (TransactionScope objTran = new TransactionScope())
					{
						_reg.IsVerified = true;
						db.Entry(_reg).State = System.Data.Entity.EntityState.Modified;
						db.SaveChanges();
						//block  unverified mobile numbers
						var inActiveUsr = db.Registrations.Where(usr => usr.Mobile.Trim() == MobileNo && usr.IsVerified == false).ToList();//.Select(usrid => new { usrid.UserId }); 
						if (inActiveUsr.Count > 0)
						{
							inActiveUsr.ForEach(user => user.IsBlocked = true);
							//  db.Entry(inActiveUsr).State = System.Data.Entity.EntityState.Modified;
							db.SaveChanges();
						}


						Jobj = new JObject(new JProperty("Success", new JArray(new JObject(
														 new JProperty("UserId", _reg.UserId),
														new JProperty("FullName", _reg.FullName),
														new JProperty("Email", _reg.Email),
														 new JProperty("Mobile", _reg.Mobile),
														  new JProperty("IsVerified", _reg.IsVerified),
														   new JProperty("Language", _reg.Language),
															new JProperty("FamilyName", _reg.FamilyName),
														  new JProperty("NickName", _reg.NickName),
														   new JProperty("Gender", _reg.Gender),
														   new JProperty("IsDriver", _reg.IsDriver),
														   new JProperty("Password", _reg.Password)))));
						objTran.Complete();

					}

					// Common.InsertPushMsg(_reg.UserId, "Welcome To Oregano", _reg.DeviceToken, (int)Common.NotificationType.Welcome);

				}
				return Jobj;
			}
			catch (Exception EX)
			{
				return Jobj;
			}

		}


		public JObject UpdateUserProfile(JObject Json)
		{
			JObject jobj = new JObject(new JProperty("Failure", "InvalidUser"));
			try
			{

				jobj = Json;// JObject.Parse(Json);

				IEnumerable<JToken> UsrDetails = jobj.SelectToken("UserProfile");
				foreach (JToken usr in UsrDetails)
				{
					Registration _usrProfile = JsonConvert.DeserializeObject<Registration>(usr.ToString());
					Registration usrDetails = db.Registrations.Where(user => user.UserId == _usrProfile.UserId).FirstOrDefault();

					usrDetails.FamilyName = _usrProfile.FamilyName;
					usrDetails.FullName = _usrProfile.FullName;
					usrDetails.NickName = _usrProfile.NickName;
					usrDetails.Email = _usrProfile.Email;
					usrDetails.Gender = _usrProfile.Gender;
					usrDetails.DeviceToken = _usrProfile.DeviceToken;
					usrDetails.Language = _usrProfile.Language;
					usrDetails.UserId = _usrProfile.UserId;

					// var validate = db.Registrations.Where(isvalid => isvalid.Mobile == reg.Mobile).Select(usrId => new { usrId.UserId }).FirstOrDefault();
					db.Entry(usrDetails).State = System.Data.Entity.EntityState.Modified;
					db.SaveChanges();

					jobj = new JObject(new JProperty("Success", JArray.Parse(JsonConvert.SerializeObject(db.Registrations.Where(
												   usrs => usrs.UserId == _usrProfile.UserId).Select(UDetails =>
													   new
													   {
														   UDetails.UserId,
														   UDetails.Email,
														   UDetails.FullName,
														   UDetails.Mobile,
														   UDetails.Language,
														   UDetails.FamilyName,
														   UDetails.NickName,
														   UDetails.Gender

													   }).ToList()))));


				}

				return jobj;
			}
			catch
			{
				throw;
			}

		}

		public JObject UpdateAddressDetails(JObject Json)
		{
			JObject jobj = new JObject(new JProperty("Failure", false));
			try
			{

				jobj = Json;// JObject.Parse(Json);

				IEnumerable<JToken> UsrDetails = jobj.SelectToken("UserAddress");
				foreach (JToken usr in UsrDetails)
				{
					UserAddressDetail _usrAddress = JsonConvert.DeserializeObject<UserAddressDetail>(usr.ToString());

					_usrAddress.Address = _usrAddress.Address;
					_usrAddress.AddressType = _usrAddress.AddressType;
					_usrAddress.HouseName = _usrAddress.HouseName;
					_usrAddress.HouseNo = _usrAddress.HouseNo;
					//_usrAddress.IsActive = _usrAddress.IsActive;
					_usrAddress.LandMark = _usrAddress.LandMark;
					_usrAddress.Latitude = _usrAddress.Latitude;
					_usrAddress.Longitude = _usrAddress.Longitude;
					_usrAddress.Id = _usrAddress.Id;
					// var validate = db.Registrations.Where(isvalid => isvalid.Mobile == reg.Mobile).Select(usrId => new { usrId.UserId }).FirstOrDefault();
					db.Entry(_usrAddress).State = System.Data.Entity.EntityState.Modified;
					db.SaveChanges();
					jobj = new JObject(new JProperty("Success", true));


				}

				return jobj;
			}
			catch (Exception ex)
			{
				throw;
			}

		}


		public bool ChangePassword(int UsrId, string OldPsw, string NewPsw)
		{
			bool result = false;

			try
			{

				Registration reg = db.Registrations.Single(UsrCredentials =>
													UsrCredentials.UserId == UsrId && UsrCredentials.Password == OldPsw);

				if (reg.UserId != null)
				{
					reg.Password = NewPsw;
					db.Entry(reg).State = System.Data.Entity.EntityState.Modified;
					db.SaveChanges();
					result = true;
				}

			}
			catch
			{

			}

			return result;

		}

		public JObject SaveAddress(int Userid, string UsrAddress, string HouseNo, string LandMark, string AddressType, float Latitude, float Longitude, string HouseName, string ContactPerson = "", string ContactNo = "")
		{

			JObject jobj = new JObject(new JProperty("Failure", "User Does not Exists"));
			try
			{
				var SaveAdd = db.Registrations.Find(Userid);

				if (SaveAdd != null)
				{
					UserAddressDetail Uaddresss = new UserAddressDetail();

					Uaddresss.UserId = Userid;
					Uaddresss.Address = UsrAddress;
					Uaddresss.HouseNo = HouseNo;
					Uaddresss.LandMark = LandMark;
					Uaddresss.AddressType = AddressType;
					Uaddresss.HouseName = HouseName;
					Uaddresss.Latitude = Latitude;
					Uaddresss.Longitude = Longitude;
					Uaddresss.IsActive = true;
					Uaddresss.CreatedDate = Common.KSA_DateTime();//DateTime.Now;
					Uaddresss.ContactPerson = ContactPerson;
					Uaddresss.ContactNo = ContactNo;
					db.UserAddressDetails.Add(Uaddresss);
					db.SaveChanges();

					jobj = new JObject(new JProperty("Success", "1"));
				}


				return jobj;
			}
			catch (Exception ex)
			{
				//return false;
				throw;
			}
		}

		public JObject SaveUserAddress(JObject UserAdd)
		{

			JObject jobj = new JObject(new JProperty("Failure", "User Does not Exists"));
			try
			{
				string _uAdd = UserAdd.SelectToken("UserAddress").ToString();

				UserAddressDetail _usrAddress = JsonConvert.DeserializeObject<UserAddressDetail>(_uAdd);

				var existAdd = (from add in db.UserAddressDetails where add.Id == _usrAddress.Id select add).FirstOrDefault();

				if (existAdd == null)
				{

					_usrAddress.IsActive = true;
					_usrAddress.CreatedDate = Common.KSA_DateTime(); //DateTime.Now;
					db.UserAddressDetails.Add(_usrAddress);
					db.SaveChanges();
				}
				else
				{
					existAdd.Address = _usrAddress.Address;
					existAdd.UserId = _usrAddress.UserId;
					existAdd.LandMark = _usrAddress.LandMark;
					existAdd.Latitude = _usrAddress.Latitude;
					existAdd.Longitude = _usrAddress.Longitude;
					existAdd.HouseName = _usrAddress.HouseName;
					existAdd.HouseNo = _usrAddress.HouseNo;
					existAdd.AddressType = _usrAddress.AddressType;
					existAdd.ContactPerson = _usrAddress.ContactPerson;
					existAdd.ContactNo = _usrAddress.ContactNo;
					existAdd.IsActive = true;
					existAdd.CreatedDate = Common.KSA_DateTime();

					//  db.UserAddressDetails.Add(Uaddresss);
					db.SaveChanges();

				}



				jobj = new JObject(new JProperty("Success", "1"));
				return jobj;
			}




			catch (Exception ex)
			{
				//return false;
				throw;
			}
		}

		public JObject GetUsrAddressDetails(int Userid)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No RecordFound"));
			try
			{


				var adDetails = JArray.Parse(JsonConvert.SerializeObject(db.UserAddressDetails.Where(
										 usr => usr.UserId == Userid && usr.IsActive == true).OrderByDescending(uAddress => uAddress.Id).Select(
										 uAddress => new
										 {
											 uAddress.Id,
											 uAddress.Address,
											 uAddress.AddressType,
											 uAddress.HouseNo,
											 uAddress.LandMark,
											 uAddress.Longitude,
											 uAddress.Latitude,
											 uAddress.HouseName,
											 uAddress.ContactPerson,
											 uAddress.ContactNo
										 })));


				

				return adDetails.Count > 0 ? Jobj = new JObject(new JProperty("Success", adDetails)) : Jobj;


			}
			catch (Exception ex)
			{
				return null;

			}
		}

		public JObject DeleteAddressDetails(int addressId)
		{

			JObject jobj = new JObject(new JProperty("Failure", false));
			try
			{

				UserAddressDetail _usrAddress = db.UserAddressDetails.Single(o => o.Id == addressId);
				_usrAddress.IsActive = false;
				//  _orderInfo.FavoriteName = FavoriteName;
				db.Entry(_usrAddress).State = System.Data.Entity.EntityState.Modified;
				db.SaveChanges();
				jobj = new JObject(new JProperty("Success", true));
				return jobj;
			}
			catch
			{
				return jobj;
			}
		}

		public JObject ForgetPassword(string Username)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "InvalidUser"));
			try
			{

				Registration _reg = db.Registrations.Single(reg => reg.Mobile.Trim() == Username.Trim() && reg.IsVerified == true);
				if (_reg.UserId != 0)
				{
					string encatId = Encrypt(_reg.UserId.ToString());
					//string mailfrom = "info@dr-cafe.com";
					string mailfrom = System.Configuration.ConfigurationManager.AppSettings["mailFrom"].ToString();
					string mailTo = _reg.Email;
					string Msg = "In Progress...";
					//          string Msg = "<html><body><div style='width:570px;font-family: Verdana, Geneva, sans-serif;'>" +
					//"<div align='center'><div style='height:95px;width:570px;'>" +
					//"<table  style='width:570px;border:1px solid #b68f3f'><tr><td>" +
					//"<table style='margin-left: 0px;'><tr><td><a href='http://www.dr-cafe.com'  target='_blank'>" +
					//"<img src='http://www.dr-cafe.com/DCWebsite/images/logo.gif' alt='dr.Cafe Coffee' width='474px' height='88px'  style='border:0px'/></a></td>" +
					//"<td style='width:30px;'></td><td><table><tr><td style='width:30px'><a href='http://www.facebook.com/drcafeksa' target='_blank'>" +
					//"<img src='http://www.dr-cafe.com/DCWebsite/images/fb.jpg' alt='dr.Cafe Coffee Facebook' style='border:0px'/></a></td>" +
					//"<td style='width:30px'><a href='http://twitter.com/#/drcafeksa'  target='_blank'>" +
					//"<img src='http://www.dr-cafe.com/DCWebsite/images/tweet.jpg' alt='dr.Cafe Coffee Twitter' style='border:0px'/></a></td></tr></table></td></tr>" +
					//"</table><div style='width:570px;'></td></tr><tr><td><hr style='color:#b68f3f'></td></tr><tr><td>" +
					//"<table border='0px' cellpadding='0px' cellspacing='0px' style='width:570px;'><tr style='height:10px;'><td></td></tr>" +
					//"<tr><td align='left'><font face='Verdana' style='font-size: 11px'>Dear Customer </font></td></tr>" +
					//"<tr><td style='height:8px;'></td></tr><tr><td align='left'><p align=justify><font face='Verdana' style='font-size: 11px'>" +
					//"We take your privacy and security seriously, so please take a moment to reset your password at the link below: </font></p>" +
					//"</td></tr><tr><td style='height:8px;'></td></tr><tr><td align='left'><p><font face='Verdana' style='font-size: 11px'>" +
					//"<a style='color:#b8861a; font-weight:bold' href=http://www.drcafeonline.net/resetpassword.aspx?uId=" + encatId + ">Reset your password</a></font>" +
					//"</p></td></tr><tr><td style='height:8px;'></td></tr><tr><td><p><font face='Verdana' style='font-size: 11px'>" +
					//"Please let us know if we can be of further assistance by calling <font face='Century Gothic' color='#b8861a' style='font-size: 11px;font-weight:bold'> 800 122-8222 (available 24 / 7)</font>." +
					//"</font></p></td></tr><tr><td style='height:8px;'></td></tr><tr><td align='left'><p>" +
					//"<font face='Verdana' style='font-size: 11px'>Thank you for using <font face='Times New Roman' color='#86837d' style='font-size: 11px'><i>dr.</i></font><b>" +
					//"<font face='Century Gothic' color='#b8861a' style='font-size: 11px'>CAFE COFFEE</font></b>. We look forward to seeing you soon!.</font>" +
					//"</p></td></tr><tr><td style='height:8px;'></td></tr><tr><td align='left'><p><font face='Verdana' style='font-size: 11px'>" +
					//"As this is an automated response, please do not reply to this email. </p></td></tr><tr><td style='height:8px;'></td></tr><tr><td align='right'>" +
					//"<font face='Times New Roman' color='#86837d' style='font-size: 11px'><i>dr.</i></font><b>" +
					//"<font face='Century Gothic' color='#b8861a' style='font-size: 11px'>CAFE COFFEE.</font></b></td></tr>" +
					//"<tr style='height:10px;'><td></td></tr></table></td></tr></table><br /></div></div></div></body></html>";

					string Subject = "Assistance with Forget Password";
					this.sendEmail(mailfrom, mailTo, Msg, Subject);
					Jobj = new JObject(new JProperty("Success", "Email Send"));
				}

				return Jobj;
			}
			catch
			{
				return Jobj;
			}

		}

		public static string Encrypt(string TextToBeEncrypted)
		{
			RijndaelManaged RijndaelCipher = new RijndaelManaged();
			string Password = "CSC";
			byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(TextToBeEncrypted);
			byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
			PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
			//Creates a symmetric encryptor object.
			ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
			MemoryStream memoryStream = new MemoryStream();
			//Defines a stream that links data streams to cryptographic transformations
			CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
			cryptoStream.Write(PlainText, 0, PlainText.Length);
			//Writes the final state and clears the buffer
			cryptoStream.FlushFinalBlock();
			byte[] CipherBytes = memoryStream.ToArray();
			memoryStream.Close();
			cryptoStream.Close();
			string EncryptedData = Convert.ToBase64String(CipherBytes);
			return EncryptedData;

		}

		private void sendEmail(string mailfrom, string mailTo, string Msg, string Subject)
		{
			// bool returnCode = false;

			try
			{
				string msg1 = string.Empty;
				//MailMessage MailMessage = new MailMessage(mailfrom, mailTo, Subject, Msg);
				//MailMessage.IsBodyHtml = true;

				//MailMessage.CC.Add(ccList);
				//MailMessage.Priority = MailPriority.Normal;
				//SmtpClient smtpclint = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["SMTPServer"]);
				//smtpclint.UseDefaultCredentials = true;
				//smtpclint.Send(MailMessage);


				MailMessage mMailMessage = new MailMessage();
				mMailMessage.From = new MailAddress(mailfrom);
				mMailMessage.To.Add(new MailAddress(mailTo));
				mMailMessage.Subject = Subject;
				mMailMessage.Body = Msg;
				mMailMessage.IsBodyHtml = true;
				mMailMessage.Priority = MailPriority.Normal;
				SmtpClient mSmtpClient = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["SMTPServer"],
			   Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PortNo"]));
				mSmtpClient.UseDefaultCredentials = false;
				mSmtpClient.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["mailFrom"].ToString(),
				System.Configuration.ConfigurationManager.AppSettings["mailCredentialPassword"].ToString());
				mSmtpClient.Send(mMailMessage);

				msg1 = "Successful";



			}

			catch (Exception ex)
			{
				ex.Message.ToString();
			}

			//  return returnCode;
		}

		public bool SendOTP(int? UserId, string Mobile, int? RandomNumber)
		{
			bool isSuccess = false;
			try
			{
				ServicesSoapClient jawalb = new ServicesSoapClient();

				//string smsMessage = "Your activation code is: " + RandomNumber + "\n\n" + "Enjoy one time free drink. To avail use promo code " + "''" + userPromCode + "''";
				//string smsMessage = "Your activation code is:" + "''" + RandomNumber + "''";

				string smsMessage = "Your Unique Verification Code for Bakery and Company is :" + "''" + RandomNumber + "''";
				string jawalUserName = System.Configuration.ConfigurationManager.AppSettings["jawalbSMSUserName"];
				string jawalPassword = System.Configuration.ConfigurationManager.AppSettings["jawalbSMSPassword"];
				string jawalSender = System.Configuration.ConfigurationManager.AppSettings["jawalbSMSSender"];
				string mobileNo = Mobile.Replace("+", "").Replace("-", "");
				string[] result = jawalb.SendSMS(jawalUserName, jawalPassword, jawalSender, mobileNo, smsMessage).ToArray();
				if (result[1] == "SUCCESS")
				{
					// DCAppDataContext dbContext = new DCAppDataContext();
					// var userID = (from userLogin in dbContext.tblUserLogins where userLogin.username == username select userLogin.userId).SingleOrDefault();
					SMSDetail objSMS = new SMSDetail();
					objSMS.SMSDate = Common.KSA_DateTime();//DateTime.Now;
					objSMS.UserId = UserId;
					objSMS.MobileNo = mobileNo;
					objSMS.Message = smsMessage;
					db.SMSDetails.Add(objSMS);
					db.SaveChanges();
					isSuccess = true;
				}
				return isSuccess;
			}
			catch (Exception Ex)
			{
				return isSuccess;
			}
		}

		public JObject ForgetPassword(string mobileno, string NewPsw)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Data Found"));

			try
			{

				Registration reg = db.Registrations.Single(UsrCredentials =>
													 UsrCredentials.Mobile == mobileno && UsrCredentials.IsVerified == true);

				if (reg.UserId != null)
				{
					reg.Password = NewPsw;
					db.Entry(reg).State = System.Data.Entity.EntityState.Modified;
					db.SaveChanges();
					Jobj = new JObject(new JProperty("Success", new JArray(new JObject(
												 new JProperty("UserId", reg.UserId),
												new JProperty("FullName", reg.FullName),
												new JProperty("Email", reg.Email),
												 new JProperty("Mobile", reg.Mobile),
												  new JProperty("IsVerified", reg.IsVerified),
												   new JProperty("Language", reg.Language),
													//  new JProperty("RandomNumber", reg.RandomNumber),
													new JProperty("FamilyName", reg.FamilyName),
												  new JProperty("NickName", reg.NickName),
												   new JProperty("Gender", reg.Gender),
												   new JProperty("IsDriver", reg.IsDriver),
												   new JProperty("Password", reg.Password)))));
				}

				return Jobj;

			}
			catch (Exception ex)
			{
				return new JObject(new JProperty("Failure", ex.Message));
			}



		}

		public JObject SendOTPManually(string UserID)
		{
			try
			{
				Registration reg = db.Registrations.Where(usrid => usrid.Mobile == UserID && usrid.IsVerified == true).FirstOrDefault();
				reg.RandomNumber = Common.getRandomNumber();
				db.SaveChanges();
				SendOTP(reg.UserId, reg.Mobile, reg.RandomNumber);


				JObject jobj = new JObject(new JProperty("Success",
						 new JObject(
						 new JProperty("MobileNo", reg.Mobile),
									  new JProperty("OTP", reg.RandomNumber))));
				return jobj;
			}
			catch (Exception Ex)
			{
				return new JObject(new JProperty("Failure", "+" + UserID + " Does not Exists"));
			}

		}

		public JObject CheckMobileNo(string MobileNo)
		{
			JObject jobj = new JObject(new JProperty("Failure", "User Already Exists"));
			try
			{
				var validate = db.Registrations.Where(isvalid => isvalid.Mobile == MobileNo).Select(usrId => new { usrId.UserId }).FirstOrDefault();

				if (null == validate)
				{
					int OTP = Common.getRandomNumber();
					SendOTP(null, MobileNo, OTP);
					jobj = new JObject(new JProperty("Success",
						new JObject(
						new JProperty("MobileNo", MobileNo),
									 new JProperty("OTP", OTP))));
				}
				Dispose();
				return jobj;

			}
			catch (Exception Ex)
			{
				Dispose();
				return jobj;
			}


		}

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
	}
}
