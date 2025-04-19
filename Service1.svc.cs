using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Specialized;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using System.Drawing;
using System.IO;

namespace ShopEase_Backend
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string signup(string name, string email, string password, string role)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Users (name, email_or_phone, password, role) VALUES (@name, @emailOrPhone, @password, @role)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@emailOrPhone", email);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@role", role);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return "Success";
                        }
                        else
                        {
                            return "Failure";
                        }
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                // Handle SQL exceptions (e.g., connection issues, constraint violations)
                return $"SQL Error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                // Handle any general exceptions
                return $"Error: {ex.Message}";
            }
        }


        public User login(string email,  string password)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Users WHERE email_or_phone = @Email AND password = @Password";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            User user = new User();
                            while (reader.Read())
                            {
                                user.id = reader["user_id"].ToString();
                                user.email = reader["email_or_phone"].ToString();
                                user.password = reader["password"].ToString();
                                user.name = reader["name"].ToString();
                                user.role = reader["role"].ToString();
                                
                            }
                            return user;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

        }

        public void updateUser(string email, string name, string password, string role)
        {
            string connectionString = "Data Source=DESKTOP-8G0F3Q1;Initial Catalog=ShopEase;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Users SET Name = @Name, Password = @Password, Role = @Role WHERE Email = @Email";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        //public List<Product> getProducts()
        //{
        //    List<Product> productList = new List<Product>();

        //    string connectionString = "Data Source=DESKTOP-8G0F3Q1;Initial Catalog=ShopEase;Integrated Security=True";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT * FROM Products";
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                Product product = new Product();
        //                product.name = reader["Name"].ToString();
        //                product.description = reader["Description"].ToString();
        //                product.category = reader["Category"].ToString();
        //                product.price = reader["Price"].ToString();
        //                product.image = reader["Image"].ToString();
        //                product.seller = reader["Seller"].ToString();
        //                product.rating = reader["Rating"].ToString();
        //                productList.Add(product);
        //            }
        //        }
        //    }
        //    return productList;
        //}

        //public Product GetProduct(string name)
        //{
        //    string connectionString = "Data Source=DESKTOP-8G0F3Q1;Initial Catalog=ShopEase;Integrated Security=True";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT * FROM Products WHERE Name = @Name";
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Name", name);
        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            if (reader.HasRows)
        //            {
        //                Product product = new Product();
        //                while (reader.Read())
        //                {
        //                    product.name = reader["Name"].ToString();
        //                    product.description = reader["Description"].ToString();
        //                    product.category = reader["Category"].ToString();
        //                    product.price = reader["Price"].ToString();
        //                    product.image = reader["Image"].ToString();
        //                    product.seller = reader["Seller"].ToString();
        //                    product.rating = reader["Rating"].ToString();
        //                }
        //                return product;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //}

        public string addProduct(string name, string description, decimal price, byte[] imageData, int sellerID, int rating, int quantity, bool rentable, decimal price_per_day)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "INSERT INTO Products (seller_id, name, description, price, quantity, image, is_for_rent, per_day_price) VALUES (@sellerID, @name, @description, @price, @quantity, @image, @rentable, @perDayPrice)";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@price", price);

                    // Convert Image to byte array for storing into BLOB
                    
                    command.Parameters.AddWithValue("@image", imageData);
                    

                    command.Parameters.AddWithValue("@sellerID", sellerID);
                    command.Parameters.AddWithValue("@rating", rating);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.AddWithValue("@rentable", rentable);
                    command.Parameters.AddWithValue("@perDayPrice", price_per_day); // Assuming per day price is calculated as monthly price divided by 30

                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                        return "Success";
                    }
                    catch (MySqlException ex)
                    {
                        return ex.Message;
                    }
                }
            }
        }


        public bool deleteProduct(string name)
        {
            string connectionString = "Data Source=DESKTOP-8G0F3Q1;Initial Catalog=ShopEase;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Products WHERE Name = @Name";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public void addToCart(string email, string product, int count)
        {
            string ConnectionString = "Connection String";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "INSERT INTO Cart (Email, Product, count) VALUES (@Email , @product, @count )";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Product", product);
                    command.Parameters.AddWithValue("@count", count);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }





        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
