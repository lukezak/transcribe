using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transcribe.Models;

namespace Transcribe.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private TranscribeContext _dbnew = new TranscribeContext();

        public ActionResult Index(string id, UserViewModel model)
        {
            if (id != "Anonymous")
            {
                var get = _db.Users
                    .Where(x => x.DisplayName == id)
                    .Select(s => s).FirstOrDefault();

                model.userid = get.DisplayName;
                model.uniqueUserId = get.Id;
            }
            else
            {
                model.userid = "Anonymous";
                model.uniqueUserId = "abc123";
            }

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult GetCompleted(string uniqueUserId)
        {
            var comp = (from p in _dbnew.CompletedModels
                        join r in _dbnew.RecordModels on p.RecordId equals r.RecordId into pR
                        from l in pR.DefaultIfEmpty()
                        where p.uniqueUserId == uniqueUserId
                        orderby p.CompletedId
                        select new CompletedViewModel { Completed = p, Records = l });

            return PartialView("_Completed", comp);
        }

        [ChildActionOnly]
        public ActionResult GetHearts(string uniqueUserId, HeartsViewModel model)
        {
            var heart = _dbnew.CompletedModels
                .Where(x => x.uniqueUserId == uniqueUserId)
                .Select(s => s).Count();

            model.hearts = heart;

            return PartialView("_Hearts", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
