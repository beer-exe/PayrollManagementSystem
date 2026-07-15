import React, { useEffect, useState } from 'react';
import { usePhieuDanhGia } from '../hooks/usePhieuDanhGia';
import { useKyDanhGia } from '../hooks/useKyDanhGia';
import { useNavigate } from 'react-router-dom';
import './CompetencyManagement.css';

export const TuDanhGia: React.FC = () => {
  const { data: myForms, loading: formLoading, fetchMyEvaluations, generate } = usePhieuDanhGia();
  const { data: kyDanhGias, loading: kyLoading, fetchKyDanhGia } = useKyDanhGia();
  const navigate = useNavigate();

  // Pagination for my forms
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;
  const totalItems = myForms.length;
  const totalPages = Math.ceil(totalItems / pageSize);
  const currentForms = myForms.slice((currentPage - 1) * pageSize, currentPage * pageSize);

  useEffect(() => {
    fetchMyEvaluations();
    fetchKyDanhGia();
  }, [fetchMyEvaluations, fetchKyDanhGia]);

  const handleGenerate = (idKyDanhGia: string) => {
    if (window.confirm('Bạn có chắc muốn tạo phiếu đánh giá cho kỳ này không? Các tiêu chí sẽ được lấy tự động dựa vào chức vụ hiện tại của bạn.')) {
      generate({ idKyDanhGia }).then(success => {
        if (success) fetchMyEvaluations();
      });
    }
  };

  const activeKys = kyDanhGias.filter(x => x.trangThai === 'DANG_DANH_GIA');

  return (
    <div className="cp2-container" style={{ gap: '1.5rem', overflowY: 'auto' }}>
      
      {/* Active Periods Section */}
      <div className="cp2-controls-wrapper" style={{ flex: 'none' }}>
        <div className="cp2-header" style={{ padding: '1.5rem', marginBottom: 0, borderBottom: '1px solid var(--border-color)' }}>
          <div className="cp2-header-title">
            <h2>Kỳ đánh giá đang mở</h2>
            <p>Danh sách các kỳ đánh giá bạn có thể tham gia</p>
          </div>
        </div>
        
        <div style={{ padding: '1.5rem' }}>
          {kyLoading ? (
            <div className="cp2-loader"><div className="cp2-spinner"></div></div>
          ) : activeKys.length === 0 ? (
            <div className="cp2-empty" style={{ padding: '2rem', margin: 0 }}>
              <p>Không có kỳ đánh giá nào đang mở.</p>
            </div>
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
              {activeKys.map(ky => {
                const hasForm = myForms.find(f => f.idKyDanhGia === ky.idKyDanhGia);
                return (
                  <div key={ky.idKyDanhGia} style={{ 
                    display: 'flex', 
                    justifyContent: 'space-between', 
                    alignItems: 'center', 
                    padding: '1.25rem', 
                    border: '1px solid var(--border-color)', 
                    borderRadius: '12px', 
                    backgroundColor: 'var(--bg-surface)',
                    flexWrap: 'wrap',
                    gap: '1rem'
                  }}>
                    <div>
                      <h4 style={{ margin: '0 0 0.25rem 0', fontWeight: 600, color: 'var(--text-primary)', fontSize: '1rem' }}>
                        {ky.tenKyDanhGia}
                      </h4>
                      <p style={{ margin: 0, color: 'var(--text-secondary)', fontSize: '0.85rem' }}>
                        {ky.ngayBatDau} - {ky.ngayKetThuc}
                      </p>
                    </div>
                    <div>
                      {!hasForm ? (
                        <button 
                          className="cp2-btn cp2-btn-primary" 
                          onClick={() => handleGenerate(ky.idKyDanhGia)}
                        >
                          Tạo phiếu đánh giá
                        </button>
                      ) : (
                        <span className="cp2-badge cp2-badge-success">Đã có phiếu</span>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>

      {/* My Evaluations Section */}
      <div className="cp2-controls-wrapper">
        <div className="cp2-header" style={{ padding: '1.5rem', marginBottom: 0, borderBottom: '1px solid var(--border-color)' }}>
          <div className="cp2-header-title">
            <h2>Danh sách phiếu đánh giá của tôi</h2>
            <p>Lịch sử tự đánh giá năng lực của bạn</p>
          </div>
        </div>

        <div className="cp2-table-container custom-scrollbar">
          {formLoading ? (
            <div className="cp2-loader"><div className="cp2-spinner"></div></div>
          ) : myForms.length > 0 ? (
            <table className="cp2-table">
              <thead>
                <tr>
                  <th>Kỳ đánh giá</th>
                  <th style={{ textAlign: 'center' }}>Điểm tổng hợp</th>
                  <th style={{ textAlign: 'center' }}>Hệ số P2</th>
                  <th style={{ textAlign: 'center' }}>Xếp loại</th>
                  <th style={{ textAlign: 'center' }}>Trạng thái</th>
                  <th style={{ textAlign: 'right' }}>Hành động</th>
                </tr>
              </thead>
              <tbody>
                {currentForms.map(record => {
                  let badgeClass = "cp2-badge-gray";
                  if (record.trangThai === 'CHO_NV_DANH_GIA') badgeClass = "cp2-badge-blue";
                  if (record.trangThai === 'CHO_QL_DANH_GIA') badgeClass = "cp2-badge-warning";
                  if (record.trangThai === 'DA_HOAN_THANH') badgeClass = "cp2-badge-success";

                  return (
                    <tr key={record.idPhieu}>
                      <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{record.tenKyDanhGia}</td>
                      <td style={{ textAlign: 'center', fontWeight: 600 }}>{record.diemTongHop ?? '-'}</td>
                      <td style={{ textAlign: 'center', fontWeight: 600, color: 'var(--primary)' }}>{record.heSoP2 ?? '-'}</td>
                      <td style={{ textAlign: 'center' }}>
                        {record.xepLoai ? (
                          <span className="cp2-badge cp2-badge-gray" style={{ fontWeight: 700 }}>
                            {record.xepLoai}
                          </span>
                        ) : '-'}
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        <span className={`cp2-badge ${badgeClass}`}>
                          {record.tenTrangThai || record.trangThai}
                        </span>
                      </td>
                      <td style={{ textAlign: 'right' }}>
                        <button 
                          className={`cp2-btn ${record.trangThai === 'CHO_NV_DANH_GIA' ? 'cp2-btn-primary' : 'cp2-btn-secondary'}`}
                          style={{ padding: '0.4rem 1rem' }}
                          onClick={() => navigate(`/dashboard/danh-gia/tu-danh-gia/${record.idPhieu}`)}
                        >
                          {record.trangThai === 'CHO_NV_DANH_GIA' ? 'Làm phiếu' : 'Xem chi tiết'}
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          ) : (
            <div className="cp2-empty">
              <p>Bạn chưa có phiếu đánh giá nào.</p>
            </div>
          )}
        </div>

        {totalPages > 0 && (
          <div className="cp2-pagination">
            <button 
              className="cp2-btn cp2-btn-secondary" 
              onClick={() => setCurrentPage(p => p - 1)} 
              disabled={currentPage === 1 || formLoading}
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
              disabled={currentPage === totalPages || formLoading}
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
