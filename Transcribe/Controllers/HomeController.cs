using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transcribe.Models;

namespace Transcribe.Controllers
{
    public class HomeController : Controller
    {
        private TranscribeContext _db = new TranscribeContext();

        public ActionResult Index(HomeViewModel model)
        {   
            var carousel = _db.RecordModels
                        .Where(x => x.status != "Complete" && x.page == 1)
                        .Select(s => s)
                        .OrderByDescending(o => o.RecordId).Take(4).ToList();

            model.Records = carousel;

            var progress = _db.RecordModels
                .Where(x => x.status == "Complete")
                .Select(s => s).Count();

            model.progress = progress;

            var total = _db.RecordModels
                .Select(s => s.RecordId).Count();

            model.totalRecords = total;
            if (progress != 0)
            {
                model.percentage = progress * 100 / total;
            }

            var recent = _db.ActivityModels
                .Select(s => s)
                .OrderByDescending(o => o.commit).Take(3).ToList();

            model.Activities = recent;

            var update = _db.ActivityModels
                .Select(s => s).Count();

            model.updates = update;

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult Board()
        {
            var board = _db.CompletedModels
                .Where(x => x.uniqueUserId != "abc123")
                .GroupBy(g => g.userId)
                .Select(s => new LeaderBoardViewModel { userid = s.Key,  total = s.Count()}).Take(5).ToList();

            return PartialView("_LoveBoard", board);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
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