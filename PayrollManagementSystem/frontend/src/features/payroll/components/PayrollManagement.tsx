import React, { useState, useEffect } from 'react';
import { payrollApi } from '../api/payrollApi';
import { workScheduleApi } from '../../workSchedule/api/workScheduleApi';
import { PayrollListDto } from '../types/payroll.types';
import { Toast } from '../../../components/Toast/Toast';
import { PayrollDetailModal } from './PayrollDetailModal';
import { useDataTable } from '../../../hooks/useDataTable';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import './PayrollManagement.css';

const PayrollManagement: React.FC = () => {
  const [thang, setThang] = useState<number>(new Date().getMonth() + 1);
  const [nam, setNam] = useState<number>(new Date().getFullYear());
  const [payrolls, setPayrolls] = useState<PayrollListDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [calculating, setCalculating] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);
  const [selectedPayroll, setSelectedPayroll] = useState<PayrollListDto | null>(null);
  const [validYears, setValidYears] = useState<number[]>([]);

  // DataTable
  const {
    currentData: paginatedItems,
    allFilteredAndSortedData,
    currentPage,
    totalPages,
    setCurrentPage,
    sortKey,
    sortDirection,
    handleSort,
    searchTerm,
    setSearchTerm
  } = useDataTable<PayrollListDto>({
    data: payrolls,
    initialPageSize: 10,
    searchableFields: ['tenNhanVien', 'cccdNhanVien', 'tenPhongBan', 'tenChucVu']
  });

  useEffect(() => {
    const fetchYears = async () => {
      try {
        const res = await workScheduleApi.getAll();
        if (res.succeeded && res.data) {
          const years = Array.from(new Set(res.data.map((w: any) => w.nam))).sort((a, b) => b - a);
          setValidYears(years as number[]);
          if (years.length > 0) {
            const currentYear = new Date().getFullYear();
            const closestYear = (years as number[]).reduce((prev, curr) => Math.abs(curr - currentYear) < Math.abs(prev - currentYear) ? curr : prev);
            setNam(closestYear);
          }
        }
      } catch (error) {
        console.error('Lỗi khi tải lịch làm việc', error);
      }
    };
    fetchYears();
  }, []);

  const fetchPayrolls = async () => {
    try {
      setLoading(true);
      const res = await payrollApi.getPayrollList(thang, nam);
      if (res.succeeded) {
        setPayrolls(res.data || []);
      }
    } catch (error) {
      console.error('Lỗi khi tải bảng lương', error);
      setToast({ message: 'Không thể tải bảng lương. Vui lòng kiểm tra lại!', type: 'error' });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPayrolls();
  }, [thang, nam]);

  const handleCalculate = async () => {
    if (!window.confirm(`Bạn có chắc chắn muốn tính lương cho tháng ${thang}/${nam} không? Dữ liệu cũ của kỳ này (nếu có và chưa chốt) sẽ bị thay thế.`)) return;

    try {
      setCalculating(true);
      const res = await payrollApi.calculatePayroll({ thang, nam });
      if (res.succeeded) {
        setToast({ message: 'Tính lương thành công!', type: 'success' });
        fetchPayrolls();
      }
    } catch (error: any) {
      console.error('Lỗi khi tính lương', error);
      const msg = error.response?.data?.message || 'Có lỗi xảy ra khi tính lương.';
      setToast({ message: msg, type: 'error' });
    } finally {
      setCalculating(false);
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
  };

  const handleExportExcel = () => {
    const columns: ExportColumn<PayrollListDto>[] = [
      { header: 'Mã NV (CCCD)', key: 'cccdNhanVien' },
      { header: 'Tên nhân viên', key: 'tenNhanVien' },
      { header: 'Phòng ban', key: 'tenPhongBan' },
      { header: 'Chức vụ', key: 'tenChucVu' },
      { header: 'Lương P1', key: 'p1' },
      { header: 'Công thực tế', key: 'ngayCongThucTe' },
      { header: 'Lương thời gian', key: 'luongThoiGian' },
      { header: 'Khấu trừ', key: 'khauTru' },
      { header: 'Thực lĩnh', key: 'thucLinh' }
    ];
    exportToExcel(allFilteredAndSortedData, columns, `Bang_Luong_T${thang}_${nam}.xlsx`);
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<PayrollListDto>[] = [
      { header: 'Mã NV', key: 'cccdNhanVien' },
      { header: 'Tên NV', key: 'tenNhanVien' },
      { header: 'Lương P1', key: 'p1' },
      { header: 'Công', key: 'ngayCongThucTe' },
      { header: 'L.Thời gian', key: 'luongThoiGian' },
      { header: 'Khấu trừ', key: 'khauTru' },
      { header: 'Thực lĩnh', key: 'thucLinh' }
    ];
    exportToPdf(allFilteredAndSortedData, columns, `Bang_Luong_T${thang}_${nam}.pdf`, `BẢNG LƯƠNG THÁNG ${thang}/${nam}`);
  };

  return (
    <div className="prl-container">
      {/* Header */}
      <div className="prl-header">
        <div className="prl-header-title">
          <h2>💰 Bảng tính lương</h2>
          <p>Quản lý và tính toán lương cho nhân viên theo tháng</p>
        </div>
        <div className="prl-actions">
          {validYears.length > 0 && (
            <div style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
              <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                <span style={{ fontSize: '0.875rem', fontWeight: 500, color: 'var(--text-secondary)' }}>Kỳ lương:</span>
                <select className="prl-select" style={{ width: '100px', padding: '0.4rem 1rem' }} value={thang} onChange={(e) => setThang(Number(e.target.value))}>
                  {Array.from({ length: 12 }, (_, i) => i + 1).map(m => (
                    <option key={m} value={m}>Tháng {m}</option>
                  ))}
                </select>
                <select className="prl-select" style={{ width: '110px', padding: '0.4rem 1rem' }} value={nam} onChange={(e) => setNam(Number(e.target.value))}>
                  {validYears.map(y => (
                    <option key={y} value={y}>Năm {y}</option>
                  ))}
                </select>
              </div>
              <button
                className="prl-btn prl-btn-primary"
                onClick={handleCalculate}
                disabled={calculating}
              >
                {calculating ? (
                  <>
                    <div className="prl-spinner" style={{ width: 14, height: 14, borderRightColor: 'transparent', borderTopColor: '#fff', borderLeftColor: '#fff', borderBottomColor: '#fff' }} />
                    Đang tính...
                  </>
                ) : (
                  <>
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 18, height: 18 }}>
                      <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v12m-3-2.818.879.659c1.171.879 3.07.879 4.242 0 1.172-.879 1.172-2.303 0-3.182C13.536 12.219 12.768 12 12 12c-.725 0-1.45-.22-2.003-.659-1.106-.879-1.106-2.303 0-3.182s2.9-.879 4.006 0l.415.33M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
                    </svg>
                    Chạy tính lương
                  </>
                )}
              </button>
            </div>
          )}
        </div>
      </div>

      {/* Controls */}
      <div className="prl-controls-wrapper">
        <div className="prl-filters">
          <div className="prl-input-wrapper">
            <svg xmlns="http://www.w3.org/2000/svg" className="prl-input-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" width={16} height={16}>
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            <input
              type="text"
              className="prl-input"
              placeholder="Tìm kiếm theo mã NV, tên, phòng ban..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
        </div>

        <div className="prl-table-container custom-scrollbar">
          <table className="prl-table">
            <thead>
              <tr>
                <SortableHeader label="Nhân viên" sortKey="tenNhanVien" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Phòng ban" sortKey="tenPhongBan" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                <SortableHeader label="Lương P1" sortKey="p1" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'right' }} />
                <SortableHeader label="Công (TT/Ch)" sortKey="ngayCongThucTe" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                <SortableHeader label="Lương TG" sortKey="luongThoiGian" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'right' }} />
                <SortableHeader label="Khấu trừ" sortKey="khauTru" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'right' }} />
                <SortableHeader label="Thực lĩnh" sortKey="thucLinh" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'right' }} />
                <th style={{ textAlign: 'center', width: 90 }}>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={8}>
                    <div className="prl-loader">
                      <div className="prl-spinner" />
                    </div>
                  </td>
                </tr>
              ) : paginatedItems.length === 0 ? (
                <tr>
                  <td colSpan={8}>
                    <div className="prl-empty">
                      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1} stroke="currentColor" style={{ width: 48, height: 48, margin: '0 auto 1rem', opacity: 0.5 }}>
                        <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m0 12.75h7.5m-7.5 3H12M10.5 2.25H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z" />
                      </svg>
                      <p>Chưa có dữ liệu lương của tháng {thang}/{nam}. Hãy nhấn "Chạy tính lương" để bắt đầu.</p>
                    </div>
                  </td>
                </tr>
              ) : (
                paginatedItems.map((row) => (
                  <tr key={row.idBangLuong}>
                    <td>
                      <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{row.tenNhanVien}</div>
                      <div className="mono" style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>{row.cccdNhanVien}</div>
                    </td>
                    <td>
                      {row.tenPhongBan ? (
                        <div style={{ fontWeight: 500 }}>{row.tenPhongBan}</div>
                      ) : null}
                      {row.tenChucVu ? (
                        <div style={{ 
                          fontSize: row.tenPhongBan ? '0.75rem' : 'inherit', 
                          color: row.tenPhongBan ? 'var(--text-secondary)' : 'inherit',
                          fontWeight: row.tenPhongBan ? 'normal' : 500
                        }}>
                          {row.tenChucVu}
                        </div>
                      ) : null}
                      {!row.tenPhongBan && !row.tenChucVu && (
                        <div style={{ fontSize: '0.85rem', fontStyle: 'italic', color: 'var(--text-secondary)' }}>Chưa cập nhật</div>
                      )}
                    </td>
                    <td style={{ textAlign: 'right', fontWeight: 600, color: 'var(--primary)' }}>{formatCurrency(row.p1)}</td>
                    <td style={{ textAlign: 'center' }}>
                      <div style={{ fontWeight: 500 }}>{row.ngayCongThucTe} / {row.ngayCongChuan} ngày</div>
                      <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>
                        {row.gioCongThucTe} / {row.gioCongChuan} giờ
                      </div>
                    </td>
                    <td style={{ textAlign: 'right', fontWeight: 500 }}>{formatCurrency(row.luongThoiGian)}</td>
                    <td style={{ textAlign: 'right', fontWeight: 500, color: 'var(--danger-text)' }}>-{formatCurrency(row.khauTru)}</td>
                    <td style={{ textAlign: 'right', fontWeight: 600, color: 'var(--success-text)' }}>{formatCurrency(row.thucLinh)}</td>
                    <td style={{ textAlign: 'center' }}>
                      <button
                        className="prl-btn prl-btn-secondary"
                        onClick={() => setSelectedPayroll(row)}
                        title="Xem chi tiết"
                        style={{ padding: '0.35rem 0.65rem' }}
                      >
                        Chi tiết
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {totalPages > 0 && (
          <div className="prl-pagination">
            <div className="prl-pagination-info">
              Trang <span>{currentPage}</span> / {totalPages}
            </div>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <button
                className="prl-btn prl-btn-secondary"
                disabled={currentPage <= 1 || loading}
                onClick={() => setCurrentPage(p => p - 1)}
                style={{ padding: '0.35rem 0.75rem' }}
              >
                Trước
              </button>
              <button
                className="prl-btn prl-btn-secondary"
                disabled={currentPage >= totalPages || loading}
                onClick={() => setCurrentPage(p => p + 1)}
                style={{ padding: '0.35rem 0.75rem' }}
              >
                Sau
              </button>
            </div>
          </div>
        )}
      </div>

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}

      {selectedPayroll && (
        <PayrollDetailModal
          payroll={selectedPayroll}
          onClose={() => setSelectedPayroll(null)}
        />
      )}
    </div>
  );
};

export default PayrollManagement;
