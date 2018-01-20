using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;

namespace HttpClientSample2
{
  //  https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client

    //our data class
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
    
    public class Program
    {
        //if its not statick we can exhaust the available sockets
        //za tuy si polzvame edna instancia prez cialoto vreme
        static HttpClient client = new HttpClient();

        //initializes the HttpClient instance:
        static async Task RunAsync()
        {
            //Sets the base URI for HTTP requests. Change the port number to the port
            //used in the server app.The app won't work unless port for the server app
            //is used.
          
            client.BaseAddress = new Uri("http://localhost:64195/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                
                );
        }

        //Send a GET request to retrieve a resource
        static async Task<Product> GetProductAsync(string path)
        {
            Product product = null;

            //contains the HTTP response
            HttpResponseMessage response = await client.GetAsync(path); //proveriavame s tuy dali mojem da vzemem resursa

            if (response.IsSuccessStatusCode)
            {
                //if its success the body of the response have the JSON representation of a product
                product = await response.Content.ReadAsAsync<Product>(); //vzimazme contenta na datata
            }

            return product;
        }

        //sending post request to create product
        //sends a POST request that contains a Product instance in JSON format
        static async Task<Uri> CreateProductAsync(Product product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/products", product);
            response.EnsureSuccessStatusCode(); //status code 201 created

            //// return URI of the created resource.
            //The response should include the URL of the created resources in the Location header.
            return response.Headers.Location;
        }

        //sending PUT (update)
        static async Task<Product>  UpdateProductAsync(Product product)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync<Product>(
                $"api/products/{product.Id}", product
                );

            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            product = await response.Content.ReadAsAsync<Product>();
            return product;
        }

        //sending delete
        static async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/products/{id}"
                );
            return response.StatusCode;
        }
    }
}
