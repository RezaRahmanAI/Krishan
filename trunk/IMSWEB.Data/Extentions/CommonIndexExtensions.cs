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
            return await _websiteProductRepository.GetAll()
                .Include(t => t.SisterConcern)
                .OrderByDescending(t => t.CreateDate)
                .ToListAsync();
        }


        public static async Task<IEnumerable<WebsiteProducts>> GetAllCategoryProductAsync(this IBaseRepository<WebsiteProducts> _websiteRepository, int category)
        {
            return await _websiteRepository.GetAll()
                .Include(t => t.SisterConcern)
                .Where(t => t.ProcutCategory == category)
                .OrderByDescending(t => t.CreateDate)
                .ToListAsync();
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
            return _websiteProductRepository.GetAll()
                .Include(t => t.SisterConcern)
                .OrderByDescending(t => t.CreateDate)
                .ToList();
        }


        public static async Task<IEnumerable<AboutUS>> GetAllAsyncAbout(this IBaseRepository<AboutUS> _aboutUsRepository)
        {
            return await _aboutUsRepository.GetAll().ToListAsync();
        }
        #endregion 
    }
}
