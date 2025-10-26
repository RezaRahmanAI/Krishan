using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class CommonIndexExtensions
    {
        #region for website 

        public static async Task<IEnumerable<WebsiteProducts>> GetAllWebsiteProductAsync(this IBaseRepository<WebsiteProducts> _websiteProductRepository)
        {
            return await _websiteProductRepository.GetAll().ToListAsync();
        }


        public static async Task<IEnumerable<WebsiteProducts>> GetAllCategoryProductAsync(this IBaseRepository<WebsiteProducts> _websiteRepository, int category)
        {
            var data = await _websiteRepository.GetAll()
                .Where(t => t.ProcutCategory == category)
                .Select(t => new
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Price = t.Price,
                    DocumentPath = t.DocumentPath,
                })
                .ToListAsync();
           
            var result = data.Select(t => new WebsiteProducts
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Price = t.Price,
                DocumentPath = t.DocumentPath
            }).ToList();

            return result;
        }

        public static IQueryable<WebsiteProducts> GetAllProductIQueryables(this IBaseRepository<WebsiteProducts> productRepository, int category)
        {
            var products = from p in productRepository.All 
                           where p.ProcutCategory == category
                           select new WebsiteProducts
                           {
                               Id = p.Id,
                               Title = p.Title,
                               Description = p.Description,
                               Price = p.Price,
                           };
            return products;
        }

        public static IEnumerable<WebsiteProducts> GetAllProductAsync(this IBaseRepository<WebsiteProducts> _websiteProductRepository)
        { 
            return _websiteProductRepository.GetAll().ToList();
        }


        public static async Task<IEnumerable<AboutUS>> GetAllAsyncAbout(this IBaseRepository<AboutUS> _aboutUsRepository)
        {
            return await _aboutUsRepository.GetAll().ToListAsync();
        }
        #endregion 
    }
}
