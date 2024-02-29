///For examples, see:
///https://thegraybook.vvvv.org/reference/extending/writing-nodes.html#examples

namespace TemporalSpace;
using System;
using System.Timers;
public static class LoopTimer
{


    static void Main(string[] args, float t)
    {
        // Create a timer with a 1 second interval
        Timer timer = new Timer(t);

        // Hook up the Elapsed event for the timer
        timer.Elapsed += TimerElapsed;

        // Start the timer
        timer.Start();

        // Wait for the user to press Enter before exiting
        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
    }

    static void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        // This method will be called every time the timer elapses
        Console.WriteLine("Timer elapsed at: " + e.SignalTime);
    }
}
