using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOnGoL.Frontend
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();
        }
    }
}
