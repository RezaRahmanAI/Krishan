﻿using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class BankExtensions
    {
        public static async Task<IEnumerable<Bank>> GetAllBankAsync(this IBaseRepository<Bank> bankRepository)
        {
            return await bankRepository.All.ToListAsync();
        }
    }
}
