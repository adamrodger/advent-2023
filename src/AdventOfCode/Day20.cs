using System.Collections.Generic;
using System.Linq;

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

        public int Part2(string[] input)
        {
            var graph = ModuleGraph.Parse(input);

            int i = 0;

            while (!graph.PushButtonAndCheckReceiver())
            {
                i++;
            }

            return i + 1;
        }

        private class ModuleGraph
        {
            private readonly IDictionary<string, Module> modules;

            private long highSent;
            private long lowSent;

            public long Score => this.highSent * this.lowSent;

            /// <summary>
            /// Initialises a new instance of the <see cref="ModuleGraph"/> class.
            /// </summary>
            public ModuleGraph(IDictionary<string, Module> modules)
            {
                this.modules = modules;
            }

            public static ModuleGraph Parse(IReadOnlyList<string> input)
            {
                var modules = input.Select(Module.Parse).ToDictionary(m => m.Id);

                foreach (Module module in modules.Values)
                {
                    foreach (string output in module.Outputs)
                    {
                        if (!modules.ContainsKey(output))
                        {
                            // hmmmmmm...... and it's called rx as well......
                            continue;
                        }

                        modules[output].AddInput(module.Id);
                    }
                }

                return new ModuleGraph(modules);
            }

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

                    if (!this.modules.ContainsKey(destination))
                    {
                        // hmmmmmmmmmmmmmmmmmmmmmmmmmm
                        continue;
                    }

                    Module module = this.modules[destination];

                    foreach ((string Destination, Pulse Pulse) next in module.Process(sender, pulse))
                    {
                        queue.Enqueue((module.Id, next.Destination, next.Pulse));
                    }
                }
            }

            public bool PushButtonAndCheckReceiver()
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

                    if (!this.modules.ContainsKey(destination))
                    {
                        // hmmmmmmmmmmmmmmmmmmmmmmmmmm
                        if (pulse == Pulse.Low)
                        {
                            return true;
                        }

                        continue;
                    }

                    Module module = this.modules[destination];

                    foreach ((string Destination, Pulse Pulse) next in module.Process(sender, pulse))
                    {
                        queue.Enqueue((module.Id, next.Destination, next.Pulse));
                    }
                }

                return false;
            }
        }

        private abstract class Module
        {
            public string Id { get; }

            public IList<string> Inputs { get; }

            public IReadOnlyList<string> Outputs { get; }

            /// <summary>
            /// Initialises a new instance of the <see cref="Module"/> class.
            /// </summary>
            protected Module(string id, IReadOnlyList<string> outputs)
            {
                this.Id = id;
                this.Inputs = new List<string>();
                this.Outputs = outputs;
            }

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

            public virtual void AddInput(string input)
            {
                this.Inputs.Add(input);
            }

            public abstract IEnumerable<(string Destination, Pulse Pulse)> Process(string input, Pulse pulse);
        }

        private enum Pulse
        {
            High,
            Low
        }

        private class FlipFlopModule : Module
        {
            private bool active = false;

            /// <summary>
            /// Initialises a new instance of the <see cref="FlipFlopModule"/> class.
            /// </summary>
            public FlipFlopModule(string id, string[] outputs) : base(id, outputs)
            {
            }

            public override IEnumerable<(string Destination, Pulse Pulse)> Process(string input, Pulse pulse)
            {
                if (pulse == Pulse.High)
                {
                    yield break; // ignore
                }

                this.active = !this.active;

                Pulse nextPulse = this.active ? Pulse.High : Pulse.Low;

                foreach (string output in this.Outputs)
                {
                    yield return (output, nextPulse);
                }
            }
        }

        private class ConjunctionModule : Module
        {
            private readonly List<Pulse> states = new();

            /// <summary>
            /// Initialises a new instance of the <see cref="ConjunctionModule"/> class.
            /// </summary>
            public ConjunctionModule(string id, string[] outputs) : base(id, outputs)
            {
            }

            public override void AddInput(string input)
            {
                this.states.Add(Pulse.Low);
                base.AddInput(input);
            }

            public override IEnumerable<(string Destination, Pulse Pulse)> Process(string input, Pulse pulse)
            {
                int index = this.Inputs.IndexOf(input);
                this.states[index] = pulse;

                Pulse nextPulse = this.states.All(s => s == Pulse.High) ? Pulse.Low : Pulse.High;

                foreach (string output in this.Outputs)
                {
                    yield return (output, nextPulse);
                }
            }
        }

        private class BroadcastModule : Module
        {
            /// <summary>
            /// Initialises a new instance of the <see cref="BroadcastModule"/> class.
            /// </summary>
            public BroadcastModule(string id, string[] outputs) : base(id, outputs)
            {
            }

            public override IEnumerable<(string Destination, Pulse Pulse)> Process(string input, Pulse pulse)
            {
                return this.Outputs.Select(output => (output, pulse));
            }
        }
    }
}
