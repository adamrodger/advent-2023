using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Utilities;

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

        public int Part2(string[] input)
        {
            foreach (string line in input)
            {
                throw new NotImplementedException("Part 2 not implemented");
            }

            return 0;
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
                long soil = GetOrDefault(this.SeedToSoil, seed);
                long fertiliser = GetOrDefault(this.SoilToFertiliser, soil);
                long water = GetOrDefault(this.FertiliserToWater, fertiliser);
                long light = GetOrDefault(this.WaterToLight, water);
                long temperature = GetOrDefault(this.LightToTemperature, light);
                long humidity = GetOrDefault(this.TemperatureToHumidity, temperature);
                long location = GetOrDefault(this.HumidityToLocation, humidity);

                return location;
            }

            private static long GetOrDefault(IList<ItemMap> map, long source)
            {
                ItemMap range = map.FirstOrDefault(m => m.Source <= source && m.Source + m.Range >= source);

                if (range == null)
                {
                    return source;
                }

                long rangeOffset = source - range.Source;
                return range.Destination + rangeOffset;
            }
        }

        private record ItemMap(long Destination, long Source, long Range);
    }
}
