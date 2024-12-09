using Fall2024_Assignment3_separal.Data;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();




}
}
