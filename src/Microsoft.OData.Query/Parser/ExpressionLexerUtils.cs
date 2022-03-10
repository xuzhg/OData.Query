//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Globalization;

namespace Microsoft.OData.Query.Parser
{
    /// <summary>
    /// Utilities needed by <see cref="ExpressionLexer"/> which are relatively simple and standalone.
    /// </summary>
    internal sealed class ExpressionLexerUtils
    {
        /// <summary>Whether the specified token identifier is a numeric literal.</summary>
        /// <param name="kind">Token to check.</param>
        /// <returns>true if it's a numeric literal; false otherwise.</returns>
        internal static bool IsNumeric(ExpressionTokenKind kind)
        {
            return
                kind == ExpressionTokenKind.IntegerLiteral ||
                kind == ExpressionTokenKind.DecimalLiteral ||
                kind == ExpressionTokenKind.DoubleLiteral ||
                kind == ExpressionTokenKind.Int64Literal ||
                kind == ExpressionTokenKind.SingleLiteral;
        }

        /// <summary>
        /// Checks whether <paramref name="text"/> equals to "INF"
        /// </summary>
        /// <param name="text">Text to look in.</param>
        /// <returns>true if the substring is equal using an ordinal comparison; false otherwise.</returns>
        public static bool IsInfinityLiteral(string text)
        {
            return string.Equals(text, ExpressionConstants.InfinityLiteral);
        }

        /// <summary>
        /// Makes best guess on numeric string without trailing letter like L, F, M, D
        /// </summary>
        /// <param name="numericStr">The numeric string.</param>
        /// <param name="guessedKind">The possible kind (IntegerLiteral or DoubleLiteral) from ParseFromDigit() method.</param>
        /// <returns>A more accurate ExpressionTokenKind</returns>
        public static ExpressionTokenKind MakeBestGuessOnNoSuffixStr(string numericStr, ExpressionTokenKind guessedKind)
        {
            // no suffix, so
            // (1) make a best guess (note: later we support promoting each to later one: int32->int64->single->double->decimal).
            // look at value:       "2147483647" may be Int32/long, "2147483649" must be long.
            // look at precision:   "3258.67876576549" may be single/double/decimal, "3258.678765765489753678965390" must be decimal.
            // (2) then let MetadataUtilsCommon.CanConvertPrimitiveTypeTo() method does further promotion when knowing expected semantics type.
            int tmpInt = 0;
            long tmpLong = 0;
            float tmpFloat = 0;
            double tmpDouble = 0;
            decimal tmpDecimal = 0;

            if (guessedKind == ExpressionTokenKind.IntegerLiteral)
            {
                if (int.TryParse(numericStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out tmpInt))
                {
                    return ExpressionTokenKind.IntegerLiteral;
                }

                if (long.TryParse(numericStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out tmpLong))
                {
                    return ExpressionTokenKind.Int64Literal;
                }
            }

            bool canBeSingle = float.TryParse(numericStr, NumberStyles.Float, CultureInfo.InvariantCulture, out tmpFloat);
            bool canBeDouble = double.TryParse(numericStr, NumberStyles.Float, CultureInfo.InvariantCulture, out tmpDouble);
            bool canBeDecimal = decimal.TryParse(numericStr, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out tmpDecimal);

            // 1. try high precision -> low precision
            if (canBeDouble && canBeDecimal)
            {
                decimal doubleToDecimalR;
                decimal doubleToDecimalN;

                // To keep the full precision of the current value, which if necessary is all 17 digits of precision supported by the Double type.
                bool doubleCanBeDecimalR = decimal.TryParse(tmpDouble.ToString("R", CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out doubleToDecimalR);

                // To cover the scientific notation case, such as 1e+19 in the tmpDouble
                bool doubleCanBeDecimalN = decimal.TryParse(tmpDouble.ToString("N29", CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out doubleToDecimalN);

                if ((doubleCanBeDecimalR && doubleToDecimalR != tmpDecimal) || (!doubleCanBeDecimalR && doubleCanBeDecimalN && doubleToDecimalN != tmpDecimal))
                {
                    // losing precision as double, so choose decimal
                    return ExpressionTokenKind.DecimalLiteral;
                }
            }

            // here can't use normal casting like the above double VS decimal.
            // prevent losing precision in float -> double, e.g. (double)1.234f will be 1.2339999675750732d not 1.234d
            if (canBeSingle && canBeDouble && (double.Parse(tmpFloat.ToString("R", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) != tmpDouble))
            {
                // losing precision as single, so choose double
                return ExpressionTokenKind.DoubleLiteral;
            }

            // 2. try most compatible -> least compatible
            if (canBeSingle)
            {
                return ExpressionTokenKind.SingleLiteral;
            }

            if (canBeDouble)
            {
                return ExpressionTokenKind.DoubleLiteral;
            }

            throw new OException(string.Format(CultureInfo.InvariantCulture, OResource.ExpressionLexer_InvalidNumericString, numericStr));
        }
    }
}
