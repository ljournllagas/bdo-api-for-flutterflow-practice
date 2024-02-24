using FluentValidation;
using System;

namespace Application.Common.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> MustHaveLengthBetween<T>(this IRuleBuilder<T, string> rule, int min, int max)
        {
            return rule
                .Length(min, max)
                .When((model, prop) => !string.IsNullOrEmpty(prop));
        }

        public static IRuleBuilderOptions<T, string> MustHaveMaximumLength<T>(this IRuleBuilder<T, string> rule, int length)
        {
            return rule
                .MaximumLength(length).WithMessage("{PropertyName} must not exceed {MaxLength} character/s.")
                .When((model, prop) => !string.IsNullOrEmpty(prop));
        }

        public static IRuleBuilderOptions<T, int?> MustHaveAValidKey<T>(this IRuleBuilder<T, int?> rule)
        {
            return rule
                .GreaterThan(0).WithMessage("{PropertyName} must have a valid Key")
                .When((model, prop) => prop != null);
        }

        public static IRuleBuilderOptions<T, decimal?> MustHaveAGreaterThanDecimalValue<T>(this IRuleBuilder<T, decimal?> rule)
        {
            return rule
                .GreaterThan(0).WithMessage("{PropertyName} must have a valid amount")
                .When((model, prop) => prop != null);
        }

        public static IRuleBuilderOptions<T, int?> MustHaveAGreaterThanIntValue<T>(this IRuleBuilder<T, int?> rule)
        {
            return rule
                .GreaterThan(0).WithMessage("{PropertyName} must have a valid amount")
                .When((model, prop) => prop != null);
        }

        /// <summary>
        /// Predicate builder which makes the validated property available
        /// </summary>
        /// <param name="IRuleBuilderOptions<T"></param>
        /// <param name="rule"></param>
        /// <param name="predicate"></param>
        /// <param name="applyConditionTo"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> When<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, TProperty, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
        {
            return rule.Configure(config =>
            {
                config.ApplyCondition(ctx => predicate((T)ctx.InstanceToValidate, (TProperty)ctx.PropertyValue), applyConditionTo);
            });
        }
    }
}
