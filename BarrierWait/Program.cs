using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Program
{
    public static class Program
    {
        static void BarrierSample()
        {
            var passedCount=0;
            var barrier = new Barrier(3, (b) => {
                Console.WriteLine($"Post-Phase action: passedCount={passedCount}, phase={b.CurrentPhaseNumber}");
            });
            barrier.AddParticipants(2);
            barrier.RemoveParticipant();
            var rand=new Random();

            Action<int> singleaction=(indexI) =>
            {
                for (int i = 0; i < 3; i++)
                {
                    Interlocked.Increment(ref passedCount);
                    var sleepTime = rand.Next(3, 10);
                    Console.WriteLine($"Thread {indexI} sleeps for {sleepTime}s");
                    Thread.Sleep(sleepTime * 1000);
                    Console.WriteLine($"Thread {indexI} have reached the barrier.");
                    barrier.SignalAndWait();
                }
            };
            //Must have 4 actions to be executed because count of participants is 4.
            Stopwatch sw = Stopwatch.StartNew();
            Parallel.For(0,barrier.ParticipantsRemaining,singleaction);
            Console.WriteLine($"Total elapsed time={sw.ElapsedMilliseconds/1000.0}s");
            barrier.Dispose();
        }
        public static int Main()
        {
            BarrierSample();
            return 0;
        }
    }
}