import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { usePhieuDanhGia } from '../hooks/usePhieuDanhGia';
import './CompetencyManagement.css';

export const DuyetDanhGia: React.FC = () => {
  const { data, loading, fetchManagerEvaluations } = usePhieuDanhGia();
  const navigate = useNavigate();

  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;
  const totalItems = data.length;
  const totalPages = Math.ceil(totalItems / pageSize);
  const currentData = data.slice((currentPage - 1) * pageSize, currentPage * pageSize);

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
        <div className="cp2-table-container custom-scrollbar">
          {loading ? (
            <div className="cp2-loader">
              <div className="cp2-spinner"></div>
            </div>
          ) : data.length > 0 ? (
            <table className="cp2-table">
              <thead>
                <tr>
                  <th>Kỳ đánh giá</th>
                  <th>CCCD Nhân viên</th>
                  <th style={{ textAlign: 'center' }}>Trạng thái</th>
                  <th style={{ textAlign: 'center' }}>Điểm tổng hợp</th>
                  <th style={{ textAlign: 'center' }}>Xếp loại</th>
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
