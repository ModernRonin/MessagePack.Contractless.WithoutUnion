﻿using ApprovalTests;
using FluentAssertions;
using NUnit.Framework;

namespace MessagePack.Attributeless.Tests
{
    [TestFixture]
    public class MessagePackSerializerOptionsBuilderTests
    {
        MessagePackSerializerOptionsBuilder Configure() => Configure(MessagePackSerializer.DefaultOptions);

        MessagePackSerializerOptionsBuilder Configure(MessagePackSerializerOptions options) =>
            options.Configure()
                .AutoKeyed<Samples.Address>()
                .AutoKeyed<Samples.Person>()
                .AddNativeFormatters()
                .SubType<Samples.IExtremity, Samples.Arm>()
                .SubType<Samples.IExtremity, Samples.Leg>()
                .SubType<Samples.IExtremity, Samples.Wing>()
                .SubType<Samples.IAnimal, Samples.Mammal>()
                .SubType<Samples.IAnimal, Samples.Bird>()
                .AutoKeyed<Samples.PersonWithPet>();

        MessagePackSerializerOptionsBuilder ConfigureMostSimply() =>
            ConfigureMostSimply(MessagePackSerializer.DefaultOptions);

        MessagePackSerializerOptionsBuilder ConfigureMostSimply(MessagePackSerializerOptions options) =>
            options.Configure()
                .AllSubTypesOf<Samples.IExtremity>()
                .AllSubTypesOf<Samples.IAnimal>()
                .AutoKeyed<Samples.PersonWithPet>();

        MessagePackSerializerOptionsBuilder ConfigureWithDifference() =>
            MessagePackSerializer.DefaultOptions.Configure()
                .AutoKeyed<Samples.Address>()
                .AutoKeyed<Samples.Person>()
                .AddNativeFormatters()
                .SubType<Samples.IExtremity, Samples.Leg>()
                .SubType<Samples.IExtremity, Samples.Wing>()
                .SubType<Samples.IAnimal, Samples.Mammal>()
                .SubType<Samples.IAnimal, Samples.Bird>()
                .AutoKeyed<Samples.PersonWithPet>();

        [TestCaseSource(typeof(Samples), nameof(Samples.PeopleWithTheirPets))]
        public void Roundtrip_with(Samples.PersonWithPet input)
        {
            var options = Configure().Build();

            options.TestRoundtrip(input);
        }

        [TestCaseSource(typeof(Samples), nameof(Samples.PeopleWithTheirPets))]
        public void Roundtrip_most_simple_configuration_with(Samples.PersonWithPet input)
        {
            var options = ConfigureMostSimply().Build();

            options.TestRoundtrip(input);
        }

        [TestCaseSource(typeof(Samples), nameof(Samples.PeopleWithTheirPets))]
        public void Roundtrip_with_compression(Samples.PersonWithPet input)
        {
            var options =
                Configure(MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression
                        .Lz4BlockArray))
                    .Build();

            options.TestRoundtrip(input);
        }

        [Test]
        public void Checksum_is_LIKELY_to_be_different_for_a_modified_configuration()
        {
            var oldVersion = Configure().Validation.Checksum;
            var newVersion = ConfigureWithDifference().Validation.Checksum;

            newVersion.Should().NotEqual(oldVersion);
        }

        [Test]
        public void Checksum_is_the_same_for_the_same_configuration()
        {
            var oldVersion = Configure().Validation.Checksum;
            var newVersion = Configure().Validation.Checksum;

            newVersion.Should().Equal(oldVersion);
        }

        [Test]
        public void KeyTable()
        {
            var builder = Configure();
            var keytable = (IEnumerable) builder.Validation.KeyTable;
            // not Environment.NewLine to prevent issues between the platform where the approved file was saved being different from the one the test is executed on 
            var asText = string.Join('\n', keytable);
            Approvals.Verify(asText);
        }
    }
}