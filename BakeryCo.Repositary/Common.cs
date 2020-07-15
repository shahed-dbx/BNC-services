using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeryCo.DataModel;

namespace BakeryCo.Repositary
{
  public static  class Common
    {

    
        public static DateTime KSA_DateTime()
        {

            TimeZoneInfo KSATimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
            DateTime utc = DateTime.UtcNow;
            DateTime KSA = TimeZoneInfo.ConvertTimeFromUtc(utc, KSATimeZone);

            return KSA;
        }
        public static int getRandomNumber()
        {
            Random r = new Random();
            return r.Next(1000, 9999);
        }
        public enum NotificationType
        {
            Confirmed = 1,
            Ready,
            Server,
            Rejecet,
            OnTheWay,
            Delivered,
            Offer,
            Welcome
        }
        public static void InsertPushMsg(int UserId, String Msg, string DToken, int PushMsgType)
        {
            using (BakeryCoEntities db = new BakeryCoEntities())
            {
                PushNotificationCustomer objPushMsg = new PushNotificationCustomer();
                objPushMsg.PushMessage = Msg;
                objPushMsg.IsRead = false;
                objPushMsg.IsSent = false;
                objPushMsg.PushNotificationType = PushMsgType;
                objPushMsg.Status = true;
                objPushMsg.UserId = UserId;
                objPushMsg.DeviceToken = DToken;
                objPushMsg.SentDate = KSA_DateTime();
                db.PushNotificationCustomers.Add(objPushMsg);
                db.SaveChanges();

            }
        }


        //public string sendSMS()
        //{
        //    String result;
        //    string apiKey = "your apiKey";
        //    string numbers = "918123456789"; // in a comma seperated list
        //    string message = "This is your message";
        //    string sender = "TXTLCL";

        //    String url = "https://api.textlocal.in/send/?apikey=" + apiKey + "&numbers=" + numbers + "&message=" + message + "&sender=" + sender;
        //    //refer to parameters to complete correct url string

        //    StreamWriter myWriter = null;
        //    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);

        //    objRequest.Method = "POST";
        //    objRequest.ContentLength = Encoding.UTF8.GetByteCount(url);
        //    objRequest.ContentType = "application/x-www-form-urlencoded";
        //    try
        //    {
        //        myWriter = new StreamWriter(objRequest.GetRequestStream());
        //        myWriter.Write(url);
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //    finally
        //    {
        //        myWriter.Close();
        //    }

        //    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
        //    using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
        //    {
        //        result = sr.ReadToEnd();
        //        // Close and clean up the StreamReader
        //        sr.Close();
        //    }
        //    return result;
        //}  
    }
	public sealed class DbConnector
	{
		private DbConnector()
		{

		}
		private static string GetConnectionString()
		{
			return ConfigurationManager.ConnectionStrings["appserviceconnection"].ConnectionString;
		}
		//public static byte[] AssignDocumentAdd(string Path, byte[] buffer)
		//{
		//	SqlConnection conn = new SqlConnection(GetConnectionString());
		//	conn.Open();
		//	SqlTransaction objSqlTran = conn.BeginTransaction();
		//	SqlCommand cmd = conn.CreateCommand();

		//	cmd = new SqlCommand("SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()", conn, objSqlTran);
		//	byte[] objContext = (byte[])cmd.ExecuteScalar();
		//	SqlFileStream objSqlFileStream = new SqlFileStream(Path, objContext, FileAccess.Write);
		//	objSqlFileStream.Write(buffer, 0, buffer.Length);
		//	objSqlFileStream.Close();
		//	objSqlTran.Commit();
		//	return objContext;

		//}
		//public static byte[] DocumentAdd(string Path, byte[] buffer)
		//{
		//	SqlConnection conn = new SqlConnection(GetConnectionString());
		//	conn.Open();
		//	SqlTransaction objSqlTran = conn.BeginTransaction();
		//	SqlCommand cmd = conn.CreateCommand();

		//	cmd = new SqlCommand("SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()", conn, objSqlTran);
		//	byte[] objContext = (byte[])cmd.ExecuteScalar();
		//	SqlFileStream objSqlFileStream = new SqlFileStream(Path, objContext, FileAccess.ReadWrite);
		//	objSqlFileStream.Write(buffer, 0, buffer.Length);
		//	objSqlFileStream.Close();
		//	objSqlTran.Commit();
		//	conn.Close();
		//	return objContext;

		//}
		public static string ExecuteNonQueryDocUpload(string cmdText, SqlParameter[] cmdParms, byte[] buffer)
		{
			string val = "";
			SqlConnection conn = new SqlConnection(GetConnectionString());


			//conn.Open();
			//SqlCommand cmd = conn.CreateCommand();
			//PrepareCommand(cmd, conn, null, CommandType.StoredProcedure, cmdText, cmdParms);
			//cmd.ExecuteNonQuery();
			//string val = (string)cmd.Parameters["@FilePath"].Value;
			//cmd.Parameters.Clear();
			//conn.Close();
			conn.Open();
			SqlCommand cmd = conn.CreateCommand();
			PrepareCommand(cmd, conn, null, CommandType.StoredProcedure, cmdText, cmdParms);
			var data = cmd.ExecuteReader();
			if (data.HasRows)
			{
				while (data.Read())
				{
					val = data["FilePath"].ToString();
				}
				data.Close();
			}
			cmd.Parameters.Clear();
			conn.Close();




			//conn.Open();
			//SqlTransaction objSqlTran = conn.BeginTransaction();
			//cmd = new SqlCommand("SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()", conn, objSqlTran);
			//byte[] objContext = (byte[])cmd.ExecuteScalar();


			//SqlFileStream objSqlFileStream = new SqlFileStream(val.Trim(), objContext, FileAccess.ReadWrite, FileOptions.None, 0);
			//objSqlFileStream.Write(buffer, 0, buffer.Length);
			//objSqlFileStream.Flush();
			//objSqlFileStream.Close();


			//objSqlTran.Commit();
			//conn.Close();


			return val;
		}

		public static int ExecuteNonQuery(string cmdText, SqlParameter[] cmdParms)
		{
			SqlConnection conn = new SqlConnection(GetConnectionString());
			SqlCommand cmd = conn.CreateCommand();
			PrepareCommand(cmd, conn, null, CommandType.StoredProcedure, cmdText, cmdParms);
			int val = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return val;

		}

		public static SqlDataReader ExecuteReader(string cmdText, SqlParameter[] cmdParms)
		{

			var conn = new SqlConnection(GetConnectionString());
			try
			{
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandTimeout = 0;
				PrepareCommand(cmd, conn, null, CommandType.StoredProcedure, cmdText, cmdParms);
				var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				//SqlCacheDependencyAdmin.EnableNotifications(GetConnectionString());
				//SqlCacheDependencyAdmin.EnableTableForNotifications(GetConnectionString(), "common.Category");
				//SqlCacheDependency sqlDependency = new SqlCacheDependency(cmd);
				//conn.Close();
				return dr;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static SqlDataReader ExecuteReaderII(string cmdText, SqlParameter[] cmdParms)
		{
			var conn = new SqlConnection(GetConnectionString());
			try
			{
				conn.Open();
				SqlCommand cmd = conn.CreateCommand();
				cmd.CommandTimeout = 0;
				PrepareCommand(cmd, conn, null, CommandType.StoredProcedure, cmdText, cmdParms);
				var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				//conn.Close();
				return dr;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		//public static string ExecuteDataSet(string cmdText, SqlParameter[] cmdParms)
		//{
		//	string strXml = "";
		//	SqlConnection conn = new SqlConnection(GetConnectionString());
		//	SqlCommand cmd = conn.CreateCommand();
		//	PrepareCommand(cmd, conn, null, CommandType.StoredProcedure, cmdText, cmdParms);
		//	//var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
		//	//return rdr;
		//	using (SqlDataAdapter da = new SqlDataAdapter(cmd))
		//	{
		//		DataSet ds = new DataSet();
		//		da.Fill(ds);
		//		cmd.Parameters.Clear();
		//		XmlElement xE = (XmlElement)Serialize(ds);
		//		strXml = xE.OuterXml.ToString();
		//	}
		//	conn.Close();
		//	return strXml;
		//}

		//public static XmlElement Serialize(object transformObject)
		//{
		//	XmlElement serializedElement = null;
		//	try
		//	{
		//		MemoryStream memStream = new MemoryStream();
		//		XmlSerializer serializer = new XmlSerializer(transformObject.GetType());
		//		serializer.Serialize(memStream, transformObject);
		//		memStream.Position = 0;
		//		XmlDocument xmlDoc = new XmlDocument();
		//		xmlDoc.Load(memStream);
		//		serializedElement = xmlDoc.DocumentElement;
		//	}
		//	catch (Exception SerializeException)
		//	{
		//	}
		//	return serializedElement;
		//}
		public static object ExecuteScalar(string cmdText, SqlParameter[] cmdParms)
		{
			SqlConnection conn = new SqlConnection(GetConnectionString());
			SqlCommand cmd = conn.CreateCommand();
			PrepareCommand(cmd, conn, null, CommandType.StoredProcedure, cmdText, cmdParms);
			object val = cmd.ExecuteScalar();
			cmd.Parameters.Clear();
			return val;
		}

		private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
		{
			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
			}
			cmd.Connection = conn;
			cmd.CommandText = cmdText;
			if (trans != null)
			{
				cmd.Transaction = trans;
			}
			cmd.CommandType = cmdType;
			//attach the command parameters if they are provided
			if (commandParameters != null)
			{
				AttachParameters(cmd, commandParameters);
			}
		}
		private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
		{
			foreach (SqlParameter p in commandParameters)
			{
				//check for derived output value with no value assigned
				if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
				{
					p.Value = DBNull.Value;
				}
				if (p.DbType == DbType.Date || p.DbType == DbType.DateTime || p.DbType == DbType.DateTime2)
				{
					try
					{
						if (p.SqlValue != null)
						{
							// just checking for exception
						}
					}
					catch
					{
						p.Value = null;
					}
				}

				command.Parameters.Add(p);
			}
		}
	}
}
