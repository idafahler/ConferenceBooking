using ConferenceBooking.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Tests
{
    public class ValidationHelperTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("123456")]
        [InlineData("!! !!")]
        [InlineData("Hej hej hej hej hej hej")]
        public void ValidateName_InvalidNaming_ShouldReturnString(string? name)
        {
            var result = ValidationHelper.ValidateName(name, "name");

            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("111111")]
        [InlineData("aaaaaa")]
        [InlineData("aaaaa1")]
        [InlineData("    ")]
        [InlineData("...")]
        public void ValidatePassword_InvalidPassword_ShouldReturnString(string? password)
        {
            var result = ValidationHelper.ValidatePassWord(password);

            Assert.NotNull(result);
        }
    }
}
