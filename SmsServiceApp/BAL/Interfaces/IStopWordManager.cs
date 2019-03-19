﻿using Model.ViewModels.StopWordViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Models;

namespace BAL.Managers
{
   public interface IStopWordManager
    {
        IEnumerable<StopWordViewModel> GetStopWords();
        void Insert(StopWordViewModel item);
        void Update(StopWordViewModel item);
        void Delete(int item);
      
    }
}
