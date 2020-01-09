using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
    public static class ScanHelper
    {
        private static List<string> ScannedUrls = new List<string>();

        private static int _iteration = 0;

        public static List<string> GetResultFromUrlWithDepth(string url, int depth)
        {
            while (true)
            {
                if (_iteration == 0 && depth == 1)
                {
                    var document = GetWebPageFromUrl(url);
                    GetAllUrlsFrom(document, url);
                    return ScannedUrls;
                }
                if (_iteration == depth)
                    return ScannedUrls;
                if (_iteration == 0)
                {
                    var document = GetWebPageFromUrl(url);
                    GetAllUrlsFrom(document, url);
                    _iteration++;
                }
                else
                {
                    var oldScannedUrl = new List<string>();
                    foreach (var element in ScannedUrls)
                    {
                        oldScannedUrl.Add(element);
                    }
                    foreach (var scannedUrl in oldScannedUrl)
                    {
                        var document = GetWebPageFromUrl(url);
                        GetAllUrlsFrom(document, scannedUrl);
                    }

                    ScannedUrls = oldScannedUrl.Union(ScannedUrls).Distinct().ToList();
                    _iteration++;
                }
            }
        }

        private static HtmlDocument GetWebPageFromUrl(string url)
        {
            var hw = new HtmlWeb();
            return hw.Load(url);
        }

        private static void GetAllUrlsFrom(HtmlDocument document, string url)
        {
            var anotherProtocolUrl = "";
            if (url.StartsWith("http"))
                anotherProtocolUrl = "https" + url.Remove(0, 4);
            else
                anotherProtocolUrl = "http" + url.Remove(0, 5);

            var links = document.DocumentNode.SelectNodes("//a");
            foreach (var link in links)
            {
                var rawLink = link.GetAttributeValue("href", null);
                if (rawLink != null)
                {
                    if (rawLink.StartsWith('/') && rawLink.Length >= 2 && rawLink[1] != '/')
                        rawLink = url.Remove(url.Length - 1) + rawLink;

                    if (rawLink.Contains(url) || rawLink.Contains(anotherProtocolUrl) && !ScannedUrls.Contains(rawLink))
                        ScannedUrls.Add(rawLink);
                }
            }
        }

        public static List<ExamModel> GetSavedPages()
        {
            var list = new List<ExamModel>();
            foreach (var url in ScannedUrls)
            {
                var document=GetWebPageFromUrl(url);
                var parsedText = RemoveTags(document);
                list.Add(new ExamModel{Body = parsedText, Url = url});
            }

            return list;
        }

        public static string RemoveTags(HtmlDocument document)
        {
            var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));
            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;

                if (node.Name != "#text")
                {
                    var childNodes = node.SelectNodes("./*|./text()");

                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            nodes.Enqueue(child);
                            parentNode.InsertBefore(child, node);
                        }
                    }
                    parentNode.RemoveChild(node);
                }
            }

            return document.DocumentNode.InnerHtml;
        }
    }

}