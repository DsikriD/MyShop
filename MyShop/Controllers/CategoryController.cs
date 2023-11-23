using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop_DataAccess;
using MyShop_DataAccess.Repository.IRepository;
using MyShop_Models;
using MyShop_Utility;

namespace MyShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {

        private readonly ICategoryRepository _catRepo;

        public CategoryController(ICategoryRepository catRepo)
        {
            _catRepo = catRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _catRepo.GetAll();
            return View(objList);
        }

        //Get - create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Защита от взлома
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Add(obj);//Добвление в бд
                _catRepo.Save();// Сохранение в бд - только после этого категория попадет в бд
                TempData[WC.Success] = "Категория добавлена";
                return RedirectToAction("Index");
            }
            TempData[WC.Error] = "Не получилось создать категорию";
            return View(obj);
        }

        //Get - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _catRepo.Find(id.GetValueOrDefault());

            if (obj == null)
                return NotFound();

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Защита от взлома
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Update(obj);//Редактирование в бд
                _catRepo.Save();// Сохранение в бд - только после этого категория попадет в бд
                TempData[WC.Success] = "Категория отредактирована";
                return RedirectToAction("Index");
            }
            TempData[WC.Error] = "Не получилось отредактировать категорию";
            return View(obj);
        }


        //Get - EDIT
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _catRepo.Find(id.GetValueOrDefault());

            if (obj == null)
                return NotFound();

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Защита от взлома
        public IActionResult DeletePost(int? id)
        {

            var obj = _catRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                TempData[WC.Error] = "Не получилось удалить категорию";
                return NotFound();
            }
            _catRepo.Remove(obj);//Удаление в бд
            _catRepo.Save();// Сохранение в бд - только после этого категория попадет в бд
            TempData[WC.Success] = "Категория успешно удалена";
            return RedirectToAction("Index");

        }

    }
}

