import React, { useState, useEffect, useCallback } from 'react';
import { payrollApi } from '../api/payrollApi';
import { workScheduleApi } from '../../workSchedule/api/workScheduleApi';
import { PayrollListDto, KyLuongStatusDto } from '../types/payroll.types';
import { Toast } from '../../../components/Toast/Toast';
import { PayrollDetailModal } from './PayrollDetailModal';
import { ReopenPayrollModal } from './ReopenPayrollModal';
import { useDataTable } from '../../../hooks/useDataTable';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { useAuthStore } from '@/store/useAuthStore';
import './PayrollManagement.css';

const PayrollManagement: React.FC = () => {
  const { user } = useAuthStore();
  // Nghiệp vụ quy định: Chỉ HR cấp quản lý (role HR và có hasDirectReports) mới có quyền mở chốt và chốt sớm
  const isHrManager = user?.role === 'HR' && !!user?.hasDirectReports;

  const [thang, setThang] = useState<number>(new Date().getMonth() + 1);
  const [nam, setNam] = useState<number>(new Date().getFullYear());
  const [payrolls, setPayrolls] = useState<PayrollListDto[]>([]);
  const [kyLuongStatus, setKyLuongStatus] = useState<KyLuongStatusDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [calculating, setCalculating] = useState(false);
  const [closing, setClosing] = useState(false);
  const [reopening, setReopening] = useState(false);
  const [isReopenModalOpen, setIsReopenModalOpen] = useState(false);
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

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      const [payrollRes, statusRes] = await Promise.all([
        payrollApi.getPayrollList(thang, nam),
        payrollApi.getKyLuongStatus(thang, nam)
      ]);

      if (payrollRes.succeeded) {
        setPayrolls(payrollRes.data || []);
      }
      if (statusRes.succeeded && statusRes.data) {
        setKyLuongStatus(statusRes.data);
      }
    } catch (error) {
      console.error('Lỗi khi tải dữ liệu bảng lương', error);
      setToast({ message: 'Không thể tải bảng lương. Vui lòng kiểm tra lại!', type: 'error' });
    } finally {
      setLoading(false);
    }
  }, [thang, nam]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const handleCalculate = async () => {
    if (kyLuongStatus?.isLocked) {
      setToast({ message: 'Kỳ lương này đã được chốt sổ, không thể tính lại. Chỉ HR cấp quản lý mới có quyền mở chốt.', type: 'error' });
      return;
    }

    if (!window.confirm(`Bạn có chắc chắn muốn tính lương cho tháng ${thang}/${nam} không? Dữ liệu cũ của kỳ này (nếu có và chưa chốt) sẽ bị thay thế.`)) return;

    try {
      setCalculating(true);
      const res = await payrollApi.calculatePayroll({ thang, nam });
      if (res.succeeded) {
        setToast({ message: 'Tính lương thành công!', type: 'success' });
        fetchData();
      }
    } catch (error: any) {
      console.error('Lỗi khi tính lương', error);
      const msg = error.response?.data?.Message || error.response?.data?.message || 'Có lỗi xảy ra khi tính lương.';
      setToast({ message: msg, type: 'error' });
    } finally {
      setCalculating(false);
    }
  };

  const handleClosePayroll = async () => {
    if (kyLuongStatus?.isLocked) {
      setToast({ message: 'Kỳ lương này đã được chốt rồi.', type: 'error' });
      return;
    }

    // Kiểm tra thời gian chốt lương
    const now = new Date();
    const lastDayOfMonth = new Date(nam, thang, 0); // Ngày cuối cùng của tháng
    const isBeforeEndOfMonth = now < lastDayOfMonth;

    if (isBeforeEndOfMonth) {
      if (!isHrManager) {
        setToast({
          message: `Không thể chốt kỳ lương tháng ${thang}/${nam} trước khi kết thúc tháng (ngày ${lastDayOfMonth.toLocaleDateString('vi-VN')}). Chỉ HR cấp quản lý mới có quyền chốt trước thời hạn!`,
          type: 'error'
        });
        return;
      }

      const confirmPremature = window.confirm(
        `⚠️ CẢNH BÁO CHỐT SỚM:\nTháng ${thang}/${nam} chưa kết thúc (Hôm nay là ngày ${now.toLocaleDateString('vi-VN')}, kết thúc kỳ là ngày ${lastDayOfMonth.toLocaleDateString('vi-VN')}).\n` +
        `Nếu bạn chốt sớm, ngày công của những ngày còn lại trong tháng sẽ không được tính vào bảng lương trừ khi mở chốt lại sau đó.\n\n` +
        `Bạn có chắc chắn muốn chốt sớm kỳ lương này không?`
      );
      if (!confirmPremature) return;
    } else {
      if (!window.confirm(`Bạn có chắc chắn muốn CHỐT lương tháng ${thang}/${nam}? Sau khi chốt sẽ không thể tính lại nếu không được HR cấp quản lý mở chốt.`)) return;
    }

    try {
      setClosing(true);
      const res = await payrollApi.closePayroll({ thang, nam });
      if (res.succeeded) {
        setToast({ message: `Đã chốt lương tháng ${thang}/${nam} thành công!`, type: 'success' });
        fetchData();
      }
    } catch (error: any) {
      console.error('Lỗi khi chốt lương', error);
      const msg = error.response?.data?.Message || error.response?.data?.message || 'Có lỗi xảy ra khi chốt lương.';
      setToast({ message: msg, type: 'error' });
    } finally {
      setClosing(false);
    }
  };

  const handleReopenConfirm = async (lyDo: string) => {
    try {
      setReopening(true);
      const res = await payrollApi.reopenPayroll({ thang, nam, lyDo });
      if (res.succeeded) {
        setToast({ message: `Đã mở chốt kỳ lương tháng ${thang}/${nam} thành công!`, type: 'success' });
        setIsReopenModalOpen(false);
        fetchData();
      }
    } catch (error: any) {
      console.error('Lỗi khi mở chốt lương', error);
      const msg = error.response?.data?.Message || error.response?.data?.message || 'Có lỗi xảy ra khi mở chốt kỳ lương.';
      setToast({ message: msg, type: 'error' });
    } finally {
      setReopening(false);
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
    const formatCurrencyExport = (amount: number) => {
      return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    };

    const columns: ExportColumn<PayrollListDto>[] = [
      { header: 'Mã NV', key: 'cccdNhanVien' },
      { header: 'Tên NV', key: 'tenNhanVien' },
      { header: 'Lương P1', key: 'p1', render: (item) => formatCurrencyExport(item.p1) },
      { header: 'Công', key: 'ngayCongThucTe' },
      { header: 'L.Thời gian', key: 'luongThoiGian', render: (item) => formatCurrencyExport(item.luongThoiGian) },
      { header: 'Khấu trừ', key: 'khauTru', render: (item) => formatCurrencyExport(item.khauTru) },
      { header: 'Thực lĩnh', key: 'thucLinh', render: (item) => formatCurrencyExport(item.thucLinh) }
    ];
    exportToPdf(allFilteredAndSortedData, columns, `Bang_Luong_T${thang}_${nam}.pdf`, `BẢNG LƯƠNG THÁNG ${thang}/${nam}`);
  };

  const isLocked = kyLuongStatus?.isLocked ?? false;

  return (
    <div className="prl-container">
      {/* Header */}
      <div className="prl-header">
        <div className="prl-header-title">
          <h2>💰 Quản lý bảng lương</h2>
          <p>Quản lý và tính toán lương cho nhân viên theo phương pháp 3P</p>
        </div>
        <div className="prl-actions">
          {validYears.length > 0 && (
            <div style={{ display: 'flex', gap: '1rem', alignItems: 'center', flexWrap: 'wrap' }}>
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

              {/* Action Buttons */}
              <button
                className="prl-btn prl-btn-primary"
                onClick={handleCalculate}
                disabled={calculating || isLocked}
                title={isLocked ? "Kỳ lương đã chốt. Mở chốt trước nếu cần tính lại." : "Chạy tính toán lương cho toàn bộ nhân viên"}
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
              
              {/* Dynamic Action Button: Chốt kỳ lương / Mở chốt kỳ lương */}
              {isLocked ? (
                isHrManager ? (
                  <button
                    className="prl-btn prl-btn-warning"
                    onClick={() => setIsReopenModalOpen(true)}
                    disabled={reopening}
                    title="Mở chốt kỳ lương để tính bổ sung hoặc điều chỉnh (Dành riêng cho HR Cấp Quản lý)"
                  >
                    {reopening ? (
                      <>
                        <div className="prl-spinner" style={{ width: 14, height: 14, borderRightColor: 'transparent', borderTopColor: '#fff', borderLeftColor: '#fff', borderBottomColor: '#fff' }} />
                        Đang mở...
                      </>
                    ) : (
                      <>
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 18, height: 18 }}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 10.5V6.75a4.5 4.5 0 1 1 9 0v3.75M3.75 21.75h10.5a2.25 2.25 0 0 0 2.25-2.25v-6.75a2.25 2.25 0 0 0-2.25-2.25H3.75a2.25 2.25 0 0 0-2.25 2.25v6.75a2.25 2.25 0 0 0 2.25 2.25Z" />
                        </svg>
                        Mở chốt kỳ lương
                      </>
                    )}
                  </button>
                ) : (
                  <button
                    className="prl-btn"
                    disabled={true}
                    title="Kỳ lương này đã được chốt sổ. Chỉ HR cấp quản lý mới có quyền mở chốt."
                    style={{
                      background: '#94a3b8',
                      color: 'white',
                      border: 'none',
                      opacity: 0.6,
                      cursor: 'not-allowed'
                    }}
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 18, height: 18 }}>
                      <path strokeLinecap="round" strokeLinejoin="round" d="M16.5 10.5V6.75a4.5 4.5 0 1 0-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 0 0 2.25-2.25v-6.75a2.25 2.25 0 0 0-2.25-2.25H6.75a2.25 2.25 0 0 0-2.25 2.25v6.75a2.25 2.25 0 0 0 2.25 2.25Z" />
                    </svg>
                    Đã chốt kỳ lương
                  </button>
                )
              ) : (
                <button
                  className="prl-btn"
                  onClick={handleClosePayroll}
                  disabled={closing || payrolls.length === 0}
                  title="Chốt sổ kỳ lương"
                  style={{
                    background: 'var(--success-color, #10b981)',
                    color: 'white',
                    border: 'none'
                  }}
                >
                  {closing ? (
                    <>
                      <div className="prl-spinner" style={{ width: 14, height: 14, borderRightColor: 'transparent', borderTopColor: '#fff', borderLeftColor: '#fff', borderBottomColor: '#fff' }} />
                      Đang chốt...
                    </>
                  ) : (
                    <>
                      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 18, height: 18 }}>
                        <path strokeLinecap="round" strokeLinejoin="round" d="M9 12.75 11.25 15 15 9.75M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
                      </svg>
                      Chốt kỳ lương
                    </>
                  )}
                </button>
              )}
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
                <SortableHeader label="Trạng thái" sortKey="trangThaiText" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                <th style={{ textAlign: 'center', width: 90 }}>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={9}>
                    <div className="prl-loader">
                      <div className="prl-spinner" />
                    </div>
                  </td>
                </tr>
              ) : paginatedItems.length === 0 ? (
                <tr>
                  <td colSpan={9}>
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
                      <span className={`prl-status ${
                        row.trangThai === 'DA_XAC_NHAN' ? 'success' : 
                        row.trangThai === 'YEU_CAU_XEM_XET' ? 'danger' : 'neutral'
                      }`}>
                        {row.trangThaiText}
                      </span>
                    </td>
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
          <div className="prl-pagination" style={{ justifyContent: 'flex-end' }}>
            <div style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
              <button
                className="prl-btn prl-btn-secondary"
                disabled={currentPage <= 1 || loading}
                onClick={() => setCurrentPage(p => p - 1)}
                style={{ padding: '0.35rem 0.75rem' }}
              >
                Trước
              </button>
              <span style={{ fontSize: '0.875rem', fontWeight: 500, color: 'var(--text-secondary)' }}>
                Trang <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{currentPage}</span> / <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{totalPages}</span>
              </span>
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

      {/* Modal Mở chốt kỳ lương */}
      <ReopenPayrollModal
        isOpen={isReopenModalOpen}
        onClose={() => setIsReopenModalOpen(false)}
        onConfirm={handleReopenConfirm}
        thang={thang}
        nam={nam}
        loading={reopening}
      />
    </div>
  );
};

export default PayrollManagement;
export { PayrollManagement };
