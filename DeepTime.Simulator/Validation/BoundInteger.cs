using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DeepTime.Simulator.Validation;

public class BoundInteger : ValidationRule
{
    public int? Min { get; set; } = null;
    public int? Max { get; set; } = null;
    public bool AllowsNull { get; set; } = false;

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var str = value as string;

        if (int.TryParse(str, out var number))
            if (CheckMinBound(number) && CheckMaxBound(number))
                return ValidationResult.ValidResult;
            else
                return new(false, $"Value is not in range {Min} : {Max}");
        else
            if (AllowsNull && str.Length == 0)
                return ValidationResult.ValidResult;
            else
                return new(false, "Invalid input");
    }

    private bool CheckMinBound(int number) => !Min.HasValue && number >= Min;
    private bool CheckMaxBound(int number) => !Max.HasValue && number <= Max;
}
