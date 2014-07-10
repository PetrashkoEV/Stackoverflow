using System.Web.Mvc;
using System.Web.Routing;

namespace Stackoverflow
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "ProblemPaging",
                "Page/{page}",
                new {controller = "Problems", action = "Index"}
                );

            routes.MapRoute(
                "AddQuestionsPage",
                "Questions/Add/",
                new { controller = "Problems", action = "Add" }
            );

            routes.MapRoute(
                "EditQuestionsPage",
                "Questions/Edit/{id}",
                new { controller = "Problems", action = "Edit" }
            );

            routes.MapRoute(
                "DeleteQuestionsPage",
                "Questions/Delete/{id}",
                new { controller = "Problems", action = "Delete" }
            );

            routes.MapRoute(
                "QuestionsPage",
                "Questions/{id}",
                new { controller = "Question", action = "Index" }
            );

            routes.MapRoute(
                "EditAnswerPage",
                "Answer/Edit/{id}",
                new { controller = "Question", action = "EditAnswer" }
            );

            routes.MapRoute(
                "DeleteAnswerPage",
                "Answer/Delete/{id}",
                new { controller = "Question", action = "DeleteAnswer" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{page}",
                defaults: new { controller = "Problems", action = "Index", page = 1 }
            );

            
        }
    }
}