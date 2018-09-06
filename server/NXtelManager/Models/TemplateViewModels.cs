﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NXtelData;

namespace NXtelManager.Models
{
    public class TemplateEditModel
    {
        public Template Template { get; set; }
        public IEnumerable<SelectListItem> Templates { get; set; }

        public TemplateEditModel()
        {
            Templates = GetSelectList(NXtelData.Templates.LoadStubs());
        }

        public IEnumerable<SelectListItem> GetSelectList(Templates Items)
        {
            var rv = new List<SelectListItem>();
            if (Items == null) return rv;
            foreach (var item in Items)
            {
                rv.Add(new SelectListItem
                {
                    Value = item.TemplateID.ToString(),
                    Text = (item.Description ?? "").Trim()
                });
            }
            return rv;
        }
    }
}