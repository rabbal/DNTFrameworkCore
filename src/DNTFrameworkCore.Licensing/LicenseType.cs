using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Licensing
{
    //under development 
    public class LicenseType : Enumeration
    {
        public static readonly LicenseType None = new LicenseType(0, nameof(None));
        public static readonly LicenseType Trial = new LicenseType(1, nameof(Trial));
        public static readonly LicenseType Standard = new LicenseType(2, nameof(Standard));
        public static readonly LicenseType Personal = new PersonalLicenseType();
        public static readonly LicenseType Floating = new LicenseType(4, nameof(Floating));
        public static readonly LicenseType Subscription = new LicenseType(5, nameof(Subscription));

        private LicenseType(int value, string name) : base(value, name)
        {
        }

        public virtual bool VerifySerialNumber => true;
        public virtual bool VerifyProductVersion => true;

        public static IReadOnlyList<LicenseType> List()
        {
            return new[] {None, Trial, Standard, Personal, Floating, Subscription};
        }

        public static explicit operator int(LicenseType type)
        {
            return type.Value;
        }

        public static explicit operator LicenseType(int value)
        {
            return FromValue(value);
        }

        public static explicit operator LicenseType(string name)
        {
            return FromName(name);
        }

        public static LicenseType FromName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var type = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.InvariantCultureIgnoreCase));

            if (type == null)
                throw new InvalidOperationException(
                    $"Possible values for LicenseType: {string.Join(",", List().Select(s => s.Name))}");

            return type;
        }

        public static LicenseType FromValue(int value)
        {
            var type = List().SingleOrDefault(s => s.Value == value);

            if (type == null)
                throw new InvalidOperationException(
                    $"Possible values for LicenseType: {string.Join(",", List().Select(s => s.Value))}");

            return type;
        }

        private class PersonalLicenseType : LicenseType
        {
            public PersonalLicenseType() : base(3, nameof(Personal))
            {
            }

            public override bool VerifySerialNumber => false;
            public override bool VerifyProductVersion => false;
        }
    }
}