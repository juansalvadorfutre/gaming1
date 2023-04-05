using Application.Exceptions;
using FluentAssertions;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.Application.Exception {
    public class ValidationExceptionShould
    {
        private ValidationException _sut;
        public ValidationExceptionShould()
        {
        }

        [Fact]
        public void Create_An_Empty_Error_Dictionary_When_Calling_Default_Constructor()
        {
            _sut = new ValidationException();

            _sut.Errors.Keys.Should().BeEquivalentTo(Array.Empty<string>());
        }

        [Fact]
        public void Create_A_Single_Element_Error_Dictionary_When_Having_A_Single_Validation_Failure()
        {
            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Age", "must be over 18"),
            };

            _sut = new ValidationException(failures);

            _sut.Errors.Keys.Should().BeEquivalentTo(new string[] { "Age" });
            _sut.Errors["Age"].Should().BeEquivalentTo(new string[] { "must be over 18" });
        }

        [Fact]
        public void Create_A_Multiple_Element_Error_Dictionary_Each_With_Multiple_Values_When_Having_Mulitple_Validation_Failure_For_Multiple_Properties()
        {
            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Age", "must be 18 or older"),
                new ValidationFailure("Age", "must be 25 or younger"),
                new ValidationFailure("Password", "must contain at least 8 characters"),
                new ValidationFailure("Password", "must contain a digit"),
                new ValidationFailure("Password", "must contain upper case letter"),
                new ValidationFailure("Password", "must contain lower case letter"),
            };

            _sut = new ValidationException(failures);

            _sut.Errors.Keys.Should().BeEquivalentTo(new string[] { "Password", "Age" });

            _sut.Errors["Age"].Should().BeEquivalentTo(new string[]
            {
                "must be 25 or younger",
                "must be 18 or older",
            });

            _sut.Errors["Password"].Should().BeEquivalentTo(new string[]
            {
                "must contain lower case letter",
                "must contain upper case letter",
                "must contain at least 8 characters",
                "must contain a digit",
            });
        }
    }
}
