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
    public class PurchaseOrderDetailService : IPurchaseOrderDetailService
    {
        private readonly IBaseRepository<POrderDetail> _purchaseOrderDetailRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IBaseRepository<ProductUnitType> _ProductUnitTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderDetailService(IBaseRepository<POrderDetail> purchaseOrderDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IUnitOfWork unitOfWork,
            IBaseRepository<ProductUnitType> ProductUnitTypeRepository)
        {
            _purchaseOrderDetailRepository = purchaseOrderDetailRepository;
            _productRepository = productRepository;
            _colorRepository = colorRepository;
            _unitOfWork = unitOfWork;
            _ProductUnitTypeRepository = ProductUnitTypeRepository;
        }

        public void AddPurchaseOrderDetail(POrderDetail pOrderDetail)
        {
            _purchaseOrderDetailRepository.Add(pOrderDetail);
        }

        public void SavePurchaseOrderDetail()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Tuple<decimal, int, decimal, decimal, int, int, decimal,
            Tuple<decimal, decimal, string, string, int, string, decimal, Tuple<decimal>>>>
            GetPurchaseOrderDetailById(int id)
        {
            return _purchaseOrderDetailRepository.GetPurchaseOrderDetailById(_productRepository, _colorRepository, _ProductUnitTypeRepository, id);
        }

        public void DeletePurchaseOrderDetail(int id)
        {
            _purchaseOrderDetailRepository.Delete(x => x.POrderID == id);
        }

        public IEnumerable<POrderDetail> GetPOrderDetailByID(int POrderID)
        {
            return _purchaseOrderDetailRepository.AllIncluding(i=>i.POProductDetails).Where(i => i.POrderID == POrderID);
        }

        public IQueryable<POrderDetail> GetAll()
        {
            return _purchaseOrderDetailRepository.All;
        }
    }
}
