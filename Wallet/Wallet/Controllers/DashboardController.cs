using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Wallet.Data;
using Wallet.Models;

namespace Wallet.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Transaction> transactions = await _context.Transactions.Include(t => t.Category).ToListAsync();

            //Balance

            int totalIncome = transactions.Where(t => t.Category.Type.Equals("Income")).Sum(t => t.Amount);
            ViewBag.TotalIncome = FormatTotalValue(totalIncome);

            int totalExpense = transactions.Where(t => t.Category.Type.Equals("Expense")).Sum(t => t.Amount);
            ViewBag.TotalExpense = FormatTotalValue(totalExpense);

            int totalBalance = totalIncome - totalExpense;
            ViewBag.TotalBalance = FormatTotalValue(totalBalance);

            if(transactions.Count > 0)
            {
                //Spline Chart - Income vs Expense

                List<SplineChartData> Income = transactions.Where(t => t.Category.Type.Equals("Income"))
                                                           .GroupBy(t => t.Date)
                                                           .Select(s => new SplineChartData
                                                           {
                                                               day = s.First().Date.ToString("dd-MMM"),
                                                               income = s.Sum(l => l.Amount)
                                                           }).ToList();

                List<SplineChartData> Expense = transactions.Where(t => t.Category.Type.Equals("Expense"))
                                                           .GroupBy(t => t.Date)
                                                           .Select(s => new SplineChartData
                                                           {
                                                               day = s.First().Date.ToString("dd-MMM"),
                                                               expense = s.Sum(l => l.Amount)
                                                           }).ToList();

                Transaction FirstTransaction = transactions.OrderBy(t => t.Date).FirstOrDefault();
                DateTime StartDate = FirstTransaction.Date;
                DateTime EndDate = DateTime.Now;

                string[] Days = Enumerable.Range(0, (int)(EndDate - StartDate).TotalDays)
                    .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                    .ToArray();

                ViewBag.SplineChartData = from day in Days
                                          join income in Income on day equals income.day into dayIncomeJoined
                                          from income in dayIncomeJoined.DefaultIfEmpty()
                                          join expense in Expense on day equals expense.day into dayExpenseJoined
                                          from expense in dayExpenseJoined.DefaultIfEmpty()
                                          select new
                                          {
                                              day = day,
                                              income = income == null ? 0 : income.income,
                                              expense = expense == null ? 0 : expense.expense
                                          };
            }

            return View();
        }

        private string FormatTotalValue(int data)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            return String.Format(culture, "{0:C0}", data);
        }
    }

    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;
    }
}
