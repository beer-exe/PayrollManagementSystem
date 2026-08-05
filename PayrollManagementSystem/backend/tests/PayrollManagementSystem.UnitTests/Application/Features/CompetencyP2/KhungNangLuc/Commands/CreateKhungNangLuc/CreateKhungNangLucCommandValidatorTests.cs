using FluentAssertions;
using PayrollManagementSystem.Application.Features.CompetencyP2.KhungNangLuc.Commands.CreateKhungNangLuc;
using Xunit;

namespace PayrollManagementSystem.UnitTests.Application.Features.CompetencyP2.KhungNangLuc.Commands.CreateKhungNangLuc
{
    public class CreateKhungNangLucCommandValidatorTests
    {
        private readonly CreateKhungNangLucCommandValidator _validator;

        public CreateKhungNangLucCommandValidatorTests()
        {
            _validator = new CreateKhungNangLucCommandValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_EmptyTenNangLuc_HasError(string? tenNangLuc)
        {
            var command = new CreateKhungNangLucCommand { TenNangLuc = tenNangLuc, TyTrong = 0.5m };
            var result = _validator.Validate(command);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "TenNangLuc" && e.ErrorMessage == "Tên năng lực không được để trống.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.1)]
        public void Validate_TyTrongLessThanOrEqualZero_HasError(decimal tyTrong)
        {
            var command = new CreateKhungNangLucCommand { TenNangLuc = "Test", TyTrong = tyTrong };
            var result = _validator.Validate(command);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "TyTrong" && e.ErrorMessage == "Tỷ trọng phải lớn hơn 0.");
        }

        [Fact]
        public void Validate_TyTrongGreaterThanOne_HasError()
        {
            var command = new CreateKhungNangLucCommand { TenNangLuc = "Test", TyTrong = 1.1m };
            var result = _validator.Validate(command);
            
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "TyTrong" && e.ErrorMessage == "Tỷ trọng không được vượt quá 100% (1.0).");
        }

        [Fact]
        public void Validate_ValidCommand_HasNoError()
        {
            var command = new CreateKhungNangLucCommand { TenNangLuc = "Test", TyTrong = 1.0m };
            var result = _validator.Validate(command);
            
            result.IsValid.Should().BeTrue();
        }
    }
}
