using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;
using System.Text;
using log4net;
using IMSWEB.Model.TOs;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("purchaseorder")]
    public class PurchaseOrderController : CoreController
    {
        IPurchaseOrderService _purchaseOrderService;
        IPurchaseOrderDetailService _purchaseOrderDetailService;
        IPOProductDetailService _pOProductDetailService;
        IStockService _stockService;
        IStockDetailService _stockDetailService;
        IMiscellaneousService<POrder> _miscellaneousService;
        IMapper _mapper;
        ISupplierService _supplierService;
        IColorService _colorService;
        IGodownService _godownService;
        IProductService _productService;
        ISisterConcernService _SisterConcern;
        ICashCollectionService _cashCollectionService;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PurchaseOrderController(IErrorService errorService,
            IPurchaseOrderService purchaseOrderService, IPurchaseOrderDetailService purchaseOrderDetailService,
            IPOProductDetailService pOProductDetailService, IStockService stockService, ISupplierService supplierService,
            IStockDetailService stockDetailService, IMiscellaneousService<POrder> miscellaneousService, IMapper mapper,
            IColorService colorService,
            IGodownService godownService,
            IProductService productService, ISisterConcernService SisService, ICashCollectionService cashCollectionService
            )
            : base(errorService)
        {
            _purchaseOrderService = purchaseOrderService;
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _pOProductDetailService = pOProductDetailService;
            _stockService = stockService;
            _stockDetailService = stockDetailService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _supplierService = supplierService;
            _colorService = colorService;
            _godownService = godownService;
            _productService = productService;
            _SisterConcern = SisService;
            _cashCollectionService = cashCollectionService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            TempData["purchaseOrderViewModel"] = null;
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            var customPO = _purchaseOrderService.GetAllPurchaseOrderAsync(DateRange.Item1, DateRange.Item2);
            var vmPO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>,
                IEnumerable<GetPurchaseOrderViewModel>>(await customPO);
            return View(vmPO);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            var customPO = _purchaseOrderService.GetAllPurchaseOrderAsync(ViewBag.FromDate, ViewBag.ToDate);
            var vmPO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>,
                IEnumerable<GetPurchaseOrderViewModel>>(await customPO);
            return View("Index", vmPO);
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> DeliveryOrders()
        {
            TempData["purchaseOrderViewModel"] = null;
            var customPO = _purchaseOrderService.GetAllDeliveryOrderAsync();
            var vmPO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>,
                IEnumerable<GetPurchaseOrderViewModel>>(await customPO);
            return View(vmPO);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> DamageReturnOrders()
        {
            TempData["purchaseOrderViewModel"] = null;
            var customPO = _purchaseOrderService.GetAllDamageReturnOrderAsync();
            var vmPO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>,
                IEnumerable<GetPurchaseOrderViewModel>>(await customPO);
            return View(vmPO);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData(false);
        }
        [HttpGet]
        [Authorize]
        public ActionResult CreateDeliveryOrder()
        {
            return ReturnCreateViewWithTempData(true);
        }



        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult CreateDeliveryOrder(PurchaseOrderViewModel newPurchaseOrder, FormCollection formCollection, string returnUrl)
        {
            return HandlePurchaseOrder(newPurchaseOrder, formCollection, true, string.Empty);
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(PurchaseOrderViewModel newPurchaseOrder, FormCollection formCollection, string returnUrl, string ItemType)
        {
            return HandlePurchaseOrder(newPurchaseOrder, formCollection, false, ItemType);
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateDamageReturn()
        {
            return DamageReturnCreateViewWithTempData();
        }


        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult CreateDamageReturn(SalesOrderViewModel newSalesOrder, FormCollection formCollection, string returnUrl)
        {
            return HandleDamageReturnOrder(newSalesOrder, formCollection);
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{orderId}")]
        public ActionResult Edit(int orderId, string previousAction, bool IsDO)
        {
            if (HasSoldProductCheckByPOId(orderId))
            {
                AddToastMessage("", "Some product(s) has already been sold from this order. Can't edit this purchase.",
                    ToastType.Error);
                return RedirectToAction("Index");
            }
            ViewBag.SupplierIds = GetAllSupplierForDDL();
            ViewBag.ProductIds = GetAllProductForDDL();
            var payItems = _cashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();
            if (TempData["purchaseOrderViewModel"] == null || string.IsNullOrEmpty(previousAction))
            {
                var purchaseOrder = _purchaseOrderService.GetPurchaseOrderById(orderId);
                var poDetails = _purchaseOrderDetailService.GetPurchaseOrderDetailById(orderId);

                var vmPurchaseOrder = _mapper.Map<POrder, CreatePurchaseOrderViewModel>(purchaseOrder);
                var vmPoDetails = _mapper.Map<IEnumerable<Tuple<decimal, int, decimal, decimal, int, int, decimal,
                    Tuple<decimal, decimal, string, string, int, string, decimal, Tuple<decimal>>>>, IEnumerable<CreatePurchaseOrderDetailViewModel>>(poDetails).ToList();

                var vm = new PurchaseOrderViewModel
                {
                    PODetail = new CreatePurchaseOrderDetailViewModel(),
                    PODetails = vmPoDetails,
                    PurchaseOrder = vmPurchaseOrder
                };
                vm.PurchaseOrder.PayItems = payItems;
                string payType = string.Empty;
                if (vmPurchaseOrder.PayCashAccountId.HasValue)
                {
                    vmPurchaseOrder.ItemId = vmPurchaseOrder.PayCashAccountId.ToString();
                    payType = "CA";
                }
                else if (vmPurchaseOrder.PayBankId.HasValue)
                {
                    vmPurchaseOrder.ItemId = vmPurchaseOrder.PayBankId.ToString();
                    payType = "B";
                }
                TempData["purchaseOrderViewModel"] = vm;
                if (IsDO)
                    return View("CreateDeliveryOrder", vm);
                else
                    return View("Create", vm);
            }
            else
            {
                return ReturnCreateViewWithTempData(IsDO);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(PurchaseOrderViewModel newPurchaseOrder, FormCollection formCollection, string returnUrl, bool IsDo)
        {
            return HandlePurchaseOrder(newPurchaseOrder, formCollection, IsDo, string.Empty);
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{orderId}")]
        public ActionResult Delete(int orderId)
        {
            if (HasSoldProductCheckByPOId(orderId))
            {
                AddToastMessage("", "Some product(s) has already been sold from this order. Try edit button to see details",
                    ToastType.Error);
                return RedirectToAction("Index");
            }
            var Model = _purchaseOrderService.GetPurchaseOrderById(orderId);
            if (!IsDateValid(Model.OrderDate))
            {
                return RedirectToAction("Index");
            }

            if (_purchaseOrderService.DeletePurchaseOrderUsingSP(orderId, User.Identity.GetUserId<int>()))
                AddToastMessage("", "Item has been deleted successfully", ToastType.Success);
            else
                AddToastMessage("", "Return Failed. Please contact with Object Canvas Technolgoy.", ToastType.Error);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        [Route("EditFromView/{id}/{previousAction}")]
        public ActionResult EditFromView(int id, int cid, string previousAction, bool IsDO)
        {
            PurchaseOrderViewModel purchaseOrder = (PurchaseOrderViewModel)TempData.Peek("purchaseOrderViewModel");
            if (purchaseOrder == null)
            {
                AddToastMessage("", "Item has been expired to edit", ToastType.Error);
                if (IsDO)
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("DeliveryOrders");
                    else
                        return RedirectToAction("CreateDeliveryOrder");
                }
                else
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("Create");
                }
            }

            CreatePurchaseOrderDetailViewModel itemToEdit = purchaseOrder.PODetails.Where(x => int.Parse(x.ProductId) == id &&
                    int.Parse(x.ColorId) == cid).FirstOrDefault();

            if (itemToEdit != null)
            {
                if (IsForEdit(previousAction) && HasSoldProductCheckByPODetailId(int.Parse(GetDefaultIfNull(itemToEdit.PODetailId))))
                {
                    AddToastMessage("", "Some product(s) has already been sold from this order. This order is not editable",
                        ToastType.Error);
                    return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit" });
                }

                purchaseOrder.PurchaseOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.GrandTotal)) -
                     (decimal.Parse(GetDefaultIfNull(itemToEdit.Quantity)) * decimal.Parse(GetDefaultIfNull(itemToEdit.MRPRate)))).ToString();

                purchaseOrder.PurchaseOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.PPDiscountAmount)) -
                    decimal.Parse(GetDefaultIfNull(itemToEdit.PPDiscountAmount))).ToString();

                purchaseOrder.PurchaseOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.NetDiscount)) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.Quantity)) * decimal.Parse(GetDefaultIfNull(itemToEdit.PPDiscountAmount)))
                    ).ToString();

                purchaseOrder.PurchaseOrder.TotalAmount = (decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalAmount)) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.TAmount)))
                    ).ToString();

                purchaseOrder.PurchaseOrder.PaymentDue = ((decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.PaymentDue)) + decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.RecieveAmount))) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.TAmount)))
                    ).ToString();

                if (IsForEdit(previousAction) && !string.IsNullOrEmpty(itemToEdit.PODetailId))
                {
                    itemToEdit.Status = EnumStatus.Deleted;
                    //int supplierId = int.Parse(purchaseOrder.PurchaseOrder.SupplierId);
                    int porderDetailId = int.Parse(itemToEdit.PODetailId);
                    int productId = int.Parse(itemToEdit.ProductId);
                    //int colorId = int.Parse(itemToEdit.ColorId);
                    //int userId = User.Identity.GetUserId<int>();
                    //decimal quantity = decimal.Parse(itemToEdit.Quantity);
                    ////decimal totalDue = decimal.Parse(itemToDelete.TAmount) - decimal.Parse(GetDefaultIfNull(itemToDelete.PPDiscountAmount));

                    //DataTable dtPOProductDetail = new DataTable();
                    //dtPOProductDetail.Columns.Add("ProductID", typeof(int));
                    //dtPOProductDetail.Columns.Add("ColorId", typeof(int));
                    //dtPOProductDetail.Columns.Add("IMENO", typeof(string));

                    itemToEdit.POProductDetails = _pOProductDetailService.
                                            GetPOProductDetailsById(porderDetailId, productId).ToList();
                    //CreatePOProductDetailDataTable(itemToEdit, dtPOProductDetail);

                    //_purchaseOrderService.DeletePurchaseOrderDetailUsingSP(supplierId, porderDetailId, productId,
                    //    colorId, userId, quantity, 0, dtPOProductDetail);
                }
                else
                {
                    purchaseOrder.PODetails.Remove(itemToEdit);
                }

                TempData["POProductDetails"] = itemToEdit.POProductDetails.ToList();
                TempData["OLDPOProductDetails"] = itemToEdit.POProductDetails.ToList();
                purchaseOrder.PODetail = itemToEdit;
                itemToEdit.POProductDetails.Clear();
                TempData["purchaseOrderViewModel"] = purchaseOrder;
                if (IsDO)
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit", IsDO = true });
                    else
                        return RedirectToAction("CreateDeliveryOrder");
                }
                else
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit", IsDO = false });
                    else
                        return RedirectToAction("Create");
                }

            }
            else
            {
                AddToastMessage("", "No item found to edit", ToastType.Info);
                if (IsDO)
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("DeliveryOrders");
                    else
                        return RedirectToAction("Create");
                }
                else
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("Create");
                }

            }
        }

        [HttpGet]
        [Authorize]
        [Route("deleteFromView/{id}")]
        public ActionResult DeleteFromView(int id, int cid, string previousAction, bool IsDO)
        {
            PurchaseOrderViewModel purchaseOrder = (PurchaseOrderViewModel)TempData.Peek("purchaseOrderViewModel");
            if (purchaseOrder == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                if (IsDO)
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("DeliveryOrders");
                    else
                        return RedirectToAction("CreateDeliveryOrder");
                }
                else
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("Create");
                }
            }

            CreatePurchaseOrderDetailViewModel itemToDelete = purchaseOrder.PODetails.Where(x => int.Parse(x.ProductId) == id &&
                    int.Parse(x.ColorId) == cid).FirstOrDefault();

            if (itemToDelete != null)
            {
                if (IsForEdit(previousAction) && HasSoldProductCheckByPODetailId(int.Parse(GetDefaultIfNull(itemToDelete.PODetailId))))
                {
                    AddToastMessage("", "Some product(s) has already been sold from this order. This order is not deletable",
                        ToastType.Error);
                    return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit" });
                }
                purchaseOrder.PurchaseOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.GrandTotal)) -
                    decimal.Parse(GetDefaultIfNull(itemToDelete.TAmount))).ToString();

                purchaseOrder.PurchaseOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.PPDiscountAmount)) -
                    decimal.Parse(GetDefaultIfNull(itemToDelete.PPDiscountAmount))).ToString();

                purchaseOrder.PurchaseOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.NetDiscount)) -
                    decimal.Parse(GetDefaultIfNull(itemToDelete.PPDiscountAmount))).ToString();

                purchaseOrder.PurchaseOrder.TotalAmount = (decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalAmount)) -
                    (decimal.Parse(GetDefaultIfNull(itemToDelete.TAmount)) - decimal.Parse(GetDefaultIfNull(itemToDelete.PPDiscountAmount)))).ToString();

                purchaseOrder.PurchaseOrder.PaymentDue = ((decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.PaymentDue))) -
                    (decimal.Parse(GetDefaultIfNull(itemToDelete.TAmount)) - decimal.Parse(GetDefaultIfNull(itemToDelete.PPDiscountAmount)))).ToString();

                if (IsForEdit(previousAction) && !string.IsNullOrEmpty(itemToDelete.PODetailId))
                {
                    itemToDelete.Status = EnumStatus.Deleted;
                    //int supplierId = int.Parse(purchaseOrder.PurchaseOrder.SupplierId);
                    //int porderDetailId = int.Parse(itemToDelete.PODetailId);
                    //int productId = int.Parse(itemToDelete.ProductId);
                    //int colorId = int.Parse(itemToDelete.ColorId);
                    //int userId = User.Identity.GetUserId<int>();
                    //decimal quantity = decimal.Parse(itemToDelete.Quantity);
                    ////decimal totalDue = decimal.Parse(itemToDelete.TAmount) - decimal.Parse(GetDefaultIfNull(itemToDelete.PPDiscountAmount));

                    //DataTable dtPOProductDetail = new DataTable();
                    //dtPOProductDetail.Columns.Add("ProductID", typeof(int));
                    //dtPOProductDetail.Columns.Add("ColorId", typeof(int));
                    //dtPOProductDetail.Columns.Add("IMENO", typeof(string));

                    //itemToDelete.POProductDetails = _pOProductDetailService.
                    //                        GetPOProductDetailsById(porderDetailId, productId).ToList();
                    itemToDelete.POProductDetails = new List<POProductDetail>();
                    //CreatePOProductDetailDataTable(itemToDelete, dtPOProductDetail);

                    //_purchaseOrderService.DeletePurchaseOrderDetailUsingSP(supplierId, porderDetailId, productId,
                    //    colorId, userId, quantity, 0, dtPOProductDetail);
                }
                else
                {
                    purchaseOrder.PODetails.Remove(itemToDelete);
                }

                TempData["purchaseOrderViewModel"] = purchaseOrder;
                purchaseOrder.PODetail = new CreatePurchaseOrderDetailViewModel();
                AddToastMessage("", "Item has been removed successfully", ToastType.Success);

                if (IsDO)
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit", IsDO = true });
                    else
                        return RedirectToAction("CreateDeliveryOrder");
                }
                else
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit", IsDO = false });
                    else
                        return RedirectToAction("Create");
                }
            }
            else
            {
                AddToastMessage("", "No item found to remove", ToastType.Info);
                if (IsDO)
                {
                    if (IsForEdit(previousAction))
                        return RedirectToAction("DeliveryOrders");
                    else
                        return RedirectToAction("CreateDeliveryOrder");
                }
                else
                {

                    if (IsForEdit(previousAction))
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("Create");
                }
            }
        }

        private void CheckAndAddModelErrorForAdd(PurchaseOrderViewModel newPurchaseOrder, FormCollection formCollection, bool IsDeliveryOrder)
        {
            if (string.IsNullOrEmpty(formCollection["OrderDate"]))
                ModelState.AddModelError("PurchaseOrder.OrderDate", "Purchase Date is required");

            if (string.IsNullOrEmpty(formCollection["SuppliersId"]))
                ModelState.AddModelError("PurchaseOrder.SupplierId", "Supplier is required");

            if (string.IsNullOrEmpty(formCollection["ProductsId"]))
                ModelState.AddModelError("PODetail.ProductId", "Product is required");

            if (string.IsNullOrEmpty(formCollection["ColorsId"]))
            {
                //ModelState.AddModelError("PODetail.ColorId", "Grade is required");
                newPurchaseOrder.PODetail.ColorId = _colorService.GetAllIQueryable().FirstOrDefault().ColorID.ToString();
            }

            if (string.IsNullOrEmpty(newPurchaseOrder.PurchaseOrder.ChallanNo))
                ModelState.AddModelError("PurchaseOrder.ChallanNo", "Challan No. is required");

            if (string.IsNullOrEmpty(newPurchaseOrder.PODetail.Quantity))
                ModelState.AddModelError("PODetail.Quantity", "Quantity is required");

            if (string.IsNullOrEmpty(newPurchaseOrder.PODetail.MRPRate) && !IsDeliveryOrder)
                ModelState.AddModelError("PODetail.MRPRate", "MRP Rate is required");

            if (string.IsNullOrEmpty(newPurchaseOrder.PODetail.UnitPrice))
                ModelState.AddModelError("PODetail.UnitPrice", "Purchase Rate is required");
            if (!IsDeliveryOrder)
            {

                if (string.IsNullOrEmpty(newPurchaseOrder.PODetail.SalesRate))
                    ModelState.AddModelError("PODetail.SalesRate", "Cash Sales Rate is required");

                if (User.Identity.GetConcernId() == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID || User.Identity.GetConcernId() == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID || User.Identity.GetConcernId() == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID)
                {
                    //if (string.IsNullOrEmpty(newPurchaseOrder.PODetail.CreditSalesRate))
                    //    ModelState.AddModelError("PODetail.CreditSalesRate", "Credit Sales Rate 6 is required");

                    //if (string.IsNullOrEmpty(newPurchaseOrder.PODetail.CRSalesRate3Month))
                    //    ModelState.AddModelError("PODetail.CRSalesRate3Month", "Credit Sales Rate 3 is required");

                    //if (string.IsNullOrEmpty(newPurchaseOrder.PODetail.CRSalesRate12Month))
                    //    ModelState.AddModelError("PODetail.CRSalesRate12Month", "Credit Sales Rate 12 is required");
                }
            }

            decimal CalTAmount = Math.Round((Convert.ToDecimal(newPurchaseOrder.PODetail.UnitPrice) * Convert.ToDecimal(newPurchaseOrder.PODetail.Quantity)), 4);
            decimal TAmount = Math.Round(Convert.ToDecimal(newPurchaseOrder.PODetail.TAmount), 4);
            if (Math.Abs(CalTAmount - TAmount) > 1m)
            {
                ModelState.AddModelError("PODetail.TAmount", "Total amount is not ok.");
                AddToastMessage("", "Total amount is not ok.");
            }

        }

        private void CheckAndAddModelErrorForSave(PurchaseOrderViewModel newPurchaseOrder, PurchaseOrderViewModel purchaseOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(newPurchaseOrder.PurchaseOrder.GrandTotal) ||
                decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.GrandTotal)) <= 0)
                ModelState.AddModelError("PurchaseOrder.GrandTotal", "Grand Total is required");

            if (string.IsNullOrEmpty(newPurchaseOrder.PurchaseOrder.TotalAmount) ||
                decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.TotalAmount)) <= 0)
                ModelState.AddModelError("PurchaseOrder.TotalAmount", "Net Total is required");

            //if (string.IsNullOrEmpty(newPurchaseOrder.PurchaseOrder.RecieveAmount))
            //    ModelState.AddModelError("PurchaseOrder.RecieveAmount", "Pay Amount is required");
            if (!string.IsNullOrEmpty(formCollection["OrderDate"]))
            {
                newPurchaseOrder.PurchaseOrder.OrderDate = formCollection["OrderDate"];
                purchaseOrder.PurchaseOrder.OrderDate = formCollection["OrderDate"];
            }

            if (string.IsNullOrEmpty(newPurchaseOrder.PurchaseOrder.ItemId))
            {
                ModelState.AddModelError("PurchaseOrder.ItemId", "Payment type is required!");
            }
        }

        private void AddToOrder(PurchaseOrderViewModel newPurchaseOrder, PurchaseOrderViewModel purchaseOrder, FormCollection formCollection, bool IsDeliveryOrder)
        {
            decimal newGrandTotal = decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.TAmount)) +
                (decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.PPDiscountAmount)) * decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.Quantity))) +
                (decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.PPOffer)) * decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.Quantity))) +
                 (decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.ExtraPPDISAmt)) * decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.Quantity)));
            //purchaseOrder.PurchaseOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.GrandTotal)) +
            //    decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.TAmount))).ToString();

            purchaseOrder.PurchaseOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.GrandTotal)) + newGrandTotal).ToString();

            purchaseOrder.PurchaseOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.PPDiscountAmount)) +
                decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.PPDiscountAmount))).ToString();

            purchaseOrder.PurchaseOrder.TotalDiscountPercentage = decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.TotalDiscountPercentage)).ToString();
            purchaseOrder.PurchaseOrder.TotalDiscountAmount = decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.TotalDiscountAmount)).ToString();
            purchaseOrder.PurchaseOrder.tempFlatDiscountAmount = purchaseOrder.PurchaseOrder.TotalDiscountAmount;

            purchaseOrder.PurchaseOrder.AdjAmount = decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.AdjAmount)).ToString();

            purchaseOrder.PurchaseOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.NetDiscount)) + decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDiscountAmount)) +
                (decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.PPDiscountAmount)) * decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.Quantity))) +
                (decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.ExtraPPDISAmt)) * decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.Quantity))) +
                (decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.PPOffer)) * decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PODetail.Quantity)))
                ).ToString();

            purchaseOrder.PurchaseOrder.tempNetDiscount = purchaseOrder.PurchaseOrder.NetDiscount;

            var netTotal = ((decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.GrandTotal)) +
                decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.LabourCost))) -
                (decimal.Parse(purchaseOrder.PurchaseOrder.NetDiscount) + decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.AdjAmount))));

            purchaseOrder.PurchaseOrder.TotalAmount = netTotal.ToString();
            purchaseOrder.PurchaseOrder.PaymentDue = (netTotal - decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.RecieveAmount))).ToString();

            purchaseOrder.PurchaseOrder.LabourCost = decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.LabourCost)).ToString();
            purchaseOrder.PurchaseOrder.RecieveAmount = decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.RecieveAmount)).ToString();

            purchaseOrder.PurchaseOrder.OrderDate = formCollection["OrderDate"];
            purchaseOrder.PurchaseOrder.SupplierId = formCollection["SuppliersId"];
            purchaseOrder.PurchaseOrder.IsDamagePO = newPurchaseOrder.PurchaseOrder.IsDamagePO;


            purchaseOrder.PODetail.PODetailId = newPurchaseOrder.PODetail.PODetailId;
            purchaseOrder.PODetail.ProductId = formCollection["ProductsId"];
            purchaseOrder.PODetail.ProductCode = formCollection["ProductsCode"];
            purchaseOrder.PODetail.ProductName = formCollection["ProductsName"];

            purchaseOrder.PODetail.ColorId = newPurchaseOrder.PODetail.ColorId;
            purchaseOrder.PODetail.GodownID = newPurchaseOrder.PODetail.GodownID;

            purchaseOrder.PODetail.ColorName = formCollection["ColorsName"];
            purchaseOrder.PODetail.ConvertValue = newPurchaseOrder.PODetail.ConvertValue;
            purchaseOrder.PODetail.UnitPrice = newPurchaseOrder.PODetail.UnitPrice;
            purchaseOrder.PODetail.TAmount = newPurchaseOrder.PODetail.TAmount;
            purchaseOrder.PODetail.PPDisPercentage = newPurchaseOrder.PODetail.PPDisPercentage;
            purchaseOrder.PODetail.PPDiscountAmount = newPurchaseOrder.PODetail.PPDiscountAmount;
            purchaseOrder.PODetail.MRPRate = newPurchaseOrder.PODetail.MRPRate;
            purchaseOrder.PODetail.Status = newPurchaseOrder.PODetail.Status == default(int) ? EnumStatus.New : newPurchaseOrder.PODetail.Status;
            purchaseOrder.PODetail.Quantity = newPurchaseOrder.PODetail.Quantity;
            purchaseOrder.PODetail.SalesRate = newPurchaseOrder.PODetail.SalesRate;
            purchaseOrder.PODetail.ExtraPPDISAmt = newPurchaseOrder.PODetail.ExtraPPDISAmt;
            purchaseOrder.PODetail.ExtraPPDISPer = newPurchaseOrder.PODetail.ExtraPPDISPer;
            purchaseOrder.PODetail.PPOffer = newPurchaseOrder.PODetail.PPOffer;
            purchaseOrder.PODetail.CreditSalesRate = newPurchaseOrder.PODetail.CreditSalesRate;
            purchaseOrder.PODetail.CRSalesRate12Month = newPurchaseOrder.PODetail.CRSalesRate12Month;
            purchaseOrder.PODetail.CRSalesRate3Month = newPurchaseOrder.PODetail.CRSalesRate3Month;

            purchaseOrder.PODetail.TotalArea = newPurchaseOrder.PODetail.TotalArea;
            purchaseOrder.PODetail.RatePerArea = newPurchaseOrder.PODetail.RatePerArea;

            //purchaseOrder.PODetail.CalculatedQuantity = purchaseOrder.PODetail.Status == EnumStatus.Updated ?
            //    GetUpdatedQuantity(Convert.ToInt32(double.Parse(formCollection["PrevQuantity"])), Convert.ToInt32(double.Parse(newPurchaseOrder.PODetail.Quantity)))
            //    : Convert.ToInt32(double.Parse(newPurchaseOrder.PODetail.Quantity));

            purchaseOrder.PODetail.POProductDetails = purchaseOrder.PODetail.POProductDetails ?? new List<POProductDetail>();

            int ProductType = int.Parse(GetDefaultIfNull(formCollection["ProductsType"]));
            POProductDetail popDetail = null;
            string[] IMEIS = formCollection.AllKeys
                           .Where(key => key.StartsWith("IMEINo"))
                           .Select(key => formCollection[key])
                           .ToArray();

            string[] DIMEIS = formCollection.AllKeys
                          .Where(key => key.StartsWith("DIMEINo"))
                          .Select(key => formCollection[key])
                          .ToArray();

            if (!IsDeliveryOrder)
            {

                if (ProductType == (int)EnumProductType.ExistingBC || ProductType == (int)EnumProductType.AutoBC)//Barcode=1 ,Auto Barcode=3
                {
                    for (int i = 0; i < decimal.Parse(purchaseOrder.PODetail.Quantity); i++)
                    {
                        var poProductDetail = new POProductDetail();
                        poProductDetail.ProductID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ProductId));
                        poProductDetail.ColorID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ColorId));
                        poProductDetail.IMENO = IMEIS[i];

                        if (i <= DIMEIS.Length - 1 && purchaseOrder.PurchaseOrder.IsDamagePO)
                        {
                            poProductDetail.DIMENO = DIMEIS[i];
                            popDetail = _purchaseOrderService.GetDamagePOPDetail(poProductDetail.DIMENO, poProductDetail.ProductID, poProductDetail.ColorID);
                            if (popDetail != null)
                            {
                                poProductDetail.DamagePOPDID = popDetail.POPDID;
                            }
                        }

                        if (purchaseOrder.PODetail.POProductDetails.Any(x => x.ProductID == poProductDetail.ProductID
                            && x.ColorID == poProductDetail.ColorID && x.IMENO.Equals(poProductDetail.IMENO))) continue;

                        if (!string.IsNullOrEmpty(poProductDetail.IMENO))
                            purchaseOrder.PODetail.POProductDetails.Add(poProductDetail);
                    }
                }
                else if (ProductType == (int)EnumProductType.NoBarcode)// //No Barcode
                {
                    var poProductDetail = new POProductDetail();
                    poProductDetail.ProductID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ProductId));
                    poProductDetail.ColorID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ColorId));
                    poProductDetail.IMENO = formCollection["IMEINo1"];

                    //if (purchaseOrder.PODetail.POProductDetails.Any(x => x.ProductID == poProductDetail.ProductID
                    //    && x.ColorID == poProductDetail.ColorID && x.IMENO.Equals(poProductDetail.IMENO))) continue;

                    purchaseOrder.PODetail.POProductDetails.Add(poProductDetail);
                }

            }
            else
            {
                var poProductDetail = new POProductDetail();
                poProductDetail.ProductID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ProductId));
                poProductDetail.ColorID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ColorId));
                poProductDetail.IMENO = "No Barcode";

                //if (purchaseOrder.PODetail.POProductDetails.Any(x => x.ProductID == poProductDetail.ProductID
                //    && x.ColorID == poProductDetail.ColorID && x.IMENO.Equals(poProductDetail.IMENO))) continue;

                purchaseOrder.PODetail.POProductDetails.Add(poProductDetail);
            }

            purchaseOrder.PODetails = purchaseOrder.PODetails ?? new List<CreatePurchaseOrderDetailViewModel>();
            purchaseOrder.PODetails.Add(purchaseOrder.PODetail);

            PurchaseOrderViewModel vm = new PurchaseOrderViewModel
            {
                PODetail = new CreatePurchaseOrderDetailViewModel(),
                PODetails = purchaseOrder.PODetails,
                PurchaseOrder = purchaseOrder.PurchaseOrder
            };

            TempData["purchaseOrderViewModel"] = vm;
            purchaseOrder.PODetail = new CreatePurchaseOrderDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }
        private bool SaveDamgeReturnOrder(SalesOrderViewModel newSalesOrder,
         SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            bool Result = false;

            salesOrder.SalesOrder.NetDiscount = GetDefaultIfNull(newSalesOrder.SalesOrder.NetDiscount);
            salesOrder.SalesOrder.TotalAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalAmount)).ToString();
            salesOrder.SalesOrder.PaymentDue = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)).ToString();

            salesOrder.SalesOrder.TotalDiscountPercentage = newSalesOrder.SalesOrder.TotalDiscountPercentage;
            salesOrder.SalesOrder.TotalDiscountAmount = newSalesOrder.SalesOrder.TotalDiscountAmount;
            salesOrder.SalesOrder.RecieveAmount = newSalesOrder.SalesOrder.RecieveAmount;
            salesOrder.SalesOrder.VATPercentage = newSalesOrder.SalesOrder.VATPercentage;
            salesOrder.SalesOrder.VATAmount = newSalesOrder.SalesOrder.VATAmount;
            salesOrder.SalesOrder.AdjAmount = newSalesOrder.SalesOrder.AdjAmount;

            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.CustomerId = formCollection["SuppliersId"];
            salesOrder.SalesOrder.Status = ((int)EnumPurchaseType.DamageReturn).ToString();
            //removing unchanged previous order
            salesOrder.SODetails.Where(x => !string.IsNullOrEmpty(x.SODetailId) && x.Status == default(int)).ToList()
                .ForEach(x => salesOrder.SODetails.Remove(x));

            if (!ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            {
                string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.ChallanNo));
                salesOrder.SalesOrder.InvoiceNo = invNo;
            }

            DataTable dtPurchaseOrder = CreatePurchaseOrderDataTable_DR(salesOrder);
            DataSet dsPurchaseOrderDetail = CreatePODetailDataTable_DR(salesOrder);
            DataTable dtPurchaseOrderDetail = dsPurchaseOrderDetail.Tables[0];
            DataTable dtPOProductDetail = dsPurchaseOrderDetail.Tables[1];
            DataTable dtStock = null;
            DataTable dtStockDetail = null;
            #region Log
            log.Info(new { SalesOrder = salesOrder.SalesOrder, SODetails = salesOrder.SODetails });
            #endregion
            if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            {
                Result = _purchaseOrderService.UpdateDeliveryOrderUsingSP(int.Parse(salesOrder.SalesOrder.SalesOrderId), dtPurchaseOrder, dtPurchaseOrderDetail, dtPOProductDetail, dtStock, dtStockDetail);
                if (Result)
                {
                    TempData["POrderID"] = int.Parse(salesOrder.SalesOrder.SalesOrderId);
                    TempData["IsInvoiceReadyById"] = true;
                    AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
                }
                else
                    AddToastMessage("", "Order has been Failed.", ToastType.Error);
            }
            else
            {
                Result = _purchaseOrderService.AddDeliveryOrderUsingSP(dtPurchaseOrder, dtPurchaseOrderDetail, dtPOProductDetail, dtStock, dtStockDetail);
                if (Result)
                {
                    //POrder pOrder = new POrder();
                    //pOrder = _mapper.Map<CreatePurchaseOrderViewModel, POrder>(purchaseOrder.PurchaseOrder);
                    //pOrder.POrderDetails = _mapper.Map<IEnumerable<CreatePurchaseOrderDetailViewModel>, List<POrderDetail>>(purchaseOrder.PODetails);
                    //TempData["POInvoiceData"] = pOrder;
                    //TempData["IsInvoiceReady"] = true;
                    var POrder = _purchaseOrderService.GetDamagerReturnOrderByChallanNo(salesOrder.SalesOrder.InvoiceNo);
                    TempData["POrderID"] = POrder.POrderID;
                    TempData["IsInvoiceReadyById"] = true;
                    AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
                }
                else
                    AddToastMessage("", "Order has been Failed.", ToastType.Error);
            }
            return Result;
        }

        private void SaveOrder(PurchaseOrderViewModel newPurchaseOrder, PurchaseOrderViewModel purchaseOrder, FormCollection formCollection, bool IsDeliveryOrder)
        {
            bool Result = false;

            purchaseOrder.PurchaseOrder.TotalDiscountPercentage = newPurchaseOrder.PurchaseOrder.TotalDiscountPercentage;
            purchaseOrder.PurchaseOrder.TotalDiscountAmount = newPurchaseOrder.PurchaseOrder.TotalDiscountAmount;
            purchaseOrder.PurchaseOrder.NetDiscount = GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.NetDiscount);

            purchaseOrder.PurchaseOrder.TotalAmount = decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.TotalAmount)).ToString();
            purchaseOrder.PurchaseOrder.PaymentDue = decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.PaymentDue)).ToString();
            purchaseOrder.PurchaseOrder.RecieveAmount = newPurchaseOrder.PurchaseOrder.RecieveAmount;
            purchaseOrder.PurchaseOrder.AdjAmount = decimal.Parse(GetDefaultIfNull(newPurchaseOrder.PurchaseOrder.AdjAmount)).ToString();
            purchaseOrder.PurchaseOrder.LabourCost = newPurchaseOrder.PurchaseOrder.LabourCost;
            purchaseOrder.PurchaseOrder.IsDamagePO = newPurchaseOrder.PurchaseOrder.IsDamagePO;
            purchaseOrder.PurchaseOrder.Remarks = newPurchaseOrder.PurchaseOrder.Remarks;
            purchaseOrder.PurchaseOrder.OrderDate = formCollection["OrderDate"];
            purchaseOrder.PurchaseOrder.SupplierId = formCollection["SuppliersId"];
            purchaseOrder.PurchaseOrder.TotalDue = (decimal.Parse(GetDefaultIfNull(formCollection["PurchaseOrder.CurrentDue"])) + decimal.Parse(purchaseOrder.PurchaseOrder.PaymentDue)).ToString();
            if (IsDeliveryOrder)
                purchaseOrder.PurchaseOrder.Status = Convert.ToString((int)EnumPurchaseType.DeliveryOrder);
            else
                purchaseOrder.PurchaseOrder.Status = Convert.ToString((int)EnumPurchaseType.Purchase);

            purchaseOrder.PurchaseOrder.IsDamagePO = newPurchaseOrder.PurchaseOrder.IsDamagePO;

            //removing unchanged previous order
            purchaseOrder.PODetails.Where(x => !string.IsNullOrEmpty(x.PODetailId) && x.Status == default(int)).ToList()
                .ForEach(x => purchaseOrder.PODetails.Remove(x));
            log.Info(new { purchaseOrder.PurchaseOrder, purchaseOrder.PODetails });
            DataTable dtPurchaseOrder = CreatePurchaseOrderDataTable(purchaseOrder);
            DataTable dtStock = CreateStockDataTable(purchaseOrder);
            DataSet dsPurchaseOrderDetail = CreatePODetailDataTable(purchaseOrder, dtStock);
            DataTable dtPurchaseOrderDetail = dsPurchaseOrderDetail.Tables[0];
            DataTable dtPOProductDetail = dsPurchaseOrderDetail.Tables[1];
            DataTable dtStockDetail = dsPurchaseOrderDetail.Tables[2];

            if (IsDeliveryOrder)
            {

                if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
                {
                    Result = _purchaseOrderService.UpdateDeliveryOrderUsingSP(int.Parse(purchaseOrder.PurchaseOrder.PurchaseOrderId), dtPurchaseOrder, dtPurchaseOrderDetail, dtPOProductDetail, dtStock, dtStockDetail);
                    if (Result)
                    {
                        TempData["POrderID"] = int.Parse(purchaseOrder.PurchaseOrder.PurchaseOrderId);
                        TempData["IsInvoiceReadyById"] = true;
                        AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
                    }
                    else
                        AddToastMessage("", "Order has been Failed.", ToastType.Error);
                }
                else
                {
                    Result = _purchaseOrderService.AddDeliveryOrderUsingSP(dtPurchaseOrder, dtPurchaseOrderDetail, dtPOProductDetail, dtStock, dtStockDetail);
                    if (Result)
                    {
                        POrder pOrder = new POrder();
                        pOrder = _mapper.Map<CreatePurchaseOrderViewModel, POrder>(purchaseOrder.PurchaseOrder);
                        pOrder.POrderDetails = _mapper.Map<IEnumerable<CreatePurchaseOrderDetailViewModel>, List<POrderDetail>>(purchaseOrder.PODetails);
                        TempData["POInvoiceData"] = pOrder;
                        TempData["IsInvoiceReady"] = true;
                        AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
                    }
                    else
                        AddToastMessage("", "Order has been Failed.", ToastType.Error);
                }
            }
            else
            {

                if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
                {
                    Result = _purchaseOrderService.UpdatePurchaseOrderUsingSP(int.Parse(purchaseOrder.PurchaseOrder.PurchaseOrderId), dtPurchaseOrder, dtPurchaseOrderDetail, dtPOProductDetail, dtStock, dtStockDetail);
                    if (Result)
                    {
                        TempData["POrderID"] = int.Parse(purchaseOrder.PurchaseOrder.PurchaseOrderId);
                        TempData["IsInvoiceReadyById"] = true;
                        AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
                    }
                    else
                        AddToastMessage("", "Order has been Failed.", ToastType.Error);
                }
                else
                {
                    Result = _purchaseOrderService.AddPurchaseOrderUsingSP(dtPurchaseOrder, dtPurchaseOrderDetail, dtPOProductDetail, dtStock, dtStockDetail);
                    if (Result)
                    {
                        POrder pOrder = new POrder();
                        pOrder = _mapper.Map<CreatePurchaseOrderViewModel, POrder>(purchaseOrder.PurchaseOrder);
                        pOrder.POrderDetails = _mapper.Map<IEnumerable<CreatePurchaseOrderDetailViewModel>, List<POrderDetail>>(purchaseOrder.PODetails);
                        TempData["POInvoiceData"] = pOrder;
                        TempData["IsInvoiceReady"] = true;
                        AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
                    }
                    else
                        AddToastMessage("", "Order has been Failed.", ToastType.Error);
                }
            }


            #region Barcode Purpose
            //PrintInvoice oPInvoice = new PrintInvoice();
            //oPInvoice.PrintBarcodeWeb(purchaseOrder);
            #endregion
        }

        private DataTable CreatePurchaseOrderDataTable(PurchaseOrderViewModel purchaseOrder)
        {
            DataTable dtPurchaseOrder = new DataTable();
            dtPurchaseOrder.Columns.Add("OrderDate", typeof(DateTime));
            dtPurchaseOrder.Columns.Add("ChallanNo", typeof(string));
            dtPurchaseOrder.Columns.Add("SupplierId", typeof(int));
            dtPurchaseOrder.Columns.Add("GrandTotal", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TDiscount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TotalAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("RecAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("PaymentDue", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TotalDue", typeof(decimal));
            dtPurchaseOrder.Columns.Add("AdjAmount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("Status", typeof(int));
            dtPurchaseOrder.Columns.Add("PPTDisAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("NetDiscount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("LabourCost", typeof(decimal));
            dtPurchaseOrder.Columns.Add("ConcernId", typeof(int));
            dtPurchaseOrder.Columns.Add("CreatedDate", typeof(DateTime));
            dtPurchaseOrder.Columns.Add("CreatedBy", typeof(int));
            dtPurchaseOrder.Columns.Add("IsDamageOrder", typeof(int));
            dtPurchaseOrder.Columns.Add("Remarks", typeof(string));
            dtPurchaseOrder.Columns.Add("PayCashAccountId", typeof(int));
            dtPurchaseOrder.Columns.Add("PayBankId", typeof(int));
            DataRow row = null;

            row = dtPurchaseOrder.NewRow();
            row["OrderDate"] = purchaseOrder.PurchaseOrder.OrderDate;
            row["ChallanNo"] = purchaseOrder.PurchaseOrder.ChallanNo;
            row["SupplierId"] = purchaseOrder.PurchaseOrder.SupplierId;
            row["GrandTotal"] = purchaseOrder.PurchaseOrder.GrandTotal;
            row["TDiscount"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDiscountAmount);
            row["TotalAmt"] = purchaseOrder.PurchaseOrder.TotalAmount;
            row["RecAmt"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.RecieveAmount);
            row["PaymentDue"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.PaymentDue);
            row["TotalDue"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDue); ; //(decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.PaymentDue)) - decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDue)));
            row["AdjAmount"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.AdjAmount);
            row["Status"] = purchaseOrder.PurchaseOrder.Status;
            row["PPTDisAmt"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.PPDiscountAmount);
            row["NetDiscount"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.NetDiscount);
            row["LabourCost"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.LabourCost);
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreatedDate"] = DateTime.Now;
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            row["IsDamageOrder"] = purchaseOrder.PurchaseOrder.IsDamagePO ? 1 : 0;
            row["Remarks"] = purchaseOrder.PurchaseOrder.Remarks;
            row["PayCashAccountId"] = purchaseOrder.PurchaseOrder.PayCashAccountId.HasValue ? (object)purchaseOrder.PurchaseOrder.PayCashAccountId.GetValueOrDefault() : DBNull.Value;
            row["PayBankId"] = purchaseOrder.PurchaseOrder.PayBankId.HasValue ? (object)purchaseOrder.PurchaseOrder.PayBankId.GetValueOrDefault() : DBNull.Value;

            dtPurchaseOrder.Rows.Add(row);

            return dtPurchaseOrder;
        }

        private DataTable CreatePurchaseOrderDataTable_DR(SalesOrderViewModel salesOrder)
        {
            DataTable dtPurchaseOrder = new DataTable();
            dtPurchaseOrder.Columns.Add("OrderDate", typeof(DateTime));
            dtPurchaseOrder.Columns.Add("ChallanNo", typeof(string));
            dtPurchaseOrder.Columns.Add("SupplierId", typeof(int));
            dtPurchaseOrder.Columns.Add("GrandTotal", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TDiscount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TotalAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("RecAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("PaymentDue", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TotalDue", typeof(decimal));
            dtPurchaseOrder.Columns.Add("AdjAmount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("Status", typeof(int));
            dtPurchaseOrder.Columns.Add("PPTDisAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("NetDiscount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("LabourCost", typeof(decimal));
            dtPurchaseOrder.Columns.Add("ConcernId", typeof(int));
            dtPurchaseOrder.Columns.Add("CreatedDate", typeof(DateTime));
            dtPurchaseOrder.Columns.Add("CreatedBy", typeof(int));
            dtPurchaseOrder.Columns.Add("IsDamageOrder", typeof(int));
            DataRow row = null;

            row = dtPurchaseOrder.NewRow();
            row["OrderDate"] = salesOrder.SalesOrder.OrderDate;
            row["ChallanNo"] = salesOrder.SalesOrder.InvoiceNo;
            row["SupplierId"] = salesOrder.SalesOrder.CustomerId;
            row["GrandTotal"] = salesOrder.SalesOrder.GrandTotal;
            row["TDiscount"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountAmount);
            row["TotalAmt"] = salesOrder.SalesOrder.TotalAmount;
            row["RecAmt"] = GetDefaultIfNull(salesOrder.SalesOrder.RecieveAmount);
            row["PaymentDue"] = GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue);
            row["TotalDue"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalDue); ; //(decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.PaymentDue)) - decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDue)));
            row["AdjAmount"] = GetDefaultIfNull(salesOrder.SalesOrder.AdjAmount);
            row["Status"] = salesOrder.SalesOrder.Status;
            row["PPTDisAmt"] = GetDefaultIfNull(salesOrder.SalesOrder.PPDiscountAmount);
            row["NetDiscount"] = GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount);
            row["LabourCost"] = 0;// GetDefaultIfNull(salesOrder.SalesOrder.LabourCost);
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreatedDate"] = DateTime.Now;
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            row["IsDamageOrder"] = 0;
            dtPurchaseOrder.Rows.Add(row);

            return dtPurchaseOrder;
        }

        private DataTable CreateStockDataTable(PurchaseOrderViewModel purchaseOrder)
        {
            DataTable dtStock = new DataTable();
            dtStock.Columns.Add("StockId", typeof(int));
            dtStock.Columns.Add("StockCode", typeof(string));
            dtStock.Columns.Add("ColorID", typeof(int));
            dtStock.Columns.Add("Status", typeof(int));
            dtStock.Columns.Add("EntryDate", typeof(DateTime));
            dtStock.Columns.Add("Quantity", typeof(decimal));
            dtStock.Columns.Add("ProductID", typeof(int));
            dtStock.Columns.Add("MRPPrice", typeof(decimal));
            dtStock.Columns.Add("LPPrice", typeof(decimal));
            dtStock.Columns.Add("ConcernID", typeof(int));
            dtStock.Columns.Add("CreateDate", typeof(DateTime));
            dtStock.Columns.Add("CreatedBy", typeof(int));
            dtStock.Columns.Add("GodownID", typeof(int));

            dtStock.Columns.Add("TotalSFT", typeof(decimal));


            DataRow row = null;
            var stocks = _stockService.GetAllStock();

            foreach (var item in purchaseOrder.PODetails)
            {
                var stock = stocks.FirstOrDefault(x => x.ProductID == int.Parse(item.ProductId) &&
                        x.ColorID == int.Parse(item.ColorId) && x.GodownID == int.Parse(item.GodownID));
                row = dtStock.NewRow();

                if (stock != null)
                    row["StockId"] = stock.StockID;
                else
                    row["StockId"] = DBNull.Value;
                row["StockCode"] = item.ProductCode;
                row["ColorID"] = item.ColorId;
                row["Status"] = item.Status;
                row["EntryDate"] = DateTime.Now;
                row["Quantity"] = item.Quantity;
                row["ProductID"] = item.ProductId;
                row["MRPPrice"] = GetDefaultIfNull(item.MRPRate);
                row["LPPrice"] = GetDefaultIfNull(item.UnitPrice);
                row["ConcernID"] = User.Identity.GetConcernId();
                row["CreateDate"] = DateTime.Now;
                row["CreatedBy"] = User.Identity.GetUserId<int>();
                row["GodownID"] = item.GodownID;
                row["TotalSFT"] = item.TotalArea;

                dtStock.Rows.Add(row);
            }

            return dtStock;
        }

        private DataSet CreatePODetailDataTable(PurchaseOrderViewModel purchaseOrder, DataTable dtStock)
        {
            DataSet dsPurchaseOrderDetail = new DataSet();
            DataTable dtPurchaseOrderDetail = new DataTable();
            DataTable dtPOProductDetail = new DataTable();
            DataTable dtStockDetail = new DataTable();
            DataRow row = null;
            int id;

            dtPurchaseOrderDetail.Columns.Add("POrderDetailId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("ProductId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("ColorId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("Status", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("TAmount", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPDisPer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPDisAmt", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("MrpRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("SalesRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("ExtraPPDISPer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("ExtraPPDISAmt", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPOffer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CreditSalesRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CRSalesRate12Month", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CRSalesRate3Month", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("GodownID", typeof(int));

            dtPurchaseOrderDetail.Columns.Add("SFTRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("TotalSFT", typeof(decimal));


            //POProductDetail
            dtPOProductDetail.Columns.Add("ProductID", typeof(int));
            dtPOProductDetail.Columns.Add("ColorId", typeof(int));
            dtPOProductDetail.Columns.Add("IMENO", typeof(string));
            dtPOProductDetail.Columns.Add("DamagePOPDID", typeof(int));


            // StockDetail
            dtStockDetail.Columns.Add("StockCode", typeof(string));
            dtStockDetail.Columns.Add("ProductID", typeof(int));
            dtStockDetail.Columns.Add("ColorId", typeof(int));
            dtStockDetail.Columns.Add("IMENO", typeof(string));
            dtStockDetail.Columns.Add("Status", typeof(int));
            dtStockDetail.Columns.Add("SalesRate", typeof(decimal));
            dtStockDetail.Columns.Add("CreditSRate", typeof(decimal));
            dtStockDetail.Columns.Add("Quantity", typeof(decimal));
            dtStockDetail.Columns.Add("GodownID", typeof(int));
            dtStockDetail.Columns.Add("SFTRate", typeof(decimal));
            dtStockDetail.Columns.Add("TotalSFT", typeof(decimal));


            foreach (var item in purchaseOrder.PODetails)
            {
                row = dtPurchaseOrderDetail.NewRow();
                id = int.Parse(GetDefaultIfNull(item.PODetailId));

                if (id > 0)
                    row["POrderDetailId"] = id;
                else
                    row["POrderDetailId"] = DBNull.Value;

                row["ProductId"] = item.ProductId;
                row["ColorId"] = item.ColorId;
                row["Status"] = (int)item.Status;
                row["Quantity"] = item.Quantity;
                row["UnitPrice"] = item.UnitPrice;
                row["TAmount"] = item.TAmount;
                row["PPDisPer"] = GetDefaultIfNull(item.PPDisPercentage);
                row["PPDisAmt"] = GetDefaultIfNull(item.PPDiscountAmount);
                row["MrpRate"] = GetDefaultIfNull(item.MRPRate);
                row["SalesRate"] = GetDefaultIfNull(item.SalesRate);
                row["ExtraPPDISPer"] = GetDefaultIfNull(item.ExtraPPDISPer);
                row["ExtraPPDISAmt"] = GetDefaultIfNull(item.ExtraPPDISAmt);
                row["PPOffer"] = GetDefaultIfNull(item.PPOffer);
                row["CreditSalesRate"] = GetDefaultIfNull(item.CreditSalesRate);
                row["CRSalesRate12Month"] = GetDefaultIfNull(item.CRSalesRate12Month);
                row["CRSalesRate3Month"] = GetDefaultIfNull(item.CRSalesRate3Month);
                row["Quantity"] = GetDefaultIfNull(item.Quantity);
                row["GodownID"] = GetDefaultIfNull(item.GodownID);

                row["SFTRate"] = item.RatePerArea;
                row["TotalSFT"] = item.TotalArea;


                //POProductDetail
                CreatePOProductDetailDataTable(item, dtPOProductDetail);

                //StockDetail
                CreateStockDetailDataTable(dtStock, item, dtStockDetail);

                dtPurchaseOrderDetail.Rows.Add(row);
            }

            dsPurchaseOrderDetail.Tables.Add(dtPurchaseOrderDetail);
            dsPurchaseOrderDetail.Tables.Add(dtPOProductDetail);
            dsPurchaseOrderDetail.Tables.Add(dtStockDetail);

            return dsPurchaseOrderDetail;
        }
        private DataSet CreatePODetailDataTable_DR(SalesOrderViewModel salesOrder)
        {
            DataSet dsPurchaseOrderDetail = new DataSet();
            DataTable dtPurchaseOrderDetail = new DataTable();
            DataTable dtPOProductDetail = new DataTable();
            DataRow row = null;
            int id = 0;

            dtPurchaseOrderDetail.Columns.Add("POrderDetailId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("ProductId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("ColorId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("Status", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("TAmount", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPDisPer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPDisAmt", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("MrpRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("SalesRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("ExtraPPDISPer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("ExtraPPDISAmt", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPOffer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CreditSalesRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CRSalesRate12Month", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CRSalesRate3Month", typeof(decimal));
            //POProductDetail
            dtPOProductDetail.Columns.Add("ProductID", typeof(int));
            dtPOProductDetail.Columns.Add("ColorId", typeof(int));
            dtPOProductDetail.Columns.Add("IMENO", typeof(string));
            dtPOProductDetail.Columns.Add("DamagePOPDID", typeof(int));

            //StockDetail

            var SDetails = from sod in salesOrder.SODetails
                           group sod by new { sod.ProductId, sod.ColorId } into g
                           select new CreateSalesOrderDetailViewModel
                           {
                               ProductId = g.Key.ProductId,
                               ColorId = g.Key.ColorId,
                               Status = g.Select(i => i.Status).FirstOrDefault(),
                               Quantity = g.Sum(i => Convert.ToInt32(i.Quantity)).ToString(),
                               UTAmount = g.Sum(i => Convert.ToDecimal(i.UTAmount)).ToString(),
                               UnitPrice = g.Select(i => i.UnitPrice).FirstOrDefault(),
                               PPDPercentage = g.Select(i => i.PPDPercentage).FirstOrDefault(),
                               PPDAmount = g.Select(i => i.PPDAmount).FirstOrDefault(),
                               MRPRate = g.Select(i => i.MRPRate).FirstOrDefault(),
                               IMEIList = g.Select(i => i.IMENo).ToList()
                           };



            foreach (var item in SDetails)
            {
                row = dtPurchaseOrderDetail.NewRow();
                //id = int.Parse(GetDefaultIfNull(item.PODetailId));

                if (id > 0)
                    row["POrderDetailId"] = id;
                else
                    row["POrderDetailId"] = DBNull.Value;

                row["ProductId"] = item.ProductId;
                row["ColorId"] = item.ColorId;
                row["Status"] = (int)item.Status;
                row["Quantity"] = item.Quantity;
                row["UnitPrice"] = item.UnitPrice;
                row["TAmount"] = item.UTAmount;
                row["PPDisPer"] = GetDefaultIfNull(item.PPDPercentage);
                row["PPDisAmt"] = GetDefaultIfNull(item.PPDAmount);
                row["MrpRate"] = GetDefaultIfNull(item.MRPRate);
                row["SalesRate"] = 0;// GetDefaultIfNull(item.SalesRate);
                row["ExtraPPDISPer"] = 0;// GetDefaultIfNull(item.ExtraPPDISPer);
                row["ExtraPPDISAmt"] = 0;//GetDefaultIfNull(item.ExtraPPDISAmt);
                row["PPOffer"] = 0;// GetDefaultIfNull(item.PPOffer);
                row["CreditSalesRate"] = 0;// GetDefaultIfNull(item.CreditSalesRate);
                row["CRSalesRate12Month"] = 0;
                row["CRSalesRate3Month"] = 0;

                //POProductDetail
                CreatePOProductDetailDataTable_DR(item, dtPOProductDetail);


                dtPurchaseOrderDetail.Rows.Add(row);
            }

            dsPurchaseOrderDetail.Tables.Add(dtPurchaseOrderDetail);
            dsPurchaseOrderDetail.Tables.Add(dtPOProductDetail);
            return dsPurchaseOrderDetail;
        }

        private void CreatePOProductDetailDataTable(CreatePurchaseOrderDetailViewModel poDetail, DataTable dtPOProductDetail)
        {
            DataRow row = null;
            foreach (var item in poDetail.POProductDetails)
            {
                row = dtPOProductDetail.NewRow();
                row["ProductID"] = item.ProductID;
                row["ColorID"] = poDetail.ColorId;
                row["IMENO"] = item.IMENO.Trim();
                row["DamagePOPDID"] = item.DamagePOPDID ?? 0;
                dtPOProductDetail.Rows.Add(row);
            }
        }
        private void CreatePOProductDetailDataTable_DR(CreateSalesOrderDetailViewModel poDetail, DataTable dtPOProductDetail)
        {
            DataRow row = null;
            foreach (var item in poDetail.IMEIList)
            {
                row = dtPOProductDetail.NewRow();
                row["ProductID"] = poDetail.ProductId;
                row["ColorID"] = poDetail.ColorId;
                row["IMENO"] = item.Trim();
                row["DamagePOPDID"] = 0;
                dtPOProductDetail.Rows.Add(row);
            }

        }

        private void CreateStockDetailDataTable(DataTable dtStock, CreatePurchaseOrderDetailViewModel poDetail,
            DataTable dtStockDetail)
        {
            DataRow row = null;
            foreach (var item in poDetail.POProductDetails)
            {
                row = dtStockDetail.NewRow();
                row["StockCode"] = dtStock.AsEnumerable().Where(x => x.Field<int>("ProductID") == item.ProductID).
                    Select(x => x.Field<string>("StockCode")).First();
                row["ProductID"] = item.ProductID;
                row["ColorID"] = poDetail.ColorId;
                row["IMENO"] = item.IMENO.Trim();
                row["Status"] = (int)EnumStockStatus.Stock;
                row["SalesRate"] = GetDefaultIfNull(poDetail.SalesRate);
                row["CreditSRate"] = GetDefaultIfNull(poDetail.CreditSalesRate);
                row["GodownID"] = poDetail.GodownID;

                row["SFTRate"] = poDetail.RatePerArea;
                row["TotalSFT"] = poDetail.TotalArea;


                dtStockDetail.Rows.Add(row);
            }
        }

        private bool IsForEdit(string previousAction)
        {
            return previousAction.Equals("edit");
        }

        private int GetUpdatedQuantity(int prevQuantity, int currQuantity)
        {
            int val = prevQuantity;
            if (prevQuantity > currQuantity)
                val = prevQuantity - currQuantity;
            else if (prevQuantity < currQuantity)
                val = currQuantity - prevQuantity;

            return val;
        }

        private ActionResult ReturnCreateViewWithTempData(bool IsDeliveryOrder)
        {
            ViewBag.SupplierIds = GetAllSupplierForDDL();
            ViewBag.ProductIds = GetAllProductForDDL();
            var payItems = _cashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();

            var fpay = _cashCollectionService.GetAllPayTypeHeadForPO().FirstOrDefault();

            PurchaseOrderViewModel purchaseOrder = (PurchaseOrderViewModel)TempData.Peek("purchaseOrderViewModel");
            if (purchaseOrder != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                purchaseOrder.PurchaseOrder.PayItems = payItems;
                TempData["purchaseOrderViewModel"] = purchaseOrder;
                if (IsDeliveryOrder)
                    return View("CreateDeliveryOrder", purchaseOrder);
                else
                    return View("Create", purchaseOrder);
            }
            else
            {
                string chllnNo = _miscellaneousService.GetUniqueKey(x => x.POrderID);
                var color = _colorService.GetAllColor().FirstOrDefault();
                var godown = _godownService.GetAllGodown().FirstOrDefault();
                return View(new PurchaseOrderViewModel
                {
                    PODetail = new CreatePurchaseOrderDetailViewModel() { ColorId = color.ColorID.ToString(), GodownID = godown.GodownID.ToString() },
                    PODetails = new List<CreatePurchaseOrderDetailViewModel>(),
                    PurchaseOrder = new CreatePurchaseOrderViewModel { ChallanNo = chllnNo, PayItems = payItems, ItemId = fpay.ExpenseItemID.ToString() }
                });
            }
        }
        private ActionResult DamageReturnCreateViewWithTempData()
        {
            SalesOrderViewModel SalesOrde = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
            if (SalesOrde != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                TempData["salesOrderViewModel"] = SalesOrde;
                return View("CreateDamageReturn", SalesOrde);
            }
            else
            {
                string chllnNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.ChallanNo));
                var color = _colorService.GetAllColor();
                return View(new SalesOrderViewModel
                {
                    SalesOrder = new CreateSalesOrderViewModel() { InvoiceNo = chllnNo },
                    SODetail = new CreateSalesOrderDetailViewModel(),
                    SODetails = new List<CreateSalesOrderDetailViewModel>()
                });
            }
        }

        private void CheckAndAddModelErrorForAdd(SalesOrderViewModel newSalesOrder,
        SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["OrderDate"]))
                ModelState.AddModelError("SalesOrder.OrderDate", "Sales Date is required");

            if (string.IsNullOrEmpty(formCollection["SuppliersId"]))
                ModelState.AddModelError("SalesOrder.CustomerId", "Supplier is required");
            //ProductDetailsId is ProductId
            if (string.IsNullOrEmpty(formCollection["ProductDetailsId"]))
                ModelState.AddModelError("SODetail.ProductId", "Product is required");
            else
            {
                newSalesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
                salesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
            }

            if (string.IsNullOrEmpty(newSalesOrder.SODetail.Quantity) || Convert.ToInt32(double.Parse(newSalesOrder.SODetail.Quantity)) <= 0)
            {
                ModelState.AddModelError("SODetail.Quantity", "Quantity is required");
            }

            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InvoiceNo))
                ModelState.AddModelError("SalesOrder.InvoiceNo", "Invoice No. is required");

            if (string.IsNullOrEmpty(newSalesOrder.SODetail.MRPRate))
                ModelState.AddModelError("SODetail.MRPRate", "Purchase Rate is required");

            if (string.IsNullOrEmpty(newSalesOrder.SODetail.UnitPrice))
                ModelState.AddModelError("SODetail.UnitPrice", "Sales Rate is required");

            if (string.IsNullOrEmpty(newSalesOrder.SODetail.IMENo))
            {
                ModelState.AddModelError("SODetail.IMENo", "IMENo/Barcode is required");
            }
            else
            {
                //var stockDetails = _stockDetailService.GetStockDetailByProductId(
                //    int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

                //if (stockDetails.Count() < int.Parse(newSalesOrder.SODetail.Quantity))
                //{
                //    ModelState.AddModelError("SODetail.Quantity", "Stock is not available. Stock Quantity: " + stockDetails.Count());
                //}
                //if (!stockDetails.Any(x => x.IMENO.Equals(newSalesOrder.SODetail.IMENo)))
                //    ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");

                var product = _productService.GetProductById(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));


                if (product.ProductType == (int)EnumProductType.NoBarcode)
                {
                    var stockCount = _stockService.GetStockByProductId(product.ProductID);

                    if (stockCount.Quantity < int.Parse(newSalesOrder.SODetail.Quantity))
                        ModelState.AddModelError("SODetail.Quantity", "Stock is not available. Stock Quantity: " + stockCount.Quantity);
                }
                else
                {
                    var stockDetails = _stockDetailService.GetStockDetailByProductId(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

                    if (!stockDetails.Any(x => x.IMENO.Equals(newSalesOrder.SODetail.IMENo)))
                        ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");
                }
            }
        }

        private void AddToDamageReturnOrder(SalesOrderViewModel newSalesOrder,
       SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            decimal quantity = decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.Quantity));
            decimal totalDisAmount = quantity * decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPDAmount));
            decimal totalOffer = quantity * decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPOffer));

            salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.GrandTotal)) +
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.UTAmount)) + totalDisAmount + totalOffer).ToString();

            salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PPDiscountAmount)) +
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPDAmount))).ToString();

            salesOrder.SalesOrder.TotalDiscountPercentage = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalDiscountPercentage)).ToString();
            salesOrder.SalesOrder.TotalDiscountAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalDiscountAmount)).ToString();
            salesOrder.SalesOrder.TempFlatDiscountAmount = salesOrder.SalesOrder.TotalDiscountAmount;

            salesOrder.SalesOrder.VATPercentage = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATPercentage)).ToString();
            salesOrder.SalesOrder.VATAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATAmount)).ToString();

            salesOrder.SalesOrder.AdjAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.AdjAmount)).ToString();

            //salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) + decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPDAmount)) +
            //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPOffer))).ToString();
            // decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountAmount)) +

            salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) + totalDisAmount + totalOffer).ToString();

            var netTotal = ((decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.VATAmount))) -
                (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.AdjAmount))));

            // For Total Offer Purpose



            salesOrder.SalesOrder.TotalOffer = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalOffer)) + totalOffer).ToString();

            salesOrder.SalesOrder.TotalAmount = netTotal.ToString();
            salesOrder.SalesOrder.PaymentDue = (netTotal - decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount))).ToString();
            salesOrder.SalesOrder.RecieveAmount = GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount);

            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.CustomerId = formCollection["SuppliersId"];

            salesOrder.SODetail.SODetailId = newSalesOrder.SODetail.SODetailId;
            salesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
            salesOrder.SODetail.ColorId = formCollection["ColorsId"];
            salesOrder.SODetail.ColorName = newSalesOrder.SODetail.ColorName;
            salesOrder.SODetail.StockDetailId = formCollection["StockDetailsId"];
            salesOrder.SODetail.ProductCode = formCollection["ProductDetailsCode"];
            salesOrder.SODetail.IMENo = newSalesOrder.SODetail.IMENo;
            salesOrder.SODetail.Quantity = newSalesOrder.SODetail.Quantity;
            salesOrder.SODetail.PPDPercentage = newSalesOrder.SODetail.PPDPercentage;
            //salesOrder.SODetail.PPDAmount = newSalesOrder.SODetail.PPDAmount;
            salesOrder.SODetail.PPDAmount = totalDisAmount.ToString();
            salesOrder.SODetail.UnitPrice = newSalesOrder.SODetail.UnitPrice;
            salesOrder.SODetail.MRPRate = newSalesOrder.SODetail.MRPRate;
            salesOrder.SODetail.UTAmount = newSalesOrder.SODetail.UTAmount;
            salesOrder.SODetail.ProductName = formCollection["ProductDetailsName"];
            salesOrder.SODetail.Status = newSalesOrder.SODetail.Status == default(int) ? EnumStatus.New : newSalesOrder.SODetail.Status;
            //salesOrder.SODetail.PPOffer = newSalesOrder.SODetail.PPOffer;
            salesOrder.SODetail.PPOffer = totalOffer.ToString();
            salesOrder.SODetail.CompressorWarrentyMonth = newSalesOrder.SODetail.CompressorWarrentyMonth;
            salesOrder.SODetail.MotorWarrentyMonth = newSalesOrder.SODetail.MotorWarrentyMonth;
            salesOrder.SODetail.PanelWarrentyMonth = newSalesOrder.SODetail.PanelWarrentyMonth;
            salesOrder.SODetail.SparePartsWarrentyMonth = newSalesOrder.SODetail.SparePartsWarrentyMonth;
            salesOrder.SODetail.ServiceWarrentyMonth = newSalesOrder.SODetail.ServiceWarrentyMonth;

            salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
            salesOrder.SODetails.Add(salesOrder.SODetail);

            SalesOrderViewModel vm = new SalesOrderViewModel
            {
                SODetail = new CreateSalesOrderDetailViewModel(),
                SODetails = salesOrder.SODetails,
                SalesOrder = salesOrder.SalesOrder
            };

            TempData["salesOrderViewModel"] = vm;
            salesOrder.SODetail = new CreateSalesOrderDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }
        private ActionResult HandleDamageReturnOrder(SalesOrderViewModel newSalesOrder, FormCollection formCollection)
        {
            if (newSalesOrder != null)
            {
                SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
                salesOrder = salesOrder ?? new SalesOrderViewModel()
                {
                    SalesOrder = newSalesOrder.SalesOrder
                };
                salesOrder.SODetail = new CreateSalesOrderDetailViewModel();

                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newSalesOrder, salesOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        return View("CreateDamageReturn", salesOrder);
                    }
                    var product = _productService.GetProductById(int.Parse(newSalesOrder.SODetail.ProductId));
                    if (salesOrder.SODetails != null && product.ProductType == 1 &&
                        salesOrder.SODetails.Any(x => x.Status != EnumStatus.Updated && x.Status != EnumStatus.Deleted &&
                        x.IMENo.Equals(newSalesOrder.SODetail.IMENo)))
                    {
                        AddToastMessage(string.Empty, "This product already exists in the order", ToastType.Error);
                        return View("CreateDamageReturn", salesOrder);
                    }

                    AddToDamageReturnOrder(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View("CreateDamageReturn", salesOrder);
                }
                else if (formCollection.Get("submitButton") != null)
                {
                    //CheckAndAddModelErrorForSave(newSalesOrder, salesOrder, formCollection);
                    decimal calGrandtotal = salesOrder.SODetails.Where(i => i.Status != EnumStatus.Deleted).Sum(i => Convert.ToDecimal(i.UnitPrice) * Convert.ToDecimal(i.Quantity));

                    if ((Convert.ToDecimal(newSalesOrder.SalesOrder.GrandTotal) != calGrandtotal) || (Convert.ToDecimal(newSalesOrder.SalesOrder.GrandTotal) != (Convert.ToDecimal(newSalesOrder.SalesOrder.TotalAmount) + Convert.ToDecimal(newSalesOrder.SalesOrder.NetDiscount) + Convert.ToDecimal(newSalesOrder.SalesOrder.AdjAmount) - Convert.ToDecimal(newSalesOrder.SalesOrder.VATAmount))))
                    {
                        TempData["salesOrderViewModel"] = null;
                        AddToastMessage("", "Order has been failed. Please try again.", ToastType.Error);
                        return RedirectToAction("DamageReturnOrders");
                    }

                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        return View("CreateDamageReturn", salesOrder);
                    }
                    bool Result = SaveDamgeReturnOrder(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    TempData["salesOrderViewModel"] = null;

                    return RedirectToAction("DamageReturnOrders");
                }
                else
                {
                    return View("CreateDamageReturn", new PurchaseOrderViewModel
                    {
                        PODetail = new CreatePurchaseOrderDetailViewModel(),
                        PODetails = new List<CreatePurchaseOrderDetailViewModel>(),
                        PurchaseOrder = new CreatePurchaseOrderViewModel()
                    });
                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }
        private ActionResult HandlePurchaseOrder(PurchaseOrderViewModel newPurchaseOrder, FormCollection formCollection, bool IsDeliveryOrder, string ItemType)
        {
            ViewBag.SupplierIds = GetAllSupplierForDDL();
            ViewBag.ProductIds = GetAllProductForDDL();
            if (newPurchaseOrder != null)
            {
                var payItems = _cashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
                {
                    Value = d.ExpenseItemID.ToString(),
                    Text = d.Name
                }).ToList();
                TempData["POProductDetails"] = null;
                PurchaseOrderViewModel purchaseOrder = (PurchaseOrderViewModel)TempData.Peek("purchaseOrderViewModel");
                purchaseOrder = purchaseOrder ?? new PurchaseOrderViewModel()
                {
                    PurchaseOrder = newPurchaseOrder.PurchaseOrder
                };
                purchaseOrder.PODetail = new CreatePurchaseOrderDetailViewModel();

                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newPurchaseOrder, formCollection, IsDeliveryOrder);
                    purchaseOrder.PODetails = purchaseOrder.PODetails ?? new List<CreatePurchaseOrderDetailViewModel>();
                    GetPickersValue(purchaseOrder, newPurchaseOrder, formCollection);
                    AddTempPOProductDetails(newPurchaseOrder, formCollection, IsDeliveryOrder);
                    purchaseOrder.PurchaseOrder.PayItems = payItems;

                    if (!ModelState.IsValid)
                    {
                        if (IsDeliveryOrder)
                            return View("CreateDeliveryOrder", purchaseOrder);
                        else
                            return View("Create", purchaseOrder);
                    }
                    else if ((HasDuplicateIMEIORBarcode(newPurchaseOrder, purchaseOrder, formCollection)
                        && int.Parse(GetDefaultIfNull(formCollection["ProductsType"])) == (int)EnumProductType.ExistingBC)
                        || HasDuplicateIMEIORBarcodeInRecentAddToOrder(formCollection, purchaseOrder))
                    {
                        AddToastMessage("", "Duplicate IMEI/Barcode found", ToastType.Error);
                        if (IsDeliveryOrder)
                            return View("CreateDeliveryOrder", purchaseOrder);
                        else
                            return View("Create", purchaseOrder);
                    }
                    else if (purchaseOrder.PODetails.Any(x => x.ProductId.Equals(formCollection["ProductsId"]) &&
                        x.ColorId.Equals(string.IsNullOrEmpty(formCollection["ColorsId"]) ? "1" : formCollection["ColorsId"])
                        && x.Status != EnumStatus.Deleted))
                    {
                        AddToastMessage("", "This product has already been added in the order", ToastType.Error);
                        if (IsDeliveryOrder)
                            return View("CreateDeliveryOrder", purchaseOrder);
                        else
                            return View("Create", purchaseOrder);
                    }

                    AddToOrder(newPurchaseOrder, purchaseOrder, formCollection, IsDeliveryOrder);
                    ModelState.Clear();
                    TempData["POProductDetails"] = null;
                    if (IsDeliveryOrder)
                        return View("CreateDeliveryOrder", purchaseOrder);
                    else
                        return View("Create", purchaseOrder);
                }
                else if (formCollection.Get("submitButton") != null)
                {
                    if (!IsDeliveryOrder)
                        CheckAndAddModelErrorForSave(newPurchaseOrder, purchaseOrder, formCollection);

                    if (!IsDateValid(Convert.ToDateTime(newPurchaseOrder.PurchaseOrder.OrderDate)))
                    {
                        ModelState.AddModelError("PurchaseOrder.OrderDate", "Back dated entry is not valid.");
                    }

                    if (!ModelState.IsValid)
                    {
                        purchaseOrder.PODetails = purchaseOrder.PODetails ?? new List<CreatePurchaseOrderDetailViewModel>();
                        if (IsDeliveryOrder)
                            return View("CreateDeliveryOrder", purchaseOrder);
                        else
                            return View("Create", purchaseOrder);
                    }
                    else if (purchaseOrder.PODetails == null || purchaseOrder.PODetails.Count <= 0)
                    {
                        AddToastMessage("", "No order data found to save.", ToastType.Error);
                        if (IsDeliveryOrder)
                            return View("CreateDeliveryOrder", purchaseOrder);
                        else
                            return View("Create", purchaseOrder);
                    }

                    decimal calGrandTotal = purchaseOrder.PODetails.Where(i => i.Status != EnumStatus.Deleted).Sum(i => (Convert.ToDecimal(i.UnitPrice) + Convert.ToDecimal(i.PPDiscountAmount) + Convert.ToDecimal(i.PPOffer) + Convert.ToDecimal(i.ExtraPPDISAmt)) * Convert.ToDecimal(i.Quantity));

                    purchaseOrder.PurchaseOrder.PayItems = payItems;
                    if ((Math.Truncate(Convert.ToDecimal(newPurchaseOrder.PurchaseOrder.GrandTotal)) != Math.Truncate(calGrandTotal)) ||
                        (Math.Truncate(Convert.ToDecimal(newPurchaseOrder.PurchaseOrder.GrandTotal))
                        + Math.Truncate(Convert.ToDecimal(newPurchaseOrder.PurchaseOrder.LabourCost))
                        )
                        != Math.Truncate(Convert.ToDecimal(newPurchaseOrder.PurchaseOrder.TotalAmount)
                        + Convert.ToDecimal(newPurchaseOrder.PurchaseOrder.NetDiscount)
                        + Convert.ToDecimal(newPurchaseOrder.PurchaseOrder.AdjAmount))
                       )
                    {
                        TempData["purchaseOrderViewModel"] = null;
                        ModelState.Clear();
                        AddToastMessage("", "Order has been failed. Please try again.", ToastType.Error);
                        if (IsDeliveryOrder)
                            return RedirectToAction("DeliveryOrders");
                        else
                            return RedirectToAction("Index");
                    }

                    purchaseOrder.PurchaseOrder.PayCashAccountId = ItemType.Equals("CA") ? int.Parse(newPurchaseOrder.PurchaseOrder.ItemId) : (int?)null;
                    purchaseOrder.PurchaseOrder.PayBankId = ItemType.Equals("B") ? int.Parse(newPurchaseOrder.PurchaseOrder.ItemId) : (int?)null;

                    SaveOrder(newPurchaseOrder, purchaseOrder, formCollection, IsDeliveryOrder);
                    TempData["purchaseOrderViewModel"] = null;
                    ModelState.Clear();
                    if (IsDeliveryOrder)
                        return RedirectToAction("DeliveryOrders");
                    else
                        return RedirectToAction("Index");
                }
                else
                {
                    if (IsDeliveryOrder)
                    {
                        return View("CreateDeliveryOrder", new PurchaseOrderViewModel
                        {
                            PODetail = new CreatePurchaseOrderDetailViewModel(),
                            PODetails = new List<CreatePurchaseOrderDetailViewModel>(),
                            PurchaseOrder = new CreatePurchaseOrderViewModel()
                        });
                    }
                    else
                        return View("Create", new PurchaseOrderViewModel
                        {
                            PODetail = new CreatePurchaseOrderDetailViewModel(),
                            PODetails = new List<CreatePurchaseOrderDetailViewModel>(),
                            PurchaseOrder = new CreatePurchaseOrderViewModel()
                        });
                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                if (IsDeliveryOrder)
                    return RedirectToAction("CreateDeliveryOrder");
                else
                    return RedirectToAction("Create");
            }
        }

        private bool HasSoldProductCheckByPOId(int id)
        {
            int sold = _purchaseOrderService.CheckProductStatusByPOId(id);
            return sold > 0;
        }

        private bool HasSoldProductCheckByPODetailId(int id)
        {
            int sold = _purchaseOrderService.CheckProductStatusByPODetailId(id);
            return sold > 0;
        }
        /// <summary>
        /// This method Check in Database and current add to order Product
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        private bool HasDuplicateIMEIORBarcode(PurchaseOrderViewModel newPurchaseOrder, PurchaseOrderViewModel purchaseOrder, FormCollection formCollection)
        {
            bool isDuplicate = false;
            string[] IMEIS = formCollection.AllKeys
                             .Where(key => key.StartsWith("IMEINo"))
                             .Select(key => formCollection[key])
                             .ToArray();

            if (!ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            {
                isDuplicate = formCollection.AllKeys
                        .Where(key => key.StartsWith("IMEINo"))
                        .Any(key => _purchaseOrderService.CheckIMENoDuplicacyByConcernId(User.Identity.GetConcernId(), formCollection[key]) > 0);
            }
            else // edit 
            {
                var POProductDetails = (List<POProductDetail>)TempData.Peek("OLDPOProductDetails");
                int Counter = 0;

                if (POProductDetails != null) // edit the existing PODetails(add or delete IMEI)
                {
                    for (int i = 0; i < IMEIS.Count(); i++)
                    {
                        if (!POProductDetails.Any(m => m.IMENO.Equals(IMEIS[i])))
                            Counter += _purchaseOrderService.CheckIMENoDuplicacyByConcernId(User.Identity.GetConcernId(), IMEIS[i]);
                    }
                    if (Counter > 0)
                        isDuplicate = true;
                }
                else // add New Product to PO
                {
                    isDuplicate = formCollection.AllKeys
                       .Where(key => key.StartsWith("IMEINo"))
                       .Any(key => _purchaseOrderService.CheckIMENoDuplicacyByConcernId(User.Identity.GetConcernId(), formCollection[key]) > 0);
                }


            }

            return IMEIS.Length != IMEIS.Distinct().Count() || isDuplicate;
        }
        /// <summary>
        /// This method check duplicate IMEI in recently added product.
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        private bool HasDuplicateIMEIORBarcodeInRecentAddToOrder(FormCollection formCollection, PurchaseOrderViewModel purchaseOrder)
        {
            int ProductType = int.Parse(GetDefaultIfNull(formCollection["ProductsType"]));
            string[] IMEIS = formCollection.AllKeys
                             .Where(key => key.StartsWith("IMEINo"))
                             .Select(key => formCollection[key])
                             .ToArray();
            if (ProductType != (int)EnumProductType.NoBarcode)
            {

                if (purchaseOrder.PODetails.Count() > 0)
                {
                    for (int i = 0; i < IMEIS.Length; i++)
                    {
                        foreach (var item in purchaseOrder.PODetails)
                        {
                            if (item.POProductDetails != null)
                            {
                                if (item.POProductDetails.Any(j => j.IMENO == IMEIS[i]))
                                    return true;
                            }

                        }

                    }
                }
            }

            return false;
        }

        private void AddTempPOProductDetails(PurchaseOrderViewModel purchaseOrder, FormCollection formCollection, bool IsDeliveryOrder)
        {
            if (string.IsNullOrEmpty(purchaseOrder.PODetail.Quantity)) return;

            List<POProductDetail> tempPOPDetails = new List<POProductDetail>();
            POProductDetail popDetail = null;
            int ProductType = int.Parse(GetDefaultIfNull(formCollection["ProductsType"]));

            string[] IMEIS = formCollection.AllKeys
                           .Where(key => key.StartsWith("IMEINo"))
                           .Select(key => formCollection[key])
                           .ToArray();

            string[] DIMEIS = formCollection.AllKeys
                      .Where(key => key.StartsWith("DIMEINo"))
                      .Select(key => formCollection[key])
                      .ToArray();

            if (IsDeliveryOrder)
            {
                var poProductDetail = new POProductDetail();
                poProductDetail.ProductID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ProductId));
                poProductDetail.ColorID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ColorId));
                poProductDetail.IMENO = formCollection["IMEINo1"];
                tempPOPDetails.Add(poProductDetail);
            }
            else
            {

                if (ProductType == (int)EnumProductType.ExistingBC || ProductType == (int)EnumProductType.AutoBC)
                {
                    for (int i = 0; i < decimal.Parse(purchaseOrder.PODetail.Quantity); i++)
                    {
                        var poProductDetail = new POProductDetail();
                        poProductDetail.ProductID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ProductId));
                        poProductDetail.ColorID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ColorId));
                        poProductDetail.IMENO = IMEIS[i];

                        if (i <= DIMEIS.Length - 1 && purchaseOrder.PurchaseOrder.IsDamagePO)
                        {
                            poProductDetail.DIMENO = DIMEIS[i];
                            popDetail = _purchaseOrderService.GetDamagePOPDetail(poProductDetail.DIMENO, poProductDetail.ProductID, poProductDetail.ColorID);
                            if (popDetail != null)
                            {
                                poProductDetail.DamagePOPDID = popDetail.POPDID;
                            }
                            else
                            {
                                ModelState.AddModelError("PODetail.Quantity", poProductDetail.DIMENO + " is not found in damage return order.");
                            }
                        }

                        if (!string.IsNullOrEmpty(poProductDetail.IMENO))
                            tempPOPDetails.Add(poProductDetail);
                    }
                }
                else //For No Barcode only one entity for stock details
                {
                    var poProductDetail = new POProductDetail();
                    poProductDetail.ProductID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ProductId));
                    poProductDetail.ColorID = int.Parse(GetDefaultIfNull(purchaseOrder.PODetail.ColorId));
                    poProductDetail.IMENO = formCollection["IMEINo1"];
                    tempPOPDetails.Add(poProductDetail);
                }

                if (tempPOPDetails.Count > 0)
                    TempData["POProductDetails"] = tempPOPDetails;

                if (ProductType == (int)EnumProductType.ExistingBC || ProductType == (int)EnumProductType.AutoBC)
                {
                    if (decimal.Parse(purchaseOrder.PODetail.Quantity) != tempPOPDetails.Count())
                        ModelState.AddModelError("PODetail.Quantity", "Quantity is not equal to the number of IMEI.");

                    if (purchaseOrder.PurchaseOrder.IsDamagePO)
                    {
                        if (IMEIS.Length != DIMEIS.Length)
                            ModelState.AddModelError("PODetail.Quantity", "Damage and New IMEI number is not equal.");
                    }
                }
            }
        }

        private void GetPickersValue(PurchaseOrderViewModel purchaseOrder, PurchaseOrderViewModel NewpurchaseOrder, FormCollection formCollection)
        {
            purchaseOrder.PurchaseOrder.SupplierId = formCollection["SuppliersId"];
            purchaseOrder.PODetail.ProductId = formCollection["ProductsId"];
            purchaseOrder.PODetail.ProductCode = formCollection["ProductsCode"];
            purchaseOrder.PODetail.ProductName = formCollection["ProductsName"];
            purchaseOrder.PODetail.ColorId = string.IsNullOrEmpty(formCollection["ColorsId"]) ? _colorService.GetAllColor().FirstOrDefault().ColorID.ToString() : formCollection["ColorsId"];
            NewpurchaseOrder.PODetail.ColorId = purchaseOrder.PODetail.ColorId;
            NewpurchaseOrder.PODetail.ProductId = purchaseOrder.PODetail.ProductId;
            purchaseOrder.PODetail.ProductType = int.Parse(GetDefaultIfNull(formCollection["ProductsType"]));
            NewpurchaseOrder.PODetail.GodownID = string.IsNullOrEmpty(formCollection["GodownsId"]) ? _godownService.GetAllGodown().FirstOrDefault().GodownID.ToString() : formCollection["GodownsId"];

        }

        [HttpGet]
        [Authorize]
        [Route("Purchase-report")]
        public ActionResult DailyPurchaseReport()
        {
            return View("DailyPurchaseReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult MonthlyPurchaseReport()
        {
            return View("MonthlyPurchaseReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult YearlyPurchaseReport()
        {
            return View("YearlyPurchaseReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult SuplierWisePurchaseReport()
        {
            return View("SuplierWisePurchaseReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult AdvanceSearchByIMEINO()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public JsonResult AdvanceSearchByIMEI(string IMEI)
        {
            int ConcernID = User.Identity.GetConcernId();
            var Data = _purchaseOrderService.AdvanceSearchByIMEI(ConcernID, IMEI);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWisePurchaseReport()
        {
            return View();
        }


        [HttpGet]
        [Authorize]
        public JsonResult GetUniqueBarCode(int MaxSize, int Quantity)
        {
            List<string> BarcodeList = new List<string>();
            for (int i = 0; i < Quantity; i++)
            {
                BarcodeList.Add(GetUniqueBarCode(MaxSize));
            }
            return Json(BarcodeList, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Authorize]
        public ActionResult ProductWisePurchaseDetailsReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult SRVisitAdvanceSearchByIMEINO()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public JsonResult SRVisitAdvanceSearchByIMEI(string IMEI)
        {
            int ConcernID = User.Identity.GetConcernId();
            var Data = _purchaseOrderService.SRVisitAdvanceSearchByIMEI(ConcernID, IMEI);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetInvoiceByID(int orderId, bool IsDO)
        {
            TempData["POrderID"] = orderId;
            TempData["IsInvoiceReadyById"] = true;
            if (IsDO)
                return RedirectToAction("DeliveryOrders");
            else
                return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult DamageInvoice(int orderId)
        {
            TempData["POrderID"] = orderId;
            TempData["IsInvoiceReadyById"] = true;
            return RedirectToAction("DamageReturnOrders");
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetDamagePOReport()
        {
            return View();
        }

        public JsonResult GetDamageReturnIMEI(int ProductID, int ColorID)
        {
            var ProductDetails = _purchaseOrderService.GetDamageReturnProductDetails(ProductID, ColorID);
            if (ProductDetails != null)
            {
                return Json(new { Status = true, Data = ProductDetails }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false, Data = ProductDetails }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetDamageReturnPOReport()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult AdminPOReport()
        {
            @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult SupplierLedger()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public JsonResult GetSupplierInfoById(int supplierId)
        {

            var suppliers = _supplierService.GetAllSupplierNew(User.Identity.GetConcernId(), supplierId).FirstOrDefault();
            return Json(suppliers, JsonRequestBehavior.AllowGet);

        }
        private List<TOIdNameDDL> GetAllSupplierForDDL()
        {

            var suppliers = _supplierService.GetAllSupplierNew(User.Identity.GetConcernId()).Select(s => new TOIdNameDDL
            {
                Id = s.Id,
                Name = s.Name + "(" + s.Code + "), " + "Mobile:" + s.ContactNo + ", " + "Add:" + s.Address
            }).ToList();
            return suppliers;

        }

        [HttpGet]
        [Authorize]
        public JsonResult GetProductInfoById(int productId)
        {

            //var products = _productService.GetAllProductNew(User.Identity.GetConcernId(), productId).FirstOrDefault();
            var products = _productService.GetAllProductIQueryable().FirstOrDefault();
            return Json(products, JsonRequestBehavior.AllowGet);

        }
        private List<TOIdNameDDL> GetAllProductForDDL()
        {

            var products = _productService.GetAllProductIQueryable().Select(s => new TOIdNameDDL
            {
                Id = s.ProductID,
                Name = s.ProductName + ", " + s.CategoryName + ", " + s.CompanyName + " (" + s.SizeName + ")"
            }).ToList();
            return products;

        }
    }
}