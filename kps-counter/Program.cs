using System;
using System.Linq;
using System.IO.Ports;

namespace kps_counter
{
    /// <summary>
    /// Bridges connection between DOSBox memory for Wing Commander 1 and Arduino 7-segment 4-digit display
    /// </summary>
    class Program
    {
        /// <summary>
        /// serial connection to the Arduino
        /// </summary>
        static SerialPort port;

        static void Main(string[] args)
        {
            int baud = 0;
            string name;
            Console.WriteLine("Welcome, enter parameters to begin");
            Console.WriteLine();
            Console.WriteLine("Available Ports:");
            if (SerialPort.GetPortNames().Count() > 0)
            {
                foreach (string portname in SerialPort.GetPortNames())
                {
                    Console.WriteLine("  " + portname);
                }
            } else
            {
                Console.WriteLine("No ports available! Press any key to exit.");
                Console.ReadLine();
                return;
            }

            Console.Write("Port Name [Press Enter for COM3 default]: ");
            name = Console.ReadLine();
            if (name == "") name = "COM3";

            Console.WriteLine();
            Console.Write("Baud Rate [Press Enter for 9600 default]: ");
            try
            {
                baud = int.Parse(Console.ReadLine());
            } catch
            {
                // default to 9600, as set in arduino sketch
                baud = 9600;
            }

            try
            {
                Console.WriteLine();
                Console.WriteLine("Opening serial connection to Arduino...");
                port = new SerialPort(name, baud);
                port.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);
                port.Open();
                Console.WriteLine("Serial connection with Arduino started.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught while opening serial connection! Aborting...");
                Console.WriteLine(e);
                Console.ReadLine();
                return;
            }


            WingCommanderMemoryReader reader = new WingCommanderMemoryReader();

            try
            {
                Console.WriteLine();
                Console.WriteLine("Attaching to DOSBox.exe process...");
                reader.hook();
                Console.WriteLine("Successfully attached!");
            } catch (DOSBoxMemoryException e)
            {
                Console.WriteLine("Exception caught while attaching to DOSBox! Aborting...");
                Console.WriteLine(e);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("");
            Console.WriteLine("Now communicating between DOSBox and Arduino.");
            Console.WriteLine("Press Ctrl-C to exit program");
            Console.WriteLine("");

            // loop until user aborts program
            while (true)
            {
                // fetch from memory and write to the serial port
                int kps = reader.GetSetKPS();
                port.WriteLine(kps.ToString());
            }
        }

        /// <summary>
        /// Event handler for Arduino serial input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // HACK - pause for a bit to let the serial connection catch up
            for (int i = 0; i < (10000 * port.BytesToRead) / port.BaudRate; i++) ;
            // output to display
            Console.WriteLine(port.ReadExisting());
            Console.WriteLine("");
        }
    }
}
