using Timer = System.Timers.Timer;

namespace ecommerce_API.Helpers
{
    public class PeriodicallyCaller
    {
        public static void Call(int timeDelay, Action function)
        {
            {
                Timer timer = new Timer(timeDelay);
                timer.Elapsed +=  (sender, e) => function();
                timer.Start();
            }

        }
    }
}
