import React, { useState, useEffect } from 'react';
import { kpiApi } from '../api/kpiApi';
import { PhieuKpi } from '../types/kpi.types';
import { KpiDetailModal } from './KpiDetailModal';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { Toast } from '../../../components/Toast/Toast';
import { useAuthStore } from '@/store/useAuthStore';
import './kpi.css';

export const MyKpi: React.FC = () => {
  const { user } = useAuthStore();
  const [phieuKpis, setPhieuKpis] = useState<PhieuKpi[]>([]);
  const [selectedPhieuId, setSelectedPhieuId] = useState<string | null>(null);
  const [isLoadingData, setIsLoadingData] = useState(false);
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
  } = useDataTable<PhieuKpi>({
    data: phieuKpis,
    initialPageSize: 10,
    searchableFields: ['tenKyKpi', 'trangThai']
  });

  useEffect(() => {
    if (user?.id) {
      fetchMyKpi();
    }
  }, [user]);

  const fetchMyKpi = async () => {
    setIsLoadingData(true);
    try {
      if (user?.id) {
        const response = await kpiApi.getPhieuKpisByTaiKhoan(user.id);
        setPhieuKpis(response.data);
      }
    } catch (error: any) {
      console.error('Lỗi khi tải KPI cá nhân:', error);
      setToast({ message: error.response?.data?.Message || error.response?.data?.message || 'Không thể tải KPI cá nhân', type: 'error' });
    } finally {
      setIsLoadingData(false);
    }
  };

  const handleExportExcel = () => {
    const columns: ExportColumn<PhieuKpi>[] = [
      { header: 'Kỳ đánh giá', key: 'tenKyKpi' },
      { header: 'Tổng điểm (%)', key: 'tongDiemKpi' },
      { header: 'Hệ số P3', key: 'heSoP3' },
      { header: 'Trạng thái', key: 'trangThai' }
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'MyKpi');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<PhieuKpi>[] = [
      { header: 'Kỳ đánh giá', key: 'tenKyKpi' },
      { header: 'Tổng điểm (%)', key: 'tongDiemKpi' },
      { header: 'Hệ số P3', key: 'heSoP3' },
      { header: 'Trạng thái', key: 'trangThai' }
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'MyKpi', 'KPI Của Tôi');
  };

  return (
    <div className="kpi-container">
      <div className="kpi-header">
        <div className="kpi-header-title">
          <h2>📊 KPI Của Tôi</h2>
          <p>Xem lịch sử đánh giá và cập nhật mục tiêu KPI cá nhân</p>
        </div>
      </div>

      <div className="kpi-controls-wrapper">
        <div style={{ display: 'flex', justifyContent: 'space-between', padding: '1rem', borderBottom: '1px solid var(--border-color)', gap: '1rem', flexWrap: 'wrap' }}>
          <div className="kpi-input-wrapper" style={{ flex: 1, minWidth: '250px', position: 'relative' }}>
            <input
              type="text"
              placeholder="Tìm kiếm phiếu KPI..."
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
                  <SortableHeader label="Kỳ đánh giá" sortKey="tenKyKpi" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                  <SortableHeader label="Tổng điểm (%)" sortKey="tongDiemKpi" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <SortableHeader label="Hệ số KPI" sortKey="heSoP3" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <SortableHeader label="Trạng thái" sortKey="trangThai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <th style={{ textAlign: 'center' }}>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {currentData.map((phieu) => {
                  let badgeClass = "kpi-badge-gray";
                  if (phieu.trangThaiValue === 1) badgeClass = "kpi-badge-blue";
                  if (phieu.trangThaiValue === 2) badgeClass = "kpi-badge-warning";
                  if (phieu.trangThaiValue === 3) badgeClass = "kpi-badge-success";

                  return (
                    <tr key={phieu.idPhieuKpi}>
                      <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{phieu.tenKyKpi}</td>
                      <td style={{ textAlign: 'center' }}>
                        {phieu.trangThaiValue >= 2 ? `${phieu.tongDiemKpi}%` : '--'}
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        {phieu.trangThaiValue >= 2 ? phieu.heSoP3 : '--'}
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        <span className={`kpi-badge ${badgeClass}`}>
                          {phieu.trangThai}
                        </span>
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        <button 
                          className="kpi-btn kpi-btn-secondary"
                          style={{ padding: '0.35rem 0.75rem', fontSize: '0.85rem' }}
                          onClick={() => setSelectedPhieuId(phieu.idPhieuKpi)}
                        >
                          {phieu.trangThaiValue <= 1 ? 'Cập nhật' : 'Xem chi tiết'}
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          ) : (
            <div className="kpi-empty">
              <p>Chưa có phiếu KPI nào được giao.</p>
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

      {selectedPhieuId && (
        <KpiDetailModal 
          idPhieuKpi={selectedPhieuId}
          isManagerView={false}
          onClose={() => setSelectedPhieuId(null)}
          onSuccess={(msg?: string) => {
            setSelectedPhieuId(null);
            fetchMyKpi();
            setToast({ message: msg || 'Lưu phiếu KPI thành công!', type: 'success' });
          }}
        />
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
