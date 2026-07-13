import React, { useEffect, useState } from 'react';
import { departmentApi } from '../../api/departmentApi';
import { EmployeeInDepartmentDto } from '../../types/department.types';
import { PositionDto } from '@/features/positions/types/position.types';
import { salaryStepApi } from '@/features/salarySteps/api/salaryStepApi';
import { SalaryStepDto } from '@/features/salarySteps/types/salaryStep.types';
import './DepartmentModals.css';

interface ChangePositionModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  employee: EmployeeInDepartmentDto | null;
  positions: PositionDto[];
}

export const ChangePositionModal: React.FC<ChangePositionModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
  employee,
  positions,
}) => {
  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  const [soQuyetDinh, setSoQuyetDinh] = useState('');
  const [idChucVuMoi, setIdChucVuMoi] = useState('');
  const [idBacLuongMoi, setIdBacLuongMoi] = useState('');
  const [ngayHieuLuc, setNgayHieuLuc] = useState('');
  const [lyDo, setLyDo] = useState('');

  const [salarySteps, setSalarySteps] = useState<SalaryStepDto[]>([]);
  const [loadingSteps, setLoadingSteps] = useState(false);

  useEffect(() => {
    if (isOpen) {
      setSoQuyetDinh('');
      setIdChucVuMoi('');
      setIdBacLuongMoi('');
      setNgayHieuLuc('');
      setLyDo('');
      setErrorMsg('');
      setSalarySteps([]);
    }
  }, [isOpen]);

  const handlePositionChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const positionId = e.target.value;
    setIdChucVuMoi(positionId);
    setIdBacLuongMoi('');

    if (!positionId) {
      setSalarySteps([]);
      return;
    }

    const selectedPos = positions.find((p) => p.idChucVu === positionId);
    if (!selectedPos || !selectedPos.idNgachLuong) {
      setSalarySteps([]);
      setErrorMsg('Chức vụ này chưa được cấu hình ngạch lương.');
      return;
    }

    setLoadingSteps(true);
    setErrorMsg('');
    try {
      const res = await salaryStepApi.getActive(selectedPos.idNgachLuong);
      if (res.succeeded) {
        setSalarySteps(res.data);
      }
    } catch (error) {
      console.error(error);
      setErrorMsg('Lỗi tải danh sách bậc lương cho chức vụ này.');
    } finally {
      setLoadingSteps(false);
    }
  };

  const handleSubmit = async () => {
    if (!employee) return;

    if (!soQuyetDinh.trim() || !idChucVuMoi || !idBacLuongMoi || !ngayHieuLuc) {
      setErrorMsg('Vui lòng điền đầy đủ các trường bắt buộc!');
      return;
    }

    setLoading(true);
    setErrorMsg('');

    try {
      const payload = {
        soQuyetDinh,
        cccd: employee.cccd,
        idChucVuMoi,
        idBacLuongMoi,
        ngayHieuLuc,
        lyDo,
      };

      const res = await departmentApi.changePosition(payload);
      if (res.succeeded) {
        onSuccess();
        onClose();
      }
    } catch (err) {
      const error = err as import('axios').AxiosError<{Message?: string}>;
      setErrorMsg(error.response?.data?.Message || 'Lỗi khi lưu quyết định thay đổi chức vụ');
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen || !employee) return null;

  return (
    <div className="dept-modal-overlay">
      <div className="dept-modal">
        <div className="dept-modal-header" style={{ background: 'linear-gradient(135deg, #2563eb, #1d4ed8)' }}>
          <h2 className="dept-modal-title">Quyết Định Bổ Nhiệm / Thay Đổi Chức Vụ</h2>
          <button className="dept-modal-close" onClick={onClose} disabled={loading} title="Đóng">
            &times;
          </button>
        </div>
        <div className="dept-modal-body">
          {errorMsg && (
            <div className="dept-alert error">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem', flexShrink: 0 }}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v3.75m9-.75a9 9 0 1 1-18 0 9 9 0 0 1 18 0Zm-9 3.75h.008v.008H12v-.008Z" />
              </svg>
              {errorMsg}
            </div>
          )}

          <div className="dept-modal-info-box" style={{ background: '#eff6ff', borderColor: '#bfdbfe' }}>
            <p style={{ color: '#1e40af' }}><strong>Nhân sự:</strong> {employee.hoTen}</p>
            <p style={{ margin: 0, fontSize: '0.85rem', color: '#1e40af' }}>Mã NV (CCCD): {employee.cccd} - Chức vụ hiện tại: {employee.tenChucVu}</p>
          </div>

          <div className="dept-form-group">
            <label className="dept-form-label">Số quyết định <span className="dept-required">*</span></label>
            <input 
              type="text" 
              className="dept-form-input" 
              value={soQuyetDinh} 
              onChange={e => setSoQuyetDinh(e.target.value)} 
              placeholder="VD: 126/QĐ-BN" 
            />
          </div>

          <div className="dept-form-group">
            <label className="dept-form-label">Chức vụ mới <span className="dept-required">*</span></label>
            <select 
              className="dept-form-select" 
              value={idChucVuMoi} 
              onChange={handlePositionChange}
            >
              <option value="">-- Chọn chức vụ bổ nhiệm --</option>
              {positions.map((p) => (
                <option key={p.idChucVu} value={p.idChucVu}>{p.tenChucVu}</option>
              ))}
            </select>
          </div>

          <div className="dept-form-group">
            <label className="dept-form-label">Bậc lương áp dụng (P1) <span className="dept-required">*</span></label>
            <select 
              className="dept-form-select" 
              value={idBacLuongMoi} 
              onChange={e => setIdBacLuongMoi(e.target.value)}
              disabled={salarySteps.length === 0 || loadingSteps}
            >
              <option value="">
                {loadingSteps ? 'Đang tải...' : salarySteps.length === 0 ? '-- Chọn chức vụ trước --' : '-- Chọn bậc lương --'}
              </option>
              {salarySteps.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.stepName} - {s.p1Salary.toLocaleString('vi-VN')} VNĐ
                </option>
              ))}
            </select>
          </div>

          <div className="dept-form-group">
            <label className="dept-form-label">Ngày hiệu lực <span className="dept-required">*</span></label>
            <input 
              type="date" 
              className="dept-form-input" 
              value={ngayHieuLuc} 
              onChange={e => setNgayHieuLuc(e.target.value)} 
            />
          </div>

          <div className="dept-form-group">
            <label className="dept-form-label">Lý do bổ nhiệm / thăng tiến</label>
            <textarea 
              className="dept-form-textarea" 
              value={lyDo} 
              onChange={e => setLyDo(e.target.value)} 
              placeholder="Ghi chú lý do..." 
            />
          </div>
        </div>
        <div className="dept-modal-footer">
          <button className="dept-btn-cancel" onClick={onClose} disabled={loading}>Hủy bỏ</button>
          <button className="dept-btn-submit" style={{ background: 'linear-gradient(135deg, #2563eb, #1d4ed8)' }} onClick={handleSubmit} disabled={loading}>
            {loading ? 'Đang xử lý...' : 'Xác nhận'}
          </button>
        </div>
      </div>
    </div>
  );
};
