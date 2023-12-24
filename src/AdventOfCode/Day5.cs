using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Utilities;
using Optional;

namespace AdventOfCode
{
    /// <summary>
    /// Solver for Day 5
    /// </summary>
    public class Day5
    {
        public long Part1(string[] input)
        {
            var instructions = Instructions.Parse(input);
            return instructions.Seeds.Select(instructions.SeedLocation).Min();
        }

        public long Part2(string[] input)
        {
            var instructions = Instructions.Parse(input);

            if (input.Length < 100)
            {
                // sample input
                Option<long> testSeed = instructions.LocationSeed(46);
                Debug.Assert(testSeed.ValueOr(0) == 82);
            }

            for (long location = 0; location < long.MaxValue; location++)
            {
                Option<long> seed = instructions.LocationSeed(location);

                if (seed.HasValue)
                {
                    return location;
                }
            }

            throw new InvalidOperationException("Not found. And how did you even loop this far?");
        }

        private record Instructions(IList<long> Seeds,
                                    IList<ItemMap> SeedToSoil,
                                    IList<ItemMap> SoilToFertiliser,
                                    IList<ItemMap> FertiliserToWater,
                                    IList<ItemMap> WaterToLight,
                                    IList<ItemMap> LightToTemperature,
                                    IList<ItemMap> TemperatureToHumidity,
                                    IList<ItemMap> HumidityToLocation)
        {
            public static Instructions Parse(ICollection<string> input)
            {
                long[] seeds = input.First().Numbers<long>();

                IList<ItemMap> seedToSoil = ParseSection(input, "seed-to-soil map");
                IList<ItemMap> soilToFertiliser = ParseSection(input, "soil-to-fertilizer map");
                IList<ItemMap> fertiliserToWater = ParseSection(input, "fertilizer-to-water map");
                IList<ItemMap> waterToLight = ParseSection(input, "water-to-light map");
                IList<ItemMap> lightToTemperature = ParseSection(input, "light-to-temperature map");
                IList<ItemMap> temperatureToHumidity = ParseSection(input, "temperature-to-humidity map");
                IList<ItemMap> humidityToLocation = ParseSection(input, "humidity-to-location map");

                return new Instructions(seeds,
                                        seedToSoil,
                                        soilToFertiliser,
                                        fertiliserToWater,
                                        waterToLight,
                                        lightToTemperature,
                                        temperatureToHumidity,
                                        humidityToLocation);
            }

            private static IList<ItemMap> ParseSection(ICollection<string> input, string header)
            {
                return input.SkipWhile(l => !l.StartsWith(header))
                            .Skip(1)
                            .TakeWhile(l => l.Length > 0 && char.IsAsciiDigit(l[0]))
                            .Select(l => l.Numbers<long>())
                            .Select(n => new ItemMap(n[0], n[1], n[2]))
                            .OrderBy(i => i.Source)
                            .ToArray();
            }

            public long SeedLocation(long seed)
            {
                long soil = GetFromSource(this.SeedToSoil, seed);
                long fertiliser = GetFromSource(this.SoilToFertiliser, soil);
                long water = GetFromSource(this.FertiliserToWater, fertiliser);
                long light = GetFromSource(this.WaterToLight, water);
                long temperature = GetFromSource(this.LightToTemperature, light);
                long humidity = GetFromSource(this.TemperatureToHumidity, temperature);
                long location = GetFromSource(this.HumidityToLocation, humidity);

                return location;
            }

            public Option<long> LocationSeed(long location)
            {
                long humidity = GetFromDestination(this.HumidityToLocation, location);
                long temperature = GetFromDestination(this.TemperatureToHumidity, humidity);
                long light = GetFromDestination(this.LightToTemperature, temperature);
                long water = GetFromDestination(this.WaterToLight, light);
                long fertiliser = GetFromDestination(this.FertiliserToWater, water);
                long soil = GetFromDestination(this.SoilToFertiliser, fertiliser);
                long seed = GetFromDestination(this.SeedToSoil, soil);

                // check if this is a valid starting seed
                for (int i = 0; i < this.Seeds.Count; i += 2)
                {
                    if (this.Seeds[i] <= seed && seed < this.Seeds[i] + this.Seeds[i + 1])
                    {
                        return seed.Some();
                    }
                }

                return Option.None<long>();
            }

            private static long GetFromSource(IList<ItemMap> map, long source)
            {
                ItemMap range = map.FirstOrDefault(m => m.Source <= source && source < m.Source + m.Range);

                if (range == null)
                {
                    return source;
                }

                long rangeOffset = source - range.Source;
                return range.Destination + rangeOffset;
            }

            private static long GetFromDestination(IList<ItemMap> map, long destination)
            {
                ItemMap range = map.FirstOrDefault(m => m.Destination <= destination && destination < m.Destination + m.Range);

                if (range == null)
                {
                    return destination;
                }

                long rangeOffset = destination - range.Destination;
                return range.Source + rangeOffset;
            }
        }

        private record ItemMap(long Destination, long Source, long Range);
    }
}
