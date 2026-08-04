import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { TrangThaiNhanVien, ChangeStatusDto } from '../types/employee.types';
import './EmployeeModals.css';

interface Props {
  isOpen: boolean;
  onClose: () => void;
  cccd: string;
  currentStatus: string;
  onSubmitStatus: (data: ChangeStatusDto) => Promise<boolean>;
}

export const ChangeStatusModal: React.FC<Props> = ({ isOpen, onClose, cccd, currentStatus, onSubmitStatus }) => {
  const mapStatusToEnum = (status: string): string => {
    switch (status) {
      case 'Đang làm việc': return 'DANG_LAM_VIEC';
      case 'Đã nghỉ việc': return 'DA_NGHI_VIEC';
      case 'Nghỉ thai sản': return 'THAI_SAN';
      case 'Tạm đình chỉ': return 'TAM_DINH_CHI';
      default: return status;
    }
  };

  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm<ChangeStatusDto>({
    defaultValues: { cccd, trangThaiMoi: mapStatusToEnum(currentStatus) as TrangThaiNhanVien, lyDo: '' }
  });

  useEffect(() => {
    if (isOpen) {
      reset({ cccd, trangThaiMoi: mapStatusToEnum(currentStatus) as TrangThaiNhanVien, lyDo: '' });
    }
  }, [isOpen, cccd, currentStatus, reset]);

  const onSubmit = async (data: ChangeStatusDto) => {
    const success = await onSubmitStatus(data);
    if (success) onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="emp-modal-overlay">
      <div className="emp-modal">
        <div className="emp-modal-header">
          <h3 className="emp-modal-title">Thay đổi trạng thái nhân viên</h3>
          <button className="emp-modal-close" onClick={onClose} disabled={isSubmitting}>
            &times;
          </button>
        </div>

        <div className="emp-modal-body">
          <div className="emp-alert-box">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" style={{width: '1.25rem', height: '1.25rem', marginTop: '0.1rem'}}>
              <path fillRule="evenodd" d="M18 10a8 8 0 1 1-16 0 8 8 0 0 1 16 0Zm-8-5a.75.75 0 0 1 .75.75v4.5a.75.75 0 0 1-1.5 0v-4.5A.75.75 0 0 1 10 5Zm0 10a1 1 0 1 0 0-2 1 1 0 0 0 0 2Z" clipRule="evenodd" />
            </svg>
            <p><strong>Lưu ý quan trọng:</strong> Việc thay đổi trạng thái sẽ trực tiếp ảnh hưởng đến quyền truy cập hệ thống và quá trình tính lương, chấm công của nhân viên này.</p>
          </div>

          <form id="change-status-form" onSubmit={handleSubmit(onSubmit)}>
            <div className="emp-form-group">
              <label className="emp-form-label">Trạng thái hiện tại</label>
              <div style={{ fontWeight: 600, color: '#4f46e5', padding: '0.5rem 0.75rem', backgroundColor: '#f3f4f6', borderRadius: '4px', border: '1px solid #e5e7eb' }}>
                {currentStatus === 'DANG_LAM_VIEC' ? 'Đang làm việc' : currentStatus}
              </div>
            </div>

            <div className="emp-form-group">
              <label className="emp-form-label">Trạng thái mới</label>
              <select 
                {...register('trangThaiMoi', { required: 'Vui lòng chọn trạng thái' })}
                className="emp-form-select"
              >
                <option value="DANG_LAM_VIEC">Đang làm việc</option>
                <option value="DA_NGHI_VIEC">Đã nghỉ việc</option>
                <option value="THAI_SAN">Nghỉ thai sản</option>
                <option value="TAM_DINH_CHI">Tạm đình chỉ</option>
              </select>
            </div>

            <div className="emp-form-group">
              <label className="emp-form-label">
                Lý do thay đổi <span className="required">*</span>
              </label>
              <textarea
                {...register('lyDo', { required: 'Bắt buộc phải nhập lý do thay đổi để lưu nhật ký' })}
                placeholder="Nhập lý do điều chuyển, nghỉ việc (VD: Theo quyết định số... / Lý do cá nhân...)"
                className="emp-form-textarea"
              />
              {errors.lyDo && (
                <span className="emp-form-error">
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" style={{width: '1rem', height: '1rem'}}>
                    <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-5a.75.75 0 01.75.75v4.5a.75.75 0 01-1.5 0v-4.5A.75.75 0 0110 5zm0 10a1 1 0 100-2 1 1 0 000 2z" clipRule="evenodd" />
                  </svg>
                  {errors.lyDo.message}
                </span>
              )}
            </div>
          </form>
        </div>

        <div className="emp-modal-footer">
          <button type="button" onClick={onClose} className="emp-btn-cancel" disabled={isSubmitting}>
            Hủy bỏ
          </button>
          <button type="submit" form="change-status-form" disabled={isSubmitting} className="emp-btn-submit">
            {isSubmitting ? 'Đang lưu...' : 'Lưu thay đổi'}
          </button>
        </div>
      </div>
    </div>
  );
};