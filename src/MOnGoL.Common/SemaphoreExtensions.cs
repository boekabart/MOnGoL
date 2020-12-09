using System;
using System.Threading;
using System.Threading.Tasks;

namespace MOnGoL.Common
{
    public static class SemaphoreExtensions
    {
        public static async Task<IDisposable> DisposableEnter(this SemaphoreSlim semaphore, string? location = null)
        {
            await semaphore.WaitAsync();
            return new ReleaseDisposable(semaphore, location);
        }

        private class ReleaseDisposable : IDisposable
        {
            private SemaphoreSlim enteredSemaphore;
            private readonly string? location;
            //private static int enters;

            public ReleaseDisposable(SemaphoreSlim enteredSemaphore, string? location)
            {
                this.enteredSemaphore = enteredSemaphore;
                this.location = location;
                //var active = Interlocked.Increment(ref enters);
                //Console.WriteLine($"+{location} => {active}");
            }

            public void Dispose()
            {
                enteredSemaphore?.Release();
                enteredSemaphore = null;
                //var active = Interlocked.Decrement(ref enters);
                //Console.WriteLine($"-{location} => {active}");
            }
        }
    }
}
