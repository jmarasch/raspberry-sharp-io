﻿#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Raspberry.IO.Components.Sensors.HcSr04;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

#endregion

namespace Test.Gpio.HCSR04
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.CursorVisible = false;

            const ConnectorPin triggerPin = ConnectorPin.P1Pin21;
            const ConnectorPin echoPin = ConnectorPin.P1Pin23;

            Console.WriteLine("HC-SR04 Sample: measure distance");
            Console.WriteLine();
            Console.WriteLine("\tTrigger: {0}", triggerPin);
            Console.WriteLine("\tEcho: {0}", echoPin);
            Console.WriteLine();

            var interval = GetInterval(args);

            var driver = GpioConnectionSettings.DefaultDriver;
            using (var connection = new HcSr04Connection(driver.Out(triggerPin), driver.In(echoPin)))
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        var distance = connection.GetDistance();
                        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0.0}cm", distance * 100).PadRight(16));
                        Console.CursorTop--;
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine("(Timeout): " + e.Message);
                    }

                    Timer.Sleep(interval);
                }
            }

            Console.CursorVisible = true;
        }

        #region Private Helpers

        private static int GetInterval(IEnumerable<string> args)
        {
            return args
                .SkipWhile(a => a != "-interval")
                .Skip(1)
                .Select(int.Parse)
                .DefaultIfEmpty(100)
                .First();
        }

        #endregion
    }
}
