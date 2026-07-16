import * as XLSX from 'xlsx';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { RobotoRegularBase64 } from '../assets/fonts/Roboto-Regular';

export interface ExportColumn<T> {
  header: string;
  key: keyof T | string; // Use string if it's a computed property
  render?: (item: T) => string | number; // Function to get the value if it needs special formatting
}

/**
 * Export data to an Excel (.xlsx) file
 */
export const exportToExcel = <T>(
  data: T[],
  columns: ExportColumn<T>[],
  filename: string = 'export'
) => {
  // Transform data based on columns
  const formattedData = data.map(item => {
    const rowData: Record<string, string | number> = {};
    columns.forEach(col => {
      if (col.render) {
        rowData[col.header] = col.render(item);
      } else {
        const val = (item as any)[col.key];
        rowData[col.header] = val != null ? val : '';
      }
    });
    return rowData;
  });

  // Create a new workbook and add the worksheet
  const worksheet = XLSX.utils.json_to_sheet(formattedData);
  const workbook = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(workbook, worksheet, 'Data');

  // Generate Excel file buffer
  const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
  
  // Create Blob and trigger download
  const dataBlob = new Blob([excelBuffer], { 
    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8' 
  });
  
  const link = document.createElement('a');
  link.href = URL.createObjectURL(dataBlob);
  link.download = `${filename}.xlsx`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

/**
 * Export data to a PDF (.pdf) file
 */
export const exportToPdf = <T>(
  data: T[],
  columns: ExportColumn<T>[],
  filename: string = 'export',
  documentTitle: string = 'Document'
) => {
  const doc = new jsPDF();
  
  // Add custom font for Vietnamese support
  doc.addFileToVFS('Roboto-Regular.ttf', RobotoRegularBase64);
  doc.addFont('Roboto-Regular.ttf', 'Roboto', 'normal');
  doc.setFont('Roboto');
  
  // Add Title
  doc.setFontSize(16);
  doc.text(documentTitle, 14, 15);
  doc.setFontSize(10);
  
  // Add generated date
  const dateStr = new Date().toLocaleString('vi-VN');
  doc.text(`Ngày xuất: ${dateStr}`, 14, 22);

  // Prepare table data
  const head = [columns.map(col => col.header)];
  const body = data.map(item => 
    columns.map(col => {
      if (col.render) {
        return col.render(item);
      }
      const val = (item as any)[col.key];
      return val != null ? String(val) : '';
    })
  );

  // Draw table
  autoTable(doc, {
    head: head,
    body: body,
    startY: 28,
    styles: {
      font: 'Roboto', // Use custom font for Vietnamese support
      fontSize: 9,
      fontStyle: 'normal'
    },
    headStyles: {
      fillColor: [124, 58, 237], // Match --primary color (#7c3aed)
      textColor: 255,
      halign: 'left',
      fontStyle: 'normal'
    },
    alternateRowStyles: {
      fillColor: [248, 250, 252] // Very light gray/blue
    }
  });

  doc.save(`${filename}.pdf`);
};
