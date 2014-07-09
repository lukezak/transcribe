using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Transcribe.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity.SqlServer;
using iTextSharp;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using System.Collections;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using System.Security;
using PagedList;

namespace Transcribe.Controllers
{
    

    public class TranscribeController : Controller
    {
        private TranscribeContext _db = new TranscribeContext();

        private ApplicationUserManager _userManager;

        public TranscribeController()
        {
        }

        public TranscribeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        
        public async Task<ActionResult> Index(int? id, CorrectionsViewModel model)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var record = await _db.RecordModels.FindAsync(id);

            if (record == null)
            {
                return HttpNotFound();
            }

            if (HttpContext.Session != null)
            {
                if (HttpContext.Session.IsNewSession)
                {
                    model.session = ActiveSession(record.RecordId);
                }
            }

            model.RecordId = record.RecordId;
            model.image = record.image;
            model.title = record.title;
            model.startdate = record.startdate;
            model.enddate = record.enddate;
            model.pageno = record.page;
            model.status = record.status;
            model.diffBtn = RecordDifficulty(record.difficulty);
            model.sysid = record.sysident;
            model.type = record.type;
            model.statusBtn = RecordStatus(model.status);

            if (model.status == "Complete")
            {
                model.statusLayer = "Display:inline;";
            }

            var totalPages = _db.RecordModels
                            .Where(x => x.title == model.title)
                            .Select(s => s)
                            .Count();

            var lastRecord = _db.RecordModels.OrderByDescending(o => o.RecordId).Select(s => s.RecordId).First();

            

            if (model.RecordId < lastRecord)
            {
                var nextUntranscribed = _db.RecordModels
                                        .Where(x => x.RecordId != model.RecordId && x.status != "Complete" && x.RecordId > record.RecordId)
                                        .Select(s => s.RecordId).Take(1).FirstOrDefault();
                if (nextUntranscribed != 0)
                {
                    model.nextUntranscribed = nextUntranscribed;
                }
                else
                {
                    model.nextUnDisabled = "disabled";
                }
            }

            var firstRecord = _db.RecordModels
                                .Select(s => s.RecordId)
                                .OrderBy(o => o).FirstOrDefault();

            model.totalpages = totalPages;

            if (model.RecordId != firstRecord && model.RecordId != lastRecord)
            {
                model.nextpage = model.RecordId + 1;
                model.prevpage = model.RecordId - 1;
            }
            if (model.RecordId == firstRecord)
            {
                model.nextpage = model.RecordId + 1;
                model.prevDisabled = "disabled";
            }
            if (model.RecordId == lastRecord)
            {
                model.nextDisabled = "disabled";
                model.nextUnDisabled = "disabled";
                model.prevpage = model.RecordId - 1;
            }

            return View(model);
        }

        public ActionResult Activity(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var act = _db.ActivityModels
                    .Where(x => x.RecordId == id)
                    .Select(s => s)
                    .OrderByDescending(o => o.ActivityId).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(act.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult VersionControl(CorrectionsViewModel model)
        {
            if (TempData["id"] != null)
            {
                model.RecordId = Convert.ToInt32(TempData["id"]);
            }

            var type = _db.RecordModels
                .Where(x => x.RecordId == model.RecordId)
                .Select(s => s).SingleOrDefault();

            if (type.type == "TYPED")
            {
                var txt = _db.VersionModels
                            .Where(x => x.RecordId == model.RecordId)
                            .Select(s => s)
                            .OrderBy(o => o.sortorder).ToList();

                model.Versions = txt;

                return PartialView("_VersionControl", model);
            }
            else if (type.type == "HANDWRITTEN")
            {
                var freetxt = _db.VersionModels
                            .Where(x => x.RecordId == model.RecordId)
                            .Select(s => s).Take(1).ToList();

                model.Versions = freetxt;

                return PartialView("_VersionControlFreeText", model);
            }

            return PartialView("_VersionControl", model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(CorrectionsViewModel model)
        {
            string username;
            string userid;

            if (User.Identity.IsAuthenticated)
            {
                username = User.DisplayName();
                userid = User.Identity.GetUserId();
            }
            else
            {
                username = "Anonymous";
                userid = "abc123";
            }

            if (string.IsNullOrEmpty(model.email)) //Honeypot
            {
                var v = _db.VersionModels.FirstOrDefault(x => x.VersionId == model.pk);
                v.transcription = model.value;
                await _db.SaveChangesAsync();

                var comp = _db.RecordModels.FirstOrDefault(x => x.RecordId == model.RecordId);
                comp.status = "In progress";
                await _db.SaveChangesAsync();

                var a = _db.ActivityModels.Add(new ActivityModels
                    {
                        RecordId = model.RecordId,
                        newline = model.value,
                        oldline = model.oldValue,
                        VersionId = model.pk,
                        userId = username,
                        uniqueUserId = userid,
                        commit = DateTime.UtcNow
                    });

                await _db.SaveChangesAsync();

                if (HttpContext.Session != null)
                {
                    var newsesh = _db.SessionModels.Add(new SessionModels
                    {
                        RecordId = model.RecordId,
                        currentSession = HttpContext.Session.SessionID,
                        commit = DateTime.UtcNow

                    });

                    await _db.SaveChangesAsync();
                }
            }

            TempData["id"] = model.RecordId;

            return RedirectToAction("VersionControl", model);
        }

        public ActionResult Controls(ControlsViewModel model)
        {
            return PartialView("_UserControls", model);
        }

        public ActionResult ActivityControl(ActivityViewModel model)
        {
                var act = _db.ActivityModels
                    .Where(x => x.RecordId == model.id)
                    .Select(s => s)
                    .OrderByDescending(o => o.ActivityId).Take(10).ToList();

                model.Activities = act;

                return PartialView("_ActivityControl", model);
        }

        public ActionResult CommentControl(CommentViewModel model)
        {
            if (TempData["id"] != null)
            {
                model.id = Convert.ToInt32(TempData["id"]);
            }

            var com = _db.CommentsModels
                .Where(x => x.RecordId == model.id)
                .Select(s => s)
                .OrderByDescending(o => o.CommentId).ToList();

            model.Comments = com;

            return PartialView("_CommentControl", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Comment(ControlsViewModel model)
        {
            if (ModelState.IsValid)
            {
                string username;

                if (User.Identity.IsAuthenticated)
                {
                    username = User.DisplayName();
                }
                else
                {
                    username = "Anonymous";
                }

                var com = _db.CommentsModels.Add(new CommentsModels
                {

                    RecordId = model.id,
                    comment = model.comment,
                    userId = username,
                    commit = DateTime.UtcNow

                });

                await _db.SaveChangesAsync();

                TempData["id"] = model.id;

                return RedirectToAction("CommentControl");
            }
            return RedirectToAction("Index", new { id = model.id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Report(CorrectionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var rep = _db.ReportModels.Add(new ReportModels
                    {
                        RecordId = model.RecordId,
                        report = model.reportDetails,
                        commit = DateTime.UtcNow
                    });

                await _db.SaveChangesAsync();

                return new EmptyResult();
            }

            return RedirectToAction("Index", new { id = model.RecordId });
        }

        public ActionResult CompleteControl(ControlsViewModel model)
        {
            if (TempData["id"] != null)
            {
                model.id = Convert.ToInt32(TempData["id"]);
            }

            return PartialView("_CompleteControl", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Complete(ControlsViewModel model)
        {
            string username;
            string userid;

            if (User.Identity.IsAuthenticated)
            {
                username = User.DisplayName();
                userid = User.Identity.GetUserId();
            }
            else
            {
                username = "Anonymous";
                userid = "abc123";
            }

            var comp = _db.RecordModels.FirstOrDefault(x => x.RecordId == model.id);

            if (comp.status != "Complete")
            {
                comp.status = "Complete";
                await _db.SaveChangesAsync();

                var a = _db.ActivityModels.Add(new ActivityModels
                {
                    RecordId = model.id,
                    newline = "Transcription: Complete",
                    oldline = "Transcription: In progress",
                    VersionId = 999999,
                    userId = username,
                    uniqueUserId = userid,
                    commit = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();

                var hasCompleted = _db.CompletedModels
                    .Where(x => x.RecordId == model.id && x.uniqueUserId == userid)
                    .Select(s => s.CompletedId);

                if (hasCompleted.Count() < 1)
                { 
                    var completed = _db.CompletedModels.Add(new CompletedModels
                        {
                            RecordId = model.id,
                            userId = username,
                            uniqueUserId = userid,
                            commit = DateTime.UtcNow
                        });

                    await _db.SaveChangesAsync();
                }
            }

            TempData["id"] = model.id;

            return RedirectToAction("CompleteControl");
        }

        public ActionResult InProgressControl(ControlsViewModel model)
        {
            if (TempData["id"] != null)
            {
                model.id = Convert.ToInt32(TempData["id"]);
            }

            return PartialView("_InProgressControl", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InProgress(ControlsViewModel model)
        {
            string username;
            string userid;
            string oldStatus;

            if (User.Identity.IsAuthenticated)
            {
                username = User.DisplayName();
                userid = User.Identity.GetUserId();
            }
            else
            {
                username = "Anonymous";
                userid = "abc123";
            }

            var inprog = _db.RecordModels.FirstOrDefault(x => x.RecordId == model.id);

            if (inprog.status == "Complete")
            {
                oldStatus = "Transcription: Complete";
            }
            else if (inprog.status == "Not started")
            {
                oldStatus = "Transcription: Not started";
            }
            else
            {
                oldStatus = "Transcription: In progress";
            }

            if (inprog.status != "In progress")
            {
                var a = _db.ActivityModels.Add(new ActivityModels
                {
                    RecordId = model.id,
                    newline = "Transcription: In progress",
                    oldline = oldStatus,
                    VersionId = 999999,
                    userId = username,
                    uniqueUserId = userid,
                    commit = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();

                inprog.status = "In progress";
                await _db.SaveChangesAsync();
            }

            TempData["id"] = model.id;

            return RedirectToAction("InProgressControl");
        }

        public ActionResult FavoriteControl(CorrectionsViewModel model)
        {
            if (TempData["id"] != null)
            {
                model.RecordId = Convert.ToInt32(TempData["id"]);
            }

            model.favorite = FavoriteStatus(model.RecordId);

            return PartialView("_FavoriteControl", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Favorite(CorrectionsViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                var checkfav = _db.FavoritesModels
                            .Where(x => x.RecordId == model.RecordId && x.userId == user.Id)
                            .Select(s => s).Count();

                if (checkfav != 1)
                {
                    var rec = _db.RecordModels
                            .Where(x => x.RecordId == model.RecordId)
                            .Select(s => s.title).SingleOrDefault();

                    var fav = _db.FavoritesModels.Add(new FavoritesModels
                    {
                        RecordId = model.RecordId,
                        title = rec,
                        userId = user.Id
                    });

                    await _db.SaveChangesAsync();
                }
            }

            TempData["id"] = model.RecordId;

            return RedirectToAction("FavoriteControl");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult PDF(CorrectionsViewModel model)
        {
            var rec = _db.RecordModels
                .Where(x => x.RecordId == model.RecordId)
                .Select(s => s).SingleOrDefault();

            string path = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pdffolder"]);
            string imgpath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["imgfolder"]);

            string fileName = path + rec.RecordId + ".pdf";

            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 0, 0, 0, 0);
            System.IO.FileStream fs = new FileStream(fileName, FileMode.Create);

                using (fs)
                {
                    int numFiles = 1;

                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);

                    doc.Open();

                    doc.AddTitle(rec.title);

                    try
                    {
                        for (int i = 1; i <= numFiles; i++)
                        {
                            string imageFilePath = imgpath + rec.image;
                            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageFilePath);

                            float pageWidth = doc.PageSize.Width;
                            float pageHeight = doc.PageSize.Height;

                            if (jpg.Width > jpg.Height)
                            {
                                jpg.ScaleAbsolute(pageWidth, pageHeight);
                                jpg.RotationDegrees = 90f;
                                jpg.ScaleToFit(pageWidth, pageHeight);
                            }
                            else
                            {
                                jpg.ScaleAbsolute(pageWidth, pageHeight);
                                jpg.ScaleToFit(pageWidth, pageHeight);
                            }

                            jpg.Alignment = Element.ALIGN_CENTER;

                            doc.Add(jpg);
                            doc.NewPage();
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.ToString());
                    }

                    doc.Close();
                    fs.Close();
                }

                string FileLocation = fileName;

                Document document = new Document();
                PdfReader reader = new PdfReader(FileLocation);
                PdfStamper stamper = new PdfStamper(reader, new FileStream(FileLocation.Replace(".pdf", "[OCR].pdf"), FileMode.Create));

                try
                {
                    var transcription = _db.VersionModels.Where(x => x.RecordId == rec.RecordId).Select(s => s);

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        Rectangle pageSize = reader.GetPageSizeWithRotation(i);
                        PdfContentByte pdfPageContents = stamper.GetUnderContent(i);

                        string ocrtext = null;

                        ArrayList arrList = new ArrayList();

                        foreach (var o in transcription)
                        {
                            arrList.Add(o.transcription);
                        }

                        var strings = from object o in arrList
                                      select o.ToString();

                        ocrtext = string.Join(" ", strings.ToArray());

                        Phrase p = new Phrase(ocrtext, FontFactory.GetFont(FontFactory.HELVETICA, 8f, Font.NORMAL));

                        pdfPageContents.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_INVISIBLE);

                        ColumnText ct = new ColumnText(pdfPageContents);
                        ct.SetSimpleColumn(p, 5, 190, pageSize.Width, pageSize.Height, 7, Element.ALIGN_LEFT);
                        ct.Go();
                    }

                    stamper.FormFlattening = true;
                    stamper.Close();
                    reader.Close();

                    System.IO.File.Delete(FileLocation);
                    string oldFileName = path + rec.RecordId + "[OCR].pdf";
                    string newFileName = FileLocation;
                    System.IO.File.Move(oldFileName, newFileName);
                }
                catch (Exception ex)
                {
                    Response.Write(ex.ToString());
                }

                return File(fileName, "application/pdf");
        }

        public string ActiveSession(int RecordId)
        {
            var sesh = _db.SessionModels
                        .Where(x => x.RecordId == RecordId && x.commit > SqlFunctions.DateAdd("minute", -3, DateTime.UtcNow) && x.commit <= DateTime.UtcNow && x.currentSession != HttpContext.Session.SessionID);

            int multiSession = sesh.Count();

            if (multiSession >= 1)
            {
                string activeSession = System.Configuration.ConfigurationManager.AppSettings["activeSessionMsg"];
                string value = "<div class='alert alert-info alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>" + activeSession + "</div>";

                return value;
            }
            return null;
        }

        public string RecordDifficulty(string diff)
        {
            string diffBtn;

            if (diff == "Beginner")
            {
                diffBtn = "diff-beginner";
            }
            else if (diff == "Intermediate")
            {
                diffBtn = "diff-intermediate";
            }
            else
            {
                diffBtn = "diff-advanced";
            }

            return diffBtn;
        }

        public string RecordStatus(string status)
        {
            string statusBtn;

            if (status == "Complete")
            {
                statusBtn = "btn-success";
            }
            else if (status == "In progress")
            {
                statusBtn = "btn-warning";
            }
            else
            {
                statusBtn = "btn-danger";
            }

            return statusBtn;
        }

        public string FavoriteStatus(int RecordId)
        {
            string favBtn;

            if (User.Identity.IsAuthenticated)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());

                var fav = _db.FavoritesModels
                .Where(x => x.RecordId == RecordId && x.userId == user.Id)
                .Select(s => s).Count();

                if (fav == 1)
                {
                    favBtn = "glyphicon-star";                    
                } 
                else
                {
                    favBtn = "glyphicon-star-empty";
                }

                return favBtn;
            }
            else
            {
                favBtn = "glyphicon-star-empty";
            }

            return favBtn;
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
