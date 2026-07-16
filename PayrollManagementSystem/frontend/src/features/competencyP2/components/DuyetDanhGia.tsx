import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { usePhieuDanhGia } from '../hooks/usePhieuDanhGia';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import './CompetencyManagement.css';

export const DuyetDanhGia: React.FC = () => {
  const { data, loading, fetchManagerEvaluations } = usePhieuDanhGia();
  const navigate = useNavigate();

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
  } = useDataTable<any>({
    data: data,
    initialPageSize: 10,
    searchableFields: ['tenKyDanhGia', 'cccdNhanVien']
  });

  const handleExportExcel = () => {
    const columns: ExportColumn<any>[] = [
      { header: 'Kỳ đánh giá', key: 'tenKyDanhGia' },
      { header: 'CCCD Nhân viên', key: 'cccdNhanVien' },
      { header: 'Trạng thái', key: 'tenTrangThai' },
      { header: 'Điểm tổng hợp', key: 'diemTongHop' },
      { header: 'Xếp loại', key: 'xepLoai' },
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'DuyetDanhGia');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<any>[] = [
      { header: 'Kỳ đánh giá', key: 'tenKyDanhGia' },
      { header: 'CCCD Nhân viên', key: 'cccdNhanVien' },
      { header: 'Trạng thái', key: 'tenTrangThai' },
      { header: 'Điểm tổng hợp', key: 'diemTongHop' },
      { header: 'Xếp loại', key: 'xepLoai' },
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'DuyetDanhGia', 'Danh sách duyệt đánh giá');
  };

  useEffect(() => {
    fetchManagerEvaluations();
  }, [fetchManagerEvaluations]);

  return (
    <div className="cp2-container">
      <div className="cp2-header">
        <div className="cp2-header-title">
          <h2>Danh sách & Duyệt đánh giá</h2>
          <p>Quản lý và đánh giá phiếu năng lực của nhân viên cấp dưới</p>
        </div>
      </div>

      <div className="cp2-controls-wrapper">
        <div style={{ display: 'flex', justifyContent: 'space-between', padding: '1rem', borderBottom: '1px solid var(--border-color)', gap: '1rem', flexWrap: 'wrap' }}>
          <div className="cp2-input-wrapper" style={{ flex: 1, minWidth: '250px', position: 'relative' }}>
            <input
              type="text"
              placeholder="Tìm kiếm phiếu duyệt..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="cp2-select"
              style={{ width: '100%', paddingLeft: '0.75rem' }}
            />
          </div>
          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
        </div>
        <div className="cp2-table-container custom-scrollbar">
          {loading ? (
            <div className="cp2-loader">
              <div className="cp2-spinner"></div>
            </div>
          ) : data.length > 0 ? (
            <table className="cp2-table">
              <thead>
                <tr>
                  <SortableHeader label="Kỳ đánh giá" sortKey="tenKyDanhGia" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                  <SortableHeader label="CCCD Nhân viên" sortKey="cccdNhanVien" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                  <SortableHeader label="Trạng thái" sortKey="trangThai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <SortableHeader label="Điểm tổng hợp" sortKey="diemTongHop" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <SortableHeader label="Xếp loại" sortKey="xepLoai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ textAlign: 'center' }} />
                  <th style={{ textAlign: 'right' }}>Hành động</th>
                </tr>
              </thead>
              <tbody>
                {currentData.map(record => {
                  let badgeClass = "cp2-badge-gray";
                  if (record.trangThai === 'CHO_QL_DANH_GIA') badgeClass = "cp2-badge-blue";
                  if (record.trangThai === 'DA_HOAN_THANH') badgeClass = "cp2-badge-success";

                  return (
                    <tr key={record.idPhieu}>
                      <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{record.tenKyDanhGia}</td>
                      <td>{record.cccdNhanVien}</td>
                      <td style={{ textAlign: 'center' }}>
                        <span className={`cp2-badge ${badgeClass}`}>
                          {record.tenTrangThai || record.trangThai}
                        </span>
                      </td>
                      <td style={{ textAlign: 'center', fontWeight: 600 }}>
                        {record.diemTongHop !== null ? record.diemTongHop : '-'}
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        {record.xepLoai ? (
                          <span className="cp2-badge cp2-badge-gray" style={{ fontWeight: 700 }}>
                            {record.xepLoai}
                          </span>
                        ) : '-'}
                      </td>
                      <td style={{ textAlign: 'right' }}>
                        <button 
                          className={`cp2-btn ${record.canEvaluate && record.trangThai === 'CHO_QL_DANH_GIA' ? 'cp2-btn-primary' : 'cp2-btn-secondary'}`}
                          style={{ padding: '0.4rem 1rem' }}
                          onClick={() => navigate(`/performance/duyet-danh-gia/${record.idPhieu}`)}
                        >
                          {record.canEvaluate && record.trangThai === 'CHO_QL_DANH_GIA' ? 'Chấm điểm' : 'Xem chi tiết'}
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          ) : (
            <div className="cp2-empty">
              <p>Chưa có phiếu đánh giá nào cần duyệt.</p>
            </div>
          )}
        </div>

        {totalPages > 0 && (
          <div className="cp2-pagination">
            <button 
              className="cp2-btn cp2-btn-secondary" 
              onClick={() => setCurrentPage(p => p - 1)} 
              disabled={currentPage === 1 || loading}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              Trước
            </button>
            <div className="cp2-pagination-info">
              Trang <span>{currentPage}</span> / <span>{totalPages}</span>
            </div>
            <button 
              className="cp2-btn cp2-btn-secondary" 
              onClick={() => setCurrentPage(p => p + 1)} 
              disabled={currentPage === totalPages || loading}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              Sau
            </button>
          </div>
        )}
      </div>
    </div>
  );
};
