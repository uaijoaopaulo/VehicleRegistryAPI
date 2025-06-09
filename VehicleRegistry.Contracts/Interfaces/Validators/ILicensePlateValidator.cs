namespace VehicleRegistry.Contracts.InfraStructure.Validators
{
    public interface ILicensePlateValidator
    {
        /// <summary>
        /// Validates whether the provided license plate string matches the expected format or rules.
        /// </summary>
        /// <param name="licensePlate">The license plate string to validate.</param>
        /// <returns><c>true</c> if the license plate is valid; otherwise, <c>false</c>.</returns>
        bool IsValid(string licensePlate);
    }
}