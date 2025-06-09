namespace VehicleRegistry.Contracts.InfraStructure.VehicleRegistry.Api
{
    public static class ApiResponseHelper
    {
        public static ApiResponse<T> Success<T>(T result)
        {
            return new ApiResponse<T>
            {
                Result = result
            };
        }

        public static ApiResponse<object> Failure(string error)
        {
            return new ApiResponse<object>
            {
                Errors = new List<string> { error }
            };
        }

        public static ApiResponse<object> Failure(List<string> errors)
        {
            return new ApiResponse<object>
            {
                Errors = errors
            };
        }
    }
}
