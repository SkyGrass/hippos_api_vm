using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ams.Utils.EPPlus.DataValidation.Formulas.Contracts;

namespace Ams.Utils.EPPlus.DataValidation.Contracts
{
    /// <summary>
    /// Data validation interface for custom validation.
    /// </summary>
    public interface IExcelDataValidationCustom : IExcelDataValidationWithFormula<IExcelDataValidationFormula>, IExcelDataValidationWithOperator
    {
    }
}
