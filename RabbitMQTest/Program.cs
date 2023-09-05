using RabbitMQ.Client;
using RabbitMQTest;
using System;
using System.Text;
using System.Threading.Channels;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Please Enter HostName?");
        var HostName = Console.ReadLine();
        Console.WriteLine("Please Enter Port?");
        var Port = Console.ReadLine();
        Console.WriteLine("Please Enter UserName?");
        var UserName = Console.ReadLine();
        Console.WriteLine("Please Enter Password?");
        var Password = Console.ReadLine();
        Console.WriteLine("Please Enter Number Of Queue?");
        var numberOfQueue = Console.ReadLine();
        Console.WriteLine("Please Enter App Run Mode ,Prosedure:1,CunsumerMode:2 ,Both:3?");
        string Mode = Console.ReadLine();
        Mode = string.IsNullOrWhiteSpace(Mode) ? "3" : Mode;
        var queu = "Abed_Queues";
        Console.WriteLine("Please Enter Numberof Worker");
        string Worker = Console.ReadLine();
        int worker = string.IsNullOrWhiteSpace(Worker) ? 8 : Convert.ToInt32( Worker);
        // Create a connection factory
        var factory = new ConnectionFactory()
        {
            HostName = string.IsNullOrWhiteSpace(HostName) ? "localhost" : HostName, // RabbitMQ server hostname
            UserName = string.IsNullOrWhiteSpace(UserName) ? "guest" : UserName,     // RabbitMQ username
            Password = string.IsNullOrWhiteSpace(Password) ? "guest" : Password,    // RabbitMQ password
            Port = string.IsNullOrWhiteSpace(Port) ? 5672 : Convert.ToInt32(Port)


        };
        int number = string.IsNullOrWhiteSpace(numberOfQueue) ? 1 : Convert.ToInt32(numberOfQueue);

        RabitLoadTest rabitLoad = new RabitLoadTest(factory);
        MessageConsumer messageConsumer = new MessageConsumer(factory);
        
        if ( Mode == "1")
            await rabitLoad.load(queu, number, worker);
        else if (Mode == "2") await messageConsumer.main(queu, number, worker);
        else if (Mode == "3")
        {
            Task[] tasks = { rabitLoad.load(queu, number, worker), messageConsumer.main(queu, number, worker) };
            Task.WaitAll(tasks);
        }



        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();
    }


}