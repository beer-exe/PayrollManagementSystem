import React, { useState, useEffect } from 'react';
import { departmentApi } from '../../api/departmentApi';
import { DepartmentDto, EmployeeInDepartmentDto } from '../../types/department.types';
import { PositionDto } from '@/features/positions/types/position.types';
import { salaryStepApi } from '@/features/salarySteps/api/salaryStepApi';
import { SalaryStepDto } from '@/features/salarySteps/types/salaryStep.types';
import './DepartmentModals.css';

interface TransferModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  departments: DepartmentDto[];
  positions: PositionDto[];
  employee: EmployeeInDepartmentDto | null;
}

export const TransferModal: React.FC<TransferModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
  departments,
  positions,
  employee,
}) => {
  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  const [soQuyetDinh, setSoQuyetDinh] = useState('');
  const [idPbMoi, setIdPbMoi] = useState('');
  const [idChucVuMoi, setIdChucVuMoi] = useState('');
  const [idBacLuongMoi, setIdBacLuongMoi] = useState('');
  const [ngayHieuLuc, setNgayHieuLuc] = useState('');
  const [lyDo, setLyDo] = useState('');

  const [salarySteps, setSalarySteps] = useState<SalaryStepDto[]>([]);
  const [loadingSteps, setLoadingSteps] = useState(false);

  useEffect(() => {
    if (isOpen) {
      setSoQuyetDinh('');
      setIdPbMoi('');
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
      return;
    }

    setLoadingSteps(true);
    try {
      const res = await salaryStepApi.getActive(selectedPos.idNgachLuong);
      if (res.succeeded) {
        setSalarySteps(res.data);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoadingSteps(false);
    }
  };

  const handleSubmit = async () => {
    if (!employee) return;
    
    if (!soQuyetDinh.trim() || !idPbMoi || !idChucVuMoi || !idBacLuongMoi || !ngayHieuLuc) {
      setErrorMsg('Vui lòng điền đầy đủ các trường bắt buộc!');
      return;
    }

    setLoading(true);
    setErrorMsg('');

    try {
      const payload = {
        soQuyetDinh,
        cccd: employee.cccd,
        idPbMoi,
        idChucVuMoi,
        idBacLuongMoi,
        ngayHieuLuc,
        lyDo,
      };

      const res = await departmentApi.transferEmployee(payload);
      if (res.succeeded) {
        onSuccess();
        onClose();
      }
    } catch (err) {
      const error = err as import('axios').AxiosError<{Message?: string}>;
      setErrorMsg(error.response?.data?.Message || 'Lỗi khi điều chuyển nhân sự');
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen || !employee) return null;

  const availablePositions = positions.filter((p) => p.idPhongBan === idPbMoi);

  return (
    <div className="dept-modal-overlay">
      <div className="dept-modal">
        <div className="dept-modal-header">
          <h2 className="dept-modal-title">Quyết Định Điều Chuyển Nhân Sự</h2>
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

          <div className="dept-modal-info-box">
            <p><strong>Nhân sự:</strong> {employee.hoTen}</p>
            <p style={{ margin: 0, fontSize: '0.85rem' }}>Mã NV (CCCD): {employee.cccd} - Đang làm việc tại: {employee.tenChucVu}</p>
          </div>

          <div className="dept-form-group">
            <label className="dept-form-label">Số quyết định <span className="dept-required">*</span></label>
            <input 
              type="text" 
              className="dept-form-input" 
              value={soQuyetDinh} 
              onChange={e => setSoQuyetDinh(e.target.value)} 
              placeholder="VD: 123/QĐ-NS" 
            />
          </div>

          <div className="dept-form-group">
            <label className="dept-form-label">Phòng ban mới <span className="dept-required">*</span></label>
            <select 
              className="dept-form-select" 
              value={idPbMoi} 
              onChange={e => {
                setIdPbMoi(e.target.value);
                setIdChucVuMoi('');
                setSalarySteps([]);
              }}
            >
              <option value="">-- Chọn phòng ban --</option>
              {departments.map((d) => (
                <option key={d.idPb} value={d.idPb}>{d.tenPb}</option>
              ))}
            </select>
          </div>

          <div className="dept-form-group">
            <label className="dept-form-label">Chức vụ mới <span className="dept-required">*</span></label>
            <select 
              className="dept-form-select" 
              value={idChucVuMoi} 
              onChange={handlePositionChange}
              disabled={!idPbMoi}
            >
              <option value="">-- Chọn chức vụ --</option>
              {availablePositions.map((p) => (
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
            <label className="dept-form-label">Lý do điều chuyển</label>
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
          <button className="dept-btn-submit" onClick={handleSubmit} disabled={loading}>
            {loading ? 'Đang xử lý...' : 'Xác nhận điều chuyển'}
          </button>
        </div>
      </div>
    </div>
  );
};
