using System.Text.RegularExpressions;
using VehicleRegistry.Contracts.InfraStructure.Validators;

namespace VehicleRegistry.InfraStructure.Validators
{
    public class LicensePlateValidator : ILicensePlateValidator
    {
        private readonly Regex _plateRegex = new(@"^[A-Za-z]{3}\d{4}$|^[A-Za-z]{3}\d[A-Za-z]\d{2}$", RegexOptions.Compiled);

        public bool IsValid(string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate) || licensePlate.Length < 7)
            {
                return false;
            }

            return _plateRegex.IsMatch(licensePlate);
        }
    }
}