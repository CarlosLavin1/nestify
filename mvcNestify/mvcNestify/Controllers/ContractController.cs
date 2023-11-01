using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace mvcNestify.Controllers
{
    public class ContractController : Controller
    {
        // GET: ContractController1
        public ActionResult Index()
        {
            return View();
        }

        // GET: ContractController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ContractController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContractController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ContractController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ContractController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ContractController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ContractController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
