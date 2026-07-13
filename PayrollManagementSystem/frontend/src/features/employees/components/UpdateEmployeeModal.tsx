import React, { useEffect } from 'react';
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
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" style={{width: '1rem', height: '1rem', flexShrink: 0, marginTop: '0.1rem'}}>
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

  if (!isOpen || !employee) return null;

  return (
    <div className="emp-modal-overlay">
      <div className="emp-modal" style={{ maxWidth: '800px', height: '90vh' }}>
        <div className="emp-modal-header">
          <h3 className="emp-modal-title">Cập nhật hồ sơ nhân sự</h3>
          <button className="emp-modal-close" onClick={onClose} disabled={isSubmitting}>
            &times;
          </button>
        </div>

        <div className="emp-modal-body custom-scrollbar">
          <form id="update-emp-form" onSubmit={handleSubmit(onSubmit)}>
            
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.25rem' }}>
              <div style={{ gridColumn: '1 / -1' }}>
                <label className="emp-form-label">Mã định danh (CCCD)</label>
                <input 
                  {...register('cccd')} 
                  disabled 
                  className="emp-form-input" 
                  style={{ background: '#f3f4f6', color: '#9ca3af', cursor: 'not-allowed', fontFamily: 'monospace' }}
                />
              </div>
              
              <div>
                <label className="emp-form-label">Họ và tên <span className="required">*</span></label>
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

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
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
                    className="emp-form-select"
                  >
                    <option value="">Chưa xác định</option>
                    <option value="true">Nam</option>
                    <option value="false">Nữ</option>
                  </select>
                </div>
                <div>
                  <label className="emp-form-label">Ngày sinh</label>
                  <input {...register('ngaySinh')} type="date" className="emp-form-input" />
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

              <div style={{ gridColumn: '1 / -1' }}>
                <label className="emp-form-label">Địa chỉ liên hệ</label>
                <input {...register('diaChi')} className="emp-form-input" placeholder="VD: 123 Đường ABC, Quận X..." />
              </div>

              <div>
                <label className="emp-form-label">Số BHXH</label>
                <input {...register('soBhxh')} className="emp-form-input" placeholder="Nhập mã số BHXH" style={{ fontFamily: 'monospace' }} />
              </div>

              <div>
                <label className="emp-form-label">Số BHYT</label>
                <input {...register('soBhyt')} className="emp-form-input" placeholder="Nhập mã thẻ BHYT" style={{ fontFamily: 'monospace' }} />
              </div>

              <div>
                <label className="emp-form-label">Số tài khoản ngân hàng</label>
                <input {...register('soTaiKhoan')} className="emp-form-input" placeholder="VD: 123456789" style={{ fontFamily: 'monospace' }} />
              </div>

              <div>
                <label className="emp-form-label">Tên ngân hàng</label>
                <input {...register('tenNganHang')} className="emp-form-input" placeholder="VD: Vietcombank" />
              </div>

              <div style={{ gridColumn: '1 / -1' }}>
                <label className="emp-form-label">Mã số thuế cá nhân</label>
                <input {...register('maSoThue')} className="emp-form-input" placeholder="VD: 8200123456" style={{ fontFamily: 'monospace' }} />
              </div>
            </div>

            {/* Section Thân Nhân */}
            <div style={{ marginTop: '2.5rem', paddingTop: '1.5rem', borderTop: '1px solid #e5e7eb' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                <h4 style={{ fontSize: '0.85rem', fontWeight: 600, color: '#374151', textTransform: 'uppercase', letterSpacing: '0.05em', margin: 0 }}>
                  Danh sách người phụ thuộc (Thân nhân)
                </h4>
                <button 
                  type="button" 
                  onClick={() => append({ maDinhDanh: null, tenTn: '', ngaySinh: null, idMqh: null })}
                  style={{ background: '#eff6ff', color: '#2563eb', border: '1px solid #dbeafe', borderRadius: '8px', padding: '0.35rem 0.75rem', fontSize: '0.8rem', fontWeight: 600, cursor: 'pointer' }}
                >
                  + Thêm người thân
                </button>
              </div>
              
              <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                {fields.length === 0 ? (
                  <p style={{ fontSize: '0.85rem', color: '#6b7280', fontStyle: 'italic', textAlign: 'center', padding: '1rem', background: '#f9fafb', borderRadius: '8px', border: '1px dashed #d1d5db', margin: 0 }}>
                    Chưa có thông tin người phụ thuộc.
                  </p>
                ) : (
                  fields.map((field, index) => (
                    <div key={field.id} style={{ position: 'relative', padding: '1.25rem', background: '#f9fafb', borderRadius: '12px', border: '1px solid #e5e7eb' }}>
                      <button 
                        type="button" 
                        onClick={() => remove(index)}
                        style={{ position: 'absolute', top: '0.75rem', right: '0.75rem', background: '#fff', color: '#9ca3af', border: 'none', borderRadius: '50%', padding: '0.25rem', cursor: 'pointer', boxShadow: '0 1px 2px rgba(0,0,0,0.05)' }}
                        title="Xóa"
                      >
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                      </button>

                      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                        <div style={{ gridColumn: '1 / -1' }}>
                          <label className="emp-form-label" style={{ fontSize: '0.8rem' }}>Họ và tên người thân <span className="required">*</span></label>
                          <input 
                            {...register(`thanNhans.${index}.tenTn`)} 
                            className="emp-form-input" 
                            style={{ padding: '0.5rem 0.75rem' }}
                            placeholder="VD: Nguyễn Văn B" 
                          />
                          {errors.thanNhans?.[index]?.tenTn && <p className="emp-form-error" style={{ fontSize: '0.75rem' }}><ErrorIcon />{errors.thanNhans[index]?.tenTn?.message}</p>}
                        </div>
                        <div>
                          <label className="emp-form-label" style={{ fontSize: '0.8rem' }}>Ngày sinh</label>
                          <input 
                            {...register(`thanNhans.${index}.ngaySinh`)} 
                            type="date" 
                            className="emp-form-input" 
                            style={{ padding: '0.5rem 0.75rem' }}
                          />
                        </div>
                        <div>
                          <label className="emp-form-label" style={{ fontSize: '0.8rem' }}>Mối quan hệ</label>
                          <select 
                            {...register(`thanNhans.${index}.idMqh`)} 
                            className="emp-form-select"
                            style={{ padding: '0.5rem 0.75rem' }}
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

          </form>
        </div>

        <div className="emp-modal-footer">
          <button type="button" onClick={onClose} className="emp-btn-cancel" disabled={isSubmitting}>
            Hủy bỏ
          </button>
          <button type="submit" form="update-emp-form" disabled={isSubmitting} className="emp-btn-submit">
            {isSubmitting ? 'Đang lưu...' : 'Lưu thay đổi'}
          </button>
        </div>
      </div>
    </div>
  );
};