using AngleSharp.Parser.Html;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace DataBaseFAFWiki
{
    public partial class MainForm : Form
    {
        string[] idUnits;
        string team = string.Empty;
        DataTable dt = new DataTable();
        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter();
        static SQLiteConnection con = new SQLiteConnection(ConnectionSQL());
        SQLiteCommand command = new SQLiteCommand();

        public MainForm()
        {
            InitializeComponent();
            command.Connection = con;
            TableGroupArray();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            UnitID();
        }

        async void UnitID()
        {
            List<string> list = new List<string>();

            HttpClient client = new HttpClient();
            string urlID = $"http://direct.faforever.com/faf/unitsDB/";

            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.81 Safari/537.36");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Referer", "http://direct.faforever.com/faf/unitsDB/");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            
            var response = await client.GetAsync(urlID);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string source = await response.Content.ReadAsStringAsync();
                HtmlParser domParser = new HtmlParser();
                var document = await domParser.ParseAsync(source);

                var arr = document.QuerySelectorAll("span").Where(x => x.ClassName == "unitHotLink");

                DataSet ds = new DataSet();
                con.Open();
                foreach (var a in arr)
                {
                    string q = a.GetAttribute("onclick");
                    q = q.Substring(q.IndexOf('\''), 8);
                    string q2 = a.TextContent.Replace("\n", "").Replace("\t", "").Replace(" ", "");
                    q2 = q2.Substring(0, 2);
                    q2 = (q2 == "T1" || q2 == "T2" || q2 == "T3" || q2 == "T4" ? q2 : "");

                    string sqlQuery = $"INSERT INTO Units(ID, tech) VALUES({q}', '{q2}')";
                    command.CommandText = sqlQuery;
                    command.ExecuteNonQuery();
                }
                con.Close();
            }
        }

        async void SetDataUnit(string id)
        {
            HttpClient client = new HttpClient();
            string urlID = $"http://direct.faforever.com/faf/unitsDB/?id={id}";
            //string homeUrl = "http://direct.faforever.com/faf/unitsDB/";

            client.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.81 Safari/537.36");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Referer", "http://direct.faforever.com/faf/unitsDB/");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

            var response = await client.GetAsync(urlID);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string source = await response.Content.ReadAsStringAsync();
                HtmlParser domParser = new HtmlParser();
                var document = await domParser.ParseAsync(source);

                //var img1 = document.QuerySelector($"body > div.comparisonBoard > div:nth-child(1) > div > div > div > div > div.previewImg > img.strategicIconOverlap");
                //var img2 = document.QuerySelector($"body > div.comparisonBoard > div:nth-child(1) > div > div > div > div > div.previewImg > img.strategicIcon");
                //var img3 = document.QuerySelector($"body > div.comparisonBoard > div:nth-child(1) > div > div > div > div > div.previewImg > img.backgroundIconOverlap");
                //string img1Url = homeUrl + img1.OuterHtml.ToString().Remove(0, 49).Replace("hue-rotate", "").Replace("style=","").Replace("filter: ", "");

                var name = document.QuerySelector("body > div.comparisonBoard > div:nth-child(1) > div > div > div > div > div:nth-child(2) > p.previewTitle");
                var hp = document.QuerySelector("body > div.comparisonBoard > div:nth-child(2) > div > div");

                var energy_cost = document.QuerySelector("body > div.comparisonBoard > div:nth-child(3) > div > div:nth-child(1) > div.flexColumns > div.energyCost.bubbleInfo");
                var mass_cost = document.QuerySelector("body > div.comparisonBoard > div:nth-child(3) > div > div:nth-child(1) > div.flexColumns > div.massCost.bubbleInfo");
                var build_time = document.QuerySelector("body > div.comparisonBoard > div:nth-child(3) > div > div:nth-child(1) > div.flexColumns > div.buildTimeCost.bubbleInfo");

                var energy_yield = document.QuerySelector("body > div.comparisonBoard > div:nth-child(3) > div > div:nth-child(2) > div.flexColumns > div.energyCost.bubbleInfo");
                var mass_yield = document.QuerySelector("body > div.comparisonBoard > div:nth-child(3) > div > div:nth-child(2) > div.flexColumns > div.massCost.bubbleInfo");
                var build_capacity = document.QuerySelector("body > div.comparisonBoard > div:nth-child(3) > div > div:nth-child(2) > div.flexColumns > div.buildTimeCost.bubbleInfo");

                var energy_storage = document.QuerySelector("body > div.comparisonBoard > div:nth-child(3) > div > div:nth-child(3) > div.flexColumns > div.energyCost.bubbleInfo");
                var mass_storage = document.QuerySelector("body > div.comparisonBoard > div:nth-child(3) > div > div:nth-child(3) > div.flexColumns > div.massCost.bubbleInfo");

                var abilities = document.QuerySelector("body > div.comparisonBoard > div:nth-child(4) > div > div > div.flexWrap");
                var vision = document.QuerySelector("body > div.comparisonBoard > div:nth-child(6) > div > div > div.flexColumns > div");

                var wreckageHP = document.QuerySelector("body > div.comparisonBoard > div:nth-child(7) > div > div > div.flexColumns > div:nth-child(2) > div:nth-child(1)");
                var wreckageHPp = document.QuerySelector("body > div.comparisonBoard > div:nth-child(7) > div > div > div.flexColumns > div:nth-child(3) > div:nth-child(1)");
                var wreckageMass = document.QuerySelector("body > div.comparisonBoard > div:nth-child(7) > div > div > div.flexColumns > div:nth-child(2) > div:nth-child(2)");
                var wreckageMassp = document.QuerySelector("body > div.comparisonBoard > div:nth-child(7) > div > div > div.flexColumns > div:nth-child(3) > div:nth-child(2)");

                var turnRate = document.QuerySelector("body > div.comparisonBoard > div:nth-child(5) > div > div > div.flexRows > div:nth-child(1)");
                var maxSpeed = document.QuerySelector("body > div.comparisonBoard > div:nth-child(5) > div > div > div.flexRows > div:nth-child(2)");

                var weapons = document.QuerySelector("body > div.comparisonBoard > div:nth-child(10) > div > div > div.flexColumn");
                string strWeapon = "-";
                if (weapons != null)
                {
                    var weapon = weapons.ChildNodes; strWeapon = "";
                    for (int i = 0; i < weapon.Length; i++)
                        if (weapon[i].TextContent != null)
                        {
                            var weaponTypes = weapon[i].ChildNodes;
                            for (int j = 0; j < weaponTypes.Length; j++)
                            {
                                var weaponProperty = weaponTypes[j].ChildNodes;
                                for (int k = 0; k < weaponProperty.Length; k++)
                                {
                                    strWeapon += weaponProperty[k].TextContent
                                        .Replace("\t\t\t\t", " ")
                                        .Replace("\t", "")
                                        .Replace("\n\n", "")
                                        .Replace("\n", "") + "\n";
                                }
                            }
                        }
                }
                var veterancyT1 = document.QuerySelector($"#T1{id} > div");
                string strVeterancyT1 = "-";
                if (veterancyT1 != null)
                {
                    var veterancy = veterancyT1.ChildNodes; strVeterancyT1 = "";
                    for (int i = 0; i < veterancy.Length; i++)
                        if (veterancy[i].TextContent != null)
                        {
                            strVeterancyT1 += veterancy[i].TextContent
                                .Replace("\t\t\t\t", " ")
                                .Replace("\t", "")
                                .Replace("\n\n", "")
                                .Replace("\n", "")
                                .Replace("  ", "") + "\n";
                        }
                }
                var veterancyT2 = document.QuerySelector($"#T2{id} > div");
                string strVeterancyT2 = "-";
                if (veterancyT2 != null)
                {
                    var veterancy = veterancyT2.ChildNodes; strVeterancyT2 = "";
                    for (int i = 0; i < veterancy.Length; i++)
                        if (veterancy[i].TextContent != null)
                        {
                            strVeterancyT2 += veterancy[i].TextContent
                                .Replace("\t\t\t\t", " ")
                                .Replace("\t", "")
                                .Replace("\n\n", "")
                                .Replace("\n", "")
                                .Replace("  ", "") + "\n";
                        }
                }
                var veterancyT3 = document.QuerySelector($"#T3{id} > div");
                string strVeterancyT3 = "-";
                if (veterancyT3 != null)
                {
                    var veterancy = veterancyT3.ChildNodes; strVeterancyT3 = "";
                    for (int i = 0; i < veterancy.Length; i++)
                        if (veterancy[i].TextContent != null)
                        {
                            strVeterancyT3 += veterancy[i].TextContent
                                .Replace("\t\t\t\t", " ")
                                .Replace("\t", "")
                                .Replace("\n\n", "")
                                .Replace("\n", "")
                                .Replace("  ", "") + "\n";
                        }
                }
                var veterancyT4 = document.QuerySelector($"#T4{id} > div");
                string strVeterancyT4 = "-";
                if (veterancyT4 != null)
                {
                    var veterancy = veterancyT4.ChildNodes; strVeterancyT4 = "";
                    for (int i = 0; i < veterancy.Length; i++)
                        if (veterancy[i].TextContent != null)
                        {
                            strVeterancyT4 += veterancy[i].TextContent
                                .Replace("\t\t\t\t", " ")
                                .Replace("\t", "")
                                .Replace("\n\n", "")
                                .Replace("\n", "")
                                .Replace("  ", "") + "\n";
                        }
                }
                var veterancySACU = document.QuerySelector($"#SACU{id} > div");
                string strVeterancySACU = "-";
                if (veterancySACU != null)
                {
                    var veterancy = veterancySACU.ChildNodes; strVeterancySACU = "";
                    for (int i = 0; i < veterancy.Length; i++)
                        if (veterancy[i].TextContent != null)
                        {
                            strVeterancySACU += veterancy[i].TextContent
                                .Replace("\t\t\t\t", " ")
                                .Replace("\t", "")
                                .Replace("\n\n", "")
                                .Replace("\n", "")
                                .Replace("  ", "") + "\n";
                        }
                }
                var veterancyACU = document.QuerySelector($"#ACU{id} > div");
                string strVeterancyACU = "-";
                if (veterancyACU != null)
                {
                    var veterancy = veterancyACU.ChildNodes; strVeterancyACU = "";
                    for (int i = 0; i < veterancy.Length; i++)
                        if (veterancy[i].TextContent != null)
                        {
                            strVeterancyACU += veterancy[i].TextContent
                                .Replace("\t\t\t\t", " ")
                                .Replace("\t", "")
                                .Replace("\n\n", "")
                                .Replace("\n", "")
                                .Replace("  ", "") + "\n";
                        }
                }
                var enhancements = document.QuerySelector("body > div.comparisonBoard > div:nth-child(11) > div > div > div.flexColumn");
                string strEnhancements = "-";
                if (enhancements != null)
                {
                    var enhancement = enhancements.ChildNodes; strEnhancements = "";
                    for (int i = 0; i < enhancement.Length; i++)
                        if (enhancement[i].TextContent != null)
                        {
                            var enhancementTypes = enhancement[i].ChildNodes;
                            for (int j = 0; j < enhancementTypes.Length; j++)
                            {
                                strEnhancements += enhancementTypes[j].TextContent
                                        .Replace("\t\t\t\t", " ")
                                        .Replace("\t", "")
                                        .Replace("'", "")
                                        .Replace("\n", "")
                                        .Replace("  ", "")
                                        + "\n";
                            }
                        }
                }
                var blueprints = document.QuerySelector($"#blueprintsSection{id}");
                string strBlueprints = "-";
                if (blueprints != null)
                {
                    var blueprint = blueprints.ChildNodes; strBlueprints = "";
                    for (int i = 0; i < blueprint.Length; i++)
                        if (blueprint[i].TextContent != null)
                        {
                            for (int j = 0; j < blueprint.Length; j++)
                            {
                                var bpName = blueprint[j].ChildNodes;
                                if (bpName != null && bpName.Length > 4)
                                {
                                    strBlueprints += bpName[3].TextContent
                                    .Replace("\t\t\t\t", " ")
                                    .Replace("\t", "")
                                    .Replace("\n\n", "")
                                    .Replace("\n", "")
                                    .Replace("  ", "") + "\n";
                                }
                            }
                        }
                }

                var teamColor = document.QuerySelector("body > div.comparisonBoard > div:nth-child(1) > div > div > div > div > div:nth-child(2)");
                if (teamColor.OuterHtml.Contains("color:#C8F7C5")) team = "Aeon";
                else if (teamColor.OuterHtml.Contains("color:#ADB6C4")) team = "UEF";
                else if (teamColor.OuterHtml.Contains("color:#F1A9A0")) team = "Cybran";
                else if (teamColor.OuterHtml.Contains("color:#FDE3A7")) team = "Seraphim";
                else team = "-";

                con.Open();
                string query = $"UPDATE Units SET team = '{team}', " +
                        $"name = '{StrBuild(name.TextContent).Replace("'", "")}', " +
                        $"HP = '{StrBuild(hp.TextContent)}', " +

                        $"energy_cost = '{StrBuild(energy_cost != null ? energy_cost.TextContent : "-").Replace("Energy cost", "")}', " +
                        $"mass_cost = '{StrBuild(mass_cost != null ? mass_cost.TextContent : "-").Replace("Mass cost", "")}', " +
                        $"build_time = '{StrBuild(build_time != null ? build_time.TextContent : "-").Replace("Build time", "")}', " +

                        $"energy_yield = '{StrBuild(energy_yield != null ? energy_yield.TextContent : "-").Replace("Energy yield / drain", "").Replace("Energy storage", " Energy storage")}', " +
                        $"mass_yield = '{StrBuild(mass_yield != null ? mass_yield.TextContent : "-").Replace("Mass yield / drain", "").Replace("Mass storage", " Mass storage")}', " +
                        $"build_capacity = '{StrBuild(build_capacity != null ? build_capacity.TextContent : "-").Replace("Build capacity", "")}', " +

                        $"energy_storage = '{StrBuild(energy_storage != null ? energy_storage.TextContent : "-").Replace("Energy storage", "")}', " +
                        $"mass_storage = '{StrBuild(mass_storage != null ? mass_storage.TextContent : "-").Replace("Mass storage", "")}', " +

                        $"abilities = '{StrBuild(abilities != null ? abilities.TextContent : "-").Replace("Build time", "")}', " +
                        $"vision = '{StrBuild(vision != null ? vision.TextContent : "-").Replace("Vision : ", "")}', " +

                        $"wreckageHP = '{StrBuild(wreckageHP != null ? wreckageHP.TextContent : "-")} " +
                        $"{StrBuild(wreckageHPp != null ? wreckageHPp.TextContent : "")}', " +
                        $"wreckageMass = '{StrBuild(wreckageMass != null ? wreckageMass.TextContent : "-")} " +
                        $"{StrBuild(wreckageMassp != null ? wreckageMassp.TextContent : "")}', " +

                        $"turnRate = '{StrBuild(turnRate != null ? turnRate.TextContent : "-").Replace("Turn rate : ", "")}', " +
                        $"maxSpeed = '{StrBuild(maxSpeed != null ? maxSpeed.TextContent : "-").Replace("Max speed : ", "")}', " +

                        $"weapons = '{strWeapon}', " +
                        $"veterancyT1 = '{strVeterancyT1}', " +
                        $"veterancyT2 = '{strVeterancyT2}', " +
                        $"veterancyT3 = '{strVeterancyT3}', " +
                        $"veterancyT4 = '{strVeterancyT4}', " +
                        $"veterancySACU = '{strVeterancySACU}', " +
                        $"veterancyACU = '{strVeterancyACU}', " +
                        $"enhancements = '{strEnhancements}', " +
                        $"blueprints = '{strBlueprints}' " +

                        $"WHERE ID = '{id}'";
                command.CommandText = query;
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        private static string ConnectionSQL()
        {
            return @"Data Source=D:\Alexandr Olegovich\Projects\DataBaseFAFWiki\DataBaseFAFWiki\bin\Debug\fafWiki.db";
        }

        string StrBuild(string text)
        {
            char[] denied = new[] { '\n', '\t', '\r' };
            StringBuilder strB = new StringBuilder();
            foreach (var ch in text)
                if (!denied.Contains(ch))
                    strB.Append(ch);
            return strB.ToString();
        }

        private void button_Click(object sender, System.EventArgs e)
        {
            button.Text = "Update Data Units";
            button.BackColor = System.Drawing.Color.LightCoral;
            
            button.Enabled = false;
            for (int i = 0; i < idUnits.Length; i++)
                SetDataUnit(idUnits[i]);
            TableGroupArray();
            button.BackColor = System.Drawing.Color.PaleGreen;
            button.Enabled = true;
        }

        void TableGroupArray()
        {
            DataSet ds = new DataSet();
            string sqlQuery = "SELECT ID from Units";
            command.CommandText = sqlQuery;
            con.Open();
            using (var reader = command.ExecuteReader())
            {
                List<string> listID = new List<string>();
                while (reader.Read())
                    listID.Add(reader.GetString(0));
                idUnits = listID.ToArray();
            }
            con.Close();

            command.CommandText = "SELECT * from Units";
            dataAdapter.SelectCommand = command;
            con.Open();
            dataAdapter.Fill(ds, "Units");
            dataGridView.DataSource = ds.Tables["Units"];
            con.Close();
        }
    }
}