﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace Okazje
{
    public partial class Form1 : Form
    {
        Okazje.Klasy.OperacjeBazyDanych Baza = new Klasy.OperacjeBazyDanych();
        string command;
        List<string> CommandFields = new List<string>();
        DataTable gt_CommandReturn = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }
        private void clearVariables()
        {
            command = "";
            CommandFields.Clear();
            gt_CommandReturn.Clear();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            getCategories();
        }
        private void insertCategories(DataTable categories)
        {
            clearVariables();
            command = "select * from kategorie";
            //command = "select MAX(numerKategorii) from KATEGORIE";
            CommandFields.Add("numerKategorii");
            DataTable lt_result = new DataTable();
            lt_result.Columns.Add("numerKategorii");
            lt_result = Baza.Selection(command, CommandFields);
            var lv_max_index_kategoria = lt_result.Rows[0][0].ToString();

            MessageBox.Show(lv_max_index_kategoria);

            clearVariables();

        }
        private void getCategories()
        {
            DataTable lt_categories = new DataTable();
            lt_categories.Columns.Add("Kategoria");

            var url = "https://www.x-kom.pl/";
            var htmlCode = "";
            using (WebClient client = new WebClient()) { htmlCode = client.DownloadString(url); }
            Regex rx = new Regex(@"href=\""(.*?)\""", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            MatchCollection matches = rx.Matches(htmlCode);
            foreach (Match m in matches)
            {
                if (m.Value.Contains("/g-"))
                    lt_categories.Rows.Add(url + m.Value.Substring(7, m.Value.Length - 7));
            }
            insertCategories(lt_categories);
        }
        private void getProductsFromCategoties(DataTable categories)
        {
            DataTable lt_ProductsFromCat = new DataTable();
            lt_ProductsFromCat.Columns.Add("Url");

            var url = "https://www.x-kom.pl/";
            var htmlCode = "";

            foreach (DataRow row in categories.Rows)
            {
                var categoryURL = row[0].ToString();
                using (WebClient client = new WebClient()) { htmlCode = client.DownloadString(categoryURL); }

                Regex rx = new Regex(@"href=\""(.*?)\""", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = rx.Matches(htmlCode);
                foreach (Match m in matches)
                {
                    if (m.Value.Contains("/p/"))
                        if (m.Value.Contains("www.x-kom.p"))
                        {
                            var val = m.Value.Substring(6, m.Value.Length - 7);
                            lt_ProductsFromCat.Rows.Add(val);
                        }
                        else
                        {
                            var val = url + m.Value.Substring(7, m.Value.Length - 7);
                            lt_ProductsFromCat.Rows.Add(val);
                        }
                }
            }
        }
    }
}
