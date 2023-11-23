using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop_DataAccess;
using MyShop_DataAccess.Repository;
using MyShop_DataAccess.Repository.IRepository;
using MyShop_Models;
using MyShop_Utility;

namespace MyShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {

        private readonly IApplicationTypeRepository _appTypeRepo;


        public ApplicationTypeController(IApplicationTypeRepository appTypeRepo) {
            _appTypeRepo = appTypeRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _appTypeRepo.GetAll();
            return View(objList);
        }

        //Get - create
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Защита от взлома
        public IActionResult Create(ApplicationType  obj)
        {
            if (ModelState.IsValid)
            {
                _appTypeRepo.Add(obj);//Добвление в бд
                _appTypeRepo.Save();// Сохранение в бд - только после этого категория попадет в бд
                TempData[WC.Success] = "Тип добавлен";
                return RedirectToAction("Index");
            }
            TempData[WC.Error] = "Не получилось добавить тип";
            return View(obj);
        }

        //Get - EDIT
        public IActionResult Edit(int? id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }

            var obj = _appTypeRepo.Find(id.GetValueOrDefault());

            if( obj == null )
                return NotFound();

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Защита от взлома
        public IActionResult Edit(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _appTypeRepo.Update(obj);//Редактирование в бд
                _appTypeRepo.Save();// Сохранение в бд - только после этого категория попадет в бд
                TempData[WC.Success] = "Тип отредактирован";
                return RedirectToAction("Index");
            }
            TempData[WC.Error] = "Не получилось отредактировать тип";
            return View(obj);
        }


        //Get - EDIT
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _appTypeRepo.Find(id.GetValueOrDefault());

            if (obj == null)
                return NotFound();

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Защита от взлома
        public IActionResult DeletePost(int? id)
        {
            
                var obj = _appTypeRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                TempData[WC.Success] = "Не получилось удалить тип";
                return NotFound();
            }
                _appTypeRepo.Remove(obj);//Удаление в бд
                _appTypeRepo.Save();// Сохранение в бд - только после этого категория попадет в бд
                TempData[WC.Success] = "Тип успешно удален";
                return RedirectToAction("Index");
 
        }

    }
}

