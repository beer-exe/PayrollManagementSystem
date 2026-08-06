import React, { useState, useEffect } from 'react';
import './MyAttendance.css';
import { useMyAttendance } from '../hooks/useMyAttendance';
import { ChamCongDto } from '../../chamCong/types/chamCong.types';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';

const LOAI_NGAY_COLOR: Record<string, string> = {
  'Làm đủ ca': 'patt-badge patt-status--ontime',
  'Nửa ca': 'patt-badge patt-status--late',
  'Đi trễ / Về sớm': 'patt-badge patt-status--late',
  'Vắng có phép': 'patt-badge patt-status--default',
  'Vắng không phép': 'patt-badge patt-status--absent',
  'Nghỉ lễ': 'patt-badge patt-status--default',
  'Nghỉ cuối tuần': 'patt-badge patt-status--default',
};

const TRANG_THAI_COLOR: Record<string, string> = {
  'Chưa xác nhận': 'patt-badge patt-status--absent',
  'Đã xác nhận': 'patt-badge patt-status--ontime',
  'Cần giải trình': 'patt-badge patt-status--late',
};

const MyAttendance: React.FC = () => {
  const [currentDate, setCurrentDate] = useState(new Date());
  const { attendanceList, isLoading, error, fetchAttendance } = useMyAttendance();

  useEffect(() => {
    fetchAttendance(currentDate.getMonth() + 1, currentDate.getFullYear());
  }, [currentDate, fetchAttendance]);

  const handlePrevMonth = () => {
    setCurrentDate(prev => new Date(prev.getFullYear(), prev.getMonth() - 1, 1));
  };

  const handleNextMonth = () => {
    setCurrentDate(prev => new Date(prev.getFullYear(), prev.getMonth() + 1, 1));
  };

  const {
    currentData,
    allFilteredAndSortedData,
    currentPage,
    totalPages,
    setCurrentPage,
    sortKey,
    sortDirection,
    handleSort,
    searchTerm,
    setSearchTerm
  } = useDataTable<ChamCongDto>({
    data: attendanceList,
    initialPageSize: 10,
    searchableFields: ['ngayChamCong', 'loaiNgayCong', 'trangThai']
  });

  // Reset pagination when date changes
  useEffect(() => {
    setCurrentPage(1);
  }, [currentDate, setCurrentPage]);

  const handleExportExcel = () => {
    const columns: ExportColumn<ChamCongDto>[] = [
      { header: 'Ngày', key: 'ngayChamCong' },
      { header: 'Giờ vào', key: 'gioVao' },
      { header: 'Giờ ra', key: 'gioRa' },
      { header: 'Số giờ làm', key: 'soGioLamThucTe' },
      { header: 'Công', key: 'soNgayCong' },
      { header: 'Loại ngày', key: 'loaiNgayCong' },
      { header: 'Trạng thái', key: 'trangThai' },
      { header: 'Ghi chú', key: 'ghiChu' },
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'KetQuaChamCongCaNhan');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<ChamCongDto>[] = [
      { header: 'Ngày', key: 'ngayChamCong' },
      { header: 'Giờ vào', key: 'gioVao' },
      { header: 'Giờ ra', key: 'gioRa' },
      { header: 'Số giờ', key: 'soGioLamThucTe' },
      { header: 'Công', key: 'soNgayCong' },
      { header: 'Loại ngày', key: 'loaiNgayCong' },
      { header: 'Trạng thái', key: 'trangThai' },
      { header: 'Ghi chú', key: 'ghiChu' },
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'KetQuaChamCongCaNhan', 'Kết quả chấm công cá nhân');
  };

  const formatTime = (timeStr?: string | null) => {
    if (!timeStr) return '--:--';
    return timeStr.substring(0, 5);
  };

  const formatDate = (dateStr: string) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
  };

  return (
    <div className="patt-container">
      <div className="patt-header">
        <div className="patt-header-title">
          <h2>📅 Kết quả chấm công cá nhân</h2>
          <p>Xem lịch sử chấm công và giờ làm việc của bạn</p>
        </div>
      </div>

      <div className="patt-controls-wrapper">
        <div className="patt-filters">
          <div className="patt-input-wrapper">
            <svg className="patt-input-icon" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.1rem', height: '1.1rem' }}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
            </svg>
            <input
              type="text"
              placeholder="Tìm theo ngày, loại, trạng thái..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="patt-input"
            />
          </div>
          
          <select 
            className="patt-select"
            value={currentDate.getMonth()} 
            onChange={(e) => setCurrentDate(new Date(currentDate.getFullYear(), parseInt(e.target.value), 1))}
          >
            {Array.from({ length: 12 }, (_, i) => (
              <option key={i} value={i}>Tháng {i + 1}</option>
            ))}
          </select>

          <select 
            className="patt-select"
            value={currentDate.getFullYear()} 
            onChange={(e) => setCurrentDate(new Date(parseInt(e.target.value), currentDate.getMonth(), 1))}
          >
            {[0, 1, 2].map(offset => {
              const year = new Date().getFullYear() - offset;
              return <option key={year} value={year}>Năm {year}</option>
            })}
          </select>

          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
        </div>

        {error && <div className="patt-error" style={{ margin: '16px' }}>{error}</div>}

        <div className="patt-content">
        {isLoading ? (
          <div className="patt-loading">Đang tải dữ liệu...</div>
        ) : (
          <div className="patt-table-wrapper">
            {attendanceList.length === 0 ? (
              <div className="patt-empty">Chưa có dữ liệu chấm công trong tháng này.</div>
            ) : (
              <>
                <table className="patt-table">
                <thead>
                  <tr>
                    <SortableHeader label="Ngày" sortKey="ngayChamCong" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                    <SortableHeader label="Loại Ngày" sortKey="loaiNgayCong" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                    <SortableHeader label="Giờ Vào" sortKey="gioVao" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                    <SortableHeader label="Giờ Ra" sortKey="gioRa" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                    <SortableHeader label="Số Giờ Làm" sortKey="soGioLamThucTe" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                    <SortableHeader label="Công" sortKey="soNgayCong" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                    <SortableHeader label="Trạng Thái" sortKey="trangThai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                    <th>Ghi Chú</th>
                  </tr>
                </thead>
                <tbody>
                  {currentData.map((item) => (
                    <tr key={item.id}>
                      <td style={{ fontWeight: 500, color: 'var(--text-secondary)' }}>{formatDate(item.ngayChamCong)}</td>
                      <td><span className={LOAI_NGAY_COLOR[item.loaiNgayCong] ?? 'patt-badge'}>{item.loaiNgayCong}</span></td>
                      <td>{formatTime(item.gioVao)}</td>
                      <td>{formatTime(item.gioRa)}</td>
                      <td style={{ fontWeight: 600 }}>{item.soGioLamThucTe.toFixed(2)}</td>
                      <td style={{ fontWeight: 600 }}>{item.soNgayCong}</td>
                      <td>
                        <span className={TRANG_THAI_COLOR[item.trangThai] ?? 'patt-badge'}>
                          {item.trangThai}
                        </span>
                      </td>
                      <td style={{ fontSize: '12px', color: 'var(--text-muted)' }}>{item.ghiChu || '-'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
              {totalPages > 0 && (
                <div className="patt-pagination">
                  <button 
                    className="patt-btn-secondary" 
                    disabled={currentPage === 1} 
                    onClick={() => setCurrentPage(p => p - 1)}
                  >
                    Trước
                  </button>
                  <span style={{ fontSize: '0.875rem', fontWeight: 500, color: 'var(--text-secondary)' }}>
                    Trang <span style={{ color: 'var(--text-primary)', fontWeight: 600 }}>{currentPage}</span> / <span style={{ color: 'var(--text-primary)', fontWeight: 600 }}>{totalPages}</span>
                  </span>
                  <button 
                    className="patt-btn-secondary" 
                    disabled={currentPage === totalPages} 
                    onClick={() => setCurrentPage(p => p + 1)}
                  >
                    Sau
                  </button>
                </div>
              )}
              </>
            )}
          </div>
        )}
      </div>
      </div>
    </div>
  );
};

export default MyAttendance;
