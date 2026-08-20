using FluentAssertions;
using PayrollManagementSystem.Application.Features.Auth.Commands.RefreshToken;

namespace PayrollManagementSystem.UnitTests.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandValidatorTests
    {
        private readonly RefreshTokenCommandValidator _validator;

        public RefreshTokenCommandValidatorTests()
        {
            _validator = new RefreshTokenCommandValidator();
        }

        [Theory]
        [InlineData("", "refresh_token")]
        [InlineData(null, "refresh_token")]
        public void Validate_EmptyAccessToken_HasError(string accessToken, string refreshToken)
        {
            var command = new RefreshTokenCommand { AccessToken = accessToken, RefreshToken = refreshToken };
            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "AccessToken" && e.ErrorMessage == "Access Token không được để trống.");
        }

        [Theory]
        [InlineData("access_token", "")]
        [InlineData("access_token", null)]
        public void Validate_EmptyRefreshToken_HasError(string accessToken, string refreshToken)
        {
            var command = new RefreshTokenCommand { AccessToken = accessToken, RefreshToken = refreshToken };
            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "RefreshToken" && e.ErrorMessage == "Refresh Token không được để trống.");
        }

        [Fact]
        public void Validate_ValidCommand_HasNoError()
        {
            var command = new RefreshTokenCommand { AccessToken = "access_token", RefreshToken = "refresh_token" };
            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
