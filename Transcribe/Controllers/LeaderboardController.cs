using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transcribe.Models;
using PagedList;

namespace Transcribe.Controllers
{
    public class LeaderboardController : Controller
    {

        private TranscribeContext _db = new TranscribeContext();

        public ActionResult Index(int? page)
        {
            var board = _db.CompletedModels
                .Where(x => x.uniqueUserId != "abc123")
                .GroupBy(g => g.userId)
                .Select(s => new LeaderBoardViewModel {
                    userid = s.Key, 
                    total = s.Count()
                }).Take(50).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(board.ToPagedList(pageNumber, pageSize));
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
