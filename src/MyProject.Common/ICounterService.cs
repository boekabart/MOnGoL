using System;
using System.Threading.Tasks;

namespace MyProject.Common
{
    public interface ICounterService
    {
        Task Increment(int byHowMuch);
        EventHandler<int> OnNewValue { get; set; }
    }
}

