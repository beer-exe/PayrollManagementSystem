import React, { useEffect } from 'react';
import { Modal } from 'antd';
import { useForm, useFieldArray } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { updateEmployeeSchema, UpdateEmployeeFormValues } from '../schemas/employeeSchema';
import { UserProfileDetail } from '@/types/profile.types';
import { useSystemData } from '../../departments/hooks/useSystemData';
import './EmployeeModals.css';

interface Props {
  isOpen: boolean;
  onClose: () => void;
  employee: UserProfileDetail | null;
  onSubmitUpdate: (cccd: string, data: UpdateEmployeeFormValues) => Promise<boolean>;
}

const ErrorIcon = () => (
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" className="w-4 h-4 shrink-0 mt-0.5">
    <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-5a.75.75 0 01.75.75v4.5a.75.75 0 01-1.5 0v-4.5A.75.75 0 0110 5zm0 10a1 1 0 100-2 1 1 0 000 2z" clipRule="evenodd" />
  </svg>
);

export const UpdateEmployeeModal: React.FC<Props> = ({ isOpen, onClose, employee, onSubmitUpdate }) => {
  const { register, control, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm<UpdateEmployeeFormValues>({
    resolver: zodResolver(updateEmployeeSchema),
    mode: 'onTouched',
  });

  const { relations } = useSystemData();

  const { fields, append, remove } = useFieldArray({
    control,
    name: 'thanNhans',
  });

  useEffect(() => {
    if (isOpen && employee) {
      reset({
        cccd: employee.cccd,
        hoTen: employee.hoTen,
        email: employee.email || '',
        sdt: employee.sdt || '',
        gioiTinh: employee.gioiTinh,
        ngaySinh: employee.ngaySinh || '',
        danToc: employee.danToc || '',
        diaChi: employee.diaChi || '',
        chuyenNganh: employee.chuyenNganh || '',
        soBhxh: employee.soBhxh || '',
        soBhyt: employee.soBhyt || '',
        soTaiKhoan: employee.soTaiKhoan || '',
        tenNganHang: employee.tenNganHang || '',
        maSoThue: employee.maSoThue || '',
        thanNhans: employee.thanNhans?.map(t => ({
          maDinhDanh: t.maDinhDanh,
          tenTn: t.tenTn,
          ngaySinh: t.ngaySinh || '',
          idMqh: t.idMqh || null
        })) || [],
      });
    }
  }, [isOpen, employee, reset]);

  const onSubmit = async (data: UpdateEmployeeFormValues) => {
    if (!employee) return;
    
    // Clean up empty strings for idMqh
    const cleanedData = {
      ...data,
      thanNhans: data.thanNhans?.map(tn => ({
        ...tn,
        idMqh: tn.idMqh === '' ? null : tn.idMqh
      }))
    };
    
    const success = await onSubmitUpdate(employee.cccd, cleanedData);
    if (success) onClose();
  };

  if (!employee) return null;

  return (
    <Modal
      title={<h3 className="emp-modal-title">Cập nhật hồ sơ nhân sự</h3>}
      open={isOpen}
      onCancel={onClose}
      footer={null}
      width={750}
      destroyOnClose
    >
      <form onSubmit={handleSubmit(onSubmit)} className="mt-4 space-y-6 animate-[fadeIn_0.3s_ease-out]">
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-5 gap-y-4">
          
          <div className="sm:col-span-2">
            <label className="emp-form-label">Mã định danh (CCCD)</label>
            <input 
              {...register('cccd')} 
              disabled 
              className="emp-form-input bg-gray-100 dark:bg-gray-800/60 text-gray-500 dark:text-gray-400 cursor-not-allowed font-mono" 
            />
          </div>
          
          <div>
            <label className="emp-form-label">Họ và tên <span className="text-red-500">*</span></label>
            <input {...register('hoTen')} className="emp-form-input" placeholder="VD: Nguyễn Văn A" />
            {errors.hoTen && <p className="emp-form-error"><ErrorIcon />{errors.hoTen.message}</p>}
          </div>

          <div>
            <label className="emp-form-label">Email liên hệ</label>
            <input {...register('email')} type="email" className="emp-form-input" placeholder="VD: email@congty.com" />
            {errors.email && <p className="emp-form-error"><ErrorIcon />{errors.email.message}</p>}
          </div>

          <div>
            <label className="emp-form-label">Số điện thoại</label>
            <input {...register('sdt')} className="emp-form-input" placeholder="VD: 0901234567" />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="emp-form-label">Giới tính</label>
              <select 
                  {...register('gioiTinh', { 
                    setValueAs: (value) => {
                      if (value === 'true') return true;
                      if (value === 'false') return false;
                      return null;
                    }
                  })} 
                  className="emp-form-input cursor-pointer"
                >
                  <option value="">Chưa xác định</option>
                  <option value="true">Nam</option>
                  <option value="false">Nữ</option>
                </select>
            </div>
            <div>
              <label className="emp-form-label">Ngày sinh</label>
              <input {...register('ngaySinh')} type="date" className="emp-form-input cursor-pointer" />
            </div>
          </div>

          <div>
            <label className="emp-form-label">Dân tộc</label>
            <input {...register('danToc')} className="emp-form-input" placeholder="VD: Kinh" />
          </div>

          <div>
            <label className="emp-form-label">Chuyên ngành</label>
            <input {...register('chuyenNganh')} className="emp-form-input" placeholder="VD: Kế toán, CNTT..." />
          </div>

          <div className="sm:col-span-2">
            <label className="emp-form-label">Địa chỉ liên hệ</label>
            <input {...register('diaChi')} className="emp-form-input" placeholder="VD: 123 Đường ABC, Quận X..." />
          </div>

          <div>
            <label className="emp-form-label">Số BHXH</label>
            <input {...register('soBhxh')} className="emp-form-input font-mono text-sm" placeholder="Nhập mã số BHXH" />
          </div>

          <div>
            <label className="emp-form-label">Số BHYT</label>
            <input {...register('soBhyt')} className="emp-form-input font-mono text-sm" placeholder="Nhập mã thẻ BHYT" />
          </div>

          <div>
            <label className="emp-form-label">Số tài khoản ngân hàng</label>
            <input {...register('soTaiKhoan')} className="emp-form-input font-mono text-sm" placeholder="VD: 123456789" />
          </div>

          <div>
            <label className="emp-form-label">Tên ngân hàng</label>
            <input {...register('tenNganHang')} className="emp-form-input" placeholder="VD: Vietcombank" />
          </div>

          <div className="sm:col-span-2">
            <label className="emp-form-label">Mã số thuế cá nhân</label>
            <input {...register('maSoThue')} className="emp-form-input font-mono text-sm" placeholder="VD: 8200123456" />
          </div>
        </div>

        {/* Section Thân Nhân */}
        <div className="mt-8 pt-6 border-t border-gray-100 dark:border-gray-800">
          <div className="flex justify-between items-center mb-4">
            <h4 className="text-sm font-semibold text-gray-800 dark:text-gray-100 uppercase tracking-wider">Danh sách người phụ thuộc (Thân nhân)</h4>
            <button 
              type="button" 
              onClick={() => append({ maDinhDanh: null, tenTn: '', ngaySinh: null, idMqh: null })}
              className="px-3 py-1.5 text-xs bg-indigo-50 text-indigo-600 rounded-lg hover:bg-indigo-100 transition-colors font-medium border border-indigo-100 dark:bg-indigo-900/30 dark:text-indigo-400 dark:border-indigo-800/50"
            >
              + Thêm người thân
            </button>
          </div>
          
          <div className="space-y-4">
            {fields.length === 0 ? (
              <p className="text-sm text-gray-500 dark:text-gray-400 italic text-center py-4 bg-gray-50 dark:bg-gray-800/30 rounded-lg border border-dashed border-gray-200 dark:border-gray-700">Chưa có thông tin người phụ thuộc.</p>
            ) : (
              fields.map((field, index) => (
                <div key={field.id} className="relative p-4 bg-gray-50 dark:bg-gray-800/40 rounded-xl border border-gray-100 dark:border-gray-700/50 animate-[fadeIn_0.3s_ease-out]">
                  <button 
                    type="button" 
                    onClick={() => remove(index)}
                    className="absolute top-3 right-3 text-gray-400 hover:text-red-500 transition-colors bg-white dark:bg-gray-800 rounded-full p-1 shadow-sm"
                    title="Xóa"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" /></svg>
                  </button>
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div className="sm:col-span-2">
                      <label className="emp-form-label text-xs">Họ và tên người thân <span className="text-red-500">*</span></label>
                      <input 
                        {...register(`thanNhans.${index}.tenTn`)} 
                        className="emp-form-input py-1.5" 
                        placeholder="VD: Nguyễn Văn B" 
                      />
                      {errors.thanNhans?.[index]?.tenTn && <p className="emp-form-error text-[11px]"><ErrorIcon />{errors.thanNhans[index]?.tenTn?.message}</p>}
                    </div>
                    <div>
                      <label className="emp-form-label text-xs">Ngày sinh</label>
                      <input 
                        {...register(`thanNhans.${index}.ngaySinh`)} 
                        type="date" 
                        className="emp-form-input py-1.5 cursor-pointer" 
                      />
                    </div>
                    <div>
                      <label className="emp-form-label text-xs">Mối quan hệ</label>
                      <select 
                        {...register(`thanNhans.${index}.idMqh`)} 
                        className="emp-form-input py-1.5 cursor-pointer"
                      >
                        <option value="">Chọn mối quan hệ</option>
                        {relations.map(rel => (
                          <option key={rel.idMqh} value={rel.idMqh}>
                            {rel.tenQuanHe}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        <div className="flex justify-end gap-3 mt-8 pt-5 border-t border-gray-100 dark:border-gray-800">
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