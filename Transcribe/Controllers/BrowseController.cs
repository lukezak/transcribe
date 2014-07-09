using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transcribe.Models;
using PagedList;

namespace Transcribe.Controllers
{
    public class BrowseController : Controller
    {
        private TranscribeContext _db = new TranscribeContext();

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            int pageSize = 8;
            int pageNumber = (page ?? 1);

            var orderedList = _db.RecordModels
                    .GroupBy(g => g.title)
                    .Select(s => s.FirstOrDefault());

            if (!String.IsNullOrEmpty(searchString))
            {
                    orderedList = _db.RecordModels
                    .GroupBy(g => g.title)
                    .Select(s => s.FirstOrDefault())
                    .Where(r => r.title.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    orderedList = orderedList.OrderByDescending(r => r.title);
                    break;
                case "Date":
                    orderedList = orderedList.OrderBy(r => r.commit);
                    break;
                case "date_desc":
                    orderedList = orderedList.OrderByDescending(r => r.commit);
                    break;
                default:
                    orderedList = orderedList.OrderBy(r => r.title);
                    break;
            }

            return View(orderedList.ToPagedList(pageNumber, pageSize));
        }

        [ChildActionOnly]
        public ActionResult Totals(BrowseViewModel model)
        {
            if (model.RecordTitle != null)
            {

                var status = _db.RecordModels
                            .Where(x => x.title == model.RecordTitle && x.status == "Complete")
                            .Select(s => s)
                            .Count();

                var num = _db.RecordModels
                            .Where(x => x.title == model.RecordTitle)
                            .Select(s => s)
                            .Count();

                model.totalTranscribed = status;
                model.total = num;
            }

            return PartialView("_TotalTranscribed", model);
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
