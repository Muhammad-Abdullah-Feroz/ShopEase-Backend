using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Org.BouncyCastle.Utilities;

namespace ShopEase_Backend
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string signup(string name, string email, string password, string role);

        [OperationContract]
        User login(string email, string password);

        [OperationContract]
        bool ResetPassword(string id, string password);
        [OperationContract]
        bool add_user_image(int userId, byte[] image);
        [OperationContract]
        bool Add_user(string name, string email, string password, string role, byte[] image);
        [OperationContract]
        List<User> GetUsers();
        [OperationContract]
        bool deleteUser(string id);
        [OperationContract]
        byte[] get_dp(string id);

        [OperationContract]
        List<Product> GetProducts();
        [OperationContract]
        List<Product> GetSellerProducts(int sellerId);
        [OperationContract]
        List<Product> GetProductsForUser();
        [OperationContract]
        int getProductRatings(int productId);
        [OperationContract]
        bool UpdateProduct(int productId, string name, string description, decimal price, int quantity, bool isForRent, decimal? perDayPrice);
        [OperationContract]
        List<Order> GetOrders(int sellerId);
        [OperationContract]
        string addProduct(string name, string description, decimal price, byte[] imageData, int sellerID, int rating, int quantity, bool rentable, decimal price_per_day);
        [OperationContract]
        bool deleteProduct(string id);

        [OperationContract]
        bool AddProductToCart(int buyerId, int productId);
        [OperationContract]
        List<cart_item> GetCartItemsForBuyer(int buyerId);
        [OperationContract]
        string PlaceOrderForBuyOnly(int buyerId, string address, string phoneNumber, string paymentMethod, string cardNo);
        [OperationContract]

        List<UserOrderHistory> GetUserOrderHistory(int buyerId);
        [OperationContract]
        string RentSingleProductFromCart(int buyerId, int productId, int rentDays, string address, string phoneNumber, string paymentMethod, string cardNo);
        [OperationContract]

        string DispatchOrder(int orderId, string trackingNumber, string status);

        [OperationContract]
        string GetData(int value);
        [OperationContract]
        string AddReview(int buyerId, int productId, int rating, string comment);
        [OperationContract]
        List<Product> GetProductsForReview(int buyerId);
        [OperationContract]
        List<Review> GetBuyerReviews(int buyerId);
        [OperationContract]
        string EditReview(int reviewId, int newRating, string newComment);
        [OperationContract]
        string DeleteReview(int reviewId);
        [OperationContract]
        List<Review> GetReviews();
        [OperationContract]
        List<Review> GetSellerProductReviews(int sellerId);
        [OperationContract]
        string GetTrackingNumberOrStatus(int orderId);


        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
