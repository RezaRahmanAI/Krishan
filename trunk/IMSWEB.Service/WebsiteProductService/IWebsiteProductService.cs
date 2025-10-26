using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IWebsiteProductService
    {
        void Add(WebsiteProducts Model);
        void Update(WebsiteProducts Model);
        void SaveWebsitePRoduct();
        bool Save();
        void Delete(int id);
        IEnumerable<WebsiteProducts> GetAllProducts();
        Task<IEnumerable<WebsiteProducts>> GetAllProductAsync();  
        Task<IEnumerable<WebsiteProducts>> GetAllCategoryProductAsync(int category);   
        Task<IEnumerable<WebsiteProducts>> GetAllNewProductAsync(int category);   
        WebsiteProducts GetById(int id);

        IQueryable<WebsiteProducts> GetAllProductIQueryables(int category);


        IEnumerable<WebsiteProducts> GetProductAsync();  



    }
}
