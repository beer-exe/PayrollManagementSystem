import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { usePhieuDanhGia } from '../hooks/usePhieuDanhGia';
import './CompetencyManagement.css';

export const TuDanhGiaForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { detail, loading, fetchById, submitTuDanhGia } = usePhieuDanhGia();
  
  const [formData, setFormData] = useState<Record<string, string | number>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (id) {
      fetchById(id);
    }
  }, [id, fetchById]);

  useEffect(() => {
    if (detail && detail.chiTietDanhGias) {
      const initialValues: Record<string, string | number> = {};
      detail.chiTietDanhGias.forEach(c => {
        initialValues[`diem_${c.idChiTiet}`] = c.diemTuDanhGia ?? '';
        initialValues[`nhanXet_${c.idChiTiet}`] = c.nhanXetNhanVien || '';
      });
      setFormData(initialValues);
    }
  }, [detail]);

  const handleChange = (idChiTiet: string, field: 'diem' | 'nhanXet', value: string | number) => {
    setFormData(prev => ({ ...prev, [`${field}_${idChiTiet}`]: value }));
  };

  const handleSave = async (isSubmit: boolean) => {
    if (!detail) return;

    if (isSubmit) {
      // Validate all scores
      const missingScores = detail.chiTietDanhGias.some(c => 
        formData[`diem_${c.idChiTiet}`] === '' || formData[`diem_${c.idChiTiet}`] == null
      );
      if (missingScores) {
        alert("Vui lòng chấm điểm cho tất cả các tiêu chí trước khi gửi duyệt.");
        return;
      }
    }

    setIsSubmitting(true);
    try {
      const chiTiets = detail.chiTietDanhGias.map(c => ({
        idChiTiet: c.idChiTiet,
        diemTuDanhGia: Number(formData[`diem_${c.idChiTiet}`] || 0),
        nhanXetNhanVien: String(formData[`nhanXet_${c.idChiTiet}`] || '')
      }));

      const success = await submitTuDanhGia({
        idPhieu: detail.idPhieu,
        isSubmit,
        chiTiets
      });

      if (success) {
        navigate('/dashboard/danh-gia/tu-danh-gia');
      }
    } catch (e) {
      console.error(e);
      alert("Đã xảy ra lỗi khi lưu phiếu.");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (loading && !detail) {
    return (
      <div className="cp2-container" style={{ justifyContent: 'center', alignItems: 'center' }}>
        <div className="cp2-loader"><div className="cp2-spinner"></div></div>
      </div>
    );
  }

  if (!detail && !loading) {
    return (
      <div className="cp2-container" style={{ justifyContent: 'center', alignItems: 'center' }}>
        <div className="cp2-empty" style={{ width: '100%', maxWidth: '400px' }}>
          <h3>Không tìm thấy phiếu</h3>
          <p>Phiếu đánh giá không tồn tại hoặc bạn không có quyền truy cập.</p>
          <button className="cp2-btn cp2-btn-secondary mt-4" onClick={() => navigate(-1)}>Quay lại</button>
        </div>
      </div>
    );
  }

  const isEditable = detail?.trangThai === 'CHO_NV_DANH_GIA';
  
  let badgeClass = "cp2-badge-gray";
  if (detail?.trangThai === 'CHO_NV_DANH_GIA') badgeClass = "cp2-badge-blue";
  if (detail?.trangThai === 'CHO_QL_DANH_GIA') badgeClass = "cp2-badge-warning";
  if (detail?.trangThai === 'DA_HOAN_THANH') badgeClass = "cp2-badge-success";

  return (
    <div className="cp2-container">
      <div className="cp2-controls-wrapper" style={{ flex: 'none', height: '100%', display: 'flex', flexDirection: 'column' }}>
        
        <div className="cp2-header" style={{ padding: '1.5rem', marginBottom: 0, borderBottom: '1px solid var(--border-color)' }}>
          <div className="cp2-header-title">
            <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
              <button 
                className="cp2-btn-actions" 
                onClick={() => navigate('/dashboard/danh-gia/tu-danh-gia')}
                style={{ padding: '0.25rem' }}
                title="Quay lại"
              >
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18" />
                </svg>
              </button>
              <h2 style={{ margin: 0, fontSize: '1.25rem' }}>Phiếu tự đánh giá: {detail?.tenKyDanhGia}</h2>
              <span className={`cp2-badge ${badgeClass}`}>{detail?.trangThai}</span>
            </div>
            <p style={{ marginLeft: '2.5rem' }}>Hãy đánh giá khách quan về năng lực của bạn trong kỳ này</p>
          </div>
        </div>

        <div className="cp2-table-container custom-scrollbar" style={{ padding: '1.5rem', paddingBottom: 0 }}>
          <table className="cp2-table" style={{ border: '1px solid var(--border-color)', borderRadius: '8px' }}>
            <thead>
              <tr>
                <th style={{ width: '25%' }}>Tiêu chí</th>
                <th style={{ width: '30%' }}>Mô tả</th>
                <th style={{ width: '10%', textAlign: 'center' }}>Tỷ trọng</th>
                <th style={{ width: '15%' }}>Tự đánh giá</th>
                <th style={{ width: '20%' }}>Nhận xét cá nhân</th>
              </tr>
            </thead>
            <tbody>
              {detail?.chiTietDanhGias.map((c) => (
                <tr key={c.idChiTiet}>
                  <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{c.tenNangLuc}</td>
                  <td style={{ fontSize: '0.85rem' }}>{c.moTa || '-'}</td>
                  <td style={{ textAlign: 'center' }}>
                    <span className="cp2-badge cp2-badge-blue">
                      {Number((c.tyTrong * 100).toFixed(1))}%
                    </span>
                  </td>
                  <td>
                    <input
                      type="number"
                      min="0"
                      max="10"
                      step="0.5"
                      disabled={!isEditable}
                      value={formData[`diem_${c.idChiTiet}`] ?? ''}
                      onChange={(e) => handleChange(c.idChiTiet, 'diem', e.target.value)}
                      className="cp2-form-input"
                      placeholder="0-10"
                      style={{ padding: '0.4rem' }}
                    />
                  </td>
                  <td>
                    <textarea
                      disabled={!isEditable}
                      value={formData[`nhanXet_${c.idChiTiet}`] ?? ''}
                      onChange={(e) => handleChange(c.idChiTiet, 'nhanXet', e.target.value)}
                      className="cp2-form-textarea"
                      placeholder="Giải trình thêm..."
                      style={{ minHeight: '60px', padding: '0.4rem' }}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          
          {!isEditable && (
            <div style={{ 
              marginTop: '1.5rem', 
              padding: '1.5rem', 
              backgroundColor: 'var(--bg-hover)', 
              border: '1px solid var(--border-color)', 
              borderRadius: '8px' 
            }}>
              <h3 style={{ fontSize: '1rem', fontWeight: 600, marginBottom: '1rem', color: 'var(--text-primary)' }}>
                Kết quả đánh giá từ Quản lý
              </h3>
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1rem', marginBottom: '1rem' }}>
                <div style={{ background: 'var(--bg-surface)', padding: '1rem', borderRadius: '8px', border: '1px solid var(--border-color)' }}>
                  <p style={{ margin: '0 0 0.25rem 0', color: 'var(--text-secondary)', fontSize: '0.875rem' }}>Điểm tổng hợp</p>
                  <p style={{ margin: 0, fontWeight: 700, fontSize: '1.25rem', color: 'var(--text-primary)' }}>{detail?.diemTongHop ?? 'Chưa chấm'}</p>
                </div>
                <div style={{ background: 'var(--bg-surface)', padding: '1rem', borderRadius: '8px', border: '1px solid var(--border-color)' }}>
                  <p style={{ margin: '0 0 0.25rem 0', color: 'var(--text-secondary)', fontSize: '0.875rem' }}>Hệ số P2</p>
                  <p style={{ margin: 0, fontWeight: 700, fontSize: '1.25rem', color: 'var(--primary)' }}>{detail?.heSoP2 ?? 'Chưa chấm'}</p>
                </div>
                <div style={{ background: 'var(--bg-surface)', padding: '1rem', borderRadius: '8px', border: '1px solid var(--border-color)' }}>
                  <p style={{ margin: '0 0 0.25rem 0', color: 'var(--text-secondary)', fontSize: '0.875rem' }}>Xếp loại</p>
                  <p style={{ margin: 0, fontWeight: 700, fontSize: '1.25rem', color: 'var(--success-text)' }}>{detail?.xepLoai ?? 'Chưa xếp loại'}</p>
                </div>
              </div>
              <div style={{ background: 'var(--bg-surface)', padding: '1rem', borderRadius: '8px', border: '1px solid var(--border-color)' }}>
                <p style={{ margin: '0 0 0.25rem 0', color: 'var(--text-secondary)', fontSize: '0.875rem' }}>Nhận xét chung của Quản lý</p>
                <p style={{ margin: 0, color: 'var(--text-primary)' }}>{detail?.nhanXetChung || <i>(Không có nhận xét)</i>}</p>
              </div>
            </div>
          )}
        </div>

        {isEditable && (
          <div style={{ 
            padding: '1.5rem', 
            borderTop: '1px solid var(--border-color)', 
            display: 'flex', 
            justifyContent: 'flex-end', 
            gap: '1rem',
            backgroundColor: 'var(--bg-surface)',
            marginTop: 'auto'
          }}>
            <button 
              className="cp2-btn cp2-btn-secondary" 
              onClick={() => handleSave(false)} 
              disabled={isSubmitting}
            >
              Lưu nháp
            </button>
            <button 
              className="cp2-btn cp2-btn-primary" 
              onClick={() => handleSave(true)} 
              disabled={isSubmitting}
            >
              {isSubmitting ? 'Đang gửi...' : 'Gửi Quản lý duyệt'}
            </button>
          </div>
        )}
      </div>
    </div>
  );
};
