import React, { useEffect } from 'react';
import { Modal } from 'antd';
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
  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm<ChangeStatusDto>({
    defaultValues: { cccd, trangThaiMoi: currentStatus as TrangThaiNhanVien, lyDo: '' }
  });

  useEffect(() => {
    if (isOpen) {
      reset({ cccd, trangThaiMoi: currentStatus as TrangThaiNhanVien, lyDo: '' });
    }
  }, [isOpen, cccd, currentStatus, reset]);

  const onSubmit = async (data: ChangeStatusDto) => {
    const success = await onSubmitStatus(data);
    if (success) onClose();
  };

  return (
    <Modal
      title={<h3 className="emp-modal-title">Thay đổi trạng thái nhân viên</h3>}
      open={isOpen}
      onCancel={onClose}
      footer={null} // Tắt footer mặc định của AntD vì react-hook-form cần trigger submit bằng thẻ form bên trong
      destroyOnClose
      width={500}
    >
      <div className="emp-alert-box mt-2">
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" className="w-5 h-5 shrink-0 mt-0.5">
          <path fillRule="evenodd" d="M18 10a8 8 0 1 1-16 0 8 8 0 0 1 16 0Zm-8-5a.75.75 0 0 1 .75.75v4.5a.75.75 0 0 1-1.5 0v-4.5A.75.75 0 0 1 10 5Zm0 10a1 1 0 1 0 0-2 1 1 0 0 0 0 2Z" clipRule="evenodd" />
        </svg>
        <p><strong>Lưu ý quan trọng:</strong> Việc thay đổi trạng thái sẽ trực tiếp ảnh hưởng đến quyền truy cập hệ thống và quá trình tính lương, chấm công của nhân viên này.</p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label className="emp-form-label">Trạng thái mới</label>
          <select 
            {...register('trangThaiMoi', { required: 'Vui lòng chọn trạng thái' })}
            className="emp-form-input cursor-pointer"
          >
            <option value="DANG_LAM_VIEC">Đang làm việc</option>
            <option value="DA_NGHI_VIEC">Đã nghỉ việc</option>
            <option value="THAI_SAN">Nghỉ thai sản</option>
            <option value="TAM_DINH_CHI">Tạm đình chỉ</option>
          </select>
        </div>

        <div>
          <label className="emp-form-label">
            Lý do thay đổi <span className="text-red-500">*</span>
          </label>
          <textarea
            {...register('lyDo', { required: 'Bắt buộc phải nhập lý do thay đổi để lưu nhật ký' })}
            rows={4}
            placeholder="Nhập lý do điều chuyển, nghỉ việc (VD: Theo quyết định số... / Lý do cá nhân...)"
            className="emp-form-input resize-none"
          />
          {errors.lyDo && (
            <span className="emp-form-error">
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" className="w-4 h-4">
                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-5a.75.75 0 01.75.75v4.5a.75.75 0 01-1.5 0v-4.5A.75.75 0 0110 5zm0 10a1 1 0 100-2 1 1 0 000 2z" clipRule="evenodd" />
              </svg>
              {errors.lyDo.message}
            </span>
          )}
        </div>

        <div className="flex justify-end gap-3 mt-6 pt-4 border-t border-gray-100 dark:border-gray-800">
          <button type="button" onClick={onClose} className="emp-btn-cancel">
            Hủy bỏ
          </button>
          <button type="submit" disabled={isSubmitting} className="emp-btn-submit">
            {isSubmitting && (
              <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            )}
            {isSubmitting ? 'Đang lưu...' : 'Lưu thay đổi'}
          </button>
        </div>
      </form>
    </Modal>
  );
};