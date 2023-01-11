using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Wallet.Controllers;
using Wallet.Data;

namespace Wallet
{
    public class Task
    {
        //Task registry 
        public class ScheduleRegistry : Registry
        {
            public ScheduleRegistry()
            {
                // Schedule a simple job to run at a specific time -> runs everday at 12:00
                Schedule(() => new Job()).ToRunEvery(0).Days().At(12, 00);
            }
        }

        //Task Job
        public class Job : IJob
        {
            //Calls method from TransactionController -> which checks for repeat day of transactions and if its today adds new Transaction
            public void Execute()
            {
                var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                                         .UseSqlServer(@"Server=(local)\\sqlexpress;Database=Wallet;Trusted_Connection=True;MultipleActiveResultSets=True;")
                                         .Options;

                using var context = new ApplicationDbContext(contextOptions);

                TransactionController tc = new TransactionController(context);
                tc.CheckDailyForRepetitions();
            }
        }
    }
}
