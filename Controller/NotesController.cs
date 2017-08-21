using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using DbConnection;

namespace quotingDojo{

using Newtonsoft.Json;

    public static class SessionExtensions // Somewhere in your namespace, outside other classes
    {
        public static void SetObjectAsJson(this ISession session, string key, object value) // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
        {
            session.SetString(key, JsonConvert.SerializeObject(value)); // This helper function simply serializes theobject to JSON and stores it as a string in session
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)  // generic type T is a stand-in indicating that we need to specify the type on retrieval
        {
            string value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);   // Upon retrieval the object is deserialized based on the type we specified
        }
    }

    public class NotesController : Controller
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View("index");
        }

        [HttpGet]
        [Route("/query")]
        public IActionResult Query()
        {
            List<Dictionary<string, object>> AllNotes = DbConnector.Query("SELECT * FROM notes ORDER BY id ASC");
            return Json(AllNotes);
        }

        [HttpPost]
        [Route("/add")]
        public IActionResult Add(string description, string note)
        {
            string query = $"INSERT INTO notes (description, note, created_at) VALUES('{description}', '{note}', NOW());";
            DbConnector.Execute(query);
            return Redirect("/");
        }

        [Route("/update/{nid}")]
        public IActionResult Update(int nid, string description, string note)
        {
            string query=$"UPDATE notes SET description='{description}', note='{note}', updated_at=NOW() WHERE id={nid};";
            DbConnector.Execute(query);
            return Redirect("/");
        }

        [Route("/delete/{nid}")]
        public IActionResult Delete(int nid)
        {
            if((nid > 1) && (nid < 3000))
            {
                string query=$"DELETE FROM notes WHERE id='{nid}';";
                DbConnector.Execute(query);
                return Redirect("/");
            }
            else
            {
                TempData["Error"] = "invalid entry";
                return Redirect("/");
            }
        }
    }
}