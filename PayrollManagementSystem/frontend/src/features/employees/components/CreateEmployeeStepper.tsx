import React, { useState, useEffect } from 'react';
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
import { salaryStepApi } from '../../salarySteps/api/salaryStepApi';
import { SalaryStepDto } from '../../salarySteps/types/salaryStep.types';
import './EmployeeModals.css';

// Custom Error Icon
const ErrorIcon = () => (
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" style={{width: '1rem', height: '1rem', flexShrink: 0, marginTop: '0.1rem'}}>
    <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-5a.75.75 0 01.75.75v4.5a.75.75 0 01-1.5 0v-4.5A.75.75 0 0110 5zm0 10a1 1 0 100-2 1 1 0 000 2z" clipRule="evenodd" />
  </svg>
);

// Form Input Wrapper with Icon
const IconInput = ({ icon, ...props }: any) => (
  <div className="emp-stepper-icon-input">
    <div className="icon-wrapper">
      {icon}
    </div>
    <input {...props} className={`emp-form-input ${props.className || ''}`} />
  </div>
);

// Form Select Wrapper
const IconSelect = ({ icon, children, ...props }: any) => (
  <div className="emp-stepper-icon-input">
    <div className="icon-wrapper">
      {icon}
    </div>
    <select {...props} className={`emp-form-select ${props.className || ''}`}>
      {children}
    </select>
  </div>
);

const StepPersonal = () => {
  const { register, formState: { errors } } = useFormContext<CreateEmployeeFormValues>();
  
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
      <div>
        <h3 style={{ fontSize: '1.1rem', fontWeight: 700, color: '#111827', marginBottom: '1rem', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.5rem' }}>Thông tin định danh</h3>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.25rem' }}>
          <div style={{ gridColumn: '1 / -1' }}>
            <label className="emp-form-label">CCCD (Mã định danh) <span className="required">*</span></label>
            <IconInput icon={<IdcardOutlined />} {...register('cccd')} placeholder="Nhập 9-12 số căn cước" />
            {errors.cccd && <p className="emp-form-error"><ErrorIcon />{errors.cccd.message}</p>}
          </div>
          
          <div>
            <label className="emp-form-label">Họ và tên <span className="required">*</span></label>
            <IconInput icon={<UserOutlined />} {...register('hoTen')} placeholder="VD: Nguyễn Văn A" />
            {errors.hoTen && <p className="emp-form-error"><ErrorIcon />{errors.hoTen.message}</p>}
          </div>
          
          <div>
            <label className="emp-form-label">Số điện thoại</label>
            <IconInput icon={<PhoneOutlined />} {...register('sdt')} placeholder="VD: 0901234567" />
            {errors.sdt && <p className="emp-form-error"><ErrorIcon />{errors.sdt.message}</p>}
          </div>

          <div style={{ gridColumn: '1 / -1' }}>
            <label className="emp-form-label">Email công việc</label>
            <IconInput icon={<MailOutlined />} {...register('email')} type="email" placeholder="VD: email@congty.com" />
            {errors.email && <p className="emp-form-error"><ErrorIcon />{errors.email.message}</p>}
          </div>
        </div>
      </div>

      <div>
        <h3 style={{ fontSize: '1.1rem', fontWeight: 700, color: '#111827', marginBottom: '1rem', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.5rem' }}>Thông tin tài chính & BHXH</h3>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.25rem' }}>
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

          <div style={{ gridColumn: '1 / -1' }}>
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
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
      <div>
        <h3 style={{ fontSize: '1.1rem', fontWeight: 700, color: '#111827', marginBottom: '1rem', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.5rem' }}>Hợp đồng lao động</h3>
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
          <div>
            <label className="emp-form-label">Số Hợp Đồng <span className="required">*</span></label>
            <IconInput icon={<FileTextOutlined />} {...register('soHopDong')} style={{ fontFamily: 'monospace' }} placeholder="VD: HDLD-001/2026" />
            {errors.soHopDong && <p className="emp-form-error"><ErrorIcon />{errors.soHopDong.message}</p>}
          </div>
          
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.25rem' }}>
            <div>
              <label className="emp-form-label">Loại Hợp Đồng <span className="required">*</span></label>
              <IconSelect icon={<FileTextOutlined />} {...register('loaiHopDong')}>
                <option value="">-- Chọn loại HĐ --</option>
                <option value="Thử việc">Thử việc (2 tháng)</option>
                <option value="Có thời hạn">Có thời hạn (1 năm)</option>
                <option value="Không thời hạn">Không thời hạn</option>
              </IconSelect>
              {errors.loaiHopDong && <p className="emp-form-error"><ErrorIcon />{errors.loaiHopDong.message}</p>}
            </div>
            
            <div>
              <label className="emp-form-label">Ngày bắt đầu <span className="required">*</span></label>
              <IconInput icon={<CalendarOutlined />} {...register('ngayBatDauHopDong')} type="date" />
              {errors.ngayBatDauHopDong && <p className="emp-form-error"><ErrorIcon />{errors.ngayBatDauHopDong.message}</p>}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

const StepPosition = () => {
  const { register, watch, setValue, formState: { errors } } = useFormContext<CreateEmployeeFormValues>();
  const { departments, positions, isLoading } = useSystemData();
  const [bacLuongs, setBacLuongs] = useState<SalaryStepDto[]>([]);
  const [loadingBacLuong, setLoadingBacLuong] = useState(false);
  
  const selectedIdPb = watch('idPb');
  const selectedIdChucVu = watch('idChucVu');

  const filteredPositions = positions.filter(p => p.idPhongBan === selectedIdPb);

  useEffect(() => {
    if (!selectedIdPb) {
      setValue('idChucVu', '');
    }
  }, [selectedIdPb, setValue]);

  useEffect(() => {
    const fetchBacLuongs = async () => {
      if (!selectedIdChucVu) {
        setBacLuongs([]);
        setValue('idBacLuong', '');
        return;
      }
      
      const pos = positions.find(p => p.idChucVu === selectedIdChucVu);
      if (pos?.idNgachLuong) {
        setLoadingBacLuong(true);
        try {
          const res = await salaryStepApi.getActive(pos.idNgachLuong);
          if (res.succeeded) {
            setBacLuongs(res.data);
          } else {
            setBacLuongs([]);
          }
        } catch (error) {
          setBacLuongs([]);
        } finally {
          setLoadingBacLuong(false);
        }
      } else {
        setBacLuongs([]);
        setValue('idBacLuong', '');
      }
    };
    fetchBacLuongs();
  }, [selectedIdChucVu, positions, setValue]);
  
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
      <div>
        <h3 style={{ fontSize: '1.1rem', fontWeight: 700, color: '#111827', marginBottom: '1rem', borderBottom: '1px solid #e5e7eb', paddingBottom: '0.5rem' }}>Phân công công tác</h3>
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
          <div>
            <label className="emp-form-label">Số QĐ Bổ Nhiệm <span className="required">*</span></label>
            <IconInput icon={<FileTextOutlined />} {...register('soQuyetDinh')} style={{ fontFamily: 'monospace' }} placeholder="VD: QD-001/2026" />
            {errors.soQuyetDinh && <p className="emp-form-error"><ErrorIcon />{errors.soQuyetDinh.message}</p>}
          </div>
          
          <div>
            <label className="emp-form-label">Phòng Ban <span className="required">*</span></label>
            <IconSelect icon={<ClusterOutlined />} {...register('idPb')}>
              <option value="">{isLoading ? 'Đang tải dữ liệu...' : '-- Chọn phòng ban --'}</option>
              {departments.map(d => <option key={d.idPb} value={d.idPb}>{d.tenPb}</option>)}
            </IconSelect>
            {errors.idPb && <p className="emp-form-error"><ErrorIcon />{errors.idPb.message}</p>}
          </div>
          
          <div>
            <label className="emp-form-label">Chức Vụ <span className="required">*</span></label>
            <IconSelect icon={<UserOutlined />} {...register('idChucVu')} disabled={!selectedIdPb || isLoading}>
              <option value="">{isLoading ? 'Đang tải dữ liệu...' : (!selectedIdPb ? '-- Vui lòng chọn phòng ban trước --' : '-- Chọn chức vụ --')}</option>
              {filteredPositions.map(p => <option key={p.idChucVu} value={p.idChucVu}>{p.tenChucVu}</option>)}
            </IconSelect>
            {errors.idChucVu && <p className="emp-form-error"><ErrorIcon />{errors.idChucVu.message}</p>}
          </div>

          <div>
            <label className="emp-form-label">Bậc Lương (Ngạch Lương) <span className="required">*</span></label>
            <IconSelect icon={<DollarOutlined />} {...register('idBacLuong')} disabled={!selectedIdChucVu || loadingBacLuong || bacLuongs.length === 0}>
              <option value="">{loadingBacLuong ? 'Đang tải dữ liệu...' : (!selectedIdChucVu ? '-- Vui lòng chọn chức vụ trước --' : (bacLuongs.length === 0 ? '-- Không có dữ liệu bậc lương --' : '-- Chọn bậc lương --'))}</option>
              {bacLuongs.map(b => <option key={b.id} value={b.id}>{b.stepName} - {b.p1Salary.toLocaleString()} VNĐ</option>)}
            </IconSelect>
            {errors.idBacLuong && <p className="emp-form-error"><ErrorIcon />{errors.idBacLuong.message}</p>}
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
    const step1Fields = ['soHopDong', 'loaiHopDong', 'ngayBatDauHopDong'];
    
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
    <div className="emp-stepper-container">
      
      {/* Nút Đóng Tuyệt Đối */}
      <button 
        onClick={onCancel}
        style={{ position: 'absolute', top: '1rem', right: '1rem', zIndex: 50, background: 'transparent', border: 'none', color: '#9ca3af', cursor: 'pointer', padding: '0.5rem', borderRadius: '50%', transition: 'all 0.2s' }}
        onMouseOver={e => { e.currentTarget.style.background = '#f3f4f6'; e.currentTarget.style.color = '#4b5563'; }}
        onMouseOut={e => { e.currentTarget.style.background = 'transparent'; e.currentTarget.style.color = '#9ca3af'; }}
      >
        <CloseOutlined style={{ fontSize: '1.25rem' }} />
      </button>

      {/* Sidebar Trái */}
      <div className="emp-stepper-sidebar">
        <div>
          <div style={{ display: 'none', width: '3rem', height: '3rem', background: 'rgba(255,255,255,0.2)', borderRadius: '1rem', alignItems: 'center', justifyContent: 'center', marginBottom: '1.5rem', border: '1px solid rgba(255,255,255,0.3)', boxShadow: '0 4px 6px rgba(0,0,0,0.1)' }} className="md-flex">
            <UserOutlined style={{ fontSize: '1.5rem' }} />
          </div>
          <div>
            <h2 style={{ fontSize: '1.75rem', fontWeight: 700, margin: '0 0 0.5rem 0' }}>Thêm Nhân Sự</h2>
            <p style={{ color: '#c7d2fe', fontSize: '0.85rem', lineHeight: 1.5, margin: '0 0 2rem 0' }}>
              Thiết lập hồ sơ nhân sự mới trong hệ thống chỉ với 3 bước đơn giản.
            </p>
          </div>

          <div className="emp-steps-list">
            {steps.map((step) => {
              let statusClass = 'future';
              if (activeStep > step.id) statusClass = 'past';
              else if (activeStep === step.id) statusClass = 'current';

              let textClass = 'other';
              if (activeStep === step.id) textClass = 'current';

              return (
                <div key={step.id} className="emp-step-item">
                  <div className={`emp-step-icon ${statusClass}`}>
                    {activeStep > step.id ? <CheckCircleFilled /> : <span>{step.id + 1}</span>}
                  </div>
                  <div className={`emp-step-text ${textClass}`}>
                    <h4>{step.title}</h4>
                    <p>{step.description}</p>
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        <div style={{ fontSize: '0.75rem', color: 'rgba(199, 210, 254, 0.6)', marginTop: '2.5rem' }}>
          Hoàn thành: {Math.round((activeStep / 2) * 100)}%
          <div style={{ width: '100%', background: 'rgba(49, 46, 129, 0.5)', height: '0.35rem', borderRadius: '99px', marginTop: '0.5rem', overflow: 'hidden' }}>
            <div style={{ background: '#4ade80', height: '100%', transition: 'width 0.5s', width: `${(activeStep / 2) * 100}%` }}></div>
          </div>
        </div>
      </div>

      {/* Nội dung Form Bên Phải */}
      <div className="emp-stepper-content">
        <FormProvider {...methods}>
          <form onSubmit={handleSubmit(onSubmit, onError)} style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
            
            <div className="emp-stepper-body custom-scrollbar">
              <AnimatePresence mode="wait">
                <motion.div
                  key={activeStep}
                  variants={slideVariants}
                  initial="initial"
                  animate="animate"
                  exit="exit"
                  transition={{ duration: 0.3, ease: 'easeInOut' }}
                  style={{ maxWidth: '42rem', margin: '0 auto', width: '100%' }}
                >
                  {activeStep === 0 && <StepPersonal />}
                  {activeStep === 1 && <StepContract />}
                  {activeStep === 2 && <StepPosition />}
                </motion.div>
              </AnimatePresence>
            </div>

            <div className="emp-stepper-footer">
              {activeStep === 0 ? (
                <button type="button" onClick={onCancel} className="emp-btn-cancel">Hủy bỏ</button>
              ) : (
                <button type="button" onClick={() => setActiveStep(prev => prev - 1)} className="emp-btn-cancel" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}><path strokeLinecap="round" strokeLinejoin="round" d="M15.75 19.5L8.25 12l7.5-7.5" /></svg>
                  Quay lại
                </button>
              )}

              <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                {activeStep < 2 && (
                  <button type="button" onClick={handleNext} className="emp-btn-cancel" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', color: '#7c3aed', borderColor: '#ddd6fe' }}>
                    Tiếp tục
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}><path strokeLinecap="round" strokeLinejoin="round" d="M8.25 4.5l7.5 7.5-7.5 7.5" /></svg>
                  </button>
                )}
                
                <button type="submit" disabled={isSubmitting} className="emp-btn-submit" style={{ minWidth: '140px', justifyContent: 'center' }}>
                  {isSubmitting ? 'Đang xử lý...' : 'Lưu nhân viên'}
                </button>
              </div>
            </div>
            
          </form>
        </FormProvider>
      </div>
    </div>
  );
};