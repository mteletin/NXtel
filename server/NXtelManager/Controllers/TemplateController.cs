﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NXtelData;
using NXtelManager.Attributes;
using NXtelManager.Models;

namespace NXtelManager.Controllers
{
    [Authorize]
    public class TemplateController : Controller
    {
        public ActionResult Index()
        {
            var templates = Templates.LoadStubs(true);
            return View(templates);
        }

        public ActionResult Edit(int? ID)
        {
            int id = ID ?? -1;
            var model = new TemplateEditModel();
            model.Template = Template.Load(id);

            var flat = model.Template.FlattenTemplates();
            string x = "";
            foreach (var t in flat)
                x += t.TemplateID + ": " + t.Description + "\r\n";

            if (id != -1 && model.Template.TemplateID <= 0)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        [MultipleButton("save")]
        public ActionResult Save(Template Template)
        {
            TemplateEditModel model;
            Template.Fixup();
            if (ModelState.IsValid)
            {
                string err;
                if (!Template.Save(Template, out err))
                {
                    ModelState.AddModelError("", err);
                    model = new TemplateEditModel();
                    model.Template = Template;
                    return View("Edit", model);
                }
                return RedirectToAction("Index");
            }
            model = new TemplateEditModel();
            model.Template = Template;
            return View("Edit", model);
        }

        [HttpPost]
        [MultipleButton("delete")]
        public ActionResult Delete(Template Template)
        {
            TemplateEditModel model;
            if (Template == null || Template.TemplateID <= 0)
                return RedirectToAction("Index");
            string err;
            if (!Template.Delete(out err))
            {
                ModelState.AddModelError("", err);
                model = new TemplateEditModel();
                model.Template = Template;
                return View("Edit", model);
            }
            return RedirectToAction("Index");
        }
    }
}