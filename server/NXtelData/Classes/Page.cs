﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace NXtelData
{
    public class Page : PageBase
    {
        public int PageID { get; set; }
        public int PageNo { get; set; }
        public int FrameNo { get; set; }
        public string Title { get; set; }
        public byte? DateX { get; set; }
        public byte? DateY { get; set; }
        public byte? TimeX { get; set; }
        public byte? TimeY { get; set; }
        public bool BoxMode { get; set; }
        public Templates Templates { get; set; }
        public Routes Routing { get; set; }
        public string SelectedTemplates { get; set; }
        public string SelectedRoutes { get; set; }
        public int ContentHeight { get; set; }
        public int ContentCurrentLine { get; set; }
        public int ToPageNo { get; set; }
        public int ToFrameNo { get; set; }
        public int? TeleSoftwareID { get; set; }
        public Pages PageRange { get; set; }

        public Page()
        {
            PageID = -1;
            Title = URL = SelectedTemplates = SelectedRoutes = "";
            Templates = new Templates();
            Routing = new Routes();
            PageRange = new Pages();
            this.ConvertContentsFromURL();
        }

        public string Frame
        {
            get
            {
                return ((char)Convert.ToByte(((byte)"a"[0]) + FrameNo)).ToString();
            }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    FrameNo = 0;
                    return;
                }
                char chr = (value.Trim().ToLower())[0];
                if (chr < 'a')
                    FrameNo = 0;
                else if (chr > 'z')
                    FrameNo = 25;
                else
                    FrameNo = chr - 'a';
            }
        }

        public string ToFrame
        {
            get
            {
                return ((char)Convert.ToByte(((byte)"a"[0]) + ToFrameNo)).ToString();
            }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    ToFrameNo = 0;
                    return;
                }
                char chr = (value.Trim().ToLower())[0];
                if (chr < 'a')
                    ToFrameNo = 0;
                else if (chr > 'z')
                    ToFrameNo = 25;
                else
                    ToFrameNo = chr - 'a';
            }
        }

        public string PageAndFrame
        {
            get
            {
                return PageNo + Frame;
            }
        }

        public string ToPageAndFrame
        {
            get
            {
                return ToPageNo + ToFrame;
            }
        }

        public static Page Load(int PageNo, int FrameNo)
        {
            var item = PageCache.GetPage(PageNo, FrameNo);
            if (item == null)
            {
                item = new Page();
                using (var con = new MySqlConnection(DBOps.ConnectionString))
                {
                    con.Open();
                    string sql = @"SELECT * 
                    FROM page 
                    WHERE @PageFrameNo>=FromPageFrameNo
                    AND @PageFrameNo<=ToPageFrameNo
                    ORDER BY FromPageFrameNo,ToPageFrameNo
                    LIMIT 1;";
                    var cmd = new MySqlCommand(sql, con);
                    decimal pageFrameNo = PageNo + (Convert.ToDecimal(FrameNo) / 100m);
                    cmd.Parameters.AddWithValue("PageFrameNo", pageFrameNo);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            item.Read(rdr);
                            break;
                        }
                    }
                    if (item.PageID > 0)
                    {
                        item.Templates = Templates.LoadForPage(item.PageID, con);
                        item.Routing = Routes.LoadForPage(item.PageID, con);
                        item.Compose();
                        new TSEncoder().Encode(ref item);
                    }
                }
            }
            if (item.PageNo != PageNo || item.FrameNo != FrameNo)
            {
                var page = item.PageRange.FirstOrDefault(p => p.PageNo == PageNo && p.FrameNo == FrameNo);
                if (page != null)
                {
                    page.PageRange.Clear();
                    page.PageRange.AddRange(item.PageRange);
                    page.Templates.Clear();
                    page.Templates.AddRange(item.Templates);
                    page.Compose();
                    var nextPage = page.PageRange.FirstOrDefault(p => p.PageNo == page.NextPageNo && p.FrameNo == page.NextFrameNo);
                    if (nextPage == null)
                    {
                        page.Routing.AddOrUpdate((byte)RouteKeys.Enter, Options.MainIndexPageNo, Options.MainIndexFrameNo);
                        page.Routing.AddOrUpdate((byte)RouteKeys.K0, Options.MainIndexPageNo, Options.MainIndexFrameNo);
                    }
                    else
                    {
                        page.Routing.AddOrUpdate((byte)RouteKeys.Enter, page.NextPageNo, page.NextFrameNo);
                        page.Routing.AddOrUpdate((byte)RouteKeys.K0, page.NextPageNo, page.NextFrameNo);
                    }
                    item = page;
                }
            }
            return item;
        }

        public static Page Load(int PageID)
        {
            var item = new Page();
            using (var con = new MySqlConnection(DBOps.ConnectionString))
            {
                con.Open();
                string sql = "SELECT * FROM page WHERE PageID=" + PageID;
                var cmd = new MySqlCommand(sql, con);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        item.Read(rdr);
                        break;
                    }
                }
                if (item.PageID > 0)
                {
                    item.Templates = Templates.LoadForPage(item.PageID, con);
                    item.SetSelectedTemplates();
                    item.Routing = Routes.LoadForPage(item.PageID, con);
                    item.SetSelectedRoutes();
                }
            }
            return item;
        }

        public static bool Save(Page Page, out string Err)
        {
            Err = "";
            try
            {
                if (Page.PageID <= 0)
                    return Page.Create(out Err);
                else
                    return Page.Update(out Err);
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return false;
            }
        }

        public bool Create(out string Err)
        {
            Err = "";
            try
            {
                using (var con = new MySqlConnection(DBOps.ConnectionString))
                {
                    con.Open();
                    string sql = @"INSERT INTO page
                        (PageNo,FrameNo,Title,Contents,BoxMode,URL,
                        FromPageFrameNo,ToPageFrameNo,TeleSoftwareID)
                        VALUES(@PageNo,@FrameNo,@Title,@Contents,@BoxMode,@URL,
                        @FromPageFrameNo,@ToPageFrameNo,@TeleSoftwareID);
                        SELECT LAST_INSERT_ID();";
                    var cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("PageNo", PageNo);
                    cmd.Parameters.AddWithValue("FrameNo", FrameNo);
                    cmd.Parameters.AddWithValue("Title", (Title ?? "").Trim());
                    cmd.Parameters.AddWithValue("BoxMode", BoxMode);
                    cmd.Parameters.AddWithValue("URL", (URL ?? "").Trim());
                    cmd.Parameters.AddWithValue("Contents", Contents);
                    decimal fromPageFrameNo = PageNo + (Convert.ToDecimal(FrameNo) / 100m);
                    cmd.Parameters.AddWithValue("FromPageFrameNo", fromPageFrameNo);
                    decimal toPageFrameNo = ToPageNo + (Convert.ToDecimal(ToFrameNo) / 100m);
                    cmd.Parameters.AddWithValue("ToPageFrameNo", toPageFrameNo);
                    int? tsid = TeleSoftwareID != null && TeleSoftwareID > 0 ? TeleSoftwareID : null;
                    cmd.Parameters.AddWithValue("TeleSoftwareID", tsid);
                    int rv = cmd.ExecuteScalarInt32();
                    if (rv > 0)
                        PageID = rv;
                    if (PageID <= 0)
                        Err = "The page could not be saved.";

                    if (PageID > 0)
                    {
                        Templates.SaveForPage(PageID, out Err, con);
                        if (!string.IsNullOrWhiteSpace(Err))
                            return false;
                        Routing.SaveForPage(PageID, out Err, con);
                        if (!string.IsNullOrWhiteSpace(Err))
                            return false;
                    }

                    return PageID > 0;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate entry"))
                    Err = "Page " + PageNo + Frame + " already exists.";
                else
                    Err = ex.Message;
                return false;
            }
        }

        public bool Update(out string Err)
        {
            Err = "";
            try
            {
                using (var con = new MySqlConnection(DBOps.ConnectionString))
                {
                    con.Open();
                    string sql = @"UPDATE page
                        SET PageNo=@PageNo,
                        FrameNo=@FrameNo,
                        Title=@Title,
                        BoxMode=@BoxMode,
                        URL=@URL,
                        Contents=@Contents,
                        FromPageFrameNo=@FromPageFrameNo,
                        ToPageFrameNo=@ToPageFrameNo,
                        TeleSoftwareID=@TeleSoftwareID
                        WHERE PageID=@PageID;
                        SELECT ROW_COUNT();";
                    var cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("PageID", PageID);
                    cmd.Parameters.AddWithValue("PageNo", PageNo);
                    cmd.Parameters.AddWithValue("FrameNo", FrameNo);
                    cmd.Parameters.AddWithValue("Title", (Title ?? "").Trim());
                    cmd.Parameters.AddWithValue("BoxMode", BoxMode);
                    cmd.Parameters.AddWithValue("URL", (URL ?? "").Trim());
                    cmd.Parameters.AddWithValue("Contents", Contents);
                    decimal fromPageFrameNo = PageNo + (Convert.ToDecimal(FrameNo) / 100m);
                    cmd.Parameters.AddWithValue("FromPageFrameNo", fromPageFrameNo);
                    decimal toPageFrameNo = ToPageNo + (Convert.ToDecimal(ToFrameNo) / 100m);
                    cmd.Parameters.AddWithValue("ToPageFrameNo", toPageFrameNo);
                    int? tsid = TeleSoftwareID != null && TeleSoftwareID > 0 ? TeleSoftwareID : null;
                    cmd.Parameters.AddWithValue("TeleSoftwareID", tsid);
                    int rv = cmd.ExecuteScalarInt32();
                    if (rv <= 0)
                        Err = "The page could not be saved.";

                    if (PageID > 0)
                    {
                        Templates.SaveForPage(PageID, out Err, con);
                        if (!string.IsNullOrWhiteSpace(Err))
                            return false;
                        Routing.SaveForPage(PageID, out Err, con);
                        if (!string.IsNullOrWhiteSpace(Err))
                            return false;
                    }

                    return rv > 0;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate entry"))
                    Err = "Page " + PageNo + Frame + " already exists.";
                else
                    Err = ex.Message;
                return false;
            }
        }

        public bool Delete(out string Err)
        {
            Err = "";
            try
            {
                using (var con = new MySqlConnection(DBOps.ConnectionString))
                {
                    con.Open();
                    string sql = @"DELETE FROM page WHERE PageID=@PageID;";
                    var cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("PageID", PageID);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return false;
            }
        }

        public bool IsPageRangeValid()
        {
            using (var con = new MySqlConnection(DBOps.ConnectionString))
            {
                con.Open();
                string sql = @"SELECT COUNT(*) AS cnt
                    FROM `page`
                    WHERE PageID<>@PageID
                    AND FromPageFrameNo<@ToPageFrameNo
                    AND @FromPageFrameNo<ToPageFrameNo;";
                var cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("FromPageFrameNo", NormalisedFromPageFrameNo);
                cmd.Parameters.AddWithValue("ToPageFrameNo", NormalisedToPageFrameNo);
                cmd.Parameters.AddWithValue("PageID", PageID);
                return cmd.ExecuteScalarInt32() == 0;
            }
        }

        public void Read(MySqlDataReader rdr, bool StubOnly = false)
        {
            this.PageID = rdr.GetInt32("PageID");
            this.PageNo = rdr.GetInt32("PageNo");
            this.FrameNo = rdr.GetInt32("FrameNo");
            this.Title = rdr.GetString("Title").Trim();
            if (string.IsNullOrEmpty(this.Title))
                this.Title = "None";
            decimal toPageFrameNo = rdr.GetDecimal("ToPageFrameNo");
            this.ToPageNo = Convert.ToInt32(toPageFrameNo);
            this.ToFrameNo = Convert.ToInt32((toPageFrameNo - this.ToPageNo) * 100);
            if (StubOnly) return;
            this.Contents = rdr.GetBytesNullable("Contents");
            this.URL = rdr.GetStringNullable("URL").Trim();
            this.BoxMode = rdr.GetBoolean("BoxMode");
            this.TeleSoftwareID = rdr.GetInt32Nullable("TeleSoftwareID");
            this.ConvertContentsFromURL();
        }

        public void SetVersion(string Version)
        {
            const int LEN = 9;
            string ver = ("v" + Version.Trim()).PadLeft(LEN);
            for (int i = 0; i < Contents.Length - LEN + 1; i++)
            {
                if (Convert.ToChar(Contents[i]).ToString() == "["
                    && Convert.ToChar(Contents[i + 1]).ToString() == "V"
                    && Convert.ToChar(Contents[i + 2]).ToString() == "E"
                    && Convert.ToChar(Contents[i + 3]).ToString() == "R"
                    && Convert.ToChar(Contents[i + 4]).ToString() == "S"
                    && Convert.ToChar(Contents[i + 5]).ToString() == "I"
                    && Convert.ToChar(Contents[i + 6]).ToString() == "O"
                    && Convert.ToChar(Contents[i + 7]).ToString() == "N"
                    && Convert.ToChar(Contents[i + 8]).ToString() == "]")
                {
                    Contents[i] = Convert.ToByte(ver[0]);
                    Contents[i + 1] = Convert.ToByte(ver[1]);
                    Contents[i + 2] = Convert.ToByte(ver[2]);
                    Contents[i + 3] = Convert.ToByte(ver[3]);
                    Contents[i + 4] = Convert.ToByte(ver[4]);
                    Contents[i + 5] = Convert.ToByte(ver[5]);
                    Contents[i + 6] = Convert.ToByte(ver[6]);
                    Contents[i + 7] = Convert.ToByte(ver[7]);
                    Contents[i + 8] = Convert.ToByte(ver[8]);
                    _contents7BitEncoded = null;
                    return;
                }
            }
        }

        public void Compose()
        {
            if (Contents == null)
                Contents = Encoding.ASCII.GetBytes(new string(' ', 960));
            if (Contents.Length != 960)
                Contents = Pad(Contents, 960, 32);
            foreach (var template in FlattenTemplates() ?? new Templates())
                template.Compose(this);
            Contents = Pad(Contents, 960, 32);
        }

        public override void Fixup()
        {
            Frame = Frame;

            // Templates
            Templates = new Templates();
            Templates templates = null;
            foreach (string cid in (SelectedTemplates ?? "").Split(','))
            {
                int id;
                int.TryParse(cid, out id);
                if (id <= 0)
                    continue;
                if (Templates.Any(t => t.TemplateID == id))
                    continue;
                if (templates == null)
                    templates = Templates.Load();
                var matched = templates.FirstOrDefault(t => t.TemplateID == id);
                if (matched != null)
                    Templates.Add(matched);
            }

            // Routing
            Routing = new Routes();
            foreach (string rr in (SelectedRoutes ?? "").Split(','))
            {
                var split = rr.Split(';');
                if (split.Length < 5)
                    continue;
                var route = new Route();
                byte bVal;
                byte.TryParse(split[0], out bVal);
                route.KeyCode = bVal;
                int iVal;
                if (int.TryParse(split[1], out iVal) && iVal >= 0)
                    route.NextPageNo = iVal;
                route.NextFrame = split[2].Trim().ToLower();
                bool lVal;
                bool.TryParse((split[3] ?? "").Trim().ToLower().Replace("t", "T").Replace("f", "F"), out lVal);
                route.GoNextPage = lVal;
                bool.TryParse((split[4] ?? "").Trim().ToLower().Replace("t", "T").Replace("f", "F"), out lVal);
                route.GoNextFrame = lVal;
                var matched = Routing.FirstOrDefault(r => r.KeyCode == route.KeyCode);
                if (matched == null)
                    Routing.Add(route);
            }

            // Range
            if (ToPageNo <= 0 && ToFrameNo <= 0)
            {
                ToPageNo = PageNo;
                ToFrameNo = FrameNo;
            }
        }

        public void SetSelectedTemplates()
        {
            var sel = (Templates ?? new Templates()).Select(t => t.TemplateID).Distinct().OrderBy(i => i);
            SelectedTemplates = string.Join(",", sel);
        }

        public void SetSelectedRoutes()
        {
            string join = "";
            string val = "";
            foreach (var route in Routing ?? new Routes())
            {
                string sel = route.KeyCode + ";"
                    + (route.NextPageNo == null ? "" : route.NextPageNo.ToString()) + ";"
                    + route.NextFrame + ";"
                    + route.GoNextPage.ToString().ToLower() + ";"
                    + route.GoNextFrame.ToString().ToLower();
                val += join + sel;
                join = ",";
            }
            SelectedRoutes = val;
        }

        public Templates FlattenTemplates()
        {
            var rv = new Templates();
            foreach (var t in Templates)
                t.AddChildTemplates(ref rv);
            return rv;
        }

        public int NextPageNo
        {
            get
            {
                if (FrameNo == 25)
                    return PageNo + 1;
                else
                    return PageNo;
            }
        }

        public int NextFrameNo
        {
            get
            {
                if (FrameNo == 25)
                    return 0;
                else
                    return FrameNo + 1;
            }
        }

        public decimal NormalisedFromPageFrameNo
        {
            get
            {
                return PageNo + (Convert.ToDecimal(FrameNo) / 100m);
            }
        }

        public decimal NormalisedToPageFrameNo
        {
            get
            {
                return ToPageNo + (Convert.ToDecimal(ToFrameNo) / 100m);
            }
        }
    }
}
