using Microsoft.EntityFrameworkCore;

namespace Radiant.Common.Database
{
    public class RadiantDbContext : DbContext 
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public RadiantDbContext()
        {
            this.Database.EnsureCreated();
        }
    }
}
