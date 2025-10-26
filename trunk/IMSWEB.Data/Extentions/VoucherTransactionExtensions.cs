using IMSWEB.Model;
using IMSWEB.Model.TOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class VoucherTransactionExtensions
    {

        // public static List<VoucherTransactionReportModel> VoucherTransactionLedgerData(this IBaseRepository<ShareInvestment> _voucherTransactionRepository,
        // IBaseRepository<Customer> _CustomerRepository, IBaseRepository<ShareInvestmentHead> _ShareInvestmentHeadRepository,      
        //DateTime fromDate, DateTime toDate, int ConcernID, int ExpenseItemID, string headType)
        // {
        //     List<VoucherTransactionReportModel> ledgers = new List<VoucherTransactionReportModel>();
        //     List<VoucherTransactionReportModel> FinalLedgers = new List<VoucherTransactionReportModel>();

        //     #region InvestmentID

        //     var InvestOpeningData = (/*from VT in _voucherTransactionRepository.All*/
        //                              from exp in _ShareInvestmentHeadRepository.All /*on VT.InvestmentHeadId equals exp.SIHID*/
        //                              where exp.SIHID == ExpenseItemID
        //                              select new VoucherTransactionReportModel
        //                              {
        //                                  Opening = exp.OpeningBalance,
        //                                  ModuleType = "Opening",
        //                                  ItemName = exp.Name,
        //                                  VoucherDate = exp.OpeningDate,
        //                                  Narration = "Opening Balance"

        //                              }).ToList();
        //     ledgers.AddRange(InvestOpeningData);

        //     var InvestPaymentVoucherData = (from VT in _voucherTransactionRepository.All
        //                                     join exp in _ShareInvestmentHeadRepository.All on VT.SIHID equals exp.SIHID
        //                                     where /*(VT.VoucherDate >= fromDate && VT.VoucherDate <= toDate) &&*/
        //                                      VT.TransactionType == EnumInvestTransType.Pay && VT.SIHID == ExpenseItemID
        //                                     select new VoucherTransactionReportModel
        //                                     {
        //                                         VoucherNo = VT.SIID.ToString(),
        //                                         VoucherDate = VT.CreateDate,
        //                                         DebitAmount = 0m,
        //                                         CreditAmount = VT.Amount,
        //                                         ModuleType = "Payment Voucher",
        //                                         Narration = VT.Purpose,
        //                                         ItemName = exp.Name

        //                                     }).ToList();
        //     ledgers.AddRange(InvestPaymentVoucherData);

        //     var InvestRecieptVoucherData = (from VT in _voucherTransactionRepository.All
        //                                     join exp in _ShareInvestmentHeadRepository.All on VT.SIHID equals exp.SIHID
        //                                     where /*(VT.VoucherDate >= fromDate && VT.VoucherDate <= toDate) &&*/
        //                                      VT.TransactionType == EnumInvestTransType.Receive && VT.SIHID == ExpenseItemID
        //                                     select new VoucherTransactionReportModel
        //                                     {
        //                                         VoucherNo =  VT.SIID.ToString(),
        //                                         VoucherDate = VT.CreateDate,
        //                                         DebitAmount = VT.Amount,
        //                                         CreditAmount = 0m,

        //                                         ModuleType = "Reciept Voucher",
        //                                         Narration = VT.Purpose,
        //                                         ItemName = exp.Name

        //                                     }).ToList();
        //     ledgers.AddRange(InvestRecieptVoucherData); 

        //     #endregion


        //     decimal openingdue = ledgers.Select(i => i.Opening).FirstOrDefault();

        //     decimal balance = openingdue;
        //     ledgers = ledgers.OrderBy(i => i.VoucherDate).ToList();

        //     foreach (var item in ledgers)
        //     {
        //         //decimal balance = item.Opening;
        //         if (headType.ToLower().Equals("s"))
        //         {
        //             item.Balance = balance + (item.CreditAmount - item.DebitAmount);
        //         }
        //         else if (headType.ToLower().Equals("in"))
        //         {
        //             item.Balance = balance + (item.CreditAmount - item.DebitAmount);
        //         }
        //         else if (headType.ToLower().Equals("sc"))
        //         {
        //             item.Balance = balance + (item.CreditAmount - item.DebitAmount);
        //         }
        //         else
        //         {
        //             item.Balance = balance + (item.DebitAmount - item.CreditAmount);
        //         }

        //         item.Particulars = string.IsNullOrEmpty(item.Particulars) ? string.Join(Environment.NewLine, item.Narration) : item.Particulars;
        //         balance = item.Balance;
        //     }



        //     if (ledgers.Count > 0)
        //     {
        //         if (headType.ToLower().Equals("s"))
        //         {
        //             var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderByDescending(i => i.VoucherDate < fromDate).LastOrDefault();
        //             if (OpeningTrans != null)
        //                 FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = OpeningTrans.VoucherDate, Particulars = "Opening Balance", Narration = "Opening Balance", Balance = OpeningTrans.Balance, CreditAmount = 0m });
        //             else
        //                 FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", Balance = openingdue, CreditAmount = 0m });

        //             ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
        //             FinalLedgers.AddRange(ledgers);
        //         }
        //         else  
        //         {
        //             var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderByDescending(i => i.VoucherDate < fromDate).LastOrDefault();
        //             if (OpeningTrans != null)
        //                 FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = OpeningTrans.VoucherDate, Particulars = "Opening Balance", Narration = "Opening Balance", Balance = OpeningTrans.Balance, CreditAmount = 0m });
        //             else
        //                 FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", Balance = openingdue, CreditAmount = 0m });

        //             ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
        //             FinalLedgers.AddRange(ledgers);
        //         }                

        //     }
        //     else
        //     {
        //         if (headType.ToLower().Equals("s"))
        //         {
        //             FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", CreditAmount = openingdue, DebitAmount = 0m, Balance = openingdue });
        //         }
        //         else 
        //         {
        //             FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", CreditAmount = openingdue, DebitAmount = 0m, Balance = openingdue });
        //         }

        //     }

        //     return FinalLedgers;
        // }       



        public static List<VoucherTransactionReportModel> VoucherTransactionLedgerData(
    this IBaseRepository<ShareInvestment> _voucherTransactionRepository,
    IBaseRepository<Customer> _CustomerRepository,
    IBaseRepository<ShareInvestmentHead> _ShareInvestmentHeadRepository,
    DateTime fromDate, DateTime toDate, int ConcernID, int ExpenseItemID, string headType)
        {
            List<VoucherTransactionReportModel> ledgers = new List<VoucherTransactionReportModel>();
            List<VoucherTransactionReportModel> FinalLedgers = new List<VoucherTransactionReportModel>();

            #region InvestmentID

            // Opening balance data
            var InvestOpeningData = (from exp in _ShareInvestmentHeadRepository.All
                                     where exp.SIHID == ExpenseItemID
                                     select new VoucherTransactionReportModel
                                     {
                                         Opening = exp.OpeningBalance,
                                         ModuleType = "Opening",
                                         ItemName = exp.Name,
                                         VoucherDate = exp.OpeningDate,
                                         Narration = "Opening Balance"
                                     }).ToList();
            ledgers.AddRange(InvestOpeningData);

            // Payment voucher data
            var InvestPaymentVoucherData = (from VT in _voucherTransactionRepository.All
                                            join exp in _ShareInvestmentHeadRepository.All on VT.SIHID equals exp.SIHID
                                            where VT.TransactionType == EnumInvestTransType.Pay && VT.SIHID == ExpenseItemID
                                            select new VoucherTransactionReportModel
                                            {
                                                VoucherNo = VT.SIID.ToString(),
                                                VoucherDate = VT.EntryDate,
                                                DebitAmount = 0m,
                                                CreditAmount = VT.Amount,
                                                ModuleType = "Payment Voucher",
                                                Narration = VT.Purpose,
                                                ItemName = exp.Name
                                            }).ToList();
            ledgers.AddRange(InvestPaymentVoucherData);

            // Receipt voucher data
            var InvestRecieptVoucherData = (from VT in _voucherTransactionRepository.All
                                            join exp in _ShareInvestmentHeadRepository.All on VT.SIHID equals exp.SIHID
                                            where VT.TransactionType == EnumInvestTransType.Receive && VT.SIHID == ExpenseItemID
                                            select new VoucherTransactionReportModel
                                            {
                                                VoucherNo = VT.SIID.ToString(),
                                                VoucherDate = VT.EntryDate,
                                                DebitAmount = VT.Amount,
                                                CreditAmount = 0m,
                                                ModuleType = "Receipt Voucher",
                                                Narration = VT.Purpose,
                                                ItemName = exp.Name
                                            }).ToList();
            ledgers.AddRange(InvestRecieptVoucherData);

            #endregion

            decimal openingdue = ledgers.Select(i => i.Opening).FirstOrDefault();
            decimal balance = openingdue;
            ledgers = ledgers.OrderBy(i => i.VoucherDate).ToList();
            string headTypeLower = headType.ToLower();  // Cache ToLower result

            foreach (var item in ledgers)
            {
                // Calculate balance based on headType
                if (headTypeLower == "s" || headTypeLower == "in" || headTypeLower == "sc")
                {
                    item.Balance = balance + (item.CreditAmount - item.DebitAmount);
                }
                else
                {
                    item.Balance = balance + (item.DebitAmount - item.CreditAmount);
                }

                item.Particulars = string.IsNullOrEmpty(item.Particulars) ? item.Narration : item.Particulars;
                balance = item.Balance;
            }

            if (ledgers.Count > 0)
            {
                var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderBy(i => i.VoucherDate).LastOrDefault();

                // Add opening balance based on headType
                if (OpeningTrans != null)
                {
                    FinalLedgers.Add(new VoucherTransactionReportModel
                    {
                        VoucherDate = OpeningTrans.VoucherDate,
                        Particulars = "Opening Balance",
                        Narration = "Opening Balance",
                        Balance = OpeningTrans.Balance,
                        CreditAmount = 0m
                    });
                }
                else
                {
                    FinalLedgers.Add(new VoucherTransactionReportModel
                    {
                        VoucherDate = fromDate,
                        Particulars = "Opening Balance",
                        Balance = openingdue,
                        CreditAmount = 0m
                    });
                }

                // Filter and add the ledgers in date range
                ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
                FinalLedgers.AddRange(ledgers);
            }
            else
            {
                FinalLedgers.Add(new VoucherTransactionReportModel
                {
                    VoucherDate = fromDate,
                    Particulars = "Opening Balance",
                    CreditAmount = openingdue,
                    DebitAmount = 0m,
                    Balance = openingdue
                });
            }

            return FinalLedgers;
        }


    }
}
