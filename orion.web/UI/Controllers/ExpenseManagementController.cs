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

        public ExpenseManagementController(IGetListOfExpenditures listExpendituresHandler,
            IDeleteExpenditure deleteExpenditure,
            IUpdateArcFlashLabelExpenditure updateArcFlashLabelExpenditure)
        {
            this.listExpendituresHandler = listExpendituresHandler;
            this.deleteExpenditure = deleteExpenditure;
            this.updateArcFlashLabelExpenditure = updateArcFlashLabelExpenditure;
        }
        public IActionResult Index()
        {
            return View();
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
            //var exp = mdl.SuccessResult.
            return PartialView("ExpenseDetailModal", new EditExpenseModel()
            {
                ArcFlashLabelExpenditure = mdl.ArcFlashLabelExpenditures.First(),
                AvailableJobs = mdl.AvailableJobs,
                AvailableEmployees = mdl.AvailableEmployees,
                ExpenseType = ExpenditureTypeEnum.ArcFlashLabelExpenditure,
            });
          
        }

        [HttpPost]
         public async Task<IActionResult> UpdateExpense([FromForm] EditExpenseModel model)
        {

            var res = await updateArcFlashLabelExpenditure.Process(new UpdateArcFlashLabelExpenditureMessage(model.ArcFlashLabelExpenditure.Detail, model.ArcFlashLabelExpenditure.Detail.Id));
            if(res.Success != null)
                NotificationsController.AddNotification(this.User.SafeUserName(), $"Arc Flash Label Expenditure Updated");

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

