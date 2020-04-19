using System;
using System.Linq;
using LeChuck.Stateless.StateMachine;
using LeChuck.Stateless.StateMachine.Extensions;
using LeChuck.StateMachine;
using LeChuck.StateMachine.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleStateMachine.StateMachines;
using SampleStateMachine.StateMachines.LockStateMachine;
using SampleStateMachine.StateMachines.StepStateMachine;

namespace SampleStateMachine
{
    class Program
    {
        static void Main()
        {
            var configuration = GetConfiguration();
            var serviceProvider = GetServiceProvider();
            var machineFactory = serviceProvider.GetService<IStateMachineFactory>();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select which state machine you want to run.");
                Console.WriteLine("  1 - FormMachine (StateMachineDemo): Simple state machine with linear steps and a 'Cancel' command.");
                Console.WriteLine("  2 - Lock: Simple state machine to control a locking mechanism");
                Console.WriteLine("  0 - Exit app");

                IStateMachine machine = null;
                switch (Console.ReadKey().KeyChar)
                {
                    case '1':
                        machine = machineFactory.Create<IStepStateMachine>("stepMachine").GetAwaiter().GetResult();
                        RunStepMachine(machine, StepStateMachineWorkflow.States.Done, StepStateMachineWorkflow.States.Cancelled);
                        break;
                    case '2':
                        machine = machineFactory.Create<ILockStateMachine>("lockMachine").GetAwaiter().GetResult();
                        RunMachine(machine, LockStateMachineWorkflow.States.Finished);
                        break;
                    case '0':
                        return;
                }
            }
        }

        static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
        }

        static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddLogging(cfg => cfg.SetMinimumLevel(LogLevel.Information).AddConsole());

            services.AddStateMachines();
            // Register Our State Machine classes
            services.AddTransient<IStepStateMachine, StepStateMachine>();
            services.AddTransient<ILockStateMachine, LockStateMachine>();

            // Register the workflow configuration for the machines
            services.AddSingleton<StepStateMachineWorkflow>();
            services.AddSingleton<LockStateMachineWorkflow>();

            // Register a simple In Memory storage for the machines
            services.AddSingleton<IStateMachineStore, StateMachineInMemoryStore>();

            return services.BuildServiceProvider();

        }

        static void RunMachine(IStateMachine machine, params string[] finishStates)
        {
            while (!finishStates.Contains(machine.State))
            {
                Console.Clear();
                Console.WriteLine($"Running {machine.MachineType} instance {machine.MachineId}");
                Console.WriteLine();
                Console.WriteLine($"Current machine state: {machine.State}");

                int i = 0;
                var available = machine.GetAvailableCommands().ToArray();
                if (!available?.Any() ?? true)
                    throw new BadCommandException();
                var commands = string.Join("\n", available.Select(c => $"  {++i} - {c}."));
                Console.WriteLine($"\nAvailable Commands:\n{commands}");
                Console.Write("Press number: ");

                var key = $"{Console.ReadKey().KeyChar}";
                if (int.TryParse(key, out int option) && option <= available.Length)
                {
                    machine.ExecuteCommand(available[option - 1]);
                }
            }

            Console.Write("\nExiting machine... Press any key...");
            Console.ReadKey();
        }

        static void RunStepMachine(IStateMachine machine, params string[] finishStates)
        {
            Console.Clear();
            Console.WriteLine($"Running {machine.MachineType} instance {machine.MachineId}");
            Console.WriteLine();

            while (!finishStates.Contains(machine.State))
            {
                machine.ExecuteStep();
            }

            Console.Write("\nExiting machine... Press any key...");
            Console.ReadKey();
        }

    }

}
