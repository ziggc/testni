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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            




            richTextBox2.Text = "Select * from c";
            if (radioButton1.Checked==true)
            {
                radioButton3.Checked = false;
                radioButton2.Checked = false;


            }
            else if (radioButton2.Checked == true)
                {
                radioButton1.Checked = false;
                radioButton3.Checked = false;


            }
            else if (radioButton3.Checked == true)
            {
                radioButton2.Checked = false;
                radioButton1.Checked = false;


            }


        }
                //conection database******************Default
        public string DatabaseId = "";
        public string CollectionID = "";
        public string DocomentId = "";

        public int stdel = 1;

        //*****end***
        public double RequestCharge { get; }
        public string query ="";
        //default value connection string;
        public  string EndpointUrl = "";
        public  string PrimaryKey = "";
        public double totalRU;
        private DocumentClient client;
        SQLiteConnection m_dbConnection;


        //***
        private async Task Doccall()   //Docoment lista!
        {


            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            FeedOptions options = new FeedOptions { MaxItemCount = 1 };
            var queryable = client.CreateDocumentQuery(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionID), options)
                    .AsDocumentQuery();

            List<Document> list = new List<Document>();               
            while (queryable.HasMoreResults)
            {
                foreach (Document entry in await queryable.ExecuteNextAsync())
                {
                    list.Add(entry);
                }

            }

            comboBox3.DataSource = list;
            comboBox3.DisplayMember = "id";
           DocomentId = comboBox3.GetItemText(this.comboBox3.SelectedItem);
            if (comboBox3.Text.Equals("Docoment ID") == false)
            {
                textBox2.Text = "Status: Everyting working";
                //textBox2.BackColor = Color.Green;
            }
            else
            {
                textBox2.Text = "Status: Program getting ready.!";
                //textBox2.BackColor = Color.Red;

            }
        }
            private async Task Colecall()   //colletion list!
        {


            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            //***
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == DatabaseId).AsEnumerable().FirstOrDefault();

            var collections = client.CreateDocumentCollectionQuery(database.SelfLink).ToArray();
            
            //**
            string continuation = string.Empty;
            List<Database> databases = new List<Database>();
            do
            {
                // Read the feed 10 items at a time until there are no more items to read
                FeedResponse<Database> response = await client.ReadDatabaseFeedAsync(new FeedOptions
                {
                    MaxItemCount = -1,
                    RequestContinuation = continuation

                });
               collections= client.CreateDocumentCollectionQuery(database.SelfLink).ToArray();
                databases.AddRange(response.AsEnumerable<Database>());


                // Get the continuation so that we know when to stop.
                continuation = response.ResponseContinuation;
            } while (!string.IsNullOrEmpty(continuation));

            comboBox2.DataSource = collections;
            comboBox2.DisplayMember = "id";
            CollectionID=comboBox2.GetItemText(this.comboBox2.SelectedItem);
    Doccall();
          
        }
              

        private async Task Databasecall()    ///database list
        { 
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            string continuation = string.Empty;
            List<Database> databases = new List<Database>();
            do
            {
                // Read the feed 10 items at a time until there are no more items to read
                FeedResponse<Database> response = await client.ReadDatabaseFeedAsync(new FeedOptions
                {
                    MaxItemCount = -1,
                    RequestContinuation = continuation
                });

                databases.AddRange(response.AsEnumerable<Database>());

                // Get the continuation so that we know when to stop.
                continuation = response.ResponseContinuation;
            } while (!string.IsNullOrEmpty(continuation));
            comboBox1.DataSource = databases;
            comboBox1.DisplayMember = "id";
            DatabaseId =comboBox1.GetItemText(comboBox1.SelectedItem); ;
            Colecall();


        }
        private async Task GetStartedDemo()
        {
            query = richTextBox2.Text;
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == DatabaseId).AsEnumerable().FirstOrDefault();
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            var result = this.client.CreateDocumentQuery(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionID), (query),
                queryOptions).AsDocumentQuery();

            totalRU = 0.0;

            while (result.HasMoreResults)
            {
                foreach (Document entry in await result.ExecuteNextAsync())
                {
                    richTextBox1.Text = richTextBox1.Text + entry.ToString();
                }

                try
                {
                    var queryResult = await result.ExecuteNextAsync();
                    totalRU += queryResult.RequestCharge;
                    textBox2.Text = "Status: Everyting working";
                   // textBox2.BackColor=Color.Green;
                        break;

                }
                catch
                (Exception e)
                {
                    Console.WriteLine(e);
                    textBox2.Text = "Status: Something went wrong!";
                    //textBox2.BackColor = Color.Red;

                }
            }
 
            label1.Text = totalRU + " RU";
            label5.Text = totalRU + " RU";
            

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox2.Text.Equals("") == true)
            {
                MessageBox.Show("Enter Query");

            }
            else { 
                label1.Text = "Calc RU";
                try
                {
                    GetStartedDemo();
                }
                catch (DocumentClientException de)
                {
                    Exception baseException = de.GetBaseException();
                    Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                }
                catch (Exception a)
                {
                    Exception baseException = a.GetBaseException();
                    Console.WriteLine("Error: {0}, Message: {1}", a.Message, baseException.Message);
                }
                finally
                {
                    Console.WriteLine("exit.");

                }  // call metode demo. glavna metoda
        }

        }//Gumb 1 sproži celoten postopek 
         

        private void richTextBox2_MouseClick(object sender, MouseEventArgs e)
        {

            if (richTextBox2.Text.Equals("Enter QUERY!") == true)
            {
                
                richTextBox2.Text = "";

            }
           
        } // howertext
              
        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                textBox2.Text = "Connecting...";
                //textBox2.BackColor = Color.Red;
                int i = 0;
                string umesni = comboBox4.GetItemText(this.comboBox4.SelectedItem).ToString();
                int druga = 0;

                int drugaz = 0;

                do
                {
                    if (umesni.Substring(0, 17).Equals("AccountEndpoint ="))
                    {
                    }
                    else if (umesni[i].Equals(';') == true && drugaz < 1)
                    {
                        drugaz++;
                        druga = i;
                    }


                    i++;
                } while (umesni.Length != i);
                EndpointUrl = umesni.Substring(16, druga - 16);

                PrimaryKey = umesni.Substring(druga + 12, umesni.Length - druga - 12);



                Databasecall();
            }
            catch
            {
                textBox2.Text = "Nothing Selected";
                //textBox2.BackColor = Color.Red;

            }
            //        int i = 0;
            //        string umesni = textBox4.Text ;
            //        int druga = 0;

            //        int drugaz =0;
            //        try
            //        {
            //            do
            //            {
            //                if (umesni.Substring(0, 17).Equals("AccountEndpoint ="))
            //                {
            //                   }
            //               else if(umesni[i].Equals(';')==true && drugaz<1)
            //                {
            //                    drugaz++;
            //                    druga = i;
            //                }


            //                i++;
            //            } while (umesni.Length != i);
            //   EndpointUrl = umesni.Substring(16, druga - 16);

            //    PrimaryKey = umesni.Substring(druga + 12, umesni.Length - druga - 13);
            //            richTextBox1.Text = EndpointUrl;
            //            richTextBox1.Text = richTextBox1 + "\n" + PrimaryKey;


            //}
            //        catch (Exception b)
            //        {


            //        }
            //        Databasecall();
        }//String key to 2 keys

 // Checkbox parametrs


        public void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Text = "Status: Program getting ready.!";
            //textBox2.BackColor = Color.Red;
            Refreshdele();


            ////Databasecall();


        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          DatabaseId = comboBox1.GetItemText(this.comboBox1.SelectedItem);
         Colecall();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CollectionID = comboBox2.GetItemText(this.comboBox2.SelectedItem);
            //Doccall();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int i = int.Parse(textBox1.Text);
                if (i == 0)
                {
                    double totalRUmult = totalRU * (i+1);
                    label5.Text = totalRUmult + "RU";
                }
                else
                {
                    double totalRUmult = totalRU * i;
                    label5.Text = totalRUmult + "RU";
                }
                
            } catch { }
           
        }
        public async Task CreateDoc()  //create RU 
        {
            try
            {
                this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
                var oMycustomclassname1 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(richTextBox1.Text);
                var collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionID);
                var result = await client.CreateDocumentAsync(collectionUri, new { oMycustomclassname1 });
                Console.WriteLine($"RU used: {result.RequestCharge}");
                totalRU = result.RequestCharge;
                label1.Text = totalRU.ToString();
                textBox2.Text = "Status: Everyting working";
                //textBox2.BackColor = Color.Green;
            }
            catch
            {
                textBox2.Text = "Status: Bad data.!";
                //textBox2.BackColor = Color.Red;
            }
            

        }
        public async Task CreateUpdatec()  //Update RU 
        {
            try
            {
    this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            var oMycustomclassname1 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(richTextBox1.Text);
            var result = await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionID, DocomentId), oMycustomclassname1);
            
            totalRU = result.RequestCharge;
            label1.Text = totalRU.ToString();
                textBox2.Text = "Status: Everyting working";
                //textBox2.BackColor = Color.Green;
            }
            catch
            {
                textBox2.Text = "Status: Bad data.!";
                //textBox2.BackColor = Color.Red;
            }
        
        }

            private void button3_Click(object sender, EventArgs e)
        {

            CreateDoc();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            CreateUpdatec();

           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                try
                {
                    this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
                    this.client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
                    textBox2.Text = "Status: Deleted // " + stdel;
                    //textBox2.BackColor = Color.Green;
                    stdel++;
                }
                catch
                {
                    textBox2.Text = "Status: Not deleted.!";
                    //textBox2.BackColor = Color.Red;
                }

            }
            else if (radioButton2.Checked == true)
            {
                try
                {
                    this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
                    this.client.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionID));
                    textBox2.Text = "Status: Deleted // " + stdel;
                    //textBox2.BackColor = Color.Green;
                    stdel++;
    }
                catch
                {
                    textBox2.Text = "Status: Not deleted.!";
                    //textBox2.BackColor = Color.Red;
                }

            }
           else if (radioButton1.Checked == true)
            {
                try
                {
                    this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
                    this.client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionID,DocomentId));
                    textBox2.Text = "Status: Deleted // " + stdel;
                    //textBox2.BackColor = Color.Green;
                    stdel++;
                }
                catch
                {
                    textBox2.Text = "Status: Not deleted.!";
                    //textBox2.BackColor = Color.Red;
                }

            }
            else
            {

                MessageBox.Show("Nothing selected to delete!!!");
            }
           
    
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }



        //private void button7_Click(object sender, EventArgs e)
        //{
        //    try {
        //        m_dbConnection =
        //  new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
        //        m_dbConnection.Open();
        //          string sql2 = "insert into povezava (constring) values ('" + textBox3.Text.ToString() + "')";
        //        SQLiteCommand command2 = new SQLiteCommand(sql2, m_dbConnection);
        //        command2.ExecuteNonQuery();
        //        m_dbConnection.Close();
        //        textBox2.Text = "String Added";
        //        //textBox2.BackColor = Color.Green;
        //        Refreshdele();
        //    } catch
        //    {
        //        textBox2.Text = "Enter string";
        //        //textBox2.BackColor = Color.Red;

        //    }
            
           
        //}

        private void button8_Click(object sender, EventArgs e)
        {



        }

        private void button9_Click(object sender, EventArgs e)
        {
            try {
                                m_dbConnection =
    new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                m_dbConnection.Open();
                string sql1 = "delete from povezava where constring like" + "'" + comboBox4.GetItemText(comboBox4.SelectedItem).ToString() + "'";
                Console.WriteLine(comboBox4.GetItemText(comboBox4.SelectedItem).ToString());
                SQLiteCommand command1 = new SQLiteCommand(sql1, m_dbConnection);
                SQLiteDataReader reader = command1.ExecuteReader();
                m_dbConnection.Close();
                textBox2.Text = "Deleted";
                //textBox2.BackColor = Color.Green;
                Refreshdele();
            } catch
            {
                textBox2.Text = "Not Deleted";
                //textBox2.BackColor = Color.Red;
                
            }
        

        }


        private void button6_Click(object sender, EventArgs e)
        {
   
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

        private void label1_TextChanged(object sender, EventArgs e)
        {
            label5.Text = label1.Text;
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
         new pop().Show();
        }

        private void comboBox4_DropDown(object sender, EventArgs e)
        {
            Refreshdele();
        }
    }
}

