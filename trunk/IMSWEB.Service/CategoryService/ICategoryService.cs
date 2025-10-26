using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICategoryService
    {
        void AddCategory(Category Category);
        void UpdateCategory(Category Category);
        void SaveCategory();
        IEnumerable<Category> GetAllCategory();
        Task<IEnumerable<Category>> GetAllCategoryAsync();
        Category GetCategoryById(int id);
        void DeleteCategory(int id);
        IQueryable<Category> GetAllIQueryable();

        IQueryable<Category> GetAllIQueryable(int ConcernID);

    }
}
