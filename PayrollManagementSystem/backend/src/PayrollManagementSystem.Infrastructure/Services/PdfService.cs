using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.SystemManagement.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PayrollManagementSystem.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            // Cấu hình QuestPDF theo chuẩn community license (miễn phí cho doanh nghiệp nhỏ)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] ExportSystemLogsToPdf(IEnumerable<SystemLogDto> logs)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(x => ComposeContent(x, logs));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("BÁO CÁO NHẬT KÝ HỆ THỐNG").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}").FontSize(11);
                });
            });
        }

        private void ComposeContent(IContainer container, IEnumerable<SystemLogDto> logs)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);  // ID
                        columns.ConstantColumn(120); // Ngày giờ
                        columns.ConstantColumn(80);  // Level
                        columns.RelativeColumn();    // Nội dung
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("ID");
                        header.Cell().Element(CellStyle).Text("Thời gian");
                        header.Cell().Element(CellStyle).Text("Mức độ");
                        header.Cell().Element(CellStyle).Text("Nội dung");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var log in logs)
                    {
                        table.Cell().Element(CellStyle).Text(log.Id.ToString());
                        // Convert UTC to GMT+7 for display
                        var localTime = TimeZoneInfo.ConvertTimeFromUtc(log.RaiseDate, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                        table.Cell().Element(CellStyle).Text(localTime.ToString("dd/MM/yyyy HH:mm:ss"));
                        table.Cell().Element(CellStyle).Text(log.Level);
                        table.Cell().Element(CellStyle).Text(log.Message ?? "");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Trang ");
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        }
    }
}
