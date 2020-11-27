using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace Yad2Updater
{
    enum TypeOfSearch
    {
        Appartments_For_Sale = 0,
        Appartments_For_Rent = 1,
        Cars_Yad2 = 2
    }

    public partial class Form1 : Form
    {
        string desktop_path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        DataTable table = new DataTable();
        public StreamWriter sw;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox_Type.SelectedIndex = 1;
            //textBox_URL.Text = "https://www.yad2.co.il/vehicles/private-cars";
            textBox_URL.Text = "https://www.yad2.co.il/realestate/rent";
        }

        private void CreateAppartmentsDataTableColumns()
        {
            //create columns of DataTable
            table.Columns.Add("Query ID", typeof(string));
            table.Columns.Add("Street Address", typeof(string));
            table.Columns.Add("City", typeof(string));
            table.Columns.Add("Rooms", typeof(string));
            table.Columns.Add("Floor", typeof(string));
            table.Columns.Add("Square Meter", typeof(string));
            table.Columns.Add("Price Per Month", typeof(string));
        }

        private void CreateCarsDataTableColumns()
        {
            //create columns of DataTable
            table.Columns.Add("Query ID", typeof(string));
            table.Columns.Add("Make", typeof(string));
            table.Columns.Add("Model", typeof(string));
            table.Columns.Add("Year", typeof(string));
            table.Columns.Add("Yad", typeof(string));
            table.Columns.Add("SMK", typeof(string));
            //table.Columns.Add("Kilometerage", typeof(string));
            table.Columns.Add("Price", typeof(string));
        }

        private void SaveTableToCSV(DataTable _data_table)
        {
            try
            {
                for (int i = 0; i < _data_table.Columns.Count; i++)
                {
                    sw.Write(_data_table.Columns[i]);
                    if (i < _data_table.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }

                sw.Write(sw.NewLine);
                foreach (DataRow dr in _data_table.Rows)
                {
                    for (int i = 0; i < _data_table.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            string value = dr[i].ToString();
                            if (value.Contains(','))
                            {
                                value = String.Format("\"{0}\"", value);
                                sw.Write(value);
                            }
                            else
                            {
                                sw.Write(dr[i].ToString());
                            }
                        }
                        if (i < _data_table.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
                table.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button_ExportToCSV_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(desktop_path + "\\Yad2");

            string output_filepath = desktop_path + "\\Yad2\\" + DateTime.Now.ToString("dd.MM.yyyy - HH;mm;ss") + ".csv";

            sw = new StreamWriter(output_filepath, true ,Encoding.UTF8);

            //sw = File.CreateText(output_filepath);       //create stream writer
            SaveTableToCSV(table);

        }

        private void button_StartFetchAppartments_Click(object sender, EventArgs e)
        {
            string url = textBox_URL.Text;

            CreateAppartmentsDataTableColumns();


        }

        private void button_StartFecthCarsYad2_Click(object sender, EventArgs e)
        {
 /*           string url = textBox_URL.Text;
            var web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);


            CreateCarsDataTableColumns();

            for (int i = 0; i < 40; i++)
            {
            }

            MessageBox.Show("Cars Yad2 Fetching finished!");
*/

        }

        private void button_StartFetch_Click(object sender, EventArgs e)
        {
            string url = textBox_URL.Text;
            TypeOfSearch type = (TypeOfSearch)comboBox_Type.SelectedIndex;
            
            switch (type)
            {
                case TypeOfSearch.Appartments_For_Sale:

                    break;
                case TypeOfSearch.Appartments_For_Rent:
                    {
                        CreateAppartmentsDataTableColumns();
                        for (int page = 0; page < Int32.Parse(textBox_Pages.Text); page++)
                        {
                            var web = new HtmlWeb();
                            HtmlAgilityPack.HtmlDocument doc = web.Load(url);

                            for (int i = 0; i < 40; i++)
                            {
                                string query_id = doc.DocumentNode.SelectNodes("//*[@id=\"feed_item_" + i + "\"]")[0].GetAttributeValue("item-id", "aaaaaa");
                                string item_url = "https://www.yad2.co.il/item/" + query_id;
                                var item_web = new HtmlWeb();
                                HtmlAgilityPack.HtmlDocument item_doc = item_web.Load(item_url);

                                //var item_page = item_doc.DocumentNode.SelectSingleNode("//*[@id=\"__layout\"]").InnerText;
                                foreach (var aa in item_doc.DocumentNode.GetAttributes())
                                    if (aa.Value == "main-title") ;


                                var item_page = item_doc.DocumentNode.GetAttributeValue("main-title", "aaaaa");
                                //var item_page = item_doc.DocumentNode.Element("h4");
                                //SelectSingleNode("//*[@class=\"main_title\"]").InnerText;

                                //*[@id="__layout"]/div/main/div/div[3]/div[5]/div/div[1]/div/div/div[2]/div[2]/div/h4
                                string _street_addr = item_doc.DocumentNode.SelectSingleNode("//*[@id=\"__layout\"]/div/main/div").InnerText;
                                //string _street_addr = item_doc.DocumentNode.SelectSingleNode("//*[@id=\"__layout\"]/div/main/div/div[3]/div[5]/div/div[1]/div/div/div[2]/div[2]/div/h4").ToString();
                                //string _street_addr = item_doc.DocumentNode.SelectNodes("//*[@id=\"__layout\"]/div/main/div/div[3]/div[5]/div/div[1]/div/div/div[2]/div[2]/div/h4")[0].InnerText;
                                string street_addr = Regex.Replace(_street_addr, @"\s+", " ");
                                string city = item_doc.DocumentNode.SelectNodes("//*[@id=\"__layout\"]/div/main/div/div[3]/div[5]/div/div[1]/div/div/div[2]/div[2]/div/span/span[2]")[0].InnerText;
                                string neighborhood = item_doc.DocumentNode.SelectNodes("//*[@id=\"__layout\"]/div/main/div/div[3]/div[5]/div/div[1]/div/div/div[2]/div[2]/div/span/span[1]")[0].InnerText;
                                string rooms = item_doc.DocumentNode.SelectNodes("//*[@id=\"__layout\"]/div/main/div/div[3]/div[5]/div/div[1]/div/div/div[2]/div[2]/div/div[1]/dl[1]/dd")[0].InnerText;
                                string floor = item_doc.DocumentNode.SelectNodes("//*[@id=\"__layout\"]/div/main/div/div[3]/div[5]/div/div[1]/div/div/div[2]/div[2]/div/div[1]/dl[2]/dd")[0].InnerText;
                                string sq_meter = item_doc.DocumentNode.SelectNodes("//*[@id=\"__layout\"]/div/main/div/div[3]/div[5]/div/div[1]/div/div/div[2]/div[2]/div/div[1]/dl[3]/dd")[0].InnerText;
                                string _price_per_month = item_doc.DocumentNode.SelectNodes("//*[@id=\"__layout\"]/div/main/div/div[3]/div[5]/div/div[1]/div/div/div[2]/div[2]/div/strong")[0].InnerText;
                                string price_per_month = Regex.Replace(_price_per_month, @"\s+", " ");


                                /* //fetching for main page:
                                string _street_addr = doc.DocumentNode.SelectNodes("//*[@id=\"feed_item_" + i + "_title\"]/span[1]")[0].InnerText;
                                string street_addr = Regex.Replace(_street_addr, @"\s+", " ");
                                string city = doc.DocumentNode.SelectNodes("//*[@id=\"feed_item_" + i + "\"]/div/div[1]/div[2]/span[2]")[0].InnerText;
                                string rooms = doc.DocumentNode.SelectNodes("//*[@id=\"data_rooms_" + i + "\"]")[0].InnerText;
                                string floor = doc.DocumentNode.SelectNodes("//*[@id=\"data_floor_" + i + "\"]")[0].InnerText;
                                string sq_meter = doc.DocumentNode.SelectNodes("//*[@id=\"data_SquareMeter_" + i + "\"]")[0].InnerText;
                                string _price_per_month = doc.DocumentNode.SelectNodes("//*[@id=\"feed_item_" + i + "_price\"]")[0].InnerText;
                                string price_per_month = Regex.Replace(_price_per_month, @"\s+", " ");
                                */
                                table.Rows.Add(query_id, street_addr, city, rooms, floor, sq_meter, price_per_month);

                            }

                            //url += "&page=" + (page + 1);
                        }
                    }
                    break;
                case TypeOfSearch.Cars_Yad2:
                    {


                        CreateCarsDataTableColumns();
                        for (int page = 0; page < Int32.Parse(textBox_Pages.Text); page++)
                        {
                            var web = new HtmlWeb();
                            HtmlAgilityPack.HtmlDocument doc = web.Load(url);


                            for (int i = 0; i < 40; i++)
                            {

                                string query_id = doc.DocumentNode.SelectNodes("//*[@id=\"feed_item_" + i + "\"]")[0].GetAttributeValue("item-id","NotExist");

                                string item_url = "https://www.yad2.co.il/item/" + query_id;
                                var item_web = new HtmlWeb();
                                HtmlAgilityPack.HtmlDocument item_doc = item_web.Load(item_url);
                                string make_and_model = doc.DocumentNode.SelectNodes("//*[@id=\"feed_item_" + i + "_title\"]/span")[0].InnerText;
                                make_and_model.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                                string info = doc.DocumentNode.SelectNodes("//*[@id=\"feed_item_" + i + "\"]/div/div[1]/div[2]/span[2]")[0].InnerText;
                                //string query_id = doc.DocumentNode.SelectNodes("//*[@id=\"accordion_wide_0\"]/div/div[3]/div[2]/span")[0].InnerText;
                                string make4 = doc.DocumentNode.SelectNodes("//*[@id=\"feed_item_" + i + "_title\"]/span[1]")[0].InnerText;
                                string model = doc.DocumentNode.SelectNodes("//*[@id=\"feed_item_" + i + "\"]/div/div[1]/div[2]/span[2]")[0].InnerText;
                                string year = doc.DocumentNode.SelectNodes("//*[@id=\"data_year_" + i + "\"]")[0].InnerText;
                                string yad = doc.DocumentNode.SelectNodes("//*[@id=\"data_hand_" + i + "\"]")[0].InnerText;
                                string smk = doc.DocumentNode.SelectNodes("//*[@id=\"data_engine_size_" + i + "\"]")[0].InnerText;
                                string _price_per_month = doc.DocumentNode.SelectNodes("//*[@id=\"feed_item_" + i + "_price\"]")[0].InnerText;
                                string price_per_month = Regex.Replace(_price_per_month, @"\s+", " ");
                                
                                table.Rows.Add(query_id, make4, model, year, yad, smk, price_per_month);
                            }

                            url += "&page=" + (page + 1);
                        }

                    }
                    break;
                default:
                    break;
            }


            MessageBox.Show("Fetching finished!");



        }
    }
}
