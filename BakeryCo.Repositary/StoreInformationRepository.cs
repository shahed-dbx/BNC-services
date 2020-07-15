using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeryCo.DataModel;

namespace BakeryCo.Repositary
{
	public class StoreInformationRepository
	{

		BakeryCoEntities db = new BakeryCoEntities();
		public JObject getStoresInfo(string day, string UserVersion, string AppType)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));

			try
			{
				var stortime = db.procSelectSpecifiedDayStoreTime(day, UserVersion, AppType).ToList();

				JArray resultarray = new JArray(JArray.Parse(JsonConvert.SerializeObject(stortime)));
				#region AppVersion JObj Preparation
				JObject res1 = new JObject();
				string Version = "0";
				string UpdatesAvailable = "0";
				string UpdateSeverity = "0";
				if (resultarray != null)
				{
					foreach (JObject ssdd in resultarray)
					{
						var AppVer = ssdd.SelectToken("AppVersion");
						if (AppVer != null)
						{
							string AppVerStr = Convert.ToString(AppVer);
							if (AppVerStr != "[]")
							{
								JArray a = JArray.Parse(AppVerStr);  //JArray sdfds = new JArray(JArray.Parse(JsonConvert.SerializeObject(AppVer)));
								if (a != null)
								{
									foreach (var innerAppver in a)
									{
										Version = Convert.ToString(innerAppver.SelectToken("Version"));
										UpdatesAvailable = Convert.ToString(innerAppver.SelectToken("UpdatesAvailable"));
										UpdateSeverity = Convert.ToString(innerAppver.SelectToken("UpdateSeverity"));
									}
								}
							}
						}
						//Preparing AppVersionJson.
						res1 = new JObject(new JProperty("Version", Version),
									(new JProperty("UpdatesAvailable", UpdatesAvailable)),
								   (new JProperty("UpdateSeverity", UpdateSeverity)));
						ssdd.Add(new JProperty("AppVersionDb", res1));
						
					}
				}

				// adding AppVersion to Main json result
				//JObject item = (JObject)resultarray.Last;
				//item.Add(new JProperty("AppVersionDb", res1));
				#endregion
				// Retun main json .
				return resultarray.Count > 0 ? Jobj = new JObject(new JProperty("success", resultarray)) : Jobj;
				

			}
			catch (Exception ex)
			{
				return Jobj;
			}


		}



		public JObject getStoreTimings(int storeId)
		{
			JObject Jobj = new JObject(new JProperty("Failure", "No Record Found"));
			try
			{

				var res = db.procSelectStoreInfo(storeId).ToList();

				JArray resultArray = new JArray(JArray.Parse(JsonConvert.SerializeObject(res)));

				return resultArray.Count > 0 ? Jobj = new JObject(new JProperty("Success", resultArray)) : Jobj;




			}
			catch
			{
				return Jobj;
			}

		}
	}
}
