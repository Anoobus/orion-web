using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orion.web.BLL;
using orion.web.BLL.ArcFlashExpenditureExpenses;
using orion.web.BLL.Expenditures;
using orion.web.Employees;
using orion.web.Notifications;
using orion.web.UI.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace orion.web.UI.Controllers
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class ExpenseManagementController : Controller
    {
        private readonly IGetListOfExpenditures listExpendituresHandler;
        private readonly IDeleteExpenditure deleteExpenditure;
        private readonly IUpdateArcFlashLabelExpenditure updateArcFlashLabelExpenditure;
        private readonly IGetCreateExpenseModel getCreateExpenseModel;
        private readonly IUpdateMiscExpenditure updateMiscExpenditure;
        private readonly IUpdateContractorExpenditure updateContractorExpenditure;
        private readonly IUpdateTimeAndExpenseExpenditure updateTimeAndExpenseExpenditure;
        private readonly IUpdateCompanyVehicleExpenditure updateCompanyVehicleExpenditure;

        public ExpenseManagementController(IGetListOfExpenditures listExpendituresHandler,
            IDeleteExpenditure deleteExpenditure,
            IUpdateArcFlashLabelExpenditure updateArcFlashLabelExpenditure,
            IGetCreateExpenseModel getCreateExpenseModel,
            IUpdateMiscExpenditure updateMiscExpenditure,
            IUpdateContractorExpenditure updateContractorExpenditure,
            IUpdateTimeAndExpenseExpenditure updateTimeAndExpenseExpenditure,
            IUpdateCompanyVehicleExpenditure updateCompanyVehicleExpenditure)
        {
            this.listExpendituresHandler = listExpendituresHandler;
            this.deleteExpenditure = deleteExpenditure;
            this.updateArcFlashLabelExpenditure = updateArcFlashLabelExpenditure;
            this.getCreateExpenseModel = getCreateExpenseModel;
            this.updateMiscExpenditure = updateMiscExpenditure;
            this.updateContractorExpenditure = updateContractorExpenditure;
            this.updateTimeAndExpenseExpenditure = updateTimeAndExpenseExpenditure;
            this.updateCompanyVehicleExpenditure = updateCompanyVehicleExpenditure;
        }
        public async Task<IActionResult> Index()
        {
            var mdl = (await listExpendituresHandler.Process(new GetAllExpendituresRequest(includeArcFlashlabels: false, includeCompanyVehicles: false, includeMiscExpenditure: false, includeContractorExpenditures: false, includeTimeAndExpenseExpenditures: false))).Success;


            var vm = new ExpenseLandingPageModel()
            {
                AvailableEmployees = mdl.AvailableEmployees,
                AvailableJobs = mdl.AvailableJobs
            };
            return View(vm);
        }

        public async Task<IActionResult> ExpenseList()
        {
            return View("AllExpenseList",
                (await listExpendituresHandler.Process(new GetAllExpendituresRequest())).Success);
        }


        [Route("/expenses/{expense-id}/edit-modal")]
         public async Task<IActionResult> EditModal([FromRoute(Name = "expense-id")] Guid expenseId)
        {

            var mdl = (await listExpendituresHandler.Process(new GetAllExpendituresRequest(limitToSingleExpense: expenseId))).Success;
            var vm = new ExpenseViewModel()
            {
                ArcFlashLabelExpenditure = mdl.ArcFlashLabelExpenditures.FirstOrDefault(),
                MiscExpenditure = mdl.MiscExpenditures.FirstOrDefault(),
                CompanyVehicleExpenditure = mdl.CompanyVehicleExpenditures.FirstOrDefault(),
                ContractorExpenditure = mdl.ContractorExpenditures.FirstOrDefault(),
                TimeAndExpenceExpenditure = mdl.TimeAndExpenceExpenditures.FirstOrDefault(),
                IsBrandNewExpenditureCreation = false,
                AvailableJobs = mdl.AvailableJobs,
                AvailableEmployees = mdl.AvailableEmployees,              
            };
            if (vm.ArcFlashLabelExpenditure != null)
                vm.ExpenseType = ExpenditureTypeEnum.ArcFlashLabelExpenditure;
            if (vm.MiscExpenditure != null)
                vm.ExpenseType = ExpenditureTypeEnum.MiscExpenditure;
            if (vm.CompanyVehicleExpenditure != null)
                vm.ExpenseType = ExpenditureTypeEnum.CompanyVehicleExpenditure;
            if (vm.ContractorExpenditure != null)
                vm.ExpenseType = ExpenditureTypeEnum.ContractorExpenditure;
            if (vm.TimeAndExpenceExpenditure != null)
                vm.ExpenseType = ExpenditureTypeEnum.TimeAndExpenceExpenditure;

            return PartialView("ExpenseDetailModal", vm);
          
        }

        [Route("/expenses/{expense-type}")]
        public async Task<IActionResult> NewModal([FromRoute(Name = "expense-type")] string type)
        {

            var mdl = (await getCreateExpenseModel.Process(new GetCreateExpenseModelMessage(type))).Success;
          
            return PartialView("ExpenseDetailModal", mdl);          
        }

        [HttpPost]
         public async Task<IActionResult> SaveExpense([FromForm] ExpenseViewModel model)
        {
            var saved = false;
            switch (model.ExpenseType)
            {
                case ExpenditureTypeEnum.ArcFlashLabelExpenditure:
                    var afl = await updateArcFlashLabelExpenditure.Process(new UpdateArcFlashLabelExpenditureMessage(model.ArcFlashLabelExpenditure.Detail, model.ArcFlashLabelExpenditure.Detail.Id));
                    saved = afl.Success != null;
                    break;
                case ExpenditureTypeEnum.MiscExpenditure:
                    var misc = await updateMiscExpenditure.Process(new UpdateMiscExpenditureMessage(model.MiscExpenditure.Detail, model.MiscExpenditure.Detail.Id));
                    saved = misc.Success != null;
                    break;
                case ExpenditureTypeEnum.ContractorExpenditure:
                    var ce = await updateContractorExpenditure.Process(new UpdateContractorMessage(model.ContractorExpenditure.Detail, model.ContractorExpenditure.Detail.ExternalId));
                    saved = ce.Success != null;
                    break;
                case ExpenditureTypeEnum.TimeAndExpenceExpenditure:
                    var te = await updateTimeAndExpenseExpenditure.Process(new UpdateTimeAndExpenseExpenditureMessage(model.TimeAndExpenceExpenditure.Detail, model.TimeAndExpenceExpenditure.Detail.Id));
                    saved = te.Success != null;
                    break;
                case ExpenditureTypeEnum.CompanyVehicleExpenditure:
                    var cv = await updateCompanyVehicleExpenditure.Process(new UpdateCompanyVehicleExpenditureMessage(model.CompanyVehicleExpenditure.Detail, model.CompanyVehicleExpenditure.Detail.ExternalId));
                    saved = cv.Success != null;
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            if(saved)
                NotificationsController.AddNotification(this.User.SafeUserName(), $"Expenditure Saved");

            return RedirectToAction(nameof(ExpenseList));


        }

        [Route("/expenses/{expense-id}")]
         [HttpDelete()]
         public async Task<IActionResult> DeleteExpense([FromRoute(Name = "expense-id")] Guid expenseId)
        {
            var temp = await deleteExpenditure.Process(new DeleteExpenditureRequest(expenseId));
            if (temp.Success != null)
                NotificationsController.AddNotification(this.User.SafeUserName(), "Expenditure deleted");

            return temp.AsApiResult();          
        }
    }



}

