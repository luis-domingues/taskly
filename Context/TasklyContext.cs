using Microsoft.EntityFrameworkCore;
using Taskly.Models;

namespace Taskly.Context;

public class TasklyContext : DbContext
{
    public TasklyContext(DbContextOptions<TasklyContext> options) : base(options) {  }

    public DbSet<User> Users { get; set; }
}