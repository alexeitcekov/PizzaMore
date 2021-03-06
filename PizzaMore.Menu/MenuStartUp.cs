﻿namespace PizzaMore.Menu
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using PizzaMore.Data;
    using PizzaMore.Data.Models;
    using PizzaMore.Utility;

    class MenuStartUp
    {
        private const string NavBarPath = "../www/PizzaMore/menu-top.html";
        private const string BottomPagePath = "../www/PizzaMore/menu-bottom.html";
        private const string DefaultMenuPage = "../www/PizzaMore/Menu.html";
        private const string DefaultIncorrectParamsPath = "../www/PizzaMore/404.html";

        public static Header Header = new Header();
        public static Session Session;
        private static IDictionary<string, string> requestParameters = new Dictionary<string, string>();
        static void Main()
        {
            Session = WebUtil.GetSession();

            if (Session == null)
            {
                ShowErrorPage(DefaultIncorrectParamsPath);
            }

            if (WebUtil.IsGet())
            {
                Header.Print();
                WebUtil.PrintFileContent(DefaultMenuPage);
            }
            else if (WebUtil.IsPost())
            {
                requestParameters = WebUtil.RetrievePostParameters();
                VoteForPizza(requestParameters);
                ShowPage();
            }
            else
            {
                ShowErrorPage(DefaultIncorrectParamsPath);
            }
        }

        private static void VoteForPizza(IDictionary<string, string> requestParameters)
        {
            if (!requestParameters.ContainsKey("pizzaVote") || !requestParameters.ContainsKey("pizzaid"))
            {
                return;
            }

            var vote = requestParameters["pizzaVote"];

            if (vote != "up" && vote != "down")
            {
                return;
            }

            var pizzaId = requestParameters["pizzaid"];

            int id;

            if (!int.TryParse(pizzaId, out id))
            {
                return;
            }

            

            var db = new PizzaMoreContext();

            var pizza = db.Pizzas.FirstOrDefault(x => x.Id == id);

            if (pizza == null)
            {
                return;
            }

            if (vote == "up")
            {
                pizza.UpVotes++;
            }
            else
            {
                pizza.DownVotes--;
            }

            db.Pizzas.AddOrUpdate(pizza);

            db.SaveChanges();
        }

        private static void ShowPage()
        {
            Header.Print();

            GenerateNavBar();

            WebUtil.PrintFileContent(NavBarPath);

            GenerateAllSuggestions();

            WebUtil.PrintFileContent(BottomPagePath);
        }
        

        private static void GenerateAllSuggestions()
        {
            var db = new PizzaMoreContext();

            var pizzas = db.Pizzas;

            Console.WriteLine("<div class=\"card-deck\">");
            foreach (var pizza in pizzas)
            {
                Console.WriteLine("<div class=\"card\">");
                Console.WriteLine($"<img class=\"card-img-top\" src=\"{pizza.ImgUrl}\" width=\"200px\"alt=\"Card image cap\">");
                Console.WriteLine("<div class=\"card-block\">");
                Console.WriteLine($"<h4 class=\"card-title\">{pizza.Title}</h4>");
                Console.WriteLine($"<p class=\"card-text\"><a href=\"PizzaMore.DetailsPizza.exe?pizzaid={pizza.Id}\">Recipe</a></p>");
                Console.WriteLine("<form method=\"POST\">");
                Console.WriteLine($"<div class=\"radio\"><label><input type = \"radio\" name=\"pizzaVote\" value=\"up\">Up</label></div>");
                Console.WriteLine($"<div class=\"radio\"><label><input type = \"radio\" name=\"pizzaVote\" value=\"down\">Down</label></div>");
                Console.WriteLine($"<input type=\"hidden\" name=\"pizzaid\" value=\"{pizza.Id}\" />");
                Console.WriteLine("<input type=\"submit\" class=\"btn btn-primary\" value=\"Vote\" />");
                Console.WriteLine("</form>");
                Console.WriteLine("</div>");
                Console.WriteLine("</div>");
            }
            Console.WriteLine("</div>");
        }


        private static void GenerateNavBar()
        {
            Console.WriteLine("<nav class=\"navbar navbar-default\">" +
                "<div class=\"container-fluid\">" +
                "<div class=\"navbar-header\">" +
                "<a class=\"navbar-brand\" href=\"Home.exe\">PizzaMore</a>" +
                "</div>" +
                "<div class=\"collapse navbar-collapse\" id=\"bs-example-navbar-collapse-1\">" +
                "<ul class=\"nav navbar-nav\">" +
                "<li ><a href=\"PizzaMore.AddPizza.exe\">Suggest Pizza</a></li>" +
                "<li><a href=\"PizzaMore.YourSuggestions.exe\">Your Suggestions</a></li>" +
                "</ul>" +
                "<ul class=\"nav navbar-nav navbar-right\">" +
                "<p class=\"navbar-text navbar-right\"></p>" +
                "<p class=\"navbar-text navbar-right\"><a href=\"Home.exe?logout=true\" class=\"navbar-link\">Sign Out</a></p>" +
                $"<p class=\"navbar-text navbar-right\">Signed in as {Session.User.Email}</p>" +
                "</ul> </div></div></nav>");

        }

        private static void ShowErrorPage(string path)
        {
            Header.Print();

            WebUtil.PrintFileContent(path);
        }
    }
}
