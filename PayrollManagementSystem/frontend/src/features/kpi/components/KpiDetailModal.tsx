import React, { useState, useEffect } from 'react';
import { kpiApi } from '../api/kpiApi';
import { useAuthStore } from '@/store/useAuthStore';
import { PhieuKpiDetail, ChiTietKpi } from '../types/kpi.types';
import { Toast } from '../../../components/Toast/Toast';

interface KpiDetailModalProps {
  idPhieuKpi: string;
  isManagerView?: boolean;
  onClose: () => void;
  onSuccess: (msg?: string) => void;
}

export const KpiDetailModal: React.FC<KpiDetailModalProps> = ({ idPhieuKpi, isManagerView, onClose, onSuccess }) => {
  const { user } = useAuthStore();
  const [phieu, setPhieu] = useState<PhieuKpiDetail | null>(null);
  const [chiTiet, setChiTiet] = useState<ChiTietKpi[]>([]);
  const [nhanXet, setNhanXet] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' | 'info' } | null>(null);

  useEffect(() => {
    fetchDetail();
  }, [idPhieuKpi]);

  const fetchDetail = async () => {
    try {
      const res = await kpiApi.getPhieuKpiDetail(idPhieuKpi);
      setPhieu(res.data);
      setChiTiet(res.data.chiTietKpis.length > 0 ? res.data.chiTietKpis : [createEmptyRow()]);
      setNhanXet(res.data.nhanXet || '');
    } catch (error) {
      console.error('Lỗi tải chi tiết KPI', error);
      setToast({ message: 'Không thể tải dữ liệu KPI', type: 'error' });
    }
  };

  const createEmptyRow = (): ChiTietKpi => ({
    mucTieu: '',
    donViTinh: '',
    trongSo: 0,
    chiTieu: 0,
    thucTe: 0,
    loaiTieuChiValue: 'CANG_NHIEU_CANG_TOT'
  });

  const handleAddRow = () => {
    setChiTiet([...chiTiet, createEmptyRow()]);
  };

  const handleRemoveRow = (index: number) => {
    setChiTiet(chiTiet.filter((_, i) => i !== index));
  };

  const handleRowChange = (index: number, field: keyof ChiTietKpi, value: any) => {
    const newChiTiet = [...chiTiet];
    newChiTiet[index] = { ...newChiTiet[index], [field]: value };
    setChiTiet(newChiTiet);
  };

  // Quản lý giao KPI (chỉ gọi khi trạng thái = 0 và isManagerView)
  const handleAssignKpi = async () => {
    if (!phieu || !user?.id) return;
    
    // Validate
    const tongTrongSo = chiTiet.reduce((sum, item) => sum + Number(item.trongSo), 0);
    if (tongTrongSo !== 100 && chiTiet.length > 0) {
      setToast({ message: `Tổng trọng số phải bằng 100%. Hiện tại là ${tongTrongSo}%`, type: 'error' });
      return;
    }

    setIsLoading(true);
    try {
      await kpiApi.assignKpi(phieu.idPhieuKpi, {
        chiTietKpis: chiTiet.map(c => ({
          ...c,
          loaiTieuChi: c.loaiTieuChiValue
        }))
      });
      onSuccess('Đã giao KPI cho nhân viên!');
    } catch (error: any) {
      setToast({ message: error.response?.data?.Message || error.response?.data?.message || 'Có lỗi xảy ra', type: 'error' });
    } finally {
      setIsLoading(false);
    }
  };

  // Nhân viên cập nhật tiến độ (chỉ gọi khi trạng thái = 1 và !isManagerView)
  const handleSaveProgress = async (submit: boolean = false) => {
    if (!phieu) return;

    setIsLoading(true);
    try {
      await kpiApi.saveChiTietKpi(phieu.idPhieuKpi, chiTiet.map(c => ({
        ...c,
        loaiTieuChi: c.loaiTieuChiValue
      })));
      if (submit) {
        await kpiApi.submitPhieuKpi(phieu.idPhieuKpi);
        onSuccess('Đã nộp phiếu KPI chờ duyệt!');
      } else {
        onSuccess('Đã lưu tiến độ!');
      }
    } catch (error: any) {
      setToast({ message: error.response?.data?.Message || error.response?.data?.message || 'Có lỗi xảy ra', type: 'error' });
    } finally {
      setIsLoading(false);
    }
  };

  // Quản lý phê duyệt (chỉ gọi khi trạng thái = 2 và isManagerView)
  const handleApprove = async () => {
    if (!phieu) return;
    setIsLoading(true);
    try {
      await kpiApi.approvePhieuKpi(phieu.idPhieuKpi, {
        nhanXet: nhanXet
      });
      onSuccess('Đã phê duyệt KPI!');
    } catch (error: any) {
      setToast({ message: error.response?.data?.Message || error.response?.data?.message || 'Có lỗi xảy ra', type: 'error' });
    } finally {
      setIsLoading(false);
    }
  };

  if (!phieu) return null;

  // Xác định quyền hạn theo trạng thái và vai trò
  const isAssigning = isManagerView && phieu.canManage && phieu.trangThaiValue === 0;
  const isUpdatingProgress = !isManagerView && phieu.trangThaiValue === 1;
  const canApprove = isManagerView && phieu.canManage && phieu.trangThaiValue === 2;

  // Cột nào được phép sửa?
  const isTargetEditable = isAssigning;
  const isActualEditable = isUpdatingProgress;

  return (
    <div className="kpi-modal-overlay">
      <div className="kpi-modal large">
        <div className="kpi-modal-header">
          <h3 className="kpi-modal-title">Chi tiết KPI: {phieu.tenNhanVien} ({phieu.thang}/{phieu.nam})</h3>
          <button className="kpi-modal-close" onClick={onClose}>&times;</button>
        </div>
        
        <div className="kpi-modal-body">
          <div className="kpi-info-grid">
            <div className="kpi-info-item">
              <span className="kpi-info-label">Kỳ Đánh Giá</span>
              <span className="kpi-info-value">{phieu.tenKyKpi}</span>
            </div>
            <div className="kpi-info-item">
              <span className="kpi-info-label">Trạng Thái</span>
              <span className={`kpi-status-badge kpi-status-${phieu.trangThaiValue}`}>
                {phieu.trangThai}
              </span>
            </div>
            <div className="kpi-info-item">
              <span className="kpi-info-label">Hệ Số KPI Hiện Tại</span>
              <span className="kpi-info-value">{phieu.heSoP3}</span>
            </div>
          </div>

          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px' }}>
            <h3 style={{ margin: 0, fontSize: '1rem', fontWeight: 600 }}>Mục tiêu & Kết quả</h3>
            {isAssigning && (
              <button type="button" className="kpi-btn kpi-btn-secondary" onClick={handleAddRow}>
                + Thêm mục tiêu
              </button>
            )}
          </div>

          <div className="kpi-target-list">
            {chiTiet.map((row, index) => (
              <div key={index} className="kpi-target-card">
                <div className="kpi-target-card-header">
                  <div className="kpi-target-field" style={{ flex: 1 }}>
                    <span className="kpi-target-field-label">Mục tiêu</span>
                    {isTargetEditable ? (
                      <input className="kpi-form-input" placeholder="Nhập tên mục tiêu" value={row.mucTieu} onChange={(e) => handleRowChange(index, 'mucTieu', e.target.value)} />
                    ) : <span className="kpi-target-field-value font-medium">{row.mucTieu}</span>}
                  </div>
                  
                  {isAssigning && (
                    <button type="button" className="kpi-btn-remove-card" title="Xóa mục tiêu" onClick={() => handleRemoveRow(index)}>
                      &times;
                    </button>
                  )}
                </div>

                <div className="kpi-target-card-body">
                  <div className="kpi-target-field">
                    <span className="kpi-target-field-label">Loại tiêu chí</span>
                    {isTargetEditable ? (
                      <select className="kpi-form-input" value={row.loaiTieuChiValue} onChange={(e) => handleRowChange(index, 'loaiTieuChiValue', e.target.value)}>
                        <option value="CANG_NHIEU_CANG_TOT">[+] Càng nhiều càng tốt</option>
                        <option value="CANG_IT_CANG_TOT">[-] Càng ít càng tốt</option>
                      </select>
                    ) : (
                      <span style={{ fontSize: '0.85em', color: row.loaiTieuChiValue === 'CANG_IT_CANG_TOT' ? '#d97706' : '#16a34a', fontWeight: 600, padding: '0.35rem 0' }}>
                        {row.loaiTieuChiValue === 'CANG_IT_CANG_TOT' ? '[-] Càng ít càng tốt' : '[+] Càng nhiều càng tốt'}
                      </span>
                    )}
                  </div>

                  <div className="kpi-target-field">
                    <span className="kpi-target-field-label">Đơn vị tính</span>
                    {isTargetEditable ? (
                      <input className="kpi-form-input" placeholder="VD: Triệu, Bài viết..." value={row.donViTinh} onChange={(e) => handleRowChange(index, 'donViTinh', e.target.value)} />
                    ) : <span className="kpi-target-field-value">{row.donViTinh}</span>}
                  </div>

                  <div className="kpi-target-field">
                    <span className="kpi-target-field-label">Trọng số (%)</span>
                    {isTargetEditable ? (
                      <input type="number" className="kpi-form-input" placeholder="0" value={row.trongSo} onChange={(e) => handleRowChange(index, 'trongSo', e.target.value)} />
                    ) : <span className="kpi-target-field-value">{row.trongSo}%</span>}
                  </div>

                  <div className="kpi-target-field">
                    <span className="kpi-target-field-label">Chỉ tiêu</span>
                    {isTargetEditable ? (
                      <input type="number" className="kpi-form-input" placeholder="0" value={row.chiTieu} onChange={(e) => handleRowChange(index, 'chiTieu', e.target.value)} />
                    ) : <span className="kpi-target-field-value">{row.chiTieu}</span>}
                  </div>

                  <div className="kpi-target-field" style={{ background: isActualEditable ? 'var(--bg-hover)' : 'transparent', padding: isActualEditable ? '0.5rem' : '0', borderRadius: '8px' }}>
                    <span className="kpi-target-field-label" style={{ color: isActualEditable ? 'var(--primary)' : 'var(--text-secondary)' }}>Thực tế</span>
                    {isActualEditable ? (
                      <input type="number" className="kpi-form-input" placeholder="0" value={row.thucTe} onChange={(e) => handleRowChange(index, 'thucTe', e.target.value)} />
                    ) : <span className="kpi-target-field-value font-bold">{row.thucTe}</span>}
                  </div>

                  {(!isAssigning) && (
                    <div className="kpi-target-field">
                      <span className="kpi-target-field-label">Tỷ lệ hoàn thành</span>
                      <span className="kpi-target-field-value text-primary font-bold">{row.tiLeHoanThanh}%</span>
                    </div>
                  )}

                  {(!isAssigning) && (
                    <div className="kpi-target-field">
                      <span className="kpi-target-field-label">Điểm KPI</span>
                      <span className="kpi-target-field-value text-success font-bold">{row.diemKpi}</span>
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>

          {phieu.trangThaiValue >= 2 && (
            <div className="kpi-result-banner">
              <span className="kpi-result-text">Tổng Điểm KPI:</span>
              <span className="kpi-result-number">{phieu.tongDiemKpi}% ➔ Hệ số KPI = {phieu.heSoP3}</span>
            </div>
          )}

          {(isManagerView && phieu.canManage || phieu.trangThaiValue === 3) && phieu.trangThaiValue >= 2 && (
            <div style={{ marginTop: '24px' }}>
              <label style={{ display: 'block', marginBottom: '8px', fontWeight: 500 }}>Nhận xét của Quản lý</label>
              <textarea 
                className="kpi-form-textarea" 
                rows={3} 
                value={nhanXet}
                onChange={(e) => setNhanXet(e.target.value)}
                disabled={phieu.trangThaiValue === 3 || !phieu.canManage}
                placeholder={phieu.trangThaiValue === 3 ? "Không có nhận xét." : "Nhập nhận xét..."}
              />
            </div>
          )}
        </div>
        
        <div className="kpi-modal-footer">
          <button type="button" className="kpi-btn kpi-btn-secondary" onClick={onClose}>Đóng</button>
          
          {isAssigning && (
            <button type="button" className="kpi-btn kpi-btn-primary" onClick={handleAssignKpi} disabled={isLoading}>
              Giao KPI
            </button>
          )}

          {isUpdatingProgress && (
            <>
              <button type="button" className="kpi-btn kpi-btn-secondary" onClick={() => handleSaveProgress(false)} disabled={isLoading}>
                Lưu Tiến Độ
              </button>
              <button type="button" className="kpi-btn kpi-btn-primary" onClick={() => handleSaveProgress(true)} disabled={isLoading}>
                Nộp Phê Duyệt
              </button>
            </>
          )}

          {canApprove && (
            <button type="button" className="kpi-btn kpi-btn-primary" style={{ background: '#16a34a' }} onClick={handleApprove} disabled={isLoading}>
              Phê Duyệt Và Chốt Hệ Số KPI
            </button>
          )}
        </div>
      </div>
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
