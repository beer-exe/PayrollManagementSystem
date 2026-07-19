import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { usePhieuDanhGia } from '../hooks/usePhieuDanhGia';
import { useMucQuyDoi } from '../hooks/useMucQuyDoi';
import './CompetencyManagement.css';

export const DuyetDanhGiaForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { detail, loading, fetchById, submitManagerEvaluation } = usePhieuDanhGia();
  const { data: rules, fetchQuyDoi } = useMucQuyDoi();
  
  const [submitting, setSubmitting] = useState(false);
  const [formData, setFormData] = useState<{
    nhanXetChung: string;
    chiTiets: Record<string, { diem: string | number; nhanXet: string }>;
  }>({
    nhanXetChung: '',
    chiTiets: {}
  });

  useEffect(() => {
    fetchQuyDoi();
  }, [fetchQuyDoi]);

  useEffect(() => {
    if (id) {
      fetchById(id);
    }
  }, [id, fetchById]);

  useEffect(() => {
    if (detail) {
      const initialChiTiets: Record<string, { diem: string | number; nhanXet: string }> = {};
      detail.chiTietDanhGias.forEach(item => {
        initialChiTiets[item.idChiTiet] = {
          diem: item.diemQuanLyDanhGia !== null ? item.diemQuanLyDanhGia : (item.diemTuDanhGia !== null ? item.diemTuDanhGia : ''),
          nhanXet: item.nhanXetQuanLy || ''
        };
      });
      
      setFormData({
        nhanXetChung: detail.nhanXetChung || '',
        chiTiets: initialChiTiets
      });
    }
  }, [detail]);

  const handleChiTietChange = (idChiTiet: string, field: 'diem' | 'nhanXet', value: string | number) => {
    setFormData(prev => ({
      ...prev,
      chiTiets: {
        ...prev.chiTiets,
        [idChiTiet]: {
          ...prev.chiTiets[idChiTiet],
          [field]: value
        }
      }
    }));
  };

  const onFinish = async (isSubmit: boolean) => {
    if (!id || !detail) return;
    
    if (isSubmit) {
      const missingScores = detail.chiTietDanhGias.some(c => {
        const val = formData.chiTiets[c.idChiTiet]?.diem;
        return val === '' || val == null;
      });
      if (missingScores) {
        alert("Vui lòng chấm điểm cho tất cả các tiêu chí trước khi chốt đánh giá.");
        return;
      }
    }

    setSubmitting(true);
    try {
      const chiTiets = detail.chiTietDanhGias.map(c => ({
        idChiTiet: c.idChiTiet,
        diemQuanLyDanhGia: Number(formData.chiTiets[c.idChiTiet]?.diem || 0),
        nhanXetQuanLy: String(formData.chiTiets[c.idChiTiet]?.nhanXet || '')
      }));

      const success = await submitManagerEvaluation({
        idPhieu: id,
        isSubmit,
        nhanXetChung: formData.nhanXetChung,
        chiTiets
      });

      if (success) {
        navigate('/performance/duyet-danh-gia');
      }
    } catch (e) {
      console.error(e);
      alert("Đã xảy ra lỗi khi lưu đánh giá.");
    } finally {
      setSubmitting(false);
    }
  };

  if (loading || !detail) {
    return (
      <div className="cp2-container" style={{ justifyContent: 'center', alignItems: 'center' }}>
        <div className="cp2-loader"><div className="cp2-spinner"></div></div>
      </div>
    );
  }

  const isEditable = detail.canEvaluate && detail.trangThai === 'CHO_QL_DANH_GIA';


  // Tính điểm Nhân viên
  const empScore = detail.chiTietDanhGias.reduce((sum, item) => sum + (item.diemTuDanhGia || 0) * item.tyTrong, 0);

  // Tính điểm Quản lý (Live)
  const mgrScore = detail.chiTietDanhGias.reduce((sum, item) => {
    const diem = Number(formData.chiTiets[item.idChiTiet]?.diem || 0);
    return sum + diem * item.tyTrong;
  }, 0);

  // Tra cứu bảng quy đổi
  const getQuyDoi = (score: number) => {
    const rule = rules.find(r => score >= r.diemToiThieu && score <= r.diemToiDa);
    if (!rule) return { xepLoai: 'Chưa xác định', heSo: 0 };
    return { xepLoai: rule.xepLoai, heSo: rule.heSoP2 };
  };

  const empResult = getQuyDoi(empScore);
  const mgrResult = getQuyDoi(mgrScore);

  let badgeClass = "cp2-badge-gray";
  if (detail.trangThai === 'CHO_QL_DANH_GIA') badgeClass = "cp2-badge-blue";
  if (detail.trangThai === 'DA_HOAN_THANH') badgeClass = "cp2-badge-success";

  return (
    <div className="cp2-container">
      <div className="cp2-controls-wrapper" style={{ flex: 'none', height: '100%', display: 'flex', flexDirection: 'column' }}>
        
        <div className="cp2-header" style={{ padding: '1.5rem', marginBottom: 0, borderBottom: '1px solid var(--border-color)' }}>
          <div className="cp2-header-title">
            <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
              <button 
                className="cp2-btn-actions" 
                onClick={() => navigate('/performance/duyet-danh-gia')}
                style={{ padding: '0.25rem' }}
                title="Quay lại"
              >
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18" />
                </svg>
              </button>
              <h2 style={{ margin: 0, fontSize: '1.25rem' }}>Duyệt Phiếu Đánh Giá</h2>
              <span className={`cp2-badge ${badgeClass}`}>{detail.tenTrangThai || detail.trangThai}</span>
            </div>
          </div>
        </div>

        <div className="custom-scrollbar" style={{ padding: '1.5rem', overflowY: 'auto' }}>
          
          <div style={{ 
            marginBottom: '1.5rem', 
            padding: '1rem 1.5rem', 
            backgroundColor: 'var(--bg-surface)', 
            border: '1px solid var(--border-color)', 
            borderRadius: '8px',
            display: 'flex',
            gap: '2rem',
            flexWrap: 'wrap'
          }}>
            <div>
              <span style={{ color: 'var(--text-secondary)', fontSize: '0.875rem' }}>Kỳ đánh giá:</span>
              <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{detail.tenKyDanhGia}</div>
            </div>
            <div>
              <span style={{ color: 'var(--text-secondary)', fontSize: '0.875rem' }}>CCCD Nhân viên:</span>
              <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{detail.cccdNhanVien}</div>
            </div>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: '1.5rem', marginBottom: '1.5rem' }}>
            
            {/* Nhân viên Tự đánh giá */}
            <div style={{ backgroundColor: 'var(--bg-hover)', border: '1px solid var(--border-color)', borderRadius: '8px', padding: '1.25rem' }}>
              <h3 style={{ margin: '0 0 1rem 0', fontSize: '1rem', color: 'var(--text-primary)', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z" />
                </svg>
                Nhân viên Tự đánh giá
              </h3>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '1rem' }}>
                <div>
                  <span style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', display: 'block' }}>Tổng điểm</span>
                  <strong style={{ fontSize: '1.25rem' }}>{empScore.toFixed(2)}</strong>
                </div>
                <div>
                  <span style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', display: 'block' }}>Hệ số P2</span>
                  <strong style={{ fontSize: '1.25rem', color: 'var(--success-text)' }}>{empResult.heSo}</strong>
                </div>
                <div>
                  <span style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', display: 'block' }}>Xếp loại</span>
                  <span className="cp2-badge cp2-badge-blue">{empResult.xepLoai}</span>
                </div>
              </div>
            </div>

            {/* Quản lý Đánh giá */}
            <div style={{ backgroundColor: 'var(--primary-light)', border: '1px solid var(--primary-light)', borderRadius: '8px', padding: '1.25rem' }}>
              <h3 style={{ margin: '0 0 1rem 0', fontSize: '1rem', color: 'var(--primary)', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                Quản lý Đánh giá (Dự kiến)
              </h3>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '1rem' }}>
                <div>
                  <span style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', display: 'block' }}>Tổng điểm</span>
                  <strong style={{ fontSize: '1.25rem' }}>{mgrScore.toFixed(2)}</strong>
                </div>
                <div>
                  <span style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', display: 'block' }}>Hệ số P2</span>
                  <strong style={{ fontSize: '1.25rem', color: 'var(--primary)' }}>{mgrResult.heSo}</strong>
                </div>
                <div>
                  <span style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', display: 'block' }}>Xếp loại</span>
                  <span className="cp2-badge cp2-badge-blue">{mgrResult.xepLoai}</span>
                </div>
              </div>
            </div>

          </div>

          <table className="cp2-table" style={{ border: '1px solid var(--border-color)', borderRadius: '8px', marginBottom: '1.5rem' }}>
            <thead>
              <tr>
                <th style={{ width: '25%' }}>Tiêu chí Năng lực</th>
                <th style={{ width: '25%' }}>Yêu cầu tối thiểu</th>
                <th style={{ width: '20%' }}>NV Tự đánh giá</th>
                <th style={{ width: '30%' }}>Quản lý Đánh giá</th>
              </tr>
            </thead>
            <tbody>
              {detail.chiTietDanhGias.map((c) => (
                <tr key={c.idChiTiet}>
                  <td>
                    <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{c.tenNangLuc}</div>
                    <div style={{ color: 'var(--text-secondary)', fontSize: '0.75rem', marginTop: '0.25rem' }}>Trọng số: {c.tyTrong}</div>
                  </td>
                  <td style={{ fontSize: '0.85rem' }}>{c.yeuCauToiThieu || '-'}</td>
                  
                  <td>
                    <div style={{ backgroundColor: 'var(--bg-hover)', padding: '0.75rem', borderRadius: '6px', border: '1px solid var(--border-color)' }}>
                      <div style={{ fontWeight: 600, color: 'var(--primary)', marginBottom: '0.25rem' }}>Điểm: {c.diemTuDanhGia ?? '-'}</div>
                      <div style={{ fontSize: '0.85rem' }}>
                        {c.nhanXetNhanVien || <span style={{ fontStyle: 'italic', color: 'var(--text-muted)' }}>Không có nhận xét</span>}
                      </div>
                    </div>
                  </td>

                  <td>
                    <div style={{ backgroundColor: 'var(--primary-light)', padding: '0.75rem', borderRadius: '6px', border: '1px solid var(--primary-light)' }}>
                      <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', marginBottom: '0.5rem' }}>
                        <span style={{ fontWeight: 600, fontSize: '0.85rem', color: 'var(--primary)' }}>Điểm:</span>
                        <input
                          type="number"
                          min="0"
                          max="10"
                          step="0.5"
                          disabled={!isEditable}
                          value={formData.chiTiets[c.idChiTiet]?.diem ?? ''}
                          onChange={(e) => handleChiTietChange(c.idChiTiet, 'diem', e.target.value)}
                          className="cp2-form-input"
                          style={{ padding: '0.25rem 0.5rem', width: '80px' }}
                        />
                      </div>
                      <textarea
                        disabled={!isEditable}
                        value={formData.chiTiets[c.idChiTiet]?.nhanXet ?? ''}
                        onChange={(e) => handleChiTietChange(c.idChiTiet, 'nhanXet', e.target.value)}
                        className="cp2-form-textarea"
                        placeholder="Nhận xét của QL..."
                        style={{ minHeight: '60px', padding: '0.4rem' }}
                      />
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          <div style={{ backgroundColor: 'var(--bg-surface)', border: '1px solid var(--border-color)', borderRadius: '8px', padding: '1.25rem' }}>
            <label style={{ display: 'block', fontWeight: 600, fontSize: '1.1rem', marginBottom: '0.75rem', color: 'var(--text-primary)' }}>
              Nhận xét chung của Quản lý
            </label>
            <textarea
              disabled={!isEditable}
              value={formData.nhanXetChung}
              onChange={(e) => setFormData(prev => ({ ...prev, nhanXetChung: e.target.value }))}
              className="cp2-form-textarea"
              placeholder="Đánh giá tổng quan về nhân viên..."
              style={{ minHeight: '100px' }}
            />
          </div>

        </div>

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
            onClick={() => navigate('/performance/duyet-danh-gia')} 
            disabled={submitting}
          >
            Quay lại
          </button>
          
          {isEditable && (
            <>
              <button 
                className="cp2-btn cp2-btn-secondary" 
                onClick={() => onFinish(false)} 
                disabled={submitting}
              >
                Lưu nháp
              </button>
              <button 
                className="cp2-btn cp2-btn-primary" 
                onClick={() => onFinish(true)} 
                disabled={submitting}
              >
                {submitting ? 'Đang xử lý...' : 'Chốt đánh giá'}
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  );
};
