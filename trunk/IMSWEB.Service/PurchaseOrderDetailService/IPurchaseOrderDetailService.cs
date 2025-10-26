﻿using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IPurchaseOrderDetailService
    {
        void AddPurchaseOrderDetail(POrderDetail pOrderDetail);
        void SavePurchaseOrderDetail();
        IEnumerable<Tuple<decimal, int, decimal, decimal, int, int, decimal, Tuple<decimal, decimal, string, string, int, string, decimal, Tuple<decimal>>>> 
            GetPurchaseOrderDetailById(int id);
        void DeletePurchaseOrderDetail(int id);
        IEnumerable<POrderDetail> GetPOrderDetailByID(int POrderID);
        IQueryable<POrderDetail> GetAll();
    }
}
