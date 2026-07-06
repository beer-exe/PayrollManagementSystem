import React, { useState } from 'react';
import { useForm, FormProvider, useFormContext } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Steps } from 'antd';
import { createEmployeeSchema, CreateEmployeeFormValues } from '../schemas/employeeSchema';
import { useSystemData } from '../../departments/hooks/useSystemData';
import './EmployeeModals.css';

const ErrorIcon = () => (
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" className="w-4 h-4 shrink-0 mt-0.5">
    <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-5a.75.75 0 01.75.75v4.5a.75.75 0 01-1.5 0v-4.5A.75.75 0 0110 5zm0 10a1 1 0 100-2 1 1 0 000 2z" clipRule="evenodd" />
  </svg>
);


const StepPersonal = () => {
  const { register, formState: { errors } } = useFormContext<CreateEmployeeFormValues>();
  
  return (
    <div className="mt-6 space-y-4 animate-[fadeIn_0.3s_ease-out]">
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-4 gap-y-4">
        <div className="sm:col-span-2">
          <label className="emp-form-label">CCCD (Mã định danh) <span className="text-red-500">*</span></label>
          <input {...register('cccd')} className="emp-form-input" placeholder="Nhập 9-12 số căn cước" />
          {errors.cccd && <p className="emp-form-error"><ErrorIcon />{errors.cccd.message}</p>}
        </div>
        
        <div>
          <label className="emp-form-label">Họ và tên <span className="text-red-500">*</span></label>
          <input {...register('hoTen')} className="emp-form-input" placeholder="VD: Nguyễn Văn A" />
          {errors.hoTen && <p className="emp-form-error"><ErrorIcon />{errors.hoTen.message}</p>}
        </div>
        
        <div>
          <label className="emp-form-label">Email công việc</label>
          <input {...register('email')} type="email" className="emp-form-input" placeholder="VD: email@congty.com" />
          {errors.email && <p className="emp-form-error"><ErrorIcon />{errors.email.message}</p>}
        </div>
        
        <div className="sm:col-span-2">
          <label className="emp-form-label">Số điện thoại liên hệ</label>
          <input {...register('sdt')} className="emp-form-input" placeholder="VD: 0901234567" />
          {errors.sdt && <p className="emp-form-error"><ErrorIcon />{errors.sdt.message}</p>}
        </div>

        <div>
          <label className="emp-form-label">Số BHXH</label>
          <input {...register('soBhxh')} className="emp-form-input" placeholder="Nhập số BHXH" />
          {errors.soBhxh && <p className="emp-form-error"><ErrorIcon />{errors.soBhxh.message}</p>}
        </div>
        
        <div>
          <label className="emp-form-label">Số BHYT</label>
          <input {...register('soBhyt')} className="emp-form-input" placeholder="Nhập số BHYT" />
          {errors.soBhyt && <p className="emp-form-error"><ErrorIcon />{errors.soBhyt.message}</p>}
        </div>

        <div>
          <label className="emp-form-label">Số tài khoản ngân hàng</label>
          <input {...register('soTaiKhoan')} className="emp-form-input" placeholder="VD: 123456789" />
          {errors.soTaiKhoan && <p className="emp-form-error"><ErrorIcon />{errors.soTaiKhoan.message}</p>}
        </div>
        
        <div>
          <label className="emp-form-label">Tên ngân hàng</label>
          <input {...register('tenNganHang')} className="emp-form-input" placeholder="VD: Vietcombank" />
          {errors.tenNganHang && <p className="emp-form-error"><ErrorIcon />{errors.tenNganHang.message}</p>}
        </div>

        <div className="sm:col-span-2">
          <label className="emp-form-label">Mã số thuế cá nhân</label>
          <input {...register('maSoThue')} className="emp-form-input" placeholder="VD: 8200123456" />
          {errors.maSoThue && <p className="emp-form-error"><ErrorIcon />{errors.maSoThue.message}</p>}
        </div>
      </div>
    </div>
  );
};

const StepContract = () => {
  const { register, formState: { errors } } = useFormContext<CreateEmployeeFormValues>();
  
  return (
    <div className="mt-6 space-y-4 animate-[fadeIn_0.3s_ease-out]">
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-4 gap-y-4">
        <div className="sm:col-span-2">
          <label className="emp-form-label">Số Hợp Đồng <span className="text-red-500">*</span></label>
          <input {...register('soHopDong')} className="emp-form-input font-mono text-sm" placeholder="VD: HDLD-001/2026" />
          {errors.soHopDong && <p className="emp-form-error"><ErrorIcon />{errors.soHopDong.message}</p>}
        </div>
        
        <div>
          <label className="emp-form-label">Loại Hợp Đồng <span className="text-red-500">*</span></label>
          <select {...register('loaiHopDong')} className="emp-form-input cursor-pointer">
            <option value="">-- Chọn loại hợp đồng --</option>
            <option value="Thử việc">Thử việc (2 tháng)</option>
            <option value="Có thời hạn">Có thời hạn (1 năm)</option>
            <option value="Không thời hạn">Không thời hạn</option>
          </select>
          {errors.loaiHopDong && <p className="emp-form-error"><ErrorIcon />{errors.loaiHopDong.message}</p>}
        </div>
        
        <div>
          <label className="emp-form-label">Ngày bắt đầu <span className="text-red-500">*</span></label>
          <input {...register('ngayBatDauHopDong')} type="date" className="emp-form-input cursor-pointer" />
          {errors.ngayBatDauHopDong && <p className="emp-form-error"><ErrorIcon />{errors.ngayBatDauHopDong.message}</p>}
        </div>
        
        <div className="sm:col-span-2">
          <label className="emp-form-label">Lương Cơ Bản (VNĐ) <span className="text-red-500">*</span></label>
          <input 
            {...register('luongCoBan', { valueAsNumber: true })} 
            type="number" 
            className="emp-form-input font-mono" 
            placeholder="VD: 10000000" 
          />
          {errors.luongCoBan && <p className="emp-form-error"><ErrorIcon />{errors.luongCoBan.message}</p>}
        </div>
      </div>
    </div>
  );
};

const StepPosition = () => {
  const { register, formState: { errors } } = useFormContext<CreateEmployeeFormValues>();
  const { departments, positions, isLoading } = useSystemData();
  
  return (
    <div className="mt-6 space-y-4 animate-[fadeIn_0.3s_ease-out]">
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-4 gap-y-4">
        <div className="sm:col-span-2">
          <label className="emp-form-label">Số Quyết Định Bổ Nhiệm <span className="text-red-500">*</span></label>
          <input {...register('soQuyetDinh')} className="emp-form-input font-mono text-sm" placeholder="VD: QD-001/2026" />
          {errors.soQuyetDinh && <p className="emp-form-error"><ErrorIcon />{errors.soQuyetDinh.message}</p>}
        </div>
        
        <div>
          <label className="emp-form-label">Phòng Ban <span className="text-red-500">*</span></label>
          <select {...register('idPb')} className="emp-form-input cursor-pointer">
            <option value="">{isLoading ? 'Đang tải dữ liệu...' : '-- Chọn phòng ban --'}</option>
            {departments.map(d => <option key={d.idPb} value={d.idPb}>{d.tenPb}</option>)}
          </select>
          {errors.idPb && <p className="emp-form-error"><ErrorIcon />{errors.idPb.message}</p>}
        </div>
        
        <div>
          <label className="emp-form-label">Chức Vụ <span className="text-red-500">*</span></label>
          <select {...register('idChucVu')} className="emp-form-input cursor-pointer">
            <option value="">{isLoading ? 'Đang tải dữ liệu...' : '-- Chọn chức vụ --'}</option>
            {positions.map(p => <option key={p.idChucVu} value={p.idChucVu}>{p.tenChucVu}</option>)}
          </select>
          {errors.idChucVu && <p className="emp-form-error"><ErrorIcon />{errors.idChucVu.message}</p>}
        </div>
      </div>
    </div>
  );
};


interface Props {
  onSubmitSuccess: (data: CreateEmployeeFormValues) => Promise<boolean>;
  onCancel: () => void;
}

export const CreateEmployeeStepper: React.FC<Props> = ({ onSubmitSuccess, onCancel }) => {
  const [activeStep, setActiveStep] = useState(0);

  const methods = useForm<CreateEmployeeFormValues>({
    resolver: zodResolver(createEmployeeSchema),
    mode: 'onTouched',
  });

  const { trigger, handleSubmit, formState: { isSubmitting } } = methods;

  const handleNext = async () => {
    let fieldsToValidate: (keyof CreateEmployeeFormValues)[] = [];
    
    if (activeStep === 0) fieldsToValidate = ['cccd', 'hoTen', 'email', 'sdt'];
    if (activeStep === 1) fieldsToValidate = ['soHopDong', 'loaiHopDong', 'luongCoBan', 'ngayBatDauHopDong'];
    
    const isStepValid = await trigger(fieldsToValidate);
    if (isStepValid) setActiveStep((prev) => prev + 1);
  };

  const onSubmit = async (data: CreateEmployeeFormValues) => {
    await onSubmitSuccess(data);
  };

  return (
    <div className="w-full bg-white dark:bg-transparent">
      {/* Sử dụng Component Steps của Ant Design thay cho custom UI */}
      <div className="mb-6 px-4">
        <Steps
          current={activeStep}
          size="small"
          items={[
            { title: 'Thông tin cá nhân' },
            { title: 'Hợp đồng lao động' },
            { title: 'Vị trí công tác' }
          ]}
        />
      </div>

      <FormProvider {...methods}>
        <form onSubmit={handleSubmit(onSubmit)}>
          
          {/* Vùng chứa nội dung Form */}
          <div className="min-h-[260px] px-2">
            {activeStep === 0 && <StepPersonal />}
            {activeStep === 1 && <StepContract />}
            {activeStep === 2 && <StepPosition />}
          </div>

          {/* Khu vực Footer (Điều hướng) */}
          <div className="flex justify-between mt-8 pt-4 border-t border-gray-100 dark:border-gray-800">
            {activeStep === 0 ? (
               <button type="button" onClick={onCancel} className="emp-btn-cancel">
                 Hủy bỏ
               </button>
            ) : (
               <button type="button" onClick={() => setActiveStep((prev) => prev - 1)} className="emp-btn-cancel">
                 <span className="flex items-center gap-1">
                   <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-4 h-4"><path strokeLinecap="round" strokeLinejoin="round" d="M15.75 19.5L8.25 12l7.5-7.5" /></svg>
                   Quay lại
                 </span>
               </button>
            )}

            {activeStep === 2 ? (
              <button type="submit" disabled={isSubmitting} className="emp-btn-submit">
                {isSubmitting && (
                  <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                )}
                {isSubmitting ? 'Đang lưu...' : 'Hoàn tất & Lưu'}
              </button>
            ) : (
              <button type="button" onClick={handleNext} className="emp-btn-submit">
                Tiếp tục
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-4 h-4"><path strokeLinecap="round" strokeLinejoin="round" d="M8.25 4.5l7.5 7.5-7.5 7.5" /></svg>
              </button>
            )}
          </div>
        </form>
      </FormProvider>
    </div>
  );
};