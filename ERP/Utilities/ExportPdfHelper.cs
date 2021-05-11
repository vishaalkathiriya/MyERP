using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace ERP.Utilities
{
    public class ExportPdfHelper
    {
        /// <summary>
        /// Exports the specified HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="linkCss">The link CSS.</param>
        /// <param name="useChinaFont">if set to <c>true</c> [use china font].</param>
        public static void Export(string html, string fileName, string linkCss) //, bool useChinaFont = false
        {
            //reset response
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/pdf";

            //define pdf filename
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            HttpContext.Current.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Generate PDF
            using (var document = new Document(PageSize.A4, 0f, 0f, 0f, 0f))
            {
                //need when in live mode.
                //html = FormatImageLinks(html);

                //define output control HTML
                var memStream = new MemoryStream();
                TextReader xmlString = new StringReader(html);

                PdfWriter writer = PdfWriter.GetInstance(document, memStream);

                //open doc
                document.Open();

                // register all fonts in current computer
                //FontFactory.RegisterDirectories();

                // Set factories
                var htmlContext = new HtmlPipelineContext(null);
                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                // Set css
                ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                cssResolver.AddCssFile(HttpContext.Current.Server.MapPath(linkCss), true);

                //if (useChinaFont)
                //    cssResolver.AddCssFile(HttpContext.Current.Server.MapPath("~/Style/chinaPdfFont.css"), true);

                // Export
                IPipeline pipeline = new CssResolverPipeline(cssResolver,
                                                             new HtmlPipeline(htmlContext,
                                                                              new PdfWriterPipeline(document, writer)));
                var worker = new XMLWorker(pipeline, true);
                var xmlParse = new XMLParser(true, worker);
                xmlParse.Parse(xmlString);
                xmlParse.Flush();

                document.Close();
                document.Dispose();

                HttpContext.Current.Response.BinaryWrite(memStream.ToArray());
            }

            HttpContext.Current.Response.End();
            HttpContext.Current.Response.Flush();
        }

        /// <summary>
        /// Exports the specified HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="linkCss">The link CSS.</param>
        /// <param name="useChinaFont">if set to <c>true</c> [use china font].</param>
        public static void ExportInline(string html, string fileName, string linkCss, float mleft = 30f, float mright = 30f, float mtop = 30f, float mbottom = 30f) //, bool useChinaFont = false
        {
            //reset response
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/pdf";

            //define pdf filename
            HttpContext.Current.Response.AddHeader("content-disposition", "inline;attachment; filename=" + fileName);
            HttpContext.Current.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Generate PDF
            using (var document = new Document(PageSize.A4, mleft, mright, mtop, mbottom))
            {
                //need when in live mode.
                //html = FormatImageLinks(html);

                //define output control HTML
                var memStream = new MemoryStream();
                TextReader xmlString = new StringReader(html);

                PdfWriter writer = PdfWriter.GetInstance(document, memStream);

                //open doc
                document.Open();

                // register all fonts in current computer
                //FontFactory.RegisterDirectories();

                // Set factories
                var htmlContext = new HtmlPipelineContext(null);
                htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                // Set css
                ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                cssResolver.AddCssFile(HttpContext.Current.Server.MapPath(linkCss), true);

                //if (useChinaFont)
                //    cssResolver.AddCssFile(HttpContext.Current.Server.MapPath("~/Style/chinaPdfFont.css"), true);

                // Export
                IPipeline pipeline = new CssResolverPipeline(cssResolver,
                                                             new HtmlPipeline(htmlContext,
                                                                              new PdfWriterPipeline(document, writer)));
                var worker = new XMLWorker(pipeline, true);
                var xmlParse = new XMLParser(true, worker);
                xmlParse.Parse(xmlString);
                xmlParse.Flush();

                document.Close();
                document.Dispose();

                HttpContext.Current.Response.BinaryWrite(memStream.ToArray());
            }

            HttpContext.Current.Response.End();
            HttpContext.Current.Response.Flush();
        }

        /// <summary>
        /// Convert relative link to absolute link
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string FormatImageLinks(string input)
        {
            if (input == null)
                return string.Empty;
            string tempInput = input;
            const string pattern = @"<img(.|\n)+?>";
            HttpContext context = HttpContext.Current;

            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.
            foreach (Match m in Regex.Matches(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.RightToLeft))
            {
                if (m.Success)
                {
                    string tempM = m.Value;
                    string pattern1 = "src=[\'|\"](.+?)[\'|\"]";
                    Regex reImg = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    Match mImg = reImg.Match(m.Value);

                    if (mImg.Success)
                    {
                        string src = mImg.Value.ToLower().Replace("src=", "").Replace("\"", "").Replace("\'", "");

                        if (!src.StartsWith("http://") && !src.StartsWith("https://"))
                        {
                            //Insert new URL in img tag
                            src = "src=\"" + context.Request.Url.Scheme + "://" +
                                  context.Request.Url.Authority + src + "\"";
                            try
                            {
                                tempM = tempM.Remove(mImg.Index, mImg.Length);
                                tempM = tempM.Insert(mImg.Index, src);

                                //insert new url img tag in whole html code
                                tempInput = tempInput.Remove(m.Index, m.Length);
                                tempInput = tempInput.Insert(m.Index, tempM);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }
            return tempInput;
        }
    }
}