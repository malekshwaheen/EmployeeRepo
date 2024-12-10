using EmployeeProjectManagement.API.Entites;
using Microsoft.EntityFrameworkCore;
using System;

namespace EmployeeProjectManagement.API.Context
{
	public class EMPDbContext : DbContext
	{
		public EMPDbContext(DbContextOptions<EMPDbContext> options) : base(options)
		{

		}

		public DbSet<Employee> Employees { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<EmployeeProject> EmployeeProjects { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<EmployeeProject>()
				.HasKey(ep => new { ep.EmployeeId, ep.ProjectId });

			modelBuilder.Entity<EmployeeProject>()
				.HasOne(ep => ep.Employee)
				.WithMany(e => e.EmployeeProjects)
				.HasForeignKey(ep => ep.EmployeeId);

			modelBuilder.Entity<EmployeeProject>()
				.HasOne(ep => ep.Project)
				.WithMany(p => p.EmployeeProjects)
				.HasForeignKey(ep => ep.ProjectId);
		}
	}
}
