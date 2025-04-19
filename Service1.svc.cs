using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Specialized;

namespace ShopEase_Backend
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public void signup(string name, string email, string password, string role)
        {
            string connectionString = "Data Source=DESKTOP-8G0F3Q1;Initial Catalog=ShopEase;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (Name, Email, Password, Role) VALUES (@Name, @Email, @Password, @Role)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public User login(string email,  string password)
        {
            string connectionString = "Connection String";
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        User user = new User();
                        while (reader.Read())
                        {
                            user.email = reader["Email"].ToString();
                            user.password = reader["Password"].ToString();
                            user.name = reader["Name"].ToString();
                            user.role = reader["Role"].ToString();
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

        public List<Product> getProducts()
        {
            List<Product> productList = new List<Product>();

            string connectionString = "Data Source=DESKTOP-8G0F3Q1;Initial Catalog=ShopEase;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Product product = new Product();
                        product.name = reader["Name"].ToString();
                        product.description = reader["Description"].ToString();
                        product.category = reader["Category"].ToString();
                        product.price = reader["Price"].ToString();
                        product.image = reader["Image"].ToString();
                        product.seller = reader["Seller"].ToString();
                        product.rating = reader["Rating"].ToString();
                        productList.Add(product);
                    }
                }
            }
            return productList;
        }

        public Product GetProduct(string name)
        {
            string connectionString = "Data Source=DESKTOP-8G0F3Q1;Initial Catalog=ShopEase;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products WHERE Name = @Name";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        Product product = new Product();
                        while (reader.Read())
                        {
                            product.name = reader["Name"].ToString();
                            product.description = reader["Description"].ToString();
                            product.category = reader["Category"].ToString();
                            product.price = reader["Price"].ToString();
                            product.image = reader["Image"].ToString();
                            product.seller = reader["Seller"].ToString();
                            product.rating = reader["Rating"].ToString();
                        }
                        return product;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void addProduct(string name, string description, string category, string price, string image, string seller, string rating)
        {
            string connectionString = "Data Source=DESKTOP-8G0F3Q1;Initial Catalog=ShopEase;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Products (Name, Description, Category, Price, Image, Seller, Rating) VALUES (@Name, @Description, @Category, @Price, @Image, @Seller, @Rating)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Image", image);
                    command.Parameters.AddWithValue("@Seller", seller);
                    command.Parameters.AddWithValue("@Rating", rating);
                    connection.Open();
                    command.ExecuteNonQuery();
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
