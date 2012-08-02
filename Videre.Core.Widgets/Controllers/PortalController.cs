﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Videre.Core.ActionResults;
using Videre.Core.Services;
//using DomainObjects = CodeEndeavors.ResourceManager.DomainObjects;
using CodeEndeavors.Extensions;
using Videre.Core.Extensions;
using CoreModels = Videre.Core.Models;
using CoreServices = Videre.Core.Services;
using System.IO;

namespace Videre.Core.Widgets.Controllers
{
    public class PortalController : Controller
    {

        public JsonResult<CoreModels.Portal> SavePortal(CoreModels.Portal portal)
        {
            return API.Execute<CoreModels.Portal>(r =>
            {
                Security.VerifyActivityAuthorized("Portal", "Administration");
                if (CoreServices.Portal.Save(portal))
                    r.Data = portal;
            });
        }

        public JsonResult<List<CoreModels.WidgetManifest>> GetManifests()
        {
            return API.Execute<List<CoreModels.WidgetManifest>>(r =>
            {
                r.Data = CoreServices.Portal.GetWidgetManifests();
            });
        }

        //public JsonResult GetManifestsPaged(string sidx, string sord, int page, int rows)
        //{
        //    var manifests = CoreServices.Portal.GetWidgetManifests();
        //    int pageIndex = Convert.ToInt32(page) - 1;
        //    int pageSize = rows;
        //    int totalRecords = manifests.Count;
        //    int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

        //    //manifests = manifests.OrderBy(sidx + " " + sord).Skip(pageIndex * pageSize).Take(pageSize);
        //    manifests = manifests.Skip(pageIndex * pageSize).Take(pageSize).ToList();

        //    return Json(new
        //    {
        //        total = totalPages,
        //        page,
        //        records = totalRecords,
        //        rows = (
        //            from manifest in manifests
        //            select new
        //            {
        //                i = manifest.Id,
        //                cell = new string[] { manifest.Category, manifest.Name, manifest.Title }
        //            }).ToArray()
        //    });

        //}

        public JsonResult<bool> SaveManifest(CoreModels.WidgetManifest manifest)
        {
            return API.Execute<bool>(r =>
            {
                Security.VerifyActivityAuthorized("Portal", "Administration");
                r.Data = !string.IsNullOrEmpty(CoreServices.Portal.Save(manifest));
            });
        }

        public JsonResult<bool> DeleteManifest(string id)
        {
            return API.Execute<bool>(r =>
            {
                Security.VerifyActivityAuthorized("Portal", "Administration");
                r.Data = CoreServices.Portal.DeleteManifest(id);
            });
        }

        public JsonResult<List<CoreModels.SecureActivity>> GetSecureActivities()
        {
            return API.Execute<List<CoreModels.SecureActivity>>(r =>
            {
                r.Data = CoreServices.Security.GetSecureActivities();
            });
        }

        public JsonResult<bool> SaveSecureActivity(CoreModels.SecureActivity activity)
        {
            return API.Execute<bool>(r =>
            {
                Security.VerifyActivityAuthorized("Portal", "Administration");
                r.Data = !string.IsNullOrEmpty(CoreServices.Security.Save(activity));
            });
        }

        public JsonResult<bool> DeleteSecureActivity(string id)
        {
            return API.Execute<bool>(r =>
            {
                Security.VerifyActivityAuthorized("Portal", "Administration");
                r.Data = CoreServices.Security.DeleteSecureActivity(id);
            });
        }

        public FileContentResult ExportPortal(string id)
        {
            Security.VerifyActivityAuthorized("Portal", "Administration");
            var export = CoreServices.Portal.ExportPortal(id);
            var json = export.ToJson(pretty: true, ignoreType: "db");   //todo: use db for export, or its own type?
            return File(System.Text.Encoding.UTF8.GetBytes(json), "text/plain", "ExportPortal.json");
        }

        public FileContentResult ExportTemplates(string id)
        {
            Security.VerifyActivityAuthorized("Portal", "Administration");
            var export = CoreServices.Portal.ExportTemplates(id, true);
            var json = export.ToJson(pretty: true, ignoreType: "db");   //todo: use db for export, or its own type?
            return File(System.Text.Encoding.UTF8.GetBytes(json), "text/plain", "ExportPortalTemplates.json");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult<dynamic> ImportPortal(string qqfile)
        {
            return API.Execute<dynamic>(r =>
            {
                r.ContentType = "text/html";
                string fileName = null;
                //var tempFileName = Portal.GetTempFile(CoreServices.Portal.CurrentPortalId);
                System.IO.Stream stream = null;
                if (string.IsNullOrEmpty(Request["qqfile"]))    //IE
                {
                    fileName = Request.Files[0].FileName;
                    stream = Request.Files[0].InputStream;
                }
                else
                {
                    fileName = qqfile;
                    stream = Request.InputStream;
                }
                var ext = fileName.Substring(fileName.LastIndexOf(".") + 1);
                var saveFileName = Update.UpdateDir + fileName;
                if (Web.MimeTypes.ContainsKey(ext) && (ext.Equals("json", StringComparison.InvariantCultureIgnoreCase) || ext.Equals("zip", StringComparison.InvariantCultureIgnoreCase)))
                {
                    var fileSize = stream.WriteStream(saveFileName);
                    r.Data = new
                    {
                        uniqueName = new FileInfo(saveFileName).Name,
                        fileName = fileName,
                        fileSize = fileSize,
                        mimeType = Web.MimeTypes[ext]
                    };
                    r.AddMessage(Localization.GetPortalText("ImportMessage.Text", "File has been placed in the update folder.  It will be applied shortly"));
                }
                else
                    throw new Exception(Localization.GetExceptionText("InvalidMimeType.Error", "{0} is invalid.", ext));
            });
        }

        public JsonResult<List<CoreModels.Theme>> GetInstalledThemes()
        {
            return API.Execute<List<CoreModels.Theme>>(r =>
            {
                Security.VerifyActivityAuthorized("Portal", "Administration");
                r.Data = CoreServices.UI.GetThemes();
            });
        }

        public JsonResult<List<CoreModels.Theme>> InstallTheme(CoreModels.Theme theme)
        {
            return API.Execute<List<CoreModels.Theme>>(r =>
            {
                Security.VerifyActivityAuthorized("Portal", "Administration");
                CoreServices.UI.InstallTheme(theme);
                r.Data = CoreServices.UI.GetThemes();
            });
        }

        public JsonResult<List<CoreModels.Theme>> UninstallTheme(string name)
        {
            return API.Execute<List<CoreModels.Theme>>(r =>
            {
                Security.VerifyActivityAuthorized("Portal", "Administration");
                CoreServices.UI.UninstallTheme(name);
                r.Data = CoreServices.UI.GetThemes();
            });
        }

    }
}
