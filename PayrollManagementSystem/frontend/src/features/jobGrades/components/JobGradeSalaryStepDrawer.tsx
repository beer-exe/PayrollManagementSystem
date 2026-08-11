import React, { useEffect, useState } from 'react';
import dayjs from 'dayjs';
import { salaryStepApi } from '../../salarySteps/api/salaryStepApi';
import { SalaryStepDto } from '../../salarySteps/types/salaryStep.types';
import { Toast } from '@/components/Toast/Toast';

interface Props {
  jobGradeId: string | null;
  jobGradeName: string;
  isOpen: boolean;
  onClose: () => void;
}

export const JobGradeSalaryStepDrawer: React.FC<Props> = ({ jobGradeId, jobGradeName, isOpen, onClose }) => {
  const [activeSteps, setActiveSteps] = useState<SalaryStepDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [historyData, setHistoryData] = useState<SalaryStepDto[]>([]);
  const [historyModalOpen, setHistoryModalOpen] = useState(false);
  const [formModalOpen, setFormModalOpen] = useState(false);
  const [isUpdatingVersion, setIsUpdatingVersion] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);
  
  const [formData, setFormData] = useState({
    stepName: '',
    p1Salary: '',
    effectiveDate: ''
  });
  const [formErrors, setFormErrors] = useState<Record<string, string>>({});

  const fetchActiveSteps = async () => {
    setLoading(true);
    try {
      const res = await salaryStepApi.getActive(jobGradeId!);
      if (res.succeeded) setActiveSteps(res.data);
    } finally { setLoading(false); }
  };

  useEffect(() => {
    if (isOpen && jobGradeId) fetchActiveSteps();
  }, [isOpen, jobGradeId]);

  const handleOpenCreate = () => {
    setIsUpdatingVersion(false);
    setFormData({ stepName: '', p1Salary: '', effectiveDate: '' });
    setFormErrors({});
    setFormModalOpen(true);
  };

  const handleOpenUpdateVersion = (record: SalaryStepDto) => {
    setIsUpdatingVersion(true);
    setFormData({
      stepName: record.stepName,
      p1Salary: record.p1Salary.toString(),
      effectiveDate: ''
    });
    setFormErrors({});
    setFormModalOpen(true);
  };

  const viewHistory = async (stepName: string) => {
    try {
      const res = await salaryStepApi.getHistory(jobGradeId!, stepName);
      if (res.succeeded) {
        setHistoryData(res.data);
        setHistoryModalOpen(true);
      }
    } catch (error) { 
      setToast({ message: "Lỗi tải dữ liệu lịch sử", type: "error" });
    }
  };

  const handleDelete = (stepName: string) => {
    if (window.confirm(`Xóa toàn bộ dữ liệu của ${stepName}? Sẽ bị chặn nếu đã áp dụng cho nhân sự.`)) {
      salaryStepApi.delete(jobGradeId!, stepName)
        .then(res => {
          if (res.succeeded) {
            setToast({ message: "Đã xóa thành công", type: "success" });
            fetchActiveSteps();
          }
        })
        .catch(error => {
          const err = error as import('axios').AxiosError<{Message?: string}>;
          setToast({ message: err.response?.data?.Message || "Xóa thất bại", type: "error" });
        });
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    if (formErrors[name]) setFormErrors(prev => ({ ...prev, [name]: '' }));
  };

  const validate = () => {
    const errors: Record<string, string> = {};
    if (!formData.stepName.trim()) errors.stepName = "Vui lòng nhập tên bậc";
    if (!formData.p1Salary) errors.p1Salary = "Vui lòng nhập mức lương P1";
    if (!formData.effectiveDate) errors.effectiveDate = "Vui lòng chọn ngày áp dụng";
    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleFormSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    try {
      if (isUpdatingVersion) {
        await salaryStepApi.updateVersion({
          jobGradeId: jobGradeId!,
          stepName: formData.stepName,
          newP1Salary: Number(formData.p1Salary),
          newEffectiveDate: formData.effectiveDate
        });
        setToast({ message: "Cập nhật phiên bản lương mới thành công!", type: "success" });
      } else {
        await salaryStepApi.create({
          jobGradeId: jobGradeId!,
          stepName: formData.stepName,
          p1Salary: Number(formData.p1Salary),
          effectiveDate: formData.effectiveDate
        });
        setToast({ message: "Tạo bậc lương thành công!", type: "success" });
      }
      setFormModalOpen(false);
      fetchActiveSteps();
    } catch (error) { 
      const err = error as import('axios').AxiosError<{Message?: string}>;
      setToast({ message: err.response?.data?.Message || 'Có lỗi xảy ra', type: "error" });
    }
  };

  if (!isOpen) return null;

  return (
    <>
      <div className="jg-drawer-overlay" onClick={onClose}>
        <div className="jg-drawer" onClick={e => e.stopPropagation()}>
        <div className="jg-drawer-header">
          <h2 className="jg-drawer-title">Cấu Hình Bậc Lương - Ngạch {jobGradeName}</h2>
          <button className="jg-drawer-close" onClick={onClose}>
            &times;
          </button>
        </div>

        <div className="jg-drawer-body custom-scrollbar">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem', flexWrap: 'wrap', gap: '1rem' }}>
            <button className="jg-btn jg-btn-primary" onClick={handleOpenCreate}>
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
              </svg>
              Thêm Bậc Mới
            </button>
          </div>

          <div className="jg-table-container custom-scrollbar" style={{ border: '1px solid var(--border-color)', borderRadius: '12px', background: 'var(--bg-surface)' }}>
            {loading ? (
              <div className="jg-loader">
                <div className="jg-spinner"></div>
              </div>
            ) : activeSteps.length > 0 ? (
              <table className="jg-table">
                <thead>
                  <tr>
                    <th>Tên Bậc</th>
                    <th>Mức Lương P1 (VNĐ)</th>
                    <th>Ngày Áp Dụng</th>
                    <th style={{ textAlign: 'center' }}>Trạng Thái</th>
                    <th style={{ textAlign: 'right' }}>Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {activeSteps.map(record => (
                    <tr key={record.id} className={record.status === 'CHUA_AP_DUNG' ? 'muted' : ''}>
                      <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{record.stepName}</td>
                      <td style={{ color: 'var(--success-text)', fontWeight: 600 }}>{record.p1Salary.toLocaleString('vi-VN')}</td>
                      <td>{dayjs(record.effectiveDate).format('DD/MM/YYYY')}</td>
                      <td style={{ textAlign: 'center' }}>
                        {record.status === 'CHUA_AP_DUNG' ? (
                          <span className="jg-badge jg-badge-warning">Chưa áp dụng</span>
                        ) : (
                          <span className="jg-badge jg-badge-success">Đang áp dụng</span>
                        )}
                      </td>
                      <td>
                        <div className="jg-actions">
                          <button className="jg-btn jg-btn-text" onClick={() => viewHistory(record.stepName)} title="Lịch sử">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.1rem', height: '1.1rem' }}>
                              <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                          </button>
                          <button className="jg-btn jg-btn-text info" onClick={() => handleOpenUpdateVersion(record)} title="Cập nhật phiên bản mới">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.1rem', height: '1.1rem' }}>
                              <path strokeLinecap="round" strokeLinejoin="round" d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L6.832 19.82a4.5 4.5 0 01-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 011.13-1.897L16.863 4.487zm0 0L19.5 7.125" />
                            </svg>
                          </button>
                          <button className="jg-btn jg-btn-text danger" onClick={() => handleDelete(record.stepName)} title="Xóa">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.1rem', height: '1.1rem' }}>
                              <path strokeLinecap="round" strokeLinejoin="round" d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0" />
                            </svg>
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            ) : (
              <div className="jg-empty">
                <p>Chưa có bậc lương nào được cấu hình cho ngạch này.</p>
              </div>
            )}
          </div>
        </div>
      </div>
      </div>

      {formModalOpen && (
        <div className="jg-modal-overlay" style={{ zIndex: 1100 }}>
          <div className="jg-modal">
            <div className="jg-modal-header">
              <h3 className="jg-modal-title">
                {isUpdatingVersion ? "Cập Nhật Phiên Bản Lương" : "Thêm Mới Bậc Lương"}
              </h3>
              <button className="jg-modal-close" onClick={() => setFormModalOpen(false)}>
                &times;
              </button>
            </div>
            
            <div className="jg-modal-body">
              <form id="step-form" onSubmit={handleFormSubmit}>
                <div className="jg-form-group">
                  <label className="jg-form-label">Tên Bậc <span className="required">*</span></label>
                  <input
                    type="text"
                    name="stepName"
                    value={formData.stepName}
                    onChange={handleInputChange}
                    disabled={isUpdatingVersion}
                    className="jg-form-input"
                    placeholder="VD: Bậc 1"
                  />
                  {formErrors.stepName && <span className="jg-form-error">{formErrors.stepName}</span>}
                </div>
                
                <div className="jg-form-group">
                  <label className="jg-form-label">Mức Lương P1 (VNĐ) <span className="required">*</span></label>
                  <input
                    type="number"
                    name="p1Salary"
                    value={formData.p1Salary}
                    onChange={handleInputChange}
                    className="jg-form-input"
                    placeholder="VD: 5000000"
                  />
                  {formErrors.p1Salary && <span className="jg-form-error">{formErrors.p1Salary}</span>}
                </div>

                <div className="jg-form-group">
                  <label className="jg-form-label">Ngày Áp Dụng Mới <span className="required">*</span></label>
                  <input
                    type="date"
                    name="effectiveDate"
                    value={formData.effectiveDate}
                    onChange={handleInputChange}
                    className="jg-form-input"
                  />
                  {formErrors.effectiveDate && <span className="jg-form-error">{formErrors.effectiveDate}</span>}
                </div>
              </form>
            </div>

            <div className="jg-modal-footer">
              <button type="button" className="jg-btn jg-btn-secondary" onClick={() => setFormModalOpen(false)}>Hủy bỏ</button>
              <button type="submit" form="step-form" className="jg-btn jg-btn-primary">Lưu lại</button>
            </div>
          </div>
        </div>
      )}

      {historyModalOpen && (
        <div className="jg-modal-overlay" style={{ zIndex: 1100 }}>
          <div className="jg-modal large">
            <div className="jg-modal-header">
              <h3 className="jg-modal-title">Lịch Sử Thay Đổi</h3>
              <button className="jg-modal-close" onClick={() => setHistoryModalOpen(false)}>
                &times;
              </button>
            </div>
            
            <div className="jg-modal-body custom-scrollbar" style={{ padding: '0', maxHeight: '60vh' }}>
              <table className="jg-table">
                <thead style={{ position: 'sticky', top: 0, zIndex: 2 }}>
                  <tr>
                    <th>Mức Lương P1 (VNĐ)</th>
                    <th>Từ Ngày</th>
                    <th>Đến Ngày</th>
                    <th style={{ textAlign: 'center' }}>Trạng Thái</th>
                  </tr>
                </thead>
                <tbody>
                  {historyData.map(record => (
                    <tr key={record.id}>
                      <td style={{ color: 'var(--success-text)', fontWeight: 600 }}>{record.p1Salary.toLocaleString('vi-VN')}</td>
                      <td>{dayjs(record.effectiveDate).format('DD/MM/YYYY')}</td>
                      <td>{record.endDate ? dayjs(record.endDate).format('DD/MM/YYYY') : 'Hiện tại'}</td>
                      <td style={{ textAlign: 'center' }}>
                        {record.status === 'HIEU_LUC' ? (
                          <span className="jg-badge jg-badge-success">Hiệu lực</span>
                        ) : (
                          <span className="jg-badge jg-badge-gray">Hết hạn</span>
                        )}
                      </td>
                    </tr>
                  ))}
                  {historyData.length === 0 && (
                    <tr>
                      <td colSpan={4} style={{ textAlign: 'center', padding: '2rem' }}>Không có lịch sử</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
    </>
  );
};