using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("Stock")]
    public class StockController : CoreController
    {
        IStockService _StockService;
        IStockDetailService _StockDetailService;
        IMapper _mapper;

        public StockController(IErrorService errorService, IStockDetailService StockDetailServie,
            IStockService StockService, IMapper mapper)
            : base(errorService)
        {
            _StockService = StockService;
            _mapper = mapper;
            _StockDetailService = StockDetailServie;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult Index()
        {
            var StocksAsync = _StockService.GetforStockReport(User.Identity.Name, User.Identity.GetConcernId(), 0, 0, 0, 0, 0,0);
            var vmodel = _mapper.Map<IEnumerable<ProductWisePurchaseModel>, IEnumerable<GetStockViewModel>>(StocksAsync);
            return View(vmodel);


        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> StockDetails(int? Page)
        {
            int PageSize = 10;
            int Pages = Page.HasValue ? Convert.ToInt32(Page) : 1;
            var StocksAsync = _StockService.GetAllStockDetailAsync();
            var vmodel = _mapper.Map<IEnumerable<Tuple<int, string, string,
            string, string, string, string, Tuple<string>>>, List<GetStockDetailViewModel>>(await StocksAsync);
            var pagelist = vmodel.ToPagedList(Pages, PageSize);
            return View(pagelist);
        }

        [HttpPost]
        [Authorize]
        public ActionResult StockDetails(FormCollection formCollection)
        {
            int PageSize = 10;
            int Pages = 1;
            IQueryable<ProductDetailsModel> StocksAsync = null;
            if (!string.IsNullOrEmpty(formCollection["ProductName"]))
            {
                string productname = formCollection["ProductName"];
                StocksAsync = _StockService.GetStockDetails().Where(i => i.ProductName.Contains(productname));
            }
            var vmodel = _mapper.Map<IQueryable<ProductDetailsModel>, List<GetStockDetailViewModel>>(StocksAsync);
            var pagelist = vmodel.ToPagedList(Pages, PageSize);
            return View(pagelist);
        }

        [HttpGet]
        [Authorize]
        public ActionResult StockReport()
        {
            return View("StockReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult StockSummaryReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWisePriceProtection()
        {
            return View("ProductWisePriceProtection");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DailyStockVSSalesSummary()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult StockLedgerReport()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult UpdateSalesRate(int ProductID, int ColorID, decimal SalesRate, decimal CreditSalesRate3, decimal CreditSalesRate6, decimal CreditSalesRate12)
        {
            if (ProductID != 0 && ColorID != 0)
            {
                var collection = _StockDetailService.GetStockDetailByProductIdColorID(ProductID, ColorID);
                if (collection.Count() != 0)
                {
                    foreach (var item in collection)
                    {
                        item.SRate = SalesRate;
                        item.CreditSRate = CreditSalesRate6;
                        item.CRSalesRate3Month = CreditSalesRate3;
                        item.CRSalesRate12Month = CreditSalesRate12;
                    }
                    _StockDetailService.SaveStockDetail();
                    AddToastMessage("", "Update Successfully", ToastType.Success);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            AddToastMessage("", "Update Failed;", ToastType.Error);

            return Json(false, JsonRequestBehavior.AllowGet);

        }
    }
}