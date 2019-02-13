using System;
using System.Reflection;
using DNTFrameworkCore.Auditing;
using DNTFrameworkCore.Helper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace DNTFrameworkCore.Tests.Auditing
{
    [TestFixture]
    public class AuditingHelperTests
    {
        [Test]
        public void Should_Save()
        {
            var helper = BuildAuditingHelper(options =>
            {
                options.Enabled = true;
                options.EnabledForAnonymousUsers = true;
                options.Selectors.Add(new NamedTypeSelector("PartyService", type => type == typeof(PartyService)));
            });

            helper.Save(new AuditInfo());
        }

        [Test]
        public void Should_Audit_When_Selectors_Contains_ExpectedType()
        {
            var helper = BuildAuditingHelper(options =>
            {
                options.Enabled = true;
                options.EnabledForAnonymousUsers = true;
                options.Selectors.Add(new NamedTypeSelector("PartyService", type => type == typeof(PartyService)));
            });

            helper.ShouldAudit(typeof(PartyService).GetMethod(nameof(PartyService.PublicMethod))).ShouldBeTrue();
        }

        [Test]
        public void Should_Not_Audit_When_Selectors_Not_Contains_ExpectedType()
        {
            var helper = BuildAuditingHelper(options =>
            {
                options.Enabled = true;
                options.EnabledForAnonymousUsers = true;
            });

            helper.ShouldAudit(typeof(PartyService).GetMethod(nameof(PartyService.PublicMethod))).ShouldBeFalse();
        }

        [Test]
        public void Should_Not_Audit_When_Type_With_SkipAuditing()
        {
            var helper = BuildAuditingHelper(options =>
            {
                options.Enabled = true;
                options.EnabledForAnonymousUsers = true;
                options.Selectors.Add(new NamedTypeSelector("SkipAuditingType",
                    type => type == typeof(SkipAuditingType)));
            });

            helper.ShouldAudit(typeof(SkipAuditingType).GetMethod(nameof(SkipAuditingType.PublicMethod)))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_Not_Audit_When_Method_With_SkipAuditing()
        {
            var helper = BuildAuditingHelper(options =>
            {
                options.Enabled = true;
                options.EnabledForAnonymousUsers = true;
                options.Selectors.Add(new NamedTypeSelector("PartyService", type => type == typeof(PartyService)));
            });

            helper.ShouldAudit(typeof(PartyService).GetMethod(nameof(PartyService.SkipAuditingMethod))).ShouldBeFalse();
        }

        [Test]
        public void Should_Not_Audit_When_Method_Is_Not_Public()
        {
            var helper = BuildAuditingHelper(options =>
            {
                options.Enabled = true;
                options.EnabledForAnonymousUsers = true;
                options.Selectors.Add(new NamedTypeSelector("PartyService", type => type == typeof(PartyService)));
            });

            helper.ShouldAudit(typeof(PartyService).GetMethod("PrivateMethod",
                BindingFlags.NonPublic | BindingFlags.Instance)).ShouldBeFalse();
        }

        [Test]
        public void Should_Not_Audit_When_MethodInfo_Null()
        {
            var helper = BuildAuditingHelper(options =>
            {
                options.Enabled = true;
                options.EnabledForAnonymousUsers = true;
                options.Selectors.Add(new NamedTypeSelector("PartyService", type => type == typeof(PartyService)));
            });

            helper.ShouldAudit(null).ShouldBeFalse();
        }

        [Test]
        public void Should_Not_Audit_When_Auditing_Disabled_For_AnonymousUsers_With_NullUserSession()
        {
            var helper = BuildAuditingHelper(options =>
            {
                options.EnabledForAnonymousUsers = false;
                options.Selectors.Add(new NamedTypeSelector("PartyService", type => type == typeof(PartyService)));
            });

            helper.ShouldAudit(typeof(PartyService).GetMethod(nameof(PartyService.PublicMethod))).ShouldBeFalse();
        }

        [Test]
        public void Should_Not_Audit_When_Auditing_Disabled()
        {
            var helper = BuildAuditingHelper(options =>
            {
                options.Enabled = false;
                options.EnabledForAnonymousUsers = true;
                options.Selectors.Add(new NamedTypeSelector("PartyService", type => type == typeof(PartyService)));
            });

            helper.ShouldAudit(typeof(PartyService).GetMethod(nameof(PartyService.PublicMethod))).ShouldBeFalse();
        }

        [Test]
        public void Should_Not_BuildAuditInfo_Serialize_The_Ignored_Properties_And_Ignored_Types()
        {
            var helper = BuildAuditingHelper(options => options.IgnoredTypes.Add(typeof(Exception)));

            var info = helper.BuildAuditInfo(typeof(PartyService),
                typeof(PartyService).GetMethod(nameof(PartyService.PublicMethod)), new[]
                {
                    new AuditingHelperTestParty
                    {
                        DisplayName = "Gh Rabbal",
                        Age = 26,
                        Exception = new Exception("this should be ignored!"),
                        Address = new AuditingHelperTestPartyAddress
                        {
                            Country = "Iran",
                            Address = "Urmia ..."
                        }
                    }
                });

            info.Parameters.ShouldBe("{\"party\":{\"displayName\":\"Gh Rabbal\",\"address\":{\"country\":\"Iran\"}}}");
        }

        private static IAuditingHelper BuildAuditingHelper(Action<AuditingOptions> options)
        {
            var services = new ServiceCollection();

            services.AddDNTFramework()
                .AddAuditingOptions(options);
            services.AddLogging();

            var helper = services.BuildServiceProvider().GetRequiredService<IAuditingHelper>();
            return helper;
        }

        private class AuditingHelperTestParty
        {
            public string DisplayName { get; set; }

            [SkipAuditing] public int Age { get; set; }

            public Exception Exception { get; set; }

            public AuditingHelperTestPartyAddress Address { get; set; }
        }

        private class AuditingHelperTestPartyAddress
        {
            public string Country { get; set; }
            [SkipAuditing] public string Address { get; set; }
        }

        private class PartyService
        {
            public void PublicMethod(AuditingHelperTestParty party)
            {
            }

            private void PrivateMethod(AuditingHelperTestParty party)
            {
            }

            [SkipAuditing]
            public void SkipAuditingMethod(AuditingHelperTestParty party)
            {
                //save party
            }
        }

        [SkipAuditing]
        private class SkipAuditingType
        {
            public void PublicMethod()
            {
            }
        }
    }
}