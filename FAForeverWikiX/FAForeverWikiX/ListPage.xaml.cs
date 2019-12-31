using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FAForeverWikiX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ListPage : ContentPage
	{
        private string fraction;
        private StackLayout listLayout;

		public ListPage(string fraction)
		{
			InitializeComponent();
            this.fraction = fraction;
            DynamicAddUnits();
        }

        private void DynamicAddUnits()
        {
            var butUnit = new Button();
            butUnit.StyleId = "mostrar";
            butUnit.Text = "+++";
            butUnit.BackgroundColor = Color.ForestGreen;
            listLayout.Children.Add(butUnit);
        }

        private void ConnectToDataBase()
        {
            var dbpath = Path.Combine(
                @"D:\Alexandr Olegovich\Projects\DataBaseFAFWiki\DataBaseFAFWiki\bin\Debug\", 
                "fafWiki.db");

            SqliteConnection db = new SqliteConnection(dbpath);
            SqliteCommand com = new SqliteCommand();
            DataSet ds = new DataSet();

            string sqlQuery = "SELECT ID from Units";
            com.CommandText = sqlQuery;
            db.Open();
            using (var reader = com.ExecuteReader())
            {
                List<string> listID = new List<string>();
                while (reader.Read())
                    listID.Add(reader.GetString(0));
            }
            db.Close();
        }
	}
}