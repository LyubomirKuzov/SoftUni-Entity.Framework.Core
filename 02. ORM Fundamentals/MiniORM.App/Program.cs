using MiniORM.App.Data;
using System;
using System.Linq;

namespace MiniORM.App
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MiniORM;Integrated Security=true";

            var context = new SoftUniDbContextClass(connectionString);

            context.Employees.Add(new Data.Entities.Employee
            {
                FirstName = "Gosho",
                LastName = "Inserted",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true
            });

            var employee = context.Employees.Last();

            employee.FirstName = "Modified";

            context.SaveChanges();
        }
    }
}
