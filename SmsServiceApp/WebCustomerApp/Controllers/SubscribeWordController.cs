﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.ViewModels.SubscribeWordViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class SubscribeWordController : Controller
    {
        private readonly ISubscribeWordManager subscribeWordManager;

        public SubscribeWordController(ISubscribeWordManager subscribeWordManager)
        {
            this.subscribeWordManager = subscribeWordManager;
        }

        /// <summary>
        /// View for creation new StopWord
        /// </summary>
        /// <returns>Create StopWord View</returns>
        [HttpGet]
        public IActionResult Create(int companyId)
        {
            SubscribeWordViewModel word = new SubscribeWordViewModel(){CompanyId = companyId};
            return View(word);
        }
        /// <summary>
        /// Send new StopWord fron view to db
        /// </summary>
        /// <param name="item">ViewModel of StopWord from View</param>
        /// <returns>StopWord index View</returns>
       
        [HttpPost]
        [Route("~/SubscribeWord/Create")]
        public IActionResult Create(SubscribeWordViewModel item)
        {
            if (ModelState.IsValid)
            {
                subscribeWordManager.Insert(item);
            }
           

            return RedirectToAction("SubscribeWord", "Company", new { companyId = item.CompanyId});
        }

        /// <summary>
        /// Gets EditView with StopWord info from db
        /// </summary>
        /// <param name="id">Id of stopword which need to edit</param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(int id,int companyId)
        {
                SubscribeWordViewModel word = subscribeWordManager.GetWords().FirstOrDefault(c => c.Id == id);
                ViewData["companyId"] = companyId;
            if (word == null)
            {
                    return NotFound();
            }

            word.CompanyId = companyId;
                return View(word);
        }
        /// <summary>
        ///  Send updated StopWord to db
        /// </summary>
        /// <param name="wordEdit">Edited fo stopword</param>
        /// <returns>StopWord index View</returns>
        [HttpPost]
     
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SubscribeWordViewModel wordEdit)
        {

            if (ModelState.IsValid)
            {
                subscribeWordManager.Update(wordEdit);
                return RedirectToAction("SubscribeWord", "Company",new{ wordEdit.CompanyId });
            }
            return View(wordEdit);
        }

       

        /// <summary>
        /// Delete selected item from db
        /// </summary>
        /// <param name="id">Id of StopWord which select to delete</param>
        /// <returns>StopWord Index View</returns>
        [HttpGet]
        public IActionResult DeleteConfirmed(int id,int companyId)
        {
            subscribeWordManager.Delete(id);
            return RedirectToAction("SubscribeWord","Company",new { companyId  });
        }

    }
}
