
using System.Collections.Generic;
using System.Web.Mvc;
using RestSharp;
using System.Linq;
namespace ScrumwiseReporting.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            var restClient = new RestClient("https://api.scrumwise.com/service/api/v1");
            restClient.Authenticator = new HttpBasicAuthenticator("scrumwise@jamie-dixon.co.uk", "F7C66791D5181976523B27F8496D2D0D8AEF0DE03F6AA3A1557FD06BEA9ACFA5");
            var request = new RestRequest("getData");

            request.AddParameter("includeProperties", "Project.backlogItems,BacklogItem.tasks, Data.persons, Project.sprints, Task.timeEntries");
            var response = restClient.Execute<ReturnData>(request);
            var viewRespinse = restClient.Execute(request);

            var sprints = response.Data.Result.Projects.SelectMany(x => x.Sprints).ToList();

            foreach (var sprint in sprints)
            {
                sprint.BacklogItems =
                    response.Data.Result.Projects.SelectMany(x => x.BacklogItems)
                        .Where(x => x.SprintID == sprint.Id)
                        .ToList();
            }

            var vm = new HomepageViewModel
                     {
                         Raw = viewRespinse.Content,
                         Persons =  response.Data.Result.Persons,
                         Sprints = sprints
           
                     };

            return View("Index", vm);
        }
	}

    public class HomepageViewModel
    {
        public IEnumerable<Sprint> Sprints { get; set; }

        public string Raw { get; set; }

        public IEnumerable<Person> Persons { get; set; }
    }



    public class ReturnData
    {
        public Result Result { get; set; }
    }

    public class Result
    {
        public string ObjectType { get; set; }
        public List<Project> Projects { get; set;}
        public List<Person> Persons { get; set; } 
    }

    public class Project
    {
        public string Id { get; set; }
        public List<BacklogItem> BacklogItems { get; set; }
        public List<Sprint> Sprints { get; set; } 
    }

    public class BacklogItem
    {
        public string Id { get; set; }
        public string SprintID { get; set; }
        public string Name { get; set; }
        public List<Task>  Tasks { get; set; }
    }

    public class Person
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
    }

    public class Sprint
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<BacklogItem> BacklogItems { get; set; }
    }

    public class Task
    {
        public string Id { get; set; }
        public string BacklogItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Estimate { get; set; }
        public List<double> TimeEntries { get; set; } 
        public List<string> AssignedPersonIDs { get; set; }
    }

}