import React, { useState } from 'react';
import { departmentApi } from '../../api/departmentApi';
import './DepartmentModals.css';

interface CreateDeptModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const CreateDeptModal: React.FC<CreateDeptModalProps> = ({ isOpen, onClose, onSuccess }) => {
  const [loading, setLoading] = useState(false);
  const [idPb, setIdPb] = useState('');
  const [tenPb, setTenPb] = useState('');
  const [errorMsg, setErrorMsg] = useState('');

  if (!isOpen) return null;

  const handleCreate = async () => {
    if (!idPb.trim() || !tenPb.trim()) {
      setErrorMsg('Vui lòng nhập đầy đủ mã và tên phòng ban!');
      return;
    }
    if (idPb.length > 50) {
      setErrorMsg('Mã phòng ban không được vượt quá 50 ký tự!');
      return;
    }
    if (tenPb.length > 100) {
      setErrorMsg('Tên phòng ban không được vượt quá 100 ký tự!');
      return;
    }

    setLoading(true);
    setErrorMsg('');
    try {
      const res = await departmentApi.createDepartment({ idPb, tenPb });
      if (res.succeeded) {
        setIdPb('');
        setTenPb('');
        onSuccess();
        onClose();
      }
    } catch (err) {
      const error = err as import('axios').AxiosError<{Message?: string}>;
      setErrorMsg(error.response?.data?.Message || 'Có lỗi xảy ra khi tạo phòng ban');
    } finally { 
      setLoading(false); 
    }
  };

  const handleCancel = () => {
    setIdPb('');
    setTenPb('');
    setErrorMsg('');
    onClose();
  };

  return (
    <div className="dept-modal-overlay">
      <div className="dept-modal">
        <div className="dept-modal-header">
          <h2 className="dept-modal-title">Thêm phòng ban mới</h2>
          <button className="dept-modal-close" onClick={handleCancel} disabled={loading} title="Đóng">
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
          
          <div className="dept-form-group">
            <label className="dept-form-label">Mã Phòng Ban <span className="dept-required">*</span></label>
            <input 
              type="text" 
              className="dept-form-input" 
              value={idPb} 
              onChange={e => setIdPb(e.target.value)} 
              placeholder="VD: PB_MARKETING" 
            />
          </div>
          
          <div className="dept-form-group">
            <label className="dept-form-label">Tên Phòng Ban <span className="dept-required">*</span></label>
            <input 
              type="text" 
              className="dept-form-input" 
              value={tenPb} 
              onChange={e => setTenPb(e.target.value)} 
              placeholder="VD: Phòng Marketing" 
            />
          </div>
        </div>
        <div className="dept-modal-footer">
          <button className="dept-btn-cancel" onClick={handleCancel} disabled={loading}>Hủy bỏ</button>
          <button className="dept-btn-submit" onClick={handleCreate} disabled={loading}>
            {loading ? 'Đang tạo...' : 'Tạo mới'}
          </button>
        </div>
      </div>
    </div>
  );
};