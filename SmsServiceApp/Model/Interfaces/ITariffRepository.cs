﻿using System.Collections.Generic;
using WebApp.Models;

namespace Model.Interfaces
{
	public interface ITariffRepository : IBaseRepository<Tariff>
	{
        void ChangeTariffPricing(Tariff currentTariff, decimal newPrice, string userRole);
		IEnumerable<Tariff> GetByOperatorId(int operatorId);
	}
}
