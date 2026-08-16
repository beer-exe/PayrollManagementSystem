import React, { useState, useEffect } from 'react';
import { kpiApi } from '../api/kpiApi';
import { KyKpi, PhieuKpi } from '../types/kpi.types';
import { KpiDetailModal } from './KpiDetailModal';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { Toast } from '../../../components/Toast/Toast';
import './kpi.css';

export const KpiApprovalList: React.FC = () => {
  const [kyKpis, setKyKpis] = useState<KyKpi[]>([]);
  const [selectedKyId, setSelectedKyId] = useState<string>('');
  const [phieuKpis, setPhieuKpis] = useState<PhieuKpi[]>([]);
  const [selectedPhieuId, setSelectedPhieuId] = useState<string | null>(null);
  const [isLoadingKy, setIsLoadingKy] = useState(false);
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
    searchableFields: ['tenNhanVien', 'cccdNhanVien', 'trangThai']
  });

  useEffect(() => {
    fetchKyKpis();
  }, []);

  useEffect(() => {
    if (selectedKyId) {
      fetchPhieuKpis(selectedKyId);
    } else {
      setPhieuKpis([]);
    }
  }, [selectedKyId]);

  const fetchKyKpis = async () => {
    setIsLoadingKy(true);
    try {
      const response = await kpiApi.getKyKpis();
      setKyKpis(response.data);
      if (response.data.length > 0) {
        setSelectedKyId(response.data[0].idKyKpi);
      }
    } catch (error) {
      console.error('Lỗi khi tải danh sách kỳ KPI:', error);
      setToast({ message: 'Không thể tải danh sách kỳ đánh giá', type: 'error' });
    } finally {
      setIsLoadingKy(false);
    }
  };

  const fetchPhieuKpis = async (kyId: string) => {
    setIsLoadingData(true);
    try {
      const response = await kpiApi.getPhieuKpisByKy(kyId);
      setPhieuKpis(response.data);
    } catch (error: any) {
      console.error('Lỗi khi tải danh sách phiếu KPI:', error);
      setToast({ message: error.response?.data?.Message || error.response?.data?.message || 'Không thể tải danh sách phiếu KPI', type: 'error' });
    } finally {
      setIsLoadingData(false);
    }
  };

  const handleExportExcel = () => {
    const columns: ExportColumn<PhieuKpi>[] = [
      { header: 'Họ và tên', key: 'tenNhanVien' },
      { header: 'CCCD', key: 'cccdNhanVien' },
      { header: 'Tổng điểm (%)', key: 'tongDiemKpi' },
      { header: 'Hệ số P3', key: 'heSoP3' },
      { header: 'Trạng thái', key: 'trangThai' }
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'PheDuyetKpi');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<PhieuKpi>[] = [
      { header: 'Họ và tên', key: 'tenNhanVien' },
      { header: 'CCCD', key: 'cccdNhanVien' },
      { header: 'Tổng điểm (%)', key: 'tongDiemKpi' },
      { header: 'Hệ số P3', key: 'heSoP3' },
      { header: 'Trạng thái', key: 'trangThai' }
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'PheDuyetKpi', 'Danh sách Phê Duyệt KPI');
  };

  return (
    <div className="kpi-container">
      <div className="kpi-header">
        <div className="kpi-header-title">
          <h2>✅ Phê Duyệt KPI</h2>
          <p>Duyệt phiếu đánh giá KPI và chốt hệ số KPI cho nhân viên</p>
        </div>
      </div>

      <div className="kpi-controls-wrapper" style={{ flex: 'none', marginBottom: '1.5rem', minHeight: 'auto' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', padding: '1rem' }}>
          <label style={{ fontWeight: 600, color: 'var(--text-primary)', whiteSpace: 'nowrap' }}>Chọn kỳ đánh giá:</label>
          {isLoadingKy ? (
            <div className="kpi-spinner" style={{ width: '1.5rem', height: '1.5rem', borderWidth: '2px' }}></div>
          ) : (
            <select 
              className="kpi-form-input" 
              style={{ width: '300px', cursor: 'pointer' }}
              value={selectedKyId}
              onChange={(e) => setSelectedKyId(e.target.value)}
            >
              <option value="">-- Chọn kỳ KPI --</option>
              {kyKpis.map(k => (
                <option key={k.idKyKpi} value={k.idKyKpi}>{k.tenKyKpi} ({k.thang}/{k.nam})</option>
              ))}
            </select>
          )}
        </div>
      </div>

      <div className="kpi-controls-wrapper">
        <div style={{ display: 'flex', justifyContent: 'space-between', padding: '1rem', borderBottom: '1px solid var(--border-color)', gap: '1rem', flexWrap: 'wrap' }}>
          <div className="kpi-input-wrapper" style={{ flex: 1, minWidth: '250px', position: 'relative' }}>
            <input
              type="text"
              placeholder="Tìm kiếm nhân viên (Tên, CCCD) hoặc trạng thái..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="kpi-form-input"
              style={{ width: '100%', paddingLeft: '0.75rem' }}
              disabled={!selectedKyId}
            />
          </div>
          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
        </div>

        <div className="kpi-table-container custom-scrollbar">
          {isLoadingData ? (
            <div className="kpi-loader">
              <div className="kpi-spinner"></div>
            </div>
          ) : !selectedKyId ? (
            <div className="kpi-empty">
              <p>Vui lòng chọn kỳ đánh giá để xem danh sách phiếu KPI.</p>
            </div>
          ) : currentData.length > 0 ? (
            <table className="kpi-table">
              <thead>
                <tr>
                  <SortableHeader label="Nhân viên" sortKey="tenNhanVien" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                  <SortableHeader label="Tổng điểm" sortKey="tongDiemKpi" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <SortableHeader label="Hệ số P3" sortKey="heSoP3" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
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
                      <td>
                        <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{phieu.tenNhanVien}</div>
                        <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>{phieu.cccdNhanVien}</div>
                      </td>
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
                          {phieu.trangThaiValue === 2 ? 'Phê duyệt' : 'Xem chi tiết'}
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          ) : (
            <div className="kpi-empty">
              <p>Kỳ đánh giá này chưa có phiếu KPI nào hoặc không tìm thấy dữ liệu phù hợp.</p>
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
          isManagerView={true}
          onClose={() => setSelectedPhieuId(null)}
          onSuccess={(msg?: string) => {
            setSelectedPhieuId(null);
            fetchPhieuKpis(selectedKyId);
            setToast({ message: msg || 'Lưu thay đổi thành công!', type: 'success' });
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
