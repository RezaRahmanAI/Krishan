using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TOs;

namespace IMSWEB.Service
{
    public interface IProductService
    {
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void SaveProduct();
        Task<IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<string, decimal, decimal, decimal>>>> GetAllProductAsync();
        IQueryable<ProductWisePurchaseModel> GetAllProductIQueryable();
        IQueryable<ProductWisePurchaseModel> GetAllProductIQueryableIndex(EnumProductStockType StockType);
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, int>>> GetAllProduct();

        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, int>>>>> GetAllProductFromDetail();
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, decimal>>>>> GetAllProductFromDetailById(int productId = 0);

        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int,
            string, string, Tuple<string, string, string, string, string>>>> GetAllDamageProductFromDetail();
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>> SRWiseGetAllProductFromDetail(int EmployeeID);


        Product GetProductById(int id);
        void DeleteProduct(int id);

        //IEnumerable<Tuple<int, string, string,decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>> GetAllSalesProductFromDetailByCustomerID();
        //GetAllProductFromDetail
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>>> 
            GetAllSalesProductFromDetailByCustomerID(int CustomerID);
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal,Tuple<string> >>>> GetAllProductFromDetailForCredit();
        IEnumerable<Tuple<int,string, string, string, string>> GetProductDetails();

        IEnumerable<ProductWisePurchaseModel> GetAllSalesProductByCustomerID(int CustomerID);
        IEnumerable<ProductWisePurchaseModel> GetSalesDetailByCustomerIDNew(int CustomerID, string IMEI);

        IEnumerable<Tuple<int, string, string, decimal,
   string, string, string, Tuple<decimal?, string, decimal, decimal, int, string, int>>> GetAllProductDetails();

        IQueryable<ProductWisePurchaseModel> GetProducts();

        IQueryable<ProductWisePurchaseModel> GetProductDetailsBySDetailID(int StockDetailID);

        Product GetProductByConcernAndName(int ConcernID, string ProductName);
        List<TOProduct> GetAllProductNew(int concernId, int productId = 0);
        IQueryable<ProductWisePurchaseModel> GetAllProductIQueryableForProduction(EnumProductStockType StockType);
        IQueryable<ProductWisePurchaseModel> GetAllProductIQueryableForProduction();
    }
}
