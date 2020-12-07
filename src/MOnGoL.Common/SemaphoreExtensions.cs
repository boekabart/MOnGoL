using System;
using System.Threading;
using System.Threading.Tasks;

namespace MOnGoL.Common
{
    public static class SemaphoreExtensions
    {
        public static async Task<IDisposable> DisposableEnter(this SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return new ReleaseDisposable(semaphore);
        }

        private class ReleaseDisposable : IDisposable
        {
            private SemaphoreSlim enteredSemaphore;

            public ReleaseDisposable(SemaphoreSlim enteredSemaphore)
            {
                this.enteredSemaphore = enteredSemaphore;
            }
            public void Dispose()
            {
                enteredSemaphore?.Release();
                enteredSemaphore = null;
            }
        }
    }
}
