using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using LeChuck.Stateless.StateMachine;
using LeChuck.StateMachine;

namespace SampleStateMachine.StateMachines.StepStateMachine
{

    public interface IStepStateMachine : IStateMachine { }

    public class StepStateMachine : StateMachine, IStepStateMachine
    {
        private StepModel _model { get; set; } = new StepModel();

        public StepStateMachine(StepStateMachineWorkflow workflow, IStateMachineStore store) : base(workflow, store)
        { }

        // Persist custom stuff other than state
        public override string SerializeData()
        {
            var dict = JsonSerializer.Deserialize<IDictionary<string, object>>(base.SerializeData());
            dict.Add(nameof(_model), _model);
            return JsonSerializer.Serialize(dict);
        }

        public override void DeserializeData(string data)
        {
            base.DeserializeData(data);

            var dict = JsonSerializer.Deserialize<IDictionary<string, object>>(data);
            _model = (dict.ContainsKey(nameof(_model))) ? (StepModel) dict[nameof(_model)] : null;
        }

        protected override async Task<bool> OnCommand(string command, string payload = null)
        {
            // Only cancel command works here... 
            Console.WriteLine("The machine has been cancelled!");
            return await Task.FromResult(true);
        }

        protected override async Task OnNewState(string newState)
        {
            // Be nice and don't switch use a Selector pattern here!! this is just a demo
            switch (newState)
            {
                case StepStateMachineWorkflow.States.InputName:
                    Console.WriteLine("Type your name and press enter:");
                    break;

                case StepStateMachineWorkflow.States.InputEmail:
                    Console.WriteLine("Type your email and press enter:");
                    break;

                case StepStateMachineWorkflow.States.Confirmation:
                    Console.Write("Data is correct? (Y/N) ");
                    break;
                case StepStateMachineWorkflow.States.Done:
                    // It's done .. let's save
                    Console.WriteLine($"Done:\n{JsonSerializer.Serialize(_model, new JsonSerializerOptions{WriteIndented = true})}");
                    DummySaveEntity(_model);
                    break;
                default:
                    Console.WriteLine("Invalid state!");
                    break;
            }

            await Task.CompletedTask;
        }

        private void DummySaveEntity(StepModel model)
        {
            // do nothing just for demo purpose
        }

        protected override async Task<bool> OnNextStep(string currentState, string payload)
        {
            // Be nice and don't switch use a Selector pattern here!! this is just a demo
            switch (currentState)
            {
                case StepStateMachineWorkflow.States.InputName:
                    _model.Name = Console.ReadLine();
                    // if you need validation here is the place and return false
                    break;

                case StepStateMachineWorkflow.States.InputEmail:
                    _model.Email = Console.ReadLine();
                    // if you need validation here is the place and return false
                    break;

                case StepStateMachineWorkflow.States.Confirmation:
                    var response = $"{Console.ReadKey().KeyChar}".ToLower();
                    Console.WriteLine();
                    if (response == "y")
                        return await Task.FromResult(response == "y");

                    // Something was wrong... starting over
                    await SetNewState(StepStateMachineWorkflow.States.InputName);
                    return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }
    }
}
