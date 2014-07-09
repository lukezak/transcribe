using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Transcribe.Models;
using Microsoft.AspNet.Identity;
using System.Net;
using PagedList;
using System.Collections;

namespace Transcribe.Controllers
{

    public class AdminController : Controller
    {
        private TranscribeContext _db = new TranscribeContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult List(string sortOrder, string currentFilter, string searchString, int? page)
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

            var orderedList = _db.RecordModels.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                orderedList = orderedList.Where(r => r.title.ToUpper().Contains(searchString.ToUpper()));
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
                    orderedList = orderedList.OrderByDescending(r => r.RecordId);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(orderedList.ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecordModels recordModels = await _db.RecordModels.FindAsync(id);
            if (recordModels == null)
            {
                return HttpNotFound();
            }
            return View(recordModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RecordId,title,startdate,enddate,difficulty,page,status,sysident,image,commit")] RecordModels recordModels)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(recordModels).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(recordModels);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecordModels recordModels = await _db.RecordModels.FindAsync(id);
            if (recordModels == null)
            {
                return HttpNotFound();
            }
            return View(recordModels);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RecordModels recordModels = await _db.RecordModels.FindAsync(id);
            _db.RecordModels.Remove(recordModels);
            await _db.SaveChangesAsync();
            return RedirectToAction("List");
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(UploadViewModel model)
        {
            string crop;

            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = model.file;
                var fileName = Guid.NewGuid();
                string ext = System.IO.Path.GetExtension(file.FileName);
                string path = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["imgfolder"]);
                string ocrpath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["tesseract"]);

                if (file != null && file.ContentLength > 0)
                {
                    if (model.hdncoords != null && model.hdncoords.Length > 0)
                    {
                        crop = model.hdncoords;
                    }
                    else
                    {
                        crop = "";
                    }

                    ImageResizer.ImageJob i = new ImageResizer.ImageJob(file, path + fileName + ext, new ImageResizer.Instructions("format=jpg&quality=95&maxwidth=3200" + crop));
                    i.CreateParentDirectory = true;
                    i.Build();

                    System.Diagnostics.Process si = new System.Diagnostics.Process();
                    si.StartInfo.WorkingDirectory = ocrpath;
                    si.StartInfo.UseShellExecute = false;
                    si.StartInfo.FileName = "cmd.exe";
                    si.StartInfo.Arguments = "/c tesseract " + "\"" + path + "\\" + fileName + ext + "\" \"" + path + "\\" + fileName + "\" -l eng";
                    si.StartInfo.CreateNoWindow = true;
                    si.StartInfo.RedirectStandardInput = true;
                    si.StartInfo.RedirectStandardOutput = true;
                    si.StartInfo.RedirectStandardError = true;
                    si.Start();
                    si.StandardOutput.ReadToEnd();
                    si.Close();
                }

                string line;
                string all;
                int counter = 1;

                using (_db)
                {
                    var NewRecord = _db.RecordModels.Add(new RecordModels
                    {
                        title = model.title,
                        startdate = model.startdate,
                        enddate = model.enddate,
                        difficulty = model.difficulty,
                        status = "Not started",
                        page = model.pageno,
                        sysident = model.sysident,
                        type = model.type,
                        image = fileName + ext,
                        commit = DateTime.UtcNow
                    });

                    await _db.SaveChangesAsync();

                    if (model.type == "TYPED")
                    {
                        using (StreamReader sr = new StreamReader(path + fileName + ".txt"))
                        {
                            while (sr.Peek() != -1)
                            {
                                line = sr.ReadLine();

                                if (!string.IsNullOrWhiteSpace(line))
                                {
                                    _db.VersionModels.Add(new VersionModels
                                    {
                                        RecordId = NewRecord.RecordId,
                                        userId = User.DisplayName(),
                                        transcription = line.Trim(),
                                        sortorder = counter++,
                                        commit = DateTime.UtcNow
                                    });

                                    await _db.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    else if (model.type == "HANDWRITTEN")
                    {
                        using (StreamReader sr = new StreamReader(path + fileName + ".txt"))
                        {
                            while (sr.Peek() != -1)
                            {
                                string freetext;

                                all = sr.ReadToEnd();

                                if (string.IsNullOrWhiteSpace(all))
                                {
                                    freetext = "empty";
                                }
                                else
                                {
                                    freetext = all;
                                }

                                _db.VersionModels.Add(new VersionModels
                                {
                                    RecordId = NewRecord.RecordId,
                                    userId = User.DisplayName(),
                                    transcription = freetext.Trim(),
                                    sortorder = counter++,
                                    commit = DateTime.UtcNow
                                });

                                await _db.SaveChangesAsync();
                            }
                            if (string.IsNullOrWhiteSpace(sr.ReadToEnd().ToString()))
                            {
                                _db.VersionModels.Add(new VersionModels
                                {
                                    RecordId = NewRecord.RecordId,
                                    userId = User.DisplayName(),
                                    transcription = "empty",
                                    sortorder = counter++,
                                    commit = DateTime.UtcNow
                                });

                                await _db.SaveChangesAsync();
                            }
                        }
                    }
                }
                if (model.persist != "N")
                {
                    return View("Upload", model);
                }
                else
                {
                    return RedirectToAction("Upload", "Admin");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Report(int? page)
        {
            var reportList = _db.ReportModels
                             .Select(r => r)
                             .OrderByDescending(o => o.ReportId);

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(reportList.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Export(int? page)
        {
            var exportList = _db.RecordModels
                             .Where(x => x.status == "Complete")
                             .Select(r => r)
                             .OrderByDescending(o => o.RecordId);

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(exportList.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileContentResult ExportById(ExportViewModel model)
        {
            var name = _db.RecordModels
                .Where(x => x.RecordId == model.RecordId)
                .Select(s => s.title).FirstOrDefault();

            var comp = _db.VersionModels
                .Where(x => x.RecordId == model.RecordId)
                .Select(s => s);

            ArrayList arrList = new ArrayList();

            foreach (var o in comp)
            {
                arrList.Add(o.transcription);
            }

            var strings = from object o in arrList
                          select o.ToString();

            string txt = string.Join(" ", strings.ToArray());

            return File(new System.Text.UTF8Encoding().GetBytes(txt), "text/plain", name.ToString() + ".txt");
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