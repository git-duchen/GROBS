using System;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using X.PagedList;
using GROBS.EFModels;
using GROBS.IBSL;
using GROBS.BSL;
using GROBS.Common;
using GROBS.Models;
using GROBS.Filters;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Runtime.Remoting.Contexts;
using System.Web;

namespace GROBS.Controllers
{
    public class auth_kehuController : Controller
    {
        private Iauth_kehuService ob_auth_kehuservice = ServiceFactory.auth_kehuservice;
        [OutputCache(Duration = 30)]
        public ActionResult Index(string page)
        {
            if (string.IsNullOrEmpty(page))
                page = "1";
            int userid = (int)Session["user_id"];
            string pagetag = "auth_kehu_index";
            Expression<Func<auth_custacct_v, bool>> where = PredicateExtensionses.True<auth_custacct_v>();
            searchcondition sc = searchconditionService.GetInstance().GetEntityById(searchcondition => searchcondition.UserID == userid && searchcondition.PageBrief == pagetag);
            if (sc != null && sc.ConditionInfo != null)
            {
                string[] sclist = sc.ConditionInfo.Split(';');
                foreach (string scl in sclist)
                {
                    string[] scld = scl.Split(',');
                    switch (scld[0])
                    {
                        case "mingcheng":
                            string mingcheng = scld[1];
                            string mingchengequal = scld[2];
                            string mingchengand = scld[3];
                            if (!string.IsNullOrEmpty(mingcheng))
                            {
                                if (mingchengequal.Equals("="))
                                {
                                    if (mingchengand.Equals("and"))
                                        where = where.And(auth_kehu => auth_kehu.Mingcheng == mingcheng);
                                    else
                                        where = where.Or(auth_kehu => auth_kehu.Mingcheng == mingcheng);
                                }
                                if (mingchengequal.Equals("like"))
                                {
                                    if (mingchengand.Equals("and"))
                                        where = where.And(auth_kehu =>auth_kehu.Mingcheng.Contains(mingcheng));
                                    else
                                        where = where.Or(auth_kehu =>auth_kehu.Mingcheng.Contains(mingcheng));
                                }
                            
                            }
                            break;
                        default:
                            break;
                    }
                }
                ViewBag.SearchCondition = sc.ConditionInfo;
            }

            //where = where.And(auth_kehu => auth_kehu.IsDelete == false);

            var tempData = ob_auth_kehuservice.LoadCustomerAccount(where.Compile()).ToPagedList<auth_custacct_v>(int.Parse(page), int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["ShowPerPage"]));
           ViewBag.auth_kehu = tempData;
            return View(tempData);
        }

        [HttpPost]
        [OutputCache(Duration = 30)]
        public ActionResult Index()
        {
            int userid = (int)Session["user_id"];
            string pagetag = "auth_kehu_index";
            string page = "1";
            string mingcheng = Request["mingcheng"] ?? "";
            string mingchengequal = Request["mingchengequal"] ?? "";
            string mingchengand = Request["mingchengand"] ?? "";
            Expression<Func<auth_custacct_v, bool>> where = PredicateExtensionses.True<auth_custacct_v>();
            searchcondition sc = searchconditionService.GetInstance().GetEntityById(searchcondition => searchcondition.UserID == userid && searchcondition.PageBrief == pagetag);
            if (sc == null)
            {
                sc = new searchcondition();
                sc.UserID = userid;
                sc.PageBrief = pagetag;
                if (!string.IsNullOrEmpty(mingcheng))
                {
                    if (mingchengequal.Equals("="))
                    {
                        if (mingchengand.Equals("and"))
                            where = where.And(auth_kehu => auth_kehu.Mingcheng == mingcheng);
                        else
                            where = where.Or(auth_kehu => auth_kehu.Mingcheng == mingcheng);
                    }
                    if (mingchengequal.Equals("like"))
                    {
                        if (mingchengand.Equals("and"))
                            where = where.And(auth_kehu => auth_kehu.Mingcheng.Contains(mingcheng));
                        else
                            where = where.Or(auth_kehu => auth_kehu.Mingcheng.Contains(mingcheng));
                    }
                
                }            
                searchconditionService.GetInstance().AddEntity(sc);
            }
            else
            {
                sc.ConditionInfo = "";
                if (!string.IsNullOrEmpty(mingcheng))
                {
                    if (mingchengequal.Equals("="))
                    {
                        if (mingchengand.Equals("and"))
                            where = where.And(auth_kehu => auth_kehu.Mingcheng == mingcheng);
                        else
                            where = where.Or(auth_kehu => auth_kehu.Mingcheng == mingcheng);
                    }
                    if (mingchengequal.Equals("like"))
                    {
                        if (mingchengand.Equals("and"))
                            where = where.And(auth_kehu => auth_kehu.Mingcheng.Contains(mingcheng));
                        else
                            where = where.Or(auth_kehu => auth_kehu.Mingcheng.Contains(mingcheng));
                    }               
                }
                if (!string.IsNullOrEmpty(mingcheng))
                    sc.ConditionInfo = sc.ConditionInfo + string.Format("{0},{1},{2},{3};", "mingcheng", mingcheng, mingchengequal, mingchengand);
                else
                    sc.ConditionInfo = sc.ConditionInfo + string.Format("{0},{1},{2},{3};", "mingcheng", "", mingchengequal, mingchengand);
                searchconditionService.GetInstance().UpdateEntity(sc);
            }
            ViewBag.SearchCondition = sc.ConditionInfo;
            //where = where.And(auth_kehu => auth_kehu.IsDelete == false);

            var tempData = ob_auth_kehuservice.LoadCustomerAccount(where.Compile()).ToPagedList<auth_custacct_v>(int.Parse(page), int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["ShowPerPage"]));
            ViewBag.auth_kehu = tempData;
            return View(tempData);
        }

        public ActionResult Add()
        {
            ViewBag.userid = (int)Session["user_id"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save()
        {
            string id = Request["id"] ?? "";
            string kehuid = Request["kehuid"] ?? "";
            string zhanghao = Request["zhanghao"] ?? "";
            string mima = Request["mima"] ?? "";
            string nicheng = Request["nicheng"] ?? "";
            string tingyongsf = Request["tingyongsf"] ?? "";
            string col1 = Request["col1"] ?? "";
            string makedate = Request["makedate"] ?? "";
            string makeman = Request["makeman"] ?? "";
            try
            {
                auth_kehu ob_auth_kehu = new auth_kehu();
                ob_auth_kehu.KehuID = kehuid == "" ? 0 : int.Parse(kehuid);
                ob_auth_kehu.Zhanghao = zhanghao.Trim();
                ob_auth_kehu.Mima = mima.Trim();
                ob_auth_kehu.Nicheng = nicheng.Trim();
                ob_auth_kehu.TingyongSF = tingyongsf == "" ? false : Boolean.Parse(tingyongsf);
                ob_auth_kehu.Col1 = col1.Trim();
                ob_auth_kehu.MakeDate = makedate == "" ? DateTime.Now : DateTime.Parse(makedate);
                ob_auth_kehu.MakeMan = makeman == "" ? 0 : int.Parse(makeman);
                ob_auth_kehu = ob_auth_kehuservice.AddEntity(ob_auth_kehu);
                if (ob_auth_kehu != null)
                {
                    RegisterViewModel model = new RegisterViewModel { Email=ob_auth_kehu.Zhanghao,Password=ob_auth_kehu.Mima,ConfirmPassword=ob_auth_kehu.Mima};
                    AccountController _ac = new AccountController(HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(),HttpContext.GetOwinContext().Get<ApplicationSignInManager>());
                    var _bt= _ac.RegistNow(model);
                }
                ViewBag.auth_kehu = ob_auth_kehu;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [OutputCache(Duration = 10)]
        public ActionResult Edit(int id)
        {
            auth_kehu tempData = ob_auth_kehuservice.GetEntityById(auth_kehu => auth_kehu.ID == id && auth_kehu.IsDelete == false);
            ViewBag.auth_kehu = tempData;
            if (tempData == null)
                return View();
            else
            {
                auth_kehuViewModel auth_kehuviewmodel = new auth_kehuViewModel();
                auth_kehuviewmodel.ID = tempData.ID;
                auth_kehuviewmodel.KehuID = tempData.KehuID;
                auth_kehuviewmodel.Zhanghao = tempData.Zhanghao;
                auth_kehuviewmodel.Mima = tempData.Mima;
                auth_kehuviewmodel.Nicheng = tempData.Nicheng;
                auth_kehuviewmodel.TingyongSF = tempData.TingyongSF;
                auth_kehuviewmodel.Col1 = tempData.Col1;
                auth_kehuviewmodel.MakeDate = tempData.MakeDate;
                auth_kehuviewmodel.MakeMan = tempData.MakeMan;
                return View(auth_kehuviewmodel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update()
        {
            string id = Request["id"] ?? "";
            string kehuid = Request["kehuid"] ?? "";
            string zhanghao = Request["zhanghao"] ?? "";
            string mima = Request["mima"] ?? "";
            string nicheng = Request["nicheng"] ?? "";
            string tingyongsf = Request["tingyongsf"] ?? "";
            string col1 = Request["col1"] ?? "";
            string makedate = Request["makedate"] ?? "";
            string makeman = Request["makeman"] ?? "";
            int uid = int.Parse(id);
            try
            {
                auth_kehu p = ob_auth_kehuservice.GetEntityById(auth_kehu => auth_kehu.ID == uid);
                p.KehuID = kehuid == "" ? 0 : int.Parse(kehuid);
                p.Zhanghao = zhanghao.Trim();
                p.Mima = mima.Trim();
                p.Nicheng = nicheng.Trim();
                p.TingyongSF = tingyongsf == "" ? false : Boolean.Parse(tingyongsf);
                p.Col1 = col1.Trim();
                p.MakeDate = makedate == "" ? DateTime.Now : DateTime.Parse(makedate);
                p.MakeMan = makeman == "" ? 0 : int.Parse(makeman);
                ob_auth_kehuservice.UpdateEntity(p);
                ViewBag.saveok = ViewAddTag.ModifyOk;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.saveok = ViewAddTag.ModifyNo;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete()
        {
            string sdel = Request["del"] ?? "";
            int id;
            auth_kehu ob_auth_kehu;
            foreach (string sD in sdel.Split(','))
            {
                if (sD.Length > 0)
                {
                    id = int.Parse(sD);
                    ob_auth_kehu = ob_auth_kehuservice.GetEntityById(auth_kehu => auth_kehu.ID == id && auth_kehu.IsDelete == false);
                    ob_auth_kehu.IsDelete = true;
                    ob_auth_kehuservice.UpdateEntity(ob_auth_kehu);
                }
            }
            return RedirectToAction("Index");
        }
    }
}

