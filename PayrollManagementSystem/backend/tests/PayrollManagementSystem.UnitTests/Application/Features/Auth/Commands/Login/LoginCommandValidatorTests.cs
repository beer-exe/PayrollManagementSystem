using FluentAssertions;
using PayrollManagementSystem.Application.Features.Auth.Commands.Login;

namespace PayrollManagementSystem.UnitTests.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidatorTests
    {
        private readonly LoginCommandValidator _validator;

        public LoginCommandValidatorTests()
        {
            _validator = new LoginCommandValidator();
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData(null, "password")]
        public void Validate_EmptyUsername_HasError(string username, string password)
        {
            var command = new LoginCommand { TenTaiKhoan = username, MatKhau = password };
            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "TenTaiKhoan" && e.ErrorMessage == "Tên tài khoản không được để trống.");
        }

        [Theory]
        [InlineData("admin", "")]
        [InlineData("admin", null)]
        public void Validate_EmptyPassword_HasError(string username, string password)
        {
            var command = new LoginCommand { TenTaiKhoan = username, MatKhau = password };
            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "MatKhau" && e.ErrorMessage == "Mật khẩu không được để trống.");
        }

        [Fact]
        public void Validate_ValidCommand_HasNoError()
        {
            var command = new LoginCommand { TenTaiKhoan = "admin", MatKhau = "password123" };
            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
