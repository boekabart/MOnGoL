using System;
using System.Threading.Tasks;

namespace MOnGoL.Common
{
    public interface ICounterService
    {
        Task Increment(int byHowMuch);
        EventHandler<int> OnNewValue { get; set; }
    }
}

