using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class StockDetailService : IStockDetailService
    {
        private readonly IBaseRepository<StockDetail> _stockDetailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StockDetailService(IBaseRepository<StockDetail> stockDetailRepository, IUnitOfWork unitOfWork)
        {
            _stockDetailRepository = stockDetailRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddStockDetail(StockDetail StockDetail)
        {
            _stockDetailRepository.Add(StockDetail);
        }

        public void SaveStockDetail()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<StockDetail> GetStockDetailByProductId(int id)
        {
            return _stockDetailRepository.FindBy(x=>x.ProductID == id);
        }
       
        public void DeleteStockDetail(int id)
        {
            _stockDetailRepository.Delete(x => x.SDetailID == id);
        }
        public StockDetail GetById(int id)
        {
            return _stockDetailRepository.FindBy(x => x.SDetailID == id).FirstOrDefault();
        }


        public IQueryable<StockDetail> GetAll()
        {
            return _stockDetailRepository.All;
        }

        public StockDetail GetStockDetail(int ProductID, int ColorID, string IMEI)
        {
            return _stockDetailRepository.FindBy(i=>i.ProductID==ProductID && i.ColorID==ColorID && i.IMENO.Equals(IMEI.Trim())).FirstOrDefault();
        }
        public IEnumerable<StockDetail> GetStockDetailByProductIdColorID(int ProductID,int ColorID)
        {
            return _stockDetailRepository.FindBy(x => x.ProductID == ProductID && x.ColorID == ColorID && x.Status==(int)EnumStockStatus.Stock);
        }
        public void Update(StockDetail StockDetail)
        {
            _stockDetailRepository.Update(StockDetail);
        }
    }
}
