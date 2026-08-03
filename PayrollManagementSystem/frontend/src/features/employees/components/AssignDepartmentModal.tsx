import React, { useState, useEffect } from 'react';
import { departmentApi } from '../../departments/api/departmentApi';
import { DepartmentDto } from '../../departments/types/department.types';
import { PositionDto } from '@/features/positions/types/position.types';
import { salaryStepApi } from '@/features/salarySteps/api/salaryStepApi';
import { SalaryStepDto } from '@/features/salarySteps/types/salaryStep.types';
import { hrDecisionsApi } from '@/features/hrDecisions/api/hrDecisions.api';
import { UserProfileDetail } from '@/types/profile.types';
import { positionApi } from '@/features/positions/api/positionApi';

interface AssignDepartmentModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  employee: UserProfileDetail | null;
}

export const AssignDepartmentModal: React.FC<AssignDepartmentModalProps> = ({
  isOpen,
  onClose,
  onSuccess,
  employee,
}) => {
  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');

  const [departments, setDepartments] = useState<DepartmentDto[]>([]);
  const [positions, setPositions] = useState<PositionDto[]>([]);

  const [soQuyetDinh, setSoQuyetDinh] = useState('');
  const [idPbMoi, setIdPbMoi] = useState('');
  const [idChucVuMoi, setIdChucVuMoi] = useState('');
  const [idBacLuongMoi, setIdBacLuongMoi] = useState('');
  const [ngayHieuLuc, setNgayHieuLuc] = useState('');
  const [lyDo, setLyDo] = useState('');

  const [salarySteps, setSalarySteps] = useState<SalaryStepDto[]>([]);
  const [loadingSteps, setLoadingSteps] = useState(false);

  const fetchSuggestedCode = async () => {
    try {
      const res = await hrDecisionsApi.generateCode('BN');
      if (res.succeeded) setSoQuyetDinh(res.data);
    } catch (err) {
      console.error('Failed to generate code:', err);
    }
  };

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

      // Fetch init data
      fetchSuggestedCode();

      departmentApi.getDepartments().then(res => {
        if (res.succeeded) setDepartments(res.data);
      }).catch(err => console.error(err));

      positionApi.getPositions({ trangThai: 'HOAT_DONG' }).then(res => {
        if (res.succeeded) setPositions(res.data);
      }).catch(err => console.error(err));
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
        loaiQuyetDinh: "Bổ nhiệm phòng ban"
      };

      const res = await departmentApi.transferEmployee(payload);
      if (res.succeeded) {
        onSuccess();
        onClose();
      }
    } catch (err) {
      const error = err as import('axios').AxiosError<{Message?: string}>;
      setErrorMsg(error.response?.data?.Message || 'Lỗi khi xếp phòng nhân sự');
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen || !employee) return null;

  const availablePositions = positions.filter((p) => p.idPhongBan === idPbMoi);

  return (
    <div className="emp-modal-overlay">
      <div className="emp-modal-large" style={{ maxWidth: '600px' }}>
        <div className="emp-modal-header">
          <h3 className="emp-modal-title">Quyết Định Xếp Phòng Nhân Sự</h3>
          <button className="emp-modal-close" onClick={onClose} disabled={loading} title="Đóng">
            &times;
          </button>
        </div>
        
        <div className="emp-modal-body" style={{ padding: '1.5rem', maxHeight: '70vh', overflowY: 'auto' }}>
          {errorMsg && (
            <div className="dept-alert error" style={{ marginBottom: '1rem', padding: '0.75rem', backgroundColor: 'var(--danger-bg)', color: 'var(--danger-text)', borderRadius: '4px', border: '1px solid #f87171' }}>
              {errorMsg}
            </div>
          )}

          <div style={{ marginBottom: '1.5rem', padding: '1rem', backgroundColor: 'var(--bg-hover)', borderRadius: '4px', border: '1px solid var(--border-color)' }}>
            <p style={{ margin: '0 0 0.5rem 0', color: 'var(--text-primary)' }}><strong>Nhân sự:</strong> {employee.hoTen}</p>
            <p style={{ margin: 0, fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Mã NV (CCCD): {employee.cccd}</p>
          </div>

          <div className="emp-form-group">
            <label className="emp-form-label">Số quyết định <span className="emp-required" style={{color: 'red'}}>*</span></label>
            <input 
              type="text" 
              className="emp-form-input" 
              value={soQuyetDinh} 
              onChange={e => setSoQuyetDinh(e.target.value)} 
              placeholder="VD: 123/QĐ-BN" 
            />
          </div>

          <div className="emp-form-group">
            <label className="emp-form-label">Phòng ban <span className="emp-required" style={{color: 'red'}}>*</span></label>
            <select 
              className="emp-form-input" 
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

          <div className="emp-form-group">
            <label className="emp-form-label">Chức vụ <span className="emp-required" style={{color: 'red'}}>*</span></label>
            <select 
              className="emp-form-input" 
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

          <div className="emp-form-group">
            <label className="emp-form-label">Bậc lương áp dụng (P1) <span className="emp-required" style={{color: 'red'}}>*</span></label>
            <select 
              className="emp-form-input" 
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

          <div className="emp-form-group">
            <label className="emp-form-label">Ngày hiệu lực <span className="emp-required" style={{color: 'red'}}>*</span></label>
            <input 
              type="date" 
              className="emp-form-input" 
              value={ngayHieuLuc} 
              onChange={e => setNgayHieuLuc(e.target.value)} 
            />
          </div>

          <div className="emp-form-group">
            <label className="emp-form-label">Lý do bổ nhiệm</label>
            <textarea 
              className="emp-form-input" 
              value={lyDo} 
              onChange={e => setLyDo(e.target.value)} 
              placeholder="Ghi chú lý do..." 
              style={{ minHeight: '80px', width: '100%', padding: '0.5rem', borderRadius: '4px', border: '1px solid var(--border-color)', backgroundColor: 'var(--bg-surface)', color: 'var(--text-primary)' }}
            />
          </div>
        </div>
        
        <div className="emp-modal-footer">
          <button className="emp-btn-secondary" onClick={onClose} disabled={loading}>Hủy bỏ</button>
          <button className="emp-btn-primary" onClick={handleSubmit} disabled={loading}>
            {loading ? 'Đang xử lý...' : 'Xác nhận xếp phòng'}
          </button>
        </div>
      </div>
    </div>
  );
};
