
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

namespace testni
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public double RequestCharge { get; }
        public string query ="";

        private const string EndpointUrl = "https://prva.documents.azure.com:443/";
        private const string PrimaryKey = "C6LMYLeDDpgCMMBeSqVL9vpfYWl6Jfxke2cgOnDOHnNZCtD4L5r4ANQX3jrlkcCnTX45nFuKn78KNHDb3gOhqw==";
        private DocumentClient client;


        private async Task GetStartedDemo()
        {
            query = textBox1.Text;
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == "FamilyDB").AsEnumerable().FirstOrDefault();
            Console.WriteLine("1. Query for a database returned: {0}", database == null ? "no results" : database.Id);
            richTextBox1.Text = "1. Query for a database returned: )";

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            // Here we find the Andersen family via its LastName
            var familyQuery = this.client.CreateDocumentQuery<Family>(
                    UriFactory.CreateDocumentCollectionUri("FamilyDB", "FamilyCollection"),(query),
                queryOptions).AsDocumentQuery();

            // The query is executed synchronously here, but can also be executed asynchronously via the IDocumentQuery<T> interface
            Console.WriteLine("Running LINQ query...");
            /*
                        foreach (Family family in (familyQuery as IEnumerable<Family>))
                        {
                            Console.WriteLine("\tRead {0}", family);
                        }

                        */
           

            double totalRU = 0.0;

            while (familyQuery.HasMoreResults)
            {
                try
                {
                    var queryResult = await familyQuery.ExecuteNextAsync();
                    totalRU += queryResult.RequestCharge;
                    //allDocuments.AddRange(queryResult.ToList());
                    break;

                }
                catch
                (Exception e)
                {
                    Console.WriteLine(e);
                    
                }
            }
            Console.WriteLine(totalRU+"");
            label1.Text = totalRU + "";
            

                /*
                try
                {
                    ResourceResponse<Document> response = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri("FamilyDB", "FamilyCollection", "Andersen.1"));
                    Console.WriteLine("ffff");
                    // Console.WriteLine(response.Document.CustomerName);
                    Console.WriteLine(response.RequestCharge);
                    Console.WriteLine(response.ActivityId);
                    Console.WriteLine(response.StatusCode);
                    // HttpStatusCode.Created or 201


                }
                catch
                {
                    Console.WriteLine("ffff");

                }*/



            }

        //End WTCPTC
        public class Family
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }
            public string LastName { get; set; }
            public Parent[] Parents { get; set; }
            public Child[] Children { get; set; }
            public Address Address { get; set; }
            public bool IsRegistered { get; set; }
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        public class Parent
        {
            public string FamilyName { get; set; }
            public string FirstName { get; set; }
        }

        public class Child
        {
            public string FamilyName { get; set; }
            public string FirstName { get; set; }
            public string Gender { get; set; }
            public int Grade { get; set; }
            public Pet[] Pets { get; set; }
        }

        public class Pet
        {
            public string GivenName { get; set; }
        }

        public class Address
        {
            public string State { get; set; }
            public string County { get; set; }
            public string City { get; set; }
        }



        private void button1_Click(object sender, EventArgs e)
        {
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
                Console.WriteLine("End of demo, press any key to exit.");

            }
        }
    }
}

