import React, { useState } from 'react';
import { profileApi } from '../../profile/api/profileApi';
import { useAuthStore } from '@/store/useAuthStore';
import { useNavigate } from 'react-router-dom';
import './ChangePasswordModal.css';

interface Props {
  isOpen: boolean;
  onClose: () => void;
}

const EyeIcon = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
    <circle cx="12" cy="12" r="3"></circle>
  </svg>
);

const EyeOffIcon = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path>
    <line x1="1" y1="1" x2="23" y2="23"></line>
  </svg>
);

export const ChangePasswordModal: React.FC<Props> = ({ isOpen, onClose }) => {
  const { logout } = useAuthStore();
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    oldPassword: '',
    newPassword: '',
    confirmNewPassword: ''
  });
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [success, setSuccess] = useState(false);
  const [showOldPassword, setShowOldPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  if (!isOpen) return null;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
    setError('');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.oldPassword || !formData.newPassword || !formData.confirmNewPassword) {
      setError('Vui lòng điền đầy đủ các trường.');
      return;
    }

    if (formData.newPassword.length < 6) {
      setError('Mật khẩu mới phải có ít nhất 6 ký tự.');
      return;
    }

    if (formData.newPassword !== formData.confirmNewPassword) {
      setError('Xác nhận mật khẩu không khớp với mật khẩu mới.');
      return;
    }

    setIsSubmitting(true);
    setError('');

    try {
      const payload = {
        oldPassword: formData.oldPassword,
        newPassword: formData.newPassword,
        confirmNewPassword: formData.confirmNewPassword
      };
      
      const response = await profileApi.changePassword(payload);
      const result = response as unknown as { succeeded?: boolean; message?: string; data?: unknown };

      if (result && result.succeeded) {
        setSuccess(true);
        setTimeout(() => {
          handleClose();
          logout();
          navigate('/login');
        }, 1500);
      } else {
        setError(result?.message || 'Có lỗi xảy ra khi đổi mật khẩu.');
      }
    } catch (err: unknown) {
      const error = err as { response?: { data?: { Message?: string } }, message?: string };
      setError(error.response?.data?.Message || error.message || 'Đổi mật khẩu thất bại.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    setFormData({
      oldPassword: '',
      newPassword: '',
      confirmNewPassword: ''
    });
    setError('');
    setSuccess(false);
    setShowOldPassword(false);
    setShowNewPassword(false);
    setShowConfirmPassword(false);
    onClose();
  };

  return (
    <div className="pwd-modal-overlay" onClick={handleClose}>
      <div className="pwd-modal" onClick={e => e.stopPropagation()}>
        <div className="pwd-modal-header">
          <h2 className="pwd-modal-title">Đổi mật khẩu</h2>
          <button className="pwd-modal-close" onClick={handleClose}>&times;</button>
        </div>

        <div className="pwd-modal-body">
          {success ? (
            <div style={{ textAlign: 'center', padding: '2rem 1rem' }}>
              <svg style={{ width: '4rem', height: '4rem', color: '#10b981', margin: '0 auto 1rem' }} fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <h3 style={{ color: 'var(--text-primary, #111827)', marginBottom: '0.5rem', fontWeight: 600 }}>Thành công!</h3>
              <p style={{ color: 'var(--text-secondary, #6b7280)', margin: 0 }}>Mật khẩu của bạn đã được cập nhật.</p>
            </div>
          ) : (
            <form onSubmit={handleSubmit}>
              {error && <div className="pwd-form-global-error">{error}</div>}
              
              <div className="pwd-form-group">
                <label className="pwd-form-label">Mật khẩu cũ <span style={{ color: 'var(--danger)' }}>*</span></label>
                <div className="pwd-input-wrapper">
                  <input
                    type={showOldPassword ? "text" : "password"}
                    name="oldPassword"
                    value={formData.oldPassword}
                    onChange={handleChange}
                    className="pwd-form-input"
                    placeholder="Nhập mật khẩu hiện tại"
                  />
                  <button type="button" className="pwd-toggle-icon" onClick={() => setShowOldPassword(!showOldPassword)} aria-label="Toggle password visibility">
                    {showOldPassword ? <EyeOffIcon /> : <EyeIcon />}
                  </button>
                </div>
              </div>

              <div className="pwd-form-group">
                <label className="pwd-form-label">Mật khẩu mới <span style={{ color: 'var(--danger)' }}>*</span></label>
                <div className="pwd-input-wrapper">
                  <input
                    type={showNewPassword ? "text" : "password"}
                    name="newPassword"
                    value={formData.newPassword}
                    onChange={handleChange}
                    className="pwd-form-input"
                    placeholder="Tối thiểu 6 ký tự"
                  />
                  <button type="button" className="pwd-toggle-icon" onClick={() => setShowNewPassword(!showNewPassword)} aria-label="Toggle password visibility">
                    {showNewPassword ? <EyeOffIcon /> : <EyeIcon />}
                  </button>
                </div>
              </div>

              <div className="pwd-form-group">
                <label className="pwd-form-label">Xác nhận mật khẩu mới <span style={{ color: 'var(--danger)' }}>*</span></label>
                <div className="pwd-input-wrapper">
                  <input
                    type={showConfirmPassword ? "text" : "password"}
                    name="confirmNewPassword"
                    value={formData.confirmNewPassword}
                    onChange={handleChange}
                    className="pwd-form-input"
                    placeholder="Nhập lại mật khẩu mới"
                  />
                  <button type="button" className="pwd-toggle-icon" onClick={() => setShowConfirmPassword(!showConfirmPassword)} aria-label="Toggle password visibility">
                    {showConfirmPassword ? <EyeOffIcon /> : <EyeIcon />}
                  </button>
                </div>
              </div>
            </form>
          )}
        </div>

        {!success && (
          <div className="pwd-modal-footer">
            <button type="button" className="pwd-btn-cancel" onClick={handleClose} disabled={isSubmitting}>
              Hủy bỏ
            </button>
            <button type="button" className="pwd-btn-submit" onClick={handleSubmit} disabled={isSubmitting}>
              {isSubmitting ? 'Đang xử lý...' : 'Lưu thay đổi'}
            </button>
          </div>
        )}
      </div>
    </div>
  );
};
