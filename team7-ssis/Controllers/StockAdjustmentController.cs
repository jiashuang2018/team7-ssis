﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using team7_ssis.Models;
using team7_ssis.Repositories;
using team7_ssis.Services;
using team7_ssis.Tests.Services;
using team7_ssis.ViewModels;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Net;

namespace team7_ssis.Controllers
{
    public class StockAdjustmentController : Controller
    {
        ApplicationDbContext context;
        StockAdjustmentService stockAdjustmentService;
        UserService userService;
        UserRepository userRepository;
        ItemPriceService itemPriceService;
        public StockAdjustmentController()
        {
            context =new ApplicationDbContext();
            stockAdjustmentService = new StockAdjustmentService(context);
            userService = new UserService(context);
            userRepository = new UserRepository(context);
            itemPriceService = new ItemPriceService(context);
        }
       

        // GET: StockAdjustment
        public ActionResult Index()
        {
            return View();
        }

   

        public ActionResult New()
        {
            string UserName = System.Web.HttpContext.Current.User.Identity.GetUserName();
            Department d = userService.FindUserByEmail(UserName).Department;

            List<ApplicationUser> supervisors=new List<ApplicationUser>();

            List<ApplicationUser> managers =new List<ApplicationUser>();

            if (d != null)
            {
                supervisors = userService.FindSupervisorsByDepartment(d);
                managers = new List<ApplicationUser>() { d.Head };
            }


                List<SelectListItem> listItem_supervicor = new List<SelectListItem>();
            foreach (ApplicationUser a in supervisors)
            {
                SelectListItem item1 = new SelectListItem()
                {
                    Value = a.Id,
                    Text = a.FirstName.ToString() + " " + a.LastName.ToString()
                };
                listItem_supervicor.Add(item1);
            }
            SelectList select1 = new SelectList(listItem_supervicor, "Value", "Text");
            ViewBag.select1 = select1;

            List<SelectListItem> listItem_managers = new List<SelectListItem>();

            foreach (ApplicationUser a in managers)
            {
                SelectListItem item2 = new SelectListItem()
                {
                    Value = a.Id,
                    Text = a.FirstName.ToString() + " " + a.LastName.ToString()
                };
                listItem_managers.Add(item2);
            }
            SelectList select2 = new SelectList(listItem_managers, "Value", "Text");
            ViewBag.select2 = select2;
            return View();
        }

        //public ActionResult SaveAsDraft()
        //{
        //    return View();
        //}

        public ActionResult Details(string id)
        {
            //get the stockadjustment
            StockAdjustment sa = stockAdjustmentService.FindStockAdjustmentById(id);
            StockAdjustmentViewModel sv = new StockAdjustmentViewModel();
            sv.StockAdjustmentId = sa.StockAdjustmentId;
            sv.CreatedBy = (sa.CreatedBy == null) ? "" : sa.CreatedBy.FirstName + " " + sa.CreatedBy.LastName;
            sv.CreatedDateTime = sa.CreatedDateTime;
            sv.ApprovedBySupervisor = sa.ApprovedBySupervisor == null ? "" : sa.ApprovedBySupervisor.FirstName + " "
                + sa.ApprovedBySupervisor.LastName;
            sv.UpdateDateTime = sa.UpdatedDateTime == null ? DateTime.Now : (DateTime)sa.UpdatedDateTime;

            //get the stockadjustment details
            List<StockAdjustmentDetail> detail_list = sa.StockAdjustmentDetails;
            List<StockAdjustmentDetailViewModel> ViewModel_list = new List<StockAdjustmentDetailViewModel>();
            StockAdjustmentDetailListViewModel ResultListViewModel = new StockAdjustmentDetailListViewModel();


            foreach (StockAdjustmentDetail sd in detail_list)
            {
                StockAdjustmentDetailViewModel sadv = new StockAdjustmentDetailViewModel();

                sadv.ItemCode = sd.ItemCode;
                sadv.Description = sd.Item.Description;
                sadv.Reason = sd.Reason;
                sadv.UnitPrice = itemPriceService.GetDefaultPrice(sd.Item, 1);
                sadv.Adjustment = (sd.AfterQuantity - sd.OriginalQuantity).ToString();
                if (sadv.UnitPrice.CompareTo("250") == -1)
                {
                    sadv.PriceColor = "grey";
                }
                else
                {
                    sadv.PriceColor = "red";
                }
                ViewModel_list.Add(sadv);
            }

            ResultListViewModel.StockAdjustmentDetailsModel = ViewModel_list;
            ResultListViewModel.StockAdjustmentModel = sv;
            return View("Details", ResultListViewModel);
        }

        public ActionResult AddItem()
        {
            string user = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<string> Session_list = new List<string>();
            if (System.Web.HttpContext.Current.Session[user+"stock"] != null) //already has sessionID 
            {
                Session_list = (List<string>)System.Web.HttpContext.Current.Session[user+"stock"];
            }
            else
            {
                System.Web.HttpContext.Current.Session[user+"stock"] = Session_list;
            }

            return View();
        }

        //public ActionResult SaveInSession(List<String> itemCodes)
        //{
        //    string user = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    List<string> itemcodes_list = (List<string>)System.Web.HttpContext.Current.Session[user + "stock"];
        //    foreach (string i in itemCodes)
        //    {
        //        itemcodes_list.Add(i);
        //    }

        //    return View("New");
        //}


    }
}