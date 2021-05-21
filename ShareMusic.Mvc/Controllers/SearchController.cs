using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShareMusic.Mvc.Data;
using ShareMusic.Mvc.Models;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShareMusic.Mvc.Controllers
{
    public class SearchController : Controller
    {
        private readonly ShareMusicMvcContext _context;
        public SearchController(ShareMusicMvcContext context)
        {
            _context = context;
        }
        public IActionResult Index(IFormCollection form)
        {
            string keySearch = form["searchtext"];
            List<int> PostId = new List<int>();
            Dictionary<string, string> Keyword = new Dictionary<string, string>();
            Keyword.Add("người dùng", "user"); Keyword.Add("nguoi dung", "user");
            Keyword.Add("chủ đề", "theme"); Keyword.Add("thể loại", "theme"); Keyword.Add("chu de", "theme"); Keyword.Add("the loai", "theme");
            Keyword.Add("*mới", "new"); Keyword.Add("*moi", "new");
            Keyword.Add("_của ai", "user"); Keyword.Add("_cua ai", "user");
            Keyword.Add("_tiêu đề", "title"); Keyword.Add("_tieu de", "title");
            IGraph g = new Graph();
            FileLoader.Load(g, "E:\\SematicWeb\\share-music-mvc\\ShareMusic.Mvc\\Data\\data.rdf");
            SparqlQueryParser parser = new SparqlQueryParser();
            string key = findKey(keySearch, Keyword);
            string attribute = findAttribute(keySearch, Keyword);
            string stringQuery = "SELECT ";
            Console.WriteLine("KeySearch = " + keySearch);
            if (attribute.Contains("id"))
            {
                stringQuery += "?id\n";
                stringQuery = stringQuery + "WHERE\n{\n" + key;
                stringQuery += "?x " + "<http://www.sharemusic.vn/id>" + " ?id.\n";
                stringQuery += "}";
            }
            else
            {
                if (attribute.Contains("user"))
                {
                    stringQuery += "?user\n";
                    stringQuery = stringQuery + "WHERE\n{\n" + key;
                    stringQuery += "?x " + "<http://www.sharemusic.vn/user>" + " ?user.\n";
                    stringQuery += "}";
                }
                else
                {
                    stringQuery += "?title\n";
                    stringQuery = stringQuery + "WHERE\n{\n" + key;
                    stringQuery += "?x " + "<http://www.sharemusic.vn/title>" + " ?title.\n";
                    stringQuery += "}";
                }
            }
            Console.WriteLine(stringQuery);

            string otherResult = "";
            string defaultQuery = "SELECT ?id\nWHERE\n{\n?x <http://www.sharemusic.vn/id> ?id.\n}";
            ViewBag.Result = "";
            if (stringQuery != defaultQuery)
            {
                SparqlQuery query = parser.ParseFromString(stringQuery);
                Object results = g.ExecuteQuery(query);
                if (results is SparqlResultSet)
                {
                    SparqlResultSet rset = (SparqlResultSet)results;
                    foreach (SparqlResult rs in rset)
                    {
                        string result = rs.ToString();
                        if (result.ToString().Contains("id"))
                        {
                            int id = GetId(result);
                            PostId.Add(id);
                        }
                        else
                        {
                            otherResult = GetOtherResult(result);
                        }
                    }
                }
                if (otherResult == "")
                {
                    List<Post> posts = new List<Post>();
                    if (PostId != null)
                    foreach (int id in PostId)
                    {
                        Post p = _context.Posts.Include(c=>c.Category).Where(p => p.Id == id).FirstOrDefault();
                        posts.Add(p);
                        ViewBag.Result = "id";
                    }
                    return View(posts);
                }
                else
                {
                    ViewBag.Result = otherResult;
                    return View();
                }
            }
            else
            {
                ViewBag.Result = "Default";
                var filteredPosts = _context.Posts.Include(x => x.Category).Select(p => p);
                filteredPosts = filteredPosts.Where(p => p.Title.Contains(keySearch));
                if (filteredPosts.ToList().Count == 0) ViewBag.Result = "Empty";
                return View(filteredPosts.ToList());
            }
        }

        public static string findKey(string keySearch, Dictionary<string, string> Keyword)
        {
            string key = "";
            foreach (var kw in Keyword)
            {
                if (keySearch.Contains(kw.Key))
                {
                    string t = keySearch.Substring(keySearch.IndexOf(kw.Key) + kw.Key.Length + 1);
                    key = key + "?x " + "<http://www.sharemusic.vn/" + kw.Value + ">" + " \"" + t + "\".\n";
                }
                if (keySearch.Contains(kw.Key.Substring(1)))
                {
                    if (kw.Key[0] == '*')
                    {
                        key = key + "?x " + "<http://www.sharemusic.vn/" + kw.Value + ">" + " \"" + kw.Key.Substring(1) + "\".\n";
                    }
                }
            }
            return key;
        }
        public static string findAttribute(string keySearch, Dictionary<string, string> Keyword)
        {
            string keyFind = "";
            foreach (var kw in Keyword)
            {
                if (keySearch.Contains(kw.Key.Substring(1)))
                {
                    if (kw.Key[0] == '_')
                    {

                        keyFind = "?x " + "<http://www.sharemusic.vn/" + kw.Value + ">" + " ?" + kw.Value + ".\n";
                        return keyFind;
                    }
                }
            }

            keyFind = "?x " + "<http://www.sharemusic.vn/id>" + " ?id.\n";
            return keyFind;
        }

        public static int GetId(string s)
        {
            int rs;
            int i = s.IndexOf("?id");
            rs = Convert.ToInt32(s.Substring(i + 5));
            return rs;
        }

        public static string GetOtherResult(string s)
        {
            int i = s.IndexOf(" = ");
            return s.Substring(i + 3);
        }

    }
}
