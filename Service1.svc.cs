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
using Org.BouncyCastle.Utilities;
using MySqlX.XDevAPI;
using System.Configuration;

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
                    MySqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Step 1: Insert the user
                        string insertUserQuery = @"
                    INSERT INTO Users (name, email_or_phone, password, role) 
                    VALUES (@name, @emailOrPhone, @password, @role);
                    SELECT LAST_INSERT_ID();";

                        int newUserId;
                        using (MySqlCommand cmd = new MySqlCommand(insertUserQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.Parameters.AddWithValue("@emailOrPhone", email);
                            cmd.Parameters.AddWithValue("@password", password);
                            cmd.Parameters.AddWithValue("@role", role);

                            object userIdObj = cmd.ExecuteScalar();
                            newUserId = Convert.ToInt32(userIdObj);
                        }

                        // Step 2: If user is a buyer, create a cart
                        if (role.ToLower() == "buyer")
                        {
                            string insertCartQuery = "INSERT INTO Cart (buyer_id) VALUES (@buyerId)";
                            using (MySqlCommand cmd = new MySqlCommand(insertCartQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@buyerId", newUserId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return $"Error: {ex.Message}";
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                return $"SQL Error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
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

        public bool ResetPassword(string id, string password)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE Users SET password = @Password WHERE user_id = @UserId";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@UserId", id);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        public bool add_user_image(int userId, byte[] image)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE Users SET image = @Image WHERE user_id = @UserId";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Image", image);
                    command.Parameters.AddWithValue("@UserId", userId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // If at least 1 row updated, return true
                }
            }
        }

        public bool Add_user(string name, string email, string password, string role, byte[] image)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"INSERT INTO Users (name, email_or_phone, password, role, image)
                         VALUES (@Name, @Email, @Password, @Role, @Image)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Image", image ?? (object)DBNull.Value); 

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Users";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User();
                            user.id = reader["user_id"].ToString();
                            user.email = reader["email_or_phone"].ToString();
                            user.password = reader["password"].ToString();
                            user.name = reader["name"].ToString();
                            user.role = reader["role"].ToString();
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }

        public bool deleteUser(string id)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "DELETE FROM Users WHERE user_id = @UserId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", id);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public byte[] get_dp(string id)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT image FROM Users WHERE user_id = @UserId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", id);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        return (byte[])result;
                    }
                }
            }
            return null;
        }

        public List<Product> GetProducts()
        {
            List<Product> products = new List<Product>();
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT p.product_id, u.name AS seller_name, p.name, p.description, 
                                p.price, p.quantity, p.is_for_rent
                         FROM Products p
                         JOIN Users u ON p.seller_id = u.user_id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductId = Convert.ToInt32(reader["product_id"]),
                                seller_name = reader["seller_name"].ToString(), // Store seller name here
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                Price = Convert.ToDecimal(reader["price"]),
                                Quantity = Convert.ToInt32(reader["quantity"]),
                                IsForRent = Convert.ToBoolean(reader["is_for_rent"])
                            };

                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        public List<Product> GetSellerProducts(int sellerId)
        {
            List<Product> products = new List<Product>();
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT product_id, name, description, price, quantity, is_for_rent, per_day_price FROM Products WHERE seller_id = @SellerId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SellerId", sellerId);
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.ProductId = Convert.ToInt32(reader["product_id"]);
                            product.Name = reader["name"].ToString();
                            product.Description = reader["description"].ToString();
                            product.Price = Convert.ToDecimal(reader["price"]);
                            product.Quantity = Convert.ToInt32(reader["quantity"]);
                            product.IsForRent = Convert.ToBoolean(reader["is_for_rent"]);
                            product.PerDayPrice = reader["per_day_price"] != DBNull.Value ? Convert.ToDecimal(reader["per_day_price"]) : 0;

                            //if (reader["image"] != DBNull.Value)
                            //{
                            //    byte[] imageBytes = (byte[])reader["image"];
                            //    using (MemoryStream ms = new MemoryStream(imageBytes))
                            //    {
                            //        product.Image = Image.FromStream(ms);
                            //    }
                            //}

                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        public bool UpdateProduct(int productId, string name, string description, decimal price, int quantity, bool isForRent, decimal? perDayPrice)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";
            bool success = false;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"
                UPDATE Products 
                SET 
                name = @Name,
                description = @Description,
                price = @Price,
                quantity = @Quantity,
                is_for_rent = @IsForRent,
                per_day_price = @PerDayPrice
                WHERE product_id = @ProductId";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@IsForRent", isForRent);

                    if (perDayPrice.HasValue)
                        command.Parameters.AddWithValue("@PerDayPrice", perDayPrice.Value);
                    else
                        command.Parameters.AddWithValue("@PerDayPrice", DBNull.Value);

                    command.Parameters.AddWithValue("@ProductId", productId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    success = rowsAffected > 0;
                }
            }

            return success;
        }

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

        public List<Product> GetProductsForUser()
        {
            List<Product> products = new List<Product>();
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            p.product_id, 
            p.name, 
            p.description,
            p.price, 
            p.quantity,
            p.is_for_rent,
            p.per_day_price,
            p.image, 
            u.name AS seller_name
        FROM 
            products p
        JOIN 
            users u ON p.seller_id = u.user_id
        WHERE 
            p.quantity > 0;
        ";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product p = new Product();

                            p.ProductId = reader["product_id"] != DBNull.Value ? Convert.ToInt32(reader["product_id"]) : 0;
                            p.Name = reader["name"]?.ToString();
                            p.Description = reader["description"]?.ToString();
                            p.Price = reader["price"] != DBNull.Value ? Convert.ToDecimal(reader["price"]) : 0;
                            p.Quantity = reader["quantity"] != DBNull.Value ? Convert.ToInt32(reader["quantity"]) : 0;
                            p.IsForRent = reader["is_for_rent"] != DBNull.Value ? Convert.ToBoolean(reader["is_for_rent"]) : false;
                            p.PerDayPrice = reader["per_day_price"] != DBNull.Value ? Convert.ToDecimal(reader["per_day_price"]) : 0;
                            p.seller_name = reader["seller_name"]?.ToString();
                            p.image = reader["image"] != DBNull.Value ? (byte[])reader["image"] : null;

                            products.Add(p);
                        }
                    }
                }
            }

            return products;
        }


        public int getProductRatings(int productId)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT AVG(rating) FROM reviews WHERE product_id = @ProductId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        return Convert.ToInt32(Math.Round(Convert.ToDouble(result)));
                    }
                }
            }
            return 0;
        }
        public bool deleteProduct(string id)
        {
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))  // Use MySqlConnection instead of SqlConnection
            {
                string query = "DELETE FROM Products WHERE product_id = @ProductId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", id);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool AddProductToCart(int buyerId, int productId)
        {
            try
            {
                string connStr = "server=localhost;uid=root;pwd=password;database=Shopease;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    // Get the user's cart_id
                    string cartQuery = "SELECT cart_id FROM cart WHERE buyer_id = @buyerId";
                    MySqlCommand cartCmd = new MySqlCommand(cartQuery, conn);
                    cartCmd.Parameters.AddWithValue("@buyerId", buyerId);
                    object cartIdObj = cartCmd.ExecuteScalar();

                    if (cartIdObj != null)
                    {
                        int cart_id = Convert.ToInt32(cartIdObj);

                        // Insert product to cart_items
                        string insertQuery = @"
                    INSERT INTO cart_items (cart_id, product_id, quantity)
                    VALUES (@cartId, @productId, 1)";
                        MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@cartId", cart_id);
                        insertCmd.Parameters.AddWithValue("@productId", productId);
                        insertCmd.ExecuteNonQuery();

                        // Decrease product quantity in product table
                        string updateQuery = @"
                    UPDATE Products
                    SET quantity = quantity - 1
                    WHERE product_id = @productId AND quantity > 0";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@productId", productId);
                        updateCmd.ExecuteNonQuery();

                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }


        public List<cart_item> GetCartItemsForBuyer(int buyerId)
        {
            List<cart_item> items = new List<cart_item>();
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT 
                p.product_id,
                p.name,
                p.price,
                p.image,
                ci.quantity
            FROM cart_items ci
            JOIN cart c ON ci.cart_id = c.cart_id
            JOIN products p ON ci.product_id = p.product_id
            WHERE c.buyer_id = @buyerId;
        ";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@buyerId", buyerId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cart_item item = new cart_item
                            {
                                ProductId = Convert.ToInt32(reader["product_id"]),
                                ProductName = reader["name"].ToString(),
                                ProductPrice = Convert.ToDecimal(reader["price"]),
                                Quantity = Convert.ToInt32(reader["quantity"]),
                                Image = reader["image"] != DBNull.Value ? (byte[])reader["image"] : null
                            };

                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        public string PlaceOrderForBuyOnly(int buyerId, string address, string phoneNumber, string paymentMethod, string cardNo)
        {
            string connStr = "server=localhost;uid=root;pwd=password;database=Shopease;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Get cart_id
                        int cartId = 0;
                        string getCartIdQuery = "SELECT cart_id FROM cart WHERE buyer_id = @buyerId;";
                        using (MySqlCommand cmd = new MySqlCommand(getCartIdQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@buyerId", buyerId);
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                                cartId = Convert.ToInt32(result);
                            else
                                return "Cart not found for the buyer.";
                        }

                        // Step 2: Insert into orders
                        long orderId;
                        string insertOrderQuery = @"
                    INSERT INTO orders (buyer_id, address, phone_number, payment_method, card_number, status)
                    VALUES (@buyerId, @address, @phone, @payment, @card, 'Processing');";
                        using (MySqlCommand cmd = new MySqlCommand(insertOrderQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@buyerId", buyerId);
                            cmd.Parameters.AddWithValue("@address", address);
                            cmd.Parameters.AddWithValue("@phone", phoneNumber);
                            cmd.Parameters.AddWithValue("@payment", paymentMethod);
                            cmd.Parameters.AddWithValue("@card", cardNo);
                            cmd.ExecuteNonQuery();
                            orderId = cmd.LastInsertedId;
                        }

                        // Step 3: Get only NON-RENT cart items
                        List<(int productId, int quantity, decimal price)> items = new List<(int, int, decimal)>();
                        string getCartItemsQuery = @"
                            SELECT ci.product_id, ci.quantity, p.price
                            FROM cart_items ci
                            JOIN products p ON ci.product_id = p.product_id
                            WHERE ci.cart_id = @cartId;";
                        using (MySqlCommand cmd = new MySqlCommand(getCartItemsQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@cartId", cartId);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    items.Add((
                                        Convert.ToInt32(reader["product_id"]),
                                        Convert.ToInt32(reader["quantity"]),
                                        Convert.ToDecimal(reader["price"])
                                    ));
                                }
                            }
                        }

                        if (items.Count == 0)
                        {
                            return "No products found in the cart.";
                        }


                        // Step 4: Insert into order_items and reduce quantity
                        foreach (var item in items)
                        {
                            string insertItemQuery = @"
                        INSERT INTO order_items (order_id, product_id, quantity, price_per_item, is_rented, rent_days)
                        VALUES (@orderId, @productId, @qty, @price, 0, 0);";
                            using (MySqlCommand cmd = new MySqlCommand(insertItemQuery, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@orderId", orderId);
                                cmd.Parameters.AddWithValue("@productId", item.productId);
                                cmd.Parameters.AddWithValue("@qty", item.quantity);
                                cmd.Parameters.AddWithValue("@price", item.price);
                                cmd.ExecuteNonQuery();
                            }

                            string updateProductQty = "UPDATE products SET quantity = quantity - @qty WHERE product_id = @productId;";
                            using (MySqlCommand cmd = new MySqlCommand(updateProductQty, conn, trans))
                            {
                                cmd.Parameters.AddWithValue("@qty", item.quantity);
                                cmd.Parameters.AddWithValue("@productId", item.productId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Step 5: Remove NON-RENT items from cart
                        string deleteCartItems = @"
                    DELETE FROM cart_items WHERE cart_id = @cartId;;";
                        using (MySqlCommand cmd = new MySqlCommand(deleteCartItems, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@cartId", cartId);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return "Order placed successfully.";
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return "Error placing order: " + ex.Message;
                    }
                }
            }
        }

        public string RentSingleProductFromCart(int buyerId, int productId, int rentDays, string address, string phoneNumber, string paymentMethod, string cardNo)
        {
            string connStr = "server=localhost;uid=root;pwd=password;database=Shopease;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Get cart_id
                        int cartId = 0;
                        string getCartIdQuery = "SELECT cart_id FROM cart WHERE buyer_id = @buyerId;";
                        using (MySqlCommand cmd = new MySqlCommand(getCartIdQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@buyerId", buyerId);
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                                cartId = Convert.ToInt32(result);
                            else
                                return "Cart not found.";
                        }

                        // Step 2: Get product quantity & price from cart
                        int quantity = 0;
                        decimal price = 0;
                        string getProductQuery = @"
                    SELECT ci.quantity, p.price
                    FROM cart_items ci
                    JOIN products p ON ci.product_id = p.product_id
                    WHERE ci.cart_id = @cartId AND ci.product_id = @productId AND p.is_for_rent = 1;";
                        using (MySqlCommand cmd = new MySqlCommand(getProductQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@cartId", cartId);
                            cmd.Parameters.AddWithValue("@productId", productId);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    quantity = Convert.ToInt32(reader["quantity"]);
                                    price = Convert.ToDecimal(reader["price"]);
                                }
                                else
                                {
                                    return "Product not found in cart or not rentable.";
                                }
                            }
                        }

                        // Step 3: Insert into orders
                        long orderId;
                        string insertOrderQuery = @"
                    INSERT INTO orders (buyer_id, address, phone_number, payment_method, card_number, status)
                    VALUES (@buyerId, @address, @phone, @payment, @card, 'Processing');";
                        using (MySqlCommand cmd = new MySqlCommand(insertOrderQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@buyerId", buyerId);
                            cmd.Parameters.AddWithValue("@address", address);
                            cmd.Parameters.AddWithValue("@phone", phoneNumber);
                            cmd.Parameters.AddWithValue("@payment", paymentMethod);
                            cmd.Parameters.AddWithValue("@card", cardNo);
                            cmd.ExecuteNonQuery();
                            orderId = cmd.LastInsertedId;
                        }

                        // Step 4: Insert into order_items as rented
                        string insertItemQuery = @"
                    INSERT INTO order_items (order_id, product_id, quantity, price_per_item, is_rented, rent_days)
                    VALUES (@orderId, @productId, @qty, @price, 1, @days);";
                        using (MySqlCommand cmd = new MySqlCommand(insertItemQuery, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@orderId", orderId);
                            cmd.Parameters.AddWithValue("@productId", productId);
                            cmd.Parameters.AddWithValue("@qty", quantity);
                            cmd.Parameters.AddWithValue("@price", price);
                            cmd.Parameters.AddWithValue("@days", rentDays);
                            cmd.ExecuteNonQuery();
                        }

                        // Step 5: Update quantity in products table
                        string updateProductQty = "UPDATE products SET quantity = quantity - @qty WHERE product_id = @productId;";
                        using (MySqlCommand cmd = new MySqlCommand(updateProductQty, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@qty", quantity);
                            cmd.Parameters.AddWithValue("@productId", productId);
                            cmd.ExecuteNonQuery();
                        }

                        // Step 6: Remove this item from cart
                        string deleteFromCart = "DELETE FROM cart_items WHERE cart_id = @cartId AND product_id = @productId;";
                        using (MySqlCommand cmd = new MySqlCommand(deleteFromCart, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@cartId", cartId);
                            cmd.Parameters.AddWithValue("@productId", productId);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return "Product rented successfully.";
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return "Error renting product: " + ex.Message;
                    }
                }
            }
        }

        //using MySql.Data.MySqlClient; // ✅ make sure this is added at the top

        public List<UserOrderHistory> GetUserOrderHistory(int buyerId)
        {
            List<UserOrderHistory> history = new List<UserOrderHistory>();

            using (MySqlConnection con = new MySqlConnection("your_mysql_connection_string_here"))
            {
                string query = @"
                                SELECT 
                    o.order_id,
                    p.name,
                    oi.quantity,
                    o.status,
                    oi.is_rented
                FROM 
                    `orders` o
                JOIN 
                    order_items oi ON o.order_id = oi.order_id
                JOIN 
                    products p ON oi.product_id = p.product_id
                WHERE 
                    o.buyer_id = @buyerId;";

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@buyerId", buyerId);

                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        history.Add(new UserOrderHistory
                        {
                            OrderId = reader.GetInt32("order_id"),
                            ProductName = reader.GetString("product_name"),
                            Quantity = reader.GetInt32("quantity"),
                            Status = reader.GetString("status"),
                            Type = reader.GetBoolean("is_rented") ? "Rented" : "Bought"
                        });
                    }
                }
            }

            return history;
        }

     

        public List<Order> GetOrders(int sellerId)
        {
            List<Order> orders = new List<Order>();
            string connectionString = "server=localhost;uid=root;pwd=password;database=Shopease;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT o.order_id, o.product_id, o.buyer_id, o.quantity, o.total_price, 
                                o.payment_method, o.status, o.courier_tracking_id
                         FROM Orders o
                         JOIN Products p ON o.product_id = p.product_id
                         WHERE p.seller_id = @SellerId";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SellerId", sellerId);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Order order = new Order();
                            order.OrderId = Convert.ToInt32(reader["order_id"]);
                            order.ProductId = Convert.ToInt32(reader["product_id"]);
                            order.BuyerId = Convert.ToInt32(reader["buyer_id"]);
                            order.Quantity = Convert.ToInt32(reader["quantity"]);
                            order.TotalPrice = Convert.ToDecimal(reader["total_price"]);
                            order.PaymentMethod = reader["payment_method"].ToString();
                            order.Status = reader["status"].ToString();
                            order.CourierTrackingId = reader["courier_tracking_id"]?.ToString();

                            orders.Add(order);
                        }
                    }
                }
            }

            return orders;
        }

        //public void addToCart(string email, string product, int count)
        //{
        //    string ConnectionString = "Connection String";
        //    using (SqlConnection connection = new SqlConnection(ConnectionString))
        //    {
        //        string query = "INSERT INTO Cart (Email, Product, count) VALUES (@Email , @product, @count )";
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Email", email);
        //            command.Parameters.AddWithValue("@Product", product);
        //            command.Parameters.AddWithValue("@count", count);
        //            connection.Open();
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

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
