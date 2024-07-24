﻿using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldStore.BusinessLogics
{
    public class Shopping : IShopping
    {
        private readonly ILogger<Shopping> _logger;
        private readonly GStoreDbContext _store;

        public Shopping(ILogger<Shopping> logger, GStoreDbContext store)
        {
            _logger = logger;
            _store = store;
        }

        public async Task<bool> Buy(int weight, long userId)
        {
            return false;
        }

        public async Task<bool> CheckGoldInventory(int weight, int goldType = 1)
        {
            return await _store.GoldRepositories.AnyAsync(x => x.Weight < weight && x.GoldType == goldType);
        }

        public async Task<bool> Sell(int weight, long userId)
        {
            return false;
        }
    }
}
