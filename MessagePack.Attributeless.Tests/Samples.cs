﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoBogus;
using AutoBogus.Conventions;
using NUnit.Framework;

namespace MessagePack.Attributeless.Tests
{
    public static class Samples
    {
        static Samples() => AutoFaker.Configure(builder => { builder.WithConventions(); });

        public class Address
        {
            public string City { get; set; }
            public string Country { get; set; }
            public string StreetAddress { get; set; }
            public string ZipCode { get; set; }
        }

        public abstract class AnAnimal : IAnimal
        {
            public IExtremity[] Extremities { get; set; }
            public string Name { get; set; }
        }

        public abstract class AnExtremity : IExtremity
        {
            public Side Side { get; set; }
        }

        public class Arm : AnExtremity
        {
            public byte NumberOfFingers { get; set; }
        }

        public class Bird : AnAnimal
        {
            public TimeSpan IncubationPeriod { get; set; }
        }

        public class Leg : AnExtremity
        {
            public byte NumberOfToes { get; set; }
        }

        public class Mammal : AnAnimal
        {
            public TimeSpan Gestation { get; set; }
        }

        public class Person
        {
            public IList<Address> Addresses { get; set; }
            public DateTime Birthday { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class PersonWithPet
        {
            public Person Human { get; set; }
            public IAnimal Pet { get; set; }
        }

        public class Wing : AnExtremity
        {
            public int Span { get; set; }
        }

        public enum Side
        {
            Left,
            Right
        }

        public interface IAnimal
        {
            IExtremity[] Extremities { get; set; }
            string Name { get; set; }
        }

        public interface IExtremity
        {
            Side Side { get; set; }
        }

        public static Person MakePerson() => AutoFaker.Generate<Person>();

        public static IEnumerable<IAnimal> AnimalCases
        {
            get
            {
                yield return new Mammal
                {
                    Name = "Papio (Baboon)",
                    Gestation = TimeSpan.FromDays(6 * 30),
                    Extremities = new IExtremity[]
                    {
                        new Arm
                        {
                            Side = Side.Left,
                            NumberOfFingers = 5
                        },
                        new Arm
                        {
                            Side = Side.Right,
                            NumberOfFingers = 5
                        },
                        new Leg
                        {
                            Side = Side.Left,
                            NumberOfToes = 5
                        },
                        new Leg
                        {
                            Side = Side.Right,
                            NumberOfToes = 5
                        }
                    }
                };
                yield return new Bird
                {
                    Name = "Falco peregrinus",
                    IncubationPeriod = TimeSpan.FromDays(30),
                    Extremities = new IExtremity[]
                    {
                        new Wing
                        {
                            Side = Side.Left,
                            Span = 120
                        },
                        new Wing
                        {
                            Side = Side.Right,
                            Span = 120
                        },
                        new Leg
                        {
                            Side = Side.Left,
                            NumberOfToes = 4
                        },
                        new Leg
                        {
                            Side = Side.Right,
                            NumberOfToes = 4
                        }
                    }
                };
            }
        }

        public static IEnumerable<IExtremity> ExtremityCases
        {
            get
            {
                yield return AutoFaker.Generate<Arm>();
                yield return AutoFaker.Generate<Leg>();
                yield return AutoFaker.Generate<Wing>();
            }
        }

        public static IEnumerable<TestCaseData> PeopleWithTheirPets =>
            AnimalCases.ToArray()
                .Select(a => new PersonWithPet
                {
                    Pet = a,
                    Human = MakePerson()
                })
                .Select((x, i) => new TestCaseData(x).SetName("{m} Case #" + i));
    }
}