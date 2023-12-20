using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 20
    /// </summary>
    public class Day20
    {
        public long Part1(string[] input)
        {
            var graph = ModuleGraph.Parse(input);

            for (int i = 0; i < 1000; i++)
            {
                graph.PushButton();
            }

            return graph.Score;
        }

        public long Part2(string[] input)
        {
            var graph = ModuleGraph.Parse(input);
            return graph.MinimumButtonPushesToOutputLowPulse();
        }

        /// <summary>
        /// Graph of modules
        /// </summary>
        private record ModuleGraph(IDictionary<string, Module> Modules)
        {
            private long highSent;
            private long lowSent;

            /// <summary>
            /// Score of the graph,which is number of high pulses times number of low pulses sent so far
            /// </summary>
            public long Score => this.highSent * this.lowSent;

            /// <summary>
            /// Parse the input to a graph of modules
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Module graph</returns>
            public static ModuleGraph Parse(IReadOnlyList<string> input)
            {
                var modules = input.Select(Module.Parse).Append(new OutputModule("rx")).ToDictionary(m => m.Id);

                foreach (Module module in modules.Values)
                {
                    foreach (string output in module.Outputs)
                    {
                        modules[output].AddInput(module.Id);
                    }
                }

                return new ModuleGraph(modules);
            }

            /// <summary>
            /// Push the button which starts sending pulses around all modules and returns when no more pulses are sent
            /// </summary>
            public void PushButton()
            {
                Queue<(string Sender, string Destination, Pulse Pulse)> queue = new();
                queue.Enqueue(("button", "broadcast", Pulse.Low));

                while (queue.Count > 0)
                {
                    (string sender, string destination, Pulse pulse) = queue.Dequeue();

                    if (pulse == Pulse.Low)
                    {
                        this.lowSent++;
                    }
                    else
                    {
                        this.highSent++;
                    }

                    Module module = this.Modules[destination];

                    foreach ((string Destination, Pulse Pulse) next in module.Process(sender, pulse))
                    {
                        queue.Enqueue((module.Id, next.Destination, next.Pulse));
                    }
                }
            }

            /// <summary>
            /// Calculate how many pushes of the button it would take for the output module to receive a low pulse
            /// </summary>
            /// <returns></returns>
            public long MinimumButtonPushesToOutputLowPulse()
            {
                /*
                 See inputs/day20_graphviz.dot, which you can visualise here: https://dreampuf.github.io/GraphvizOnline

                 These 4 modules are 'special' - they all need to get a low pulse at exactly the same time, and each
                 receives a pulse once a big cycle is complete. That means the answer is the LCM of all their periods.
                
                 We could find these programmatically for anyone's input assuming we all got basically the same thing just
                 with different labels (i.e. an rx node with a single conjunction node as its input, which itself has 4
                 conjunction nodes as inputs which are the terminating points of sub-cycles).

                 In my input this is:

                    fk  rz       lf  br
                     |   |       |   |
                     |   ---v v---   |
                     -----> lb <------
                             |
                             v
                            rx
                */
                Dictionary<string, long> specialModules = new Dictionary<string, long>
                {
                    ["fk"] = -1,
                    ["rz"] = -1,
                    ["lf"] = -1,
                    ["br"] = -1,
                };

                long buttonPushes = 0;

                while (true)
                {
                    buttonPushes++;

                    Queue<(string Sender, string Destination, Pulse Pulse)> queue = new();
                    queue.Enqueue(("button", "broadcast", Pulse.Low));

                    while (queue.Count > 0)
                    {
                        (string sender, string destination, Pulse pulse) = queue.Dequeue();

                        if (pulse == Pulse.Low && specialModules.TryGetValue(destination, out long cycle) && cycle == -1)
                        {
                            specialModules[destination] = buttonPushes;

                            if (specialModules.Values.All(v => v != -1))
                            {
                                return specialModules.Values.Aggregate((acc, v) => acc.LowestCommonMultiple(v));
                            }
                        }

                        Module module = this.Modules[destination];

                        foreach ((string Destination, Pulse Pulse) next in module.Process(sender, pulse))
                        {
                            queue.Enqueue((module.Id, next.Destination, next.Pulse));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A module in the graph
        /// </summary>
        private abstract class Module
        {
            /// <summary>
            /// Module ID
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// Module inputs
            /// </summary>
            public IList<string> Inputs { get; }

            /// <summary>
            /// Module outputs
            /// </summary>
            public IReadOnlyList<string> Outputs { get; }

            /// <summary>
            /// Initialises a new instance of the <see cref="Module"/> class.
            /// </summary>
            /// <param name="id">Module ID</param>
            /// <param name="outputs">Module outputs</param>
            protected Module(string id, IReadOnlyList<string> outputs)
            {
                this.Id = id;
                this.Inputs = new List<string>();
                this.Outputs = outputs;
            }

            /// <summary>
            /// Parse a module from the given input line
            /// </summary>
            /// <param name="input">Input</param>
            /// <returns>Module</returns>
            public static Module Parse(string input)
            {
                var parts = input.Split(" -> ");
                var outputs = parts[1].Split(", ");

                if (input.StartsWith('%'))
                {
                    string id = parts[0][1..];
                    return new FlipFlopModule(id, outputs);
                }

                if (input.StartsWith('&'))
                {
                    string id = parts[0][1..];
                    return new ConjunctionModule(id, outputs);
                }

                return new BroadcastModule("broadcast", outputs);
            }

            /// <summary>
            /// Add an input to this module
            /// </summary>
            /// <param name="input"></param>
            public virtual void AddInput(string input)
            {
                this.Inputs.Add(input);
            }

            /// <summary>
            /// Process a pulse
            /// </summary>
            /// <param name="input">Input module that generate the pulse</param>
            /// <param name="pulse">Pulse type</param>
            /// <returns>Pulses to send next (which may be none)</returns>
            public abstract IEnumerable<(string Destination, Pulse Pulse)> Process(string input, Pulse pulse);
        }

        /// <summary>
        /// Pulse type
        /// </summary>
        private enum Pulse
        {
            High,
            Low
        }

        /// <summary>
        /// Flip-flop module, which flips state when receiving a low pulse and sends the opposite pulse to all outputs
        /// </summary>
        private class FlipFlopModule : Module
        {
            private bool active = false;

            /// <summary>
            /// Initialises a new instance of the <see cref="FlipFlopModule"/> class.
            /// </summary>
            public FlipFlopModule(string id, string[] outputs) : base(id, outputs)
            {
            }

            /// <summary>
            /// Process a pulse
            /// </summary>
            /// <param name="input">Input module that generate the pulse</param>
            /// <param name="pulse">Pulse type</param>
            /// <returns>Pulses to send next (which may be none)</returns>
            public override IEnumerable<(string Destination, Pulse Pulse)> Process(string input, Pulse pulse)
            {
                if (pulse == Pulse.High)
                {
                    return Enumerable.Empty<(string, Pulse)>();
                }

                this.active = !this.active;

                Pulse nextPulse = this.active ? Pulse.High : Pulse.Low;

                return this.Outputs.Select(output => (output, nextPulse));
            }
        }

        /// <summary>
        /// Conjunction module, which sends a low pulse if all inputs are high, otherwise sends a high pulse
        /// </summary>
        private class ConjunctionModule : Module
        {
            private readonly List<Pulse> states = new();

            /// <summary>
            /// Initialises a new instance of the <see cref="ConjunctionModule"/> class.
            /// </summary>
            public ConjunctionModule(string id, string[] outputs) : base(id, outputs)
            {
            }

            /// <summary>
            /// Add an input to this module
            /// </summary>
            /// <param name="input"></param>
            public override void AddInput(string input)
            {
                // keep track of input states, assuming they're in the same order as the inputs themselves
                this.states.Add(Pulse.Low);
                base.AddInput(input);
            }

            /// <summary>
            /// Process a pulse
            /// </summary>
            /// <param name="input">Input module that generate the pulse</param>
            /// <param name="pulse">Pulse type</param>
            /// <returns>Pulses to send next (which may be none)</returns>
            public override IEnumerable<(string Destination, Pulse Pulse)> Process(string input, Pulse pulse)
            {
                int index = this.Inputs.IndexOf(input);
                this.states[index] = pulse;

                Pulse nextPulse = this.states.All(s => s == Pulse.High) ? Pulse.Low : Pulse.High;

                return this.Outputs.Select(output => (output, nextPulse));
            }
        }

        /// <summary>
        /// Broadcast module, which simply propagates a pulse to all outputs
        /// </summary>
        private class BroadcastModule : Module
        {
            /// <summary>
            /// Initialises a new instance of the <see cref="BroadcastModule"/> class.
            /// </summary>
            public BroadcastModule(string id, string[] outputs) : base(id, outputs)
            {
            }

            /// <summary>
            /// Process a pulse
            /// </summary>
            /// <param name="input">Input module that generate the pulse</param>
            /// <param name="pulse">Pulse type</param>
            /// <returns>Pulses to send next (which may be none)</returns>
            public override IEnumerable<(string Destination, Pulse Pulse)> Process(string input, Pulse pulse)
            {
                return this.Outputs.Select(output => (output, pulse));
            }
        }

        /// <summary>
        /// Output module, which receives all pulses and never transmit any
        /// </summary>
        private class OutputModule : Module
        {
            /// <summary>
            /// Initialises a new instance of the <see cref="OutputModule"/> class.
            /// </summary>
            public OutputModule(string id) : base(id, Array.Empty<string>())
            {
            }

            /// <summary>
            /// Process a pulse
            /// </summary>
            /// <param name="input">Input module that generate the pulse</param>
            /// <param name="pulse">Pulse type</param>
            /// <returns>Pulses to send next (which may be none)</returns>
            public override IEnumerable<(string Destination, Pulse Pulse)> Process(string input, Pulse pulse)
            {
                return Enumerable.Empty<(string Destination, Pulse Pulse)>();
            }
        }
    }
}
