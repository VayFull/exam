using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class ExamModel
    {
        [Key]
        public long Id { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
    }

    public class ExamContext:DbContext
    {
        public ExamContext(DbContextOptions<ExamContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ExamModel> Exam { get; set; }
    }
}