using System;
using System.Threading.Tasks;
using System.IO;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.Linq;
using Newtonsoft.Json;
using AngleSharp.Dom;
namespace html_crawler
{
    class Program
    {
        static async Task Main(string[] args)
        {

            string path = @"c:\temp\Wikipedia.html";


            string source = GetHTML(path);
            var document = await GetDocument(source);

            var languages = GetLanguages(document);

            foreach ( var lang in languages)
            {
                Console.WriteLine(lang);
            }

            
            Console.ReadLine();
        }

        public static string[] GetLanguages(IDocument document)
        {
            var centralFeaturedDiv = document.All.First(m => m.ClassList.Contains("central-featured"));


            //var allLangs = centralFeaturedDiv
            //    .Children
            //    .Where(element => element.ClassList.Contains("central-featured-lang"))
            //    .Select(element => element.TextContent)
            //    .ToArray();

            var allLangsAsJson = centralFeaturedDiv
                .Children
                .Where(element => element.ClassList.Contains("central-featured-lang"))
                .Select( LanguageObject)
                .Select(StringifyExtractedText)
                .ToArray();


            return allLangsAsJson;
        }

        public static ExtractedText LanguageObject(IElement element)
        {
            var contentOfAnchor = element.Children.First(item => item.LocalName == "a");
            var contentOfStrong = contentOfAnchor.Children.First(item => item.LocalName == "strong").TextContent;
            var contentOfSmall = contentOfAnchor.Children.First(item => item.LocalName == "small").TextContent;
            var result = new ExtractedText();
            result.Language = contentOfStrong;
            result.Info = contentOfSmall;
            return result;
        }

        public static string StringifyExtractedText(ExtractedText extractedText)
        {
            return  JsonConvert.SerializeObject(extractedText);
        }

        
        public static string GetHTML(string path)

        {
            string source = string.Empty;

            using (StreamReader sr = File.OpenText(path))
            {

                string liniaCurenta;
                while ((liniaCurenta = sr.ReadLine()) != null)
                {
                    source = source + liniaCurenta;
                }
            }
            return source;
        }

        public static async Task<AngleSharp.Dom.IDocument> GetDocument(string source)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(source));
            return document;
        }
    }

    class ExtractedText
    {
        public string Language { get; set; }
        public string Info { get; set; }
    }
}
