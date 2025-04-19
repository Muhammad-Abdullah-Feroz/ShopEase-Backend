using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

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
        void updateUser(string email, string name, string password, string role);

        //[OperationContract]
        //List<Product> getProducts();

        //[OperationContract]
        //Product GetProduct(string name);

        [OperationContract]
        string addProduct(string name, string description, decimal price, byte[] imageData, int sellerID, int rating, int quantity, bool rentable, decimal price_per_day);

        [OperationContract]
        bool deleteProduct(string name);

        [OperationContract]
        void addToCart(string email, string product, int count);
        [OperationContract]
        string GetData(int value);
        
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
