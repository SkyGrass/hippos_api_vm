using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ams.Utils.EPPlus.DataValidation.Formulas.Contracts;

namespace Ams.Utils.EPPlus.DataValidation.Contracts
{
    /// <summary>
    /// Validation interface for datetime validations
    /// </summary>
    public interface IExcelDataValidationDateTime : IExcelDataValidationWithFormula2<IExcelDataValidationFormulaDateTime>, IExcelDataValidationWithOperator
    {
    }
}
