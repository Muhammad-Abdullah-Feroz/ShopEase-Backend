using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace ShopEase_Backend
{
    public class User
    {
        public string id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string role { get; set; }

        public void setUser(string id, string email, string password, string name, string role)
        {
            this.id = id;
            this.email = email;
            this.password = password;
            this.name = name;
            this.role = role;
        }

        public void addToCart(string product, int count)
        {
            string ConnectionString = "Connection String";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "INSERT INTO Cart (Email, Product, count) VALUES (@Email , @product, @count )";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", this.email);
                    command.Parameters.AddWithValue("@Product", product);
                    command.Parameters.AddWithValue("@count", count);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}