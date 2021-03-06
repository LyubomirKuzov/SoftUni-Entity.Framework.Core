﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using QuizSystem.ConsoleUI.Models;
using QuizSystem.Data;
using QuizSystem.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace QuizSystem.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var jsonService = serviceProvider.GetService<IJsonImportService>();
            jsonService.Import("EF-Core-Quiz.json", "EF Core Test v2");

            /*
            var dbContext = serviceProvider.GetService<ApplicationDbContext>();
            dbContext.Database.Migrate()
            */

            /*
            var quizService = serviceProvider.GetService<IQuizService>();
            quizService.Add("C# DB");
            */

            /*
            var questionService = serviceProvider.GetService<IQuestionService>();
            questionService.Add("What is EF Core?", 1);
            */

            /*
            var answerService = serviceProvider.GetService<IAnswerService>();
            answerService.Add("It is MicroORM", 0, false, 1);
            */

            /*
            var userAnswerService = serviceProvider.GetService<IUserAnswerService>();
            userAnswerService.AddUserAnswer("5ea964fc-4f78-4042-9bf5-bfca339a2d6a", 1, 1, 1);
            */

            /*
            var quizService = serviceProvider.GetService<IQuizService>();
            var quiz = quizService.GetQuizById(1);

            Console.WriteLine(quiz.Title);

            foreach (var question in quiz.Questions)
            {
                Console.WriteLine(question.Title);

                foreach (var answer in question.Answers)
                {
                    Console.WriteLine(answer.Title);
                }
            }
            */

            /*
            var quizService = serviceProvider.GetService<IUserAnswerService>();

            var quizPoints = quizService.UserResult("5ea964fc-4f78-4042-9bf5-bfca339a2d6a", 1);

            Console.WriteLine(quizPoints);
            */
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options =>
                options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IQuizService, QuizService>();
            services.AddTransient<IQuestionService, QuestionService>();
            services.AddTransient<IAnswerService, AnswerService>();
            services.AddTransient<IUserAnswerService, UserAnswerService>();
            services.AddTransient<IJsonImportService, JsonImportService>();
        }
    }
}
