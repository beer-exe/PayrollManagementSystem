import React, { useState } from 'react';
import { useForm, FormProvider, useFormContext } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { motion, AnimatePresence } from 'framer-motion';
import { 
  UserOutlined, 
  IdcardOutlined, 
  MailOutlined, 
  PhoneOutlined, 
  BankOutlined, 
  FileTextOutlined, 
  DollarOutlined, 
  CalendarOutlined, 
  ClusterOutlined, 
  CloseOutlined,
  CheckCircleFilled,
  SafetyCertificateOutlined
} from '@ant-design/icons';
import { createEmployeeSchema, CreateEmployeeFormValues } from '../schemas/employeeSchema';
import { useSystemData } from '../../departments/hooks/useSystemData';
import './EmployeeModals.css';

// Custom Error Icon
const ErrorIcon = () => (
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" className="w-4 h-4 shrink-0 mt-0.5">
    <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-5a.75.75 0 01.75.75v4.5a.75.75 0 01-1.5 0v-4.5A.75.75 0 0110 5zm0 10a1 1 0 100-2 1 1 0 000 2z" clipRule="evenodd" />
  </svg>
);

// Form Input Wrapper with Icon
const IconInput = ({ icon, ...props }: any) => (
  <div className="relative">
    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-gray-400">
      {icon}
    </div>
    <input {...props} className={`emp-form-input pl-10 ${props.className || ''}`} />
  </div>
);

// Form Select Wrapper
const IconSelect = ({ icon, children, ...props }: any) => (
  <div className="relative">
    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-gray-400">
      {icon}
    </div>
    <select {...props} className={`emp-form-input pl-10 appearance-none ${props.className || ''}`}>
      {children}
    </select>
  </div>
);

const StepPersonal = () => {
  const { register, formState: { errors } } = useFormContext<CreateEmployeeFormValues>();
  
  return (
    <div className="space-y-6">
      <div>
        <h3 className="text-lg font-bold text-gray-800 dark:text-gray-100 mb-4 border-b pb-2">Thông tin định danh</h3>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-5 gap-y-4">
          <div className="sm:col-span-2">
            <label className="emp-form-label">CCCD (Mã định danh) <span className="text-red-500">*</span></label>
            <IconInput icon={<IdcardOutlined />} {...register('cccd')} placeholder="Nhập 9-12 số căn cước" />
            {errors.cccd && <p className="emp-form-error"><ErrorIcon />{errors.cccd.message}</p>}
          </div>
          
          <div>
            <label className="emp-form-label">Họ và tên <span className="text-red-500">*</span></label>
            <IconInput icon={<UserOutlined />} {...register('hoTen')} placeholder="VD: Nguyễn Văn A" />
            {errors.hoTen && <p className="emp-form-error"><ErrorIcon />{errors.hoTen.message}</p>}
          </div>
          
          <div>
            <label className="emp-form-label">Số điện thoại</label>
            <IconInput icon={<PhoneOutlined />} {...register('sdt')} placeholder="VD: 0901234567" />
            {errors.sdt && <p className="emp-form-error"><ErrorIcon />{errors.sdt.message}</p>}
          </div>

          <div className="sm:col-span-2">
            <label className="emp-form-label">Email công việc</label>
            <IconInput icon={<MailOutlined />} {...register('email')} type="email" placeholder="VD: email@congty.com" />
            {errors.email && <p className="emp-form-error"><ErrorIcon />{errors.email.message}</p>}
          </div>
        </div>
      </div>

      <div>
        <h3 className="text-lg font-bold text-gray-800 dark:text-gray-100 mb-4 border-b pb-2">Thông tin tài chính & BHXH</h3>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-5 gap-y-4">
          <div>
            <label className="emp-form-label">Số tài khoản ngân hàng</label>
            <IconInput icon={<DollarOutlined />} {...register('soTaiKhoan')} placeholder="VD: 123456789" />
            {errors.soTaiKhoan && <p className="emp-form-error"><ErrorIcon />{errors.soTaiKhoan.message}</p>}
          </div>
          
          <div>
            <label className="emp-form-label">Tên ngân hàng</label>
            <IconInput icon={<BankOutlined />} {...register('tenNganHang')} placeholder="VD: Vietcombank" />
            {errors.tenNganHang && <p className="emp-form-error"><ErrorIcon />{errors.tenNganHang.message}</p>}
          </div>

          <div className="sm:col-span-2">
            <label className="emp-form-label">Mã số thuế cá nhân</label>
            <IconInput icon={<FileTextOutlined />} {...register('maSoThue')} placeholder="VD: 8200123456" />
            {errors.maSoThue && <p className="emp-form-error"><ErrorIcon />{errors.maSoThue.message}</p>}
          </div>

          <div>
            <label className="emp-form-label">Số BHXH</label>
            <IconInput icon={<SafetyCertificateOutlined />} {...register('soBhxh')} placeholder="Nhập số BHXH" />
            {errors.soBhxh && <p className="emp-form-error"><ErrorIcon />{errors.soBhxh.message}</p>}
          </div>
          
          <div>
            <label className="emp-form-label">Số BHYT</label>
            <IconInput icon={<SafetyCertificateOutlined />} {...register('soBhyt')} placeholder="Nhập số BHYT" />
            {errors.soBhyt && <p className="emp-form-error"><ErrorIcon />{errors.soBhyt.message}</p>}
          </div>
        </div>
      </div>
    </div>
  );
};

const StepContract = () => {
  const { register, formState: { errors } } = useFormContext<CreateEmployeeFormValues>();
  
  return (
    <div className="space-y-6">
      <div>
        <h3 className="text-lg font-bold text-gray-800 dark:text-gray-100 mb-4 border-b pb-2">Hợp đồng lao động</h3>
        <div className="grid grid-cols-1 gap-y-5">
          <div>
            <label className="emp-form-label">Số Hợp Đồng <span className="text-red-500">*</span></label>
            <IconInput icon={<FileTextOutlined />} {...register('soHopDong')} className="font-mono" placeholder="VD: HDLD-001/2026" />
            {errors.soHopDong && <p className="emp-form-error"><ErrorIcon />{errors.soHopDong.message}</p>}
          </div>
          
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-5 gap-y-5">
            <div>
              <label className="emp-form-label">Loại Hợp Đồng <span className="text-red-500">*</span></label>
              <IconSelect icon={<FileTextOutlined />} {...register('loaiHopDong')}>
                <option value="">-- Chọn loại HĐ --</option>
                <option value="Thử việc">Thử việc (2 tháng)</option>
                <option value="Có thời hạn">Có thời hạn (1 năm)</option>
                <option value="Không thời hạn">Không thời hạn</option>
              </IconSelect>
              {errors.loaiHopDong && <p className="emp-form-error"><ErrorIcon />{errors.loaiHopDong.message}</p>}
            </div>
            
            <div>
              <label className="emp-form-label">Ngày bắt đầu <span className="text-red-500">*</span></label>
              <IconInput icon={<CalendarOutlined />} {...register('ngayBatDauHopDong')} type="date" />
              {errors.ngayBatDauHopDong && <p className="emp-form-error"><ErrorIcon />{errors.ngayBatDauHopDong.message}</p>}
            </div>
          </div>
          
          <div>
            <label className="emp-form-label">Lương Cơ Bản (VNĐ) <span className="text-red-500">*</span></label>
            <IconInput 
              icon={<DollarOutlined />}
              {...register('luongCoBan', { valueAsNumber: true })} 
              type="number" 
              className="font-mono text-lg text-violet-700 dark:text-violet-400 font-bold" 
              placeholder="VD: 10000000" 
            />
            {errors.luongCoBan && <p className="emp-form-error"><ErrorIcon />{errors.luongCoBan.message}</p>}
          </div>
        </div>
      </div>
    </div>
  );
};

const StepPosition = () => {
  const { register, formState: { errors } } = useFormContext<CreateEmployeeFormValues>();
  const { departments, positions, isLoading } = useSystemData();
  
  return (
    <div className="space-y-6">
      <div>
        <h3 className="text-lg font-bold text-gray-800 dark:text-gray-100 mb-4 border-b pb-2">Phân công công tác</h3>
        <div className="grid grid-cols-1 gap-y-5">
          <div>
            <label className="emp-form-label">Số QĐ Bổ Nhiệm <span className="text-red-500">*</span></label>
            <IconInput icon={<FileTextOutlined />} {...register('soQuyetDinh')} className="font-mono" placeholder="VD: QD-001/2026" />
            {errors.soQuyetDinh && <p className="emp-form-error"><ErrorIcon />{errors.soQuyetDinh.message}</p>}
          </div>
          
          <div>
            <label className="emp-form-label">Phòng Ban <span className="text-red-500">*</span></label>
            <IconSelect icon={<ClusterOutlined />} {...register('idPb')}>
              <option value="">{isLoading ? 'Đang tải dữ liệu...' : '-- Chọn phòng ban --'}</option>
              {departments.map(d => <option key={d.idPb} value={d.idPb}>{d.tenPb}</option>)}
            </IconSelect>
            {errors.idPb && <p className="emp-form-error"><ErrorIcon />{errors.idPb.message}</p>}
          </div>
          
          <div>
            <label className="emp-form-label">Chức Vụ <span className="text-red-500">*</span></label>
            <IconSelect icon={<UserOutlined />} {...register('idChucVu')}>
              <option value="">{isLoading ? 'Đang tải dữ liệu...' : '-- Chọn chức vụ --'}</option>
              {positions.map(p => <option key={p.idChucVu} value={p.idChucVu}>{p.tenChucVu}</option>)}
            </IconSelect>
            {errors.idChucVu && <p className="emp-form-error"><ErrorIcon />{errors.idChucVu.message}</p>}
          </div>
        </div>
      </div>
    </div>
  );
};

interface Props {
  onSubmitSuccess: (data: CreateEmployeeFormValues) => Promise<boolean>;
  onCancel: () => void;
}

const steps = [
  { id: 0, title: 'Thông tin cá nhân', description: 'Định danh & liên hệ' },
  { id: 1, title: 'Hợp đồng', description: 'Loại HĐ & Mức lương' },
  { id: 2, title: 'Phân công', description: 'Vị trí công tác' }
];

export const CreateEmployeeStepper: React.FC<Props> = ({ onSubmitSuccess, onCancel }) => {
  const [activeStep, setActiveStep] = useState(0);

  const methods = useForm<CreateEmployeeFormValues>({
    resolver: zodResolver(createEmployeeSchema),
    mode: 'onTouched',
  });

  const { handleSubmit, formState: { isSubmitting } } = methods;

  const handleNext = () => {
    setActiveStep((prev) => prev + 1);
  };

  const onError = (errors: any) => {
    const step0Fields = ['cccd', 'hoTen', 'email', 'sdt', 'soTaiKhoan', 'tenNganHang', 'maSoThue', 'soBhxh', 'soBhyt'];
    const step1Fields = ['soHopDong', 'loaiHopDong', 'luongCoBan', 'ngayBatDauHopDong'];
    
    if (step0Fields.some(field => errors[field])) {
      setActiveStep(0);
    } else if (step1Fields.some(field => errors[field])) {
      setActiveStep(1);
    } else {
      setActiveStep(2);
    }
  };

  const onSubmit = async (data: CreateEmployeeFormValues) => {
    await onSubmitSuccess(data);
  };

  // Animation variants
  const slideVariants = {
    initial: { x: 20, opacity: 0 },
    animate: { x: 0, opacity: 1 },
    exit: { x: -20, opacity: 0 }
  };

  return (
    <div className="flex h-[700px] bg-white dark:bg-gray-900 overflow-hidden w-full relative">
      
      {/* Nút Đóng Tuyệt Đối */}
      <button 
        onClick={onCancel}
        className="absolute top-4 right-4 z-50 p-2 rounded-full text-gray-400 hover:bg-gray-100 hover:text-gray-600 dark:hover:bg-gray-800 transition-colors"
      >
        <CloseOutlined className="text-xl" />
      </button>

      {/* Sidebar Trái (Gradient) */}
      <div className="w-[320px] bg-gradient-to-br from-violet-600 to-indigo-900 text-white p-8 flex flex-col justify-between shrink-0 shadow-[4px_0_24px_rgba(0,0,0,0.1)] relative z-10">
        <div>
          <div className="w-12 h-12 bg-white/20 rounded-2xl flex items-center justify-center mb-6 backdrop-blur-sm border border-white/30 shadow-lg">
            <UserOutlined className="text-2xl text-white" />
          </div>
          <h2 className="text-3xl font-bold mb-2 tracking-tight">Thêm Nhân Sự</h2>
          <p className="text-indigo-200 text-sm mb-10 leading-relaxed">
            Thiết lập hồ sơ nhân sự mới trong hệ thống chỉ với 3 bước đơn giản.
          </p>

          <div className="space-y-8 relative before:absolute before:inset-0 before:ml-[11px] before:-translate-x-px md:before:mx-auto md:before:translate-x-0 before:h-full before:w-0.5 before:bg-gradient-to-b before:from-transparent before:via-white/20 before:to-transparent">
            {steps.map((step) => (
              <div key={step.id} className="relative flex items-center gap-4 group">
                <div className={`w-6 h-6 rounded-full flex items-center justify-center z-10 transition-all duration-300 shadow-sm ${
                  activeStep > step.id ? 'bg-green-400 text-indigo-900 ring-4 ring-green-400/30' : 
                  activeStep === step.id ? 'bg-white text-violet-600 ring-4 ring-white/30 scale-110' : 
                  'bg-indigo-800/50 text-white/50 border border-white/20'
                }`}>
                  {activeStep > step.id ? <CheckCircleFilled className="text-sm" /> : <span className="text-xs font-bold">{step.id + 1}</span>}
                </div>
                <div className={`transition-all duration-300 ${activeStep === step.id ? 'opacity-100 translate-x-1' : 'opacity-60'}`}>
                  <h4 className={`text-sm font-bold ${activeStep === step.id ? 'text-white' : 'text-indigo-200'}`}>{step.title}</h4>
                  <p className="text-xs text-indigo-300/80">{step.description}</p>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="text-xs text-indigo-300/60 mt-10">
          Hoàn thành: {Math.round((activeStep / 2) * 100)}%
          <div className="w-full bg-indigo-900/50 h-1.5 rounded-full mt-2 overflow-hidden">
            <div className="bg-green-400 h-full rounded-full transition-all duration-500" style={{ width: `${(activeStep / 2) * 100}%` }}></div>
          </div>
        </div>
      </div>

      {/* Nội dung Form Bên Phải */}
      <div className="flex-1 flex flex-col relative bg-gray-50/50 dark:bg-gray-900/50">
        <FormProvider {...methods}>
          <form onSubmit={handleSubmit(onSubmit, onError)} className="flex flex-col h-full">
            
            {/* Vùng cuộn */}
            <div className="flex-1 overflow-y-auto p-10 custom-scrollbar">
              <AnimatePresence mode="wait">
                <motion.div
                  key={activeStep}
                  variants={slideVariants}
                  initial="initial"
                  animate="animate"
                  exit="exit"
                  transition={{ duration: 0.3, ease: 'easeInOut' }}
                  className="max-w-2xl mx-auto w-full"
                >
                  {activeStep === 0 && <StepPersonal />}
                  {activeStep === 1 && <StepContract />}
                  {activeStep === 2 && <StepPosition />}
                </motion.div>
              </AnimatePresence>
            </div>

            {/* Footer */}
            <div className="p-6 bg-white dark:bg-gray-800 border-t border-gray-100 dark:border-gray-700 flex justify-between items-center shrink-0 shadow-[0_-4px_6px_-1px_rgba(0,0,0,0.02)] relative z-10">
              {activeStep === 0 ? (
                <button type="button" onClick={onCancel} className="emp-btn-cancel text-gray-500">Hủy bỏ</button>
              ) : (
                <button type="button" onClick={() => setActiveStep(prev => prev - 1)} className="emp-btn-cancel group flex items-center gap-2">
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-4 h-4 group-hover:-translate-x-1 transition-transform"><path strokeLinecap="round" strokeLinejoin="round" d="M15.75 19.5L8.25 12l7.5-7.5" /></svg>
                  Quay lại
                </button>
              )}

              <div className="flex items-center gap-3">
                {activeStep < 2 && (
                  <button type="button" onClick={handleNext} className="emp-btn-cancel group flex items-center gap-2 text-violet-600 border-violet-200 hover:border-violet-300 hover:bg-violet-50">
                    Tiếp tục
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-4 h-4 group-hover:translate-x-1 transition-transform"><path strokeLinecap="round" strokeLinejoin="round" d="M8.25 4.5l7.5 7.5-7.5 7.5" /></svg>
                  </button>
                )}
                
                <button type="submit" disabled={isSubmitting} className="emp-btn-submit min-w-[140px]">
                  {isSubmitting ? (
                    <span className="flex items-center gap-2">
                      <svg className="animate-spin h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                      </svg>
                      Đang xử lý
                    </span>
                  ) : 'Lưu nhân viên'}
                </button>
              </div>
            </div>
            
          </form>
        </FormProvider>
      </div>
    </div>
  );
};