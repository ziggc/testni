using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Converters;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Dynamic;
using System.Data.SQLite;

namespace testni
{
    public partial class pop : Form
    {
        public pop()
        {
            InitializeComponent();
             }
        SQLiteConnection m_dbConnection;

     
        private void button7_Click(object sender, EventArgs e)
        {
            Insert();  
        }

        private void pop_Load(object sender, EventArgs e)
        {
            Refreshdele();
        }
        private async Task Refreshdele()
        {
            comboBox4.Items.Clear();
            m_dbConnection =
     new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
            string sql1 = "select * from povezava";
            SQLiteCommand command1 = new SQLiteCommand(sql1, m_dbConnection);
            SQLiteDataReader reader = command1.ExecuteReader();
            string a = "";
            while (reader.Read())
            {
                comboBox4.Items.Add("" + reader["constring"]).ToString();
            }
            m_dbConnection.Close();

        }
        private async Task Delete()
        {
            try
            {
                m_dbConnection =
new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                m_dbConnection.Open();
                string sql1 = "delete from povezava where constring like" + "'" + comboBox4.GetItemText(comboBox4.SelectedItem).ToString() + "'";
                Console.WriteLine(comboBox4.GetItemText(comboBox4.SelectedItem).ToString());
                SQLiteCommand command1 = new SQLiteCommand(sql1, m_dbConnection);
                SQLiteDataReader reader = command1.ExecuteReader();
                m_dbConnection.Close();
                 Refreshdele();
            }
            catch
            {
             

            }
        }

        private async Task Insert()
        {
            
           m_dbConnection =  new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                    m_dbConnection.Open();
                    string sql2 = "insert into povezava (constring) values ('" + textBox3.Text.ToString() + "')";
                    SQLiteCommand command2 = new SQLiteCommand(sql2, m_dbConnection);
                    command2.ExecuteNonQuery();
                    m_dbConnection.Close();
                     Refreshdele();
             


            }

        private void button9_Click(object sender, EventArgs e)
        {
            Delete();
        }
    }
}
