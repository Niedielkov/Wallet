using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wallet.Data;
using Wallet.Models;

namespace Wallet.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions.Include(t => t.Category).ToArrayAsync();
            return View(transactions);
        }

        public IActionResult AddIncome(bool isCreateRepetitive)
        {
            PopulateCategories("Income");
            return View(new Transaction
            {
                IsRepeatitive = isCreateRepetitive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIncome([Bind("TransactionId,CategoryId,Amount,Note,Date,RepeatDay")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateCategories("Income");
            return View(transaction);
        }

        public IActionResult AddExpense(bool isCreateRepetitive) 
        {
            PopulateCategories("Expense");
            return View(new Transaction
            {
                IsRepeatitive = isCreateRepetitive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpense([Bind("TransactionId,CategoryId,Amount,Note,Date,RepeatDay")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateCategories("Expense");
            return View(transaction);
        }

        public IActionResult Edit(int id)
        {
            PopulateCategories("All");
            return View(_context.Transactions.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("TransactionId,CategoryId,Amount,Note,Date,RepeatDay")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Update(transaction);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateCategories("All");
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }

            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategories(string transactionsType)
        {
            var CategoryCollection = new List<Category>();

            if (transactionsType == "Income")
                CategoryCollection = _context.Categories.Where(c => c.Type.Equals(transactionsType)).ToList();
            else if (transactionsType == "Expense")
                CategoryCollection = _context.Categories.Where(c => c.Type.Equals(transactionsType)).ToList();
            else
                CategoryCollection = _context.Categories.ToList();
            
            Category DefaultCategory = new Category { CategoryId = 0, Title = "Choose a category" };
            CategoryCollection.Insert(0, DefaultCategory);
            ViewBag.Categories = CategoryCollection;
        }

        //Called from Task
        public void CheckDailyForRepetitions()
        {
            var repetitiveTransactions = _context.Transactions.Where(t => t.RepeatDay == DateTime.Now.Day).ToList();

            if (repetitiveTransactions.Count > 0)
            {
                var newTransactions = new List<Transaction>();

                foreach (var transaction in repetitiveTransactions)
                {
                    Transaction newTransaction = new Transaction()
                    {
                        CategoryId = transaction.CategoryId,
                        Amount = transaction.Amount,
                        Note = transaction.Note
                    };

                    newTransactions.Add(newTransaction);
                }

                _context.UpdateRange(newTransactions);
                _context.SaveChanges();
            }
        }
    }
}
