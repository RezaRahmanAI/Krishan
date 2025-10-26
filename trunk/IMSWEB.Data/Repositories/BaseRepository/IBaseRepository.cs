using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;

namespace IMSWEB.Data
{
    public interface IBaseRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> All { get; }

        IEnumerable<T> ExecSP<T>(string sql, params object[] parameters);
        T SQLQuery<T>(string sql);
        IEnumerable<T> SQLQueryList<T>(string sql, params object[] parameters);

        void AddMultiple(IEnumerable<T> list);
        IQueryable<T> GetAllByConcernID(int concernID);
        IQueryable<T> GetAllByConcern(int concernId);
    }
}
