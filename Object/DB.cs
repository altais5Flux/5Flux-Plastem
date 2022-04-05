using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebservicesSage.Object
{
    class DB
    {
        private static SqlConnection cnn;
        public static void Connect()
        {
            string connetionString;
            connetionString = @"Data Source=" + ConfigurationManager.AppSettings["SERVER"].ToString() + ";Initial Catalog=" + ConfigurationManager.AppSettings["DBNAME"].ToString() + ";User ID=" + ConfigurationManager.AppSettings["SQLUSER"].ToString() + ";Password=" + ConfigurationManager.AppSettings["SQLPWD"].ToString();
            //connetionString = @"Data Source=XAPP01503\SAGEI7;Initial Catalog=BIJOU;User ID=demo-altais;Password=Inax2f5q!";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
        }

        public static void Disconnect()
        {
            cnn.Close();
        }

        public static SqlDataReader Select(string sql)
        {
            //Connect();

            SqlCommand command = new SqlCommand(sql, cnn);
            SqlDataReader dataReader1 = command.ExecuteReader();

            //Disconnect();
            return dataReader1;
        }

    }
}
