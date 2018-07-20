using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMoviesJuly2018.Models; //linking in the database to the controller
using System.Data.Entity;
using System.Net;

namespace MvcMoviesJuly2018.Controllers
{
    public class HomeController : Controller
    {
        private MoviesJuly2018Entities db = new MoviesJuly2018Entities(); // database connection object 
        public ActionResult Index(string searchString, string movieGenre)
        {
            //get the data for the dropdown list
            List<string> genreList = new List<string>();

            //get genres from db
            var genreQuery = from g in db.Movies
                             orderby g.Genre
                             select g.Genre;
            //adding unique genres to list
            genreList.AddRange(genreQuery.Distinct()); //.Distinct gets rid of duplicate values

            //turn genre list into a SelectList and put in the ViewBag to go to the view
            ViewBag.movieGenre = new SelectList(genreList);
            //------------------------------------------------------------------------------------------------------------------End of creating the dropdown list


            //equivalent to the SQL Link * FROM table - known as a LINQ query for all the records in the database
            var movies = from m in db.Movies
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(x => x.Title.Contains(searchString)); // benefit of .contain, only need to match letters in the string, rather than == - meaning the exact same whole string 
            }
            // search for movies by genre
            if (!String.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre); // benefit of .contain, only need to match letters in the string, rather than == - meaning the exact same whole string 
            }

            return View(movies);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Title,ReleaseDate,Genre,Price,ImageURL")]Movie movie)
        {
            if(movie.ImageURL == null)
            {
                movie.ImageURL = "https://lajoyalink.com/wp-content/uploads/2018/03/Movie.jpg";
            }
            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,ReleaseDate,Genre,Price,ImageURL")]Movie movie) 
        {
            if (movie.ImageURL == null)
            {
                movie.ImageURL = "https://lajoyalink.com/wp-content/uploads/2018/03/Movie.jpg";
            }
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        public ActionResult Edit(int? id)
        {
            Movie movie = db.Movies.Find(id);

            //handles missing id in URL
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (movie == null)
            {
                return HttpNotFound();
            }

            return View();
        }

        public ActionResult Details(int? id)
        {
            //handles missing id in URL
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Movie movie = db.Movies.Find(id);
            if(movie == null)
            {
                return HttpNotFound();
            }

            return View(movie);
        }

        public ActionResult Delete(int? id)
        {
            //handles missing id in URL
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}