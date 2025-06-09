using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRegistry.Contracts.InfraStructure.Validators
{
    public interface ILicensePlateValidator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="licensePlate"></param>
        /// <returns></returns>
        bool IsValid(string licensePlate);
    }
}
