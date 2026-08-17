import React, { useState, useEffect } from 'react';
import { kpiApi } from '../api/kpiApi';
import { KyKpi } from '../types/kpi.types';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { Toast } from '../../../components/Toast/Toast';
import './kpi.css';

export const KpiManagement: React.FC = () => {
  const [kyKpis, setKyKpis] = useState<KyKpi[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [tenKyKpi, setTenKyKpi] = useState('');
  const [thang, setThang] = useState(new Date().getMonth() + 1);
  const [nam, setNam] = useState(new Date().getFullYear());
  const [isLoadingData, setIsLoadingData] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' | 'info' } | null>(null);

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
  } = useDataTable<KyKpi>({
    data: kyKpis,
    initialPageSize: 10,
    searchableFields: ['tenKyKpi', 'trangThai']
  });

  useEffect(() => {
    fetchKyKpis();
  }, []);

  const fetchKyKpis = async () => {
    setIsLoadingData(true);
    try {
      const response = await kpiApi.getKyKpis();
      setKyKpis(response.data);
    } catch (error) {
      console.error('Lỗi khi tải danh sách kỳ KPI:', error);
      setToast({ message: 'Không thể tải danh sách kỳ KPI', type: 'error' });
    } finally {
      setIsLoadingData(false);
    }
  };

  const handleCreateKyKpi = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      await kpiApi.createKyKpi({ tenKyKpi, thang, nam });
      setToast({ message: 'Tạo kỳ KPI thành công!', type: 'success' });
      setIsModalOpen(false);
      fetchKyKpis();
    } catch (error: any) {
      console.error('Lỗi tạo kỳ KPI:', error);
      setToast({ message: error.response?.data?.Message || error.response?.data?.message || 'Có lỗi xảy ra khi tạo kỳ KPI', type: 'error' });
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleExportExcel = () => {
    const columns: ExportColumn<KyKpi>[] = [
      { header: 'Tên kỳ KPI', key: 'tenKyKpi' },
      { header: 'Tháng', key: 'thang' },
      { header: 'Năm', key: 'nam' },
      { header: 'Trạng thái', key: 'trangThai' },
      { header: 'Số phiếu đã duyệt', key: 'soPhieuDaDuyet' },
      { header: 'Tổng số phiếu', key: 'tongSoPhieu' }
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'KyKpi');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<KyKpi>[] = [
      { header: 'Tên kỳ KPI', key: 'tenKyKpi' },
      { header: 'Tháng', key: 'thang' },
      { header: 'Năm', key: 'nam' },
      { header: 'Trạng thái', key: 'trangThai' },
      { header: 'Đã duyệt', key: 'soPhieuDaDuyet' },
      { header: 'Tổng', key: 'tongSoPhieu' }
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'KyKpi', 'Danh sách Kỳ KPI');
  };

  return (
    <div className="kpi-container">
      <div className="kpi-header">
        <div className="kpi-header-title">
          <h2>🎯 Quản lý Kỳ KPI</h2>
          <p>Tạo và quản lý các kỳ đánh giá hiệu suất (KPI) định kỳ</p>
        </div>
        <button 
          className="kpi-btn kpi-btn-primary" 
          onClick={() => {
            setTenKyKpi('');
            setThang(new Date().getMonth() + 1);
            setNam(new Date().getFullYear());
            setIsModalOpen(true);
          }}
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
          </svg>
          Tạo kỳ KPI
        </button>
      </div>

      <div className="kpi-controls-wrapper">
        <div style={{ display: 'flex', justifyContent: 'space-between', padding: '1rem', borderBottom: '1px solid var(--border-color)', gap: '1rem', flexWrap: 'wrap' }}>
          <div className="kpi-input-wrapper" style={{ flex: 1, minWidth: '250px', position: 'relative' }}>
            <input
              type="text"
              placeholder="Tìm kiếm kỳ KPI..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="kpi-form-input"
              style={{ width: '100%', paddingLeft: '0.75rem' }}
            />
          </div>
          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
        </div>

        <div className="kpi-table-container custom-scrollbar">
          {isLoadingData ? (
            <div className="kpi-loader">
              <div className="kpi-spinner"></div>
            </div>
          ) : currentData.length > 0 ? (
            <table className="kpi-table">
              <thead>
                <tr>
                  <SortableHeader label="Tên Kỳ KPI" sortKey="tenKyKpi" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                  <SortableHeader label="Tháng" sortKey="thang" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <SortableHeader label="Năm" sortKey="nam" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <SortableHeader label="Trạng thái" sortKey="trangThai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <th style={{ textAlign: 'center' }}>Tiến độ duyệt</th>
                </tr>
              </thead>
              <tbody>
                {currentData.map((ky) => {
                  let badgeClass = "kpi-badge-gray";
                  if (ky.trangThaiValue === 1) badgeClass = "kpi-badge-blue"; // Đang thực hiện
                  if (ky.trangThaiValue === 2) badgeClass = "kpi-badge-warning"; // Chờ phê duyệt
                  if (ky.trangThaiValue === 3) badgeClass = "kpi-badge-success"; // Đã phê duyệt / Đã chốt

                  return (
                    <tr key={ky.idKyKpi}>
                      <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{ky.tenKyKpi}</td>
                      <td style={{ textAlign: 'center' }}>{ky.thang}</td>
                      <td style={{ textAlign: 'center' }}>{ky.nam}</td>
                      <td style={{ textAlign: 'center' }}>
                        <span className={`kpi-badge ${badgeClass}`}>
                          {ky.trangThai}
                        </span>
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        {ky.soPhieuDaDuyet} / {ky.tongSoPhieu}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          ) : (
            <div className="kpi-empty">
              <p>Không có dữ liệu kỳ KPI nào.</p>
            </div>
          )}
        </div>

        {totalPages > 0 && (
          <div className="kpi-pagination">
            <button 
              className="kpi-btn kpi-btn-secondary" 
              onClick={() => setCurrentPage(p => p - 1)} 
              disabled={currentPage === 1 || isLoadingData}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              Trước
            </button>
            <div className="kpi-pagination-info">
              Trang <span>{currentPage}</span> / <span>{totalPages}</span>
            </div>
            <button 
              className="kpi-btn kpi-btn-secondary" 
              onClick={() => setCurrentPage(p => p + 1)} 
              disabled={currentPage === totalPages || isLoadingData}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              Sau
            </button>
          </div>
        )}
      </div>

      {isModalOpen && (
        <div className="kpi-modal-overlay">
          <div className="kpi-modal">
            <div className="kpi-modal-header">
              <h3 className="kpi-modal-title">Tạo Kỳ KPI Mới</h3>
              <button className="kpi-modal-close" onClick={() => setIsModalOpen(false)} disabled={isSubmitting}>
                &times;
              </button>
            </div>

            <div className="kpi-modal-body">
              <form id="kpi-form" onSubmit={handleCreateKyKpi}>
                <div className="kpi-form-group">
                  <label className="kpi-form-label">Tên Kỳ KPI <span className="required">*</span></label>
                  <input
                    type="text"
                    className="kpi-form-input"
                    value={tenKyKpi}
                    onChange={(e) => setTenKyKpi(e.target.value)}
                    required
                    placeholder="VD: Đánh giá KPI Tháng 8/2026"
                  />
                </div>
                
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem', marginBottom: '1rem' }}>
                  <div className="kpi-form-group" style={{ marginBottom: 0 }}>
                    <label className="kpi-form-label">Tháng <span className="required">*</span></label>
                    <input
                      type="number"
                      className="kpi-form-input"
                      min={1}
                      max={12}
                      value={thang}
                      onChange={(e) => setThang(Number(e.target.value))}
                      required
                    />
                  </div>
                  <div className="kpi-form-group" style={{ marginBottom: 0 }}>
                    <label className="kpi-form-label">Năm <span className="required">*</span></label>
                    <input
                      type="number"
                      className="kpi-form-input"
                      min={2000}
                      value={nam}
                      onChange={(e) => setNam(Number(e.target.value))}
                      required
                    />
                  </div>
                </div>

                <div style={{ marginTop: '1rem', fontSize: '0.85rem', color: 'var(--text-secondary)' }}>
                  Lưu ý: Hệ thống sẽ tự động phát sinh Phiếu KPI trống cho tất cả nhân viên đang làm việc.
                </div>
              </form>
            </div>

            <div className="kpi-modal-footer">
              <button type="button" className="kpi-btn kpi-btn-secondary" onClick={() => setIsModalOpen(false)} disabled={isSubmitting}>
                Hủy bỏ
              </button>
              <button type="submit" form="kpi-form" className="kpi-btn kpi-btn-primary" disabled={isSubmitting}>
                {isSubmitting ? 'Đang tạo...' : 'Tạo kỳ'}
              </button>
            </div>
          </div>
        </div>
      )}

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
    </div>
  );
};
