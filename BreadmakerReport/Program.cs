using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using RatingAdjustment.Services;
using BreadmakerReport.Models;
using System.Collections.Generic;

namespace BreadmakerReport
{
    class Program
    {
        static string dbfile = @".\data\breadmakers.db";
        static RatingAdjustmentService ratingAdjustmentService = new RatingAdjustmentService();

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Bread World");
            var BreadmakerDb = new BreadMakerSqliteContext(dbfile);
            var BMList = BreadmakerDb.Breadmakers;

            var Alldata = (from data in BMList
                           select new
                           {
                               Title = data.title,
                               Reviews = (Double)data.Reviews.Count(),
                               Average = (Double)BreadmakerDb.Reviews.
                                   Where(row => row.BreadmakerId == data.BreadmakerId)
                                   .Select(row => row.stars).Sum() / data.Reviews.Count(),
                               Adjust = ratingAdjustmentService.Adjust(
                                        (Double)BreadmakerDb.Reviews
                                        .Where(row => row.BreadmakerId == data.BreadmakerId)
                                        .Select(row => row.stars).Sum() / data.Reviews.Count()
                                         ,
                                        (Double)data.Reviews.Count())
                           }).ToList().OrderByDescending(r => r.Adjust).ToList();
            // TODO: add LINQ logic ...
            //       ...
            //.ToList();

            Console.WriteLine("[#]  Reviews Average  Adjust    Description");
            for (var j = 0; j < 3; j++)
            {
                var i = Alldata[j];
                // TODO: add output
                // Console.WriteLine( ... );
                Console.WriteLine($"[{j}] {i.Reviews, 7} {Math.Round(i.Average,2), -7} {Math.Round(i.Adjust, 2), -6} {i.Title}");
            }
        }
    }
}
